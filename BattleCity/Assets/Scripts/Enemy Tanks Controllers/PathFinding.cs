using UnityEngine;
using System.Collections;

/// <summary>
/// Class which creates path from start to destination (executed via FindPath method)
/// </summary>
public class PathFinding : MonoBehaviour
{

    #region Variables
    /// <summary>
    /// Each node will have unique ID, position and parent. (parent marked as another nodes ID).
    /// </summary>
    struct PathNode
    {
        public int id;
        public int parent; 
        public Vector2 nodePos;
        public bool isLocked;
        public float distance;
    };

    const int maxNodes = 128; // 13 * 13 = 169(tiles) but we wont use that many!

    static int isPathDetected = 0;

    const int layer = 1 << 12; // 13 - player, 12 - any enemy
    #endregion

    /// <summary>
    /// Forming wave to find shortest path from start to destination. If path not found, returning null.
    /// </summary>
    /// <param name="start">Starting point of object.</param>
    /// <param name="destination">Destination.</param>
    /// <returns>Returns shorterst path from start to destination in Vector2 array. If path not found, returning null.</returns>
    public static Vector2[] FindPath(Vector2 start, Vector2 destination)
    {
        isPathDetected = 0;
        PathNode[] pNodes = new PathNode[maxNodes];
        for(int i = 0; i < maxNodes; i++)
        {
            pNodes[i].id = -1;
            pNodes[i].parent = -1;
            pNodes[i].nodePos = Vector2.zero;
            pNodes[i].isLocked = false;
            pNodes[i].distance = 128;
        }
        int nodeCount = 4;
        // 0 - not detected; 1 - detected; -1 - failed to detect
        PathNode[] firstNodes = new PathNode[4];
        start = SnappingToGrid(start);
        destination = SnappingToGrid(destination);
        firstNodes = FirstNodes(start, destination);
        pNodes[0] = firstNodes[0];
        pNodes[1] = firstNodes[1];
        pNodes[2] = firstNodes[2];
        pNodes[3] = firstNodes[3];
        while (isPathDetected == 0)
        {
            int currentNearestID = FindNearestNodeID(pNodes, nodeCount);
            if(currentNearestID != -1)
            {
                PathNode nearestNode = new PathNode();
                for(int i = 0; i < nodeCount; i++)
                {
                    if (pNodes[i].id == currentNearestID)
                    {
                        nearestNode = pNodes[i];
                        break;
                    }
                }
                AddingNewNodes(nearestNode, destination, start, pNodes, ref nodeCount);
            }
            else
            {
                isPathDetected = -1;
            }
        }
        if (isPathDetected == -1)
        {
            return null;
        }
        return CreatePath(pNodes, start, destination, nodeCount);
    }
    
