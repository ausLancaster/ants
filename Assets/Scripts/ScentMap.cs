using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScentMap : MonoBehaviour
{

    private const int X_RESOLUTION = 64;
    private const int Y_RESOLUTION = 32;
    private const bool VISUALISE = true;
    private const int SCENT_AREA_RADIUS = 3;
    private const int MAX_SCENT = 300;

    [SerializeField]
    private readonly Level level;
    private Dictionary<Coordinate, Scent> map;
    private float scaleX;
    private float scaleY;

    void Start()
    {
        map = new Dictionary<Coordinate, Scent>();
        scaleX = Level.WIDTH / ((float)X_RESOLUTION);
        scaleY = Level.HEIGHT / ((float)Y_RESOLUTION);
    }

    private void Update()
    {
        foreach (Coordinate key in map.Keys.ToList())
        {
            if (map.ContainsKey(key))
            {
                map[key].amount--;
                if (map[key].amount == 0)
                {
                    map.Remove(key);
                }
            }
        }
    }

    public Scent GetScentAt(Vector3 position)
    {
        Coordinate coord = GetIndexAt(position);
        if (map.ContainsKey(coord)) {
            return map[coord];
        } else
        {
            return null;
        }
    }

    public void AddScentNeighbours(Vector3 position, Vector3 foodPos)
    {
        Coordinate coord = GetIndexAt(position);
        AddScent(coord, 1f, foodPos);
        AddScent(new Coordinate(coord.x - 1, coord.y), 1f, foodPos);
        AddScent(new Coordinate(coord.x + 1, coord.y), 1f, foodPos);
        AddScent(new Coordinate(coord.x, coord.y - 1), 1f, foodPos);
        AddScent(new Coordinate(coord.x, coord.y + 1), 1f, foodPos);
    }

    public void AddScentArea(Vector3 position, Vector3 foodPos)
    {
        Coordinate coord = GetIndexAt(position);
        for (int x=-SCENT_AREA_RADIUS; x <= SCENT_AREA_RADIUS; x++)
        {
            for (int y = -SCENT_AREA_RADIUS; y <= SCENT_AREA_RADIUS; y++)
            {
                if (x*x + y*y <= SCENT_AREA_RADIUS*SCENT_AREA_RADIUS)
                {
                    AddScent(new Coordinate(coord.x + x, coord.y + y), 1 - ((Mathf.Abs(x) + Mathf.Abs(y)) / ((float)SCENT_AREA_RADIUS)), foodPos);
                }
            }
        }
    }

    public void AddScent(Coordinate coord, float ratio, Vector3 foodPos)
    {
        map[coord] = new Scent(coord, (int)(ratio * MAX_SCENT), foodPos);
        /*if ((map.ContainsKey(coord) && map[coord] == null) || !map.ContainsKey(coord))
        {
            Scent scent = new Scent(coord, (int) (ratio * MAX_SCENT), foodPos);
            map[coord] = scent;
        }
        if (map.ContainsKey(coord) && map[coord] != null)
        {
            map[coord].amount += (int) (ratio * MAX_SCENT);
            if (map[coord].amount > MAX_SCENT)
            {
                map[coord].amount = MAX_SCENT;
            }
        }*/
    }

    public Coordinate GetIndexAt(Vector3 position)
    {
        int i = (int)((position.x - Level.MIN_X) / scaleX);
        int j = (int)((position.y - Level.MIN_Y) / scaleY);

        return new Coordinate(i, j);
    }

    public struct Coordinate
    {
        public int x;
        public int y;

        public Coordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class Scent
    {
        public Coordinate pos;
        public int amount;
        public Vector3 foodPosition;

        public Scent(Coordinate pos, int amount, Vector3 foodPosition)
        {
            this.pos = pos;
            this.amount = amount;
            this.foodPosition = foodPosition;
        }
    }
}
