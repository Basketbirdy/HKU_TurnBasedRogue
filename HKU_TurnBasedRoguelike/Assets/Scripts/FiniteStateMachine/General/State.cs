using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class State<T> where T : GameManager
{
    private FSM<T> fsm;

    public State(FSM<T> fsm)
    {
        this.fsm = fsm;
    }

    public abstract void Enter();

    public abstract void Execute();

    public abstract void Exit();
}
