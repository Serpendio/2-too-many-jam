using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    public InputMaster controls;
    public Vector3 direction;

    float speed = 1f;

    // Awake is called even before Start() is called.
    void Awake()
    {
        // Need to create instance of inputs object - MUST be done first thing
        controls = new InputMaster();
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
        var moveDirection = controls.Player.Movement.ReadValue<Vector2>();
        direction = new Vector3(moveDirection.x, moveDirection.y, 0 );
        transform.position += direction * speed * Time.deltaTime;
    }
}
