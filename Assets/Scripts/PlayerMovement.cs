using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private readonly float RUN_SPEED = 0.02f;
    private readonly float TURN_SPEED = 8f;

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.rotation * (RUN_SPEED * Vector3.up);
        }
        if (Input.GetKey(KeyCode.S)) {
            transform.position += transform.rotation * (RUN_SPEED * Vector3.up);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * TURN_SPEED, Space.Self);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * TURN_SPEED, Space.Self);
        }
    }
}
