/// <summary>
/// this class hold information only for tank spawning ID (enemies) for each level (1 - 35) 
/// when levels end, program loads level 1 again but all levels are spawning tanks for level 35.
/// </summary>
public class EnemiesInfo
{
    //What tanks to spawn on level
    public static int[,] levels = new int[36, 4]
    {
        {18, 2, 0, 0}, // Level 1
        {14, 4, 0, 2},
        {14, 4, 0, 2},
        {2, 5, 10, 3},
        {8, 5, 5, 2}, // Level 5
        {9,2,7,2},
        {10,4,6,0},
        {7,4,7,2},
        {6,4,7,3},
        {12,2,4,2}, // Level 10
        {0,10,4,6},
        {0,6,8,6},
        {0,8,8,4},
        {0,4,10,6},
        {2,10,0,8}, // Level 15
        {16,2,0,2},
        {8,2,8,2},
        {2,8,6,4},
        {4,4,4,8},
        {2,8,2,8}, // Level 20
        {6,2,8,4},
        {6,8,2,4},
        {0,10,4,6},
        {10,4,4,2},
        {0,8,2,10}, // Level 25
        {4,6,4,6},
        {2,8,2,8},
        {15,2,2,1},
        {0,4,10,6},
        {4,8,4,4}, // Level 30
        {0,8,6,6},
        {6,4,2,8},
        {0,8,4,8},
        {0,10,4,6},
        {0,6,4,10}, // Level 35
        {0,6,4,10} // For all repeating levels above 35'th
    };

    /// <summary>
    /// Gets 4 sized array of tanks which have to be spawned for specific level.
    /// </summary>
    /// <param name="level">Which level tanks needed.</param>
    /// <returns>Number of tanks for each tank level.</returns>
    public static int[] GetWhichTanksToSpawn(int level)
    {
        int[] tanks = new int[4];
        for (int i = 0; i < 4; i++)
        {
            tanks[i] = levels[level - 1, i];
        }
        return tanks;
    }

}

