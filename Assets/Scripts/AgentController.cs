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
	[SerializeField] private Transform[] feet;
	private bool isLeftFootOnFloor = false;
	private bool isRightFootOnFloor = false;
	
	private Dictionary<string, bool> bodyPartTouchingFloor = new Dictionary<string, bool>();
	
	[SerializeField] private TextMeshProUGUI timerText;
	
	[SerializeField] private GoalController goalController;
	[SerializeField] private Transform goal;
	[SerializeField] private Material goalNormalMaterial;
	private Vector3 initialGoalPosition;
	
	private Vector3[] initialPositions;
    private Quaternion[] initialRotations;
	
	
	public float rotationSpeed = 800000000f;
	
	private float episodeTimer = 65f;
	private float maxDiffDistance;
	
	void Start()
	{
		Time.timeScale = 1f;
	}
	
	void Awake()
    {
        bodyPartTouchingFloor.Add("Head", false);
        bodyPartTouchingFloor.Add("Upper Body", false);
        bodyPartTouchingFloor.Add("Middle Body", false);
        bodyPartTouchingFloor.Add("Lower Body", false);
        bodyPartTouchingFloor.Add("Right Thigh", false);
        bodyPartTouchingFloor.Add("Left Thigh", false);
        // bodyPartTouchingFloor.Add("RightCalf", false);
        // bodyPartTouchingFloor.Add("LeftCalf", false);
    }
	
	public override void Initialize()
    {
        // Initialize the arrays to the number of body parts
        initialPositions = new Vector3[bodyParts.Length];
        initialRotations = new Quaternion[bodyParts.Length];
		
		initialGoalPosition = goal.position;
		
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
		
		// Reset the position and material of the goal
		goal.position = initialGoalPosition;
		Renderer goalRenderer = goal.GetComponent<Renderer>();
		goalRenderer.material = goalNormalMaterial;
		goalController.isPressed = false;
		
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

		float resetDuration = 5f; // Five seconds to reset
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
		
		foreach (Rigidbody bodyPart in bodyParts)
		{
			Vector3 localPosition = bodyPart.position;
			sensor.AddObservation(localPosition);

			Vector3 localRotation = bodyPart.rotation.eulerAngles;
			sensor.AddObservation(localRotation);
		}
		
		// Observation for the goal's relative position with respect to agent
		Vector3 centroid = CalculateCentroid();
		Vector3 relativePosition = goal.position - centroid;
		sensor.AddObservation(relativePosition);
		
		// Observation of the countdown
		// sensor.AddObservation(episodeTimer);
	}
	
	public void SetFootOnFloor(bool isLeftFoot, bool isTouchingFloor)
	{
		if (isLeftFoot)
		{
			isLeftFootOnFloor = isTouchingFloor;
		}
		else
		{
			isRightFootOnFloor = isTouchingFloor;
		}
	}
	
	public void SetBodyPartOnFloor(string bodyPartName, bool isTouchingFloor)
    {
        if (bodyPartTouchingFloor.ContainsKey(bodyPartName))
        {
            bodyPartTouchingFloor[bodyPartName] = isTouchingFloor;
        }
        else
        {
            Debug.LogWarning($"Body part {bodyPartName} not found in dictionary.");
        }
    }
	
	public void GoalIsPressed()
	{
		Debug.Log("Agent hit the pad!");
		AddReward(20f);
		AddReward(episodeTimer);
		EndEpisode();
	}
	
	public void BodyFloorRewards()
    {
        foreach (KeyValuePair<string, bool> entry in bodyPartTouchingFloor)
        {
            if (entry.Value)
            {
                AddReward(-1f);
            }
        }
    }
	
    public override void OnActionReceived(ActionBuffers actions)
    {
		float previousTime = Mathf.CeilToInt(episodeTimer);
		episodeTimer -= Time.deltaTime;
		float currentTime = Mathf.CeilToInt(episodeTimer);
        UpdateTimerDisplay();
		
		if (currentTime != previousTime && currentTime <= 60)
		{
			float distanceToGoal = Vector3.Distance(CalculateCentroid(), goal.position);
			float distReward = -Mathf.Exp(0.1f * distanceToGoal);
			AddReward(distReward);
			
			if (isLeftFootOnFloor)
			{
				AddReward(1f);
			}
			if (isRightFootOnFloor)
			{
				AddReward(1f);
			}
			
			BodyFloorRewards();
			
			if (bodyParts[1].position.y > 1.45f)
			{
				AddReward(2f);
			}
		}
        if (episodeTimer <= 0f)
        {
			// float totalReward = GetCumulativeReward();
			// Debug.Log($"Total reward this episode: {totalReward}");
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
	
	void Update()
	{
		if (Input.anyKey)
		{
			RequestDecision();
		}
	}
}
