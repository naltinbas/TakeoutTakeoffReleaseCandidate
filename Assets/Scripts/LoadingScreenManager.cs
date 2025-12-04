using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour
{
    private Sprite _loadingScreenLevelOne;
    private Sprite _loadingScreenLevelTwo;
    private Sprite _loadingScreenLevelThree;

    private GameObject _loadingCanvas;
    private Image _loadingImage;

    private void Awake()
    {
        _loadingScreenLevelOne = Resources.Load<Sprite>("loadingScreenLevelOne");
        _loadingScreenLevelTwo = Resources.Load<Sprite>("loadingScreenLevelTwo");
        _loadingScreenLevelThree = Resources.Load<Sprite>("loadingScreenLevelThree");

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

        switch (levelNumber)
        {
            case 1:
                _loadingImage.sprite = _loadingScreenLevelOne;
                break;
            case 2:
                _loadingImage.sprite = _loadingScreenLevelTwo;
                break;
            case 3:
                _loadingImage.sprite = _loadingScreenLevelThree;
                break;
        }
    }
}