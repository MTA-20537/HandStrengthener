using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class TriggerTester : FileManager
{
    public int measurementsPerActive;
    public int measurementsPerRest;

    SignalProcessor sp;
    List<float> source;

    // Start is called before the first frame update
    void Start()
    {
        sp = GameObject.Find("GameManager").GetComponent<SignalProcessor>();
        source = getDataAsArray();

        // TODO: remove this eventually...
        Invoke("testOnceOnStaticConditions", 1);
    }

    private void printTestSummary(List<float> results)
    {
        // ...
    }

    public void testOnceOnStaticConditions()
    {
        // split the source data into chunks given by class variables
        int totalCycles = Mathf.FloorToInt(source.Count / (measurementsPerActive + measurementsPerRest));
        List<bool[]> results = new List<bool[]>();
        for (int i = 0; i < totalCycles; i++)
        {
            List<float> domain;
            if (i == 0) domain = new List<float>(source.Take(measurementsPerActive));
            else        domain = new List<float>(source.Skip(i * (measurementsPerActive + measurementsPerRest)).Take(measurementsPerActive));
            results.Add(sp.ProcessAll(domain));
        }
        
        // hand off for further processing?

        // convert the results to the expected format by FileManager
        List<float> writeableResults = new List<float>();
        foreach (bool[] resultsSet in results)
        {
            foreach (bool result in resultsSet)
            {
                if (result) writeableResults.Add(1f);
                else writeableResults.Add(0f);
            }
        }

        // save data and display it
        saveDataFromArray(writeableResults, 
            "a" + measurementsPerActive + "r" + measurementsPerRest + "d" + new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds());
        printTestSummary(writeableResults);
    }
}
