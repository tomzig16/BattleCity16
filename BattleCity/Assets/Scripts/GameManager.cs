using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Main class (heart of the game).
/// Whole game process is written here.
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Variables
    #region Constants
    private const float pickupLifeDuration = 15f;
    private const float freezeDuration = 10f;
    public const float spawnTime = 3f;
    private const float levelSwitchTime = 3f;
    private const float scoreShowTime = 5f;
    private const float baseProtectionDuration = 20f;
    #endregion
    #region Most of settings (time, player, gamestate etc.)
    //DONT FORGET to write score on a screen
    public static int score = 0;
    public static int levelScore = 0;
    public static int highScore;

    private static float playerSpawnTimeStamp;
    public static float levelSwitchTimeStamp;
    private static float baseProtectionTimeStamp;

    public GameObject playerObject;
    public static GameObject playerInstance;
    private Vector3 playerSpawnPos = new Vector2(4.5f, 0.5f);

    private Animator spawningAnimator;
    public GameObject explosion;
    public static GameObject explosionInstance;
    private static float enemiesFrozeOn;

    public static int playerLifesLeft = 3;
    public static int playersLevel = 0;
    public static int isGameOver = 0; // 0 - not game over, 1 - victory, -1 - lose
    private static bool isPlayerOnAMap;
    public static int currentLevel = 0;
    public static int enemiesLeft = 20;

    public static bool isScoreShown = false;
    public static bool isNextLevelReady = false;
    private static bool isGameRepeating = false;

    public static bool isGamePaused = false;
    public static float howLongPaused;


    /// <summary>
    /// 0 - 1 Tier, 1 - 2 Tier, 2 - 3 Tier, 3 - 4 Tier.
    /// </summary>
    public static int[] killsPerLevel = new int[4];

    #endregion
    #region PickUps
    [Header("Text for points")]
    // These needed to get objects from editor. [SerializeField()] does not work! Later make these static.
    public GameObject _pointOne; // 100 points
    public GameObject _pointTwo; // 200 points
    public GameObject _pointThree; // 300 points
    public GameObject _pointFour; // 400 points
    public GameObject _pointFive; // 500 points

    [Header("Pickups")]
    public GameObject pickupShield;
    public GameObject pickupBaseBlock;
    public GameObject pickupLevelUp;
    public GameObject pickupDestroy;
    public GameObject pickupFreeze;
    public GameObject pickupLife;
    #endregion
    #region Points text
    public static GameObject pointsOne; // 100 points
    public static GameObject pointsTwo; // 200 points
    public static GameObject pointsThree; // 300 points
    public static GameObject pointsFour; // 400 points
    public static GameObject pointsFive; // 500 points

    public static GameObject _pickupShield;
    public static GameObject _pickupBaseBlock;
    public static GameObject _pickupLevelUp;
    public static GameObject _pickupDestroy;
    public static GameObject _pickupFreeze;
    public static GameObject _pickupLife;
    #endregion
    #region Base
    public GameObject _steelWallPrefab;
    public GameObject _brickWallPrefab;

    private static GameObject steelWall;
    private static GameObject brickWall;

    private static bool isBaseProtected = false;
    //////////////////////////////////////////////////////////
    public Sprite steelSprite;
    public Sprite brickSprite; // this sprite used from different sprite set!
    private bool isSteelHiden = false;
    private float hidenOn;

    #endregion

    public static UIManager uiManager;
    public static SoundManager mainSoundManager;

    #endregion
    // Use this for initialization
    void Start()
    {
        this.transform.position = playerSpawnPos;
        spawningAnimator = this.GetComponent<Animator>();
        //this.GetComponent<SpriteRenderer>().enabled = false;
        spawningAnimator.SetBool("Is spawning", false);

        isGameRepeating = false;

        score = 0;
        levelScore = 0;
        DontDestroyOnLoad(this);
        playerSpawnTimeStamp = Time.time;

        SettingUpPickupObjects();
        SettingUpPointObjects();

        uiManager = this.GetComponent<UIManager>();
        mainSoundManager = this.GetComponent<SoundManager>();

        explosionInstance = explosion;

        for(int i = 0; i < 4; i++)
        {
            killsPerLevel[i] = 0;
        }
        print("Game Manager has been loaded successfully.");
        // PlayerSpawn();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGamePaused)
        {
            if (currentLevel != 0) // if in game.
            {
                if ((isPlayerOnAMap == false) && (isGameOver == 0))
                {
                    spawningAnimator.SetBool("Is spawning", true);
                    this.GetComponent<SpriteRenderer>().enabled = true;
                    PlayerSpawn();
                }
                else if ((isPlayerOnAMap == true) && (isGameOver == 1))
                {
                    //mainSoundManager.SRCbackgroundNoise.Stop();
                    if (EndGameTimer())
                    {
                        SwitchLevel(true);
                    }
                }
                else if (playerLifesLeft < 0 || isGameOver == -1)
                {
                    //UIManager.gameOverTXT.SetActive(true);
                    if (/*Application.loadedLevel*/ SceneManager.GetActiveScene().buildIndex != 36)
                    {
                        UIManager.gameOverTXT.SetActive(true);
                    }
                    if (EndGameTimer()) // starting timer to show "GameOver" text.
                    {
                        SwitchLevel(false);
                        if(!isNextLevelReady && !isScoreShown)
                        {
                            UIManager.sceneType = "Menu";
                            Destroy(this.gameObject);
                        }
                    }
                }

                if (EnemyAIv2.isFrozen)
                {
                    if (Time.time - enemiesFrozeOn >= freezeDuration)
                    {
                        EnemyAIv2.isFrozen = false;
                    }
                }

                if (isBaseProtected)
                {
                    BaseBeingProtected();
                }
            }
            #region Keys for testings. (COMMENTED)
            /*if (Input.GetKeyDown(KeyCode.KeypadMultiply))
            {
                SpawnPickup();
            }
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                print(playersLevel + " current players level");
            }
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                playersLevel++;
            }
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                DebugVisualHelper.DrawPath(PathFinding.FindPath(playerInstance.transform.position, new Vector2(6.5f, 12.5f)));
            }*/
            #endregion
        }
    }

    /// <summary>
    /// Switches between levels.
    /// </summary>
    /// <param name="_isItVictory">Is it victory?</param>
    public static void SwitchLevel(bool _isItVictory)
    {
        if(!isScoreShown && currentLevel != 0)
        {
            score += levelScore;
            //Application.LoadLevel("Score");
            SceneManager.LoadScene("Score");
            isNextLevelReady = false;
            isScoreShown = true;
            UIManager.scoreBeganAt = Time.time;
            UIManager.sceneType = "Score";
            mainSoundManager.SRCbackgroundNoise.Stop();
        }
        if (isNextLevelReady || currentLevel == 0)
        {
            if (_isItVictory)
            {
                if(currentLevel == 35)
                {
                    currentLevel = 0;
                    isGameRepeating = true;
                }
                //Application.LoadLevel("Stage_" + ++currentLevel);
                SceneManager.LoadScene("Stage_" + ++currentLevel);
                if (!isGameRepeating)
                {
                    Spawner.tanksForThisLevel = EnemiesInfo.GetWhichTanksToSpawn(currentLevel);
                }
                else
                {
                    Spawner.tanksForThisLevel = EnemiesInfo.GetWhichTanksToSpawn(35); // last levels tank set.
                }
                isPlayerOnAMap = false;
                playerSpawnTimeStamp = Time.time;
                EnemyAIv2.isFrozen = false;
                uiManager.ResetingTanksUI();
                isGameOver = 0;
                PlayerController.areShotsBeingFired[0] = false;
                PlayerController.areShotsBeingFired[1] = false;
                enemiesLeft = 20;
                SoundManager.isBackgroundNoiseActive = true;
                //            mainSoundManager.SRCbackgroundNoise.Stop();
                isBaseProtected = false;
                if (currentLevel - 1 > 0)
                {
                    mainSoundManager.SRCbackgroundNoise.Play();
                }
            }
            else
            {
                //Application.LoadLevel(0);
                SceneManager.LoadScene(0);
                currentLevel = 0;
                if (highScore < score)
                {
                    highScore = score;
                }
                isGameOver = 0;
                playerLifesLeft = 3;
                playersLevel = 0;
                PlayerController.controllsEnabled = true;
            }
            isNextLevelReady = false;
            isScoreShown = false;
            levelScore = 0;
        }
    }

    /// <summary>
    /// Starts actuall game.
    /// </summary>
    public static void Startgame()
    {
        //Application.LoadLevel("Stage_1");
        SceneManager.LoadScene("Stage_1");
        Spawner.tanksForThisLevel = EnemiesInfo.GetWhichTanksToSpawn(1);
        // If its second time player plays, these settings are to avoid bugs.
        playerSpawnTimeStamp = Time.time;
        EnemyAIv2.isFrozen = false;
        currentLevel++;
        uiManager.ResetingTanksUI();
        enemiesLeft = 20;
        score = 0;
        levelScore = 0;
        playerLifesLeft = 3;
    }

    /// <summary>
    /// If player got killed.
    /// </summary>
    public static void PlayerGotKilled()
    {
        playerSpawnTimeStamp = Time.time;
        GameObject explosionInst = Instantiate(explosionInstance, playerInstance.transform.position, Quaternion.identity) as GameObject;
        Animator animator = explosionInst.GetComponent<Animator>();
        animator.SetBool("Is explosion big", true);
        Destroy(explosionInst, 0.35f);
        Destroy(playerInstance.gameObject);
        isPlayerOnAMap = false;
        playerLifesLeft--;
        print(playerLifesLeft + "Lifes left");
        //Debug.Break();
        playersLevel = 0;
        mainSoundManager.ExplosionSound();
        mainSoundManager.SRCbackgroundNoise.Stop();
        mainSoundManager.SRCbackgroundNoise.clip = mainSoundManager.SFXbackgroundNoise;
        mainSoundManager.SRCbackgroundNoise.Play();
        if (playerLifesLeft < 0 && isGameOver == 0)
        {
            isGameOver = -1;
            levelSwitchTimeStamp = Time.time;
            mainSoundManager.SRCbackgroundNoise.Stop();
        }
        UIManager.HealthUpdate();
    }

    /// <summary>
    /// If enemy got hitten (method checks if destroy or not).
    /// </summary>
    /// <param name="enemyTank">Enemy tank which got hitten.</param>
    /// <param name="enemyHealthLeft">How much health that enemy has got.</param>
    public static void EnemyGotHitten(GameObject enemyTank, int enemyHealthLeft)
    {
        Vector2 _tankPosition = enemyTank.transform.position;
        int _enemyType = int.Parse(enemyTank.name.Split('_')[1]);
        if ((_enemyType == 0) || (_enemyType == 10))
        {
            levelScore += 100;
            Destroy(Instantiate(pointsOne, _tankPosition, Quaternion.identity), 3); // Destroys object after 3 seconds
            GameObject explosionInst = Instantiate(explosionInstance, enemyTank.transform.position, Quaternion.identity) as GameObject;
            Animator animator = explosionInst.GetComponent<Animator>();
            animator.SetBool("Is explosion big", true);
            Destroy(explosionInst, 0.35f); // change time
            mainSoundManager.ExplosionSound();
            killsPerLevel[0]++;
        }
        else if ((_enemyType == 1) || (_enemyType == 11))
        {
            levelScore += 200;
            Destroy(Instantiate(pointsTwo, _tankPosition, Quaternion.identity), 3); // Destroys object after 3 seconds
            GameObject explosionInst = Instantiate(explosionInstance, enemyTank.transform.position, Quaternion.identity) as GameObject;
            Animator animator = explosionInst.GetComponent<Animator>();
            animator.SetBool("Is explosion big", true);
            Destroy(explosionInst, 0.35f);
            mainSoundManager.ExplosionSound();
            killsPerLevel[1]++;
        }
        else if ((_enemyType == 2) || (_enemyType == 12))
        {
            levelScore += 300;
            Destroy(Instantiate(pointsThree, _tankPosition, Quaternion.identity), 3); // Destroys object after 3 seconds
            GameObject explosionInst = Instantiate(explosionInstance, enemyTank.transform.position, Quaternion.identity) as GameObject;
            Animator animator = explosionInst.GetComponent<Animator>();
            animator.SetBool("Is explosion big", true);
            Destroy(explosionInst, 0.35f);
            mainSoundManager.ExplosionSound();
            killsPerLevel[2]++;
        }
        else if ((_enemyType == 3) || (_enemyType == 13))
        {
            if(enemyHealthLeft == 0)
            {
                levelScore += 400;
                Destroy(Instantiate(pointsFour, _tankPosition, Quaternion.identity), 3); // Destroys object after 3 seconds
                GameObject explosionInst = Instantiate(explosionInstance, enemyTank.transform.position, Quaternion.identity) as GameObject;
                Animator animator = explosionInst.GetComponent<Animator>();
                animator.SetBool("Is explosion big", true);
                Destroy(explosionInst, 0.35f);
                mainSoundManager.ExplosionSound();
                killsPerLevel[3]++;
            }
            else
            {
//                enemyTank.GetComponent<EnemyTankManager>().tankAnimator.SetInteger("Tank Level", 3);
                Color[] enemyColors = { Color.yellow, Color.green};
                enemyTank.GetComponent<SpriteRenderer>().color = enemyColors[enemyHealthLeft - 1];
                enemyTank.GetComponent<Animator>().SetBool("Is tank with pick", false);
                mainSoundManager.SRCexplosion1.Stop();
                mainSoundManager.SRCexplosion1.clip = mainSoundManager.SFXhitsArmor;
                mainSoundManager.SRCexplosion1.Play();
            }
        }
        // Tanks with powerups.
        if (_enemyType >= 10)
        {
            SpawnPickup();
            if(_enemyType == 13)
            {
                string newName = enemyTank.name.Split('_')[0] + "_3";
                enemyTank.name = newName;
            }
        }
        if (enemyHealthLeft == 0)
        {
            Destroy(enemyTank.gameObject);
            enemiesLeft--;
            if (enemiesLeft == 0 && isGameOver != 1)
            {
                isGameOver = 1;
                levelSwitchTimeStamp = Time.time;
            }
        }
        //print("Score: " + score);
    }

    /// <summary>
    /// Spawns player.
    /// </summary>
    void PlayerSpawn()
    {
        if (Time.time - playerSpawnTimeStamp >= spawnTime)
        {
            isPlayerOnAMap = true;
            playerInstance = Instantiate(playerObject) as GameObject;//, playerSpawnPos, Quaternion.identity) as GameObject;
            playerInstance.transform.position = this.transform.position;
            //SendMessage("SpawnPlayer", SendMessageOptions.RequireReceiver);
            spawningAnimator.SetBool("Is spawning", false);
            
        }
    }
    
    /// <summary>
    /// Spawns pickup at random position on the whole map.
    /// </summary>
    static void SpawnPickup()
    {
        int pickupType = Random.Range(0, 6);
        Vector2 spawnPosition = new Vector2(Random.Range(0.5f, 12.5f), Random.Range(0.5f, 12.5f));
        mainSoundManager.SRCpickup.clip = mainSoundManager.SFXpickupSpawned;
        mainSoundManager.SRCpickup.Play();
        GameObject lastPickUp;
        // pickup alive 15 seconds
        switch (pickupType)
        {
            case 0:
                lastPickUp = Instantiate(_pickupLife, spawnPosition, Quaternion.identity) as GameObject;
                lastPickUp.name = "PICK_LIFE";
                Destroy(lastPickUp, pickupLifeDuration);
                break;
            case 1:
                lastPickUp = Instantiate(_pickupLevelUp, spawnPosition, Quaternion.identity) as GameObject;
                lastPickUp.name = "PICK_LEVELUP";
                Destroy(lastPickUp, pickupLifeDuration);
                break;
            case 2:
                lastPickUp = Instantiate(_pickupDestroy, spawnPosition, Quaternion.identity) as GameObject;
                lastPickUp.name = "PICK_DESTROY";
                Destroy(lastPickUp, pickupLifeDuration);
                break;
            case 3:
                lastPickUp = Instantiate(_pickupFreeze, spawnPosition, Quaternion.identity) as GameObject;
                lastPickUp.name = "PICK_FREEZE";
                Destroy(lastPickUp, pickupLifeDuration);
                break;
            case 4:
                lastPickUp = Instantiate(_pickupBaseBlock, spawnPosition, Quaternion.identity) as GameObject;
                lastPickUp.name = "PICK_BASEBLOCK";
                Destroy(lastPickUp, pickupLifeDuration);
                break;
            case 5:
                lastPickUp = Instantiate(_pickupShield, spawnPosition, Quaternion.identity) as GameObject;
                lastPickUp.name = "PICK_SHIELD";
                Destroy(lastPickUp, pickupLifeDuration);
                break;
        }
    }

    /// <summary>
    /// Executed when player collects pickup.
    /// </summary>
    /// <param name="_pickup">Which pickup was collected.</param>
    /// <param name="_position">Pickups position.</param>
    public static void PickupCollected(GameObject _pickup, Vector2 _position)
    {
        mainSoundManager.SRCpickup.clip = mainSoundManager.SFXpickupTaken;
        mainSoundManager.SRCpickup.Play();
        levelScore += 500;
        Destroy(Instantiate(pointsFive, _position, Quaternion.identity), 3);
        switch (_pickup.name)
        {
            case "PICK_SHIELD":
                //print("Shield Picked up");
                PlayerController.ShieldToogle();
                break;
            case "PICK_BASEBLOCK":
                //Make base blocked
                GameObject brickFolder = GameObject.Find("Bricks folder");
                GameObject steelFolder = GameObject.Find("Steel folder");
                Destroy(brickFolder);
                Destroy(steelFolder);
                brickWall = Instantiate(brickWall, Vector3.zero, Quaternion.identity) as GameObject;
                steelWall = Instantiate(steelWall, Vector3.zero, Quaternion.identity) as GameObject;
                brickWall.name = "Bricks folder";
                steelWall.name = "Steel folder";
                isBaseProtected = true;
                baseProtectionTimeStamp = Time.time;
                steelWall.SetActive(true);
                brickWall.SetActive(false);
                break;
            case "PICK_FREEZE":
                EnemyAIv2.isFrozen = true;
                enemiesFrozeOn = Time.time;
                break;
            case "PICK_DESTROY":
                GameObject[] enemiesInScene = GameObject.FindGameObjectsWithTag("ENEMY");
                foreach(GameObject enemy in enemiesInScene)
                {
                    GameObject explosionInst = Instantiate(explosionInstance, enemy.transform.position, Quaternion.identity) as GameObject;
                    Animator animator = explosionInst.GetComponent<Animator>();
                    animator.SetBool("Is explosion big", true);
                    Destroy(explosionInst, 0.35f); 
                    Destroy(enemy.gameObject);
                    enemiesLeft--;
                    // In original game system does not give points for each tank destroyed, so we wont too.
                    mainSoundManager.ExplosionSound();
                }
                if (enemiesLeft == 0)
                {
                    isGameOver = 1;
                    levelSwitchTimeStamp = Time.time;
                }
                break;
            case "PICK_LIFE":
                playerLifesLeft++;
                mainSoundManager.SRCpickup.clip = mainSoundManager.SFXadditionalHP;
                mainSoundManager.SRCpickup.Play();
                UIManager.HealthUpdate();
                break;
            case "PICK_LEVELUP":
                if (playersLevel < 3)
                {
                    playersLevel++;
                }
                break;
        }
        Destroy(_pickup);

    }

    /// <summary>
    /// End game timer, which was used to keep player in scene when game actually ends.
    /// </summary>
    /// <returns>"Is scene can already switch?" 0/1</returns>
    private bool EndGameTimer()
    {
        if(levelSwitchTime < Time.time - levelSwitchTimeStamp)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Points have to be static objects, so this method "converts" from nonstatic into static.
    /// </summary>
    private void SettingUpPointObjects()
    {
        pointsOne = _pointOne;
        pointsTwo = _pointTwo;
        pointsThree = _pointThree;
        pointsFour = _pointFour;
        pointsFive = _pointFive;
    }

    /// <summary>
    /// Pickups have to be static objects, so this method "converts" from nonstatic into static.
    /// </summary>
    private void SettingUpPickupObjects() // Base here too.
    {
        _pickupBaseBlock = pickupBaseBlock;
        _pickupDestroy = pickupDestroy;
        _pickupFreeze = pickupFreeze;
        _pickupLevelUp = pickupLevelUp;
        _pickupLife = pickupLife;
        _pickupShield = pickupShield;

        brickWall = _brickWallPrefab;
        steelWall = _steelWallPrefab;
    }

    /// <summary>
    /// Spawns explosion at certain position.
    /// </summary>
    /// <param name="_position">Position on which explosion should spawn.</param>
    /// <param name="isExplosionBig">Big explosion are used for tanks and base.</param>
    public static void SpawnExplosionAt(Vector2 _position, bool isExplosionBig)
    {
        GameObject explosionInst = Instantiate(explosionInstance, _position, Quaternion.identity) as GameObject;
        Animator animator = explosionInst.GetComponent<Animator>();
        animator.SetBool("Is explosion big", isExplosionBig);
        Debug.Break();
    }

    /// <summary>
    /// If base is being protected by steel walls.
    /// </summary>
    void BaseBeingProtected()
    {
        if ((currentLevel != 36) && (currentLevel != 0))
        {
            if (baseProtectionDuration - 3 <= Time.time - baseProtectionTimeStamp)
            {
                float hideDuration = 0.25f;
                if (!isSteelHiden)
                {
                    if (Time.time - hidenOn >= hideDuration)
                    {
                        SpriteRenderer[] sRend = steelWall.GetComponentsInChildren<SpriteRenderer>();
                        foreach (SpriteRenderer singleSRend in sRend)
                        {
                            singleSRend.sprite = brickSprite;
                        }
                        hidenOn = Time.time;
                        isSteelHiden = true;
                    }
                }
                else
                {
                    if (Time.time - hidenOn >= hideDuration)
                    {
                        SpriteRenderer[] sRend = steelWall.GetComponentsInChildren<SpriteRenderer>();
                        foreach (SpriteRenderer singleSRend in sRend)
                        {
                            singleSRend.sprite = steelSprite;
                        }
                        hidenOn = Time.time;
                        isSteelHiden = false;
                    }
                }
                if (baseProtectionDuration <= Time.time - baseProtectionTimeStamp)
                {
                    steelWall.SetActive(false);
                    brickWall.SetActive(true);
                    isBaseProtected = false;
                }
            }
        }
    }

    /// <summary>
    /// Pauses whole game. 
    /// </summary>
    public static void PauseTrigger()
    {
        if(isGamePaused)
        {

            //UIManager.pauseTXT.SetActive(true);
            isGamePaused = false;
            howLongPaused = howLongPaused - Time.time;
            UIManager.scoreTimeStamp += howLongPaused;
            Spawner.enemySpawnInstatiationTimeStamp += howLongPaused;
            Spawner.enemySpawnTimeStamp += howLongPaused;
            baseProtectionTimeStamp += howLongPaused;
            levelSwitchTimeStamp += howLongPaused;
            playerSpawnTimeStamp += howLongPaused;
            EnemyAI.lastMDecisionChangedOn += howLongPaused;
            EnemyAI.lastSDecisionChangedOn += howLongPaused;
            enemiesFrozeOn += howLongPaused;
        }
        else
        {
            isGamePaused = true;
            howLongPaused = Time.time;
        }
    }
}
