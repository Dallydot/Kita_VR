using System.Collections;
using UnityEngine;

public class GameClockController : MonoBehaviour
{
    public static GameClockController Instance;

    [Header("Time Settings")]
    private const float REAL_SECONDS_PER_INGAME_DAY = 100f;

    private float day;
    private bool timeFrozen = false;

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
    // Uhr nur laufen lassen, wenn keine Quest aktiv ist, die die Zeit stoppen soll
    bool anyQuestActive = false;
    if (QuestManager.Instance != null)
    {
        foreach (var q in QuestManager.Instance.quests)
        {
            if (q.state == QuestState.Active)
            {
                anyQuestActive = true;
                break;
            }
        }
    }

    timeFrozen = anyQuestActive;

    if (!timeFrozen)
    {
        day += Time.deltaTime / REAL_SECONDS_PER_INGAME_DAY;
    }

    UpdateClockHands(GetTimeOfDay());
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

    public float GetTimeOfDay()
    {
        return (day % 1f) * 24f;
    }

    public void FreezeTime(bool freeze)
    {
        timeFrozen = freeze;
        Debug.Log("Uhr " + (freeze ? "gestoppt" : "läuft wieder"));
    }
}
