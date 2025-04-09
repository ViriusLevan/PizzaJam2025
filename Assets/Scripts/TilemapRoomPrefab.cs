using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapRoomPrefab : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] public Transform upAnchor, downAnchor, leftAnchor, rightAnchor;

    //[SerializeField] public Vector3Int upPoint, downPoint, leftPoint, rightPoint;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(_tilemap==null)
            _tilemap = GetComponent<Tilemap>();

        // var upPosition = upAnchor.transform.position;
        // upPoint = new Vector3Int((int) upPosition.x
        //     ,(int) upPosition.y
        //     ,(int) upPosition.z);
        // var downPosition = downAnchor.transform.position;
        // downPoint = new Vector3Int((int) downPosition.x
        //     ,(int) downPosition.y
        //     ,(int) downPosition.z);
        // var leftPosition = leftAnchor.transform.position;
        // leftPoint = new Vector3Int((int) leftPosition.x
        //     ,(int) leftPosition.y
        //     ,(int) leftPosition.z);
        // var rightPosition = rightAnchor.transform.position;
        // rightPoint = new Vector3Int((int) rightPosition.x
        //     ,(int) rightPosition.y
        //     ,(int) rightPosition.z);
    }
    
    
    
    Vector3Int GetWorldAnchor(Vector3Int roomOrigin, Vector3Int localAnchor)
    {
        return roomOrigin + localAnchor;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
