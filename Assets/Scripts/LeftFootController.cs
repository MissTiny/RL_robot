using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftFootController : MonoBehaviour
{
	private AgentController agentController;
    
    void Awake()
    {
        agentController = GetComponentInParent<AgentController>();

        if(agentController == null)
        {
            Debug.LogError("AgentController component not found in parent GameObjects.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Floor")
        {
            agentController.SetFootOnFloor(true, true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Floor")
        {
            agentController.SetFootOnFloor(true, false);
        }
    }
}
