using System.Collections;
using UnityEngine;

public class GameClockController : MonoBehaviour
{
    public static GameClockController Instance;

    [Header("Time Settings")]
    private const float REAL_SECONDS_PER_INGAME_DAY = 100f; // später 500f oder länger

    private const float NPC_START_HOUR = 2.5f;     // NPCs starten zu dieser Stunde
    private const float FREEZE_AT_HOUR = 7f;       // Uhr wird bei 7 Uhr gestoppt
    private const float SHOW_TIME_SEVEN = 3f;      // optionale kurze Anzeige

    private float day;
    private bool timeFrozen = false;
    private bool eventStarted = false;
    private bool timeShown = false;

    [Header("Clock Hands")]
    public Transform clockHourHandTransform;
    public Transform clockMinuteHandTransform;

    [Header("UI")]
    public GameObject siebenUhr;

    private void Awake()
    {
        Instance = this;
        if (siebenUhr != null)
            siebenUhr.SetActive(false);

        if (clockHourHandTransform == null)
            clockHourHandTransform = transform.Find("hourHand");
        if (clockMinuteHandTransform == null)
            clockMinuteHandTransform = transform.Find("minuteHand");
    }

    private void Update()
    {
        float timeOfDay = GetTimeOfDay();
        // Uhr läuft nur, solange sie nicht eingefroren wurde
        if (!timeFrozen)
        {
            day += Time.deltaTime / REAL_SECONDS_PER_INGAME_DAY;
        }

        UpdateClockHands(timeOfDay);

        // NPCs starten
        if (!eventStarted && timeOfDay >= NPC_START_HOUR)
        {
            eventStarted = true;
            StartCoroutine(StartNPCsRoutine());
        }

        // Kurze Anzeige bei SHOW_TIME_SEVEN (optional)
        if (!timeShown && timeOfDay >= SHOW_TIME_SEVEN)
        {
            timeShown = true;
            StartCoroutine(ShowTimeRoutine());
        }

        // Uhr stoppen bei FREEZE_AT_HOUR
        if (!timeFrozen && timeOfDay >= SHOW_TIME_SEVEN)
        {
            timeFrozen = true;
            if (siebenUhr != null)
                siebenUhr.SetActive(true);

            // 7-Uhr-Quest starten ohne LINQ
            if (QuestManager.Instance != null)
            {
                foreach (var q in QuestManager.Instance.quests)
                {
                    float tolerance = 0.01f;
                    if (q.autoStart && q.state == QuestState.Inactive && Mathf.Abs(q.startHour - SHOW_TIME_SEVEN) < tolerance)
                    {
                        q.StartQuest();

                        

                        Debug.Log("7-Uhr-Quest gestartet, Zeit pausiert");
                    }
                }
            }
        }
    }

    private void UpdateClockHands(float timeOfDay)
    {
        float hours = timeOfDay;
        float minutes = (timeOfDay - Mathf.Floor(timeOfDay)) * 60f;

        if (clockHourHandTransform != null)
            clockHourHandTransform.eulerAngles = new Vector3(0, 0, -120 + (-hours / 12f * 360f));

        if (clockMinuteHandTransform != null)
            clockMinuteHandTransform.eulerAngles = new Vector3(0, 0, -(minutes / 60f * 360f));
    }

    private IEnumerator StartNPCsRoutine()
    {
        foreach (var npc in FindObjectsOfType<NPCMovementController>())
        {
            npc.StartNPC();
        }
        yield break;
    }

    private IEnumerator ShowTimeRoutine()
    {
        if (siebenUhr != null)
            siebenUhr.SetActive(true);
        yield return new WaitForSeconds(2f);
        if (siebenUhr != null)
            siebenUhr.SetActive(false);
    }

    public float GetTimeOfDay()
    {
        float dayNormalized = day % 1f;
        return dayNormalized * 24f;
    }

    public void FreezeTime(bool freeze)
    {
        timeFrozen = freeze;
        Debug.Log("Uhr " + (freeze ? "gestoppt" : "läuft wieder"));
    }
}
