using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AntState { ReturnToNest, FollowTrail, SearchForFood }


public class AntBehaviour : MonoBehaviour
{
    private readonly bool CAN_STEAL = true;
    private const float MAX_WIGGLE = 30f;
    private const float SEARCH_SPEED = 0.02f;
    private const float RETURN_SPEED = 0.015f;
    private const float SENSE_ANGLE = 30f;
    private const float SENSE_DISTANCE = 0.06f;
    private const float MIN_CORRECTION_ANGLE = 30f;
    private const float ANGLE_CORRECTION_SPEED = 800f;//200f;
    private const float CLOSE_NEST_DIST = 3f;
    private const float STEAL_CHANCE = 0.3f;

    private Level level;
    private ScentMap scentMap;
    private float speed;
    private int steps;
    [SerializeField]
    private AntState state = AntState.SearchForFood;
    private Vector3 closestNestPos = Vector3.zero;
    private Vector3 targetFoodPos;
    private List<Vector3> checkedPositions;
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
        checkedPositions = new List<Vector3>();
    }

    void Update()
    {
        switch (state)
        {
            case AntState.FollowTrail:
                FollowTrail();
                break;
            case AntState.SearchForFood:
                SearchForFood();
                break;
            case AntState.ReturnToNest:
                ReturnToNest();
                break;
        }

        Wiggle();

        steps++;
    }

    void SearchForFood()
    {
        speed = SEARCH_SPEED;
        // start following scent if there is a scent at current position
        ScentMap.Scent scent = scentMap.GetScentAt(transform.position);
        if (scent != null && !checkedPositions.Contains(scent.foodPosition))
        {
            state = AntState.FollowTrail;
            targetFoodPos = scent.foodPosition;
            checkedPositions.Add(scent.foodPosition);
        }
    }

    void FollowTrail()
    {
        speed = SEARCH_SPEED;
        MoveTowardLocation(targetFoodPos);
    }

    void ReturnToNest()
    {
        speed = RETURN_SPEED;
        MoveTowardLocation(closestNestPos);
        scentMap.AddScentNeighbours(transform.position, targetFoodPos);
    }

    void DeliverFood(bool stolen)
    {
        if (!stolen) Destroy(carriedObject.gameObject);
        carriedObject = null;
        state = AntState.SearchForFood;
        spriteRenderer.color = searchingColor;
        ReverseDirection();
        checkedPositions = new List<Vector3>();
    }

    void MoveTowardLocation(Vector3 location)
    {

        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, location - transform.position);
        if (Quaternion.Angle(transform.rotation, targetRotation) > MIN_CORRECTION_ANGLE)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                targetRotation, 
                ANGLE_CORRECTION_SPEED * Time.deltaTime
                );
        }
    }

    // randomly change direction to give "wiggle" effect, move forward
    void Wiggle()
    {
        // check if ant has reached end of food trail
        if ((state == AntState.FollowTrail) && ((transform.position - targetFoodPos).sqrMagnitude < (speed*2)*(speed*2))) {
            transform.position = targetFoodPos;
            state = AntState.SearchForFood;
        }
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

    private void OnTriggerStay2D(Collider2D other)
    {
        if (state == AntState.ReturnToNest)
        {
            if (other.gameObject.CompareTag("Nest"))
            {
                // drop off food and start searching for food again
                DeliverFood(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (state == AntState.ReturnToNest)
        {
            if (other.gameObject.CompareTag("Nest"))
            {
                // drop off food and start searching for food again
                DeliverFood(false);
            }
        } else
        {
            if (other.gameObject.CompareTag("Food"))
            {
                carriedObject = other.gameObject.GetComponent<Carriable>();
                if (carriedObject == null) throw new System.Exception("Tried to pick up food that has no Carriable script");
                if (!(!CAN_STEAL && carriedObject.isCarried) && 
                    !(CAN_STEAL && carriedObject.isCarried && !(UnityEngine.Random.value < STEAL_CHANCE)))
                {
                    // pick up food and return to nest
                    if (carriedObject.isCarried)
                    {
                        carriedObject.carrier.DeliverFood(true);
                    }
                    carriedObject.transform.parent = transform;
                    carriedObject.transform.localPosition = jawsOffset;
                    carriedObject.transform.rotation = transform.rotation;
                    carriedObject.carrier = this;
                    carriedObject.GetComponent<Carriable>().isCarried = true;
                    state = AntState.ReturnToNest;
                    spriteRenderer.color = returningColor;
                    targetFoodPos = carriedObject.transform.position;
                }
            }
        }

    }
}
