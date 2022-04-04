using UnityEngine;


[RequireComponent(typeof(CharacterController2D))]
public sealed class PatrolController : MonoBehaviour
{
    [SerializeField] float _velocity = 2;
    [SerializeField] float _patrolDuration = 2f;
    [SerializeField] float _waitDuration = 2f;

    CharacterController2D _character;

    float _time;

    private void Awake()
    {
        _character = GetComponent<CharacterController2D>();
    }

    private void Start()
    {
        _time = 0.0f;
    }

    private void Update()
    {
        var dt = Time.deltaTime;
        UpdatePosition(dt);
    }

    void UpdatePosition(float dt)
    { 
        _time += dt;
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
        _character.Move(_velocity*dx);
    }
}
