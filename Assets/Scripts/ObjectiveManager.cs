using TMPro;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    // Lazy-cached references (avoids GameObject.Find on every access)
    private static TextMeshProUGUI _objectiveText;
    private static TextMeshProUGUI _mealCounterText;

    private static TextMeshProUGUI ObjectiveText
    {
        get
        {
            if (_objectiveText == null)
            {
                var go = GameObject.Find("ObjectiveTextGameObject");
                if (go != null)
                    _objectiveText = go.GetComponent<TextMeshProUGUI>();
            }
            return _objectiveText;
        }
    }

    private static TextMeshProUGUI MealCounterText
    {
        get
        {
            if (_mealCounterText == null)
            {
                var go = GameObject.Find("MealCounterTextGameObject");
                if (go != null)
                    _mealCounterText = go.GetComponent<TextMeshProUGUI>();
            }
            return _mealCounterText;
        }
    }

    public static void SetObjectiveText(string text)
    {
        if (ObjectiveText != null)
            ObjectiveText.SetText(text);
    }

    public static void SetMealCounterText(int collected, int outstanding)
    {
        if (MealCounterText != null)
        {
            var mealText = $"Remaining: {outstanding}\\nCollected: {collected}";
            MealCounterText.SetText(mealText);
        }
    }

    public static void SetObjectiveColor(bool active)
    {
        // ObjectiveText.color = active ? Color.green : Color.red;
    }
}
