using UnityEngine;
using System;

public class FrameRate : MonoBehaviour {

    public bool showInputs;
    public bool onScreenFPS;
    public bool showHighAndLow;
    public int highAndLowRefreshRate = 30;
    public bool averageFPSOn;

    private float averageFPS;
    string input;

    public float updateInterval = 0.5F;
    private double lastInterval;
    private int frames;
    private int averageframes;
    private float fps;
    private float fpsHigh;
    private float fpsLow;
    private int counter;

    void Start() {
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
        averageframes = 0;
        fpsLow = 1000;
        fpsHigh = 0;
        counter = highAndLowRefreshRate;
    }

    void OnGUI() {
        if (onScreenFPS) {
            GUILayout.Box("FPS: " + fps.ToString("f2"));
            if (showHighAndLow) {
                GUILayout.Box("Highest FPS: " + fpsHigh.ToString("f2"));
                GUILayout.Box("Lowest FPS: " + fpsLow.ToString("f2"));
            }
        }

        if (averageFPSOn) {
            GUILayout.Box("Average FPS: " + averageFPS.ToString("f2"));
            //GUILayout.Label("Frames: " + counter.ToString("f2"));
        }

        if (showInputs)
        {
            GUILayout.Box(String.Format("Last Input = {0}", input));
        }
    }

    void Update() {
        if (onScreenFPS) {
            ++frames;

            float timeNow = Time.realtimeSinceStartup;
            if (timeNow > lastInterval + updateInterval) {
                fps = (float)(frames / (timeNow - lastInterval));
                if (showHighAndLow) {
                    if (counter != 0) {
                        counter++;
                        if (counter > highAndLowRefreshRate) {
                            fpsLow = 1000;
                            fpsHigh = 0;
                            counter = 1;
                        }
                    }
                        
                    if (fps < fpsLow)
                        fpsLow = fps;

                    if (fps > fpsHigh)
                        fpsHigh = fps;
                }
                
                frames = 0;
                lastInterval = timeNow;
            }



        }

        if (averageFPSOn) {
            ++averageframes;
            averageFPS = averageframes / Time.realtimeSinceStartup;
        }

        if (showInputs)
        {
            if (Input.anyKeyDown)
            {
                input = Input.inputString;
            }
            if (Input.GetMouseButtonDown(0))
            {
                input = "Left Mouse Button";
            }
            if (Input.GetMouseButtonDown(1))
            {
                input = "Right Mouse Button";
            }
        }

    }
}
