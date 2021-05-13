using System.Collections.Generic;
using UnityEngine;

public class RecordToArray : MonoBehaviour
{
    public int[] testingResults = new int[7];
    public bool[] result;
    public List<float> dataArray;

    public bool ignoreInputWindow = false;
    public int numberOfMeasurementsDuringTesting = 80;
    
    private static List<float> testingMeasurements = new List<float> { };

    float bciValue;
    GameManager gameData;
    SignalProcessor sp;
    LoggingManager lm;

    // Start is called before the first frame update
    void Start()
    {
        gameData = GameObject.Find("GameManager").GetComponent<GameManager>();
        sp = GameObject.Find("GameManager").GetComponent<SignalProcessor>();
        lm = GameObject.Find("LoggingManager").GetComponent<LoggingManager>();
        dataArray = new List<float>();

        sp.onSignalProcessedOnce.AddListener(this.LogTriggers);
    }

    private void LogTriggers(bool[] triggers)
    {
        // only log the results if we are not in testing mode (ignoreInputWindow == true)
        Dictionary<string, object> resultsDictionary = new Dictionary<string, object>() {
            { "Trigger0", triggers[0] ? 1 : 0 },
            { "Trigger1", triggers[1] ? 1 : 0 },
            { "Trigger2", triggers[2] ? 1 : 0 },
            { "Trigger3", triggers[3] ? 1 : 0 },
            { "Trigger4", triggers[4] ? 1 : 0 },
            { "Trigger5", triggers[5] ? 1 : 0 },
            { "Trigger6", triggers[6] ? 1 : 0 }
        };
        lm.Log("Game", resultsDictionary);
        //lm.Log("Game", "Trigger1", 1);

        for (int i = 0; i < triggers.Length; i++) Debug.Log("Logged the following trigger at index " + i + " with value " + (triggers[i] ? 1 : 0));
        //Debug.Log("Triggers logged!");
        /*resultsDictionary.Add("Triggers", new { "0": triggers[0] });
        lm.Log("Game", resultsDictionary);*/
    }

    public void OnDestroy()
    {
        // TODO: not sure when the lm does this by itself. This should probably be removed when done with testing!
        lm.SaveAllLogs();
    }

    // Update is called once per frame
    void Update()
    {
        if (ignoreInputWindow && dataArray.Count >= numberOfMeasurementsDuringTesting)
        {
            bool[] results = sp.ProcessAll(this.dataArray);
            for (int i = 0; i < results.Length; i++)
            {
                if (results[i]) testingResults[i]++;
            }
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
            // save data point to memory for processing
            dataArray.Add(value);
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