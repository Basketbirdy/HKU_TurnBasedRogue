using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdle : State<EnemyBehaviour>
{
    FSM<EnemyBehaviour> fsm;

    public EnemyIdle(FSM<EnemyBehaviour> fsm) : base(fsm)
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

            TurnManager.instance.AdvanceTurn(1);
            Debug.Log("EnemyTurns; Advanced turn");

            fsm.owner.turnAffectedObjects = TurnManager.instance.GetList();

            fsm.owner.processingTurn = false;
        }

        if(fsm.owner.turnAffectedObjects.Count > 0) 
        {
            if (fsm.owner.turnAffectedObjects.Contains(fsm.owner.gameObject))
            {
                fsm.ChangeState(typeof(EnemyChase));
            }
        }
    }

    public override void Exit()
    {
        
    }
}
