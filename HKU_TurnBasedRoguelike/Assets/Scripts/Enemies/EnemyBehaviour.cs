using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Enemy enemyData;

    public float hp;

    [Header("Enemy data")]
    [Header("Combat")]
    public float damageMultiplier;
    [Space]
    public float defenseMultiplier;
    [Header("States")]
    [SerializeField] bool isTurn = false;
    [Space]
    [SerializeField] int turnIndex;

    // Start is called before the first frame update
    void Start()
    {
        hp = enemyData.maxHp;
        damageMultiplier = enemyData.damageMultiplier;
        defenseMultiplier = enemyData.defenseMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        TurnManager.onAdvanceTurn += CheckTurn;
    }

    private void OnDisable()
    {
        TurnManager.onAdvanceTurn -= CheckTurn;
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("Combat; " + gameObject.name + " Took " + damage + " damage");
        // calculate damage
        float actualDamage = damage - Mathf.RoundToInt(UnityEngine.Random.Range(0f, enemyData.baseDefense * defenseMultiplier));

        if (hp - actualDamage <= 0) { Die(); }
        else { hp -= actualDamage; }
    }

    private void Die()
    {
        Debug.Log("Combat; " + gameObject.name + " Died");
        TurnManager.instance.AdvanceTurn(1);

        Destroy(gameObject);
    }

    private void CheckTurn()
    {
        if(!TurnManager.instance.GetList().Contains(gameObject))
        {
            return;
        }

        turnIndex = TurnManager.instance.ReturnObjIndex(gameObject);

        int currentIndex = TurnManager.instance.ReturnActiveIndex();

        if (currentIndex == turnIndex)
        {
            isTurn = true;
        }
        else
        {
            isTurn = false;
        }

        Debug.Log("Turns; triggered CheckTurn function");
    }
}
