using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSystem : MonoBehaviour
{
    public string questName;
    public string description;

    public float startHour = 3f;
    public QuestState state = QuestState.Inactive;

    public bool autoStart => startHour >= 0f;

    public void StartQuest()
    {
        if (state != QuestState.Inactive)
        return;

        state = QuestState.Active;
        Debug.Log("Quest gestartet: " + questName);
    }

    public void CompleteQuest()
    {
        if (state != QuestState.Active)
        return;

        state = QuestState.Completed;
        Debug.Log("Quest abgeschlossen: " + questName);
    }
}
