using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class idle_state : FSM_BaseState
{
    private FSM_CharMovement my_sm;

    public idle_state(FSM_CharMovement myStateMachine) : base("idle_state", myStateMachine)
    {

        my_sm = (FSM_CharMovement)stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("ENTER() IDLE");
        horizontalInput = 0;
        jumpInput = false;
        dashInput = false;
        
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        my_sm.animator.Play("idle");
        //Debug.Log("tRANSFORM: " + my_sm.rigidBody.transform.position);
        handleStateInputs();

    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        //avoid moving in idle by error
        Vector2 velocity = my_sm.rigidBody.linearVelocity;
        velocity.x = 0 /*(-ChunksManager.Instance.Speed)*/;
        my_sm.rigidBody.linearVelocity = velocity;
    }
    public override void Exit()
    {
        base.Exit();
    }

    public void handleStateInputs()
    {
        //   ### --- ###
        horizontalInput = my_sm.inputAction_move.ReadValue<Vector2>().x;
        if (Mathf.Abs(horizontalInput) > Mathf.Abs(0.1f))
        {
            if(my_sm.getCurrState() != my_sm.jump) // si no estoy saltando cambiar a estado correr
            {
                stateMachine.ChangeState(my_sm.run);
            }      
        }
        //   ### --- ###
        float bJump = my_sm.inputAction_jump.ReadValue<float>();
        if (bJump == 1.0f)
        {
            jumpInput = true;
            stateMachine.ChangeState(my_sm.jump);
        }

        //   ### --- ###
        float bDash = my_sm.inputAction_dash.ReadValue<float>();
        if(bDash == 1.0f)
        {
            dashInput = true;
            stateMachine.ChangeState(my_sm.dash);
        }
    }


}

