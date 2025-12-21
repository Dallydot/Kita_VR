using UnityEngine;
using UnityEngine.InputSystem;

public enum QuestState { Inactive, Active, Completed }

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest System/Quest")]
public class Quest : ScriptableObject
{
    public string questName;
    public string description;

    [Header("Auto Start Settings")]
    public float startHour = 3f; // -1 = kein Timer
    public QuestState state = QuestState.Inactive;
    public bool autoStart => startHour >= 0f;

    public void StartQuest()
    {
        if (state != QuestState.Inactive) return;
        state = QuestState.Active;
        Debug.Log("Quest gestartet: " + questName);
    }

    public void CompleteQuest()
    {
        if (state != QuestState.Active) return;
        state = QuestState.Completed;
        Debug.Log("Quest abgeschlossen: " + questName);
    }

    [Header("Tastendrück-Quest")]
    public bool requiresButtonPresses = false;
    public int requiredButtons = 3;
    public int currentButtonPresses = 0;

    public void RegisterButtonPress()
    {
        if (!requiresButtonPresses || state != QuestState.Active) return;

        currentButtonPresses++;
        Debug.Log($"BUTTON gedrückt! ({currentButtonPresses}/{requiredButtons})");

        if (currentButtonPresses >= requiredButtons)
        {
            CompleteQuest();
            currentButtonPresses = requiredButtons;
            Debug.Log("Quest abgeschlossen");
        }
    }

}
