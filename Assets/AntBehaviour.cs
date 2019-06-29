using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntBehaviour : MonoBehaviour
{
    private const float MAX_WIGGLE = 20f;
    private const float SEARCH_SPEED = 0.02f;
    private const float RETURN_SPEED = 0.015f;

    [SerializeField]
    private Map map;
    private float speed;
    private int steps;
    private bool hasFood = false;
    private GameObject closestNest;
    private SpriteRenderer renderer;
    private Color searchingColor;
    private Color returningColor;

    public void Initialize(Map map)
    {
        this.map = map;
        steps = 0;
        renderer = GetComponentInChildren<SpriteRenderer>();
        searchingColor = renderer.color;
        returningColor = new Color(renderer.color.r + 0.1f, renderer.color.g + 0.1f, renderer.color.b + 0.1f);
    }

    void Update()
    {
        if (!hasFood)
        {
            SearchForFood();
        } else
        {
            ReturnToNest();
        }
        Wiggle();
        steps++;
    }

    void SearchForFood()
    {
        speed = SEARCH_SPEED;

    }

    void ReturnToNest()
    {
        speed = RETURN_SPEED;
    }

    // randomly change direction to give "wiggle" effect, move forward
    void Wiggle()
    {
        if (steps%4 == 0)
        {
            transform.Rotate(Vector3.forward * Random.Range(-MAX_WIGGLE, MAX_WIGGLE), Space.Self);
        }
        if (!CanMove()) transform.Rotate(Vector3.forward * 180);
        transform.position = CalculateMove();
    }

    // calculate position if the ant moves in the direction of its current rotation at its current speed
    Vector3 CalculateMove()
    {
        return transform.position + (transform.rotation * (Vector3.up * speed));
    }

    // is the next move valid?
    bool CanMove()
    {
        return map.ContainsWithinBounds(CalculateMove());
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        print(collider.name);
        if (collider.gameObject.CompareTag("Food"))
        {
            Destroy(collider.gameObject);
            hasFood = true;
            renderer.color = returningColor;
        }
    }
}
