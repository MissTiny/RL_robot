using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour
{
    [SerializeField] private AgentController agentController;
	
	private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Agent")
        {
			agentController.ObstacleIsTriggered();
        }
    }
}
