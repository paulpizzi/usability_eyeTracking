using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;




public class ResultsCalculation : MonoBehaviour
{


    private void Start()
    {
       // SaveToFile();
    }

    public float CalculateScanPathPixel(List<Vector2> _positionList)
    {
        float scanPathLength = 0.0f;
        int counter = 0;
        Vector2 tempPos = Vector2.zero;
        foreach(Vector2 pos in _positionList)
        {
            if (counter != 0)
            {
                scanPathLength += Vector2.Distance(tempPos, pos);
            }
            counter++;
            tempPos = pos;
        }
        return scanPathLength;
    }

    public float CalculateTime(List<float> _timeList)
    {
        float scanPathTime = 0.0f;

        foreach (float time in _timeList)
        {
            scanPathTime += time;
        }
        return scanPathTime;
    }

    public int GetFixationNumber(List<Vector2> _positionList)
    {
        return _positionList.Count;
    }

    public int GetSaccadeNumber(List<Vector2> _positionList)
    {
        return (_positionList.Count-1);
    }

    public void SaveToFile(List<Vector2> _positionList, List<float> _timeList,float _trackingTimeComplete, int saccadeTresh, float minFixation)
    {
        var path = Application.dataPath + "\\Results\\";
        var fileName = "Data_" + System.DateTime.Now.ToString("_yyyy-MM-dd_hh-mm-ss") + ".txt";
        {
            path += fileName;
            path = path.Replace(@"\", @"/");
            Debug.Log("Test: " + path);
            var sr = File.CreateText(path);
            sr.WriteLine("Data_File_Eye_Tracking");
            sr.WriteLine("Saccade distance[Pixel]: " + saccadeTresh + " || Fixation time[ms]: " + minFixation * 1000);
            sr.WriteLine("--------------------------------");
            sr.WriteLine("");

            sr.WriteLine("Scan path length [Pixel]: " + CalculateScanPathPixel(_positionList));
            sr.WriteLine("Total fixation Time[s]: " + CalculateTime(_timeList) + " | [ms]: " + (CalculateTime(_timeList)*1000));
            sr.WriteLine("Total Tracking Time[s]: " + _trackingTimeComplete + " | [ms]: " + (_trackingTimeComplete * 1000));
            sr.WriteLine("Fixation Number: " + GetFixationNumber(_positionList));
            sr.WriteLine("Saccade Number: " + GetSaccadeNumber(_positionList));

            sr.WriteLine("--------------------------------");
            sr.WriteLine("");
            sr.WriteLine("Position NR. | " + "Position | " + "Fixation Time [ms]" );
            int counter = 0;
            foreach(Vector2 pos in _positionList)
            {
                sr.WriteLine("Pos " + counter + " , " + pos + " , " + (_timeList[counter]*1000));
                counter++;
            }

            sr.Close();
        }
    }
   
}
