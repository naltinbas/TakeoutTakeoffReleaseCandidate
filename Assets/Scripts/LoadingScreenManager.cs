using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour
{
    // Indexed by level number - replaces per-field switch statement
    private Sprite[] _loadingScreenSprites;

    private GameObject _loadingCanvas;
    private Image _loadingImage;

    private void Awake()
    {
        _loadingScreenSprites = new Sprite[]
        {
            null, // index 0 - TitleMenu has no loading screen
            Resources.Load<Sprite>("loadingScreenLevelOne"),
            Resources.Load<Sprite>("loadingScreenLevelTwo"),
            Resources.Load<Sprite>("loadingScreenLevelThree")
        };

        _loadingCanvas = new GameObject("LoadingScreenCanvas");
        var canvas = _loadingCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 500;

        _loadingCanvas.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        _loadingCanvas.AddComponent<GraphicRaycaster>();

        var imageGO = new GameObject("LoadingImage");
        imageGO.transform.SetParent(_loadingCanvas.transform, false);

        _loadingImage = imageGO.AddComponent<Image>();
        _loadingImage.preserveAspect = false;

        RectTransform rt = _loadingImage.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        _loadingCanvas.SetActive(false);
    }

    public void ShowLoadingScreen(int levelNumber)
    {
        _loadingCanvas.SetActive(true);

        if (levelNumber >= 0 && levelNumber < _loadingScreenSprites.Length && _loadingScreenSprites[levelNumber] != null)
        {
            _loadingImage.sprite = _loadingScreenSprites[levelNumber];
        }
    }
}
