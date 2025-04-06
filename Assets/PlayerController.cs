using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Android;
using Random = System.Random;

enum Ability 
{
    Clone ,
    Card_Summon,
    Fire_Line,
    Shadow_Warp,
    Dash,
    Life_Drain,
    Stone_Shurricane ,
    KnockBack,
    Toxic_Waste,
    Homing_Missile,
    Empty
    
}

public class PlayerController : MonoBehaviour
{
    private float horizontalInput;
    private float verticalInput;
    [SerializeField] private float speed;
    public Vector2 targetPos;

    [SerializeField] public float gridSize;

    //[SerializeField] private Rigidbody2D rb2d;
    Ability Left = Ability.KnockBack; // Faces
    Ability Right = Ability.Stone_Shurricane;
    Ability Top = Ability.KnockBack;
    Ability Bottom = Ability.KnockBack;
    Ability Forward = Ability.KnockBack;
    Ability Backwards = Ability.Fire_Line;


    public LayerMask obstacleMask;

    private bool b = true;
    [SerializeField] private GameObject fireLine, stone;

    public bool canClone = true;

    public float health = 100f;
    [SerializeField] private Collider2D c;
    [SerializeField] public SpriteRenderer sr;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Vector2 direction = transform.up;

        // Draw ray in Scene view
        //

