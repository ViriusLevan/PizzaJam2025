using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

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

    [SerializeField] private GameObject[] roomPrefabs;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //TestGeneration();
        DrunkardsWalk();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
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

}
