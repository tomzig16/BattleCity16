using UnityEngine;
using System.Collections;

/// <summary>
/// Configures enemy settings for further use in AI.
/// Also calls for movement and shoting.
/// </summary>
public class EnemyTankManager : EnemyAIv2
{
    #region Variables
    private float movementSpeed = 0.025f;
    public bool isShotMade { get; set; }
    public int tankHealth { get; set; }
    #endregion

    // Use this for initialization
    /*void Start()
    {
        tankHealth = 1;
        ConfiguringStartup();
        
        // Name syntax - ENEMY_TYPE
        string[] nameValues = this.gameObject.name.Split('_');
        tankLevel = int.Parse(nameValues[1]);
        //tankAnimator.SetInteger("Tank Level", tankLevel);

        isShotMade = false;
        mDecChangingTimeRange = 0.75f;
        sDecChangingTimeRange = 0.75f;
        currentMoveDirection = "DIR_D";
        shotDirection = "DIR_D";
        defaultMDecChangingTimeRange = mDecChangingTimeRange;
        defaultSDecChanginTimeeRange = sDecChangingTimeRange;

        if((tankLevel == 1) || (tankLevel == 11))
        {
            movementSpeed = movementSpeed * 1.5f;
           // UpdatePath();
        }
        else if((tankLevel == 2) || (tankLevel == 12))
        {
            //UpdatePath();
        }
        else if((tankLevel == 3) || (tankLevel == 13))
        {
            movementSpeed = movementSpeed * 0.75f;
            tankHealth = 3;
            //UpdatePath();
        }
        tankAnimator.SetBool("Tank is moving", true);
    }
    */

    void Start()
    {
        tankHealth = 1;
        ConfiguringStartup();

        // Name syntax - ENEMY_TYPE
        string[] nameValues = this.gameObject.name.Split('_');
        tankLevel = int.Parse(nameValues[1]);
        //tankAnimator.SetInteger("Tank Level", tankLevel);

        isShotMade = false;
        currentMoveDirection = "DIR_D";
        shotDirection = "DIR_D";

        if ((tankLevel == 1) || (tankLevel == 11))
        {
            movementSpeed = movementSpeed * 1.5f;
            // UpdatePath();
        }
        else if ((tankLevel == 2) || (tankLevel == 12))
        {
            //UpdatePath();
        }
        else if ((tankLevel == 3) || (tankLevel == 13))
        {
            movementSpeed = movementSpeed * 0.75f;
            tankHealth = 3;
            //UpdatePath();
        }
        tankAnimator.SetBool("Tank is moving", true);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!GameManager.isGamePaused)
        {
            if (!isFrozen)
            {
                //MoveDecisionChanger();
                MovementRequest();
                PerformMovement(movementSpeed);
                ShotRequest();
                //ShotDecision();
            }
        }
    }
    
}