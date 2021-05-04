using System.Collections.Generic;
using UnityEngine;

public class RecordToArray : MonoBehaviour
{
    public int[] testingResults = new int[6];
    public bool[] result;
    public List<float> dataArray;

    public bool testing = false;
    public int numberOfMeasurementsDuringTesting = 80;
    
    private static List<float> testingMeasurements = new List<float> { };

    float bciValue;
    GameManager gameData;
    SignalProcessor sp;

    // Start is called before the first frame update
    void Start()
    {
        gameData = GameObject.Find("GameManager").GetComponent<GameManager>();
        sp = GameObject.Find("GameManager").GetComponent<SignalProcessor>();
        dataArray = new List<float>();
    }

    // Update is called once per frame
    void Update()
    {
        if (testing && dataArray.Count >= numberOfMeasurementsDuringTesting)
        {
            bool[] results = sp.ProcessAll(this.dataArray);
            for (int i = 0; i < results.Length; i++)
            {
                if (results[i]) testingResults[i]++;
            }
            this.dataArray.Clear();
        }
        else if (!testing)
        {
            this.result = sp.ProcessAllOnce(this.dataArray);
            if (this.dataArray.Count > 0 && sp.isProcessed) dataArray.Clear();
        }
    }

    public void OnBCIEvent(float value)
    {
        if (testing || gameData.inputWindow == InputWindowState.Open)
        {
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