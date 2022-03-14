using UnityEngine;


[RequireComponent(typeof(PlatformerRigidbody2D))]
public sealed class PatrolController : MonoBehaviour
{
    [SerializeField] float _speed = 13;
    [SerializeField] float _patrolDuration = 2;
    [SerializeField] float _waitDuration = 2;
    [SerializeField] Animator _animator;

    public PlatformerRigidbody2D Rigidbody { get; private set; }

    float _time;

    private void Awake()
    {
        Rigidbody = GetComponent<PlatformerRigidbody2D>();
    }

    private void Start()
    {
        _time = 0.0f;
    }

    private void Update()
    {
        _time += Time.deltaTime;
        var dx = 0.0f;
        if (_time < _patrolDuration)
        {
            dx = 1;
        }
        else if (_time >= _patrolDuration + _waitDuration && _time < 2 * _patrolDuration + _waitDuration)
        {
            dx = -1;
        }
        else if (_time > 2 * _patrolDuration + 2* _waitDuration)
        {
            _time = 0.0f;
        }

        Rigidbody.Velocity.x = dx * _speed;
    }
}
