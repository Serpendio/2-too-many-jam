using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : CreatureBase
{
    private PlayerInput _playerInput;

    [Header("Movement Variables")]
    private Rigidbody2D _rb;

    // Dashing - Header + SerializeField creates dropdown in Player Inspector
    [Header("Dash Variables")] [SerializeField]
    private bool canDash = true;

    [SerializeField] private bool isDashing;
    [SerializeField] private float dashPower;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;
    private TrailRenderer dashTrail;

    // Awake is called even before Start() is called.
    protected override void Awake()
    {
        base.Awake();

        _rb = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        dashTrail = GetComponent<TrailRenderer>();
    }

    private void Update()
    {
        // Stop player from moving if dashing
        if (isDashing)
        {
            return;
        }

        var move = _playerInput.actions["Movement"].ReadValue<Vector2>();
        _rb.velocity = new Vector2(move.x * speed, move.y * speed);
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
        var move = _playerInput.actions["Movement"].ReadValue<Vector2>();
        _rb.velocity = new Vector2(move.x * dashPower, move.y * dashPower);
        dashTrail.emitting = true;

        yield return new WaitForSeconds(dashTime);
        dashTrail.emitting = false;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void OnEnable()
    {
        _playerInput.enabled = true;
    }

    private void OnDisable()
    {
        _playerInput.enabled = false;
    }
}