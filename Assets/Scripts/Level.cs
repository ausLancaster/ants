using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Level : MonoBehaviour
{
    private const int ANTS_NUM = 50;
    private const int FOOD_PATCH_NUM = 3; // must be no greater than 9
    public const int MIN_X = -10;
    public const int MAX_X = 10;
    public const int MIN_Y = -5;
    public const int MAX_Y = 5;
    public const int WIDTH = MAX_X - MIN_X;
    public const int HEIGHT = MAX_Y - MIN_Y;

    [SerializeField]
    private AntFactory antFactory;

    void Start()
    {
        PlaceAnts();
        PlaceFoodPatches(FOOD_PATCH_NUM);
    }

    private void PlaceAnts()
    {
        for (int i = 0; i < ANTS_NUM; i++)
        {
            antFactory.CreateAnt(new Vector3(UnityEngine.Random.Range(MIN_X, MAX_X), UnityEngine.Random.Range(MIN_Y, MAX_Y), 0));
        }
    }

    private void PlaceFoodPatches(int n)
    {
        if (FOOD_PATCH_NUM > 9) throw new ArgumentOutOfRangeException("Food patch must be no greater than 0");
        Shuffle shuffle = new Shuffle(9);
        for (int i = 0; i < FOOD_PATCH_NUM; i++)
        {
            int ninth = shuffle.GetNext();
            if (ninth == 4) ninth = shuffle.GetNext();
            Vector3 location = new Vector3(
                (ninth % 3) * (WIDTH / 3f) + MIN_X + UnityEngine.Random.value * (WIDTH / 3f),
                (ninth / 3) * (HEIGHT / 3f) + MIN_Y + UnityEngine.Random.value * (HEIGHT / 3f),
                0
                );
            antFactory.CreateFoodPatch(location);
        }
    }

    public bool ContainsWithinBounds(Vector3 pos)
    {
        return pos.x >= MIN_X && pos.x <= MAX_X && pos.y >= MIN_Y && pos.y <= MAX_Y;
    }
}
