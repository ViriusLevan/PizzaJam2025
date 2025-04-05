using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private float horizontalInput;
    private float verticalInput;
    [SerializeField] private float speed;
    public Vector2 targetPos;
    [SerializeField] public float gridSize;
    //[SerializeField] private Rigidbody2D rb2d;
    public int Left = 3; // Faces
    public int Right = 2;
    public int Top = 1;
    public int Bottom = 4;
    public int Forward = 5;
    public int Backwards = 6;

     
    public LayerMask obstacleMask;

    private bool b = true;
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
                 else if(rayLength < 0)
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
                 else if(rayLengthR < 0)
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
            int f = Forward; // temp
            int b = Backwards;
            int bb = Bottom;
            int t = Top;
            Top = b; //Swaps
            Forward = t;
            Bottom = f;
            Backwards = bb;
        }
        else if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            targetPos.y -= gridSize;
            int f = Forward; // temp
            int b = Backwards;
            int bb = Bottom;
            int t = Top;
            Top = f; //Swaps
            Forward = bb;
            Bottom = b;
            Backwards = t;
        }
        else if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            targetPos.x -= gridSize;
            int l = Left;
            int r = Right;
            int t = Top;
            int b = Bottom;

            Top = r;
            Right = b;
            Bottom = l;
            Left = t;
        }
        else if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            targetPos.x += gridSize;
            int l = Left;
            int r = Right;
            int t = Top;
            int b = Bottom;

            Top = l;
            Right = t;
            Bottom = r;
            Left = b;
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
            transform.position = new Vector2(transform.position.x , transform.position.y + (diff * 0.25f));
            if (Mathf.Abs(diff) < 0.1)
            {
                transform.position = new Vector2(transform.position.x, targetPos.y);
            }
        }
        else if (!transform.position.x.Equals(targetPos.x))
        {
            float diff = targetPos.x - transform.position.x;
            transform.position = new Vector2(transform.position.x + (diff * 0.25f), transform.position.y );
            if (Mathf.Abs(diff)  < 0.1)
            {
                transform.position = new Vector2(targetPos.x, transform.position.y);
            }
        }
    }

    void OnMove(InputValue value)
        {
            Vector2 input = value.Get<Vector2>();
            horizontalInput = -input.x;
            verticalInput = input.y;
            
        }
}
