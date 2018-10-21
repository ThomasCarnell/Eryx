using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShipUI : MonoBehaviour {

    [Header("UI Text References")]
    public TextMeshProUGUI currentSpeedText;    //The text element for the current speed
    public TextMeshProUGUI currentLapText;      //The text element for the current lap
    public Slider healthSliderIndicator;
    public Slider healthSliderFinal;


    public void SetLapDisplay(int currentLap, int numberOfLaps)
    {
        //If we are trying to set a lap greater than the total number of laps, exit
        if (currentLap > numberOfLaps)
            return;

        //Update the current lap text
        currentLapText.text = currentLap + " / " + numberOfLaps;
    }

    public void SetSpeedDisplay(float currentSpeed)
    {
        //Turn the current speed into an integer and set it in the UI
        int speed = (int)currentSpeed;
        currentSpeedText.text = speed.ToString("F0");
    }

    public void InitHealth(float maxHealthAmount){

        healthSliderFinal.maxValue = maxHealthAmount;
        healthSliderIndicator.maxValue = maxHealthAmount;

        healthSliderFinal.value = maxHealthAmount;
        healthSliderIndicator.value = maxHealthAmount;
    }


    public void SetHealthDisplayFinal(float healthAmount){

        healthSliderFinal.value = healthAmount;

    }

    public void SetHealthIndicator(float healthAmount){

        healthSliderIndicator.value = healthAmount;

    }

}
