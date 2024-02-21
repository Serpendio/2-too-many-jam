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
    [Space]
    [Header("Spell")]
    [SerializeField] GameObject projectile;
    [Header("Spell Info")]
    [SerializeField] private int maxMana;
    [SerializeField] private int mana;
    [SerializeField] private float castCooldown;
    [Header("Projectile Info")]
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float destroyTime;

    // Awake is called even before Start() is called.
    protected override void Awake()
    {
        base.Awake();
        
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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

        if (_playerInput.actions["Shoot"].triggered) {
            // Gets the point in world space of the cursor, then rotates the new projectile so that it is facing the mouse
            Vector2 direction = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            GameObject spellShot = Instantiate(projectile, transform.position, transform.rotation);

            spellShot.GetComponent<Transform>().rotation = Quaternion.Slerp(transform.rotation, rotation, 1);
            spellShot.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            spellShot.GetComponent<Rigidbody2D>().AddForce(spellShot.GetComponent<Transform>().up * projectileSpeed);
            Destroy(spellShot, destroyTime);
        }

        var move = _playerInput.actions["Movement"].ReadValue<Vector2>();
        UpdateDir(move);
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