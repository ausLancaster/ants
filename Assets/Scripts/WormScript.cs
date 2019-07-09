using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormScript : MonoBehaviour
{
    private const int PUSH_DURATION = 16;
    private const int WAITS_TO_PUSHES = 3;
    private const float PUSH_FORCE = 60;
    private const float MOV_ANGLE_OFFSET = 60f;

    [SerializeField]
    float minHeadTailSeparation = 4f;

    [SerializeField]
    private Rigidbody2D headRB;
    [SerializeField]
    private Rigidbody2D tailRB;
    [SerializeField]
    private Transform headTransform;
    [SerializeField]
    private Transform tailTransform;
    private List<Rigidbody2D> segmentRbs;
    private List<Transform> segmentTransforms;
    private Transform selectedTransform;
    private Rigidbody2D selectedRB;
    private static int steps;
    private float offsetAngle;

    // Start is called before the first frame update
    void Start()
    {
        steps = 0;
        segmentTransforms = new List<Transform>();
        segmentRbs = new List<Rigidbody2D>();
        foreach (Transform child in transform)
        {
            segmentTransforms.Add(child);
            segmentRbs.Add(child.GetComponent<Rigidbody2D>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        int instruction = (steps / PUSH_DURATION) % WAITS_TO_PUSHES;
        if (instruction == 0)
        {
            if (steps % PUSH_DURATION == 0)
            {
                // pick which segment to push with

                float headDist = GetDirectionToMouse(new Vector2(headTransform.position.x, headTransform.position.y), false).magnitude;
                float tailDist = GetDirectionToMouse(new Vector2(tailTransform.position.x, tailTransform.position.y), false).magnitude;

                if (tailDist - headDist < minHeadTailSeparation)
                {
                    // choose head
                    selectedTransform = headTransform;
                    selectedRB = headRB;
                }
                else
                {
                    // choose tail
                    selectedTransform = tailTransform;
                    selectedRB = tailRB;
                }

                offsetAngle = UnityEngine.Random.Range(-MOV_ANGLE_OFFSET, MOV_ANGLE_OFFSET);

            }


            // push chosen segment
            Vector2 dir = GetDirectionToMouse(new Vector2(selectedTransform.position.x, selectedTransform.position.y));
            dir = Vector2Extension.Rotate(dir, offsetAngle);
            selectedRB.AddForce((PUSH_FORCE * 0.5f) * dir);
        }



        /*if (instruction == 0)
        {
            // pick random segment
            int segmentId = UnityEngine.Random.Range(0, segmentTransforms.Count);
            Vector3 v = segmentTransforms[segmentId].position;
            Rigidbody2D rb = segmentRbs[segmentId];
            // push segment
            Vector2 dir = GetDirectionToMouse(new Vector2(v.x, v.y));
            rb.AddForce(PUSH_FORCE * dir);
        }*/



        steps++;
    }
    
    private Vector2 GetDirectionToMouse(Vector2 from, bool normalized = true)
    {
        Vector3 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 v2 = new Vector2(v.x, v.y) - from;
        if (normalized)
        {
            v2 = v2.normalized;
        }
        return v2;

        //return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
