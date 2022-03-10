using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/PlatformerRigidbody2DSettings")]
public sealed class PlatformerRigidbody2DSettings : ScriptableObject
{
    public float JumpApexThreshold = 10f;
    public float MinSpeed = -40f;
    public float MinGravity = 80f;
    public float MaxGravity = 120f;
    public LayerMask TerrainLayer = int.MaxValue;
}
