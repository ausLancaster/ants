using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vision : MonoBehaviour
{
    private AntBehaviour ant;

    private void Start()
    {
        ant = GetComponentInParent<AntBehaviour>();
    }

    private void OnTriggerEnter(Collider other)
    {
        print("seen");
        if (other.gameObject.transform.parent.CompareTag("Ant")) {
            AntBehaviour otherAnt = other.gameObject.transform.parent.GetComponent<AntBehaviour>();
            ant.SpotAnt(otherAnt);
        }
    }
}
