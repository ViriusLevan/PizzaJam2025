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
    }
}
