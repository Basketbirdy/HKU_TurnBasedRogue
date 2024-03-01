using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFSM : FSM<GameManager>
{
    public GameFSM(GameManager owner) : base(owner)
    {
        this.owner = owner;

        // Add instances of all classes that inherit from type State.
        states.Add(typeof(MainMenuState), new MainMenuState(this));
        states.Add(typeof(PlayingState), new PlayingState(this));
        states.Add(typeof(PauseState), new PauseState(this));
    }
}
