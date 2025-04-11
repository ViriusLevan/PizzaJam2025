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
        bspNodes = new List<BSPNode>();
        nodesWithRooms = new List<BSPNode>();

        BSPNode rootNode = new BSPNode(root);
        
        
        do
        {
            rooms.Clear();
            bspNodes.Clear();
            SplitSpace(rootNode);
            CreateRooms(bspNodes[0]);
        } while (rooms.Count < 10);
        
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

        }
        else
        {
            CreateRooms(node.left);
            CreateRooms(node.right);
        }
    }
    
    [SerializeField] private List<GameObject> roomPrefabs;
    [SerializeField] private List<BSPNode> nodesWithRooms;

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

                CopyTilesToMain(currentNode.tilemap, mainTilemap, new Vector3Int(position.x, position.y, 0));
                nodesWithRooms.Add(currentNode);
            }

        }
        else
        {
            PlaceRoomPrefabs(currentNode.left);
            PlaceRoomPrefabs(currentNode.right);
        }
    }
    
    private void CopyTilesToMain(Tilemap source, Tilemap target, Vector3Int offset)
    {
        BoundsInt bounds = source.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = source.GetTile(pos);
            if (tile != null)
            {
                Vector3Int targetPos = pos + offset;
                target.SetTile(targetPos, tile);
            }
        }
    }

    private GameObject ChooseFittingRoomPrefab(RectInt room)
    {
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
                Vector3Int tilePosA = Vector3Int.RoundToInt(doorA.position);
                Vector3Int tilePosB = Vector3Int.RoundToInt(doorB.position);
                Debug.DrawLine(doorA.position, doorB.position, Color.red, 100f);
                nodeA.tilemap.gameObject.SetActive(false);
                nodeB.tilemap.gameObject.SetActive(false);
                CreateCorridor(tilePosA, tilePosB);
            }

            // Recurse
            ConnectRooms(node.left);
            ConnectRooms(node.right);
        }
    }

    [SerializeField] private Tilemap mainTilemap;
    [SerializeField] private TileBase corridorTile, upWall,downWall,leftWall,rightWall;

    private void CreateCorridor(Vector3Int start, Vector3Int end)
    {
        // Randomize direction (horizontal-first or vertical-first)
        bool horizontalFirst = Random.value > 0.5f;

        if (horizontalFirst)
        {
            // Horizontal
            for (int x = Mathf.Min(start.x, end.x); x <= Mathf.Max(start.x, end.x); x++)
            {
                Vector3Int currentPos = new Vector3Int(x, start.y, 0);
                mainTilemap.SetTile(currentPos, corridorTile);
                if (!mainTilemap.HasTile(currentPos + new Vector3Int(0,1,0)))
                {
                    mainTilemap.SetTile(currentPos + new Vector3Int(0,1,0), upWall);
                }
                if (!mainTilemap.HasTile(currentPos + new Vector3Int(0,-1,0)))
                {
                    mainTilemap.SetTile(currentPos + new Vector3Int(0,-1,0), downWall);
                }
            }

            // Vertical
            for (int y = Mathf.Min(start.y, end.y); y <= Mathf.Max(start.y, end.y); y++)
            {
                Vector3Int currentPos = new Vector3Int(end.x, y, 0);
                mainTilemap.SetTile(currentPos, corridorTile);
                if (!mainTilemap.HasTile(currentPos + new Vector3Int(1,0,0)))
                {
                    mainTilemap.SetTile(currentPos + new Vector3Int(1,0,0), rightWall);
                }
                if (!mainTilemap.HasTile(currentPos + new Vector3Int(-1,0,0)))
                {
                    mainTilemap.SetTile(currentPos + new Vector3Int(-1,0,0), leftWall);
                }
            }
        }
        else
        {
            // Vertical
            for (int y = Mathf.Min(start.y, end.y); y <= Mathf.Max(start.y, end.y); y++)
            {
                Vector3Int currentPos = new Vector3Int(start.x, y, 0);
                mainTilemap.SetTile(currentPos, corridorTile);
                if (!mainTilemap.HasTile(currentPos + new Vector3Int(1,0,0)))
                {
                    mainTilemap.SetTile(currentPos + new Vector3Int(1,0,0), rightWall);
                }
                if (!mainTilemap.HasTile(currentPos + new Vector3Int(-1,0,0)))
                {
                    mainTilemap.SetTile(currentPos + new Vector3Int(-1,0,0), leftWall);
                }
            }

            // Horizontal
            for (int x = Mathf.Min(start.x, end.x); x <= Mathf.Max(start.x, end.x); x++)
            {
                Vector3Int currentPos = new Vector3Int(x, end.y, 0);
                mainTilemap.SetTile(currentPos, corridorTile);
                if (!mainTilemap.HasTile(currentPos + new Vector3Int(0,1,0)))
                {
                    mainTilemap.SetTile(currentPos + new Vector3Int(0,1,0), upWall);
                }
                if (!mainTilemap.HasTile(currentPos + new Vector3Int(0,-1,0)))
                {
                    mainTilemap.SetTile(currentPos + new Vector3Int(0,-1,0), downWall);
                }
            }
        }
    }

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
}
