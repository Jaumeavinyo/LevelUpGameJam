using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM : MonoBehaviour
{

    protected FSM_BaseState currentState;
    public FSM_BaseState previousState;
    void Start()
    {
        currentState = getInitialState();
        previousState = currentState;
        if (currentState != null)
        {
            currentState.Enter();
        }
    }
    virtual public void Update()
    {

    }
    private void LateUpdate()
    {
        if (currentState != null)
        {
            currentState.UpdatePhysics();
        }
    }
    public void ChangeState(FSM_BaseState newState)
    {
        currentState.Exit();
        previousState = currentState;
        currentState = newState;
        currentState.Enter();
    }

    protected virtual FSM_BaseState getInitialState()
    {
        return null;
    }

    private void OnGUI()
    {
        string state_string = currentState != null ? currentState.name : "(state null)";

        //GUILayout.Label(state_string);
    }
}