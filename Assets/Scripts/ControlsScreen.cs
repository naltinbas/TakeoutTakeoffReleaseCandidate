using System;
using UnityEngine;
using UnityEngine.UI;

public class ControlsScreen : MonoBehaviour
{
    private GameObject _panel;
    public static bool hasAlreadyRevealed;
    public static event Action StartGame;
    public bool shouldStartGame;

    private void Awake()
    {
        if(hasAlreadyRevealed)
            return;
        // Load the controls image from Resources
        Sprite sprite = Resources.Load<Sprite>("controls");
        if (sprite == null)
        {
            Debug.LogError("Controls image not found in Resources/controls.png");
            return;
        }

        // Create a full-screen UI panel
        Canvas canvas = new GameObject("ControlsCanvas").AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        CanvasScaler scaler = canvas.gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvas.gameObject.AddComponent<GraphicRaycaster>();

        _panel = new GameObject("ControlsPanel");
        _panel.transform.SetParent(canvas.transform, false);

        Image image = _panel.AddComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = false;

        RectTransform rt = _panel.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        hasAlreadyRevealed = true;
    }

    private void Update()
    {
        if (Input.anyKeyDown && _panel)
        {
            Destroy(_panel.transform.parent.gameObject); // remove entire canvas
            if(shouldStartGame)
                StartGame?.Invoke();
        }
    }
}