using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.GraphicsBuffer;

public class EnemyAI : MonoBehaviour, IDamagable
{
    [Header("References")]
    [SerializeField] Enemy enemyData;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    [Space]
    [SerializeField] ParticleSystem hitParticles;
    [SerializeField] ParticleSystem[] chargeParticles;
    [SerializeField] ParticleSystem chargedParticles;
    [Space]
    [SerializeField] GameObject player;
    private PlayerMovement playerMovement;
    private PlayerCombat playerCombat;

    [SerializeField] Tilemap tilemap;

    [Header("EnemyData")]
    [SerializeField] float health;
    [SerializeField] Vector3Int currentTile;
    [SerializeField] Vector3 targetCell;
    [Space]
    [SerializeField] float damageMultiplier;
    [SerializeField] float defenseMultiplier;
    [Space]
    [SerializeField] bool isCharged = false;

    [Header("States")]
    [SerializeField] bool isTurn;
    [SerializeField] bool processingTurn;
    [Space]
    [SerializeField] int turnIndex;

    private void OnEnable()
    {
        TurnManager.onAdvanceTurn += CheckTurn;
    }

    private void OnDisable()
    {
        TurnManager.onAdvanceTurn -= CheckTurn;
    }

    // Start is called before the first frame update
    void Start()
    {
        // get references
        player = FindObjectOfType<PlayerMovement>().gameObject;
        playerMovement = player.GetComponent<PlayerMovement>();
        playerCombat = player.GetComponent<PlayerCombat>();
        tilemap = FindObjectOfType<Tilemap>();
        
        // define values
        currentTile = TileUtils.GetCellPosition(tilemap, transform.position);
        targetCell = currentTile;

        EnemyStatsSetup();
    }

    // Update is called once per frame
    void Update()
    {
        if(isTurn)
        {
            if (!processingTurn)
            {
                processingTurn = true;

                // get the distance of the player in int
                int distance = Mathf.RoundToInt(Vector3.Distance(transform.position, player.transform.position));
                Debug.Log("EnemyAI; distance from player is: " +  distance);

                if(distance <= enemyData.attackRange)
                {
                    if (!enemyData.needsCharge)
                    {
                        MeleeAttack();
                    }
                    else
                    {
                        if (isCharged == true)
                        {
                            MeleeAttack();
                            ParticleUtils.TriggerSystem(chargedParticles, false);
                        }
                        else
                        {
                            isCharged = true;

                            // charge particles
                            ParticleUtils.TriggerMultiple(chargeParticles, true);
                            ParticleUtils.TriggerSystem(chargedParticles, true);
                        }
                    }
                }
                else
                {
                    // idle/chase logic here
                    Vector3 direction = playerMovement.GetCurrentCellPosition() - currentTile;
                    direction = direction.normalized * enemyData.movementRange;
                    direction = new Vector3(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y), 0);
                    Vector3Int directionInt = Vector3Int.FloorToInt(direction);
                    Debug.Log("EnemyAI; direction: " + direction);

                    targetCell = TileUtils.GetWorldCellPosition(tilemap, currentTile + directionInt);
                    Debug.Log("EnemyAI; targetcell: " + targetCell);

                    if(targetCell.x > transform.position.x)
                    {
                        spriteRenderer.flipX = true;
                    }
                    else
                    {
                        spriteRenderer.flipX = false;
                    }
                }
            }

            if(processingTurn && transform.position != TileUtils.GetWorldCellPosition(tilemap, Vector3Int.FloorToInt(targetCell)))
            {
                transform.position = Vector3.MoveTowards(transform.position, targetCell, enemyData.movementSpeed * Time.deltaTime);
                animator.SetBool("isMoving", true);
            }
            else if (processingTurn && transform.position == TileUtils.GetWorldCellPosition(tilemap, Vector3Int.FloorToInt(targetCell)))
            {
                processingTurn = false;
                animator.SetBool("isMoving", false);
                TurnManager.instance.AdvanceTurn(1);
            }
        }
    }

    public void MeleeAttack()
    {
        // attack logic here
        Debug.Log("EnemyAI; Attacked player");
        animator.SetTrigger("Attack"); // DoDamage gets called in animation event
        isCharged = false;
    }

    private void EnemyStatsSetup()
    {
        health = enemyData.maxHp;
        damageMultiplier = enemyData.damageMultiplier;
        defenseMultiplier = enemyData.defenseMultiplier;
    }

    public void DoDamage()
    {
        float damage = enemyData.damage * damageMultiplier;
        playerCombat.TakeDamage(damage);
    }

    private void CheckTurn()
    {
        if (!TurnManager.instance.GetList().Contains(gameObject))
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

    public void TakeDamage(float damage)
    {
        Debug.Log("Combat; " + gameObject.name + " Took " + damage + " damage");
        // calculate damage
        float actualDamage = damage - Mathf.RoundToInt(UnityEngine.Random.Range(0f, enemyData.baseDefense * defenseMultiplier));

        if (health - actualDamage <= 0) { Die(); }
        else { health -= actualDamage; ParticleUtils.TriggerSystem(hitParticles, true); }
    }

    private void Die()
    {
        Debug.Log("Combat; " + gameObject.name + " Died");
        ParticleUtils.DestroyAfterSeperation(hitParticles, true);

        TurnManager.instance.AdvanceTurn(1);

        Destroy(gameObject);
    }
}