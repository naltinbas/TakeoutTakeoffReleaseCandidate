using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

public class PauseMenuManager : MonoBehaviour
{
    private class OnRectChanged : MonoBehaviour
    {
        private RectTransform _rt;
        private Sprite _spr;
        private System.Action<RectTransform, Sprite> _resizer;

        public void Init(RectTransform rt, Sprite spr, System.Action<RectTransform, Sprite> resizer)
        { _rt = rt; _spr = spr; _resizer = resizer; }

        void OnRectTransformDimensionsChange()
        {
            if (_rt && _spr != null && _resizer != null) _resizer(_rt, _spr);
        }
    }
    private class ButtonClickFx : MonoBehaviour,
        IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [Header("Animation")]
        [SerializeField] float pressScale = 0.93f;
        [SerializeField] float downTime  = 0.06f;
        [SerializeField] float upTime    = 0.10f;

        [Header("Tint (optional)")]
        [SerializeField] bool  useTint   = false;
        [SerializeField] Color pressTint = new Color(0.95f, 0.95f, 0.95f, 1f);

        Vector3 _startScale;
        Image   _img;
        Color   _startColor;
        bool    _pressed;
        float   _t;           // 0..1 lerp param
        float   _dur;         // current anim duration
        Vector3 _targetScale;
        Color   _targetColor;

        void Awake()
        {
            _startScale = transform.localScale;
            _img = GetComponent<Image>();
            if (_img) _startColor = _img.color;
        }

        void Update()
        {
            if (_dur <= 0f) return;
            _t += Time.unscaledDeltaTime / _dur;
            float k = Mathf.Clamp01(_t);
            transform.localScale = Vector3.Lerp(transform.localScale, _targetScale, k);
            if (useTint && _img) _img.color = Color.Lerp(_img.color, _targetColor, k);
            if (k >= 1f) _dur = 0f;
        }

        public void OnPointerDown(PointerEventData e)
        {
            _pressed = true;
            _t = 0f; _dur = downTime;
            _targetScale = _startScale * pressScale;
            _targetColor = useTint ? pressTint : (_img ? _img.color : Color.white);
        }

        public void OnPointerUp(PointerEventData e) { Release(); }
        public void OnPointerExit(PointerEventData e) { Release(); }

