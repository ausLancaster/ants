using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AntState { ReturnToNest, DragToNest, FollowTrail, SearchForFood, Attack }
public enum Team { Blue, Red }
public enum AttackState { SwipeIn, SwipeOut, CloseIn, Backpedal, Wait }

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
    private const float PULL_FORCE = 1.25f;
    private const float COLLISION_SPEED_MODIFIER = 0.1f;
    private const float PULL_WIGGLE = 0.25f;
    private float ATTACK_POS_NEAR = 0.1f;
    private float ATTACK_POS_MEDIUM = 0.2f;
    private float ATTACK_POS_FAR = 0.4f;
    private float SWIPE_IN_SPEED = 0.06f;
    private float SWIPE_OUT_SPEED = 0.03f;
    private float CLOSE_IN_SPEED = 0.04f;
    private float SWIPE_CHANCE = 0.005f;
    private float SWIPE_DURATION = 0.5f;

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
    private Vector3 jawsOffset = new Vector3(0, 0.16f, 0);
    private Carriable carriedObject;
    private HingeJoint2D hinge;
    private Rigidbody2D rb;
    [SerializeField]
    private float speedModifier = 1;
    [SerializeField]
    private GameObject collidingWith;
    private bool isSlowedByCollision = false;
    public Team team;
    private Animator animator;
    private GameObject jawsAndBody;
    private GameObject hingedTo;
    private Vector3 hingedToOffset;
    private AntBehaviour attackTarget;
    private AttackState attackState;
    private float swipeTimer;
    private Vector3 v_n;
    private float m;

    public void Initialize(Level level, ScentMap scentMap)
    {
        this.level = level;
        this.scentMap = scentMap;
        steps = 0;
        spriteRenderer = GetComponent<SpriteRenderer>();
        searchingColor = spriteRenderer.color;
        checkedPositions = new List<Vector3>();
        hinge = GetComponentInChildren<HingeJoint2D>();
        rb = GetComponentInChildren<Rigidbody2D>();
        animator = GetComponent<Animator>();
        jawsAndBody = GetComponentInChildren<JawsAndBody>().gameObject;
    }

    void Update()
    {
        Vector3 changePos = jawsAndBody.transform.position - transform.position;
        Quaternion changeRot = jawsAndBody.transform.rotation * Quaternion.Inverse(transform.rotation);

        transform.position = jawsAndBody.transform.position;
        transform.rotation = jawsAndBody.transform.rotation;
        jawsAndBody.transform.position -= changePos;
        jawsAndBody.transform.rotation *= Quaternion.Inverse(changeRot);

        switch (state)
        {
            case AntState.FollowTrail:
                FollowTrail();
                Wiggle();
                break;
            case AntState.SearchForFood:
                SearchForFood();
                Wiggle();
                break;
            case AntState.ReturnToNest:
                ReturnToNest();
                Wiggle();
                break;
            case AntState.DragToNest:
                DragFoodHome();
                break;
            case AntState.Attack:
                Attack();
                break;
        }
        if (isSlowedByCollision)
        {
            speedModifier = COLLISION_SPEED_MODIFIER;
        }
        else
        {
            speedModifier = 1f;
        }
        isSlowedByCollision = false;


        steps++;

        if (Input.GetMouseButtonDown(1))
        {
            animator.ResetTrigger("AttackTrigger");
            animator.SetTrigger("AttackTrigger");
        }
    }

    private void FixedUpdate()
    {
        if (hinge.isActiveAndEnabled && hingedTo != null)
        {
            hinge.connectedAnchor = hingedTo.transform.rotation * hingedToOffset;
       }
    }

    void SearchForFood()
    {
        speed = SEARCH_SPEED * speedModifier;
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
        speed = SEARCH_SPEED * speedModifier;
        MoveTowardLocation(targetFoodPos);
    }

    void ReturnToNest()
    {
        speed = RETURN_SPEED * speedModifier;
        if (steps % 4 == 0) MoveTowardLocation(closestNestPos);
        scentMap.AddScentNeighbours(transform.position, targetFoodPos);
    }

    void DragFoodHome()
    {
        rb.AddForce((closestNestPos - transform.position + transform.rotation * jawsOffset).normalized * PULL_FORCE);

        if ((steps / 8)%2 == 0)
        {
            rb.AddTorque(PULL_WIGGLE);
        }
        else
        {
            rb.AddTorque(-PULL_WIGGLE);
        }

        /*if (steps % 4 == 0)
        {
            transform.RotateAround(transform.position + transform.rotation * jawsOffset, Vector3.forward, UnityEngine.Random.Range(-MAX_WIGGLE/4, MAX_WIGGLE/4));       
        }*/
    }

    private void DeliverFood()
    {
        hinge.enabled = false;
        if (carriedObject)
        {
            GameObject toDestroy = carriedObject.gameObject;
            carriedObject.RemoveFoodFromCarriers();
            Destroy(toDestroy);
        }
    }

    private void StealFood()
    {
        carriedObject.RemoveFoodFromCarriers(this);
    }

    private void DropFood()
    {
        if (carriedObject.droppable)
        {
            carriedObject.transform.parent = null;
            carriedObject.isCarried = false;
            carriedObject.originalPosition = transform.position + transform.rotation * jawsOffset;
        }
        LoseFood();
    }

    public void LoseFood()
    {
        hinge.enabled = false;
        carriedObject = null;
        state = AntState.SearchForFood;
        spriteRenderer.color = searchingColor;
        ReverseDirection();
        checkedPositions = new List<Vector3>();
        hinge.connectedBody = null;
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
        // if the direction the ant is moving will put it out of bounds, reverse direciton
        if (!CanMove()) ReverseDirection();

        // move
        transform.position = CalculateMove();

        // move ant within bounds if it has been pushed off the map
        //transform.position = level.MoveToWithinBounds(transform.position);
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
        if (state == AntState.ReturnToNest || state == AntState.DragToNest)
        {
            if (other.gameObject.CompareTag("Nest"))
            {
                // drop off food and start searching for food again
                DeliverFood();
            }
        }
    }

    public void Trigger(Collider2D other)
    {
        if (other.gameObject.CompareTag("Vision"))
        {
            AntBehaviour otherAnt = other.gameObject.transform.parent.GetComponent<AntBehaviour>();
            otherAnt.SpotAnt(this);
        }

        if (state == AntState.ReturnToNest || state == AntState.DragToNest)
        {
            if (other.gameObject.CompareTag("Nest"))
            {
                // drop off food and start searching for food again
                DeliverFood();
                if (carriedObject)
                {
                    throw new System.Exception("Delivered food but still carrying something");
                }
            }
        } else if (state == AntState.FollowTrail || state == AntState.SearchForFood)
        {
            if (other.gameObject.CompareTag("Food"))
            {
                Carriable otherCarriable = other.gameObject.GetComponent<Carriable>();
                if (otherCarriable == null) throw new System.Exception("Tried to pick up food that has no Carriable script");
                if (!(!CAN_STEAL && otherCarriable.isCarried) && 
                    !(CAN_STEAL && otherCarriable.isCarried && !(UnityEngine.Random.value < STEAL_CHANCE)))
                {
                    carriedObject = otherCarriable;
                    if (carriedObject.isCarried)
                    {
                        // steal food
                        StealFood();
                    }
                    // pick up food and return to nest
                    carriedObject.transform.parent = transform;
                    carriedObject.transform.localPosition = jawsOffset;
                    carriedObject.transform.rotation = transform.rotation;
                    carriedObject.AddCarrier(this);
                    carriedObject.isCarried = true;
                    state = AntState.ReturnToNest;
                    targetFoodPos = carriedObject.originalPosition;
                }
            }
        }

    }

    public void Collision(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Food")
            && !(state == AntState.ReturnToNest || state == AntState.DragToNest))
        {
            // pick up big food
            carriedObject = collision.gameObject.GetComponent<Carriable>();
            if (!carriedObject) throw new System.Exception("Tried to pick up food that has no Carriable script");
            if (!carriedObject.delivered)
            {
                Vector3 hingePoint = new Vector3(collision.GetContact(0).point.x, collision.GetContact(0).point.y, 0);
                transform.position = hingePoint - transform.rotation * jawsOffset;
                Rigidbody2D otherRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (!otherRb) throw new System.Exception("Tried to pick up food that has no Rigidbody");
                hinge.enabled = true;
                hinge.connectedBody = otherRb;
                hingedTo = collision.gameObject;
                hingedToOffset = Quaternion.Inverse(collision.gameObject.transform.rotation) * (collision.gameObject.transform.position - hingePoint);
                hinge.connectedAnchor = hingePoint - collision.gameObject.transform.position;
                carriedObject.AddCarrier(this);
                carriedObject.isCarried = true;
                state = AntState.DragToNest;
                targetFoodPos = carriedObject.originalPosition;
            }
            else
            {
                carriedObject = null;
            }
        }
    }

    public void CollisionStay(Collision2D collision)
    {
        GameObject collisionParent = collision.gameObject.transform.parent.gameObject;
        if (collisionParent != null
         && !collisionParent.CompareTag("Ant")
         && !collision.gameObject.CompareTag("Nest"))
        {
            collidingWith = collision.gameObject;
            isSlowedByCollision = true;
        }
    }

    public void SpotAnt(AntBehaviour other)
    {

        if (team == Team.Red || true) {
            if (state != AntState.Attack && other.team != team)
            {
                if (carriedObject)
                {
                    DropFood();
                }
                state = AntState.Attack;
                attackTarget = other;
            }
        }


    }

    private void Attack() {
        if (steps % 10 == 0) {
            Vector3 v = attackTarget.transform.position - transform.position;
            m = v.sqrMagnitude;
            //print(m);
            //print(advancing);
            print(v);
            v_n = v.normalized;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, v_n);

        }



        Vector3 nextPos = Vector3.negativeInfinity;

        if (m < ATTACK_POS_NEAR)
        {
            attackState = AttackState.SwipeOut;
        } else if (m > ATTACK_POS_FAR)
        {
            attackState = AttackState.CloseIn;
        }

        if (attackState == AttackState.SwipeOut)
        {
            nextPos = transform.position - v_n * SWIPE_OUT_SPEED;
            if (m > ATTACK_POS_MEDIUM)
            {
                attackState = AttackState.Wait;
            }
        }
        else if (attackState == AttackState.SwipeIn)
        {
            nextPos = transform.position + v_n * SWIPE_IN_SPEED;
            swipeTimer -= Time.deltaTime;
            if (swipeTimer <= 0)
            {
                attackState = AttackState.SwipeOut;
            }
        }
        else if (attackState == AttackState.Wait)
        {
            if (Random.value < SWIPE_CHANCE)
            {
                attackState = AttackState.SwipeIn;
                swipeTimer = SWIPE_DURATION;
            }
        } else if (attackState == AttackState.CloseIn)
        {
            nextPos = transform.position + v_n * CLOSE_IN_SPEED;
        }

        /*if (m > ATTACK_DISTANCE_MAX && !advancing)
        {
            nextPos = transform.position + v_n * ATTACK_MOVEMENT_SPEED;
            // too far away from target
            advancing = true;
        }
        else if (m < ATTACK_DISTANCE_MIN && advancing)
        {
            // to close to target
            nextPos = transform.position - v_n * ATTACK_MOVEMENT_SPEED;
            advancing = false;
        }*/

        /*if (advancing)
        {
            // advance towards target
            nextPos = transform.position + v_n * ATTACK_MOVEMENT_SPEED;
        }
        else
        {
            // retreat from target
            nextPos = transform.position - v_n * ATTACK_MOVEMENT_SPEED;
        }*/

        if (level.ContainsWithinBounds(nextPos) && !nextPos.Equals(Vector3.negativeInfinity))
        {
            transform.position = nextPos;
        }
    }

}
