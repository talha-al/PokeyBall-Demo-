using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kamera : MonoBehaviour
{
    public GameObject top;
    private Vector3 mesafe;

    void Start()
    {
        mesafe = transform.position - top.transform.position;
    }

    void Update()
    {
        transform.position = top.transform.position + mesafe;
    }
}
