using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cake : MonoBehaviour
{

    public bool isGound = false;

    Rigidbody2D rb2D;
    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }
    public void Drop()
    {
        rb2D.simulated = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isGound)
            isGound = true;
    }
}
