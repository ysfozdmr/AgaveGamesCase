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
    [Header("Tags")] string TagObstacle;
    string TagTurningObstacle;
    string TagSwingingObstacle;
    string TagChangeBack;
    string TagTurningTrigger;

    [Header("Movement Settings")] public float movementSpeed;
    Animator playerAnimCont;
    SplineFollower splineFollower;

    [Header("Camera Settings")] public CinemachineVirtualCamera turningObjectCam;
    public CinemachineVirtualCamera swingingObjectCam;
    public List<GameObject> FailingCubes = new List<GameObject>();
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
        TagTurningTrigger = GC.TagTurningTrigger;
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
            index++;
        }
    }

    public void splinechanger()
    {
        splineFollower.Restart(0.19f);
    }
    IEnumerator FailingCor()
    {
        yield return new WaitForSeconds(1.5f);
        if (index == 0)
        {
            playerAnimCont.enabled = true;
            transform.position = FailingCubes[0].gameObject.transform.position;
            splineFollower.Restart(0.19f);
        }
        else if (index == 1)
        {
            playerAnimCont.enabled = true;
            transform.position = FailingCubes[1].gameObject.transform.position;
        }
    }
    
    void Movement()
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
}