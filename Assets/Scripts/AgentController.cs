using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using TMPro;

using Random = UnityEngine.Random;

public class AgentController : Agent
{
	[SerializeField] private Rigidbody[] bodyParts;
	[SerializeField] private Transform[] feet;
	private bool isLeftFootOnFloor = false;
	private bool isRightFootOnFloor = false;
	// private bool wasLeftFootOnFloor = false;
	// private bool wasRightFootOnFloor = false;
	// private float timeSinceLastStep = 0f;
	
	private Dictionary<string, bool> bodyPartTouchingFloor = new Dictionary<string, bool>();
	
	[SerializeField] private TextMeshProUGUI timerText;
	
	[SerializeField] private GoalController goalController;
	[SerializeField] private Transform goal;
	[SerializeField] private Material goalNormalMaterial;
	private Vector3 initialGoalPosition;
	private Renderer goalRenderer;
	
	[SerializeField] private CheckpointController cp1Controller;
	[SerializeField] private CheckpointController cp2Controller;
	[SerializeField] private Transform checkpoint1;
	[SerializeField] private Transform checkpoint2;
	[SerializeField] private CheckpointController cp3Controller;
	[SerializeField] private CheckpointController cp4Controller;
	[SerializeField] private Transform checkpoint3;
	[SerializeField] private Transform checkpoint4;
	[SerializeField] private CheckpointController cp5Controller;
	[SerializeField] private CheckpointController cp6Controller;
	[SerializeField] private Transform checkpoint5;
	[SerializeField] private Transform checkpoint6;
	[SerializeField] private CheckpointController cp7Controller;
	[SerializeField] private CheckpointController cp8Controller;
	[SerializeField] private Transform checkpoint7;
	[SerializeField] private Transform checkpoint8;
	[SerializeField] private CheckpointController cp9Controller;
	[SerializeField] private CheckpointController cp10Controller;
	[SerializeField] private Transform checkpoint9;
	[SerializeField] private Transform checkpoint10;
	[SerializeField] private CheckpointController cp11Controller;
	[SerializeField] private CheckpointController cp12Controller;
	[SerializeField] private Transform checkpoint11;
	[SerializeField] private Transform checkpoint12;
	[SerializeField] private CheckpointController cp13Controller;
	[SerializeField] private CheckpointController cp14Controller;
	[SerializeField] private Transform checkpoint13;
	[SerializeField] private Transform checkpoint14;
	
	[SerializeField] private Material checpointNormalMaterial;
	private Vector3 initialCP1Position;
	private Vector3 initialCP2Position;
	private Renderer checkpoint1Renderer;
	private Renderer checkpoint2Renderer;
	private Renderer checkpoint3Renderer;
	private Renderer checkpoint4Renderer;
	private Renderer checkpoint5Renderer;
	private Renderer checkpoint6Renderer;
	private Renderer checkpoint7Renderer;
	private Renderer checkpoint8Renderer;
	private Renderer checkpoint9Renderer;
	private Renderer checkpoint10Renderer;
	private Renderer checkpoint11Renderer;
	private Renderer checkpoint12Renderer;
	private Renderer checkpoint13Renderer;
	private Renderer checkpoint14Renderer;

	
	private Vector3[] initialPositions;
    private Quaternion[] initialRotations;
	
	[SerializeField] private Transform firstWall;
	[SerializeField] private Transform secondWall;
	private Vector3 initialFirstWallPosition;
	private Vector3 initialSecondWallPosition;
	
	public float rotationSpeed = 800000000f;
	
	private float episodeMaxTimer = 65f;
	private float  resetDuration = 5f;
	private float episodeTimer;
	private float maxDiffDistance;
	private float latestDistance; 
	private bool initDistance;
	
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
		goalRenderer = goal.GetComponent<Renderer>();
		
		checkpoint1Renderer = checkpoint1.GetComponent<Renderer>();
		checkpoint2Renderer = checkpoint2.GetComponent<Renderer>();
		checkpoint3Renderer = checkpoint3.GetComponent<Renderer>();
		checkpoint4Renderer = checkpoint4.GetComponent<Renderer>();
		checkpoint5Renderer = checkpoint5.GetComponent<Renderer>();
		checkpoint6Renderer = checkpoint6.GetComponent<Renderer>();
		checkpoint7Renderer = checkpoint7.GetComponent<Renderer>();
		checkpoint8Renderer = checkpoint8.GetComponent<Renderer>();
		checkpoint9Renderer = checkpoint9.GetComponent<Renderer>();
		checkpoint10Renderer = checkpoint10.GetComponent<Renderer>();
		checkpoint11Renderer = checkpoint11.GetComponent<Renderer>();
		checkpoint12Renderer = checkpoint12.GetComponent<Renderer>();
		checkpoint13Renderer = checkpoint13.GetComponent<Renderer>();
		checkpoint14Renderer = checkpoint14.GetComponent<Renderer>();


		initialCP1Position = checkpoint1.position;
		initialCP2Position = checkpoint2.position;
		
		initialFirstWallPosition = firstWall.position;
		initialSecondWallPosition = secondWall.position;
		
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
		episodeTimer = episodeMaxTimer;
		
		// Reset the position and material of the goal
		goal.position = initialGoalPosition;
		goalRenderer.material = goalNormalMaterial;
		goalController.isPressed = false;
		
		// Reset the position and material of two checkpoint pads
		checkpoint1Renderer.material = checpointNormalMaterial;
		checkpoint2Renderer.material = checpointNormalMaterial;
		cp1Controller.isPressed = false;
		cp2Controller.isPressed = false;

