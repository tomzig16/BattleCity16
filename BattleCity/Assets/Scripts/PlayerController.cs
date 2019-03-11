using UnityEngine;
using System.Collections;

/// <summary>
/// Player controlls are written here. 
/// </summary>
public class PlayerController : TankManagement {
    #region Variables
    private string owner = "PLAYER";
    public static bool controllsEnabled = true;
    // movement
    private const float movementSpeed = 0.05f;//0.125f; // map step

    public static bool[] areShotsBeingFired = new bool[2] { false, false }; // are shots fired

    public static bool isShieldActtive = true;
    public GameObject playersShield;

    private static float shieldDuration; // after spawn shield duration = 3 sec., on pickup - 10 sec.
    private static float shieldToogleTime;
    #endregion
    // Use this for initialization
    void Start () {
        ConfiguringStartup();
        tankLevel = 0;
        tankAnimator.SetInteger("Tank Level", tankLevel);
        shieldDuration = 3;
        shieldToogleTime = Time.time;
        isShieldActtive = true;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (!GameManager.isGamePaused)
        {
            DebugVisualHelper.DrawRay(this.transform.position);
            //Vector3 lookAt = new Vector3(this.transform.position.x, this.transform.position.y);
            if (controllsEnabled)
            {
                if ((Input.GetKey(KeyCode.UpArrow)) || (Input.GetKey(KeyCode.W))) { currentMoveDirection = "DIR_U"; shotDirection = currentMoveDirection; }
                else if ((Input.GetKey(KeyCode.DownArrow)) || (Input.GetKey(KeyCode.S))) { currentMoveDirection = "DIR_D"; shotDirection = currentMoveDirection; }
                else if ((Input.GetKey(KeyCode.RightArrow)) || (Input.GetKey(KeyCode.D))) { currentMoveDirection = "DIR_R"; shotDirection = currentMoveDirection; }
                else if ((Input.GetKey(KeyCode.LeftArrow)) || (Input.GetKey(KeyCode.A))) { currentMoveDirection = "DIR_L"; shotDirection = currentMoveDirection; }
                else { currentMoveDirection = null; }
            }
            else
            {
                currentMoveDirection = null;
            }
            PerformMovement(movementSpeed);
            if (currentMoveDirection == null)
            {
                tankAnimator.SetBool("Tank is moving", false);
                if (GameManager.mainSoundManager.SRCbackgroundNoise.clip != GameManager.mainSoundManager.SFXbackgroundNoise)
                {
                    if (GameManager.isGameOver != 1)
                    {
                        GameManager.mainSoundManager.SRCbackgroundNoise.Stop();
                        GameManager.mainSoundManager.SRCbackgroundNoise.clip = GameManager.mainSoundManager.SFXbackgroundNoise;
                        GameManager.mainSoundManager.SRCbackgroundNoise.Play();
                    }
                    else
                    {
                        GameManager.mainSoundManager.SRCbackgroundNoise.Stop();
                    }
                }
            }
            else
            {
                //this.transform.LookAt(lookAt);
                //this.transform.l
                if (GameManager.mainSoundManager.SRCbackgroundNoise.clip != GameManager.mainSoundManager.SFXplayerMoving)
                {
                    GameManager.mainSoundManager.SRCbackgroundNoise.Stop();
                    GameManager.mainSoundManager.SRCbackgroundNoise.clip = GameManager.mainSoundManager.SFXplayerMoving;
                    GameManager.mainSoundManager.SRCbackgroundNoise.Play();
                }
                tankAnimator.SetBool("Tank is moving", true);

            }
            // SHOT CONTROLS
            if (((Input.GetKeyDown(KeyCode.Space)) || (Input.GetKeyDown(KeyCode.Keypad0))) && (controllsEnabled))
            {
                if (areShotsBeingFired[0] == false)
                {
                    areShotsBeingFired[0] = true;
                    ShotingBullet(owner);
                    GameManager.mainSoundManager.SRCplayerShot.Play();
                }
                else if (areShotsBeingFired[1] == false)
                {
                    if (tankLevel > 1)
                    {
                        areShotsBeingFired[1] = true;
                        ShotingBullet(owner);
                        GameManager.mainSoundManager.SRCplayerShot.Play();
                    }
                }

            }
            //Check for players levelup
            if (GameManager.playersLevel != tankLevel)
            {
                tankLevel = GameManager.playersLevel;
                tankAnimator.SetInteger("Tank Level", tankLevel);
            }
            if (isShieldActtive)
            {
                if (Time.time - shieldToogleTime >= shieldDuration)
                {
                    ShieldToogle();
                }
            }
            playersShield.SetActive(isShieldActtive);
        }
    }

    /// <summary>
    /// Toggles shield.
    /// </summary>
    public static void ShieldToogle()
    {
        isShieldActtive = !isShieldActtive;
        if(isShieldActtive)
        {
            shieldDuration = 10;
            shieldToogleTime = Time.time;
        }
    }
    
    // OnTriggerEnter2D is here only to check if player has entered pickup trigger.
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "PICKUP")
        {
            GameManager.PickupCollected(col.gameObject, col.gameObject.transform.position);
        }
    }
}
