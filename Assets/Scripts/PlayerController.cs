using UnityEngine;


[RequireComponent(typeof(PlatformerRigidbody2D))]
public sealed class PlayerController : MonoBehaviour
{
    public PlatformerRigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<PlatformerRigidbody2D>();
    }

    enum ButtonInputState
    {
        None,
        PressedUp,
        PressedDown,
    }

    private void Update()
    {
        var jump = ButtonInputState.None;
        if(Input.GetButtonDown("Jump"))
        {
            jump = ButtonInputState.PressedDown;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            jump = ButtonInputState.PressedUp;
        }
        var xinput = Input.GetAxisRaw("Horizontal");

        var dt = Time.deltaTime;
        CalculateRun(dt, xinput);
        CalculateJump(dt, jump);
    }

    [Header("Run")]
    [SerializeField] private float _acceleration = 90;
    [SerializeField] private float _maxRunSpeed = 13;
    [SerializeField] private float _deAcceleration = 60f;
    [SerializeField] private float _apexBonus = 2;

    private void CalculateRun(float dt, float xinput)
    {
        var speed = _rigidbody.Velocity;
        if (xinput != 0)
        {
            speed.x += xinput * _acceleration * dt;
            speed.x = Mathf.Clamp(speed.x, -_maxRunSpeed, _maxRunSpeed);
            var apexBonus = Mathf.Sign(xinput) * _apexBonus * _rigidbody.JumpApex;
            speed.x += apexBonus * dt;
        }
        else
        {
            speed.x = Mathf.MoveTowards(speed.x, 0, _deAcceleration * dt);
        }
        _rigidbody.Velocity = speed;
    }

    [Header("Jumping")]
    [SerializeField] private float _jumpHeight = 30;
    [SerializeField] private float _shortJumpFactor = 0.3f;
    [SerializeField] private float _coyoteBuffer = 0.1f;
    [SerializeField] private float _jumpBuffer = 0.1f;

    private bool _pendingJump;
    private float _timeSinceLastJumpDownPressed;
    private float _timeSinceLastGrounded;

    private void CalculateJump(float dt, ButtonInputState input)
    {
        if (_pendingJump && _timeSinceLastJumpDownPressed < _jumpBuffer)
        {
            _timeSinceLastJumpDownPressed += dt;
        }
        if (input == ButtonInputState.PressedDown)
        {
            _timeSinceLastJumpDownPressed = 0.0f;
            _pendingJump = true;
        }
        if (_rigidbody.Grounded)
        {
            _timeSinceLastGrounded = 0;
        }
        else if (_timeSinceLastGrounded < _coyoteBuffer)
        {
            _timeSinceLastGrounded += dt;
        }
        if (_pendingJump && _timeSinceLastJumpDownPressed < _jumpBuffer && _timeSinceLastGrounded < _coyoteBuffer)
        {
            var speed = _rigidbody.Velocity;
            speed.y = _jumpHeight;
            _rigidbody.Velocity = speed;
            _pendingJump = false;
        }
        
        if (!_rigidbody.Grounded && input == ButtonInputState.PressedUp && _rigidbody.Velocity.y > 0)
        {
            var speed = _rigidbody.Velocity;
            speed.y *= _shortJumpFactor;
            _rigidbody.Velocity = speed;
        }
    }
}
