using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class WinState : State<GameManager>
{
    GameFSM fsm;

    public WinState(GameFSM fsm) : base(fsm)
    {
        this.fsm = fsm;
    }

    public override void Enter()
    {
        // setup game
        Time.timeScale = 0f;
        fsm.owner.playerMovement.canMove = false;

        // enable ui
        UIUtils.DisableAll(fsm.owner.uiObjects); // disable all ui
        UIUtils.EnableSpecified(fsm.owner.uiObjects, "WinScreen");
    }

    public override void Execute()
    {

    }

    public override void Exit()
    {
        Time.timeScale = 1f;
        fsm.owner.playerMovement.canMove = true;
    }
}
