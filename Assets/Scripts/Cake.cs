using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cake : MonoBehaviour
{
    public int cakeNumber = 0;
    public bool isGound = false;

    Rigidbody2D rb2D;
    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }
    public void Drop()
    {
        rb2D.simulated = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
            return;

        if (!isGound)
            isGound = true;
    }

    public void Setup(bool isGround)
    {
        if(isGround)
        {
            rb2D.simulated = true;
        }
    }
}
