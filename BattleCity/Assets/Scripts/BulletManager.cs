using UnityEngine;

/// <summary>
/// This script controlls bullets behaviour in game.
/// When player is by wall bullet should destroy wall (this is possible because OnCollisionEnter2D(Collision2D col) is called earlier than Start().
/// </summary>

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class BulletManager : MonoBehaviour {

    #region Variables
    private float bulletSpeed = 0.125f;
    private string direction;
    private string owner;
    private int tankLevel;
    private bool isAlive = false;
    #endregion
    // Use this for initialization
    void Start () {

        isAlive = true;
        
        Rigidbody2D rb;
        rb = this.GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.gravityScale = 0;
        ReadFromName();
        if (owner == "PLAYER")
        {
            this.gameObject.layer = 14;
            if (tankLevel > 0)
            {
                bulletSpeed = bulletSpeed * 1.25f;
            }
        }
        else
        {
            this.gameObject.layer = 9;
        }
        if(direction == "DIR_U")
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX;
        }
        else if (direction == "DIR_D")
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX;
            transform.RotateAround(transform.position, Vector3.forward, 180);
        }
        else if (direction == "DIR_R")
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionY;
            transform.RotateAround(transform.position, Vector3.forward, -90);
        }
        else if (direction == "DIR_L")
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionY;
            transform.RotateAround(transform.position, Vector3.forward, 90);
        }
        
    }
	
	void FixedUpdate () {
        if (!GameManager.isGamePaused)
        {
            this.gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
            this.transform.Translate(0, bulletSpeed, 0);
        }
        else
        {
            this.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
        }
    }

    /// <summary>
    /// Gets information about bullet from its name.
    /// </summary>
    void ReadFromName()
    {
        string[] values = name.Split('-');
        /* Info About Values[]:
        values[0] - who has shot this bullet
        values[1] - tank level (-1 for enemies)     
        values[2] - direction
        */
        owner = values[0];
        if (owner == "PLAYER")
            tankLevel = int.Parse(values[1]);
        else
            owner = "ENEMY";
        direction = values[2];
    }

    /// <summary>
    /// When bullet collides with an object.
    /// </summary>
    /// <param name="col">Collider which is being interacted with bullet.</param>
    void OnCollisionEnter2D(Collision2D col)
    {
        
        if(isAlive == false)
        {
            print("Forced alive state change.");
            ReadFromName();
        }
        /*Rigidbody2D rb;
        rb = this.GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        Collider2D bulletCol = this.GetComponent<Collider2D>();
        bulletCol.enabled = false;*/
        //if()isAlive
        if (col.gameObject.tag == "PLAYER_BASE")
        {
            GameManager.isGameOver = -1;
            PlayerController.controllsEnabled = false;
            GameManager.mainSoundManager.SRCexplosion1.Stop();
            GameManager.mainSoundManager.SRCexplosion1.clip = GameManager.mainSoundManager.SFXplayerDestroyed;
            GameManager.mainSoundManager.SRCexplosion1.Play();
            GameManager.mainSoundManager.SRCbackgroundNoise.Stop();

            GameObject explosionInst = Instantiate(GameManager.explosionInstance, new Vector3(6.5f, 0.5f), Quaternion.identity) as GameObject;
            Animator animator = explosionInst.GetComponent<Animator>();
            animator.SetBool("Is explosion big", true);
            Destroy(explosionInst, 0.35f);
            GameManager.levelSwitchTimeStamp = Time.time;
            GameManager.mainSoundManager.SRCbackgroundNoise.Stop();
            Destroy(col.gameObject);
            Destroy(this.gameObject);
        }
        else
        {
            if (owner == "PLAYER")
            {
                if (PlayerController.areShotsBeingFired[1])
                {
                    PlayerController.areShotsBeingFired[1] = false;
                }
                else
                {
                    PlayerController.areShotsBeingFired[0] = false;
                }
                if (col.gameObject.tag == "ENEMY")
                {
                    GameManager.EnemyGotHitten(col.gameObject, --col.gameObject.GetComponent<EnemyTankManager>().tankHealth);
                }
                else
                {
                    CheckCollisionForRays(this.transform.position, col.collider, 1 << 13);
                }
            }
            else
            {
                //GameObject.Find(owner).GetComponent<EnemyTankManager>().isShotMade = false;
                if (col.collider.tag == "PLAYER")
                {
                    if (PlayerController.isShieldActtive == false)
                    {
                        GameManager.PlayerGotKilled();
                    }
                }
                else
                {
                    CheckCollisionForRays(this.transform.position, col.collider, 1 << 12);
                }
            }
        }
        if (isAlive == true)
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Bullet destroys 4 "mini blocks" at a time, this method checks for those 4 blocks.
    /// </summary>
    /// <param name="position">Bullet position.</param>
    /// <param name="hittenCollider">Collider which was hitten.</param>
    /// <param name="layer">Physical layer.(13 for player, 12 - enemy)</param>
    void CheckCollisionForRays(Vector3 position, Collider2D hittenCollider, int layer)
    {
        Ray2D[] rays = new Ray2D[4];
        rays = GetShotsRays(position);
        Debug.DrawRay(rays[0].origin, rays[0].direction, Color.cyan, 2f);
        Debug.DrawRay(rays[1].origin, rays[1].direction, Color.red, 2f);
        Debug.DrawRay(rays[2].origin, rays[2].direction, Color.green, 2f);
        Debug.DrawRay(rays[3].origin, rays[3].direction, Color.yellow, 2f);

        bool objectHasBeenDestroyed = false; // for sound effects and animations.

        for (int i = 0; i < 4; i++) // because 4 rays
        {

            RaycastHit2D rayHit = Physics2D.Raycast(rays[i].origin, rays[i].direction, 0.2f, ~layer); 
            RaycastHit2D backHit = Physics2D.Raycast(rays[i].origin, -rays[i].direction, 0.2f, ~layer);
            if (rayHit == true)
            {
                if ((rayHit.collider.gameObject.tag == hittenCollider.gameObject.tag) || (rayHit.collider.gameObject.tag == "PICKUP"))
                {
                    if ((backHit == false) || (backHit.collider.tag == owner) || (backHit.collider.tag == "ICE"))
                    {
                        if (IsObstacleDestructable(rayHit.collider.gameObject))
                        {
                            objectHasBeenDestroyed = true;
                            Destroy(rayHit.collider.gameObject);
                        }
                    }
                    else
                    {
                        print("Backhit: " + backHit.distance + ". Collider: " + backHit.collider);
                        //backHit.collider.GetComponent<SpriteRenderer>().color = Color.red;
                    }
                }
            }
        }
        if (owner == "PLAYER")
        {
            if (objectHasBeenDestroyed)
            {
                GameManager.mainSoundManager.SRCexplosion1.Stop();
                GameManager.mainSoundManager.SRCexplosion1.clip = GameManager.mainSoundManager.SFXobstacleDestroyed;
                GameManager.mainSoundManager.SRCexplosion1.Play();
            }
            else
            {
                GameManager.mainSoundManager.SRCexplosion1.Stop();
                GameManager.mainSoundManager.SRCexplosion1.clip = GameManager.mainSoundManager.SFXhitsUnbreakable;
                GameManager.mainSoundManager.SRCexplosion1.Play();
            }
        }
        GameObject explosionInst = Instantiate(GameManager.explosionInstance, position, Quaternion.identity) as GameObject;
        Animator animator = explosionInst.GetComponent<Animator>();
        animator.SetBool("Is explosion big", false);
        Destroy(explosionInst, 0.12f); // 2 seconds for animation to play is more than enought
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Creates four rays for bullet.
    /// </summary>
    /// <param name="collisionPoint">2 colliders interacted position.</param>
    /// <returns>All four rays as an array.</returns>
    Ray2D[] GetShotsRays(Vector2 collisionPoint)
    {
        Ray2D[] rays = new Ray2D[4];
        #region The Art. 
        /* Add from up and down 0,125f */
        /*******************************/
        /******* DIRECTION = UP ********/
        /*******************************/
        /******* BULLET ORIGIN = ^ *****/
        /*******************************/
        /***** [1] [0] ^^^ [2] [3] *****/
        /*******************************/
        //collisionPoint = this.transform.position;
        #endregion

        if (direction == "DIR_U")
        {
            rays[0] = new Ray2D(new Vector3(this.transform.position.x + 0.125f, collisionPoint.y, this.transform.position.z), Vector2.up);
            rays[1] = new Ray2D(new Vector3(this.transform.position.x + 0.375f, collisionPoint.y, this.transform.position.z), Vector2.up);
            rays[2] = new Ray2D(new Vector3(this.transform.position.x - 0.125f, collisionPoint.y, this.transform.position.z), Vector2.up);
            rays[3] = new Ray2D(new Vector3(this.transform.position.x - 0.375f, collisionPoint.y, this.transform.position.z), Vector2.up);
        }
        else if (direction == "DIR_D") 
        {
            rays[0] = new Ray2D(new Vector3(this.transform.position.x + 0.125f, collisionPoint.y, this.transform.position.z), Vector2.down);
            rays[1] = new Ray2D(new Vector3(this.transform.position.x + 0.375f, collisionPoint.y, this.transform.position.z), Vector2.down);
            rays[2] = new Ray2D(new Vector3(this.transform.position.x - 0.125f, collisionPoint.y, this.transform.position.z), Vector2.down);
            rays[3] = new Ray2D(new Vector3(this.transform.position.x - 0.375f, collisionPoint.y, this.transform.position.z), Vector2.down);
        }
        else if (direction == "DIR_R")
        {
            rays[0] = new Ray2D(new Vector3(collisionPoint.x, this.transform.position.y + 0.125f, this.transform.position.z), Vector2.right);
            rays[1] = new Ray2D(new Vector3(collisionPoint.x, this.transform.position.y + 0.375f, this.transform.position.z), Vector2.right);
            rays[2] = new Ray2D(new Vector3(collisionPoint.x, this.transform.position.y - 0.125f, this.transform.position.z), Vector2.right);
            rays[3] = new Ray2D(new Vector3(collisionPoint.x, this.transform.position.y - 0.375f, this.transform.position.z), Vector2.right);
        }
        else if (direction == "DIR_L")
        {
            rays[0] = new Ray2D(new Vector3(collisionPoint.x, this.transform.position.y + 0.125f, this.transform.position.z), Vector2.left);
            rays[1] = new Ray2D(new Vector3(collisionPoint.x, this.transform.position.y + 0.375f, this.transform.position.z), Vector2.left);
            rays[2] = new Ray2D(new Vector3(collisionPoint.x, this.transform.position.y - 0.125f, this.transform.position.z), Vector2.left);
            rays[3] = new Ray2D(new Vector3(collisionPoint.x, this.transform.position.y - 0.375f, this.transform.position.z), Vector2.left);
        }

        return rays;
    }

    /// <summary>
    /// Checks if obstacle is destructable.
    /// </summary>
    /// <param name="obstacle">Object which was hitten.</param>
    /// <returns>Returns true - if object has to be destroyed (and vice versa).</returns>
    bool IsObstacleDestructable(GameObject obstacle)
    {
       // print("Obstacle check.");
        if(owner != obstacle.name)
        {
            //print(obstacle.name + " its tag: " + obstacle.tag);
            if (obstacle.tag != "WALL_UNBREAKABLE")
            {
                if (tankLevel != 3)
                {
                    if (obstacle.tag != "WALL_STEEL")
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
                
            }
        }
        return false;
    }
}
