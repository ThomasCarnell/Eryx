using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenResolution : MonoBehaviour {

    public Vector2 screenSize;

    void Start()
    {
        Screen.SetResolution((int)screenSize.x, (int)screenSize.y, true);
    }

}
