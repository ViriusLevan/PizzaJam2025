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
    private float mag = 1;
    [SerializeField] private Collider2D c;
    private bool forced = false;
    private float dist;
    private Vector2 Direction;
    
    [SerializeField] private SpriteRenderer sr;
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
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        em.health -= em.p;
        if (!em.kb)
        {
            if (s > 0.1)
            {
                s *= 0.6f;
                rb2d.linearVelocity = new Vector2(Mathf.Cos(angle) * s, Mathf.Sin(angle) * s);
                isAttacking = true;
            }
            else
            {
                s = 0;
                rb2d.linearVelocity = new Vector2(Mathf.Cos(angle) * s, Mathf.Sin(angle) * s);
                isAttacking = false;
                StartCoroutine(Wait());
                angle = Mathf.Atan2((player.position.y - transform.position.y),
                    (player.position.x - transform.position.x));
            }
        }
        else
        {
            // if (!forced)
            // {
            //     //Debug.Log( (4 -Vector3.Distance(player.position, transform.position)) * new Vector2(player.position.x - transform.position.x, player.position.y - transform.position.y) * 25f);
            //     forced = true;
            //     rb2d.linearVelocity = (4 -Vector3.Distance(player.position, transform.position)) * new Vector2(player.position.x - transform.position.x, player.position.y - transform.position.y) * 25f;
            // }else if (rb2d.linearVelocity.x < 0.2 && rb2d.linearVelocity.y <0.2)
            // {
            //     //em.kb = false;
            // }
            // Debug.Log("V");
            // Debug.Log(rb2d.linearVelocity);
            //rb2d.linearVelocity *= 0.99f;
            // rb2d.linearVelocity = (4 -Vector3.Distance(player.position, transform.position)) * new Vector2(player.position.x - transform.position.x, player.position.y - transform.position.y) * 25f;
            // mag *= 0.99f;
            // rb2d.linearVelocity *= mag;
            if (!forced)
            {
                Direction =
                    new Vector2(transform.position.x - player.position.x, transform.position.y - player.position.y)
                        .normalized;
                dist = 4 - Vector3.Distance(transform.position, player.position); // Change 4 to detection range
                dist = Mathf.Clamp(dist, 0.2f, 4f);
                forced = true;
            }
            
            rb2d.linearVelocity = 35 * dist * mag * Direction;
            mag *= 0.89f;

            if (mag < 0.3)
            {
                mag = 1;
                forced = false;
                em.kb = false;
            }

        }
    }
    
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(4f);
        if (!em.kb)
        {
            s = Speed;
        }
        
        
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag.Equals("Wall"))
        {
            s = 0;
        }
    }

    public void dmg(float d)
    {
        if (!sr.color.Equals(Color.red))
        {
            em.health -= d;
            StartCoroutine(idk());
        }

        
    }

    IEnumerator idk()
    {
        sr.color = Color.red;
        //c.enabled = false;
        float timer = 0f;
        float duration = 0.5f;
        
        while (timer < duration)
        {
            // Check if object is moving
            if (s > 0.1f)
            {
                Debug.Log("Interrupted by movement");
                break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        //c.enabled = true;
        sr.color = Color.white;
    }
}
