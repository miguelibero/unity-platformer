using UnityEngine;


[RequireComponent(typeof(PlatformerRigidbody2D))]
public sealed class PlayerController : MonoBehaviour
{
    public PlatformerRigidbody2D Rigidbody { get; private set; }
    public float HorizontalInput => _xInput;

    public bool Jumping { get; private set; }

    private float _xInput;
    private bool _jumpDownPressed;
    private bool _jumpUpPressed;

    private void Awake()
    {
        Rigidbody = GetComponent<PlatformerRigidbody2D>();
    }

    private void Update()
    {
        _jumpDownPressed = Input.GetButtonDown("Jump");
        _jumpUpPressed = Input.GetButtonUp("Jump");
        _xInput = Input.GetAxisRaw("Horizontal");

        var dt = Time.deltaTime;
        CalculateRun(dt);
        CalculateJump(dt);
    }


    [Header("Run")]
    [SerializeField] private float _acceleration = 90;
    [SerializeField] private float _maxRunSpeed = 13;
    [SerializeField] private float _deAcceleration = 60f;
    [SerializeField] private float _apexBonus = 2;

    private void CalculateRun(float dt)
    {
        var speed = Rigidbody.Velocity;
        if (_xInput != 0)
        {
            speed.x += _xInput * _acceleration * dt;
            speed.x = Mathf.Clamp(speed.x, -_maxRunSpeed, _maxRunSpeed);
            var apexBonus = Mathf.Sign(_xInput) * _apexBonus * Rigidbody.JumpApex;
            speed.x += apexBonus * dt;
        }
        else
        {
            speed.x = Mathf.MoveTowards(speed.x, 0, _deAcceleration * dt);
        }
        Rigidbody.Velocity = speed;
    }

    [Header("Jumping")]
    [SerializeField] private float _jumpHeight = 30;
    [SerializeField] private float _shortJumpFactor = 0.3f;
    [SerializeField] private float _coyoteBuffer = 0.1f;
    [SerializeField] private float _jumpBuffer = 0.1f;

    private bool _pendingJump;
    private float _timeSinceLastJumpDownPressed;
    private float _timeSinceLastGrounded;

    private void CalculateJump(float dt)
    {
        if (_pendingJump && _timeSinceLastJumpDownPressed < _jumpBuffer)
        {
            _timeSinceLastJumpDownPressed += dt;
        }
        if (_jumpDownPressed)
        {
            _timeSinceLastJumpDownPressed = 0.0f;
            _pendingJump = true;
        }
        if (Rigidbody.Grounded)
        {
            _timeSinceLastGrounded = 0;
        }
        else if (_timeSinceLastGrounded < _coyoteBuffer)
        {
            _timeSinceLastGrounded += dt;
        }
        if(Rigidbody.EnteredGround)
        {
            Jumping = false;
        }
        if (_pendingJump && _timeSinceLastJumpDownPressed < _jumpBuffer && _timeSinceLastGrounded < _coyoteBuffer)
        {
            var speed = Rigidbody.Velocity;
            speed.y = _jumpHeight;
            Rigidbody.Velocity = speed;
            _pendingJump = false;
            Jumping = true;
        }
        
        if (!Rigidbody.Grounded && _jumpUpPressed && Rigidbody.Velocity.y > 0)
        {
            var speed = Rigidbody.Velocity;
            speed.y *= _shortJumpFactor;
            Rigidbody.Velocity = speed;
        }
    }
}
