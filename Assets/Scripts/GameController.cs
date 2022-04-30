using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Header("Bools")]
    public bool isLevelStart;
    public bool isLevelDone;
    public bool isLevelFail;

    [Header("Tags")]
    public string TagObstacle;
    public string TagTurningObstacle;
    public string TagSwingingObstacle;
    public string TagChangeBack;
    public string TagTurningTrigger;

    public int lastLevel;
    private int isFirstLevelFirstlyCompleted;
    int tryNum;
    public int level = 1;
    public int randomLevelIndex;
    
    public static GameController instance;
   // UIController UI;
    PlayerController player;
    private void Awake()
    {
        instance = this;
        lastLevel = PlayerPrefs.GetInt("lastLevel", 1);
        tryNum = PlayerPrefs.GetInt("tryNum", 1);
        isFirstLevelFirstlyCompleted = PlayerPrefs.GetInt("firstlevelCompleted", 0);
    }
    void Start()
    {
        startMethods();
    }
    void startMethods()
    {
     //   UI = UIController.instance;
        player = PlayerController.instance;
    }
    public void tapToStartAction()
    {
        sendLevelStart();
    }
    void sendLevelStart()
    {
        //UI.isLevelStart = true;
        //UI.inGamePanel.SetActive(true);
        isLevelStart = true;
    }
    public void completeLevel()
    {
       // UI.inGamePanel.SetActive(false);
      //  UI.completeLevelPanel.SetActive(true);
        isLevelDone = true;
       // UI.isLevelDone = true;
        if (isFirstLevelFirstlyCompleted == 0)
        {
            isFirstLevelFirstlyCompleted = 1;
            PlayerPrefs.SetInt("firstlevelCompleted", isFirstLevelFirstlyCompleted);
        }
    }
    public void levelFail()
    {
        isLevelFail = true;
        //UI.isLevelFail = true;
    }

    public void EndGameButtonAction()
    {
        lastLevel++;
        PlayerPrefs.SetInt("lastLevel", lastLevel);
        if (isLevelDone)
        {

            if (PlayerPrefs.GetInt("Level") < 3)
            {
                PlayerPrefs.SetInt("Level", (PlayerPrefs.GetInt("Level") + 1));
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

            }

            else
            {
                PlayerPrefs.SetInt("Level", 0);
                SceneManager.LoadScene(PlayerPrefs.GetInt("Level"));
            }
        }


    }


  

    // Update is called once per frame
    void Update()
    {
        
    }
 
}
