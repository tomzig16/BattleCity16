using UnityEngine;
using System.Collections;

/// <summary>
/// This class controlls main behaviour of all tanks (such as movement and shoting).
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class TankManagement : MonoBehaviour
{
    #region Variables.

    //[Header("Bullet")]
    public GameObject bullet;

    /// <summary>
    ///DIR_U - direction up
    ///DIR_D - direction down
    ///DIR_L - direction left
    ///DIR_R - direction right     
    /// </summary>
    protected string shotDirection = "DIR_U";
    /// <summary>
    ///DIR_U - direction up
    ///DIR_D - direction down
    ///DIR_L - direction left
    ///DIR_R - direction right     
    /// </summary>
    protected string currentMoveDirection = "DIR_U";
    /// <summary>
    ///DIR_U - direction up
    ///DIR_D - direction down
    ///DIR_L - direction left
    ///DIR_R - direction right     
    /// </summary>
    private string oldMoveDirection = "DIR_D";

    private float spawnDistanceFromTank = 0.15f;

    protected Rigidbody2D rb;

    /// <summary>
    /// There are 4 (starting from 0) tank levels.
    /// </summary>
    protected int tankLevel;

    public Animator tankAnimator;

    protected bool isThisTankAlive = false;

    protected bool isTankOnIce;

    #endregion

    /// <summary>
    /// Recommended to initiate this method.
    /// It configures basic tanks physics.
    /// </summary>
    protected void ConfiguringStartup()
    {
        rb = this.GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.gravityScale = 0;

        isTankOnIce = false;

        tankAnimator = GetComponent<Animator>();
        tankAnimator.SetBool("Tank is moving", false);

        shotDirection = "DIR_U";
        if(this.gameObject.tag != "PLAYER")
        {
            shotDirection = "DIR_D";
            currentMoveDirection = "DIR_D";
            oldMoveDirection = "DIR_D";
        }
    }

    /// <summary>
    /// Rounds position to *.5f by x and y axises.
    /// </summary>
    /// <param name="_position">Position that needs to be rounded</param>
    /// <returns>Rounded position.</returns>
    protected Vector3 RoundingPosition(Vector3 _position)
    {
        Vector3 outputPosition = new Vector3();
        float xPos = _position.x - (int)_position.x;
        float yPos = _position.y - (int)_position.y;
        outputPosition.x = (int)_position.x;
        outputPosition.y = (int)_position.y;

        if (xPos < 0.125f) { } // outputPosition.x = (int)_position.x;
        else if ((xPos >= 0.125f) && (xPos < 0.375f)) { outputPosition.x += 0.25f; }
        else if ((xPos >= 0.375f) && (xPos < 0.625f)) { outputPosition.x += 0.5f; }
        else if ((xPos >= 0.625f) && (xPos < 0.875f)) { outputPosition.x += 0.75f; }
        else { outputPosition.x++; }

        // Getting Y position
        if (yPos < 0.125f) { } // outputPosition.y = (int)_position.y;
        else if ((yPos >= 0.125f) && (yPos < 0.375f)) { outputPosition.y += 0.25f; }
        else if ((yPos >= 0.375f) && (yPos < 0.625f)) { outputPosition.y += 0.5f; }
        else if ((yPos >= 0.625f) && (yPos < 0.875f)) { outputPosition.y += 0.75f; }
        else { outputPosition.y++; }

        outputPosition.z = 0; 
        return outputPosition;
    }

    /// <summary>
    /// Performs tanks movement.
    /// </summary>
    /// <param name="_speed">Tanks speed</param>
    protected void PerformMovement(float _speed)
    {
        //Vector2 moveTo = new Vector2();
        //moveTo = Vector2.zero;
        if (currentMoveDirection != oldMoveDirection)
        {
            
            this.transform.position = RoundingPosition(this.transform.position);
            oldMoveDirection = currentMoveDirection;
            //this.transform.rotation.SetEulerAngles(Vector3.zero);
            switch (currentMoveDirection)
            {
                case "DIR_U":
                    this.transform.localRotation = Quaternion.identity;
                    break;

                case "DIR_D":
                    this.transform.localRotation = Quaternion.identity;
                    this.transform.RotateAround(this.transform.position, Vector3.forward, 180);
                    break;

                case "DIR_L":
                    this.transform.localRotation = Quaternion.identity;
                    this.transform.RotateAround(this.transform.position, Vector3.forward, 90);
                    break;

                case "DIR_R":
                    this.transform.localRotation = Quaternion.identity;
                    this.transform.RotateAround(this.transform.position, Vector3.forward, -90);
                    break;
            }
        }

        if (currentMoveDirection != null || isTankOnIce == true)
        {
            rb.transform.Translate(0, _speed, 0);
        }
        //print("Perfomr Movement");
        //this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.125f, this.transform.position.z);
    }

    /// <summary>
    /// Main method to spawn bullet.
    /// </summary>
    /// <param name="_owner">Bullets owner.</param>
    protected void ShotingBullet(string _owner)
    {
        if (currentMoveDirection != null)
        {
            BulletSpawn(_owner, currentMoveDirection);
        }
        else
        {
            BulletSpawn(_owner, shotDirection);
        }
    }

    /// <summary>
    /// Spawns bullet with right parameters.
    /// </summary>
    /// <param name="_owner">Who bullet belongs to.</param>
    /// <param name="_bDirection">Bullets direction.</param>
    private void BulletSpawn(string _owner, string _bDirection)
    {
        Vector3 bulletSpawnPoint = new Vector3();
        // grid step is 0,25 (for blocks) so we must check where to spawn bullet.
        bulletSpawnPoint = RoundingPosition(this.transform.position);
        switch (_bDirection)
        {
            case "DIR_U":
                bulletSpawnPoint.y = bulletSpawnPoint.y + spawnDistanceFromTank;
                break;

            case "DIR_D":
                bulletSpawnPoint.y = bulletSpawnPoint.y - spawnDistanceFromTank;
                break;

            case "DIR_L":
                bulletSpawnPoint.x = bulletSpawnPoint.x - spawnDistanceFromTank;
                break;

            case "DIR_R":
                bulletSpawnPoint.x = bulletSpawnPoint.x + spawnDistanceFromTank;
                break;
        }
        GameObject spawnedBullet = Instantiate(bullet, bulletSpawnPoint, Quaternion.identity) as GameObject;
        spawnedBullet.name = _owner + "-" + tankLevel + "-" + _bDirection;
        Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), spawnedBullet.GetComponent<Collider2D>());
        //print("Bullet spawned at: " + bulletSpawnPoint);
    }

    // OnTriggerEnter2D used here only to detect if tank is on ice.
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "ICE")
        {
            isTankOnIce = true;
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "ICE")
        {
            isTankOnIce = false;
        }
    }
}
