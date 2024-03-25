using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] List<GameObject> turnAffectedObj;

    [Header("Turn management")]
    public int activeIndex = 0;

    //[Header("Events")]
    public static event Action onAdvanceTurn;

    #region Singleton

    public static TurnManager instance;

    public static TurnManager Instance
    {
        get { if (instance == null) { Debug.Log("TurnManager = null"); } return instance; }
    }

    private void Awake()
    {
        instance = this;
    }

    #endregion 

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AdvanceTurn(int amount)
    {
        onAdvanceTurn?.Invoke();

        Debug.Log("Turns; starting index: " + activeIndex);
        Debug.Log("Turns; turn affected objects: " + turnAffectedObj.Count);

        int newAmount = activeIndex + amount;
        int maxIndex = turnAffectedObj.Count;

        if(newAmount >= maxIndex)
        {
            newAmount = newAmount - maxIndex;

            activeIndex = newAmount;
        }
        else
        {
            activeIndex = newAmount;
        }

        Debug.Log("Turns; Active index: " + activeIndex);
    }

    public void AddToList(GameObject obj)
    {
        turnAffectedObj.Add(obj);
    }

    public void RemoveFromList(GameObject obj)
    {
        turnAffectedObj.Remove(obj);
    }

    public List<GameObject> GetList()
    {
        return turnAffectedObj;
    }

    public int ReturnObjIndex(GameObject obj)
    {
        return turnAffectedObj.IndexOf(obj);
    }

    public int ReturnActiveIndex()
    {
        return activeIndex;
    }

    public void inRangeOfPlayer()
    {

    }
}

