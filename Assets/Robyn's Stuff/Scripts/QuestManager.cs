using UnityEngine;
using UnityEngine.InputSystem;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public Quest[] quests;
    private GameClockController clock;

    [Header("UI")]
    public GameObject questText;

    [Header("Input")]
    public InputActionReference selectActionReference;

    private void Awake()
    {
    Instance = this;

    if (questText != null)
        questText.SetActive(false);

    clock = FindObjectOfType<GameClockController>();

    // INPUT ACTION AKTIVIEREN
    if (selectActionReference != null)
        selectActionReference.action.Enable();
    }


    private void Update()
    {
        float timeOfDay = clock.GetTimeOfDay();

        InputAction action = selectActionReference.action;

        if (action == null) return;

        bool buttonPressedThisFrame = action.WasPressedThisFrame();

        foreach (Quest q in quests)
        {
            if (q.autoStart && q.state == QuestState.Inactive && timeOfDay >= q.startHour)
            {
                Debug.Log($"Autostart: {q.questName} wird jetzt gestartet!");

                q.StartQuest();

                // UI EINBLENDEN
                if (questText != null)
                {
                    questText.SetActive(true);
                    Debug.Log("questText.SetActive(true) wurde ausgeführt!");
                }
                else
                {
                    Debug.LogError("questText ist NULL! UI konnte nicht aktiviert werden!");
                }

                
                // Zeit anhalten
                clock.FreezeTime(true);
            }
        }

        // BUTTON Quest prüfen
        Quest activeQuest = GetActiveQuest();
        if (activeQuest != null && activeQuest.requiresButtonPresses)
        {
            if (buttonPressedThisFrame)
            {
                activeQuest.RegisterButtonPress();
            }
        }


    }

    public Quest GetActiveQuest()
    {
        foreach (Quest q in quests)
        {
            if (q.state == QuestState.Active)
                return q;
        }
        return null;
    }

    public void QuestCompleted(Quest quest)
    {
        if (questText != null)
            questText.SetActive(false);

        quest.CompleteQuest();
        clock.FreezeTime(false);
    }
}
