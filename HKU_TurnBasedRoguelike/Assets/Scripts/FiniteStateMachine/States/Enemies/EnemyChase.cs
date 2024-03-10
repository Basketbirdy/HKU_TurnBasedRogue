using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemyChase : State<EnemyBehaviour>
{
    FSM<EnemyBehaviour> fsm;

    public EnemyChase(FSM<EnemyBehaviour> fsm) : base(fsm)
    {
        this.fsm = fsm;
    }

    public override void Enter()
    {

    }

    public override void Execute()
    {
        if (fsm.owner.isTurn && !fsm.owner.processingTurn)
        {
            fsm.owner.processingTurn = true;

            // check if the player is within range
            bool playerInRange = AttackCheck(TileUtils.GetCellPosition(fsm.owner.tilemap, fsm.owner.player.transform.position), fsm.owner.enemyData.attackRange);

            if(playerInRange) { AttackPlayer(); TurnManager.instance.AdvanceTurn(1); return; }

            // get the players cell position
            Vector3Int playerPos = fsm.owner.player.GetComponent<PlayerMovement>().GetCurrentCellPosition();

            // get the direction to move to
            Vector3 direction = (playerPos - fsm.owner.currentTile);
            Vector3 targetOffset = direction.normalized * fsm.owner.enemyData.movementRange;

            // get the cell to move to
            Vector3Int targetCell = fsm.owner.currentTile + new Vector3Int(Mathf.RoundToInt(targetOffset.x), Mathf.RoundToInt(targetOffset.y), 0);
            Debug.Log("EnemyMovement; targetcell set to: " + targetCell);

            // move to cell
            fsm.owner.targetCell = targetCell;

        }

        if (fsm.owner.transform.position == TileUtils.GetWorldCellPosition(fsm.owner.tilemap, fsm.owner.targetCell) && fsm.owner.isTurn)
        {
            TurnManager.instance.AdvanceTurn(1);
            Debug.Log("Advancing turn");
            fsm.owner.processingTurn = false;
        }
    }

    public bool AttackCheck(Vector3Int playerCell, int range)
    {
        // clickedCell is the selected cell
        // checkPos is a cell inside the range

        // check if player is within range
        for (var i = -range; i <= range; i++)
        {
            for (var j = -range; j <= range; j++)
            {
                Vector3Int checkPos = new Vector3Int(fsm.owner.currentTile.x + i, fsm.owner.currentTile.y + j, 0);
                if(playerCell == checkPos)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void AttackPlayer()
    {
        float damage = UnityEngine.Random.Range(fsm.owner.enemyData.minDamage, fsm.owner.enemyData.maxDamage) * fsm.owner.damageMultiplier;
        fsm.owner.player.GetComponent<PlayerCombat>().TakeDamage(damage);
    }

    public override void Exit()
    {
      
    }
}
