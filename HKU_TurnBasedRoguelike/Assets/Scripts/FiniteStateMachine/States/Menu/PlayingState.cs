using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingState<T> : State<T> where T : GameManager
{
    FSM<T> fsm;

    public PlayingState(FSM<T> fsm) : base(fsm)
    {
        this.fsm = fsm;
    }

    public override void Enter()
    {
        // enable ui
        UIUtils.DisableAll(fsm.owner.uiObjects); // disable all ui
        UIUtils.EnableSpecified(fsm.owner.uiObjects, "HUD");
    }

    public override void Execute()
    {

    }

    public override void Exit()
    {

    }
}
