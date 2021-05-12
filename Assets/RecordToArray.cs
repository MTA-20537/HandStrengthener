using System.Collections.Generic;
using UnityEngine;
using System;

public class RecordToArray : FileManager
{
    public int[] testingResults = new int[7];
    public bool[] result;
    public List<float> dataArray;
    private List<float> entireRecordedSeries;

    public bool ignoreInputWindow = false;
    public int numberOfMeasurementsDuringTesting = 80;
    
    private static List<float> testingMeasurements = new List<float> { };
    private long timeOfMostRecentData = 0;

    float bciValue;
    GameManager gameData;
    SignalProcessor sp;

    // Start is called before the first frame update
    void Start()
    {
        gameData = GameObject.Find("GameManager").GetComponent<GameManager>();
        sp = GameObject.Find("GameManager").GetComponent<SignalProcessor>();
        dataArray = new List<float>();
        entireRecordedSeries = new List<float>();

        // TODO: remember to delete this...
        /*List<float> d = getDataAsArray();
        foreach (float value in d) Debug.Log(value);*/
    }

    // Update is called once per frame
    void Update()
    {
        if (ignoreInputWindow && timeOfMostRecentData != 0 && new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds() - timeOfMostRecentData > 1)
        {
            // OpenViBE signal playback has concluded...
            Debug.LogWarning("No new BCI events recieved for the last second, concluding the test...");
            // save the remaining dataArray, which was not sufficient to be processed
            foreach (float value in this.dataArray) this.entireRecordedSeries.Add(value);
            // finally, save and exit
            saveDataFromArray(this.entireRecordedSeries);
            Debug.LogWarning("Save successful, pausing execution...");
            Debug.Break();
        }
        if (ignoreInputWindow && dataArray.Count >= numberOfMeasurementsDuringTesting)
        {
            bool[] results = sp.ProcessAll(this.dataArray);
            for (int i = 0; i < results.Length; i++)
            {
                if (results[i]) testingResults[i]++;
            }
            foreach (float value in this.dataArray) this.entireRecordedSeries.Add(value);
            this.dataArray.Clear();
        }
        else if (!ignoreInputWindow)
        {
            this.result = sp.ProcessAllOnce(this.dataArray);
            if (this.dataArray.Count > 0 && sp.isProcessed) dataArray.Clear();
        }
    }

    public void OnBCIEvent(float value)
    {
        if (ignoreInputWindow || gameData.inputWindow == InputWindowState.Open)
        {
            dataArray.Add(value);
            timeOfMostRecentData = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
        }
    }

    /*public void recordToArray()
    {
        Debug.Log(gameData.inputWindow);
        if(gameData.inputWindow == InputWindowState.Open) 
        {
            dataArray.Add(bciValue);
        }
        foreach (float val in dataArray)
        {
            Debug.Log(val);
        }
        if (gameData.inputWindow == InputWindowState.Closed)
        {
            InterpretData();
        }

    }*/

}