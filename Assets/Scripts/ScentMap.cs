using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScentMap : MonoBehaviour
{

    private const int X_RESOLUTION = 160;
    private const int Y_RESOLUTION = 80;
    private const bool VISUALISE = true;
    private const int SCENT_AREA_RADIUS = 3;

    [SerializeField]
    private Level level;
    [SerializeField]
    private Scent scentPrefab;
    private Dictionary<(int, int), Scent> map;
    private float scaleX;
    private float scaleY;

    void Start()
    {
        map = new Dictionary<(int, int), Scent>();
        scaleX = Level.WIDTH / ((float)X_RESOLUTION);
        scaleY = Level.HEIGHT / ((float)Y_RESOLUTION);
        scentPrefab.transform.localScale = new Vector3(scaleX, scaleY, 1);
        AddScentArea(Vector3.zero);
    }

    public int GetScentAt(Vector3 position)
    {
        int i; int j;
        (i, j) = GetIndexAt(position);
        if (map.ContainsKey((i, j))) {
            return map[(i, j)].amount;
        } else
        {
            return 0;
        }
    }

    public void AddScentArea(Vector3 position)
    {
        int i; int j;
        (i, j) = GetIndexAt(position);
        for (int x=-SCENT_AREA_RADIUS; x <= SCENT_AREA_RADIUS; x++)
        {
            for (int y = -SCENT_AREA_RADIUS; y <= SCENT_AREA_RADIUS; y++)
            {
                if (x*x + y*y <= SCENT_AREA_RADIUS*SCENT_AREA_RADIUS)
                {
                    AddScent((i + x, j + y), 1 - ((Mathf.Abs(x) + Mathf.Abs(y)) / ((float)SCENT_AREA_RADIUS)));
                }
            }
        }
    }

    public void AddScent((int i, int j) v, float ratio)
    {
        print(ratio);

        if ((map.ContainsKey((v.i, v.j)) && map[(v.i, v.j)] == null) || !map.ContainsKey((v.i, v.j))) {
            Scent scent = Instantiate(scentPrefab);
            scent.Initialize(ratio);
            scent.transform.position = new Vector3(
                ((v.i + 0.5f) * scaleX) + Level.MIN_X,
                ((v.j + 0.5f) * scaleY) + Level.MIN_Y,
                0
                );
            map[(v.i, v.j)] = scent;
        }
        if (map.ContainsKey((v.i, v.j)) && map[(v.i, v.j)] != null)
        {
            map[(v.i, v.j)].AddRatio(ratio
                );
        }
    }

    public (int, int) GetIndexAt(Vector3 position)
    {
        int i = (int)((position.x - Level.MIN_X) / scaleX);
        int j = (int)((position.y - Level.MIN_Y) / scaleY);

        return (i, j);
    }
}
