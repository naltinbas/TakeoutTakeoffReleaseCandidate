using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WelcomeScreen : MonoBehaviour
{
    [Header("Pulse Settings")]
    public float pulseSpeed = 2f;
    public float minScale = 0.95f;
    public float maxScale = 1.05f;

    private CanvasGroup _welcomeGroup;
    private TextMeshProUGUI _promptText;
    private RectTransform _textRect;

    private Vector3 _baseScale;
    private bool _isActive = true;

    void Awake()
    {
        CreateUI();
    }

    void Update()
    {
        if (!_isActive)
            return;

        // Pulse text
        float t = (Mathf.Sin(Time.unscaledTime * pulseSpeed) + 1f) * 0.5f;
        float scale = Mathf.Lerp(minScale, maxScale, t);
        _textRect.localScale = _baseScale * scale;

        // Detect any key
        if (Input.anyKeyDown)
            HideWelcomeScreen();
    }

    private void CreateUI()
    {
        // ---------- Canvas ----------
        GameObject canvasGO = new GameObject("WelcomeCanvas",
            typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999; // ensure it's on top

        CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // ---------- Load Image ----------
        Sprite welcomeSprite = Resources.Load<Sprite>("welcomeScreenWithoutText");
        if (welcomeSprite == null)
        {
            Debug.LogError("Could not find Resources/welcomeScreenWithoutText.png");
        }

        // ---------- Fullscreen Welcome Image ----------
        GameObject panelGO = new GameObject("WelcomeImage",
            typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
        panelGO.transform.SetParent(canvasGO.transform, false);

        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImage = panelGO.GetComponent<Image>();
        panelImage.sprite = welcomeSprite;
        panelImage.preserveAspect = false;   // fill the entire screen

        _welcomeGroup = panelGO.GetComponent<CanvasGroup>();

        // ---------- Load Font (LuckiestGuy) ----------
        TMP_FontAsset luckiestGuyFont = Resources.Load<TMP_FontAsset>("Fonts/LuckiestGuy-Regular SDF");
        if (luckiestGuyFont == null)
        {
            Debug.LogError("Could not find Resources/Fonts/LuckiestGuy.asset");
        }

        // ---------- Press Any Key Text ----------
        GameObject textGO = new GameObject("PressAnyKeyText", typeof(TextMeshProUGUI));
        textGO.transform.SetParent(panelGO.transform, false);

        _promptText = textGO.GetComponent<TextMeshProUGUI>();
        _promptText.text = "Press Any Key";
        _promptText.fontSize = 100f;
        _promptText.alignment = TextAlignmentOptions.Center;

        if (luckiestGuyFont != null)
        {
            _promptText.font = luckiestGuyFont;
        }

        _textRect = _promptText.rectTransform;

        // Stretch horizontally to full screen width
        _textRect.anchorMin = new Vector2(0f, 0.5f);
        _textRect.anchorMax = new Vector2(1f, 0.5f);
        _textRect.offsetMin = new Vector2(0f, 0f);
        _textRect.offsetMax = new Vector2(0f, 0f);

        // Position: 50 pixels below center
        _textRect.anchoredPosition = new Vector2(0f, -50f);

        _baseScale = _textRect.localScale;
    }

    private void HideWelcomeScreen()
    {
        _isActive = false;

        _welcomeGroup.alpha = 0f;
        _welcomeGroup.interactable = false;
        _welcomeGroup.blocksRaycasts = false;

        _welcomeGroup.gameObject.SetActive(false);
    }
}