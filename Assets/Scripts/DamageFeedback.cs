using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageFeedback : MonoBehaviour
{
    public static DamageFeedback Instance { get; private set; }

    [Header("Refs")]
    [SerializeField] private Image overlay;         // full-screen red Image
    [Header("Tuning")]
    [SerializeField] private float duration = 0.25f;
    [SerializeField, Range(0f, 1f)] private float maxAlpha = 0.35f;
    [SerializeField] private AnimationCurve fade = AnimationCurve.EaseInOut(0, 1, 1, 0);

    float _current;            // current displayed intensity (0..1)
    float _target;             // target intensity (0..1)
    Coroutine _routine;

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (overlay) SetAlpha(0f);
    }

    /// <param name="strength">0..1 (will be clamped). You can pass impulse intensity here.</param>
    public void Flash(float strength = 1f)
    {
        _target = Mathf.Clamp01(_target + strength);   // stack flashes
        if (_routine == null) _routine = StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        // Fade up quickly to _target, then fade out to 0 using curve over 'duration'
        // 1) Snap up (feels responsive)
        _current = Mathf.Max(_current, _target);
        SetAlpha(_current * maxAlpha);

        // 2) Fade out
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / duration);     // 0..1
            float a = _current * fade.Evaluate(k);     // curve handles shape
            SetAlpha(a * maxAlpha);
            yield return null;
        }

        // Reset
        _current = 0f;
        _target = 0f;
        SetAlpha(0f);
        _routine = null;
    }

    void SetAlpha(float a)
    {
        if (!overlay) return;
        var c = overlay.color;
        c.a = a;
        overlay.color = c;
    }
}
