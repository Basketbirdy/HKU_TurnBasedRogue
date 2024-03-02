using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

            // get the players cell position
            Vector3Int playerPos = fsm.owner.player.GetComponent<PlayerMovement>().GetCurrentCellPosition();

            // get the direction to move to
            Vector3 direction = (playerPos - fsm.owner.currentCell);
            Vector3 targetOffset = direction.normalized * fsm.owner.enemyData.movementRange;

            // get the cell to move to
            Vector3Int targetCell = fsm.owner.currentCell + new Vector3Int(Mathf.RoundToInt(targetOffset.x), Mathf.RoundToInt(targetOffset.y), 0);
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

    public override void Exit()
    {
      
    }
}
