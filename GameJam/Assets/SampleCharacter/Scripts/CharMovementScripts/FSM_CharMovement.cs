
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FSM_CharMovement : FSM
{
    public ChunksManager CM;

    public InputAction inputAction_jump; //own input added to player inputs
    public InputAction inputAction_move;

    public InputAction inputAction_dash;
    public InputAction inputAction_heavy_attack;
    public InputAction inputAction_light_attack;

    public InputActionAsset inputActions;

    public idle_state idle;
    public run_state run;
    public jump_state jump;
    public dash_state dash;

    public Rigidbody2D rigidBody;
    public Animator animator;
    public CapsuleCollider2D capsuleCollider;

    public float speed = 5;
    public float dashSpeed = 10;
    public float jumpForce = 6;

    public float groundDistanceDetection = 1;

    public float directionInput;
    public float lastDirectionInput;

    public bool grounded;
    public bool wallGrabbed;

    public float gravityScale;
    public float dashJumpGravityScale;

    private void Awake()
    {

        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        CM = GetComponent<ChunksManager>();
        idle = new idle_state(this);
        run = new run_state(this);
        jump = new jump_state(this);
        dash = new dash_state(this);

        grounded = isGrounded();
        directionInput = 0;
        lastDirectionInput = 0;

    }

    public override void Update()
    {

        //CHUNKS BACK MOVEMENT
        Vector2 velDir = this.rigidBody.linearVelocity;
        velDir.x = CM.Speed;
        this.rigidBody.linearVelocity = velDir;

        if (inputAction_move.ReadValue<Vector2>().x > 0)
        {
            directionInput = 1;
        }
        else if (inputAction_move.ReadValue<Vector2>().x < 0)
        {
            directionInput = -1;
        }

        setDirection();
        Debug.Log("ApplyRootMotion = " + animator.applyRootMotion);
        grounded = isGrounded();
        //wallGrabbed = isWallGrabbed();

        if (currentState != null)
        {
            currentState.UpdateLogic();
        }

        if (currentState != jump && currentState != dash)
        {
            rigidBody.gravityScale = gravityScale;
        }
        else
        {
            rigidBody.gravityScale = dashJumpGravityScale;
        }

    }

    protected override FSM_BaseState getInitialState()
    {
        return idle;
    }


    public bool isGrounded()
    {
        bool ret = false;

        RaycastHit2D rayHit = Physics2D.Raycast(capsuleCollider.bounds.center, Vector2.down, capsuleCollider.bounds.extents.y + groundDistanceDetection);
        Color rayColor;

        if (rayHit.collider != null)
        {
            ret = true;
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }
        Debug.DrawRay(capsuleCollider.bounds.center, Vector2.down * (capsuleCollider.bounds.extents.y + groundDistanceDetection), rayColor);
        return ret;
    }

    //public grabb isWallGrabbed()
    //{
    //    grabb ret = grabb.noGrabb;

    //    RaycastHit2D RrayHit = Physics2D.Raycast(capsuleCollider.bounds.center, Vector2.right, capsuleCollider.bounds.extents.x + wallDistanceDetection);
    //    RaycastHit2D LrayHit = Physics2D.Raycast(capsuleCollider.bounds.center, Vector2.left, capsuleCollider.bounds.extents.x + wallDistanceDetection);

    //    Vector2 top = new Vector2(capsuleCollider.bounds.center.x, (capsuleCollider.bounds.center.y + (capsuleCollider.bounds.size.y / 2)));

    //    RaycastHit2D TRrayHit = Physics2D.Raycast(top, Vector2.right, capsuleCollider.bounds.extents.x + wallDistanceDetection);
    //    RaycastHit2D TLrayHit = Physics2D.Raycast(top, Vector2.left, capsuleCollider.bounds.extents.x + wallDistanceDetection);

    //    Color rayColor;

    //    if (RrayHit.collider != null)
    //    {
    //        ret = grabb.rightGrabb;
    //        rayColor = Color.green;
    //    }
    //    else if (RrayHit.collider == null)
    //    {
    //        rayColor = Color.red;
    //    }
    //    if (LrayHit.collider != null)
    //    {
    //        ret = grabb.leftGrabb;
    //        rayColor = Color.green;
    //    }
    //    else if (LrayHit.collider == null)
    //    {
    //        rayColor = Color.red;
    //    }

    //    if (TRrayHit.collider != null)
    //    {
    //        ret = grabb.topRightGrabb;
    //        rayColor = Color.green;
    //    }
    //    else if (RrayHit.collider == null)
    //    {
    //        rayColor = Color.red;
    //    }
    //    if (TLrayHit.collider != null)
    //    {
    //        ret = grabb.topLeftGrabb;
    //        rayColor = Color.green;
    //    }
    //    else if (LrayHit.collider == null)
    //    {
    //        rayColor = Color.red;
    //    }

    //    Debug.DrawRay(capsuleCollider.bounds.center, Vector2.right * (capsuleCollider.bounds.extents.x + wallDistanceDetection));
    //    Debug.DrawRay(capsuleCollider.bounds.center, Vector2.left * (capsuleCollider.bounds.extents.x + wallDistanceDetection));

    //    Debug.DrawRay(top, Vector2.right * (capsuleCollider.bounds.extents.x + wallDistanceDetection));
    //    Debug.DrawRay(top, Vector2.left * (capsuleCollider.bounds.extents.x + wallDistanceDetection));
    //    return ret;
    //}

    public void setDirection()
    {
        //no inputs yet
        if (directionInput == 0 && lastDirectionInput == 0)
        {
            directionInput = lastDirectionInput = gameObject.transform.localScale.x;
        }

        if (currentState == dash)
        {

            directionInput = dash.horizontalDash;
        }

        //existing inputs
        if (directionInput > 0)
        {
            gameObject.transform.localScale = new Vector3(1, 1, 1);
            lastDirectionInput = directionInput;
        }
        else if (directionInput < 0)
        {
            gameObject.transform.localScale = new Vector3(-1, 1, 1);
            lastDirectionInput = directionInput;
        }
        else if (directionInput == 0)
        {
            if (lastDirectionInput > 0)
            {
                directionInput = lastDirectionInput; //avoid dir = 0
                gameObject.transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                directionInput = lastDirectionInput; //avoid dir = 0
                gameObject.transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }

    public void setInvulnerability(bool condition)
    {
        capsuleCollider.enabled = !condition;
        gameObject.GetComponentInChildren<BoxCollider2D>().enabled = condition;
    }

    public FSM_BaseState getCurrState()
    {
        return currentState;
    }

    private void OnEnable()
    {
        InputActionMap inputActionsMap = inputActions.FindActionMap("Player", throwIfNotFound: true);

        inputAction_move = inputActionsMap.FindAction("Move", throwIfNotFound: true);
        inputAction_move.Enable();

        inputAction_jump = inputActionsMap.FindAction("Jump", throwIfNotFound: true);
        inputAction_jump.Enable();

        inputAction_dash = inputActionsMap.FindAction("Dash", throwIfNotFound: true);
        inputAction_dash.Enable();
        //inputAction_jump = inputActions.Player.jump;
        //inputAction_jump.Enable();

        //inputAction_roll = inputActions.Player.roll;
        //inputAction_roll.Enable();

        //inputAction_dash = inputActions.Player.dash;
        //inputAction_dash.Enable();

    }

    private void OnDisable()
    {
        inputAction_move.Disable();
        inputAction_jump.Disable();
        inputAction_dash.Disable();

        //inputAction_roll.Disable();
        //inputAction_dash.Disable();

    }
    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("Collided with: " + col.collider.name);
    }
}


