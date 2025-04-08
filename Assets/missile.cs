using System;
using UnityEngine;

public class missile : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject target;
    [SerializeField] private Rigidbody2D rb2d;
    [SerializeField] private float speed;
    // Update is called once per frame
    void FixedUpdate()
    {
        float angle = Mathf.Atan2(target.transform.position.y - transform.position.y,
            target.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0,angle);
        rb2d.linearVelocityX = transform.right.x * speed;
        rb2d.linearVelocityY = transform.right.y * speed;
        if (target ==null)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Enemy"))
        {
            other.gameObject.GetComponent<enemyManger>().damage.Invoke(25f);
            Destroy(gameObject);
        }
    }
}
