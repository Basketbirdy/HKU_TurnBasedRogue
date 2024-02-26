using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseState<T> : State<T> where T : GameManager
{
    FSM<T> fsm;

    public PauseState(FSM<T> fsm) : base(fsm)
    {
        this.fsm = fsm;
    }

    public override void Enter()
    {
        // setup state
        Time.timeScale = 0f;
        fsm.owner.playerMovement.canMove = false;
        fsm.owner.isPaused = true;


        // enable ui
        UIUtils.DisableAll(fsm.owner.uiObjects); // disable all ui
        UIUtils.EnableSpecified(fsm.owner.uiObjects, "Pause");
    }

    public override void Execute()
    {

    }

    public override void Exit()
    {
        // reset state
        Time.timeScale = 1f;
        fsm.owner.playerMovement.canMove = true;
        fsm.owner.isPaused = false;
    }
}
