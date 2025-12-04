using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class PlaneHearts : MonoBehaviour
{
    [Header("Heart Settings")]
    [SerializeField] private int maxHearts = 6;
    [SerializeField] private int currentHearts;

    [Header("Heart UI Setup")]
    [SerializeField] private Transform heartsParent;   // Empty GameObject under Canvas with Horizontal Layout Group
    [SerializeField] private Image heartPrefab;        // One heart prefab (assign in Inspector)
    [SerializeField] private Sprite heartSprite;       // Full heart sprite
    [SerializeField] private Sprite emptyHeartSprite;  // Optional faded heart sprite

    private List<Image> heartImages = new List<Image>();

    [Header("Shield Settings")]
    [SerializeField] private float shieldDuration = 3f;  // How long shield lasts
    [SerializeField] private GameObject shieldVFX;       // Optional visual effect
    [SerializeField] private Image shieldIconPrefab;     // Prefab for one shield icon (UI Image)
    [SerializeField] private RectTransform shieldContainer; // Parent for shield icons
    [SerializeField] private float iconOffset = 25f;     // Overlap offset between icons

    private List<Image> activeShieldIcons = new List<Image>();

    private int _shieldCount = 0;
    private bool _isShieldActive;

    private void Start()
    {
        currentHearts = maxHearts;
        GenerateHearts();
        UpdateHeartsUI();

        if (shieldVFX != null)
            shieldVFX.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && _shieldCount > 0 && !_isShieldActive)
        {
            UseShield();
        }
    }

    // ---------------- HEARTS ---------------- //
    private void GenerateHearts()
    {
        foreach (Transform child in heartsParent)
            Destroy(child.gameObject);
        heartImages.Clear();

        for (int i = 0; i < maxHearts; i++)
        {
            Image newHeart = Instantiate(heartPrefab, heartsParent);
            newHeart.sprite = heartSprite;
            heartImages.Add(newHeart);
        }
    }

    public void TakeDamage(int amount)
    {
        if (_isShieldActive) return;

        currentHearts -= amount;
        currentHearts = Mathf.Clamp(currentHearts, 0, maxHearts);
        UpdateHeartsUI();

        if (currentHearts <= 0)
        {
            Debug.Log("Plane destroyed!");
            GetComponent<ExplosionHandler>()?.Explode();
        }
    }

    public void Heal(int amount)
    {
        currentHearts = Mathf.Clamp(currentHearts + amount, 0, maxHearts);
        UpdateHeartsUI();
    }

    private void UpdateHeartsUI()
    {
        for (int i = 0; i < heartImages.Count; i++)
        {
            if (i < currentHearts)
            {
                heartImages[i].sprite = heartSprite;
                heartImages[i].enabled = true;
            }
            else
            {
                if (emptyHeartSprite != null)
                    heartImages[i].sprite = emptyHeartSprite;
                else
                    heartImages[i].enabled = false;
            }
        }
    }

    public void ResetHearts()
    {
        currentHearts = maxHearts;
        _shieldCount = 0;
        _isShieldActive = false;

        UpdateHeartsUI();

        foreach (var icon in activeShieldIcons)
            Destroy(icon.gameObject);
        activeShieldIcons.Clear();

        if (shieldVFX != null)
            shieldVFX.SetActive(false);
    }

    // ---------------- SHIELD ---------------- //
    public void PickupShield()
    {
        AudioSourceManager.PlaySound("GetShield");

        // Add visual icon
        if (shieldIconPrefab != null && shieldContainer != null)
        {
            Image newIcon = Instantiate(shieldIconPrefab, shieldContainer);
            newIcon.gameObject.SetActive(true);

            float offsetX = -iconOffset * activeShieldIcons.Count;
            newIcon.rectTransform.anchoredPosition = new Vector2(offsetX, 0);

            activeShieldIcons.Add(newIcon);
        }

        _shieldCount++;
        Debug.Log($"Shield collected! Total shields: {_shieldCount}");
    }

    private void UseShield()
    {
        if (_shieldCount <= 0) return;

        _shieldCount--;
        AudioSourceManager.PlaySound("UseShield");

        // Remove one shield icon visually
        Image lastIcon = activeShieldIcons[activeShieldIcons.Count - 1];
        activeShieldIcons.RemoveAt(activeShieldIcons.Count - 1);
        Destroy(lastIcon.gameObject);

        StartCoroutine(ActivateShield(shieldDuration));
    }

    private IEnumerator ActivateShield(float duration)
    {
        _isShieldActive = true;
        if (shieldVFX != null)
            shieldVFX.SetActive(true);

        Debug.Log("Shield Activated!");
        yield return new WaitForSeconds(duration);

        _isShieldActive = false;
        if (shieldVFX != null)
            shieldVFX.SetActive(false);

        Debug.Log("Shield Ended!");
    }
}
