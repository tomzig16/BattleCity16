using UnityEngine;
using System.Collections;

/// <summary>
/// Second version of AI for enemy.
/// This version contains more complex behaviour (most of it is finding a path and following it).
/// </summary>
public class EnemyAIv2 : TankManagement
{
    #region Variables
    private Vector2[] path;
    private int currentPathPoint;
    private float checkPathAfter = 10f;
    private float lastPathCheck = 0f;
    private const int followPathChance = 2; // chance is 1 / followPathChance.
    //private bool followPath;
    protected string directionToPathPoint;

    private float randomMovementTimeStamp; // i wont affect this time by pause.
    private float randomMovementDuration = 1f;

    public static bool isFrozen;

    private float shootingTimeStamp = 3f; // i wont affect this time by pause.
    private const int shotChance = 75; // chance is 1 / shotChance.
    private const float shotCheckFrequency = 0.75f; //check for shoot each 'checkFrequency' seconds.

    private Vector2[] randomTravelDestinations = new Vector2[]
    {
        new Vector2(0.5f, 0.5f), // left bottom corner
        new Vector2(6.5f, 2.5f), // few tiles above player base
        new Vector2(12.5f, 0.5f), // right bottom corner
        new Vector2(6.5f, 6.5f), // center of a map
        new Vector2(0.5f, 6.5f), // left side of a map
        new Vector2(12.5f, 6.5f) // right side of a map
        //new Vector2(6.5f, 0.5f) // players base
    };
    #endregion

    /// <summary>
    /// Sets path to specific point (depends on enemy 'level') or makes tank move randomly.
    /// </summary>
    protected void MovementRequest()
    {       
        if(Time.time - lastPathCheck > checkPathAfter)
        {
            if (Random.Range(0, followPathChance) == 0)
            {
                UpdatePath();
            }
            else
            {
                path = null;
            }
        }
        DoMovement();
        DebugVisualHelper.DrawRayForMovement(currentMoveDirection, this.transform.position);
    }

    /// <summary>
    /// Updates path (if not found, path is null).
    /// </summary>
    void UpdatePath()
    {
        lastPathCheck = Time.time;
        if ((tankLevel == 0) || (tankLevel == 10))
        {
            path = PathFinding.FindPath(this.transform.position, randomTravelDestinations[Random.Range(0, randomTravelDestinations.Length)]);
        }
        else if ((tankLevel == 1) || (tankLevel == 11))
        {
            if (GameManager.playerInstance.gameObject != null)
            {
                Vector2 target = new Vector2();
                if (GameManager.playerInstance.transform.position.x < 6.5f)
                {
                    target.x = GameManager.playerInstance.transform.position.x + 6.5f;
                    target.y = GameManager.playerInstance.transform.position.y;
                }
                else
                {
                    target.x = 6.5f - (GameManager.playerInstance.transform.position.x - 6.5f);
                    target.y = GameManager.playerInstance.transform.position.y;
                }
                path = PathFinding.FindPath(this.transform.position, target);
            }
            else { path = null; }
        }
        else if ((tankLevel == 2) || (tankLevel == 12))
        {
            path = PathFinding.FindPath(this.transform.position, GameManager.playerInstance.transform.position);
        }
        else if ((tankLevel == 3) || (tankLevel == 13))
        {
            path = PathFinding.FindPath(this.transform.position, new Vector2(6.5f, 0.5f));
        }

        if (path != null)
        {
            currentPathPoint = 1;
            directionToPathPoint = GetDirectionFromVector(path[1]);
            currentMoveDirection = directionToPathPoint;
            DebugVisualHelper.DrawPath(path);
            checkPathAfter = 10f;
        }
        else
        {
            print("path is null.");
            checkPathAfter = 3f;
            randomMovementTimeStamp = Time.time;
        }
    }

