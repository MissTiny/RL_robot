using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalController : MonoBehaviour
{
	public Material normalMaterial;
	public Material pressedMaterial;
	public Vector3 pressedPositionOffset = new Vector3(0, -0.05f, 0);
    public Renderer Plane;
	public float speed = 1.0f;
	private bool isPressed = false;
	
	void Start()
	{
		GetComponent<Renderer>().material = normalMaterial;
	}
	
    private void OnTriggerEnter(Collider other)
    {
        // Don't move the target yet. Now only mark for success
        if (!isPressed && other.gameObject.tag == "Agent")
        {
            isPressed = true;
            //print("Triggered and True");
            //StartCoroutine("MoveButton", transform.position + pressedPositionOffset);
            //GetComponent<Renderer>().material = pressedMaterial;
            Plane.material = pressedMaterial;
        }
    }
	
	System.Collections.IEnumerator MoveButton(Vector3 targetPosition)
    {
        while (transform.position != targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
    }
}
