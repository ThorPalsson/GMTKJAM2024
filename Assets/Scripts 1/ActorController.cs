
using UnityEngine;
using UnityEngine.AI;

public class ActorController : MonoBehaviour
{
    public Character Character; 
    private bool inDialogue;
    private NavMeshAgent agent;

    private Vector3 queuedMovement;
    private bool hasQueue;
    
    public void RequestMovement(Vector3 pos)
    {
        queuedMovement = pos;
        hasQueue = true;
    }

    public void LeaveDialouge()
    {
        if (hasQueue)
        {
            agent.SetDestination(queuedMovement);
            hasQueue = false; 
        }
    }

}
