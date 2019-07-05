using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntBehaviour : MonoBehaviour
{
    private const float MAX_WIGGLE = 30f;
    private const float SEARCH_SPEED = 0.02f;
    private const float RETURN_SPEED = 0.015f;
    private const float SENSE_ANGLE = 30f;
    private const float SENSE_DISTANCE = 0.06f;
    private const float ANGLE_CORRECTION_SPEED = 800f;//200f;
    private const float CLOSE_NEST_DIST = 3f;

    private Level level;
    private ScentMap scentMap;
    private float speed;
    private int steps;
    private bool carryingObject = false;
    private GameObject closestNest;
    private SpriteRenderer spriteRenderer;
    private Color searchingColor;
    private Color returningColor;
    private Vector3 jawsOffset = new Vector3(0, 0.17f, 0);
    private Carriable carriedObject;

    public void Initialize(Level level, ScentMap scentMap)
    {
        this.level = level;
        this.scentMap = scentMap;
        steps = 0;
        spriteRenderer = GetComponent<SpriteRenderer>();
        searchingColor = spriteRenderer.color;
        returningColor = new Color(
            spriteRenderer.color.r + 0.2f,
            spriteRenderer.color.g + 0.2f,
            spriteRenderer.color.b + 0.2f
            );
    }

    void Update()
    {
        if (!carryingObject)
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
        Vector3 posAhead = CalculateMove();
        posAhead = transform.position + (transform.rotation * Quaternion.identity * (Vector3.up * SENSE_DISTANCE));
        float scentAhead = scentMap.GetScentAt(posAhead);
        Vector3 posLeft = CalculateMove(Quaternion.Euler(0, 0, -SENSE_ANGLE));
        posAhead = transform.position + (transform.rotation * Quaternion.Euler(0, 0, -SENSE_ANGLE) * (Vector3.up * SENSE_DISTANCE));
        float scentLeft = scentMap.GetScentAt(posLeft);
        Vector3 posRight = CalculateMove(Quaternion.Euler(0, 0, SENSE_ANGLE));
        posAhead = transform.position + (transform.rotation * Quaternion.Euler(0, 0, SENSE_ANGLE) * (Vector3.up * SENSE_DISTANCE));
        float scentRight = scentMap.GetScentAt(posRight);
        Vector3 posFarLeft = CalculateMove(Quaternion.Euler(0, 0, -SENSE_ANGLE*2));
        posAhead = transform.position + (transform.rotation * Quaternion.Euler(0, 0, -SENSE_ANGLE) * (Vector3.up * SENSE_DISTANCE));
        float scentFarLeft = scentMap.GetScentAt(posLeft);
        Vector3 posFarRight = CalculateMove(Quaternion.Euler(0, 0, SENSE_ANGLE*2));
        posAhead = transform.position + (transform.rotation * Quaternion.Euler(0, 0, SENSE_ANGLE) * (Vector3.up * SENSE_DISTANCE));
        float scentFarRight = scentMap.GetScentAt(posRight);

        // choose the position closest to nest
        Vector3 nextPos = posAhead;
        float highestScent = scentAhead;
        Quaternion rot = Quaternion.identity;
        if (scentLeft > highestScent)
        {
            highestScent = scentLeft;
            rot = Quaternion.Euler(0, 0, -SENSE_ANGLE);
        }
        if (scentRight > highestScent)
        {
            highestScent = scentRight;
            rot = Quaternion.Euler(0, 0, SENSE_ANGLE);
        }
        if (scentFarLeft > highestScent)
        {
            highestScent = scentFarLeft;
            rot = Quaternion.Euler(0, 0, -SENSE_ANGLE*2);
        }
        if (scentFarRight > highestScent)
        {
            highestScent = scentFarRight;
            rot = Quaternion.Euler(0, 0, SENSE_ANGLE*2);
        }
        transform.rotation *= rot;
    }

    void ReturnToNest()
    {
        speed = RETURN_SPEED;
        MoveTowardNest();
        scentMap.AddScentArea(transform.position);
    }

    void MoveTowardNest()
    {

        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, -transform.position);
        if (Quaternion.Angle(transform.rotation, targetRotation) > 30)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, ANGLE_CORRECTION_SPEED * Time.deltaTime);
        }
    }

    // randomly change direction to give "wiggle" effect, move forward
    void Wiggle()
    {
        if (steps%4 == 0)
        {
            transform.Rotate(Vector3.forward * Random.Range(-MAX_WIGGLE, MAX_WIGGLE), Space.Self);
        }
        if (!CanMove()) ReverseDirection();
        transform.position = CalculateMove();
    }

    void ReverseDirection()
    {
        transform.Rotate(Vector3.forward * 180);
    }

    // calculate position if the ant moves in the direction of its current rotation at its current speed
    Vector3 CalculateMove()
    {
        return CalculateMove(Quaternion.identity);
    }

    // calculate position if the ant moves in a given direction relative to its current direction at its current speed
    Vector3 CalculateMove(Quaternion direction)
    {
        return transform.position + (transform.rotation * direction * (Vector3.up * speed));
    }

    // is the next move valid?
    bool CanMove()
    {
        return level.ContainsWithinBounds(CalculateMove());
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (carryingObject)
        {
            if (collider.gameObject.CompareTag("Nest"))
            {
                Destroy(carriedObject.gameObject);
                carryingObject = false;
                spriteRenderer.color = searchingColor;
                ReverseDirection();
            }
        } else
        {
            if (collider.gameObject.CompareTag("Food"))
            {
                carriedObject = collider.gameObject.GetComponent<Carriable>();
                if (carriedObject == null) throw new System.Exception("Tried to pick up food that has no Carriable script");
                if (!carriedObject.carried)
                {
                    carriedObject.transform.parent = transform;
                    carriedObject.transform.localPosition = jawsOffset;
                    carriedObject.transform.rotation = transform.rotation;
                    carriedObject.GetComponent<Carriable>().carried = true;
                    carryingObject = true;
                    spriteRenderer.color = returningColor;
                }
            }
        }

    }
}
