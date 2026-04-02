using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Singleton - full-screen red flash on damage.
public class DamageFeedback : Singleton<DamageFeedback>
{
    [Header("Refs")]
    [SerializeField] private Image overlay;

    [Header("Tuning")]
    [SerializeField] private float duration = 0.25f;
    [SerializeField, Range(0f, 1f)] private float maxAlpha = 0.35f;
    [SerializeField] private AnimationCurve fade = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private float _current;
    private float _target;
    private Coroutine _routine;

    protected override void Awake()
    {
        base.Awake();
        if (overlay) SetAlpha(0f);
    }

    public void Flash(float strength = 1f)
    {
        _target = Mathf.Clamp01(_target + strength);
        if (_routine == null) _routine = StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        _current = Mathf.Max(_current, _target);
        SetAlpha(_current * maxAlpha);

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / duration);
            float a = _current * fade.Evaluate(k);
            SetAlpha(a * maxAlpha);
            yield return null;
        }

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
