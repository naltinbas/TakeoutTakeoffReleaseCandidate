using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class RotateAndGlow : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Rotation axis, e.g. (0,1,0) for Y-axis, (1,0,0) for X-axis, (0,0,1) for Z-axis")]
    [SerializeField] private Vector3 rotationAxis = Vector3.up;
    [SerializeField] private float rotationSpeed = 180f;

    [Header("Glow Settings")]
    [SerializeField] private Color glowColor = Color.yellow;
    [SerializeField] private float glowSpeed = 2f;
    [SerializeField] private float glowIntensity = 1f;
    
    [SerializeField] private bool shouldRotate = true;
    [SerializeField] private bool shouldGlow = true;

    private Material _material;
    private Color _baseEmission;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        _material = renderer.material;
        _baseEmission = glowColor;
        _material.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        if(shouldRotate)
            transform.Rotate(rotationAxis.normalized * rotationSpeed * Time.deltaTime, Space.Self);
        if(!shouldGlow) return;
        float emissionStrength = (Mathf.Sin(Time.time * glowSpeed) + 1f) / 2f * glowIntensity;
        Color finalColor = _baseEmission * Mathf.LinearToGammaSpace(emissionStrength);
        _material.SetColor("_EmissionColor", finalColor);
    }
}