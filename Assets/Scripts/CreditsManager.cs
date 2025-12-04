using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreditsScreen : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float fadeDuration = 0.5f;   // per-line fade
    [SerializeField] private float gapBetween   = 0.35f;  // delay between lines

    private Canvas _canvas;
    private GameObject _root;
    private Image _backdrop;
    private RectTransform _content;
    private TextMeshProUGUI _pressAnyKey;
    private readonly List<TextMeshProUGUI> _lines = new();

    private bool _showing;
    private static bool hasAlreadyDisplayed;
    private Coroutine _runner;

    private void Awake()
    {
        if(hasAlreadyDisplayed)
            return;
        _root = new GameObject("CreditsCanvas");
        _canvas = _root.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 999;
        var scaler = _root.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        _root.AddComponent<GraphicRaycaster>();
        
        var bgGO = new GameObject("Backdrop");
        bgGO.transform.SetParent(_root.transform, false);
        _backdrop = bgGO.AddComponent<Image>();
        _backdrop.color = new Color(60f / 255f, 176f / 255f, 197f / 255f, 1f);
        var bgRT = _backdrop.rectTransform;
        bgRT.anchorMin = Vector2.zero; bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero; bgRT.offsetMax = Vector2.zero;

        var contentGO = new GameObject("Content");
        contentGO.transform.SetParent(_root.transform, false);
        _content = contentGO.AddComponent<RectTransform>();
        _content.anchorMin = _content.anchorMax = new Vector2(0.5f, 0.5f);
        _content.pivot = new Vector2(0.5f, 0.5f);
        _content.anchoredPosition = Vector2.zero;
        _content.sizeDelta = new Vector2(1200f, 800f);

        var layout = contentGO.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.spacing = 16f;
        layout.padding = new RectOffset(0, 0, 0, 0);

        var footerGO = new GameObject("FooterText");
        footerGO.transform.SetParent(_root.transform, false);
        _pressAnyKey = footerGO.AddComponent<TextMeshProUGUI>();
        _pressAnyKey.text = "Press Any Key to Continue";
        _pressAnyKey.fontSize = 38;
        _pressAnyKey.alignment = TextAlignmentOptions.Center;
        _pressAnyKey.color = new Color(1, 0, 0, 0f);
        _pressAnyKey.raycastTarget = false;

        var fRT = _pressAnyKey.rectTransform;
        fRT.anchorMin = fRT.anchorMax = new Vector2(0.5f, 0);
        fRT.pivot = new Vector2(0.5f, 0f);
        fRT.anchoredPosition = new Vector2(0, 60f);
        fRT.sizeDelta = new Vector2(1200f, 100f);
    
        Show();
    }

    private void Update()
    {
        if (_showing && Input.anyKeyDown)
            Hide();
    }
    
    private void Show()
    {
        if (_showing) return;
        _showing = true;
        hasAlreadyDisplayed = true;

        foreach (Transform c in _content) Destroy(c.gameObject);
        _lines.Clear();

        AddHeader("USC Games:");
        AddName("Altinbas, Necati");
        AddName("Perazzini, Frank");
        AddName("Sivapalan, Siranjiv");

        AddHeader("Berklee College of Music:");
        AddName("Hsu, Shih-Shiou");
        
        AddSpacer(10f);

        if (_runner != null) StopCoroutine(_runner);
        _runner = StartCoroutine(FadeSequence());
    }
    
    private void Hide()
    {
        if (!_showing) return;
        if (_runner != null) StopCoroutine(_runner);
        _showing = false;
        _root.SetActive(false);
        gameObject.AddComponent<WelcomeScreen>();
    }

    private  void AddHeader(string text)
    {
        var t = MakeTMP(text, 52, FontStyles.Bold);
        t.color = new Color(1f, 0f, 0f, 0f);
        _lines.Add(t);
    }

    private void AddName(string text)
    {
        var t = MakeTMP(text, 44, FontStyles.Normal);
        _lines.Add(t);
    }

    private void AddSpacer(float h)
    {
        var g = new GameObject("Spacer");
        var rt = g.AddComponent<RectTransform>();
        g.transform.SetParent(_content, false);
        rt.sizeDelta = new Vector2(10, h);
    }

    private TextMeshProUGUI MakeTMP(string text, float size, FontStyles style)
    {
        var go = new GameObject("TMP");
        go.transform.SetParent(_content, false);
        var t = go.AddComponent<TextMeshProUGUI>();
        t.text = text;
        t.alignment = TextAlignmentOptions.Center;
        t.fontSize = size;
        t.fontStyle = style;
        var c = Color.white; c.a = 0f;
        t.color = c;
        t.enableWordWrapping = false;
        t.raycastTarget = false;
        return t;
    }

    private IEnumerator FadeSequence()
    {
        foreach (var t in _lines)
        {
            yield return StartCoroutine(FadeTextAlpha(t, 0f, 1f, fadeDuration));
            yield return new WaitForSecondsRealtime(gapBetween);
        }

        yield return new WaitForSecondsRealtime(0.5f);
        yield return StartCoroutine(FadeTextAlpha(_pressAnyKey, 0f, 1f, 0.7f));
    }

    private IEnumerator FadeTextAlpha(TextMeshProUGUI t, float a0, float a1, float dur)
    {
        float e = 0f;
        var c = t.color;
        while (e < dur)
        {
            e += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(e / dur);
            c.a = Mathf.Lerp(a0, a1, k);
            t.color = c;
            yield return null;
        }
        c.a = a1; t.color = c;
    }
}