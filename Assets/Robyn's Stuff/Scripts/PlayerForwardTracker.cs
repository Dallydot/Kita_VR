using UnityEngine;

public class PlayerForwardTracker : MonoBehaviour
{
    private Vector3 lastPosition;

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        Quest activeQuest = QuestManager.Instance.GetActiveQuest();
        if (activeQuest == null || !activeQuest.requiresWalking)
        {
            lastPosition = transform.position;
            return;
        }

        Vector3 delta = transform.position - lastPosition;
        float forwardMovement = Vector3.Dot(delta, transform.forward);

        if (forwardMovement > 0f)
        {
            activeQuest.AddDistance(forwardMovement);
        }

        lastPosition = transform.position;
    }
}

