using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.InputSystem;
using UnityEditor.Rendering;
using UnityEditor.ShaderGraph;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerMovement : MonoBehaviour
{
    public InputMaster controls;

    [Header("Movement Variables")]
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    private Vector2 moveVector;
    private float speed = 1f;
    // Direction for animation, should default to down
    [SerializeField] bool isFacingRight;
    [SerializeField] bool isFacingLeft;
    [SerializeField] bool isFacingUp;

    private Vector2 lastVector;


    // Dashing - Header + SerializeField creates dropdown in Player Inspector
    [Header("Dash Variables")]
    [SerializeField] bool canDash = true;
    [SerializeField] bool isDashing;
    [SerializeField] float dashPower;
    [SerializeField] float dashTime;
    [SerializeField] float dashCooldown;
    private TrailRenderer dashTrail;
    private float waitTime;

    // Awake is called even before Start() is called.
    void Awake()
    {
        // Need to create instance of inputs object - MUST be done first thing
        controls = new InputMaster();

        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        dashTrail = GetComponent<TrailRenderer>();
    }
    public void Move(InputAction.CallbackContext context)
    {
        moveVector = context.ReadValue<Vector2>();   
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

        rb.velocity = new Vector2(moveVector.x * speed, moveVector.y * speed);
        
        //Horizontal Direction
        if (moveVector.x > 0)
        {
            isFacingRight = true;
            isFacingLeft = false;
        }
        if (moveVector.x < 0)
        {
            isFacingLeft = true;
            isFacingRight = false;
        }
        else
        {
            isFacingLeft = false;
            isFacingRight = false;
        }
        //Up Direction
        if (moveVector.y < 0)
        {
            isFacingUp = true;
        }
        else
        {
            isFacingUp = false;
        }
    }

    private void Update()
    {
        // Stop player from moving if dashing
        if (isDashing)
        {
            return;
        }

        // TEMPORARY sprite flipping
        if (isFacingRight)
        {
            spriteRenderer.flipX = false;
        }
        else if (isFacingLeft)
        {
            spriteRenderer.flipX = true;
        }
        else if (isFacingUp)
        {
            spriteRenderer.flipY = true;
        }
        else
        {
            spriteRenderer.flipY = false;
            spriteRenderer.flipX = false;
        }
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

