using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class ProceduralGenerationManager : MonoBehaviour
{
    
    public static ProceduralGenerationManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.Log("[Singleton] Tried to instantiate a second instance of ProceduralGenerationManager.");
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    //[SerializeField] private GameObject[] roomPrefabs;
    [SerializeField] private GameObject tilemapParent;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //TestGeneration();
        //DrunkardsWalk();
        BinarySpacePartition();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public int width = 64;
    public int height = 64;
    public int minRoomSize = 16;

    private List<RectInt> rooms;
    private List<RectInt> leaves;

    private void BinarySpacePartition()
    {
        rooms = new List<RectInt>();
        leaves = new List<RectInt>();
        RectInt root = new RectInt(0, 0, width, height);
        instancedPrefabs = new Dictionary<RectInt, GameObject>();
        bspNodes = new List<BSPNode>();

        BSPNode rootNode = new BSPNode(root);
        
        
        SplitSpace(rootNode);
        CreateRooms(bspNodes[0]);
        PlaceRoomPrefabs(bspNodes[0]);

        ConnectRooms(bspNodes[0]);

        // Just to visualize in the console
        // foreach (RectInt room in rooms)
        // {
        //     Debug.Log("Room at: " + room);
        // }
        
        foreach (var node  in bspNodes)
        {
            if (node.room != null)
            {
                Debug.Log($"{node.tilemap.gameObject.name} at {node.room}");
            }
        }
    }

    [SerializeField] private List<BSPNode> bspNodes;

    private void SplitSpace(BSPNode node)
    {
        if (node.space.width < minRoomSize * 2 && node.space.height < minRoomSize * 2)
            return;

        bool splitHorizontally = node.space.width > node.space.height;

        if (splitHorizontally)
        {
            int splitX = Random.Range(minRoomSize, node.space.width - minRoomSize);
            RectInt leftRect = new RectInt(node.space.x, node.space.y, splitX, node.space.height);
            RectInt rightRect = new RectInt(node.space.x + splitX, node.space.y, node.space.width - splitX, node.space.height);

            node.left = new BSPNode(leftRect);
            node.right = new BSPNode(rightRect);
        }
        else
        {
            int splitY = Random.Range(minRoomSize, node.space.height - minRoomSize);
            RectInt bottomRect = new RectInt(node.space.x, node.space.y, node.space.width, splitY);
            RectInt topRect = new RectInt(node.space.x, node.space.y + splitY, node.space.width, node.space.height - splitY);

            node.left = new BSPNode(bottomRect);
            node.right = new BSPNode(topRect);
        }

        bspNodes.Add(node.left);
        bspNodes.Add(node.right);
        // Recurse!
        SplitSpace(node.left);
        SplitSpace(node.right);
        
        // if (space.width < minRoomSize * 2 && space.height < minRoomSize * 2)
        // {
        //     leaves.Add(space);
        //     return;
        // }
        //
        // bool splitHorizontally = (space.width > space.height);
        //
        // if (space.width >= space.height && space.width > minRoomSize * 2)
        //     splitHorizontally = true;
        // else if (space.height > minRoomSize * 2)
        //     splitHorizontally = false;
        //
        // if (splitHorizontally)
        // {
        //     int splitX = Random.Range(minRoomSize, space.width - minRoomSize);
        //     RectInt left = new RectInt(space.x, space.y, splitX, space.height);
        //     RectInt right = new RectInt(space.x + splitX, space.y, space.width - splitX, space.height);
        //     SplitSpace(left);
        //     SplitSpace(right);
        // }
        // else
        // {
        //     int splitY = Random.Range(minRoomSize, space.height - minRoomSize);
        //     RectInt bottom = new RectInt(space.x, space.y, space.width, splitY);
        //     RectInt top = new RectInt(space.x, space.y + splitY, space.width, space.height - splitY);
        //     SplitSpace(bottom);
        //     SplitSpace(top);
        // }
    }

    private void OnDrawGizmos()
    {
        if (rooms == null) return;
        if (rooms.Count < 1) return;
            
        foreach (var node  in bspNodes)
        {
            if (node.room != null && node.roomCenter!=null)
                Gizmos.DrawWireCube(new Vector3(node.roomCenter.Value.x, node.roomCenter.Value.y),
                    new Vector3(node.room.Value.width, node.room.Value.height, 1));
        }
    }

    private void CreateRooms(BSPNode node)
    {
        if (node.IsLeaf)
        {
            // Place a room inside this leaf
            int roomWidth = Random.Range(8, node.space.width - 2);
            int roomHeight = Random.Range(8, node.space.height - 2);
            int roomX = Random.Range(node.space.x + 1, node.space.xMax - roomWidth - 1);
            int roomY = Random.Range(node.space.y + 1, node.space.yMax - roomHeight - 1);

            RectInt instRoom = new RectInt(roomX, roomY, roomWidth, roomHeight);
            rooms.Add(instRoom);
            node.room = rooms[^1];
            node.roomCenter = new Vector2Int(roomX + roomWidth / 2, roomY + roomHeight / 2);
            
            //Gizmos.DrawWireCube(new Vector3( node.roomCenter.Value.x, node.roomCenter.Value.y), new Vector3(roomWidth, roomHeight, 1));

        }
        else
        {
            CreateRooms(node.left);
            CreateRooms(node.right);
        }

        // foreach (var leaf in leaves)
        // {
        //     int roomWidth = Random.Range(minRoomSize, leaf.width - 2);
        //     int roomHeight = Random.Range(minRoomSize, leaf.height - 2);
        //     int roomX = Random.Range(leaf.x + 1, leaf.xMax - roomWidth - 1);
        //     int roomY = Random.Range(leaf.y + 1, leaf.yMax - roomHeight - 1);
        //     RectInt room = new RectInt(roomX, roomY, roomWidth, roomHeight);
        //     rooms.Add(room);
        //
        //     BSPNode node = new BSPNode();
        //     node.room = new RectInt(room.xMin, room.yMin, roomWidth, roomHeight);
        //     node.roomCenter = new Vector2Int(room.x + room.width / 2, room.y + room.height / 2);
        //     
        //     bspNodes.Add(node);
        // }
    }

    public class BSPNode
    {
        public RectInt space;         // The full space of this node
        public RectInt? room;         // The room created inside this space (if it's a leaf)
        public Vector2Int? roomCenter;
        public Tilemap tilemap; 
        public TilemapRoomPrefab roomData;

        public BSPNode left;
        public BSPNode right;

        public BSPNode(RectInt space)
        {
            this.space = space;
        }

        public bool IsLeaf => left == null && right == null;
    }

    [SerializeField] private Tilemap corridorTMap;
    
    // private void ConnectRooms(BSPNode node)
    // {
    //     if (node.left != null && node.right != null)
    //     {
    //         Vector2Int? leftCenter = GetRoomCenter(node.left);
    //         Vector2Int? rightCenter = GetRoomCenter(node.right);
    //
    //         Tilemap leftTilemap = GetLeafTilemap(node.left);
    //         Tilemap rightTilemap = GetLeafTilemap(node.right);
    //
    //         if (leftCenter!=null && rightCenter!=null)
    //         {
    //             // RectInt leftRect = (RectInt)node.left.room;
    //             // RectInt rightRect = (RectInt)node.right.room;
    //             // Tilemap leftTilemap = instancedPrefabs[leftRect].GetComponent<Tilemap>();
    //             // Tilemap rightTilemap = instancedPrefabs[rightRect].GetComponent<Tilemap>();
    //             
    //             
    //             //TODO use anchors here?
    //             CreateCorridor(leftCenter.Value, rightCenter.Value
    //                 ,  leftTilemap
    //                 , rightTilemap);
    //         }
    //
    //         // Recursively connect deeper
    //         ConnectRooms(node.left);
    //         ConnectRooms(node.right);
    //     }
    // }
    
    private BSPNode GetLeafWithRoom(BSPNode node)
    {
        if (node == null) return null;

        if (node.IsLeaf && node.room != null)
            return node;

        BSPNode leftLeaf = GetLeafWithRoom(node.left);
        if (leftLeaf != null) return leftLeaf;

        BSPNode rightLeaf = GetLeafWithRoom(node.right);
        if (rightLeaf != null) return rightLeaf;

        return null;
    }

    
    private void ConnectRooms(BSPNode node)
    {
        if (node.left != null && node.right != null)
        {
            BSPNode nodeA = GetLeafWithRoom(node.left);
            BSPNode nodeB = GetLeafWithRoom(node.right);

            if (nodeA == null || nodeB == null) return;

            Vector2Int centerA = nodeA.roomCenter.Value;
            Vector2Int centerB = nodeB.roomCenter.Value;

            Transform doorA = null, doorB = null;

            // Decide direction based on center positions
            if (Mathf.Abs(centerA.x - centerB.x) > Mathf.Abs(centerA.y - centerB.y))
            {
                // Horizontal connection
                if (centerA.x < centerB.x)
                {
                    doorA = nodeA.roomData.rightAnchor;
                    doorB = nodeB.roomData.leftAnchor;
                }
                else
                {
                    doorA = nodeA.roomData.leftAnchor;
                    doorB = nodeB.roomData.rightAnchor;
                }
            }
            else
            {
                // Vertical connection
                if (centerA.y < centerB.y)
                {
                    doorA = nodeA.roomData.upAnchor;
                    doorB = nodeB.roomData.downAnchor;
                }
                else
                {
                    doorA = nodeA.roomData.downAnchor;
                    doorB = nodeB.roomData.upAnchor;
                }
            }

            if (doorA != null && doorB != null)
            {
                // Vector3 tempA = nodeA.tilemap.LocalToWorld(doorA.position);
                // Vector3 tempB = nodeB.tilemap.LocalToWorld(doorB.position);
                
                Vector3Int tilePosA = Vector3Int.RoundToInt(doorA.position);
                Vector3Int tilePosB = Vector3Int.RoundToInt(doorB.position);
                Debug.DrawLine(doorA.position, doorB.position, Color.red, 100f);
                CreateCorridor(tilePosA, tilePosB, nodeA.tilemap, nodeB.tilemap);
            }

            // Recurse
            ConnectRooms(node.left);
            ConnectRooms(node.right);
        }
    }

    [SerializeField] private Tilemap corridorTilemap;
    [SerializeField] private TileBase corridorTile;
    
    private void CreateCorridor(Vector3Int start, Vector3Int end, Tilemap startRoomTilemap, Tilemap endRoomTilemap)
    {
        // Randomize direction (horizontal-first or vertical-first)
        bool horizontalFirst = Random.value > 0.5f;

        Debug.Log($"{startRoomTilemap.gameObject.name} to {endRoomTilemap.gameObject.name}");
        
        if (horizontalFirst)
        {
            // Horizontal
            for (int x = Mathf.Min(start.x, end.x); x <= Mathf.Max(start.x, end.x); x++)
            {
                Vector3Int currentPos = new Vector3Int(x, start.y, 0);
                corridorTilemap.SetTile(currentPos, corridorTile);
                
                Vector3Int localPos = startRoomTilemap.WorldToCell(currentPos);
                if (startRoomTilemap.HasTile(localPos))
                {
                    Debug.Log($"{startRoomTilemap.gameObject.name} at position {currentPos} local {localPos}");
                    startRoomTilemap.SetTile(localPos,null);
                }
            }

            // Vertical
            for (int y = Mathf.Min(start.y, end.y); y <= Mathf.Max(start.y, end.y); y++)
            {
                Vector3Int currentPos = new Vector3Int(end.x, y, 0);
                corridorTilemap.SetTile(currentPos, corridorTile);
                
                Vector3Int localPos = endRoomTilemap.WorldToCell(currentPos);
                if (endRoomTilemap.HasTile(localPos))
                {
                    Debug.Log($"{endRoomTilemap.gameObject.name} at position {currentPos} local {localPos}");
                    endRoomTilemap.SetTile(localPos,null);
                }
            }
        }
        else
        {
            // Vertical
            for (int y = Mathf.Min(start.y, end.y); y <= Mathf.Max(start.y, end.y); y++)
            {
                Vector3Int currentPos = new Vector3Int(start.x, y, 0);
                corridorTilemap.SetTile(currentPos, corridorTile);
                
                Vector3Int localPos = startRoomTilemap.WorldToCell(currentPos);
                if (startRoomTilemap.HasTile(localPos))
                {
                    Debug.Log($"{startRoomTilemap.gameObject.name} at position {currentPos} local {localPos}");
                    startRoomTilemap.SetTile(localPos,null);
                }
            }

            // Horizontal
            for (int x = Mathf.Min(start.x, end.x); x <= Mathf.Max(start.x, end.x); x++)
            {
                Vector3Int currentPos = new Vector3Int(x, end.y, 0);
                corridorTilemap.SetTile(currentPos, corridorTile);
                
                Vector3Int localPos = endRoomTilemap.WorldToCell(currentPos);
                if (endRoomTilemap.HasTile(localPos))
                {
                    Debug.Log($"{endRoomTilemap.gameObject.name} at position {currentPos} local {localPos}");
                    endRoomTilemap.SetTile(localPos,null);
                }
            }
        }
    }
    
    // void CreateCorridor(Vector3Int from, Vector3Int to)
    // {
    //     bool horizontalFirst = Random.value > 0.5f;
    //
    //     if (horizontalFirst)
    //     {
    //         for (int x = Mathf.Min(from.x, to.x); x <= Mathf.Max(from.x, to.x); x++)
    //             corridorTilemap.SetTile(new Vector3Int(x, from.y, 0), corridorTile);
    //
    //         for (int y = Mathf.Min(from.y, to.y); y <= Mathf.Max(from.y, to.y); y++)
    //             corridorTilemap.SetTile(new Vector3Int(to.x, y, 0), corridorTile);
    //     }
    //     else
    //     {
    //         for (int y = Mathf.Min(from.y, to.y); y <= Mathf.Max(from.y, to.y); y++)
    //             corridorTilemap.SetTile(new Vector3Int(from.x, y, 0), corridorTile);
    //
    //         for (int x = Mathf.Min(from.x, to.x); x <= Mathf.Max(from.x, to.x); x++)
    //             corridorTilemap.SetTile(new Vector3Int(x, to.y, 0), corridorTile);
    //     }
    // }

    
    

    
    private Tilemap GetLeafTilemap(BSPNode node)
    {
        if (node == null) return null;

        if (node.IsLeaf && node.tilemap != null)
            return node.tilemap;

        // Recursively check children
        Tilemap left = GetLeafTilemap(node.left);
        if (left != null) return left;

        return GetLeafTilemap(node.right);
    }


    private Vector2Int? GetRoomCenter(BSPNode node)
    {
        if (node.roomCenter != null)
            return node.roomCenter;

        if (node.left != null && node.right != null)
        {
            Vector2Int? left = GetRoomCenter(node.left);
            Vector2Int? right = GetRoomCenter(node.right);
            if (left != null && right != null)
                return (left.Value + right.Value) / 2;
        }

        return null;
    }




    [SerializeField] private List<GameObject> roomPrefabs;
    private Dictionary<RectInt, GameObject> instancedPrefabs;

    private int _prefabCounter = 0;
    private void PlaceRoomPrefabs(BSPNode currentNode)
    {
        if (currentNode.IsLeaf)
        {
            RectInt room = currentNode.room.Value;
            // Pick a prefab that fits inside the room (you can filter or pick randomly)
            GameObject prefab = ChooseFittingRoomPrefab(room);

            if (prefab != null)
            {
                GameObject instance = Instantiate(prefab, tilemapParent.transform);

                // // Get prefab size from Tilemap bounds
                // Tilemap tilemap = instance.GetComponentInChildren<Tilemap>();
                // BoundsInt bounds = tilemap.cellBounds;
                // Vector2Int prefabSize = new Vector2Int(bounds.size.x, bounds.size.y);

                // Center prefab inside the room
                Vector3Int position = new Vector3Int(
                    room.x + (room.width / 2),
                    room.y + (room.height / 2),
                    0
                );

                currentNode.tilemap = instance.GetComponent<Tilemap>(); 
                currentNode.roomData = instance.GetComponent<TilemapRoomPrefab>();
                instance.transform.position = position;
                instance.name = $"Room_{_prefabCounter}";
                _prefabCounter += 1;
                instancedPrefabs.Add(room, instance);
                
            }

        }
        else
        {
            PlaceRoomPrefabs(currentNode.left);
            PlaceRoomPrefabs(currentNode.right);
        }
    }


    private GameObject ChooseFittingRoomPrefab(RectInt room)
    {
        /*First match return
        // Simple filter: return the first prefab that fits
        foreach (var prefab in roomPrefabs)
        {
            Tilemap tilemap = prefab.GetComponentInChildren<Tilemap>();
            BoundsInt bounds = tilemap.cellBounds;
            if (bounds.size.x <= room.width && bounds.size.y <= room.height)
            {
                return prefab;
            }
        }
        
        return null; // fallback
        */
        
        //Random till fits
        int failLimit = 1000;
        while (true)
        {
            int rand = Random.Range(0, roomPrefabs.Count - 1);
            
            Tilemap tilemap = roomPrefabs[rand].GetComponentInChildren<Tilemap>();
            tilemap.CompressBounds();
            BoundsInt bounds = tilemap.cellBounds;
            int xSize = bounds.xMax - bounds.xMin;
            int ySize = bounds.yMax - bounds.yMin;
            if (xSize <= room.width && ySize <= room.height)
            {
                // Debug.Log($"X {xSize} Width {room.width} || Y {ySize} Height {room.height}");
                return roomPrefabs[rand];
            }
            
            failLimit -= 1;
            if (failLimit < 0)
            {
                // Debug.Log(" choice->"+rand);
                // Debug.Log($"X {xSize} Width {room.width} || Y {ySize} Height {room.height} || {bounds.size.x}|{bounds.size.y}");
                //
                // Debug.Log($"X{bounds.xMax},{bounds.xMin} || Y{bounds.yMax},{bounds.yMin}");
                // Debug.Log("FAILED TO PICK");
                return null;
            }
        }

    }

    
    /*Drunkard's Walk Algorithm
    
    [SerializeField, Range (3, 10)] private int drunkardsRepeat;
    [SerializeField, Range (10, 80)] private int drunakrdNSteps;
    
    public void DrunkardsWalk()
    {

        Dictionary<Vector2, bool> boolDict = new Dictionary<Vector2,bool>();
        int repeatLimit = drunkardsRepeat;
        int nSteps = drunakrdNSteps;

        for (int xIter = 0; xIter < 20; xIter++)
        {
            for (int yIter = 0; yIter < 20; yIter++)
            {
                boolDict.Add(new Vector2(xIter,yIter),false);
            }
        }
        
        Vector2 initialCoordinates = new Vector2(9, 9);

        Vector2 movementUp = new Vector2(0, 1);
        Vector2 movementDown = new Vector2(0, -1);
        Vector2 movementRight = new Vector2(1, 0);
        Vector2 movementLeft = new Vector2(-1, 0);
        Vector2[] movements = new Vector2[]{movementUp,movementDown,movementLeft,movementRight};

        bool initial = true;
        
        do
        {
            //Pick a random Starting Point
            if (!initial)
            {
                List<Vector2> openedArea = boolDict
                    .Where(kv => kv.Value == false)
                    .Select(kv => kv.Key)
                    .ToList();
                
                int chosenIndex = Random.Range(0, openedArea.Count);
                initialCoordinates = openedArea[chosenIndex];
            }

            Vector2 currentCoordinate = initialCoordinates;

            while (nSteps>0)
            {
                currentCoordinate += movements[Random.Range(0, movements.Length)];

                if (!boolDict.ContainsKey(currentCoordinate))
                {
                    break;
                }

                boolDict[currentCoordinate] = true;
                nSteps -= 1;
            }

            repeatLimit -= 1;
            initial = false;
        } while (repeatLimit > 0);

        foreach (KeyValuePair<Vector2,bool> keyValuePair in boolDict)
        {
            if (!keyValuePair.Value)
            {
                continue;
            }

            Vector3 spawnPosition = new Vector3(18 * keyValuePair.Key.x, 10 * keyValuePair.Key.y);
            GameObject.Instantiate(roomPrefabs[0], spawnPosition, Quaternion.identity);
        }
        
    }
    */
    

}
