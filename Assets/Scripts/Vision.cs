using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vision : MonoBehaviour
{
    [SerializeField]
    AntBehaviour ant;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ant")) {
            print("seen");
        }
    }
}
