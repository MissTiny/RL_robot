using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using TMPro;

public class AgentController : Agent
{
	[SerializeField] private Rigidbody[] bodyParts;
	
	// [SerializeField] private Rigidbody head;
	// [SerializeField] private Rigidbody rightSmallArmRigidbody;
	// [SerializeField] private Rigidbody leftSmallArmRigidbody;
	// [SerializeField] private Rigidbody middleBodyRigidbody;
	// [SerializeField] private Rigidbody lowerBodyRigidbody;
	// [SerializeField] private Rigidbody rightThighRigidbody;
	// [SerializeField] private Rigidbody leftThighRigidbody;
	// [SerializeField] private Rigidbody rightCalfRigidbody;
	// [SerializeField] private Rigidbody leftCalfRigidbody;
	
	[SerializeField] private TextMeshProUGUI timerText;
	
	[SerializeField] private Transform goal;
	
	private Vector3[] initialPositions;
    private Quaternion[] initialRotations;
	
	public float rotationSpeed = 800000000f;
	
	private float episodeTimer = 65f;
	private float maxDiffDistance;
	
	void Start()
	{
		Time.timeScale = 1f;
	}
	
	public override void Initialize()
    {
        // Initialize the arrays to the number of body parts
        initialPositions = new Vector3[bodyParts.Length];
        initialRotations = new Quaternion[bodyParts.Length];
		
		maxDiffDistance = Vector3.Distance(goal.position, transform.position);

        // Store the initial positions and rotations
        for (int i = 0; i < bodyParts.Length; i++)
        {
            if (bodyParts[i] != null)
            {
                initialPositions[i] = bodyParts[i].position;
                initialRotations[i] = bodyParts[i].rotation;
            }
        }
    }
	
	public override void OnEpisodeBegin()
	{
		UpdateTimerDisplay();
		episodeTimer = 65f;
		
		StartCoroutine(ResetBodyParts());
	}
	
	private IEnumerator ResetBodyParts()
	{
		// Disable physics interactions while resetting
		foreach (var bodyPart in bodyParts)
		{
			if (bodyPart != null)
			{
				bodyPart.detectCollisions = false;
				bodyPart.isKinematic = true;
			}
		}

		float resetDuration = 5f; // Half a second to reset
		float timer = 0;

		while (timer < resetDuration)
		{
			foreach (var bodyPart in bodyParts)
			{
				if (bodyPart != null)
				{
					int index = Array.IndexOf(bodyParts, bodyPart);
					bodyPart.MovePosition(Vector3.Lerp(bodyPart.position, initialPositions[index], timer / resetDuration));
					bodyPart.MoveRotation(Quaternion.Slerp(bodyPart.rotation, initialRotations[index], timer / resetDuration));
				}
			}

			timer += Time.deltaTime;
			yield return null;
		}

		// Ensure all body parts are exactly at their initial positions and rotations
		for (int i = 0; i < bodyParts.Length; i++)
		{
			if (bodyParts[i] != null)
			{
				bodyParts[i].MovePosition(initialPositions[i]);
				bodyParts[i].MoveRotation(initialRotations[i]);
			}
		}

		// Re-enable physics interactions
		foreach (var bodyPart in bodyParts)
		{
			if (bodyPart != null)
			{
				bodyPart.isKinematic = false;
				bodyPart.detectCollisions = true;
			}
		}
	}
	
