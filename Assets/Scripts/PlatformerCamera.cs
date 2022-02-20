using UnityEngine;

public class PlatformerCamera : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector2 _offset;
    [SerializeField, Range(1, 20)] private float _smoothFactor;
    [SerializeField] private Collider2D _mapCollider;

    private Vector2 _min;
    private Vector2 _max;
    private bool _bounded;
    private Camera _camera;
    private Vector2 _screenSize;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void Start()
    {
        CalculateScreenSize();
        CalculateBounds();
    }

    void CalculateScreenSize()
    {
        var cam = _camera ?? Camera.main;
        var topright = new Vector3(cam.pixelWidth, cam.pixelHeight, cam.nearClipPlane);
        var botleft = new Vector3(0.0f, 0.0f, cam.nearClipPlane);
        _screenSize = cam.ScreenToWorldPoint(topright) - cam.ScreenToWorldPoint(botleft);
    }

    private void CalculateBounds()
    {
        _bounded = _mapCollider != null;
        if (!_bounded)
        {
            return;
        }
        var bounds = _mapCollider.composite != null ? _mapCollider.composite.bounds : _mapCollider.bounds;
        Vector3 offset = _screenSize * 0.5f;
        _max = bounds.max - offset;
        _min = bounds.min + offset;
    }

    private void FixedUpdate()
    {
        var dt = Time.fixedDeltaTime;
        Follow(dt);
    }

    private void Follow(float dt)
    {
        var pos = _target.position;
        var npos = new Vector2(pos.x, pos.y) + _offset;
        if (_bounded)
        {
            npos.x = Mathf.Clamp(npos.x, _min.x, _max.x);
            npos.y = Mathf.Clamp(npos.y, _min.y, _max.y);
        }
        npos = Vector2.Lerp(transform.position, npos, _smoothFactor * dt);
        transform.position = new Vector3(npos.x, npos.y, transform.position.z);
    }
}