		checkpoint3Renderer.material = checpointNormalMaterial;
		checkpoint4Renderer.material = checpointNormalMaterial;
		cp3Controller.isPressed = false;
		cp4Controller.isPressed = false;
		checkpoint5Renderer.material = checpointNormalMaterial;
		checkpoint6Renderer.material = checpointNormalMaterial;
		cp5Controller.isPressed = false;
		cp6Controller.isPressed = false;
		checkpoint7Renderer.material = checpointNormalMaterial;
		checkpoint8Renderer.material = checpointNormalMaterial;
		cp7Controller.isPressed = false;
		cp8Controller.isPressed = false;
		checkpoint9Renderer.material = checpointNormalMaterial;
		checkpoint10Renderer.material = checpointNormalMaterial;
		cp9Controller.isPressed = false;
		cp10Controller.isPressed = false;
		checkpoint11Renderer.material = checpointNormalMaterial;
		checkpoint12Renderer.material = checpointNormalMaterial;
		cp11Controller.isPressed = false;
		cp12Controller.isPressed = false;
		checkpoint13Renderer.material = checpointNormalMaterial;
		checkpoint14Renderer.material = checpointNormalMaterial;
		cp13Controller.isPressed = false;
		cp14Controller.isPressed = false;
		initDistance=false;
		// int rand = Random.Range(0, 2);
		// if (rand == 0)
		// {
			// firstWall.position = new Vector3(initialFirstWallPosition.x, initialFirstWallPosition.y, initialFirstWallPosition.z - 3.4f);
			// secondWall.position = new Vector3(initialSecondWallPosition.x, initialSecondWallPosition.y, initialSecondWallPosition.z + 3.4f);
			// checkpoint1.position = new Vector3(initialCP1Position.x, initialCP1Position.y, initialCP1Position.z + 6.7f);
			// checkpoint2.position = new Vector3(initialCP2Position.x, initialCP2Position.y, initialCP2Position.z - 6.7f);
		// }
		// else
		// {
			// firstWall.position = initialFirstWallPosition;
			// secondWall.position = initialSecondWallPosition;
			// checkpoint1.position = initialCP1Position;
			// checkpoint2.position = initialCP2Position;
		// }
		
		//StartCoroutine(ResetBodyParts());
		StartCoroutine(ResetBodyParts());

		Debug.Log($"Episode Begins"); 
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

		//float resetDuration = 5f; // Five seconds to reset
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
		AddReward(20f);
		
		AddReward(episodeTimer);
		Debug.Log($"Goal is Pressed at {episodeTimer}"); 
		EndEpisode();
	}
	
	public void CheckpointIsPressed()
	{
		AddReward(10f);
		Debug.Log($"CheckPoint is Pressed at {episodeTimer}"); 
	}
	
	public void ObstacleIsTriggered()
	{
		AddReward(-0.1f);
		Debug.Log($"Obstacle is Triggered at {episodeTimer}"); 
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
		// timeSinceLastStep += Time.deltaTime;
		float currentTime = Mathf.CeilToInt(episodeTimer);
        UpdateTimerDisplay();
		if (initDistance == false && currentTime==episodeMaxTimer - resetDuration)
		{
			latestDistance  = Vector3.Distance(CalculateCentroid(), goal.position); 
			initDistance=true;
			Debug.Log($"Time {currentTime}., Episode Begins. Current Distance to Goal: {latestDistance}");
		}
				
		if (currentTime != previousTime && currentTime <= episodeMaxTimer - resetDuration)
		{
			// Boost distance to encourage walk further
			// Vector3 goalBuff = new Vector3(goal.position.x + 20, goal.position.y, goal.position.z);
			// float distanceToGoal = Vector3.Distance(CalculateCentroid(), goalBuff);
			// float distReward = -Mathf.Exp(0.1f * distanceToGoal);
			// AddReward(distReward);

			float distanceToGoal = Vector3.Distance(CalculateCentroid(), goal.position); 
			float distReward = latestDistance - distanceToGoal; 
			if (distReward >0){
				latestDistance = distanceToGoal; 
				AddReward(distReward);
			}
 
			//Debug.Log($"Action taken: Current distance from goal is {latestDistance}, Reward {distReward}");
			if (isLeftFootOnFloor | isRightFootOnFloor)
			{
				AddReward(1f);
			}
			
			BodyFloorRewards();
			
			if (bodyParts[1].position.y < 1.45f)
			{
				AddReward(-2f);
			}
			else
			{
				AddReward(1f);
			}
		}
        if (episodeTimer <= 0f)
        {
			// float totalReward = GetCumulativeReward();
			// Debug.Log($"Total reward this episode: {totalReward}");
			Debug.Log($"Times up: Current Body Position is {CalculateCentroid()}");
            EndEpisode();
        }
		
		// if (isLeftFootOnFloor != wasLeftFootOnFloor || isRightFootOnFloor != wasRightFootOnFloor)
			// {
				// if (isLeftFootOnFloor != isRightFootOnFloor)
				// {
					// AddReward(1f);
					// timeSinceLastStep = 0f;
				// }
			// }
			// else if (Mathf.CeilToInt(timeSinceLastStep) >= 2)
			// {
				// AddReward(-0.01f);
			// }

		// wasLeftFootOnFloor = isLeftFootOnFloor;
		// wasRightFootOnFloor = isRightFootOnFloor;
		
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
