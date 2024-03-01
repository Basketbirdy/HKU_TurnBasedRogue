using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingState : State<GameManager>
{
    GameFSM fsm;

    public PlayingState(GameFSM fsm) : base(fsm)
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

// Very basic generics use

//public class test<T>
//{
//    public T Value { get; set; }

//    public void afdsfd()
//    {
//        if(Value.GetType() == typeof(int))
//        {

//        }
//    }
//}

//public class testINT : test<int>
//{
//   public void blah()
//    {
//        int math = Value + 5;
//    }
//}