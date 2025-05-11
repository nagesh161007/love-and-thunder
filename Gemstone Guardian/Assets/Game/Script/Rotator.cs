using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public float Speed = 80f;

    void Update()
    {
        transform.Rotate(new Vector3(0f, Speed * Time.deltaTime, 0f), Space.World);
    }
}
