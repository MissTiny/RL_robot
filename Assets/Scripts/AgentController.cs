using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentController : Agent
{
	[SerializeField] private Rigidbody head;
	[SerializeField] private Rigidbody rightSmallArmRigidbody;
	[SerializeField] private Rigidbody leftSmallArmRigidbody;
	[SerializeField] private Rigidbody middleBodyRigidbody;
	[SerializeField] private Rigidbody lowerBodyRigidbody;
	[SerializeField] private Rigidbody rightThighRigidbody;
	[SerializeField] private Rigidbody leftThighRigidbody;
	[SerializeField] private Rigidbody rightCalfRigidbody;
	[SerializeField] private Rigidbody leftCalfRigidbody;
	
	public float rotationSpeed = 1000f;
	
	public override void OnEpisodeBegin()
	{
		transform.localPosition = new Vector3(0f, 2.5f, 0f);
	}
	
	public override void CollectObservations(VectorSensor sensor)
	{
		sensor.AddObservation(transform.localPosition);
	}
	
    public override void OnActionReceived(ActionBuffers actions)
    {
		// Actions for the head
		float headTorqueY = actions.ContinuousActions[0];
		float headTorqueZ = actions.ContinuousActions[1];
		
        // Actions for the right small arm
		float rightTorqueX = actions.ContinuousActions[2];
		float rightTorqueY = -Mathf.Abs(actions.ContinuousActions[3]);

		// Actions for the left small arm
		float leftTorqueX = actions.ContinuousActions[4];
		float leftTorqueY = Mathf.Abs(actions.ContinuousActions[5]);
		
		// Action for the middle body's rotation
		float middleBodyTorqueY = actions.ContinuousActions[6];
		
		// Actions for the lower body
		float lowerBodyTorqueY = actions.ContinuousActions[7];
		float lowerBodyTorqueZ = actions.ContinuousActions[8];
		
		// Action for the right thigh
		float rightThighTorqueY = actions.ContinuousActions[9];
		
		// Action for the left thigh
		float leftThighTorqueY = actions.ContinuousActions[10];
		
		// Action for the right calf
		float rightCalfTorqueY = actions.ContinuousActions[11];
		
		// Action for the left calf
		float leftCalfTorqueY = actions.ContinuousActions[12];
		
		// Apply the head torque
		head.AddRelativeTorque(new Vector3(0, headTorqueY, headTorqueZ) * rotationSpeed);

		// Apply the right and left small arms' torque
		rightSmallArmRigidbody.AddRelativeTorque(new Vector3(rightTorqueX, rightTorqueY, 0) * rotationSpeed);
		leftSmallArmRigidbody.AddRelativeTorque(new Vector3(leftTorqueX, leftTorqueY, 0) * rotationSpeed);
		
		// Apply the middle body's torque
		middleBodyRigidbody.AddRelativeTorque(Vector3.forward * middleBodyTorqueY * rotationSpeed);
		
		// Apply the torque to the lower body's Rigidbody
		lowerBodyRigidbody.AddRelativeTorque(new Vector3(0, lowerBodyTorqueY, lowerBodyTorqueZ) * rotationSpeed);
		
		// Apply the torque to the right thigh's Rigidbody
		rightThighRigidbody.AddRelativeTorque(Vector3.forward * rightThighTorqueY * rotationSpeed);
		
		// Apply the torque to the left thigh's Rigidbody
		leftThighRigidbody.AddRelativeTorque(Vector3.forward * leftThighTorqueY * rotationSpeed);
		
		// Apply the torque to the right calf's Rigidbody
		rightCalfRigidbody.AddRelativeTorque(Vector3.forward * rightCalfTorqueY * rotationSpeed);
		
		// Apply the torque to the left calf's Rigidbody
		leftCalfRigidbody.AddRelativeTorque(Vector3.forward * leftCalfTorqueY * rotationSpeed);
    }
	
	private float ClampAngle(float angle, float min, float max)
	{
		angle = NormalizeAngle(angle);
		return Mathf.Clamp(angle, min, max);
	}
	
	private float NormalizeAngle(float angle)
	{
		while (angle > 180f) angle -= 360f;
		while (angle < -180f) angle += 360f;
		return angle;
	}
	
	private void OntriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Obstacle")
		{
			AddReward(1f);
			EndEpisode();
		}
	}
	
	public override void Heuristic(in ActionBuffers actionsOut)
	{
		ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal"); // left and right
		continuousActions[1] = Input.GetAxisRaw("Vertical"); // up and down 
	}
}
