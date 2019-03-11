using UnityEngine;
using System.Collections;

/// <summary>
/// Please, use EnemyAIv2.
/// Controlls enemy behaviour in game.
/// </summary>
public class EnemyAI : TankManagement {

    #region Variables
    //public static bool isFrozen;

    protected float mDecChangingTimeRange; // Decision are changed each 'decChangingTimeRange' seconds (movement)
    protected float sDecChangingTimeRange;
    public static float lastMDecisionChangedOn = 0f; // public to make pause working
    public static float lastSDecisionChangedOn = 0f;
    protected float defaultMDecChangingTimeRange;
    protected float defaultSDecChanginTimeeRange;
    protected int movementChangePossibility = 2;
    protected int shotChangePossibility = 2;

    private const int possibilityToChangePath = 4;
    private const int possibilityToFollowPath = 4;

    private Vector2[] path;
    private int pathPoints;
    private float followPathTime = 10f;
    public static float followPathTimeStamp;

    bool followingPath;
    #endregion
    
    /// <summary>
    /// Tries to shot.
    /// </summary>
    protected void ShotDecision()
    {
        if(Time.time - lastSDecisionChangedOn >= sDecChangingTimeRange)
        {
            DecideToShot();
            lastSDecisionChangedOn = Time.time;
        }
    }

    /// <summary>
    /// Making an actuall shot.
    /// </summary>
    private void DecideToShot()
    {
        if(Random.Range(0, shotChangePossibility) == shotChangePossibility - 1)
        {
            ShotingBullet(this.gameObject.name);
        }
    }

    // This is being executed first.
    /// <summary>
    /// 
    /// </summary>
    protected void MoveDecisionChanger()
    {
        if (Time.time - lastMDecisionChangedOn >= mDecChangingTimeRange) // Movement decision
        {
            // update path
            /*if (Time.time - followPathTimeStamp > followPathTime)
            {
                if (Random.Range(0, possibilityToChangePath) == 0)
                {
                    UpdatePath();
                }
                if (followingPath == false)
                {*/
                    MovementDecisionChange();
                    lastMDecisionChangedOn = Time.time;
                /*}
                else
                {
                    FollowPath();
                }
            }*/
        }
    }

    private void MovementDecisionChange()
    {
        if (Random.Range(0, movementChangePossibility) == movementChangePossibility - 1) // 1/movementChangePossibility possibility of changing decision
        {
            int directionId = Random.Range(0, 5);
            DecidingDirection(directionId);
            if (directionId != 4)
            {
                shotDirection = currentMoveDirection;
                mDecChangingTimeRange = defaultMDecChangingTimeRange;            // CHANGE HERE ON CHANGING DEFAULT
            }
        }
    }

    /// <summary>
    /// Deciding random direction.
    /// </summary>
    /// <param name="id">Random direction ID.</param>
    private void DecidingDirection(int id)
    {
        
        Collider2D tanksCol = this.GetComponent<Collider2D>();
        tanksCol.enabled = false;
        if (id == 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(rb.transform.position, Vector2.up, 2);
            if (hit == false)
            {
                currentMoveDirection = "DIR_U";
            }
            else
            {
                id++;
            }

        }

        if (id == 1)
        {
            RaycastHit2D hit = Physics2D.Raycast(rb.transform.position, Vector2.down, 2);
            if (hit == false)
            {
                currentMoveDirection = "DIR_D";
            }
            else
            {
                id++;
            }
        }

        if (id == 2)
        {
            RaycastHit2D hit = Physics2D.Raycast(rb.transform.position, Vector2.left, 2);
            if (hit == false)
            {
                currentMoveDirection = "DIR_L";
            }
            else
            {
                id++;
            }
        }

        if (id == 3)
        {
            RaycastHit2D hit = Physics2D.Raycast(rb.transform.position, Vector2.right, 2);
            if (hit == false)
            {
                currentMoveDirection = "DIR_R";
            }
            else
            {
                id++;
            }
        }
        tankAnimator.SetBool("Tank is moving", true);
        if (id == 4)
        {    //Enemy stops
            currentMoveDirection = null;
            mDecChangingTimeRange = 0.75f; // wont be in one place for so long
            tankAnimator.SetBool("Tank is moving", false);
        }
        tanksCol.enabled = true;
    }

    /// <summary>
    /// Updates path.
    /// </summary>
    protected void UpdatePath()
    {
        if ((tankLevel == 1) || (tankLevel == 11))
        {
            Vector2 newPoint = new Vector2();
            newPoint.y = GameManager.playerInstance.transform.position.y;
            newPoint.x = (GameManager.playerInstance.transform.position.x - 6.5f) * -1;
            path = PathFinding.FindPath(this.transform.position, newPoint);
        }
        else if ((tankLevel == 2) || (tankLevel == 12))
        {
            path = PathFinding.FindPath(this.transform.position, GameManager.playerInstance.transform.position);
        }
        else if ((tankLevel == 3) || (tankLevel == 13))
        {
            path = PathFinding.FindPath(this.transform.position, (new Vector2(6.5f, 0.5f))); // base position
        }
        if(path != null)
        {
            followPathTimeStamp = Time.time;
            DebugVisualHelper.DrawPath(path);
            followingPath = true;
            pathPoints = 0;
            GetDirectionFromVector();
        }
    }

    /// <summary>
    /// Follows path if it is not null
    /// </summary>
    private void FollowPath()
    {
        if(new Vector2(this.transform.position.x, this.transform.position.y) == path[pathPoints])
        {
            if(pathPoints < path.Length)
            {
                pathPoints++;
                GetDirectionFromVector();
            }
        }
    }

    /// <summary>
    /// Gets direction from vector
    /// </summary>
    private void GetDirectionFromVector()
    {
        if(this.transform.position.x == path[pathPoints].x) // if horizontally are equal
        {
            if(this.transform.position.y > path[pathPoints].y) // if player is above point
            {
                currentMoveDirection = "DIR_D";
            }
            else if (this.transform.position.y < path[pathPoints].y) // if player is below point
            {
                currentMoveDirection = "DIR_U";
            }
        }
        else if (this.transform.position.y == path[pathPoints].y) // if horizontally are equal
        {
            if (this.transform.position.x > path[pathPoints].x) // if player is from right side of the point
            {
                currentMoveDirection = "DIR_L";
            }
            else if (this.transform.position.x < path[pathPoints].x) // if player is from left side of the point
            {
                currentMoveDirection = "DIR_R";
            }
        }
        else
        {
            path = null;
            followingPath = false;
        }
    }

}
