using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform agent;
    private Vector3 initialAgentPosition;
    private Vector3 initialCameraPosition;
    private Vector3 cameraVelocity = Vector3.zero;
    public float smoothTime = 0.3f;
    
    private void Start()
    {
        initialAgentPosition = agent.position;
        initialCameraPosition = transform.position;
    }

    void Update()
    {
        float xOffset = agent.position.x - initialAgentPosition.x;
        float targetX = Mathf.Clamp(initialCameraPosition.x + xOffset, 4, 18);
        Vector3 targetCameraPosition = new Vector3(targetX, initialCameraPosition.y, initialCameraPosition.z);
        
        // SmoothDamp to gradually change the camera's position towards the target position
        transform.position = Vector3.SmoothDamp(transform.position, targetCameraPosition, ref cameraVelocity, smoothTime);
    }
}
