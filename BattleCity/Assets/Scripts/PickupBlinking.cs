using UnityEngine;
using System.Collections;

// I was about to put all the pickups stuff here, but I have done it in GameManager,
// When I understood that I should better have an other class for it, it was too much to copy.
/// <summary>
/// This class is made only for pickups so they can blink.
/// </summary>
public class PickupBlinking : MonoBehaviour {
    #region Variables
    private Sprite pickup;
    private bool isHidden;
    private float hidedOn;
    [SerializeField()]
    private float hideDuration = 0.25f;
    #endregion
    // Use this for initialization
    void Start () {
        pickup = this.GetComponent<SpriteRenderer>().sprite;
        isHidden = false;
        hidedOn = Time.time;
	}

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.isGamePaused)
        {
            if (!isHidden)
            {
                if (Time.time - hidedOn >= hideDuration)
                {
                    this.GetComponent<SpriteRenderer>().sprite = null;
                    hidedOn = Time.time;
                    isHidden = true;
                }
            }
            else
            {
                if (Time.time - hidedOn >= hideDuration)
                {
                    this.GetComponent<SpriteRenderer>().sprite = pickup;
                    hidedOn = Time.time;
                    isHidden = false;
                }

            }
        }
    }
}
