using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = System.Random;

enum Ability 
{
    Clone ,
    Card_Summon,
    Fire_Line,
    
    Shadow_Warp ,
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
    Ability Left = Ability.Clone; // Faces
    Ability Right = Ability.Stone_Shurricane;
    Ability Top = Ability.Dash;
    Ability Bottom = Ability.Empty;
    Ability Forward = Ability.Fire_Line;
    Ability Backwards = Ability.Shadow_Warp;


    public LayerMask obstacleMask;

    [SerializeField] private GameObject fireLine, stone;

    public bool canClone = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private bool ableToMove = true;
    // Update is called once per frame

    private bool forwardMovementPossible, backMovementPossible, rightMovementPossible, leftMovementPossible;
    
    void Update()
    {
        //Vector2 direction = transform.up;

        // Draw ray in Scene view
        //

        if (!ableToMove) return;

        // Do the raycast
        //Debug.DrawRay(transform.position, transform.up * rayLength, Color.green);
        
        if (Keyboard.current.wKey.wasPressedThisFrame && forwardMovementPossible)
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
            TriggerAnimation(RollDirection.forward);
        }
        else if (Keyboard.current.sKey.wasPressedThisFrame && backMovementPossible)
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
            TriggerAnimation(RollDirection.backward);
        }
        else if (Keyboard.current.aKey.wasPressedThisFrame && leftMovementPossible)
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
            TriggerAnimation(RollDirection.left);
        }
        else if (Keyboard.current.dKey.wasPressedThisFrame && rightMovementPossible)
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
            TriggerAnimation(RollDirection.right);
        }
    }

    private void UpdatePossibleDirections()
    {
        float rayLength = GetComponent<BoxCollider2D>().size.y;
        float rayLengthR = GetComponent<BoxCollider2D>().size.x;
        forwardMovementPossible = Physics2D.Raycast(transform.position, transform.up, rayLength, obstacleMask).collider==null;
        backMovementPossible = Physics2D.Raycast(transform.position, transform.up*-1, rayLength, obstacleMask).collider==null;
        
        rightMovementPossible = Physics2D.Raycast(transform.position, transform.right, rayLengthR, obstacleMask).collider==null;
        leftMovementPossible = Physics2D.Raycast(transform.position, transform.right*-1, rayLengthR, obstacleMask).collider==null;
    }

    public void FixedUpdate()
    {
        UpdatePossibleDirections();
        //rb2d.AddForce(new Vector2(horizontalInput * speed, verticalInput*speed ), ForceMode2D.Impulse);
        // targetPos.y += verticalInput * gridSize;
        // targetPos.x += horizontalInput * gridSize;

        if (!transform.position.y.Equals(targetPos.y))
        {
            float diff = targetPos.y - transform.position.y;
            
            //target pos not possible to reach
            if ((diff > 0 && !forwardMovementPossible) || (diff < 0 && !backMovementPossible))
            {
                ableToMove = true;
                targetPos = transform.position;
                return;
            }

            transform.position = new Vector2(transform.position.x, transform.position.y + (diff * 0.25f));
            if (Mathf.Abs(diff) < 0.1)
            {
                transform.position = new Vector2(transform.position.x, targetPos.y);
                ableToMove = true;
                if (diff > 0)
                {
                    UseAbility(Bottom, 'U');
                }
                else
                {
                    UseAbility(Bottom, 'D');
                }
            }
        }
        else if (!transform.position.x.Equals(targetPos.x))
        {

            float diff = targetPos.x - transform.position.x;

            //target pos not possible to reach
            if ((diff > 0 && !rightMovementPossible) || (diff < 0 && !leftMovementPossible))
            {
                ableToMove = true;
                targetPos = transform.position;
                return;
            }

            transform.position = new Vector2(transform.position.x + (diff * 0.25f), transform.position.y);
            
            if (Mathf.Abs(diff) < 0.1)
            {
                transform.position = new Vector2(targetPos.x, transform.position.y);
                ableToMove = true;
                if (diff > 0)
                {
                    UseAbility(Bottom, 'R');
                }
                else
                {
                    UseAbility(Bottom, 'L');

                }
            }
        }
    }
    
    
    private void UseAbility(Ability a, char md)
    {
        switch (a)
        {
            case Ability.Empty:
                break;
            case Ability.Dash:
                if (md == 'U')
                {
                    targetPos.y += gridSize * (1 + 1);
                    TriggerAnimation(RollDirection.forward);
                }
                else if (md == 'D')
                {
                    targetPos.y -= gridSize * (1 + 1);
                    TriggerAnimation(RollDirection.backward);
                }
                else if (md == 'L')
                {
                    targetPos.x -= gridSize * (1 + 1);
                    TriggerAnimation(RollDirection.left);
                }
                else if (md == 'R')
                {
                    targetPos.x += gridSize * (1 + 1);
                    TriggerAnimation(RollDirection.right);
                }
                ableToMove = false;
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
        }
    }
    
    
        
    [SerializeField] private Animator playerSprite;

    public enum RollDirection
    {
        forward,backward,left,right
    }

    private void TriggerAnimation(RollDirection rd)
    {
        switch (rd)
        {
            case RollDirection.forward:
                playerSprite.SetTrigger("forward");
                break;
            case RollDirection.backward:
                playerSprite.SetTrigger("backward");
                break;
            case RollDirection.left:
                playerSprite.SetTrigger("left");
                break;
            case RollDirection.right:
                playerSprite.SetTrigger("right");
                break;
        }
    }
}
