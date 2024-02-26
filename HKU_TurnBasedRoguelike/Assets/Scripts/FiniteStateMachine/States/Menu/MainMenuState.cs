using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuState<T> : State<T> where T : GameManager
{
    FSM<T> fsm;

    public MainMenuState(FSM<T> fsm) : base(fsm)
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
        UIUtils.EnableSpecified(fsm.owner.uiObjects, "MainMenu");
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
