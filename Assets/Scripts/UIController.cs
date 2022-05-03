using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Bools")]
    public bool isLevelStart;
    public bool isLevelDone;
    public bool isLevelFail;

    [Header("Canvas Controller")]
    public GameObject levelStartPanel;
    public GameObject completeLevelPanel;

    GameController GC;
    PlayerController Player;
    AIScript AI;
    
    public static UIController instance;
    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
    }
    void showTapToStart()
    {
        levelStartPanel.SetActive(true);
    }
    void closeTapToStartPanel()
    {
        levelStartPanel.SetActive(false);
    }
    public void buttonActionTapToStart()
    {
        closeTapToStartPanel();
        GC.tapToStartAction();
        AI.CallingMovement();
    }
    void Start()
    {
        startMethods();
    }

    void startMethods()
    {
        GC = GameController.instance;
        Player = PlayerController.instance;
        AI=AIScript.instance;
        showTapToStart();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
