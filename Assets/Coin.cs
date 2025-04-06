using System;
using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb2d;
    [SerializeField] private float Speed;
    private float s;
    [SerializeField] private Transform player;
    public bool isAttacking = false;
    private float angle = 45;
    public float health = 100;
    [SerializeField] private enemyManger em;

    private bool forced = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        s = Speed;
        em.dmg = 25;
        rb2d.linearVelocity = new Vector2(Mathf.Cos(angle) * Speed, Mathf.Sin(angle) * Speed);
    }

    private void Update()
    {
        em.isAttacking = isAttacking;
        health = em.health;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!em.kb)
        {
            // if (s > 0.1)
            // {
            //     s *= 0.6f;
            //     rb2d.linearVelocity = new Vector2(Mathf.Cos(angle) * s, Mathf.Sin(angle) * s);
            //     isAttacking = true;
            // }
            // else
            // {
            //     s = 0;
            //     rb2d.linearVelocity = new Vector2(Mathf.Cos(angle) * s, Mathf.Sin(angle) * s);
            //     isAttacking = false;
            //     StartCoroutine(Wait());
            //     angle = Mathf.Atan2((player.position.y - transform.position.y),
            //         (player.position.x - transform.position.x));
            // }
        }
        else
        {
            if (!forced)
            {
                Debug.Log((4 -Vector3.Distance(player.position, transform.position)) * new Vector2(player.position.x - transform.position.x, player.position.y - transform.position.y) * 10f);
                forced = true;
                rb2d.AddForce((4 -Vector3.Distance(player.position, transform.position)) * new Vector2(player.position.x - transform.position.x, player.position.y - transform.position.y) * 10f, ForceMode2D.Impulse);
            }else if (rb2d.linearVelocity.x < 0.2 && rb2d.linearVelocity.y <0.2)
            {
                //em.kb = false;
            }

            //rb2d.linearVelocity *= 0.4f;
            


        }
    }
    
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(4f);
        s = Speed;
        
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag.Equals("Wall"))
        {
            s = 0;
        }
    }

   
}
