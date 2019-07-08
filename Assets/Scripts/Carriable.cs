using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carriable : MonoBehaviour
{
    public bool isCarried { get; set; }
    public List<AntBehaviour> carriers { get; set; }

    private void Awake()
    {
        carriers = new List<AntBehaviour>();
    }

    public void AddCarrier(AntBehaviour carrier)
    {
        carriers.Add(carrier);
    }
    public void RemoveFoodFromCarriers(AntBehaviour except=null)
    {
        foreach (AntBehaviour c in carriers)
        {
            if (!(c == except))
            {
                c.LoseFood();
            }
        }
        carriers.RemoveAll(x => x != except);
    }

    private bool NotMatch(AntBehaviour match, AntBehaviour input)
    {
        return !(input == match);
    }

}