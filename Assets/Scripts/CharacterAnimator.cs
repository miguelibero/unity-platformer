using UnityEngine;
using Random = UnityEngine.Random;


[RequireComponent(typeof(PlatformerRigidbody2D))]
public sealed class CharacterAnimator : MonoBehaviour
{
    [SerializeField] private Animator _anim;

    [SerializeField] private float _maxTilt = .1f;
    [SerializeField] private float _tiltSpeed = 1;
    [SerializeField] private float _minRunSpeed = 0.1f;

    [Header("Sound")]
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip[] _footsteps;

    private PlatformerRigidbody2D _rigidbody;

    private static readonly int _runningAnimKey = Animator.StringToHash("Running");
    private static readonly int _jumpingAnimKey = Animator.StringToHash("Jumping");

    void Awake()
    {
        _rigidbody = GetComponent<PlatformerRigidbody2D>();
    }

    void Update()
    {
        var dt = Time.deltaTime;
        UpdateAnimations(dt);
        UpdateEffects();
    }

    void UpdateAnimations(float dt)
    {
        var xinput = _rigidbody.RealVelocity.x;
        var jumping = !_rigidbody.Grounded;
        var running = !jumping && Mathf.Abs(xinput) >= _minRunSpeed;
        var lookingRight = xinput > 0.0f;
        if (running)
        {
            transform.localScale = new Vector3(lookingRight ? 1 : -1, 1, 1);
        }
        var rot = new Vector3(0, 0, Mathf.Lerp(-_maxTilt, _maxTilt, Mathf.InverseLerp(-1, 1, xinput)));
        _anim.transform.rotation = Quaternion.RotateTowards(_anim.transform.rotation, Quaternion.Euler(rot), _tiltSpeed * dt);

        _anim.SetBool(_runningAnimKey, running);
        _anim.SetBool(_jumpingAnimKey, jumping);
    }

    void UpdateEffects()
    {
        if (_rigidbody.EnteredGround)
        {
            if (_source != null &&_footsteps != null && _footsteps.Length > 0)
            {
                _source.PlayOneShot(_footsteps[Random.Range(0, _footsteps.Length)]);
            }
        }
    }
}
