using UnityEngine;
using Random = UnityEngine.Random;


[RequireComponent(typeof(CharacterController2D))]
public sealed class CharacterAnimator : MonoBehaviour
{
    [SerializeField] Animator _anim;

    [SerializeField] float _minMoveVelocity = 0.1f;

    [Header("Sound")]
    [SerializeField] AudioSource _source;
    [SerializeField] AudioClip[] _footsteps;

    CharacterController2D _character;
    bool _wasGrounded;

    static readonly int _runningAnimKey = Animator.StringToHash("Running");
    static readonly int _jumpingAnimKey = Animator.StringToHash("Jumping");

    void Awake()
    {
        _character = GetComponent<CharacterController2D>();
    }

    void Update()
    {
        var dt = Time.deltaTime;
        UpdateAnimations(dt);
        UpdateEffects();
        _wasGrounded = _character.Grounded;
    }

    void UpdateAnimations(float dt)
    {
        var vx = _character.Velocity.x;
        var jumping = _character.Jumping;
        var moving = Mathf.Abs(vx) >= _minMoveVelocity;
        var lookingRight = vx > 0.0f;

        if (moving)
        {
            transform.localScale = new Vector3(lookingRight ? 1 : -1, 1, 1);
        }

        _anim.SetBool(_runningAnimKey, !jumping && moving);
        _anim.SetBool(_jumpingAnimKey, jumping);
    }

    void UpdateEffects()
    {
        if (_character.Grounded && !_wasGrounded)
        {
            if (_source != null &&_footsteps != null && _footsteps.Length > 0)
            {
                _source.PlayOneShot(_footsteps[Random.Range(0, _footsteps.Length)]);
            }
        }
    }
}
