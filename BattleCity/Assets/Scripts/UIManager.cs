using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Main user interface manager. This class controlls mostly all of the visual things on players screen.
/// This class does not controlls animations.
/// </summary>
public class UIManager : MonoBehaviour {

    #region Variables.

    [Header("Tanks to destroy")]
    [SerializeField()]
    private GameObject[] uITanks = new GameObject[20]; // 20 tanks are max.
    private static GameObject[] uITanksInGame = new GameObject[20];

    public GameObject flag;
    private static GameObject playersLifesTable;
    public GameObject lifesThingy;
    private static GameObject currentLevelTable;
    private static GameObject currentLevelTable2;
    [SerializeField()]
    private GameObject currentLevelTablePrefab;
    [SerializeField()]
    private GameObject currentLevelTablePrefab2;
    [SerializeField()]
    private GameObject playersLifeTablePrefab; // hope noone gets more than ten lifes in this game....
    [SerializeField()]
    private Sprite[] numbers = new Sprite[10];
    private static Sprite[] numbersStatic = new Sprite[10];

    [SerializeField()]
    private GameObject pauseTXTPrefab;
    [SerializeField()]
    private GameObject gameOverTXTPrefab;

    public static GameObject pauseTXT;
    public static GameObject gameOverTXT;

    /// <summary>
    /// there are 3 different scene types: Menu, Game and Score
    /// </summary>
    public static string sceneType = "Menu";
    #region Main Menu variables.
    public GameObject arrow;
    public Transform buttonStart;
    public Transform buttonExit;
    private int selection;
    #endregion
    #region Score variables.

    public static float scoreTimeStamp; // public for pause
    public static float scoreBeganAt;
    private static float scoreTick;
    private const float shortScoreTick = 0.15f;
    private const float scoreLongTick = 0.5f;
    private const float endScoreTick = 1f;

    /// <summary>
    /// which tank is counting right now
    /// 0 - 1
    /// 1 - 2
    /// 2 - 3
    /// 3 - 4
    /// 5 - level score.
    /// </summary>
    private static int tankScoreNumber = 0;

    /// <summary>
    /// Each score scene will have 12 text fields:
    /// 0 - high score (TXT_HS)
    /// 1 - current score (TXT_CS)
    /// 2 - 1 tank kill score (TXT_1KS)
    /// 3 - 1 tank kill count (TXT_1KC)
    /// 
    /// 4 - 2 tank kill score (TXT_2KS)
    /// 5 - 2 tank kill count (TXT_2KC)
    /// 
    /// 6 - 3 tank kill score (TXT_3KS)
    /// 7 - 3 tank kill count (TXT_3KC)
    /// 
    /// 8 - 4 tank kill score (TXT_4KS)
    /// 9 - 4 tank kill count (TXT_4KC)
    /// 
    /// 10 - level score (TXT_LS)
    /// 11 - which level ended (TXT_LVL)
    /// </summary>
    private static Text[] textFields = new Text[12];

    private static int tanksDestroyed = 0;

    public static bool scoreLevelLoaded = false;
    #endregion
    #endregion

    /// <summary>
    /// Updates number of enemy tank icons.
    /// </summary>
    /// <param name="_tanksLeft">How many enemies are left to spawn</param>
    public static void ShowTanksUI(int _tanksLeft)
    {
        if(_tanksLeft < 20)
        {
            uITanksInGame[_tanksLeft].SetActive(false);
        }
    }

    /// <summary>
    /// Resets tanks which are left for the level.
    /// </summary>
    public void ResetingTanksUI()
    {
        for (int i = 0; i < 20; i++)
        {
            if (sceneType == "Game")
            {
                //Debug.Log("Spawned UI tanks");
                uITanksInGame[i] = Instantiate(uITanks[i]) as GameObject;
            }

        }
        Instantiate(lifesThingy);
        pauseTXT = Instantiate(pauseTXTPrefab);
        gameOverTXT = Instantiate(gameOverTXTPrefab);
        pauseTXT.SetActive(false);
        gameOverTXT.SetActive(false);
        Instantiate(flag);
        playersLifesTable = Instantiate(playersLifeTablePrefab) as GameObject;
        HealthUpdate();
        currentLevelTable = Instantiate(currentLevelTablePrefab) as GameObject;
        if (GameManager.currentLevel > 10)
        {
            currentLevelTable2 = Instantiate(currentLevelTablePrefab2) as GameObject;
            float levelToDisplay = GameManager.currentLevel / 10;
            currentLevelTable.GetComponent<SpriteRenderer>().sprite = numbers[(int)levelToDisplay];
            levelToDisplay = levelToDisplay - (int)levelToDisplay;
            levelToDisplay = levelToDisplay * 10;
            currentLevelTable2.GetComponent<SpriteRenderer>().sprite = numbers[(int)levelToDisplay];
        }
        else
        {
            currentLevelTable.GetComponent<SpriteRenderer>().sprite = numbers[GameManager.currentLevel];
        }
    }

