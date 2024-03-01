using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (fsm.owner.isTurn)
        {
            TurnManager.instance.AdvanceTurn(1);
        }
    }

    public override void Exit()
    {
      
    }
}
