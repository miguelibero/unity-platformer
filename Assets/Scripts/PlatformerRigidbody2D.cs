using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public sealed class PlatformerRigidbody2D : MonoBehaviour
{
    [SerializeField] private PlatformerRigidbody2DSettings _settings;

    private Collider2D _collider;
    public Vector2 RealVelocity { get; private set; }

    [NonSerialized]
    public Vector2 Velocity;

    public float JumpApex { get; private set; }
    public bool Grounded { get; private set; }

    public bool EnteredGround { get; private set; }

    public bool LeftGround { get; private set; }


    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        var rigidbody = GetComponent<Rigidbody2D>();
        Debug.Assert(rigidbody.bodyType == RigidbodyType2D.Kinematic, "Rigidbody2D should be Kinematic for this one to work");
    }

    private static readonly RaycastHit2D[] _raycastHits = new RaycastHit2D[1];
    private static readonly Vector2[] _dirs = new []{ Vector2.down, Vector2.right, Vector2.up, Vector2.left };

    private void FixedUpdate()
    {
        var dt = Time.fixedDeltaTime;

        UpdatePosition(dt);
    }

    void UpdatePosition(float dt)
    {
        JumpApex = Mathf.InverseLerp(_settings.JumpApexThreshold, 0, Mathf.Abs(Velocity.y));
        var gravity = Mathf.Lerp(_settings.MinGravity, _settings.MaxGravity, JumpApex);
        Velocity.y -= gravity * dt;
        Velocity.y = Mathf.Clamp(Velocity.y, -_settings.MaxFallSpeed, float.MaxValue);

        if (Mathf.Approximately(0.0f, Velocity.magnitude))
        {
            return;
        }

        var grounded = false;
        var filter = new ContactFilter2D();
        filter.layerMask = _settings.TerrainLayer;
        filter.useTriggers = false;
        foreach (var dir in _dirs)
        {
            var dist = Vector2.Dot(Velocity, dir) * dt;
            if (_collider.Cast(dir, filter, _raycastHits, dist) == 0)
            {
                continue;
            }
            if (dir == Vector2.down)
            {
                grounded = true;
            }
            var hit = _raycastHits[0];
            if (Mathf.Approximately(0.0f, hit.distance))
            {
                // colliders are intersecting, in this case
                // don't fall and stop moving in the direction
                // of the other one
                var dx = hit.point.x - hit.centroid.x;
                if (dx * Velocity.x > 0.0f)
                {
                    Velocity.x = 0.0f;
                }
                if (Velocity.y < 0.0f)
                {
                    Velocity.y = 0.0f;
                }
                break;
            }
            var normal = hit.normal;
            Velocity -= normal * Vector2.Dot(Velocity, normal);
        }

        RealVelocity = Velocity;
        var pos = Velocity * dt;
        transform.position += new Vector3(pos.x, pos.y, 0.0f);

        EnteredGround = grounded && !Grounded;
        LeftGround = !grounded && Grounded;
        Grounded = grounded;
    }

}
