using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Proximity proximity;

    [Header("Combat data")]
    [SerializeField] float minDamage = 60f;
    [SerializeField] float maxDamage = 120f;
    [Space]
    [SerializeField] float damageMultiplier = 1f;
    [SerializeField] float defenseMultiplier = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DealDamage(GameObject enemyObj)
    {
        enemyObj?.GetComponent<EnemyBehaviour>().TakeDamage(Mathf.RoundToInt(Random.Range(minDamage * damageMultiplier , maxDamage * damageMultiplier)));
        TurnManager.instance.AdvanceTurn(1);

        Debug.Log("Combat; Dealt damage to: " + enemyObj.name); 
    }
}
