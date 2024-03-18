using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class GameFSM : FSM<GameManager>
{
    public GameFSM(GameManager owner) : base(owner)
    {
        this.owner = owner;

        // Add instances of all classes that inherit from type State.
        states.Add(typeof(MainMenuState), new MainMenuState(this));
        states.Add(typeof(PlayingState), new PlayingState(this));
        states.Add(typeof(PauseState), new PauseState(this));
        states.Add(typeof(GameOverState), new GameOverState(this));
        states.Add(typeof(WinState), new WinState(this));
    }
}
