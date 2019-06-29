using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScentMap : MonoBehaviour
{

    private const int X_RESOLUTION = 20;
    private const int Y_RESOLUTION = 10;
    private const int MAX_SCENT = 300;

    [SerializeField]
    private Level level;
    private int[,] map;

    void Start()
    {
        map = new int[X_RESOLUTION, Y_RESOLUTION];
        for (int i=0; i<X_RESOLUTION; i++)
        {
            for (int j=0; j<Y_RESOLUTION; j++)
            {
                map[i, j] = 0;
            }
        }
    }

    private void Update()
    {
        // fade scent over time
        for (int i = 0; i < X_RESOLUTION; i++)
        {
            for (int j = 0; j < Y_RESOLUTION; j++)
            {
                if (map[i, j] > 0)
                {
                    map[i, j]--;
                }
            }
        }
    }

    public int GetScentAt(Vector3 position)
    {
        int i; int j;
        (i, j) = GetIndexAt(position);
        return map[i, j];
    }

    public void AddScent(Vector3 position)
    {
        int i; int j;
        (i, j) = GetIndexAt(position);
        map[i, j] = MAX_SCENT;
    }

    public (int, int) GetIndexAt(Vector3 position)
    {
        int i = (int) ((position.x - Level.MIN_X) / Level.WIDTH) * X_RESOLUTION;
        int j = (int)((position.y - Level.MIN_Y) / Level.HEIGHT) * Y_RESOLUTION;

        return (i, j);
    }
}
