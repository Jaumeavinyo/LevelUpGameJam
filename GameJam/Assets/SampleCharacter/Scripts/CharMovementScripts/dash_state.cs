using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dash_state : FSM_BaseState
{
    private FSM_CharMovement my_sm;

    public int horizontalDash;
    public int dashEnergy;
    public float maxDashTime = 1.5f;
    public bool dashing;
    public bool stopDash;
    public dash_state(FSM_CharMovement myStateMachine) : base("dash_state", myStateMachine)
    {

        my_sm = (FSM_CharMovement)stateMachine;
    }


    public override void Enter()
    {
        base.Enter();
        jumpInput = false;
        dashInput = false;

        horizontalInput = 0;
        dashing = false;
        stopDash = false;
        dashEnergy = 200;
        //dash can not change its direction. we decide direction at entrance of the state
        if (my_sm.lastDirectionInput > 0)
        {
            horizontalDash = 1;
        }
        else if (my_sm.lastDirectionInput < 0)
        {
            horizontalDash = -1;
        }
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        if (!dashing)
        {
            dashing = true;
            dash(horizontalDash);
            my_sm.animator.Play("dash");
            
            //my_sm.audioSFX.playSound(my_sm.audioSFX.dash1);
        }
        else if (!dashing)
        {
            my_sm.ChangeState(my_sm.idle);
        }

        if((horizontalInput > 0 && horizontalDash < 0)||((horizontalInput < 0 && horizontalDash > 0)))//player cancels dash
        {
            chooseStateAfterDash();
        }

        if (my_sm.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > maxDashTime)
        {
            stopDash = true;
        }
        if (dashing && my_sm.animator.GetCurrentAnimatorStateInfo(0).IsName("dash") && stopDash )
        {
            chooseStateAfterDash();
        }

        handleStateInputs();
    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();

    }
    public override void Exit()
    {
        base.Exit();

        dashInput = false;
    }

    public void dash(int dir)
    {
        Vector2 velDir = my_sm.rigidBody.linearVelocity;
        velDir.x = my_sm.dashSpeed * dir /*+ (-ChunksManager.Instance.Speed)*/;
        my_sm.rigidBody.linearVelocity = velDir;
    }
    public void handleStateInputs()
    {
        ////   ### --- ###
        horizontalInput = my_sm.inputAction_move.ReadValue<Vector2>().x;

        //   ### --- ###
        //float bJump = my_sm.inputAction_jump.ReadValue<float>();
        //if (bJump == 1.0f)
        //{
        //    jumpInput = true;
        //    stateMachine.ChangeState(my_sm.jump);
        //}      
        
    }
    public void chooseStateAfterDash()
    {
        if (horizontalInput != 0)
        {
            if (!my_sm.grounded)
            {
                my_sm.ChangeState(my_sm.jump);
            }
            else
            {
                my_sm.ChangeState(my_sm.run);
            }
        }
        else if (horizontalInput == 0)
        {
            if (!my_sm.grounded)
            {
                my_sm.ChangeState(my_sm.jump);
            }
            else
            {
                my_sm.ChangeState(my_sm.idle);
            }
        }
    }

}

