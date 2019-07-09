using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormScript : MonoBehaviour
{
    private const int PUSH_DURATION = 16;
    private const int WAITS_TO_PUSHES = 3;
    private const float PUSH_FORCE = 60;

    [SerializeField]
    private Rigidbody2D headRB;
    [SerializeField]
    private Rigidbody2D tailRB;
    [SerializeField]
    private Transform headTransform;
    [SerializeField]
    private Transform tailTransform;
    private static int steps;

    // Start is called before the first frame update
    void Start()
    {
        steps = 0;
    }

    // Update is called once per frame
    void Update()
    {
        int instruction = (steps / PUSH_DURATION) % (WAITS_TO_PUSHES * 2);
        if (instruction == 0)
        {
            Vector2 dir = GetDirectionToMouse(new Vector2(tailTransform.position.x, tailTransform.position.y));
            // push tail
            tailRB.AddForce((PUSH_FORCE * 0.5f) * dir);
        }
        else if (instruction == WAITS_TO_PUSHES)
        {
            Vector2 dir = GetDirectionToMouse(new Vector2(headTransform.position.x, headTransform.position.y));
            // push tail
            headRB.AddForce(PUSH_FORCE * dir);
        }


        steps++;
    }

    private Vector2 GetDirectionToMouse(Vector2 from)
    {
        Vector3 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return (new Vector2(v.x, v.y) - from).normalized;

        //return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
