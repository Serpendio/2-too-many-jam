using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : CreatureBase
{
    public InputMaster controls;

    [Header("Movement Variables")]
    Rigidbody2D rb;
    private Vector2 moveVector;


    // Dashing - Header + SerializeField creates dropdown in Player Inspector
    [Header("Dash Variables")]
    [SerializeField] bool canDash = true;
    [SerializeField] bool isDashing;
    [SerializeField] float dashPower;
    [SerializeField] float dashTime;
    [SerializeField] float dashCooldown;
    private TrailRenderer dashTrail;

    // Awake is called even before Start() is called.
    protected override void Awake()
    {
        base.Awake();

        // Need to create instance of inputs object - MUST be done first thing
        controls = new InputMaster();

        dashTrail = GetComponent<TrailRenderer>();
    }
    public void Move(InputAction.CallbackContext context)
    {
        moveVector = context.ReadValue<Vector2>();   
    }

    private void FixedUpdate()
    {

        
        //Horizontal Direction
        if (moveVector.x > 0)
        {
            isFacingRight = true;
        }
        if (moveVector.x < 0)
        {
            isFacingRight = false;
        }
        else
        {
            isFacingRight = false;
        }
    }

    private void Update()
    {
        // Stop player from moving if dashing
        if (isDashing)
        {
            return;
        }

        rb.velocity = new Vector2(moveVector.x * speed, moveVector.y * speed);
    }

    public void Dash(InputAction.CallbackContext context)
    {
        Debug.Log("Dash Function Triggered!");

        if (context.performed && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        rb.velocity = new Vector2(controls.Player.Movement.ReadValue<Vector2>().x * dashPower, controls.Player.Movement.ReadValue<Vector2>().y * dashPower);
        dashTrail.emitting = true;

        yield return new WaitForSeconds(dashTime);
        dashTrail.emitting = false;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;

    }
    
    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

}

