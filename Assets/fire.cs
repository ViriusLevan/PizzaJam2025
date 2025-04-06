using System.Collections;
using UnityEngine;

public class fire : MonoBehaviour
{
    public char bd;

    [SerializeField] private Vector2 Up, Down, Left, Right;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameObject.activeSelf) return;
        
        if (bd == 'U')
        {
            transform.localPosition = new Vector3(Up.x,Up.y,0);
            StartCoroutine(Wait());
        }
        else if(bd == 'D')
        {
            transform.localPosition = new Vector3(Down.x,Down.y,0);
            StartCoroutine(Wait());
        }
        else if(bd == 'L')
        {
            transform.localPosition = new Vector3(Left.x,Left.y,0);
            StartCoroutine(Wait());
        }
        else if(bd == 'R')
        {
            transform.localPosition = new Vector3(Right.x,Right.y,0);
            StartCoroutine(Wait());
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.2f);
        gameObject.SetActive(false);
    }
}
