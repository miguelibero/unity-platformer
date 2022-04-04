using UnityEngine;


[RequireComponent(typeof(CharacterController2D))]
public sealed class InputCharacterController : MonoBehaviour
{
    CharacterController2D _character;

    void Awake()
    {
        _character = GetComponent<CharacterController2D>();
    }

    enum ButtonInputState
    {
        None,
        PressedUp,
        PressedDown,
    }

    void Update()
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
    [SerializeField] float _runVelocity = 90f;
    [SerializeField] float _runHaltTime = 0.5f;
    [SerializeField] float _jumpApexRunFactor = 2f;

    void CalculateRun(float dt, float xinput)
    {
        if (!Mathf.Approximately(0.0f, xinput))
        {
            var speed = _runVelocity + _jumpApexRunFactor * _character.JumpAphexRatio;
            _character.Move(xinput * speed, _runHaltTime);
        }
    }

    [Header("Jumping")]
    [SerializeField] float _jumpVelocity = 30f;
    [SerializeField] float _jumpAphexGravityFactor = 0.8f;
    [SerializeField] float _shortJumpFactor = 0.3f;
    [SerializeField] float _coyoteBuffer = 0.1f;
    [SerializeField] float _jumpBuffer = 0.1f;

    bool _pendingJump;
    float _timeSinceLastJumpDownPressed;
    float _timeSinceLastGrounded;

    void CalculateJump(float dt, ButtonInputState input)
    {
        var grounded = _character.Grounded;
        if (_pendingJump && _timeSinceLastJumpDownPressed < _jumpBuffer)
        {
            _timeSinceLastJumpDownPressed += dt;
        }
        if (input == ButtonInputState.PressedDown)
        {
            _timeSinceLastJumpDownPressed = 0.0f;
            _pendingJump = true;
        }
        if (grounded)
        {
            _timeSinceLastGrounded = 0;
        }
        else if (_timeSinceLastGrounded < _coyoteBuffer)
        {
            _timeSinceLastGrounded += dt;
        }

        if (_pendingJump && _timeSinceLastJumpDownPressed < _jumpBuffer && _timeSinceLastGrounded < _coyoteBuffer)
        {
            _character.Jump(_jumpVelocity, _jumpAphexGravityFactor);
            _pendingJump = false;
        }
        if (!grounded && input == ButtonInputState.PressedUp)
        {
            _character.JumpFactor(_shortJumpFactor);
        }
    }
}
