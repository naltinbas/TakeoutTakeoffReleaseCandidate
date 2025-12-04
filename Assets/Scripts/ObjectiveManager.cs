using TMPro;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    private static TextMeshProUGUI ObjectiveText => GameObject.Find("ObjectiveTextGameObject").GetComponent<TextMeshProUGUI>();
    private static TextMeshProUGUI MealCounterText => GameObject.Find("MealCounterTextGameObject").GetComponent<TextMeshProUGUI>();

    public static void SetObjectiveText(string text)
    {
        ObjectiveText.SetText(text);
    }

    public static void SetMealCounterText(int collected, int outstanding)
    {
        var mealText = $"Remaining: {outstanding}\\nCollected: {collected}";
        MealCounterText.SetText(mealText);
    }

    public static void SetObjectiveColor(bool active)
    {
        // ObjectiveText.color = active ? Color.green : Color.red;
    }
}