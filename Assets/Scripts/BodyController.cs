using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyController : MonoBehaviour
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
            agentController.SetBodyPartOnFloor(gameObject.name, true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Floor")
        {
            agentController.SetBodyPartOnFloor(gameObject.name, false);
        }
    }
}
