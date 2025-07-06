using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FSM_BaseState
{

    public string name;
    protected FSM stateMachine;
    public int internalCase;


    //INPUT
    public float horizontalInput;

    public bool jumpInput;
    public bool dashInput;
    public bool slideInput;

    public bool lightAttackInput;//not being used
    public bool heavyAttackInput;//not being used


    public FSM_BaseState(string name, FSM stateMachine, int case_ = 0)
    {
        this.name = name;
        this.stateMachine = stateMachine;
        this.internalCase = case_;
    }



    public virtual void Enter() { }
    public virtual void UpdateLogic() { }
    public virtual void UpdatePhysics() { }
    public virtual void Exit() { }


}
