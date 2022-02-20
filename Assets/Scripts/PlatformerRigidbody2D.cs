using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class PlatformerRigidbody2D : MonoBehaviour
{
    [SerializeField] private float _jumpApexThreshold = 10f;
    [SerializeField] private float _minSpeed = -40f;
    [SerializeField] private float _minGravity = 80f;
    [SerializeField] private float _maxGravity = 120f;
    [SerializeField] private LayerMask _terrainLayer = int.MaxValue;

    private Collider2D _collider;

    [NonSerialized]
    public Vector2 Velocity;

    public float JumpApex { get; private set; }
    public bool Grounded { get; private set; }

    public bool EnteredGround { get; private set; }

    public bool LeftGround { get; private set; }


    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    private static readonly RaycastHit2D[] _raycastHits = new RaycastHit2D[1];
    private static readonly Vector2[] _dirs = new []{ Vector2.down, Vector2.right, Vector2.up, Vector2.left };

    private void FixedUpdate()
    {
        var dt = Time.fixedDeltaTime;

        JumpApex = Mathf.InverseLerp(_jumpApexThreshold, 0, Mathf.Abs(Velocity.y));
        var gravity = Mathf.Lerp(_minGravity, _maxGravity, JumpApex);
        Velocity.y -= gravity * dt;
        Velocity.y = Mathf.Clamp(Velocity.y, _minSpeed, float.MaxValue);

        if(Mathf.Approximately(0.0f, Velocity.magnitude))
        {
            return;
        }

        var i = 0;
        var grounded = false;
        var filter = new ContactFilter2D();
        filter.layerMask = _terrainLayer;
        filter.useTriggers = false;
        foreach (var dir in _dirs)
        {
            var dist = Vector2.Dot(Velocity, dir) * dt;
            var count = _collider.Cast(dir, filter, _raycastHits, dist);
            if(count > 0)
            {
                if(i == 0)
                {
                    grounded = true;
                }
                var hit = _raycastHits[0];
                Velocity -= hit.normal * Vector2.Dot(Velocity, hit.normal);
            }
            i++;
        }

        var pos = Velocity * dt;
        transform.position += new Vector3(pos.x, pos.y, 0.0f);

        EnteredGround = grounded && !Grounded;
        LeftGround = !grounded && Grounded;
        Grounded = grounded;
    }

}
