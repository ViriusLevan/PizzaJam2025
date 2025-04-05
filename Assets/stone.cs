using UnityEngine;

public class stone : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb2d;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb2d.AddForce(transform.up * 30 ,ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
