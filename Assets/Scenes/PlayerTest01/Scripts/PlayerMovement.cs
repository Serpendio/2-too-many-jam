using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    public InputMaster controls;
    public Vector3 direction;

    SpriteRenderer spriteRenderer;
    Rigidbody2D body;

    float speed = 1f;

    // Awake is called even before Start() is called.
    void Awake()
    {
        // Need to create instance of inputs object - MUST be done first thing
        controls = new InputMaster();

        spriteRenderer = GetComponent<SpriteRenderer>();
        
        body = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        // Move Player
        var moveDirection = controls.Player.Movement.ReadValue<Vector2>();
        direction = new Vector3(moveDirection.x, moveDirection.y, 0 );
        transform.position += direction * speed * Time.deltaTime;

        body.velocity = new Vector2(moveDirection.x, 0f);
        spriteRenderer.flipX = body.velocity.x < 0f;

        /*
        // Sprite Direction
        if (direction.x < 0)
        {
            // Look Left
            spriteRenderer.flipX = true;
        }
        else
        {
            // Look Right
            spriteRenderer.flipX = false;
        }
        */
    }
}
