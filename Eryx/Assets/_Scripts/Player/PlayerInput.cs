//This script handles reading inputs from the player and passing it on to the vehicle. We 
//separate the input code from the behaviour code so that we can easily swap controls 
//schemes or even implement and AI "controller". Works together with the VehicleMovement script

using UnityEngine;
using NaughtyAttributes;

public class PlayerInput : MonoBehaviour
{
    public bool useComputerControls;
    [ShowIf("useComputerControls")] public bool autoSpeed;
	[ShowIf("useComputerControls")] public string verticalAxisName = "Vertical";        //The name of the thruster axis
    [ShowIf("useComputerControls")] public string horizontalAxisName = "Horizontal";    //The name of the rudder axis
    [ShowIf("useComputerControls")] public string brakingKey = "Brake";                //The name of the brake button

	//We hide these in the inspector because we want 
	//them public but we don't want people trying to change them
    [ReadOnly] public float thruster;			//The current thruster value
    [ReadOnly] public float rudder;         			//The current rudder value
    [ReadOnly] public bool isBraking;            //The current brake value
    [ReadOnly] public bool playerIsBoosting;

    private void Start()
    {
        //Init some things
        isBraking = false;
        playerIsBoosting = false;
    }

    void Update()
    {
        //If the player presses the Escape key and this is a build (not the editor), exit the game
        if (Input.GetButtonDown("Cancel") && !Application.isEditor)
            Application.Quit();



        //If a GameManager exists and the game is not active...
        if (GameManager.instance != null && !GameManager.instance.IsActiveGame())
		{
			//...set all inputs to neutral values and exit this method
			thruster = rudder = 0f;
			return;
		}

        //Get the values of the thruster, rudder, and brake from the input class

#if UNITY_EDITOR
        if (useComputerControls)
        {
            if (autoSpeed)
            {
                thruster = 1;
            }
            else
            {
                thruster = Mathf.Clamp01(Input.GetAxis(verticalAxisName)); // Clamp so you cannot reverse
            }
            rudder = Input.GetAxis(horizontalAxisName);
        }
        else if(UnityEditor.EditorApplication.isRemoteConnected)
        {
            thruster = 1;
            rudder = Input.acceleration.x;

        } else{
            thruster = 0;
            rudder = 0;
            Debug.LogError("No input mode selected");
        }

        if (!useComputerControls && !UnityEditor.EditorApplication.isRemoteConnected)
            return;
           
#elif UNITY_ANDROID

        thruster = 1;
        rudder = Input.acceleration.x;
        //isBraking = Input.GetButton(brakingKey);

#endif
    }

    public void Brake(bool stop){
            isBraking = stop;
    }

    public void UsePlayerBoost(){



    }

    public void PlayerPushingBoost(bool boosting){
            playerIsBoosting = boosting;

    }
}
