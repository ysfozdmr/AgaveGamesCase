using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using UnityEditor.VersionControl;
using Random = UnityEngine.Random;

public class AIScript : MonoBehaviour
{
    public State state;

    public enum State
    {
        Run,
        Pause
    }

    [Header("Bools")] public bool isLevelStart;
    public bool isLevelDone;
    public bool isLevelFail;
    public bool objectDone;
    public bool isItPause;

    [Header("Tags")] string TagObstacle;
    string TagChangeBack;
    string TagFinish;
    string TagCrouchingArea;

    float timer;
    float timer2;
    private float random;
    [SerializeField] private int failingCount;

    [Header("Movement Settings")] public float movementSpeed;
    Animator playerAnimCont;
    SplineFollower splineFollower;

    public List<float> RestartPlaces = new List<float>();
    public List<GameObject> FailingCubes = new List<GameObject>();

    GameController GC;
    PlayerController player;

    public static AIScript instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        StartMethods();
    }

    void StartMethods()
    {
        GC = GameController.instance;
        player = PlayerController.instance;
        splineFollower = GetComponent<SplineFollower>();
        playerAnimCont = GetComponent<Animator>();
        splineFollower.followSpeed = movementSpeed;

        GetTags();
    }


    void GetTags()
    {
        TagObstacle = GC.TagObstacle;
        TagChangeBack = GC.TagChangeBack;
        TagCrouchingArea = GC.TagCrouchingArea;
        TagFinish = GC.TagFinish;
    }

    void FixedUpdate()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, 4))
        {
            if (hit.collider.gameObject.CompareTag(TagObstacle))
            {
                Debug.DrawRay(transform.position + Vector3.up, transform.forward);
                if (state != State.Pause)
                {
                    state = State.Pause;
                }
            }
        }
    }

    public void CallingMovement()
    {
        StartCoroutine(Movement());
    }

    IEnumerator Movement()
    {
        playerAnimCont.SetBool("isRunning", true);
        while (state == State.Run)
        {
            splineFollower.enabled = true;
            yield return new WaitForEndOfFrame();
        }

        if (state == State.Pause)
        {
            StartCoroutine((Pause()));
        }
    }

    IEnumerator Pause()
    {
        isItPause = true;
        playerAnimCont.SetBool("isRunning", false);
        random = Random.Range(0.5f, 0.7f);
        while (state == State.Pause)
        {
            splineFollower.enabled = false;

            if (timer > random)
            {
                timer = 0;
                state = State.Run;
                break;
            }


            timer += Time.deltaTime;


            yield return new WaitForEndOfFrame();
        }

        if (state == State.Run)
        {
            StartCoroutine(Movement());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(TagObstacle))
        {
            splineFollower.followSpeed = 0f;
            playerAnimCont.enabled = false;
            StartCoroutine(FailingCor());
        }

        if (other.gameObject.CompareTag(TagChangeBack))
        {
            failingCount = 0;
            objectDone = true;
            FailingCubes.RemoveAt(0);
            RestartPlaces.RemoveAt(0);
        }

        if (other.gameObject.CompareTag(TagFinish))
        {
            playerAnimCont.SetTrigger("LevelEnd");
            splineFollower.enabled = false;
        }
    }

    IEnumerator FailingCor()
    {
        yield return new WaitForSeconds(1.5f);

        failingCount++;
        splineFollower.followSpeed = movementSpeed;
        playerAnimCont.enabled = true;
        transform.position = FailingCubes[0].gameObject.transform.position;
        splineFollower.Restart(RestartPlaces[0]);
    }
}