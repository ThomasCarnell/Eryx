using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{

    //The game manager holds a public static reference to itself. This is often referred to
    //as being a "singleton" and allows it to be access from all other objects in the scene.
    //This should be used carefully and is generally reserved for "manager" type objects
    public static GameManager instance;

    [Expandable]
    public CharacterProfile selectedCharacter; //The current selected character
    [Expandable]
    public CourseSetup currentCourse; //The current course

    [BoxGroup("References")]
    public VehicleMovement vehicleMovement;
    [BoxGroup("References")]
    public ShipUI shipUI;
    [BoxGroup("References")]
    public LapTimeUI lapTimeUI;
    [BoxGroup("References")]
    public PlayableDirector introTimeline;
    [BoxGroup("References")]
    public IntroSequence introSequence;


    public bool raceHasBegun;
    public bool isRaceOver;

    int currentLap;

    float[] lapTimes;



    void Awake()
    {
        //If the variable instance has not be initialized, set it equal to this
        //GameManager script...
        if (instance == null)
            instance = this;
        //...Otherwise, if there already is a GameManager and it isn't this, destroy this
        //(there can only be one GameManager)
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        if (vehicleMovement == null)
        {
            vehicleMovement = FindObjectOfType<VehicleMovement>();
        }
        if (shipUI == null)
        {
            shipUI = FindObjectOfType<ShipUI>();
        }
    }

    void OnEnable()
    {
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        //Update the lap number on the ship
        UpdateUI_LapNumber();
        introTimeline.Play();

        //Initialize intro sequence script.
        introSequence.IntroSequenceInit();

        //Wait a little while to let everything initialize
        yield return new WaitForSeconds(.1f);

        //Initialize the lapTimes array and set that the race has begun
        lapTimes = new float[currentCourse.lapsNeeded];


        //Wait until intro is done
        yield return new WaitWhile(() => introTimeline.state == PlayState.Playing);


        raceHasBegun = true;
    }

    private void Update()
    {
        UpdateUI_Speed();

        if (IsActiveGame())
        {
            lapTimes[currentLap] += Time.deltaTime;
            UpdateUI_LapTime();
        }

    }

    public void PlayerCompletedLap()
    {

        if (isRaceOver)
            return;

        currentLap++;

        UpdateUI_LapNumber();

        if (currentLap >= currentCourse.lapsNeeded)
        {
            isRaceOver = true;

            UpdateUI_FinalTime();
        }
    }


    public void UpdateUI_LapNumber()
    {
        shipUI.SetLapDisplay(currentLap + 1, currentCourse.lapsNeeded);
    }

    public void UpdateUI_LapTime()
    {
        //If we have a LapTimeUI reference, update it
        if (lapTimeUI != null)
            lapTimeUI.SetLapTime(lapTimes[currentLap]);
    }

    public void UpdateUI_FinalTime()
    {


    }

    void UpdateUI_Speed()
    {
        //If we have a VehicleMovement and ShipUI reference, update it
        shipUI.SetSpeedDisplay(Mathf.Abs(vehicleMovement.speed));
    }

    public bool IsActiveGame()
    {
        //If the race has begun and the game is not over, we have an active game
        return raceHasBegun && !isRaceOver;
    }


    public void Restart()
    {
        //Restart the scene by loading the scene that is currently loaded
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
