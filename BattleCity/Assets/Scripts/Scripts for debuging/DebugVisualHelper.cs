using UnityEngine;
using System.Collections;

/// <summary>
/// Class which used to help me debuging some things.
/// </summary>
public class DebugVisualHelper : MonoBehaviour {

    /// <summary>
    /// Draws ray in 2 directions (right and down)
    /// </summary>
    /// <param name="_position">Position from which rays should be drawn.</param>
	public static void DrawRay(Vector3 _position)
    {
        Debug.DrawRay(_position, Vector3.right, Color.red);
        Debug.DrawRay(_position, Vector3.down, Color.green);
	}

    /// <summary>
    /// Draws whole path.
    /// </summary>
    /// <param name="positions">Array which holds path positions.</param>
    public static void DrawPath(Vector2[] positions)
    {
        if (positions != null)
        {
            Color pathColor =new Color(Random.Range(0.2f, 1f), Random.Range(0.2f, 1f), Random.Range(0.2f, 1f));
            for (int i = 0; i < positions.Length - 1; i++)
            {
                Debug.DrawLine(positions[i], positions[i + 1], pathColor, 10f);
                if(i == 0)
                {
                    Object pathStart = Instantiate(new GameObject("PathStart"), positions[0], Quaternion.identity);
                    Destroy(pathStart, 5f);
                }
            }
        }
        else
        {
            print("[DVH]:Path not found!");
        }
    }

    /// <summary>
    /// Draws ray in movement direction.
    /// </summary>
    /// <param name="direction">Movement direction.</param>
    /// <param name="position">Ray origin.</param>
    public static void DrawRayForMovement(string direction, Vector2 position)
    {
        if(direction == "DIR_U")
        {
            Debug.DrawRay(position, Vector3.up);
        }
        else if (direction == "DIR_R")
        {
            Debug.DrawRay(position, Vector3.right);
        }
        else if (direction == "DIR_L")
        {
            Debug.DrawRay(position, Vector3.left);
        }
        else if (direction == "DIR_D")
        {
            Debug.DrawRay(position, Vector3.down);
        }
    }
}
