using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FSM<T>
{
    public T owner;

    public Dictionary<Type, State<T>> states;
    private State<T> currentState;

    public FSM(T owner)
    {
        this.owner = owner;
        states = new Dictionary<Type, State<T>>();

        currentState = null;
    }

    // call this function inside of owner |-- IMPRORTANT!! --|
    public void Update()
    {
        currentState?.Execute();
    }

    public void ChangeState(Type newStateType)
    { 
        currentState?.Exit();
        currentState = states[newStateType];
        currentState?.Enter();

        Debug.Log("States; Changed state to: " + currentState);
    }
}
