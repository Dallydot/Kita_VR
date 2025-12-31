using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public Quest[] quests;
    private GameClockController clock;

    [Header("UI")]
    public GameObject questText;

    private void Awake()
    {
        Instance = this;
        if (questText != null)
            questText.SetActive(false);

        clock = FindObjectOfType<GameClockController>();

        foreach (Quest q in quests)
        {
            q.ResetQuest();
        }
    }

    private void Update()
    {
        float timeOfDay = clock.GetTimeOfDay();

        foreach (Quest q in quests)
        {
            if (q.autoStart && q.state == QuestState.Inactive && timeOfDay >= q.startHour)
            {
                StartQuest(q);
            }
        }
    }

    public void StartQuest(Quest quest)
    {
        if (quest.state != QuestState.Inactive) return;

        quest.StartQuest();

        if (questText != null)
            questText.SetActive(true);

        if (clock != null)
            clock.FreezeTime(true);
    }

    public void QuestCompleted(Quest quest)
    {
        if (questText != null)
            questText.SetActive(false);

        quest.CompleteQuest();

        if (clock != null)
            clock.FreezeTime(false);
    }

    public Quest GetActiveQuest()
    {
        foreach (Quest q in quests)
            if (q.state == QuestState.Active)
                return q;
        return null;
    }
}
