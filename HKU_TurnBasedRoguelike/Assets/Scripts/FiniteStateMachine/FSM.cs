using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FSM<T>
{
    public T Owner;

    public Dictionary<Type, State<T>> states;
    private State<T> currentState;

    public FSM(T owner)
    {
        this.Owner = owner;
        states = new Dictionary<Type, State<T>>();

        // Add instances of all classes that inherit from type State.
        states.Add(typeof(MainMenuState<T>), new MainMenuState<T>(this));
        states.Add(typeof(PlayingState<T>), new PlayingState<T>(this));

        currentState = null;
    }

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
