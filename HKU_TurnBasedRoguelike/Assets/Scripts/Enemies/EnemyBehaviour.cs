using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyBehaviour : MonoBehaviour
{

    // !!! IMPORTANT !!! THIS SCRIPT IS NOT USED ANYMORE - check EnemyAI for enemy behaviour

    [Header("References")]
    [SerializeField] internal Enemy enemyData;
    [Space]
    [SerializeField] ParticleSystem hitParticles;
    private EnemyFSM fsm;
    internal GameObject player;
    internal Tilemap tilemap; 

    public float hp;

    [Header("Enemy data")]
    [Header("Movement")]
    [SerializeField] internal Vector3Int currentTile;
    [SerializeField] internal Vector3Int targetCell;
    [SerializeField] internal LayerMask playerMask;

    [Header("Combat")]
    public float damageMultiplier;
    [Space]
    public float defenseMultiplier;

    [Header("States")]
    [SerializeField] internal bool isTurn = false;
    [SerializeField] internal bool processingTurn = false;
    [Space]
    [SerializeField] int turnIndex;

    [Header("turn")]
    [SerializeField] internal List<GameObject> turnAffectedObjects;

    // !!! IMPORTANT !!! THIS SCRIPT IS NOT USED ANYMORE - check EnemyAI for enemy behaviour

    // Start is called before the first frame update
    void Start()
    {
        fsm = new EnemyFSM(this);
        fsm.ChangeState(typeof(EnemyIdle));

        player = FindObjectOfType<PlayerMovement>().gameObject;
        tilemap = FindObjectOfType<Tilemap>();

        currentTile = TileUtils.GetCellPosition(tilemap, transform.position);
        targetCell = currentTile;
        Debug.Log("enemyMovement; targetCell: " + targetCell);

        hp = enemyData.maxHp;
        damageMultiplier = enemyData.damageMultiplier;
        defenseMultiplier = enemyData.defenseMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
        fsm.Update();

        if(transform.position != targetCell)
        {
           MoveEnemy();
        }

    }

    private void OnEnable()
    {
        TurnManager.onAdvanceTurn += CheckTurn;
    }

    private void OnDisable()
    {
        TurnManager.onAdvanceTurn -= CheckTurn;
    }

    private void MoveEnemy()
    {
        transform.position = Vector3.MoveTowards(transform.position, TileUtils.GetWorldCellPosition(tilemap, targetCell), enemyData.movementSpeed * Time.deltaTime);
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("Combat; " + gameObject.name + " Took " + damage + " damage");
        // calculate damage
        float actualDamage = damage - Mathf.RoundToInt(UnityEngine.Random.Range(0f, enemyData.baseDefense * defenseMultiplier));

        if (hp - actualDamage <= 0) { Die(); }
        else { hp -= actualDamage; ParticleUtils.TriggerSystem(hitParticles, true); }
    }

    private void Die()
    {
        Debug.Log("Combat; " + gameObject.name + " Died");
        ParticleUtils.DestroyAfterSeperation(hitParticles, true);

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

        currentTile = TileUtils.GetCellPosition(tilemap, transform.position);

        Debug.Log("Turns; triggered CheckTurn function");
    }
}
