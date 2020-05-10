using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntFactory : MonoBehaviour
{
    private const float FOOD_PATCH_RADIUS = 1f;
    private const float FOOD_PATCH_SPACING = 0.25f;

    [SerializeField]
    private AntBehaviour antPrefab;
    [SerializeField]
    private GameObject foodPrefab;
    [SerializeField]
    private Level level;
    [SerializeField]
    private ScentMap scentMap;
    [SerializeField]
    private Color blueColor;
    [SerializeField]
    private Color redColor;

    public AntBehaviour CreateAnt(Vector3 location, Team team)
    {
        AntBehaviour ant = Instantiate(antPrefab);
        ant.team = team;
        if (team == Team.Blue)
        {
            ant.GetComponent<SpriteRenderer>().color = blueColor;
        } else if (team == Team.Red)
        {
            ant.GetComponent<SpriteRenderer>().color = redColor;
        }
        ant.Initialize(level, scentMap);
        ant.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));
        ant.transform.localPosition = location;
        return null;
    }

    public void CreateFoodPatch(Vector3 location)
    {
        for (float y = location.y - FOOD_PATCH_RADIUS;
            y <= location.y + FOOD_PATCH_RADIUS;
            y += FOOD_PATCH_SPACING)
        {
            for (float x = location.x - FOOD_PATCH_RADIUS;
                x <= location.x + FOOD_PATCH_RADIUS;
                x += FOOD_PATCH_SPACING)
            {
                Vector3 v = new Vector3(x, y, 0);

                // make sure food patch is circle shaped, and that it is inside map bounds
                if ((location-v).magnitude < FOOD_PATCH_RADIUS
                    && level.ContainsWithinBounds(v))
                {
                    CreateFood(v);
                }
            }
        }
    }

    public GameObject CreateFood(Vector3 location)
    {
        GameObject food = Instantiate(foodPrefab);
        food.transform.localPosition = location;
        return food;
    }
}
