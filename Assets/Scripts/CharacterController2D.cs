using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class CharacterController2D : MonoBehaviour
{
    Rigidbody2D _rigidBody;

    /* normalized value representing the percentage of the jump
     * 0 when the jump starts
     * 1 when the jump is in the aphex
     * 0 when the jump finishes
     */
    public float JumpAphexRatio { get; private set; }

    public float MaxFallSpeed = 40f;
    public float Gravity = 10f;

    float _jumpVelocity = 0.0f;
    float _jumpAphexGravityFactor = 1.0f;

    float _moveVelocity = 0.0f;
    float _moveMaxVelocity = Mathf.Infinity;
    float _moveHaltTime;


    public bool CollidingUp => _colliding[0] != null;

    public bool CollidingRight => _colliding[1] != null;

    public bool CollidingDown => _colliding[2] != null;

    public bool CollidingLeft => _colliding[3] != null;

    public bool Grounded => CollidingDown;


    public bool Jumping => _jumpVelocity > 0.0f;

    public Vector2 Velocity => _rigidBody.velocity;

    [SerializeField]
    GameObject[] _colliding = new GameObject[4];

    static readonly Vector2[] _dirs = new[] { Vector2.up, Vector2.right, Vector2.down, Vector2.left};

    const float _dirDotThreshold = 0.9f;

    void OnCollisionStay2D(Collision2D collision)
    {
        CleanColliding(collision.gameObject);
        foreach(var contact in collision.contacts)
        {
            for (var i = 0; i < _dirs.Length; i++)
            {
                var dir = _dirs[i];
                if (Vector2.Dot(contact.normal, -dir) > _dirDotThreshold)
                {
                    _colliding[i] = collision.gameObject;
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        CleanColliding(collision.gameObject);
    }

    void CleanColliding(GameObject gameObject)
    {
        for (var i = 0; i < _colliding.Length; i++)
        {
            if (_colliding[i] == gameObject)
            {
                _colliding[i] = null;
            }
        }
    }

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _rigidBody.gravityScale = 0.0f;
        _rigidBody.freezeRotation = true;
        _rigidBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void FixedUpdate()
    {
        var dt = Time.fixedDeltaTime;
        UpdateRigidBody(dt);
    }

    void UpdateJump(Vector2 v)
    {
        var finishedJump = _jumpVelocity > 0.0f && v.y < -_jumpVelocity;
        var justGrounded = v.y <= 0.0f && Grounded;
        if (finishedJump || justGrounded)
        {
            _jumpVelocity = 0.0f;
        }
        JumpAphexRatio = Mathf.InverseLerp(_jumpVelocity, 0, Mathf.Abs(v.y));
    }

    Vector2 UpdateGravity(Vector2 v, float dt)
    {
        var g = Gravity;
        var gravity = Mathf.Lerp(g, g * _jumpAphexGravityFactor, JumpAphexRatio);
        v.y -= Mathf.Clamp(gravity * dt, 0, MaxFallSpeed);
        return v;
    }

    bool CanMove(float v)
    {
        return !((CollidingLeft && v < 0.0f) || (CollidingRight && v > 0.0f));
    }

    Vector2 UpdateMove(Vector2 v, float dt)
    {
        if(!CanMove(v.x))
        {
            v.x = 0.0f;
            _moveVelocity = 0.0f;
        }
        else
        {
            v.x = Mathf.SmoothDamp(v.x, 0, ref _moveVelocity, _moveHaltTime, _moveMaxVelocity, dt);
        }
        return v;
    }

    void UpdateRigidBody(float dt)
    {
        var v = _rigidBody.velocity;
        UpdateJump(v);
        v = UpdateGravity(v, dt);
        v = UpdateMove(v, dt);
        _rigidBody.velocity = v;
    }

    public bool Move(float velocity, float haltTime = 0.5f, float maxVelocity = Mathf.Infinity)
    {
        Debug.Assert(haltTime >= 0.0f);
        if(!CanMove(velocity))
        {
            return false;
        }
        _moveVelocity = velocity;
        _moveHaltTime = haltTime;
        _moveMaxVelocity = maxVelocity;
        var v = _rigidBody.velocity;
        v.x = velocity;
        _rigidBody.velocity = v;
        return true;
    }

    public void Jump(float velocity, float aphexGravityFactor = 1.0f)
    {
        Debug.Assert(velocity > 0.0f);
        Debug.Assert(aphexGravityFactor > 0.0f && aphexGravityFactor <= 1.0f);
        _jumpVelocity = velocity;
        _jumpAphexGravityFactor = aphexGravityFactor;
        var v = _rigidBody.velocity;
        v.y = velocity;
        _rigidBody.velocity = v;
    }

    public void JumpFactor(float factor)
    {
        Debug.Assert(factor > 0.0f && factor <= 1.0f);
        var v = _rigidBody.velocity;
        if (v.y > 0.0f && _jumpVelocity > 0.0f)
        {
            v.y *= factor;
            _rigidBody.velocity = v;
        }
    }
}
