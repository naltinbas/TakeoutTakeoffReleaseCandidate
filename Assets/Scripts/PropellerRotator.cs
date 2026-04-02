using UnityEngine;

public class PropellerRotator : MonoBehaviour
{
    [Tooltip("Rotation angle in degrees per second")]
    private float _aps = 280f;

    [Tooltip("If true, ignores Time.timeScale (keeps spinning in pause).")]
    public bool useUnscaledTime = false;

    private bool _isSpinning = true;

    void Update()
    {
        if (!_isSpinning) return;

        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        transform.Rotate(Vector3.forward, _aps * dt, Space.Self);
    }

    public void StopRotation()
    {
        _isSpinning = false;
    }

    public void ResumeRotation()
    {
        _isSpinning = true;
    }
}
