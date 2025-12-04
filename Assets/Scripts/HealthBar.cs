using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HealthBar : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text percentageText;

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;
    [SerializeField] private float lerpSpeed = 5f;

    [Header("Colors")]
    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color midHealthColor = Color.yellow;
    [SerializeField] private Color lowHealthColor = Color.red;

    [Header("Bump Animation")]
    [SerializeField] private float bumpScale = 1.15f;
    [SerializeField] private float bumpDuration = 0.15f;

    private RectTransform rectTransform;
    private Vector3 originalScale;
    private Coroutine bumpRoutine;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
    }

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        float fillAmount = Mathf.Lerp(fillImage.fillAmount, currentHealth / maxHealth, Time.deltaTime * lerpSpeed);
        fillImage.fillAmount = fillAmount;

        // Interpolate color between red -> yellow -> green
        Color currentColor;
        if (fillAmount > 0.5f)
            currentColor = Color.Lerp(midHealthColor, fullHealthColor, (fillAmount - 0.5f) * 2f);
        else
            currentColor = Color.Lerp(lowHealthColor, midHealthColor, fillAmount * 2f);

        fillImage.color = currentColor;

        // Update percentage text
        float percent = fillAmount * 100f;
        percentageText.text = $"{percent:0}%";
    }

    public void SetHealth(float newHealth)
    {
        newHealth = Mathf.Clamp(newHealth, 0, maxHealth);
        if (Mathf.Abs(newHealth - currentHealth) > 0.01f)
        {
            currentHealth = newHealth;

            // Start bump animation
            if (bumpRoutine != null)
                StopCoroutine(bumpRoutine);
            bumpRoutine = StartCoroutine(BumpEffect());
        }
    }

    private IEnumerator BumpEffect()
    {
        float elapsed = 0f;
        while (elapsed < bumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / bumpDuration;
            float scale = Mathf.Lerp(1f, bumpScale, Mathf.Sin(t * Mathf.PI)); // bump in & out
            rectTransform.localScale = originalScale * scale;
            yield return null;
        }
        rectTransform.localScale = originalScale;
    }
}
