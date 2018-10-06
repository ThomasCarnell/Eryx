using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Speedomitor : MonoBehaviour {

    TextMeshProUGUI speedomitorValue;

	// Use this for initialization
	void Start () {
        speedomitorValue = GetComponent<TextMeshProUGUI>();
    }
	
    public void SetSpeedomitor(float speed){
        speedomitorValue.text = speed.ToString("F0");
    }
}
