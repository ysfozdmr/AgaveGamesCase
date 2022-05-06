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

    [Header("Tags")] string TagObstacle;
    string TagTurningObstacle;
    string TagSwingingObstacle;
    string TagChangeBack;
    private string TagCrouchingArea;
    string TagFinish;

    [Header("Movement Settings")] public float movementSpeed;
    Animator playerAnimCont;
    SplineFollower splineFollower;

    [Header("Camera Settings")] public CinemachineVirtualCamera turningObjectCam;
    public CinemachineVirtualCamera swingingObjectCam;
    public List<GameObject> FailingCubes = new List<GameObject>();
    public CinemachineVirtualCamera finishCam;

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
            splineFollower.followSpeed = 0f;
            playerAnimCont.enabled = false;
            StartCoroutine(FailingCor());
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
            swingingObjectCam.Priority = 9;
            turningObjectCam.Priority = 9;
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
            isCrouchingArea = true;
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
                   // playerAnimCont.SetBool("isRunning", true);
                    playerAnimCont.SetBool("isCrouching", false);
                    splineFollower.enabled = true;
                }
                else
                {
                    playerAnimCont.SetBool("isCrouching", true);
                    splineFollower.enabled = false;
                }
            }
        }
    }
}