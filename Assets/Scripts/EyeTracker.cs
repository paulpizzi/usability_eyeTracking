using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeTracker : MonoBehaviour
{

    public bool isTracking = false;

    public void InitTracking()
    {
        isTracking = true;
        // start tobii api eye tracking

    }

    public void FinalizeTracking()
    {
        // get gaze list and visualize it

        isTracking = false;
    }


}
