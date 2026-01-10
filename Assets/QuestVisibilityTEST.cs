using UnityEngine;

public class QuestVisibilityTEST : MonoBehaviour
{
    public Quest quest;
    private MeshRenderer mesh;

    private void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
        if (mesh == null)
            Debug.LogError("KEIN MESHRENDERER!");
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
}
