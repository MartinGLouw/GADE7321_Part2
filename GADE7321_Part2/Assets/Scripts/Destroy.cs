
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    //destroy the oject after 2 seconds
    void Start()
    {
        Destroy(gameObject, 2);
    }
}
