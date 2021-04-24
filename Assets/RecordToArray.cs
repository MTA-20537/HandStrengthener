using System.Collections.Generic;
using UnityEngine;

public class RecordToArray : MonoBehaviour
{
    public bool[] result;
    public List<float> dataArray;

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
        this.result = sp.ProcessAllOnce(this.dataArray);
        if (this.dataArray.Count > 0 && sp.isProcessed) dataArray.Clear();
    }

    public void OnBCIEvent(float value)
    {
        if (gameData.inputWindow == InputWindowState.Open)
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
