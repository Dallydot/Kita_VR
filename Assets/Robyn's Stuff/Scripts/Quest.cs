using UnityEngine;

public enum QuestState { Inactive, Active, Completed }

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest System/Quest")]
public class Quest : ScriptableObject
{
    public string questName;
    public string description;

    [Header("Auto Start Settings")]
    public float startHour = -1f;
    public QuestState state = QuestState.Inactive;
    public bool autoStart => startHour >= 0f;

    [Header("Lauf-Quest")]
    public bool requiresWalking = false;
    public float requiredDistance = 5f;
    [HideInInspector] public float walkedDistance = 0f;

    [Header("Button-Quest")]
    public bool requiresButtonPresses = false;
    public int requiredButtons = 3;
    [HideInInspector] public int currentButtonPresses = 0;

    [Header("NPC-Quest")]
    public bool requiresNPCClicks = false;
    [HideInInspector] public int totalNPCs = 0;
    [HideInInspector] public int clickedNPCs = 0;

    public void StartQuest()
    {
        if (state != QuestState.Inactive) return;
        state = QuestState.Active;
        walkedDistance = 0f;
        currentButtonPresses = 0;
        clickedNPCs = 0;
        Debug.Log("Quest gestartet: " + questName);
    }

    public void CompleteQuest()
    {
        if (state != QuestState.Active) return;
        state = QuestState.Completed;
        Debug.Log("Quest abgeschlossen: " + questName);
        if (requiresNPCClicks)
        {
            GameClockController.Instance.StartNPCs();
        }
    }

    public void ResetQuest()
    {
        state = QuestState.Inactive;
        walkedDistance = 0f;
        currentButtonPresses = 0;
        clickedNPCs = 0;
    }

    public void AddDistance(float distance)
    {
        if (state != QuestState.Active || !requiresWalking) return;
        walkedDistance += distance;
        Debug.Log($"Gelaufen: {walkedDistance:F2}/{requiredDistance}");
        if (walkedDistance >= requiredDistance)
        {
            QuestManager.Instance.QuestCompleted(this);
        }
    }

    public void RegisterButtonPress()
    {
        if (state != QuestState.Active || !requiresButtonPresses) return;
        currentButtonPresses++;
        Debug.Log($"Button gedrückt: {currentButtonPresses}/{requiredButtons}");
        if (currentButtonPresses >= requiredButtons)
        {
            QuestManager.Instance.QuestCompleted(this);
        }
    }

    public void RegisterNPCClick()
    {
        if (state != QuestState.Active || !requiresNPCClicks) return;
        clickedNPCs++;
        Debug.Log($"NPC angeklickt: {clickedNPCs}/{totalNPCs}");
        if (clickedNPCs >= totalNPCs)
        {
            QuestManager.Instance.QuestCompleted(this);
        }
    }
}
