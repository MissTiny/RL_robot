using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Distance : MonoBehaviour
{
    [SerializeField] private AgentController agent;
    [SerializeField] private TextMeshPro distanceText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        distanceText.text = Mathf.RoundToInt(agent.distance).ToString();
    }
}
