using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentController : Agent
{
	[SerializeField] private Transform head;
	[SerializeField] private Rigidbody rightSmallArmRigidbody;
	[SerializeField] private Rigidbody leftSmallArmRigidbody;
	[SerializeField] private Rigidbody middleBodyRigidbody;
	[SerializeField] private Rigidbody lowerBodyRigidbody;
	[SerializeField] private Rigidbody rightThighRigidbody;
	[SerializeField] private Rigidbody leftThighRigidbody;
	[SerializeField] private Rigidbody rightCalfRigidbody;
	[SerializeField] private Rigidbody leftCalfRigidbody;
	
	public float rotationSpeed = 100f;
	
	public override void OnEpisodeBegin()
	{
		transform.localPosition = new Vector3(0f, 3f, 0f);
	}
	
	public override void CollectObservations(VectorSensor sensor)
	{
		sensor.AddObservation(transform.localPosition);
	}
	
    public override void OnActionReceived(ActionBuffers actions)
    {
		// Rotation actions for the head
		float headRotationY = actions.ContinuousActions[0];
		float headRotationZ = actions.ContinuousActions[1];
		
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
		
		// Apply the head rotation
		Vector3 currentHeadRotation = head.localEulerAngles;
		Vector3 newHeadRotation = new Vector3(
			currentHeadRotation.x,
			currentHeadRotation.y + headRotationY * rotationSpeed * Time.deltaTime,
			ClampAngle(currentHeadRotation.z + headRotationZ * rotationSpeed * Time.deltaTime, -30f, 30f)
		);
		head.localEulerAngles = newHeadRotation;

		// Apply the right and left small arms' torque
		rightSmallArmRigidbody.AddRelativeTorque(new Vector3(rightTorqueX, rightTorqueY, 0) * rotationSpeed);
		leftSmallArmRigidbody.AddRelativeTorque(new Vector3(leftTorqueX, leftTorqueY, 0) * rotationSpeed);
		
		// Apply the middle body's torque
		middleBodyRigidbody.AddRelativeTorque(Vector3.forward * middleBodyTorqueY * rotationSpeed);
		
		// Apply the torque to the lower body's Rigidbody
		lowerBodyRigidbody.AddRelativeTorque(new Vector3(0, lowerBodyTorqueY, lowerBodyTorqueZ) * rotationSpeed);
		
		// Apply the torque to the right thigh's Rigidbody
		rightThighRigidbody.AddRelativeTorque(Vector3.forward * rightThighTorqueY * 10f);
		
		// Apply the torque to the left thigh's Rigidbody
		leftThighRigidbody.AddRelativeTorque(Vector3.forward * leftThighTorqueY * 10f);
		
		// Apply the torque to the right calf's Rigidbody
		rightCalfRigidbody.AddRelativeTorque(Vector3.forward * rightCalfTorqueY * 2f);
		
		// Apply the torque to the left calf's Rigidbody
		leftCalfRigidbody.AddRelativeTorque(Vector3.forward * leftCalfTorqueY * 2f);
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
		// if(other.gameObject.tag == "Finpad")
		// {
			// AddReward(2f);
			// EndEpisode();
		// }
		// if(other.gameObject.tag == "Obstacle")
		// {
			// AddReward(-1f);
		// }
	}
	
	public override void Heuristic(in ActionBuffers actionsOut)
	{
		ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[11] = Input.GetAxisRaw("Horizontal"); // left and right
		continuousActions[12] = Input.GetAxisRaw("Vertical"); // up and down 
	}
	
	void Update()
	{
		if (Input.anyKey)
		{
			RequestDecision();
		}
	}
}
