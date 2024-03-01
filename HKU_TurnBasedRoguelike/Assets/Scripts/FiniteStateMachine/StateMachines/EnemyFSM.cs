using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFSM : FSM<EnemyBehaviour>
{
    public EnemyFSM(EnemyBehaviour owner) : base(owner)
    {
        this.owner = owner;

        // Add instances of all classes that inherit from type State.
        states.Add(typeof(EnemyIdle), new EnemyIdle(this));
        states.Add(typeof(EnemyChase), new EnemyChase(this));
    }
}
