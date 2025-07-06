using UnityEngine;

public class run_state : FSM_BaseState
{
    private FSM_CharMovement my_sm;

    public run_state(FSM_CharMovement myStateMachine) : base("idle_state", myStateMachine)
    {

        my_sm = (FSM_CharMovement)stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("ENTER() RUN");
        horizontalInput = my_sm.inputAction_move.ReadValue<Vector2>().x;
        jumpInput = false;
        dashInput = false;

    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if(horizontalInput == 0)
        {
            my_sm.ChangeState(my_sm.idle);
        }
        my_sm.animator.Play("run");

        handleStateInputs();

    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
     
        Vector2 velDir = my_sm.rigidBody.linearVelocity;
        velDir.x = my_sm.speed * horizontalInput;
        my_sm.rigidBody.linearVelocity = velDir;    
    }
    public override void Exit()
    {
        base.Exit();
    }

    public void handleStateInputs()
    {
        ////   ### --- ###
        horizontalInput = my_sm.inputAction_move.ReadValue<Vector2>().x;
        
        //   ### --- ###
        float bJump = my_sm.inputAction_jump.ReadValue<float>();
        if (bJump == 1.0f)
        {
            jumpInput = true;
            stateMachine.ChangeState(my_sm.jump);
        }

    }


}