    /// <summary>
    /// UI needs to track what type of scene is playing right now.
    /// </summary>
    /// <param name="_sceneType">Menu, Game or Score</param>
    public void SceneChange(string _sceneType)
    {
        sceneType = _sceneType;
    }
    
    /// <summary>
    /// Main menu buttons controller.
    /// </summary>
    /// <param name="buttonID"></param>
    public void OnButtonPressed(int buttonID)
    {
        switch(buttonID)
        {
            case 0: // exit game button
                Application.Quit();
                break;

            case 1: // start game button
                /*GameObject _openCurtain = Instantiate(openLevelSwitchingCurtain, openLevelSwitchingCurtain.transform.position, Quaternion.identity) as GameObject;
                Animator curtainAnimator = _openCurtain.GetComponent<Animator>();
                //curtainAnimator.SetBool("Play", true);
                //Debug.Break();
                StartCoroutine(ClosingCurtain());
                print("Code still works");
                //isNextLevelReady = false;*/
                //GameManager.StartGame();
                //Delete later
                GameManager.SwitchLevel(true);
                sceneType = "Game";
                break;
        }
    }

    void Start()
    {
        for(int i = 0; i < numbers.Length; i++)
        {
            numbersStatic[i] = numbers[i];
        }
    }

    void Update()
    {
        if (sceneType == "Score")
        {
            if (scoreBeganAt < Time.time - scoreLongTick)
            {
                ShowScore();
            }
        }
        else if(sceneType == "Menu")
        {
            if(Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                MenuPointerBehaviour(0);
            }
            if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                MenuPointerBehaviour(1);
            }
            if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                MenuPointerBehaviour(2);
            }
        }
        else if(sceneType == "Game")
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager.PauseTrigger();
                pauseTXT.SetActive(GameManager.isGamePaused);
            }
        }
    }

    /// <summary>
    /// Main menu pointer actions based on clickID.
    /// </summary>
    /// <param name="clickID">0 - Enter, 1 - Up, 2 - Down.</param>
    private void MenuPointerBehaviour(int clickID)
    {
        switch(clickID)
        {
            case 0:
                if(selection == 0)
                {
                    OnButtonPressed(1);
                }
                if(selection == 1)
                {
                    OnButtonPressed(0);
                }
                break;

            case 1:
                if(selection == 0)
                {
                    selection = 1;
                    arrow.transform.position = new Vector3(arrow.transform.position.x, buttonExit.position.y);
                }
                else if(selection == 1)
                {
                    selection = 0;
                    arrow.transform.position = new Vector3(arrow.transform.position.x, buttonStart.position.y);
                }
                break;

            case 2:
                if(selection == 0)
                {
                    selection = 1;
                    arrow.transform.position = new Vector3(arrow.transform.position.x, buttonExit.position.y);
                }
                else if(selection == 1)
                {
                    selection = 0;
                    arrow.transform.position = new Vector3(arrow.transform.position.x, buttonStart.position.y);
                }
                break;
        }
    }

    /// <summary>
    /// At the end of the level this method shows level score.
    /// </summary>
    public static void ShowScore()
    {
        if(scoreLevelLoaded == false)
        {
            scoreTick = shortScoreTick;
            tankScoreNumber = 0;
            scoreTimeStamp = Time.time;
            textFields[0] = GameObject.Find("TXT_HS").GetComponent<Text>();
            textFields[1] = GameObject.Find("TXT_CS").GetComponent<Text>();
            textFields[2] = GameObject.Find("TXT_1KS").GetComponent<Text>();
            textFields[3] = GameObject.Find("TXT_1KC").GetComponent<Text>();
            textFields[4] = GameObject.Find("TXT_2KS").GetComponent<Text>();
            textFields[5] = GameObject.Find("TXT_2KC").GetComponent<Text>();
            textFields[6] = GameObject.Find("TXT_3KS").GetComponent<Text>();
            textFields[7] = GameObject.Find("TXT_3KC").GetComponent<Text>();
            textFields[8] = GameObject.Find("TXT_4KS").GetComponent<Text>();
            textFields[9] = GameObject.Find("TXT_4KC").GetComponent<Text>();
            textFields[10] = GameObject.Find("TXT_LS").GetComponent<Text>();
            textFields[11] = GameObject.Find("TXT_LVL").GetComponent<Text>();

            textFields[0].text = GameManager.highScore.ToString();
            textFields[1].text = GameManager.score.ToString();
            textFields[11].text = GameManager.currentLevel.ToString();

            scoreLevelLoaded = true;
        }
        else
        {
            if(scoreTick < Time.time - scoreTimeStamp)
            {
                GameManager.mainSoundManager.SRCscoreTick.Play();
                if(tankScoreNumber == 0)
                {
                    int score = tanksDestroyed * 100;
                    textFields[3].text = tanksDestroyed.ToString();
                    textFields[2].text = score.ToString();
                    if (GameManager.killsPerLevel[0] != 0)
                    {
                        scoreTick = shortScoreTick;
                        tanksDestroyed++;
                        GameManager.killsPerLevel[0]--;
                    }
                    else
                    {
                        tanksDestroyed = 0;
                        scoreTick = scoreLongTick;
                        tankScoreNumber++;
                    }
                    scoreTimeStamp = Time.time;
                }
                else if (tankScoreNumber == 1)
                {
                    int score = tanksDestroyed * 200;
                    textFields[5].text = tanksDestroyed.ToString();
                    textFields[4].text = score.ToString();
                    if (GameManager.killsPerLevel[1] != 0)
                    {
                        scoreTick = shortScoreTick;
                        tanksDestroyed++;
                        GameManager.killsPerLevel[1]--;
                    }
                    else
                    {
                        tanksDestroyed = 0;
                        scoreTick = scoreLongTick;
                        tankScoreNumber++;
                    }
                    scoreTimeStamp = Time.time;
                }
                else if (tankScoreNumber == 2)
                {
                    int score = tanksDestroyed * 300;
                    textFields[7].text = tanksDestroyed.ToString();
                    textFields[6].text = score.ToString();
                    if (GameManager.killsPerLevel[2] != 0)
                    {
                        scoreTick = shortScoreTick;
                        tanksDestroyed++;
                        GameManager.killsPerLevel[2]--;
                    }
                    else
                    {
                        scoreTick = scoreLongTick;
                        tanksDestroyed = 0;
                        tankScoreNumber++;
                    }
                    scoreTimeStamp = Time.time;
                }
                else if (tankScoreNumber == 3)
                {
                    int score = tanksDestroyed * 400;
                    textFields[9].text = tanksDestroyed.ToString();
                    textFields[8].text = score.ToString();
                    if (GameManager.killsPerLevel[3] != 0)
                    {
                        scoreTick = shortScoreTick;
                        tanksDestroyed++;
                        GameManager.killsPerLevel[3]--;
                    }
                    else
                    {
                        tanksDestroyed = 0;
                        scoreTick = scoreLongTick;
                        tankScoreNumber++;
                    }
                    scoreTimeStamp = Time.time;
                }
                else if (tankScoreNumber == 4)
                {
                    textFields[10].text = GameManager.levelScore.ToString();
                    scoreTick = endScoreTick;
                    tankScoreNumber++;
                    scoreTimeStamp = Time.time;
                }
                else if (tankScoreNumber == 5)
                {
                    sceneType = "Game";
                    scoreLevelLoaded = false;
                    GameManager.isNextLevelReady = true;
                    tankScoreNumber = 0;
                }
            }
        }

    }

    /// <summary>
    /// Updates number of how many health is left.
    /// </summary>
    public static void HealthUpdate()
    {
        if (GameManager.playerLifesLeft >= 0)
        {
            playersLifesTable.GetComponent<SpriteRenderer>().sprite = numbersStatic[GameManager.playerLifesLeft];
        }
    }

}
