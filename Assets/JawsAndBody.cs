using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JawsAndBody : MonoBehaviour
{
    [SerializeField]
    AntBehaviour ant;

    private void OnTriggerEnter2D(Collider2D other)
    {
        ant.Trigger(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ant.Collision(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        ant.CollisionStay(collision);
    }

}