        // Do the raycast
        b = true;
        while (b)
        {
            float rayLength = targetPos.y - transform.position.y;
            float rayLengthR = targetPos.x - transform.position.x;
            //Debug.DrawRay(transform.position, transform.up * rayLength, Color.green);
            RaycastHit2D hitUp = Physics2D.Raycast(transform.position, transform.up, rayLength, obstacleMask);
            RaycastHit2D hitRight = Physics2D.Raycast(transform.position, transform.right, rayLengthR, obstacleMask);
            if (hitUp.collider != null && hitUp.collider.gameObject != gameObject)
            {
                if (rayLength > 0)
                {
                    targetPos.y -= gridSize;
                }
                else if (rayLength < 0)
                {
                    targetPos.y += gridSize;
                }
                else
                {
                    b = false;
                }
            }
            else if (hitRight.collider != null && hitRight.collider.gameObject != gameObject)
            {
                if (rayLengthR > 0)
                {
                    targetPos.x -= gridSize;
                }
                else if (rayLengthR < 0)
                {
                    targetPos.x += gridSize;
                }
                else
                {
                    b = false;
                }
            }
            else
            {
                b = false;
            }
        }


        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            targetPos.y += gridSize;
            Ability f = Forward; // temp
            Ability b = Backwards;
            Ability bb = Bottom;
            Ability t = Top;
            Top = b; //Swaps
            Forward = t;
            Bottom = f;
            Backwards = bb;
            UseAblity(Bottom, 'U');
        }
        else if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            targetPos.y -= gridSize;
            Ability f = Forward; // temp
            Ability b = Backwards;
            Ability bb = Bottom;
            Ability t = Top;
            Top = f; //Swaps
            Forward = bb;
            Bottom = b;
            Backwards = t;
            UseAblity(Bottom, 'D');
        }
        else if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            targetPos.x -= gridSize;
            Ability l = Left;
            Ability r = Right;
            Ability t = Top;
            Ability b = Bottom;

            Top = r;
            Right = b;
            Bottom = l;
            Left = t;
            UseAblity(Bottom, 'L');
        }
        else if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            targetPos.x += gridSize;
            Ability l = Left;
            Ability r = Right;
            Ability t = Top;
            Ability b = Bottom;

            Top = l;
            Right = t;
            Bottom = r;
            Left = b;
            UseAblity(Bottom, 'R');
        }
    }

    public void FixedUpdate()
    {
        //rb2d.AddForce(new Vector2(horizontalInput * speed, verticalInput*speed ), ForceMode2D.Impulse);
        // targetPos.y += verticalInput * gridSize;
        // targetPos.x += horizontalInput * gridSize;
        if (!transform.position.y.Equals(targetPos.y))
        {
            float diff = targetPos.y - transform.position.y;
            transform.position = new Vector2(transform.position.x, transform.position.y + (diff * 0.25f));
            if (Mathf.Abs(diff) < 0.1)
            {
                transform.position = new Vector2(transform.position.x, targetPos.y);
                if (diff > 0)
                {
                    // UseAblity(Bottom, 'U');
                }
                else
                {
                    // UseAblity(Bottom, 'D');
                }

            }
        }
        else if (!transform.position.x.Equals(targetPos.x))
        {
            float diff = targetPos.x - transform.position.x;
            transform.position = new Vector2(transform.position.x + (diff * 0.25f), transform.position.y);
            if (Mathf.Abs(diff) < 0.1)
            {
                transform.position = new Vector2(targetPos.x, transform.position.y);
                if (diff > 0)
                {
                    // UseAblity(Bottom, 'R');
                }
                else
                {
                    // UseAblity(Bottom, 'L');

                }

            }
        }


       

        

    }
    void UseAblity(Ability a, char md)
        {
            switch (a)
            {
                case Ability.Empty:
                    break;
                case Ability.Dash:
                    if (md == 'U')
                    {
                        targetPos.y += gridSize * (1 + 1);
                    }
                    else if (md == 'D')
                    {
                        targetPos.y -= gridSize * (1 + 1);
                    }
                    else if (md == 'L')
                    {
                        targetPos.x -= gridSize * (1 + 1);
                    }
                    else if (md == 'R')
                    {
                        targetPos.x += gridSize * (1 + 1);
                    }
                    Debug.Log("Dash");
                    break;
                case Ability.Fire_Line:
                    fireLine.SetActive(true);
                    fireLine.GetComponent<fire>().bd = md;
                    break;
                case Ability.Shadow_Warp:
                    transform.position = new Vector3(gridSize * UnityEngine.Random.Range(-3, 3),
                        gridSize *  UnityEngine.Random.Range(-3, 3), 0);
                    targetPos.x = transform.position.x;
                    targetPos.y = transform.position.y;
                    break;
                case Ability.Stone_Shurricane:
                    Instantiate(stone, transform.position, Quaternion.Euler(0, 0, 0));
                    Instantiate(stone, transform.position, Quaternion.Euler(0, 0, 90));
                    Instantiate(stone, transform.position, Quaternion.Euler(0, 0, -90));
                    Instantiate(stone, transform.position, Quaternion.Euler(0, 0, 180));
                    break;
                case Ability.Clone:
                    if (canClone)
                    {
                        GameObject obj = Instantiate(gameObject, 
                            new Vector3(transform.position.x - 3, transform.position.y, 0), Quaternion.Euler(0, 0, 0));
                        PlayerController pc = obj.GetComponent<PlayerController>();
                        pc.targetPos.x = obj.transform.position.x;
                        pc.targetPos.y = obj.transform.position.y;
                        pc.canClone = false;
                        canClone = false;
                    }

                    break;
                case Ability.KnockBack:
                    Debug.Log("kb");
                    GameObject[] e = GameObject.FindGameObjectsWithTag("Enemy");
                    
                    foreach (GameObject g in e)
                    {
                        if (Vector3.Distance(transform.position, g.transform.position) < 4)
                        {
                            g.GetComponent<enemyManger>().kb = true;
                        }
                    }
                    break;
            }
        }

    private void OnCollisionEnter2D(Collision2D other)
    {
        
        if (other.gameObject.tag.Equals("Enemy"))
        {
            Debug.Log(other.gameObject.name);
            enemyManger em = other.gameObject.GetComponent<enemyManger>();
            if (!em.isAttacking)
            {
                em.health -= 10;
            }
            else
            {
                health -= em.dmg;
                sr.color = Color.red;
                StartCoroutine(dmg());
            }
        }
    }

    IEnumerator dmg()
    {
        c.enabled = false;
        yield return new WaitForSeconds(1f);
        c.enabled = true;
        sr.color = Color.white;
    }
}
