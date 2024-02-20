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
    public Vector3 direction;

    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;

    private float speed = 1f;

    /* // Currently not in use, this is for later
    bool isFacingRight;
    bool isFacingUp;
    */

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

        //controls.Player.Dash.performed += context => Dash();
    }

    private void Update()
    {
        // Stop player from moving if dashing
        if (isDashing)
        {
            return;
        }

        // Move Player
        var moveDirection = controls.Player.Movement.ReadValue<Vector2>();
        direction = new Vector3(moveDirection.x, moveDirection.y, 0 );
        transform.position += direction * speed * Time.deltaTime;

        // Flip Sprite Horizontally If Move Left
        rb.velocity = new Vector2(moveDirection.x, 0f);
        spriteRenderer.flipX = rb.velocity.x < 0f;     
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
        rb.velocity = new Vector2(transform.localScale.x * dashPower, 0);
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