        void Release()
        {
            if (!_pressed) return;
            _pressed = false;
            _t = 0f; _dur = upTime;
            _targetScale = _startScale;
            _targetColor = _startColor;
        }
    }
    
    private const float CardW = 960f, CardH = 800f;
    private const float FrameInset = 15f;
    private const float ContentInset = 20f;

    private const float PauseHeaderH = 110f;
    private const float VolumeHeaderH = 80f;
    private const float SliderRowH = 64f;
    private const float ButtonH = 92f;
    private const float RowGap = 16f;

    private const float ButtonWidthFrac = 0.78f;

    private const float HudBtn = 128f;

    private static readonly Color32 DarkBrown = new Color32(51, 29, 19, 255);
    private static readonly Color32 CardFill  = new Color32(250, 204, 70, 255);
    private static readonly Color32 TrackDark = new Color32(54, 28, 18, 255);
    private static readonly Color32 FillGold  = new Color32(255, 221, 89, 255);
    
    private static readonly Color32 TrackOuter = new Color32(43, 22, 14, 255);
    private static readonly Color32 TrackInner = new Color32(66, 34, 22, 255);
    private const float SliderOuterH = 36f;
    private const float SliderInnerH = 28f;
    private const float SliderFillInset = 6f;

    private Canvas _canvas;
    private RectTransform _cardRT, _contentRT;
    private GameObject _card, _overlay;
    private GameObject _btnClose, _btnMute;
    private Image _muteIcon;
    private Slider _volumeSlider;
    private bool _isPaused, _isMuted;
    private float _lastVolume = 1f;

    private Sprite _sprPauseMenu, _sprVolume, _sprBack, _sprExit, _icoClose, _icoMuted, _icoUnmuted, _sprBg, _sprResume, _sprControls;
    private GameObject _bg;

    private void Start()
    {
        _sprPauseMenu = LoadSprite("pauseMenu");
        _sprVolume = LoadSprite("volume");
        _sprResume = LoadSprite("LuckiestGuy_Buttons/resume_button");
        _sprControls = LoadSprite("LuckiestGuy_Buttons/controls_button");
        _sprBack = LoadSprite("LuckiestGuy_Buttons/main_menu_button");//LoadSprite("btn_back_to_main_menu");
        _sprExit = LoadSprite("LuckiestGuy_Buttons/quit_button");//LoadSprite("btn_quit");
        _icoUnmuted = LoadSprite("btn_unmuted");
        _icoMuted   = LoadSprite("btn_muted");
        _icoClose   = LoadSprite("btn_close");
        _sprBg     = LoadSprite("pauseMenuBackground");

        _lastVolume = Mathf.Clamp01(AudioListener.volume);

        CreateCanvas();
        CreateBackground();
        CreateOverlay();
        CreateCardAndContent();
        CreateHudButtons();
        BuildMenu();

        SetActiveUI(false);
    }

    private void CreateBackground()
    {
        void SizeImageToCover(RectTransform imgRt, Sprite spr)
        {
            if (spr == null) return;
            var parent = imgRt.parent as RectTransform;
            if (!parent) return;

            float pw = parent.rect.width;
            float ph = parent.rect.height;
            float arSprite = spr.rect.width / spr.rect.height;
            float arParent = pw / ph;

            float w, h;
            if (arSprite < arParent)
            {
                // sprite is relatively taller -> match parent width
                w = pw;
                h = pw / arSprite;
            }
            else
            {
                // sprite is relatively wider -> match parent height
                h = ph;
                w = ph * arSprite;
            }

            imgRt.sizeDelta = new Vector2(w, h);
        }
        _bg = UI("PauseBG", _canvas.transform);
        var rt = _bg.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);  // center
        rt.anchoredPosition = Vector2.zero;

        var img = _bg.AddComponent<Image>();
        img.sprite = _sprBg;
        img.type = Image.Type.Simple;
        img.preserveAspect = true;     // we’ll still set a cover size
        img.raycastTarget = false;

        // size it once now…
        SizeImageToCover(rt, _sprBg);

        // …and keep it correct when the canvas changes size/aspect
        _bg.AddComponent<OnRectChanged>().Init(rt, _sprBg, SizeImageToCover);

        // ensure it renders *behind* overlay and card
        _bg.transform.SetAsFirstSibling();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPaused) Resume(); else Pause();
        }
    }

    private void CreateCanvas()
    {
        var go = new GameObject("PauseMenuCanvas");
        _canvas = go.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 100;

        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        go.AddComponent<GraphicRaycaster>();
    }

    private void CreateOverlay()
    {
        _overlay = UI("Overlay", _canvas.transform);
        var img = _overlay.AddComponent<Image>();
        img.color = new Color(0,0,0,0.001f);
        Stretch(_overlay);
        _overlay.AddComponent<Button>().onClick.AddListener(Resume);
    }

    private void CreateCardAndContent()
    {
        _card = UI("Card", _canvas.transform);
        _cardRT = _card.GetComponent<RectTransform>();
        _cardRT.anchorMin = _cardRT.anchorMax = new Vector2(0.5f, 0.5f);
        _cardRT.sizeDelta = new Vector2(CardW, CardH);

        var frame = _card.AddComponent<Image>();
        frame.type = Image.Type.Sliced;
        frame.sprite = MakeRoundRect(384, 28f, DarkBrown);

        var inner = UI("Inner", _card.transform);
        var irt = inner.GetComponent<RectTransform>();
        irt.anchorMin = new Vector2(0,0); irt.anchorMax = new Vector2(1,1);
        irt.offsetMin = new Vector2(FrameInset, FrameInset);
        irt.offsetMax = new Vector2(-FrameInset, -FrameInset);

        var fill = inner.AddComponent<Image>();
        fill.type = Image.Type.Sliced;
        fill.sprite = MakeRoundRect(384, 24f, CardFill);

        var content = UI("Content", inner.transform);
        _contentRT = content.GetComponent<RectTransform>();
        _contentRT.anchorMin = new Vector2(0,0); _contentRT.anchorMax = new Vector2(1,1);
        _contentRT.offsetMin = new Vector2(ContentInset, ContentInset);
        _contentRT.offsetMax = new Vector2(-ContentInset, -ContentInset);
    }

    private void CreateHudButtons()
    {
        GameObject HudIconButton(Sprite iconSprite, Vector2 anchoredPos, UnityEngine.Events.UnityAction onClick)
        {
            var root = UI("HudIconButton", _canvas.transform);
            var rt = root.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(1, 1);
            rt.sizeDelta = new Vector2(HudBtn, HudBtn);
            rt.anchoredPosition = anchoredPos;

            // Use the sprite directly as the button visual
            var img = root.AddComponent<Image>();
            img.sprite = iconSprite;
            img.preserveAspect = true;
            img.raycastTarget = true;

            var btn = root.AddComponent<Button>();
            btn.transition = Selectable.Transition.None;
            btn.onClick.AddListener(onClick);

            AddClickFx(root); // keeps your nice scale animation on click
            return root;
        }
        // Padding from screen edges
        const float TopPad = 32f;
        const float RightPad = 32f;
        const float VerticalSpacing = 16f; // distance between X and speaker buttons

        // --- Close (X) button ---
        _btnClose = HudIconButton(_icoClose, Vector2.zero, Resume);
        var closeRT = _btnClose.GetComponent<RectTransform>();
        closeRT.anchorMin = closeRT.anchorMax = new Vector2(1, 1);
        closeRT.pivot = new Vector2(1, 1);
        closeRT.anchoredPosition = new Vector2(-RightPad, -TopPad);

        // --- Speaker button (below close) ---
        _btnMute = HudIconButton(_icoUnmuted, Vector2.zero, ToggleMute);
        var muteRT = _btnMute.GetComponent<RectTransform>();
        muteRT.anchorMin = muteRT.anchorMax = new Vector2(1, 1);
        muteRT.pivot = new Vector2(1, 1);
        muteRT.anchoredPosition = new Vector2(-RightPad, -TopPad - HudBtn - VerticalSpacing);

        _muteIcon = _btnMute.GetComponent<Image>();
        ApplyMuteSprite(false);
    }

    private float CalcButtonWidth(Sprite s, float h, float maxW)
    {
        float aspect = s ? (s.rect.width / s.rect.height) : 4f;
        return Mathf.Min(h * aspect, maxW);
    }

    private void BuildMenu()
    {
        Canvas.ForceUpdateCanvases();
        foreach (Transform c in _contentRT) Destroy(c.gameObject);

        float innerW = _contentRT.rect.width;
        float innerH = _contentRT.rect.height;
        float cursorY = innerH * 0.5f;

        var pause = ImageRow("PauseHeader", _sprPauseMenu, PauseHeaderH, 700f);
        PlaceRowBelow(pause, ref cursorY, PauseHeaderH, RowGap);

        var vol = ImageRow("VolumeHeader", _sprVolume, VolumeHeaderH, 520f);
        PlaceRowBelow(vol, ref cursorY, VolumeHeaderH, RowGap);

        float maxButtonW = innerW * ButtonWidthFrac;
        float wBack = CalcButtonWidth(_sprBack, ButtonH, maxButtonW);
        float wExit = CalcButtonWidth(_sprExit, ButtonH, maxButtonW);
        float commonW = Mathf.Min(wBack, wExit);

        var sRow = SliderRow("VolumeSlider", SliderRowH, commonW);
        PlaceRowBelow(sRow, ref cursorY, SliderRowH, RowGap);
        
        var btnResume = PngButton("ResumeButton", _sprResume, ButtonH, Resume);
        btnResume.sizeDelta = new Vector2(commonW, ButtonH);
        PlaceRowBelow(btnResume, ref cursorY, ButtonH, RowGap);
        
        var btnControls = PngButton("ControlsButton", _sprControls, ButtonH, () =>
        {
            ControlsScreen.hasAlreadyRevealed = false;
            var controlsScreen = gameObject.AddComponent<ControlsScreen>();
            controlsScreen.shouldStartGame = false;
        });
        btnControls.sizeDelta = new Vector2(commonW, ButtonH);
        PlaceRowBelow(btnControls, ref cursorY, ButtonH, RowGap);

        var btnBack = PngButton("BackButton", _sprBack, ButtonH, () =>
        {
            LevelManager.currentLevel = Level.TitleMenu;
            SceneManager.LoadScene("MainMenu");
        });
        btnBack.sizeDelta = new Vector2(commonW, ButtonH);
        PlaceRowBelow(btnBack, ref cursorY, ButtonH, RowGap);

        var btnExit = PngButton("ExitButton", _sprExit, ButtonH, Application.Quit);
        btnExit.sizeDelta = new Vector2(commonW, ButtonH);
        PlaceRowBelow(btnExit, ref cursorY, ButtonH, 0f);
    }

    private RectTransform ImageRow(string name, Sprite sprite, float targetH, float maxW)
    {
        var g = UI(name, _contentRT);
        var rt = g.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);

        var img = g.AddComponent<Image>();
        img.sprite = sprite;
        img.type = Image.Type.Simple;
        img.preserveAspect = true;

        float aspect = sprite ? (sprite.rect.width / sprite.rect.height) : 4f;
        float w = Mathf.Min(maxW, targetH * aspect);
        rt.sizeDelta = new Vector2(w, targetH);
        return rt;
    }

    private RectTransform SliderRow(string name, float rowH, float width)
    {
        var row = UI(name, _contentRT);
        var rt = row.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(width, rowH);

        var sliderGO = UI("Slider", row.transform);
        var srt = sliderGO.GetComponent<RectTransform>();
        Center(srt);
        srt.sizeDelta = new Vector2(width, SliderOuterH);

        _volumeSlider = sliderGO.AddComponent<Slider>();
        _volumeSlider.direction = Slider.Direction.LeftToRight;
        _volumeSlider.minValue = 0f; _volumeSlider.maxValue = 1f;
        _volumeSlider.wholeNumbers = false;
        _volumeSlider.transition = Selectable.Transition.None;

        var outer = UI("OuterTrack", sliderGO.transform);
        var outerImg = outer.AddComponent<Image>();
        outerImg.type = Image.Type.Sliced;
        outerImg.sprite = MakeRoundRect(256, 12f, TrackOuter);
        var outerRt = outer.GetComponent<RectTransform>();
        outerRt.anchorMin = new Vector2(0, 0.5f); outerRt.anchorMax = new Vector2(1, 0.5f);
        outerRt.sizeDelta = new Vector2(0, SliderOuterH);

        var inner = UI("InnerTrack", sliderGO.transform);
        var innerImg = inner.AddComponent<Image>();
        innerImg.type = Image.Type.Sliced;
        innerImg.sprite = MakeRoundRect(256, 11f, TrackInner);
        var innerRt = inner.GetComponent<RectTransform>();
        innerRt.anchorMin = new Vector2(0, 0.5f); innerRt.anchorMax = new Vector2(1, 0.5f);
        innerRt.sizeDelta = new Vector2(0, SliderInnerH);

        var fillMask = UI("FillMask", sliderGO.transform);
        var maskImg = fillMask.AddComponent<Image>();
        maskImg.type = Image.Type.Sliced;
        maskImg.sprite = MakeRoundRect(256, 11f, new Color32(255,255,255,255));
        var maskRt = fillMask.GetComponent<RectTransform>();
        maskRt.anchorMin = new Vector2(0, 0.5f); maskRt.anchorMax = new Vector2(1, 0.5f);
        maskRt.offsetMin = new Vector2(SliderFillInset, -SliderInnerH * 0.5f + SliderFillInset);
        maskRt.offsetMax = new Vector2(-SliderFillInset,  SliderInnerH * 0.5f - SliderFillInset);
        var rectMask = fillMask.AddComponent<Mask>();
        rectMask.showMaskGraphic = false;

        var fill = UI("Fill", fillMask.transform);
        var fImg = fill.AddComponent<Image>();
        fImg.type = Image.Type.Sliced;
        fImg.sprite = MakeRoundRect(256, 11f, FillGold);
        var fRt = fill.GetComponent<RectTransform>();
        fRt.anchorMin = new Vector2(0, 0); fRt.anchorMax = new Vector2(1, 1);
        fRt.sizeDelta = Vector2.zero;

        var hArea = UI("HandleArea", sliderGO.transform);
        var haRt = hArea.GetComponent<RectTransform>();
        haRt.anchorMin = new Vector2(0, 0.5f); haRt.anchorMax = new Vector2(1, 0.5f);
        haRt.sizeDelta = new Vector2(0, SliderOuterH);
        
        var handle = UI("Handle", hArea.transform);
        var hImg = handle.AddComponent<Image>();
        hImg.type = Image.Type.Sliced;
        hImg.sprite = MakeRoundRect(128, 14f, new Color32(255,255,255,230));
        hImg.color = new Color(1,1,1,0);
        var hRt = handle.GetComponent<RectTransform>();
        hRt.sizeDelta = new Vector2(26, 26);
        hRt.anchorMin = hRt.anchorMax = new Vector2(0, 0.5f);

        _volumeSlider.fillRect   = fRt;
        _volumeSlider.handleRect = hRt;
        _volumeSlider.value = _lastVolume;
        _volumeSlider.onValueChanged.RemoveAllListeners();
        _volumeSlider.onValueChanged.AddListener(v =>
        {
            _lastVolume = v;
            if (_isMuted) { _isMuted = false; ApplyMuteSprite(false); }
            AudioListener.volume = v;
        });

        return rt;
    }

    private RectTransform PngButton(string name, Sprite sprite, float h, UnityEngine.Events.UnityAction onClick)
    {
        var g = UI(name, _contentRT);
        var rt = g.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);

        var img = g.AddComponent<Image>();
        img.sprite = sprite; img.type = Image.Type.Simple; img.preserveAspect = true;

        var btn = g.AddComponent<Button>();
        var c = btn.colors;
        c.normalColor = Color.white;
        c.highlightedColor = new Color(1f,1f,1f,0.97f);
        c.pressedColor = new Color(1f,1f,1f,0.92f);
        btn.colors = c;
        btn.onClick.AddListener(onClick);
        btn.transition = Selectable.Transition.None;
        AddClickFx(g);
        rt.sizeDelta = new Vector2(h * (sprite ? sprite.rect.width / sprite.rect.height : 4f), h);
        return rt;
    }

    private void PlaceRowBelow(RectTransform rt, ref float cursorY, float rowHeight, float gap)
    {
        float centerY = cursorY - rowHeight * 0.5f;
        rt.anchoredPosition = new Vector2(0f, centerY);
        cursorY = centerY - rowHeight * 0.5f - gap;
    }

    private void Pause()
    {
        AudioSourceManager.PauseAudio();
        _isPaused = true;
        Time.timeScale = 0f;
        SetActiveUI(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        BuildMenu();
    }

    private void Resume()
    {
        AudioSourceManager.ResumeAudio();
        _isPaused = false;
        Time.timeScale = 1f;
        SetActiveUI(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void ToggleMute()
    {
        _isMuted = !_isMuted;
        if (_isMuted) { _lastVolume = AudioListener.volume; AudioListener.volume = 0f; }
        else AudioListener.volume = _lastVolume;
        ApplyMuteSprite(_isMuted);
    }

    private void SetActiveUI(bool on)
    {
        if (_bg)        
            _bg.SetActive(on);
        _card.SetActive(on);
        _overlay.SetActive(on);
        _btnClose.SetActive(on);
        _btnMute.SetActive(on);
    }

    private static Sprite MakeRoundRect(int size, float radius, Color32 fill)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;

        var px = new Color32[size * size];
        float r = Mathf.Max(1f, radius), r2 = r * r;
        int w = size, h = size;

        for (int y=0; y<h; y++)
        for (int x=0; x<w; x++)
        {
            float ix = Mathf.Clamp(x, (int)r, w-1-(int)r);
            float iy = Mathf.Clamp(y, (int)r, h-1-(int)r);
            float dx = x - ix, dy = y - iy;
            bool inside = (dx==0 && dy==0) || (dx*dx + dy*dy <= r2);
            px[y*w + x] = inside ? fill : new Color32(0,0,0,0);
        }
        tex.SetPixels32(px); tex.Apply();

        int b = Mathf.RoundToInt(radius);
        var border = new Vector4(b,b,b,b);

        return Sprite.Create(tex, new Rect(0,0,w,h), new Vector2(0.5f,0.5f), 100f, 0,
                             SpriteMeshType.FullRect, border, false);
    }

    private void ApplyMuteSprite(bool muted)
    {
        if (!_muteIcon) return;
        _muteIcon.sprite = muted ? _icoMuted : _icoUnmuted;
        _muteIcon.preserveAspect = true;
        _muteIcon.color = Color.white;
    }

    private static GameObject UI(string name, Transform parent)
    { var g = new GameObject(name); g.transform.SetParent(parent, false); g.AddComponent<RectTransform>(); return g; }

    private static void Stretch(GameObject go)
    { var rt = go.GetComponent<RectTransform>(); rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = rt.offsetMax = Vector2.zero; }

    private static void Center(RectTransform rt)
    { rt.anchorMin = rt.anchorMax = new Vector2(0.5f,0.5f); rt.anchoredPosition = Vector2.zero; }

    private TextMeshProUGUI TMPLabel(string text, Transform parent, float size, Color32 color)
    {
        var go = new GameObject("TMP"); go.transform.SetParent(parent,false);
        var t = go.AddComponent<TextMeshProUGUI>();
        t.text = text;
        t.font = TMP_Settings.defaultFontAsset;
        t.fontSize = size; t.color = color; t.alignment = TextAlignmentOptions.Center; t.enableWordWrapping = false; t.raycastTarget = false;
        return t;
    }

    private Sprite LoadSprite(string res) {
        var s = Resources.Load<Sprite>(res);
        if (!s) Debug.LogError($"Sprite not found at Resources/{res}.png");
        return s;
    }
    
    private void AddClickFx(GameObject go)
    {
        var fx = go.GetComponent<ButtonClickFx>();
        if (!fx) fx = go.AddComponent<ButtonClickFx>();
    }
}