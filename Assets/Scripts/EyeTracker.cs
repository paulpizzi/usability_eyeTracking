using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;
using UnityEngine.UI;

public class EyeTracker : MonoBehaviour
{ 
    public GameObject circleImage;
    public Camera cam;
    public UIHandler uiHandler;

    public InputField saccadesTreshholdPixelInput;
    public InputField minFixationTimeInput;

    public ResultsCalculation resultsCalculation;
    public LineRenderer lineRenderer;

    public bool isTracking = false;
    private int saccadesTreshholdPixel = 120;
    private float minFixationTime = 0.2f;

    private List<GameObject> circleList = new List<GameObject>();
    private List<float> timeList = new List<float>();
    private List<Vector2> positionList = new List<Vector2>();

    private Vector2 savedGazePos;
    private bool positionSaved = false;

    private float trackingTimeComplete;
    private float fixationTime = 0;

    private float minCricleSize = 0.3f;
    private float maxCricleSize = 2f;








    private void Update()
    {
        ReadKeyboardInput();

        if (isTracking)
        {
            EyeTracking();
        }
    }

    private void EyeTracking()
    {
        trackingTimeComplete += Time.deltaTime;
        GazePoint gazePoint = TobiiAPI.GetGazePoint();
        TestIfGazeIsOverThreshhold(gazePoint.Screen);

    }

    private void ReadKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isTracking)
        {
            ResetTrackingData();
            uiHandler.OnStartTrackingBtn();
        }
        else if ((Input.GetKeyDown(KeyCode.Space) && isTracking))
        {
            uiHandler.OnStopTrackingBtn();
            if (positionList.Count > timeList.Count)
            {
                AddFixationDuration(fixationTime);
            }
            DrawAll();
            SaveDataToFile();
            // DebugLogData();
        }
    }

    public void SaveDataToFile()
    {
        resultsCalculation.SaveToFile(positionList,timeList, trackingTimeComplete,saccadesTreshholdPixel ,minFixationTime);
    }

    public void InitTracking()
    {
        ReadTextInputFields();
        isTracking = true;
    }

    public void FinalizeTracking()
    {
        isTracking = false;
    }

    private void DebugLogData()
    {
        Debug.Log("scanPixel: " + resultsCalculation.CalculateScanPathPixel(positionList));
        Debug.Log("scanTime: " + resultsCalculation.CalculateTime(timeList));
        Debug.Log("saccadeNR: " + resultsCalculation.GetSaccadeNumber(positionList));
        Debug.Log("PosNR: " + resultsCalculation.GetFixationNumber(positionList));
    }
   

    public void PlaceCircle(Vector2 pos, int circleCounter,float sizeCircle)
    {
     
        Vector3 point = cam.ScreenToWorldPoint(new Vector3(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), cam.nearClipPlane));
        GameObject circlerObject = Instantiate(circleImage, point, Quaternion.identity);
        circleList.Add(circlerObject);
        var scale = circlerObject.transform.localScale;
        circlerObject.transform.localScale = new Vector3(scale.x * sizeCircle, scale.y * sizeCircle, scale.z * sizeCircle);
        circlerObject.GetComponentInChildren<TextMesh>().text = circleCounter.ToString();
    }

    public void TestIfGazeIsOverThreshhold(Vector2 gazePos)
    {
       
        fixationTime += Time.deltaTime;

        if (Mathf.RoundToInt(Vector2.Distance(gazePos,savedGazePos)) >= saccadesTreshholdPixel)
        {

            positionSaved = false;
            if (fixationTime > minFixationTime)
            {
                AddFixationDuration(fixationTime);
            }
            fixationTime = 0;
            savedGazePos = gazePos;
        }
       

        if (fixationTime > minFixationTime && !positionSaved)
        {
            positionSaved = true;
            positionList.Add(gazePos);
        }
    } 


    private void DrawSaccadeLines()
    {
        lineRenderer.positionCount = positionList.Count;
        int posCounter = 0;
        foreach (Vector2 pos in positionList)
        {
            Vector3 gazePosWorld = cam.ScreenToWorldPoint(new Vector3(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), cam.nearClipPlane));
            lineRenderer.SetPosition(posCounter, gazePosWorld);
            posCounter++;       
        }
    }


    private void AddFixationDuration(float _time)
    {
        timeList.Add(_time);
    }

    private float ScaleCircleValue()
    {
        float maxValue = 0;
        foreach (float time in timeList)
        {
            maxValue = Mathf.Max(maxValue,time);
        }

        float scaleFactor = (maxCricleSize - minCricleSize) / maxValue;      
        return scaleFactor;
    }

    private void DrawAll()
    {
        float scaleFactor = ScaleCircleValue();
        int counter = 0;  
        foreach (Vector2 pos in positionList)
        {       
            
            PlaceCircle(pos,counter,timeList[counter]*scaleFactor + minCricleSize);         
            counter++;
        }
        DrawSaccadeLines();
    }

    private void ResetTrackingData()
    {
        savedGazePos = Vector2.zero;
        positionSaved = false;
        lineRenderer.positionCount = 0;
        timeList.Clear();
        trackingTimeComplete = 0;
        fixationTime = 0;
        positionList.Clear();
        if (circleList.Count != 0)
        {
            foreach (GameObject circleObj in circleList)
            {
                Destroy(circleObj);
            }
            circleList.Clear();
        }
    }

    private void ReadTextInputFields()
    {
        if (saccadesTreshholdPixelInput.text.Length > 0)
        {
            saccadesTreshholdPixel = int.Parse(saccadesTreshholdPixelInput.text);
        }

        if (minFixationTimeInput.text.Length > 0)
        {
            minFixationTime = float.Parse(minFixationTimeInput.text, System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}

