using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordToArray : MonoBehaviour
{
    List<float> dataArray;
    float bciValue;
    GameManager gameData;
    // Start is called before the first frame update
    void Start()
    {
        gameData = GameObject.Find("GameManager").GetComponent<GameManager>();
        dataArray = new List<float>();
    }

    // Update is called once per frame
    void Update()
    {
        recordToArray();
    }

    public void OnBCIEvent(float value)
    {
        bciValue = value;
    }

    public void recordToArray()
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
            interpretSignal();
        }

    }

    public void interpretSignal()
    {
        dataArray.Clear();
    }

}
