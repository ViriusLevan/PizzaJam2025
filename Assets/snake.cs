using System.Collections;
using UnityEngine;

public class snake : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb2d;

    [SerializeField] private float speed;
    [SerializeField] private Transform player;
    private Vector2 wander;

    private Vector3 lastPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // wander = new Vector2(transform.position.x + UnityEngine.Random.Range(-5f, 5f),
        //     transform.position.y + UnityEngine.Random.Range(-5f, 5f));
        // transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(wander.y, wander.x)));
        
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, UnityEngine.Random.Range(0f,360f)));
        StartCoroutine(idk());
    }

    // Update is called once per frame
    void Update()
    {
        rb2d.linearVelocityX = transform.up.x * speed;
        rb2d.linearVelocityY = transform.up.y * speed;
        // Debug.Log("x");
        // Debug.Log(Mathf.Abs(transform.position.x - wander.x));
        // Debug.Log("y");
        // Debug.Log(Mathf.Abs(transform.position.y - wander.y));
        // if ((Mathf.Abs(transform.position.x - wander.x) < 0.5 || Mathf.Abs(transform.position.y - wander.y) < 0.5))
        //     //(lastPos == transform.position))
        // {
        //     wander = new Vector2(transform.position.x + UnityEngine.Random.Range(-5f, 5f),
        //         transform.position.y + UnityEngine.Random.Range(-5f, 5f));
        //     
        //     Debug.Log("x");
        //     Debug.Log(Mathf.Abs(transform.position.x - wander.x));
        //     Debug.Log("y");
        //     Debug.Log(Mathf.Abs(transform.position.y - wander.y));
        //     Debug.Log(lastPos);
        // }
        //
        // lastPos = transform.position;
        //Debug.Log();
        if (Vector2.Distance(transform.position, player.position) < 4f)
        {
            Debug.Log("PP");
            transform.rotation = Quaternion.Euler(new Vector3(0, 0,180 + Mathf.Atan2((player.position.y -transform.position.y),( player.position.x - transform.position.x) )));
        }
    }

    IEnumerator idk()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(2f,10f));
        //transform.rotation = Quaternion.Euler(new Vector3(0, 0, UnityEngine.Random.Range(0f,360f)));
        StartCoroutine(idk());
    }
}
