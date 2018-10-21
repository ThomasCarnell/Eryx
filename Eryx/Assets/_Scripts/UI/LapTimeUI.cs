using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LapTimeUI : MonoBehaviour {

    public TextMeshProUGUI currentRaceTime; //An array of TextMesh Pro text elements
    public TextMeshProUGUI finalTimeLabel;  //The text element for the finish time

    void Awake()
    {
        currentRaceTime.text = "";
        //finalTimeLabel.text = "";
    }

    public void SetLapTime(float lapTime)
    {

        //Convert the time to a string and set the string to show on the text 
        //element of the current lap
        currentRaceTime.text = ConvertTimeToString(lapTime);
    }

    public void SetFinalTime(float lapTime)
    {
        //Convert the time to a string and set the string to show on the text 
        //element of the final time label
        finalTimeLabel.text = ConvertTimeToString(lapTime);
    }

    string ConvertTimeToString(float time)
    {
        //Take the time and convert it into the number of minutes and seconds
        int minutes = (int)(time / 60);
        float seconds = time % 60f;

        //Create the string in the appropriate format for the time
        string output = minutes.ToString("00") + ":" + seconds.ToString("00.000");
        return output;
    }
}