    /// <summary>
    /// Executed to create first four nodes.
    /// </summary>
    /// <param name="_start">Center between four future nodes.</param>
    /// <param name="_destination">Destination.</param>
    /// <returns></returns>
    static PathNode[] FirstNodes(Vector2 _start, Vector2 _destination)
    {
        Vector2 upperPointStart = new Vector2(_start.x, _start.y + 1);
        Vector2 rightHPointStart = new Vector2(_start.x + 1, _start.y);
        Vector2 lowerPointStart = new Vector2(_start.x, _start.y - 1);
        Vector2 leftHPointStart = new Vector2(_start.x - 1, _start.y);

        PathNode[] output = new PathNode[4];
        if (upperPointStart.y < 12.75f)
        {

            //hit = Physics2D.Raycast(_start, Vector2.up, 1.45f, ~layer);
            Collider2D overlapedCol;
            overlapedCol = Physics2D.OverlapArea(new Vector2(upperPointStart.x - 0.4f, upperPointStart.y + 0.4f), new Vector2(upperPointStart.x + 0.4f, upperPointStart.y - 0.4f));
            if ((overlapedCol == null) || ((overlapedCol.tag != "WALL_BRICK") && (overlapedCol.tag != "WALL_STEEL") && (overlapedCol.tag != "OBSTACLE_WATER")))
            {
                output[0].distance = Vector2.Distance(upperPointStart, _destination);
                output[0].id = 0;
                output[0].isLocked = false;
                output[0].nodePos = upperPointStart;
                output[0].parent = -1;
            }
            else
            {
                output[0].isLocked = true;
            }
        }
        else
        {
            output[0].isLocked = true;
        }

        if(rightHPointStart.x < 12.75f)
        {
            Collider2D overlapedCol;
            overlapedCol = Physics2D.OverlapArea(new Vector2(rightHPointStart.x - 0.4f, rightHPointStart.y + 0.4f), new Vector2(rightHPointStart.x + 0.4f, rightHPointStart.y - 0.4f));
            if ((overlapedCol == null) || ((overlapedCol.tag != "WALL_BRICK") && (overlapedCol.tag != "WALL_STEEL") && (overlapedCol.tag != "OBSTACLE_WATER")))
            {
                output[1].distance = Vector2.Distance(rightHPointStart, _destination);
                output[1].id = 1;
                output[1].isLocked = false;
                output[1].nodePos = rightHPointStart;
                output[1].parent = -1;
            }
            else
            {
                output[1].isLocked = true;
            }
        }          
        else       
        {
            output[1].isLocked = true;
        }

        if (lowerPointStart.y > 0.25f)
        {
            Collider2D overlapedCol;
            overlapedCol = Physics2D.OverlapArea(new Vector2(lowerPointStart.x - 0.4f, lowerPointStart.y + 0.4f), new Vector2(lowerPointStart.x + 0.4f, lowerPointStart.y - 0.4f));
            if ((overlapedCol == null) || ((overlapedCol.tag != "WALL_BRICK") && (overlapedCol.tag != "WALL_STEEL") && (overlapedCol.tag != "OBSTACLE_WATER")))
            {
                output[2].distance = Vector2.Distance(lowerPointStart, _destination);
                output[2].id = 2;
                output[2].isLocked = false;
                output[2].nodePos = lowerPointStart;
                output[2].parent = -1;
            }
            else
            {
                output[2].isLocked = true;
            }
        }          
        else       
        {          
            output[2].isLocked = true;
        }
        if (leftHPointStart.x > 0.25f)
        {
            Collider2D overlapedCol;
            overlapedCol = Physics2D.OverlapArea(new Vector2(leftHPointStart.x - 0.4f, leftHPointStart.y + 0.4f), new Vector2(leftHPointStart.x + 0.4f, leftHPointStart.y - 0.4f));
            if ((overlapedCol == null) || ((overlapedCol.tag != "WALL_BRICK") && (overlapedCol.tag != "WALL_STEEL") && (overlapedCol.tag != "OBSTACLE_WATER")))
            {
                output[3].distance = Vector2.Distance(leftHPointStart, _destination);
                output[3].id = 3;
                output[3].isLocked = false;
                output[3].nodePos = leftHPointStart;
                output[3].parent = -1;
            }
            else
            {
                output[3].isLocked = true;
            }
        }         
        else      
        {         
            output[3].isLocked = true;
        }
        return output;
    }

