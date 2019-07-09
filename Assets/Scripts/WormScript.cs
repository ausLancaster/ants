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
    private List<Rigidbody2D> segmentRbs;
    private List<Transform> segmentTransforms;
    private static int steps;

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
        int instruction = (steps / PUSH_DURATION) % (WAITS_TO_PUSHES);
        if (instruction == 0)
        {
            // pick random segment
            int segmentId = UnityEngine.Random.Range(0, segmentTransforms.Count);
            Vector3 v = segmentTransforms[segmentId].position;
            Rigidbody2D rb = segmentRbs[segmentId];
            // push segment
            Vector2 dir = GetDirectionToMouse(new Vector2(v.x, v.y));
            rb.AddForce(PUSH_FORCE * dir);
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