	public override void CollectObservations(VectorSensor sensor)
	{
		
		// foreach (Rigidbody bodyPart in bodyParts)
		// {
			// Vector3 localPosition = bodyPart.position;
			// sensor.AddObservation(localPosition);
		// }
		
		// Observation of the agent's local position
		Vector3 centroid = CalculateCentroid();
		// sensor.AddObservation(centroid);
		
		// sensor.AddObservation(prevDistanceToGoal);
		
		// Observation for the goal's relative position with respect to agent
		Vector3 relativePosition = goal.position - centroid;
		sensor.AddObservation(relativePosition);
		
		sensor.AddObservation(goal.position);
		
		// Observation of the countdown
		// sensor.AddObservation(episodeTimer);
		
		for (int i = 0; i < bodyParts.Length; i++)
		{
			if (i == 0 || i == 2 || i == 3)
			{
				Rigidbody bodyPart = bodyParts[i];
				// Add local position relative to the centroid (or global position if more appropriate)
				Vector3 localPosition = bodyPart.transform.localPosition;
				sensor.AddObservation(localPosition);

				// Add local rotation (consider converting quaternion to Euler angles if necessary)
				Vector3 localRotation = bodyPart.transform.localRotation.eulerAngles;
				sensor.AddObservation(localRotation);
			}
		}
	}
	
    public override void OnActionReceived(ActionBuffers actions)
    {
		episodeTimer -= Time.deltaTime;
        UpdateTimerDisplay();
        if (episodeTimer <= 0f)
        {
			float distanceToGoal = Vector3.Distance(CalculateCentroid(), goal.position);
			float distReward = -Mathf.Exp(0.1f * distanceToGoal);
			// float distReward = (maxDiffDistance-distanceToGoal) / maxDiffDistance;
			// float distReward = 5f - (distanceToGoal / maxDiffDistance) * 5;
			
			AddReward(distReward);
			
            EndEpisode();
        }
		
		for (int i = 0; i < bodyParts.Length; i++)
		{
			if (bodyParts[i] != null)
			{
				Vector3 torque = Vector3.zero;
				switch (i)
				{
					case 0: // Head
						torque = new Vector3(0, actions.ContinuousActions[0], actions.ContinuousActions[1]);
						break;
					// case 1: // Right Small Arm
						// torque = new Vector3(actions.ContinuousActions[2], -Mathf.Abs(actions.ContinuousActions[3]), 0);
						// break;
					// case 2: // Left Small Arm
						// torque = new Vector3(actions.ContinuousActions[4], Mathf.Abs(actions.ContinuousActions[5]), 0);
						// break;
					case 1: // Upper Body
						torque = Vector3.forward * actions.ContinuousActions[2];
						break;
					case 2: // Middle Body
						torque = Vector3.forward * actions.ContinuousActions[3];
						break;
					case 3: // Lower Body
						torque = new Vector3(0, actions.ContinuousActions[4], actions.ContinuousActions[5]);
						break;
					case 4: // Right Thigh
						torque = Vector3.forward * actions.ContinuousActions[6];
						break;
					case 5: // Left Thigh
						torque = Vector3.forward * actions.ContinuousActions[7];
						break;
					case 6: // Right Calf
						torque = Vector3.forward * actions.ContinuousActions[8];
						break;
					case 7: // Left Calf
						torque = Vector3.forward * actions.ContinuousActions[9];
						break;
				}

				bodyParts[i].AddRelativeTorque(torque * rotationSpeed);
			}
		}
    }
	
	private Vector3 CalculateCentroid()
	{
		Vector3 centroid = Vector3.zero;
		int count = 0;
		foreach (var bodyPart in bodyParts)
		{
			if (bodyPart != null)
			{
				centroid += bodyPart.position;
				count++;
			}
		}
		if (count > 0)
			centroid /= count;

		return centroid;
	}
	
	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Obstacle")
		{
			AddReward(-1f);
		}
		
		if(other.gameObject.tag == "Goal")
		{
			AddReward(10f);
			AddReward(Mathf.CeilToInt(episodeTimer));
			EndEpisode();
		}
	}
	
	public override void Heuristic(in ActionBuffers actionsOut)
	{
		ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[1] = Input.GetAxisRaw("Horizontal"); // left and right
		continuousActions[2] = Input.GetAxisRaw("Vertical"); // up and down 
	}
	
	private void UpdateTimerDisplay()
    {
        if (timerText != null)
		{
			timerText.text = $"{Mathf.CeilToInt(episodeTimer)}";
		}
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
	
	void Update()
	{
		if (Input.anyKey)
		{
			RequestDecision();
		}
	}
}
