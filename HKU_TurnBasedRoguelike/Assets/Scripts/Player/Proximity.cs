using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Proximity : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] Vector2Int proximityRange;

    [Header("Other")]
    [SerializeField] LayerMask checkLayers;

    [Header("References")]
    [SerializeField] Tilemap tilemap;
    [SerializeField] GameObject obj;
    [SerializeField] PlayerMovement playerMovement;

    [Header("Debug")]
    [SerializeField] List<GameObject> objectsInProximity;
    [SerializeField] List<GameObject> affectedObjects;

    private void OnEnable()
    {
        TurnManager.onAdvanceTurn += CheckProximity;
    }

    private void OnDisable()
    {
        TurnManager.onAdvanceTurn -= CheckProximity;       
    }

    // Start is called before the first frame update
    void Start()
    {
        CheckProximity();
        affectedObjects = TurnManager.instance.GetList();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.N)) { TurnManager.instance.AdvanceTurn(1); }
    }

    private void CheckProximity()
    {
        Debug.Log("Turns; Triggered CheckProximity function");

        objectsInProximity = new List<GameObject>();
        affectedObjects = TurnManager.instance.GetList();

        // loop over all cells within proximitycheck range
        for (int i = -proximityRange.x; i <= proximityRange.x; i++)
        {
            for(int j = -proximityRange.y; j <= proximityRange.y; j++)
            {
                //check if there is something on the tile
                Collider2D[] obstacles = Physics2D.OverlapBoxAll(new Vector2(playerMovement.targetPos.x + i, playerMovement.targetPos.y + j), Vector2.one * 0.1f, 0f, checkLayers);

                foreach (Collider2D col in obstacles)
                {
                    Debug.Log("Turns; gameObjects hit: " + col.gameObject.name);

                    GameObject colObj = col.gameObject;

                    // if not already in list add objects to the lists
                    if (!objectsInProximity.Contains(colObj))
                    {
                        objectsInProximity.Add(colObj); 
                    }
                    Debug.Log("Turns; objectsInProximity list: " + objectsInProximity.Count); 

                    
                    if (!affectedObjects.Contains(colObj)) 
                    { 
                        TurnManager.instance.AddToList(colObj); 
                        Debug.Log("Turns; Added: " + obj.name);   
                    }
                }
            }
        }

        for(int i = 0; i < affectedObjects.Count; i++)
        {
            if (affectedObjects[i].layer == 6) { Debug.Log("Skipped player"); continue; }

            // if an object in the affected list is not in range, remove the object from the list
            if (!objectsInProximity.Contains(affectedObjects[i]) && affectedObjects[i] != null)
            {
                TurnManager.instance.RemoveFromList(affectedObjects[i]);
                i--;
                TurnManager.instance.AdvanceTurn(0);
                Debug.Log("Turns; Removed an object from turnAffected list");
            }
        }
    }
}
