using UnityEngine;
using Random = UnityEngine.Random;

public sealed class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    [SerializeField] private AudioSource _source;
    [SerializeField] private ParticleSystem _jumpParticles, _launchParticles;
    [SerializeField] private ParticleSystem _moveParticles, _landParticles;
    [SerializeField] private AudioClip[] _footsteps;
    [SerializeField] private float _maxTilt = .1f;
    [SerializeField] private float _tiltSpeed = 1;
    [SerializeField, Range(1f, 3f)] private float _maxAnimationSpeed = 2;
    [SerializeField] private float _maxParticleFallSpeed = -40;

    private PlayerController _player;
    private bool _lookingRight;
    private bool _running;

    private static readonly int _runningAnimKey = Animator.StringToHash("Running");
    private static readonly int _speedAnimKey = Animator.StringToHash("Speed");
    private static readonly int _jumpingAnimKey = Animator.StringToHash("Jumping");

    void Awake()
    {
        _player = GetComponentInParent<PlayerController>();
        if(_player == null)
        {
            throw new MissingComponentException("the parend should have a PlayerController");
        }
    }

    void Update()
    {
        var dt = Time.deltaTime;
        UpdateAnimations(dt);
        UpdateEffects();
    }

    void UpdateAnimations(float dt)
    {
        var xinput = _player.HorizontalInput;
        _running = !Mathf.Approximately(0.0f, xinput);
        _lookingRight = xinput > 0.0f;
        if (_running)
        {
            transform.localScale = new Vector3(_lookingRight ? 1 : -1, 1, 1);
        }
        var rot = new Vector3(0, 0, Mathf.Lerp(-_maxTilt, _maxTilt, Mathf.InverseLerp(-1, 1, xinput)));
        _anim.transform.rotation = Quaternion.RotateTowards(_anim.transform.rotation, Quaternion.Euler(rot), _tiltSpeed * dt);
        
        _anim.SetFloat(_speedAnimKey, Mathf.Lerp(1, _maxAnimationSpeed, Mathf.Abs(xinput)));
        _anim.SetBool(_runningAnimKey, _running);
        _anim.SetBool(_jumpingAnimKey, _player.Jumping);
    }

    void UpdateEffects()
    {
        if (_moveParticles != null)
        {
            if (_running)
            {
                var shape = _moveParticles.shape;
                shape.rotation = new Vector3(0.0f, 0.0f, _lookingRight ? 90f : 270f);
            }
            var emission = _moveParticles.emission;
            emission.enabled = _player.Rigidbody.Grounded && _running;
        }

        if (_player.Rigidbody.EnteredGround)
        {
            if (_footsteps != null && _footsteps.Length > 0)
            {
                _source.PlayOneShot(_footsteps[Random.Range(0, _footsteps.Length)]);
            }
            if (_landParticles != null)
            {
                _landParticles.transform.localScale = Vector3.one * Mathf.InverseLerp(0, _maxParticleFallSpeed, _player.Rigidbody.Velocity.y);
                _landParticles.Play();
            }
        }
        else if (_player.Rigidbody.LeftGround)
        {
            if (_jumpParticles != null)
            {
                _jumpParticles.Play();
            }
        }
    }
}
