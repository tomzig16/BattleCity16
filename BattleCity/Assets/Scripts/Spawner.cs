using UnityEngine;
using System.Collections;

/// <summary>
/// This class controlls enemy spawning process.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class Spawner : MonoBehaviour {
    #region Variables
    private const float spawnTime = 3f;
    private const float instantiationTime = 2f;
    public static float enemySpawnInstatiationTimeStamp; // public to pause
    public static float enemySpawnTimeStamp; // public to pause
    private string enemyName = "N/A";
    private bool enemyInstatiationIsReady = false;
    private int spawnID;

    [SerializeField()]
    private GameObject[] tanksToSpawn = new GameObject[8]; // exist 8 tanks; 0 - 3 basic and 4 - 7 with picup
    public static int[] tanksForThisLevel = new int[4];

    private int enemiesSpawned = 0;
    private int spawnEnemyOn = 1;
    /// <summary>
    /// [0] - left top
    /// [1] - middle top
    /// [2] - right top
    /// </summary>
    private Vector2[] enemySpawnPos = new Vector2[3]
{
        new Vector2(0.5f, 12.5f),
        new Vector2(6.5f, 12.5f),
        new Vector2(12.5f, 12.5f)
};

    private Animator spawnerAnimator;
    #endregion
    // Use this for initialization
    void Start () {
        spawnerAnimator = this.GetComponent<Animator>();
        GameManager.uiManager.ResetingTanksUI();
    }

    // Update is called once per frame
    void Update () {
        //If enemy is frozen it still can spawn, but it wont move.
        if (!GameManager.isGamePaused)
        {
            if ((GameManager.enemiesLeft > 0) && (GameManager.isGameOver == 0))
            {
                EnemySpawn();
            }
        }
	}

    /// <summary>
    /// Main method that controlls enemy spawning process.
    /// </summary>
    void EnemySpawn()
    {
        if ((Time.time - enemySpawnTimeStamp >= spawnTime) && (enemiesSpawned < 20) && (!enemyInstatiationIsReady))
        {
            if (Physics2D.OverlapArea(
                new Vector2(enemySpawnPos[spawnEnemyOn].x - 0.25f, enemySpawnPos[spawnEnemyOn].y + 0.25f), // position of top left corner
                new Vector2(enemySpawnPos[spawnEnemyOn].x + 0.25f, enemySpawnPos[spawnEnemyOn].y - 0.25f)) // position of right bottom corner
                == true)
            {
                //print("Someone on took my spawn place!");
                return; // If in spawning position already exists something, dont spawn, and wait while that "something" will go away.
            }
            if (enemiesSpawned < 16)
            {
                bool exist = false;
                while (!exist)
                {
                    int tankTypeToSpawn = Random.Range(0, 4);
                    if (tanksForThisLevel[tankTypeToSpawn] > 0)
                    {
                        tanksForThisLevel[tankTypeToSpawn]--;

                        if ((enemiesSpawned + 1 == 4) || (enemiesSpawned + 1 == 11)) // 4, 11 and 18 tanks are with powerup. 18th tank declared ine "else" case 
                            tankTypeToSpawn += 10;
                        //Tank name: ENEMY_TYPE
                        enemyName = (enemiesSpawned + 1) + "ENEMY_" + tankTypeToSpawn;
                        exist = true;
                        spawnID = tankTypeToSpawn;
                    }
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    if (tanksForThisLevel[i] > 0)
                    {
                        int tankTypeToSpawn = i;
                        tanksForThisLevel[i]--;
                        if (enemiesSpawned + 1 == 18)
                        {
                            tankTypeToSpawn = i + 10;
                        }
                        //Tank name: ENEMY_TYPE
                        enemyName = (enemiesSpawned + 1) + "ENEMY_" + tankTypeToSpawn;
                        spawnID = tankTypeToSpawn;
                        break;
                    }
                }
            }
            this.transform.position = enemySpawnPos[spawnEnemyOn++];
            enemySpawnInstatiationTimeStamp = Time.time;
            enemyInstatiationIsReady = true;
            this.GetComponent<SpriteRenderer>().enabled = true;
            if (spawnEnemyOn > 2) spawnEnemyOn = 0;
            enemySpawnTimeStamp = Time.time;
            enemiesSpawned++;
        }
        if(enemyInstatiationIsReady)
        {
            spawnerAnimator.SetBool("Is spawning", true);
            if (Time.time - enemySpawnInstatiationTimeStamp >= instantiationTime)
            {
                print(spawnID + " level tank spawned");
                EnemyInstantiation(spawnID);
            }
        }
    }

    /// <summary>
    /// Method that spawns tank itself.
    /// </summary>
    /// <param name="idToSpawn">Tank ID.</param>
    void EnemyInstantiation(int idToSpawn)
    {
        if(idToSpawn > 3)
        {
            idToSpawn = idToSpawn - 10 + 4;
        }
        GameObject spawnedObject = Instantiate(tanksToSpawn[idToSpawn], this.transform.position, new Quaternion(0,0,1,0)) as GameObject;
        spawnedObject.name = enemyName;
        enemyInstatiationIsReady = false;
        this.GetComponent<SpriteRenderer>().enabled = false;
        spawnerAnimator.SetBool("Is spawning", false);
        UIManager.ShowTanksUI(20 - enemiesSpawned);
    }
}