using UnityEngine;
using System.Collections;

/// <summary>
/// This is experimental class, which is not actually used in game, but some levels were created using this method.
/// </summary>
public class DataReadFromBitmap : MonoBehaviour {

    public Texture2D texture;
    public Texture2D bigTexture;
    public GameObject block;
    public GameObject round;

	// Use this for initialization
	void Start () {
        GameObject thingie = new GameObject("Sprite 1");
        thingie.AddComponent<SpriteRenderer>();
        thingie.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
        /*print(texture.GetPixels().Length);
        print(texture.width + " " + texture.GetPixel(10,10));*/

        /*for(int yAxis = 0; yAxis < texture.height; yAxis++)
        {
            for(int xAxis = 0; xAxis < texture.width; xAxis++)
            {
                if(texture.GetPixel(xAxis, yAxis) == Color.white)
                {
                    Instantiate(block, new Vector3(xAxis, yAxis, 0), Quaternion.identity);
                }
            }
        }*/
/*
        for (int yAxis = 0; yAxis < bigTexture.height; yAxis++)
        {
            for (int xAxis = 0; xAxis < bigTexture.width; xAxis++)
            {
                Vector3 worldPos = new Vector3(xAxis / 4f, yAxis / 4f, 0);
                if (bigTexture.GetPixel(xAxis, yAxis) == Color.white)
                {
                    Instantiate(block, worldPos, Quaternion.identity);
                }
                else if (bigTexture.GetPixel(xAxis, yAxis) == Color.red)
                {
                    Instantiate(round, worldPos, Quaternion.identity);
                }
            }
        }*/




        thingie.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