    /// <summary>
    /// Creating new nodes from nearest. Also returns if path is already detected.
    /// </summary>
    /// <param name="nearestNode">Nearest node.</param>
    /// <param name="_destination">Destination.</param>
    /// <param name="_start">Starting position.</param>
    /// <param name="allNodes">All current nodes.</param>
    /// <param name="_nodeCount">Number of current nodes.</param>
    static void AddingNewNodes(PathNode nearestNode, Vector2 _destination, Vector2 _start, PathNode[] allNodes, ref int _nodeCount)
    {
        for (int i = 0; i < _nodeCount; i++)
        {
            if (allNodes[i].id == nearestNode.id)
            {
                allNodes[i].isLocked = true;
                break;
            }
        }
        Collider2D overlapedCol;
        /* upper point */
        if (nearestNode.nodePos.y < 12f)
        {
            overlapedCol = Physics2D.OverlapArea(new Vector2(nearestNode.nodePos.x - 0.4f, nearestNode.nodePos.y + 1.4f), new Vector2(nearestNode.nodePos.x + 0.4f, nearestNode.nodePos.y + 0.6f));
            if ((overlapedCol == null) || ((overlapedCol.tag != "WALL_BRICK") && (overlapedCol.tag != "WALL_STEEL") && (overlapedCol.tag != "OBSTACLE_WATER")))
            {
                PathNode newNode = new PathNode();

                newNode.nodePos = new Vector2(nearestNode.nodePos.x, nearestNode.nodePos.y + 1);
                newNode.id = _nodeCount;
                newNode.isLocked = false;
                newNode.parent = nearestNode.id;
                newNode.distance = Vector2.Distance(newNode.nodePos, _destination);

                bool nodeExists = false;
                if (newNode.nodePos != _start)
                {
                    for (int i = 0; i < _nodeCount; i++)
                    {
                        if (allNodes[i].nodePos == newNode.nodePos)
                        {
                            nodeExists = true;
                            break;
                        }
                    }
                    if (!nodeExists)
                    {
                        if (_nodeCount + 1 == allNodes.Length)
                        {
                            isPathDetected = -1;
                            return;
                        }
                        else
                        {
                            allNodes[_nodeCount++] = newNode;
                        }
                    }
                    if (newNode.nodePos == _destination)
                    {
                        isPathDetected = 1;
                        print("Path was found! (used " + _nodeCount + " nodes to find)");
                        return;
                    }
                }
            }
        }

        /* right hand point */
        if (nearestNode.nodePos.x < 12f)
        {
            overlapedCol = Physics2D.OverlapArea(new Vector2(nearestNode.nodePos.x + 0.6f, nearestNode.nodePos.y + 0.4f), new Vector2(nearestNode.nodePos.x + 1.4f, nearestNode.nodePos.y - 0.4f));
            if ((overlapedCol == null) || ((overlapedCol.tag != "WALL_BRICK") && (overlapedCol.tag != "WALL_STEEL") && (overlapedCol.tag != "OBSTACLE_WATER")))
            {
                PathNode newNode = new PathNode();

                newNode.nodePos = new Vector2(nearestNode.nodePos.x + 1, nearestNode.nodePos.y);
                newNode.id = _nodeCount;
                newNode.isLocked = false;
                newNode.parent = nearestNode.id;
                newNode.distance = Vector2.Distance(newNode.nodePos, _destination);

                bool nodeExists = false;
                if (newNode.nodePos != _start)
                {
                    for (int i = 0; i < _nodeCount; i++)
                    {
                        if (allNodes[i].nodePos == newNode.nodePos)
                        {
                            nodeExists = true;
                            break;
                        }
                    }
                    if (!nodeExists)
                    {
                        if (_nodeCount + 1 == allNodes.Length)
                        {
                            isPathDetected = -1;
                            return;
                        }
                        else
                        {
                            allNodes[_nodeCount++] = newNode;
                        }
                    }
                    if (newNode.nodePos == _destination)
                    {
                        isPathDetected = 1;
                        print("Path was found! (used " + _nodeCount + " nodes to find)");
                        return;
                    }
                }
            }
        }

        /* lower point */
        if (nearestNode.nodePos.y > 1f)
        {
            overlapedCol = Physics2D.OverlapArea(new Vector2(nearestNode.nodePos.x - 0.4f, nearestNode.nodePos.y - 0.6f), new Vector2(nearestNode.nodePos.x + 0.4f, nearestNode.nodePos.y + 1.4f));
            if ((overlapedCol == null) || ((overlapedCol.tag != "WALL_BRICK") && (overlapedCol.tag != "WALL_STEEL") && (overlapedCol.tag != "OBSTACLE_WATER")))
            {
                PathNode newNode = new PathNode();

                newNode.nodePos = new Vector2(nearestNode.nodePos.x, nearestNode.nodePos.y - 1);
                newNode.id = _nodeCount;
                newNode.isLocked = false;
                newNode.parent = nearestNode.id;
                newNode.distance = Vector2.Distance(newNode.nodePos, _destination);

                bool nodeExists = false;
                if (newNode.nodePos != _start)
                {
                    for (int i = 0; i < _nodeCount; i++)
                    {
                        if (allNodes[i].nodePos == newNode.nodePos)
                        {
                            nodeExists = true;
                            break;
                        }
                    }
                    if (!nodeExists)
                    {
                        if (_nodeCount + 1 == allNodes.Length)
                        {
                            isPathDetected = -1;
                            return;
                        }
                        else
                        {
                            allNodes[_nodeCount++] = newNode;
                        }
                    }
                    if (newNode.nodePos == _destination)
                    {
                        isPathDetected = 1;
                        print("Path was found! (used " + _nodeCount + " nodes to find)");
                        return;
                    }
                }
            }
        }

        /* left hand point */
        if (nearestNode.nodePos.x > 1f)
        {
            overlapedCol = Physics2D.OverlapArea(new Vector2(nearestNode.nodePos.x - 1.4f, nearestNode.nodePos.y + 0.4f), new Vector2(nearestNode.nodePos.x + 0.6f, nearestNode.nodePos.y - 0.4f));
            if ((overlapedCol == null) || ((overlapedCol.tag != "WALL_BRICK") && (overlapedCol.tag != "WALL_STEEL") && (overlapedCol.tag != "OBSTACLE_WATER")))
            {
                PathNode newNode = new PathNode();

                newNode.nodePos = new Vector2(nearestNode.nodePos.x - 1, nearestNode.nodePos.y);
                newNode.id = _nodeCount;
                newNode.isLocked = false;
                newNode.parent = nearestNode.id;
                newNode.distance = Vector2.Distance(newNode.nodePos, _destination);

                bool nodeExists = false;
                if (newNode.nodePos != _start)
                {
                    for (int i = 0; i < _nodeCount; i++)
                    {
                        if (allNodes[i].nodePos == newNode.nodePos)
                        {
                            nodeExists = true;
                            break;
                        }
                    }
                    if (!nodeExists)
                    {
                        if (_nodeCount + 1 == allNodes.Length)
                        {
                            isPathDetected = -1;
                            return;
                        }
                        else
                        {
                            allNodes[_nodeCount++] = newNode;
                        }
                    }
                    if (newNode.nodePos == _destination)
                    {
                        isPathDetected = 1;
                        print("Path was found! (used " + _nodeCount + " nodes to find)");
                        return;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Finds nearest node to destination.
    /// </summary>
    /// <param name="allNodes">All nodes.</param>
    /// <param name="_nodeCount">Number of created nodes.</param>
    /// <returns>Nearest node ID.</returns>
    static int FindNearestNodeID(PathNode[] allNodes, int _nodeCount)
    {
        PathNode nearest;
        nearest.id = -1;
        nearest.distance = 128;
        for(int i = 0; i < _nodeCount; i++)
        {
            if(allNodes[i].isLocked == false)
            {
                if(allNodes[i].distance < nearest.distance)
                {
                    nearest = allNodes[i];
                }
            }
        }
        return nearest.id;
    }

    /// <summary>
    /// Snapping coordinates to map.
    /// </summary>
    /// <param name="_position">Position which has to be snaped.</param>
    /// <returns>Snapped position.</returns>
    public static Vector2 SnappingToGrid(Vector2 _position) // used in AI to snap tank
    {
        Vector2 outputPosition = new Vector2();

        Vector2 lessThanOnePos = new Vector2();
        
        lessThanOnePos.x = _position.x - (int)_position.x;
        lessThanOnePos.y = _position.y - (int)_position.y;
        // x and y positions most of the time are != 0.5 (ex. 0.523665 or 0.499989632)
        if(lessThanOnePos.x > 0.48f && lessThanOnePos.x < 0.52f)
        {
            lessThanOnePos.x = 0.5f;
        }
        if (lessThanOnePos.y > 0.48f && lessThanOnePos.y < 0.52f)
        {
            lessThanOnePos.y = 0.5f;
        }

        /*float xPos = _position.x - (int)_position.x;
        float yPos = _position.y - (int)_position.y;*/
        outputPosition.x = (int)_position.x;
        outputPosition.y = (int)_position.y;

        if (lessThanOnePos.x < 0.5f) { } // outputPosition.x = (int)_position.x;
        else if(lessThanOnePos.x > 0.5f) { outputPosition.x++; }

        // Getting Y position
        if (lessThanOnePos.y < 0.5f) { } // outputPosition.y = (int)_position.y;
        else if(lessThanOnePos.y > 0.5f) { outputPosition.y++; }

        outputPosition.x += 0.5f; // to snap on grid.
        outputPosition.y += 0.5f; 

        return outputPosition;
    }

    /// <summary>
    /// Creates path from all generated nodes (if it exists).
    /// </summary>
    /// <param name="allNodes">All nodes</param>
    /// <param name="start">Start</param>
    /// <param name="destination">Destination</param>
    /// <param name="numberOfNodes">number of nodes</param>
    /// <returns>Sorted array of possitions.</returns>
    static Vector2[] CreatePath(PathNode[] allNodes, Vector2 start, Vector2 destination, int numberOfNodes)
    {
        Vector2[] buffer = new Vector2[numberOfNodes];
        int numberOfCoordinates = 0;
        bool pathGenerated = false;
        buffer[numberOfCoordinates++] = destination;
        int parent;
        buffer[numberOfCoordinates++] = allNodes[numberOfNodes - 1].nodePos;
        parent = allNodes[numberOfNodes - 1].parent;
        while(!pathGenerated)
        {
            for (int i = 0; i < numberOfNodes; i++)
            {
                if (allNodes[i].id == parent)
                {
                    buffer[numberOfCoordinates++] = allNodes[i].nodePos;
                    parent = allNodes[i].parent;
                    break;
                }
            }
            if(parent < 4)
            {
                for(int i = 0; i < 4; i++)
                {
                    if(allNodes[i].id == parent)
                    {
                        buffer[numberOfCoordinates++] = allNodes[i].nodePos;
                        break;
                    }
                }
                pathGenerated = true;
            }
        }
        buffer[numberOfCoordinates++] = start;

        Vector2[] output = new Vector2[numberOfCoordinates];
        for(int i = 0; i < numberOfCoordinates; i++)
        {
            output[i] = buffer[numberOfCoordinates - 1 - i];
        }
        return output;
    }

}