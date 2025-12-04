using UnityEngine;

public class PropellerRotator : MonoBehaviour
{
    [Tooltip("Rotation angle in degrees per second")]
    private float aps = 280f;

    [Tooltip("If true, ignores Time.timeScale (keeps spinning in pause).")]
    public bool useUnscaledTime = false;

    private bool isSpinning = true;

    void Update()
    {
        if (!isSpinning) return;

        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        transform.Rotate(Vector3.forward, aps * dt, Space.Self);
    }

    public void StopRotation()
    {
        isSpinning = false;
    }

    public void ResumeRotation()
    {
        isSpinning = true;
    }
}