    /// <summary>
    /// Sets movement direction from tanks position to next point.
    /// </summary>
    /// <param name="nextPos">Position to which tank will drive.</param>
    /// <returns></returns>
    string GetDirectionFromVector(Vector2 nextPos)
    {
        string direction = null;
        Vector2 thisPosition = new Vector2();
        thisPosition = PathFinding.SnappingToGrid(this.transform.position);
        if (thisPosition.x > nextPos.x)
        {
            direction = "DIR_L";
        }
        else if (thisPosition.x < nextPos.x)
        {
            direction = "DIR_R";
        }
        else if (thisPosition.y > nextPos.y)
        {
            direction = "DIR_D";
        }
        else if (thisPosition.y < nextPos.y)
        {
            direction = "DIR_U";
        }
        else
        {
            RandomMovement(Random.Range(0, 5));
        }
        return direction;
    }

    /// <summary>
    /// If path is not null, follows it; else moves randomly
    /// </summary>
    private void DoMovement()
    {
        if(path == null)
        {
            // make random  movement.
            if (Time.time - randomMovementTimeStamp > randomMovementDuration)
            {
                RandomMovement(Random.Range(0, 5));
            }
        }
        else
        {
            //currentMoveDirection
            FollowPath();
        }

        if(path != null && currentMoveDirection == null)
        {
 //           Debug.LogErrorFormat("Next path point is {0} ( {1} ). Caller: {2}", currentPathPoint, path[currentPathPoint], this, gameObject.name);
            FollowPath();
        }
    }

    /// <summary>
    /// Follows path.
    /// </summary>
    private void FollowPath()
    {
        if (directionToPathPoint == "DIR_L")
        {
            if (path[currentPathPoint + 1].x > this.transform.position.x - 0.075f)
            {
                directionToPathPoint = GetDirectionFromVector(path[++currentPathPoint]);
            }
            currentMoveDirection = directionToPathPoint;
        }
        else if (directionToPathPoint == "DIR_D")
        {
            if (path[currentPathPoint + 1].y > this.transform.position.y - 0.075f)
            {
                directionToPathPoint = GetDirectionFromVector(path[++currentPathPoint]);
            }
            currentMoveDirection = directionToPathPoint;
        }
        else if (directionToPathPoint == "DIR_R")
        {
            if (path[currentPathPoint + 1].x < this.transform.position.x + 0.075f)
            {
                directionToPathPoint = GetDirectionFromVector(path[++currentPathPoint]);
            }
            currentMoveDirection = directionToPathPoint;
        }
        else if (directionToPathPoint == "DIR_U")
        {
            if (path[currentPathPoint + 1].y < this.transform.position.y + 0.075f)
            {
                directionToPathPoint = GetDirectionFromVector(path[++currentPathPoint]);
            }
            currentMoveDirection = directionToPathPoint;
        }

        if (currentMoveDirection == null)
        {
            currentPathPoint++;
            if (currentPathPoint < path.Length)
            {
                currentMoveDirection = GetDirectionFromVector(path[currentPathPoint]);
            }
            else
            {
                path = null;
                RandomMovement(0);
            }
        }
        else if(currentPathPoint + 1 == path.Length)
        {
            path = null;
            checkPathAfter = 20f;
        }
    }    

    /// <summary>
    /// Called when path is null.
    /// Makes tank move randomly.
    /// </summary>
    /// <param name="id">Movement ID (mostly its just a random number from 0 to 4)</param>
    private void RandomMovement(int id)
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
            tankAnimator.SetBool("Tank is moving", false);
            randomMovementDuration = 0.5f;
        }
        else
        {
            randomMovementDuration = 1f;
        }
        randomMovementTimeStamp = Time.time;
        tanksCol.enabled = true;
    }

    /// <summary>
    /// Tries to make a shot.
    /// </summary>
    protected void ShotRequest()
    {
        if (Time.time - shootingTimeStamp > shotCheckFrequency)
        {
            if (Random.Range(0, shotChance) == 0)
            {
                shootingTimeStamp = Time.time;
                ShotingBullet(this.gameObject.name);
            }
        }
    }
}
