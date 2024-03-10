using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverState : State<GameManager>
{
    GameFSM fsm;

    public GameOverState(GameFSM fsm) : base(fsm)
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
        UIUtils.EnableSpecified(fsm.owner.uiObjects, "GameOver");
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
