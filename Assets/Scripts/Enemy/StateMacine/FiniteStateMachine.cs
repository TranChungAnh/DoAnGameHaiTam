using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Tham chiếu đến FSM (Máy trạng thái) chứa trạng thái này
public class FiniteStateMachine 
{
   public State currentState { get; private set; }

    public void Initialize(State startingState)
    {
        currentState = startingState;
        currentState.Enter();
    }

    public void ChangeState(State newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}
