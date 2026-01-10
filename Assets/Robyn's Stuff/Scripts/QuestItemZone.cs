using UnityEngine;

[RequireComponent(typeof(Collider))]
public class QuestItemZone : MonoBehaviour
{
    [Tooltip("Welche Quest dieses Objekt sichtbar macht")]
    public Quest quest;              // Quest, die dieses Objekt steuert
    private MeshRenderer mesh;

    private void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (quest == null)
        {
            Debug.LogError("QUEST IST NULL!");
            return;
        }

        bool shouldBeVisible = quest.state == QuestState.Active;

        if (mesh.enabled != shouldBeVisible)
        {
            mesh.enabled = shouldBeVisible;
            Debug.Log("BOX SICHTBAR = " + shouldBeVisible);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Quest activeQuest = QuestManager.Instance.GetActiveQuest();
        if (activeQuest == null) return;

        if (!activeQuest.requiresItemPlacement) return;

        if (other.CompareTag(activeQuest.requiredItemTag))
        {
            Debug.Log("Quest-Item korrekt platziert!");

            QuestManager.Instance.QuestCompleted(activeQuest);

            Debug.Log("Trigger Enter: " + other.name);
            Debug.Log("Aktive Quest: " + activeQuest.questName);
            Debug.Log("Item Tag: " + other.tag);
        }
    }
}
