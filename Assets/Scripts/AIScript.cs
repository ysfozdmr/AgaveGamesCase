using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using Random = UnityEngine.Random;

public class AIScript : MonoBehaviour
{
    public State state;

    public enum State
    {
        Run,
        Pause,
        Wait
    }

    [Header("Bools")] public bool isLevelStart;
    public bool isLevelDone;
    public bool isLevelFail;
    public bool objectDone;
    public bool isItPause;
    public bool isCrouching;
    public bool isCrouchingArea;
    private bool waitState;

    [Header("Tags")] string TagObstacle;
    string TagChangeBack;
    string TagFinish;
    string TagCrouchingArea;
    string TagWheel;
    string TagWheelStep;
    string TagWaitState;

    float timer;
    private float random;
    [SerializeField] private int failingCount;


    [Header("Movement Settings")] public float movementSpeed;
    Animator playerAnimCont;
    SplineFollower splineFollower;
    [SerializeField] private GameObject hips;
    [SerializeField] private float speed;

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
        TagWheel = GC.TagWheel;
        TagWheelStep = GC.TagWheelStep;
        TagWaitState = GC.TagWaitState;
        TagFinish = GC.TagFinish;
    }

    void Update()
    {
        Raycast();
        if (waitState)
        {
            transform.position += -Vector3.forward / speed;
        }
    }

    void Raycast()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, 4))
        {
            if (hit.collider.gameObject.CompareTag(TagObstacle))
            {
                Debug.DrawRay(transform.position + Vector3.up * 2, transform.forward);
                if (state != State.Pause)
                {
                    state = State.Pause;
                }
            }
        }

        if (isCrouchingArea)
        {
            if (Physics.Raycast(transform.position + Vector3.up, -transform.forward, out hit, 4f))
            {
                if (hit.collider.gameObject.CompareTag(TagObstacle))
                {
                    Debug.DrawRay(transform.position + Vector3.up * 2, transform.forward);
                    if (state != State.Pause)
                    {
                        state = State.Pause;
                    }
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
        if (!isCrouchingArea)
        {
            playerAnimCont.SetBool("isRunning", true);
        }
        else
        {
            isCrouching = false;
            playerAnimCont.SetBool("isCrouching", false);
        }

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
        if (!isCrouchingArea)
        {
            playerAnimCont.SetBool("isRunning", false);
        }
        else
        {
            isCrouching = true;
            playerAnimCont.SetBool("isCrouching", true);
        }

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
            if (!isCrouching)
            {
                splineFollower.followSpeed = 0f;
                playerAnimCont.enabled = false;
                StartCoroutine(FailingCor());
            }
        }

        if (other.gameObject.CompareTag(TagChangeBack))
        {
            gameObject.transform.SetParent(null);
            GetComponent<Rigidbody>().freezeRotation = false;
            hips.SetActive(true);
            GetComponent<Rigidbody>().isKinematic = true;
            isCrouching = false;
            isCrouchingArea = false;
            FailingCubes.RemoveAt(0);
            RestartPlaces.RemoveAt(0);
        }

        if (other.gameObject.CompareTag(TagFinish))
        {
            StartCoroutine(FinishingCor());

        }

        if (other.gameObject.CompareTag(TagCrouchingArea))
        {
            playerAnimCont.SetTrigger("CrouchTrigger");
            isCrouchingArea = true;
        }

        if (other.gameObject.CompareTag(TagWheel))
        {
            hips.SetActive(false);
            GetComponent<Rigidbody>().isKinematic = false;
            state = State.Wait;
            waitState = true;
            splineFollower.enabled = false;
        }

        if (other.gameObject.CompareTag(TagWheelStep))
        {
            waitState = false;
            GetComponent<Rigidbody>().freezeRotation = true;
            playerAnimCont.SetBool("isRunning", false);
        }

        if (other.gameObject.CompareTag(TagWaitState))
        {
            waitState = true;
            playerAnimCont.SetBool("isRunning", true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag(TagObstacle))
        {
            if (!isCrouching)
            {
                splineFollower.followSpeed = 0f;
                playerAnimCont.enabled = false;
                StartCoroutine(FailingCor());
            }
        }

        if (other.gameObject.CompareTag(TagWheelStep))
        {
            gameObject.transform.SetParent(other.gameObject.transform);
        }
    }

    IEnumerator FinishingCor()
    {
        player.GetComponent<SplineFollower>().enabled = false;
        player.GetComponent<Animator>().SetBool("isRunning",false);
        waitState = false;
        playerAnimCont.SetTrigger("LevelEnd");
        splineFollower.enabled = false;
        yield return new WaitForSeconds(1.5f);
        GC.levelFail();
    }

    IEnumerator FailingCor()
    {
        Debug.Log("sa");
        yield return new WaitForSeconds(1.5f);

        failingCount++;
        splineFollower.followSpeed = movementSpeed;
        playerAnimCont.enabled = true;
        transform.position = FailingCubes[0].gameObject.transform.position;
        splineFollower.Restart(RestartPlaces[0]);
    }
}