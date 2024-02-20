using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public InputMaster controls;

    float speed = 10f; // Testing Transform

    // Awake is called even before Start() is called.
    void Awake()
    {
        // Need to create instance of inputs object - MUST be done first thing
        controls = new InputMaster();

        // Add Movement function to list of functions that should be called when movement action is triggered
        controls.Player.Movement.performed += context => Move(context.ReadValue<Vector2>());
    }

    void Move(Vector2 direction)
    {
        Debug.Log("Player is moving: " + direction); // DEBUG
      
        // Create a new Vector3 and get direction of vector using movement direction - Direction is -1 to 1 for both x and y.
        Vector3 move = new Vector3(direction.x, direction.y, 0); // transform only works with vector3's, just make z = 0

        // Move player by the direction multiplied by speed and DeltaTime (so it doesn't mess up with lag).
        transform.position += move * speed * Time.deltaTime;
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
