using System.Collections;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public Quest[] quests;
    private GameClockController clock;

    [Header("UI")]
    public GameObject questStartText;
    public GameObject questEndText;
    public float delayTime = 2f;

    private void Awake()
    {
        Debug.Log("Clock gefunden: " + clock);
        Instance = this;
        if (questStartText != null)
            questStartText.SetActive(false);

        clock = FindObjectOfType<GameClockController>();

        foreach (Quest q in quests)
        {
            q.ResetQuest();
        }
    }

    private void Update()
    {
        
        if (clock == null)
        {
            clock = FindObjectOfType<GameClockController>();
            if (clock == null) return; // nichts tun solange keine clock
        }

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

        if (questStartText != null)
            questStartText.SetActive(true);
        
        if (questEndText != null)
            questEndText.SetActive(false);

        if (clock != null)
            clock.FreezeTime(true);
    }

    public void QuestCompleted(Quest quest)
    {
        if (questStartText != null)
            questStartText.SetActive(false);
        
        if (questEndText != null)
            questEndText.SetActive(true);
            StartCoroutine(DelayAction(delayTime));

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

    IEnumerator DelayAction(float delayTime)
    {
    //Wait for the specified delay time before continuing.
    yield return new WaitForSeconds(delayTime);

    //Do the action after the delay time has finished.
    }
}
