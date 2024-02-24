using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuState<T> : State<T>
{
    FSM<T> fsm;

    public MainMenuState(FSM<T> fsm) : base(fsm)
    {
        this.fsm = fsm;
    }

    public override void Enter()
    {
        
    }

    public override void Execute()
    {
        
    }

    public override void Exit()
    {
        
    }
}
