using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCMovementController : MonoBehaviour
{
    [Header("Wander Settings")]
    public BoxCollider wanderArea;
    public float minDistanceToOtherNPCs = 1f;

    [Header("Agent Settings")]
    public NavMeshAgent agent;

    private static List<NPCMovementController> allNPCs = new List<NPCMovementController>();

    private Rigidbody rb;
    private Collider col;
    private bool hasStartedWalking = false; // NPC hasn't started yet
    private bool hasReachedDestination = false; // Stop after first move

    private void Awake()
    {
        allNPCs.Add(this);

        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        if (rb != null)
            rb.isKinematic = true;

        if (agent != null)
        {
            agent.radius = minDistanceToOtherNPCs / 2f;
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            agent.avoidancePriority = Random.Range(30, 70); 
            agent.updateRotation = true;
            agent.updatePosition = true;

            agent.isStopped = true; // Wait until StartNPC() is called
        }
    }

    private void Update()
    {
        if (!hasStartedWalking || hasReachedDestination || agent == null) return;

        // Check if NPC reached the destination
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // Stop movement completely
            agent.isStopped = true;
            hasReachedDestination = true;
        }
    }

    
    public void StartNPC()
    {
        if (hasStartedWalking) return;

        hasStartedWalking = true;

        // Snap NPC to NavMesh with correct Y offset
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 1f, NavMesh.AllAreas))
        {
            if (col != null)
            {
                float halfHeight = col.bounds.extents.y;
                transform.position = new Vector3(hit.position.x, hit.position.y + halfHeight, hit.position.z);
            }
            else
            {
                transform.position = hit.position;
            }
        }

        // Ensure baseOffset is set so capsule stays above ground
        if (col != null && agent != null)
            agent.baseOffset = col.bounds.extents.y;

        // Set a single destination
        MoveToRandomPoint();
        agent.isStopped = false;
    }

    private void MoveToRandomPoint()
    {
        if (agent == null || wanderArea == null) return;

        Vector3 point = GetRandomPointWithDistance();

        if (point != Vector3.zero)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(point, out hit, 5f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position); // Let agent handle Y
            }
        }
    }

    private Vector3 GetRandomPointWithDistance(int maxAttempts = 50)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 point = GetRandomPointInBoxCollider(wanderArea);
            bool tooClose = false;

            foreach (var npc in allNPCs)
            {
                if (npc == this) continue;
                if (Vector3.Distance(point, npc.transform.position) < minDistanceToOtherNPCs)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
                return point;
        }

        return Vector3.zero;
    }

    private Vector3 GetRandomPointInBoxCollider(BoxCollider box)
    {
        Vector3 center = box.transform.TransformPoint(box.center);
        Vector3 size = box.size;

        Vector3 randomPos = new Vector3(
            Random.Range(-size.x / 2, size.x / 2),
            0f,
            Random.Range(-size.z / 2, size.z / 2)
        );

        return center + randomPos;
    }

    private void OnDestroy()
    {
        allNPCs.Remove(this);
    }
}
