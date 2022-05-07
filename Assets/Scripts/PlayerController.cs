using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    [Header("Bools")] public bool isLevelStart;
    public bool isLevelDone;
    public bool isLevelFail;
    public bool isCrouchingArea;
    public bool isCrouching;
    public bool isWheeling;

    [Header("Tags")] string TagObstacle;
    string TagTurningObstacle;
    string TagSwingingObstacle;
    string TagChangeBack;
    string TagCrouchingArea;
    string TagFinish;
    string TagWheel;
    string TagWheelStep;
    private string TagWaitState;

    [Header("Movement Settings")] public float movementSpeed;
    Animator playerAnimCont;
    SplineFollower splineFollower;
    [SerializeField] private GameObject hips;

    [Header("Camera Settings")] public CinemachineVirtualCamera turningObjectCam;
    public CinemachineVirtualCamera swingingObjectCam;
    public List<GameObject> FailingCubes = new List<GameObject>();
    public CinemachineVirtualCamera finishCam;
    public CinemachineVirtualCamera crouchCam;
    public CinemachineVirtualCamera wheelCam;
    public List<CinemachineVirtualCamera> Cams = new List<CinemachineVirtualCamera>();

    public List<float> RestartPlaces = new List<float>();
    private int index;

    GameController GC;

    public static PlayerController instance;

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
        splineFollower = GetComponent<SplineFollower>();
        playerAnimCont = GetComponent<Animator>();
        splineFollower.followSpeed = movementSpeed;

        GetTags();
    }


    void GetTags()
    {
        TagObstacle = GC.TagObstacle;
        TagChangeBack = GC.TagChangeBack;
        TagSwingingObstacle = GC.TagSwingingObstacle;
        TagTurningObstacle = GC.TagTurningObstacle;
        TagCrouchingArea = GC.TagCrouchingArea;
        TagWheel = GC.TagWheel;
        TagWheelStep = GC.TagWheelStep;
        TagWaitState = GC.TagWaitState;
        TagFinish = GC.TagFinish;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Movement();
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

        if (other.gameObject.CompareTag(TagTurningObstacle))
        {
            turningObjectCam.Priority = 11;
        }

        if (other.gameObject.CompareTag(TagSwingingObstacle))
        {
            swingingObjectCam.Priority = 11;
        }

        if (other.gameObject.CompareTag(TagChangeBack))
        {
            gameObject.transform.SetParent(null);
            GetComponent<Rigidbody>().freezeRotation = false;
            hips.SetActive(true);
            GetComponent<Rigidbody>().isKinematic = true;
            isCrouching = false;
            isCrouchingArea = false;
            for (int i = 0; i < Cams.Count; i++)
            {
                Cams[i].Priority = 9;
            }

            FailingCubes.RemoveAt(0);
            RestartPlaces.RemoveAt(0);
        }

        if (other.gameObject.CompareTag(TagFinish))
        {
            StartCoroutine(FinishingCor());
            finishCam.Priority = 11;
            playerAnimCont.SetTrigger("LevelEnd");
            splineFollower.enabled = false;
        }

        if (other.gameObject.CompareTag(TagCrouchingArea))
        {
            crouchCam.Priority = 11;
            playerAnimCont.SetTrigger("CrouchTrigger");
            isCrouchingArea = true;
        }

        if (other.gameObject.CompareTag(TagWheel))
        {
            hips.SetActive(false);
            GetComponent<Rigidbody>().isKinematic = false;
            isWheeling = true;
            wheelCam.Priority = 11;
            splineFollower.enabled = false;
        }

        if (other.gameObject.CompareTag(TagWheelStep))
        {
            GetComponent<Rigidbody>().freezeRotation = true;
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

    IEnumerator FailingCor()
    {
        yield return new WaitForSeconds(1.5f);

        splineFollower.followSpeed = movementSpeed;
        playerAnimCont.enabled = true;
        transform.position = FailingCubes[0].gameObject.transform.position;
        splineFollower.Restart(RestartPlaces[0]);
    }

    IEnumerator FinishingCor()
    {
        yield return new WaitForSeconds(1.5f);
        GC.completeLevel();
    }

    void Movement()
    {
        if (isLevelStart && !isLevelDone && !isLevelFail)
        {
            if (!isCrouchingArea)
            {
                if (!isWheeling)
                {
                    if (Input.GetMouseButton(0))
                    {
                        playerAnimCont.SetBool("isRunning", true);
                        splineFollower.enabled = true;
                    }
                    else
                    {
                        playerAnimCont.SetBool("isRunning", false);
                        splineFollower.enabled = false;
                    }
                }
                else
                {
                    if (Input.GetMouseButton(0))
                    {
                        playerAnimCont.SetBool("isRunning", true);
                        transform.position += -Vector3.forward / 9;
                    }
                    else
                    {
                        playerAnimCont.SetBool("isRunning", false);
                    }
                }
            }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    isCrouching = false;
                    playerAnimCont.SetBool("isCrouching", false);
                    splineFollower.enabled = true;
                }
                else
                {
                    isCrouching = true;
                    playerAnimCont.SetBool("isCrouching", true);
                    splineFollower.enabled = false;
                }
            }
        }
    }
}