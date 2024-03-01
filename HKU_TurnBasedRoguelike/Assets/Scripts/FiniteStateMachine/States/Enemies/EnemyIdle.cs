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
        if (fsm.owner.isTurn)
        {
            TurnManager.instance.AdvanceTurn(1);
            Debug.Log("EnemyTurns; Advanced turn");
        }
    }

    public override void Exit()
    {
        
    }
}
