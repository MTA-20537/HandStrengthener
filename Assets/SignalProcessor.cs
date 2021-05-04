using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

/*public struct BCIOutput
{

    bool[] results;

    public BCIOutput(params bool[] results)
    {
        this.results = results;
    }

}*/

/*public abstract class BCITrigger
{
    protected List<float> data;
    protected bool result;
    protected bool isCalculated;

    public BCITrigger(List<float> data)
    {
        this.data = data;
        this.result = false;
        this.isCalculated = false;
    }

    public abstract List<float> Process();

    public abstract bool Benchmark(List<float> processedData);

    public bool isTriggered()
    {
        this.result = this.Benchmark(this.Process());
        this.isCalculated = true;
        return this.result;
    }

    public bool isTriggeredOnce()
    {
        bool output = this.result;
        if (!this.isCalculated) output = this.isTriggered();
        return output;
    }

}*/

public class SignalProcessor : MonoBehaviour
{
    //public static BCITrigger[] inputTriggers;

    public bool useAdaptiveThreshold;
    public float avgTriggersPerCycle;
    private List<bool[]> resultsHistory;
    private int resultsMemorySize = 10;
    private float targetTriggersPerCycle = 1.5f;
    private List<float> thresholdHistory;

    public bool isProcessed;

    public float inputThreshold;
    private float benchmarkInputThreshold = 0.85f;
    private float thresholdRatio;

    private bool[] result;
    private GameManager gm;
    private InputWindowState previousInputWindowState;

    [Serializable]
    public class OnSignalProcessedOnce : UnityEvent<bool[]> { }
    public OnSignalProcessedOnce onSignalProcessedOnce;

    private void Start()
    {
        this.resultsHistory = new List<bool[]>();
        this.thresholdHistory = new List<float>();

        this.isProcessed = false;
        this.result = new bool[] { };

        this.thresholdRatio = this.inputThreshold / this.benchmarkInputThreshold;

        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        previousInputWindowState = InputWindowState.Closed;
    }

    private void Update()
    {
        if (gm.inputWindow == InputWindowState.Open && previousInputWindowState == InputWindowState.Closed) this.Reset();
        previousInputWindowState = gm.inputWindow;
    }

    public bool[] ProcessAll(List<float> data)
    {
        // calculate threshold and trigger results
        float threshold = this.getThreshold();
        bool[] newResult = new bool[]
        {
            ProcessHighestRecordedValueTrigger(     data, threshold),
            ProcessThresholdValueFrequencyTrigger(  data, threshold),
            ProcessSlopeTrigger(                    data, threshold),
            ProcessPeakAmountTrigger(               data, threshold),
            ProcessDistanceBetweenPeaksTrigger(     data, threshold)
        };

        // append new results to memory (used for adaptive thresholding)
        this.resultsHistory.Add(newResult);
        if (this.resultsHistory.Count > this.resultsMemorySize) this.resultsHistory.RemoveAt(0);

        // calculate the average triggers per cycle in memory, to be displayed on the Unity inspector, allowing for supervision during experiments
        int totalTriggers = 0;
        this.forEachTrigger((i, j, trigger, array) =>
        {
            if (trigger) totalTriggers++;
        });
        this.avgTriggersPerCycle = (totalTriggers + 0.0f) / this.resultsHistory.Count;

        // return new trigger results
        return newResult;
    }

    public bool[] ProcessAllOnce(List<float> data)
    {
        if (data.Count > 0 && !this.isProcessed && gm.inputWindow == InputWindowState.Closed)
        {
            this.result = ProcessAll(data);

            this.onSignalProcessedOnce.Invoke(this.result);
            this.isProcessed = true;

            return this.result;
        }
        return this.result;
    }

    public void Reset()
    {
        this.isProcessed = false;
    }

    public void forEachTrigger(Action<int, int, bool, List<bool[]>> invoke)
    {
        // apply given Action on every trigger of every cycle in memory
        for (int i = 0; i < this.resultsHistory.Count; i++)
        {
            for (int j = 0; j < this.resultsHistory[i].Length; j++)
            {
                invoke(i, j, this.resultsHistory[i][j], this.resultsHistory);
            }
        }
    }

    private float getThreshold(int triggerIndex = 0)
    {
        // adaptive trigger/threshold implementation
        // NOTE: currently triggerIndex is not being used, but it can be used in case we want to inforce predictable trigger variability between algos

        // only adapt theshold when there's enough cycles for averages to make sense
        if (this.useAdaptiveThreshold && this.resultsHistory.Count >= 1/*this.resultsMemorySize*/)
        {
            // find the number of triggers in the most recent cycle to act as a deccelerator
            int mostRecentTriggers = 0;
            this.forEachTrigger((i, j, trigger, array) =>
            {
                if (i == this.resultsHistory.Count - 1)  // isolate the most recent trigger cycle
                {
                    if (trigger) mostRecentTriggers++;
                }
            });

            // the degree to which the treshold must be adjusted to achieve desired triggers per cycle
            float newOptimalTriggerRatio = this.targetTriggersPerCycle / ((this.avgTriggersPerCycle + mostRecentTriggers) / 2);  // weigh the most recent trigger results higher than the others

            /*Debug.Log("mostRecentTriggers: "      + mostRecentTriggers);
            Debug.Log("avgTriggersPerCycle: "       + this.avgTriggersPerCycle);
            Debug.Log("newOptimalTriggerRatio: "    + newOptimalTriggerRatio);*/

            // update threshold if the required change is sufficiently large
            if (newOptimalTriggerRatio > 1.5 || newOptimalTriggerRatio < 0.6)
            {
                float newThresholdRatio = (this.thresholdRatio / newOptimalTriggerRatio + this.thresholdRatio) / 2;
                Debug.LogWarning("New adaptive threshold set (major correction): " + this.thresholdRatio + " -> " + newThresholdRatio + (this.thresholdRatio - newThresholdRatio > 0 ? " (easier)": " (harder)"));
                this.thresholdRatio = newThresholdRatio;
            }
            else if (this.thresholdHistory.Count >= 1)
            {
                float totalThresholdValues = 0f;
                foreach (float threshold in this.thresholdHistory) totalThresholdValues += threshold;
                float newThresholdRatio = ((totalThresholdValues / this.thresholdHistory.Count) + this.thresholdHistory[this.thresholdHistory.Count - 1]) / 2;
                if (Mathf.Abs(this.thresholdRatio - newThresholdRatio) > 0.1)
                {
                    Debug.LogWarning("New adaptive threshold set (minor correction): " + this.thresholdRatio + " -> " + newThresholdRatio + (this.thresholdRatio - newThresholdRatio > 0 ? " (easier)" : " (harder)"));
                    this.thresholdRatio = newThresholdRatio;
                }
            }

            // update and trim threshold history
            this.thresholdHistory.Add(this.thresholdRatio);
            if (this.thresholdHistory.Count > this.resultsMemorySize) this.thresholdHistory.RemoveAt(0);
        }

        // return appropriate threshold ratio, used to convert individual threshold values in each algo
        return this.thresholdRatio;
    }

    private float[] findPeaks(List<float> data)
    {
        // not an input algo, just a helper method for finding peaks
        int kernelSize = 10;
        int operations = (int)Math.Ceiling((data.Count + 0f) / kernelSize);
        float[] highestPeaks = new float[operations];
        for (int i = 0; i < operations; i++)
        {
            for (int j = i * kernelSize; j < i * kernelSize + kernelSize && j < data.Count; j++)
            {
                if (highestPeaks[i] < data[j]) highestPeaks[i] = data[j];
            }
        }
        return highestPeaks;
    }

    public bool ProcessHighestRecordedValueTrigger(List<float> data, float globalThreshold)
    {
        float threshold = 0.5f * globalThreshold;
        float highestValue = data[0];
        foreach (float dataPoint in data)
        {
            if (dataPoint > highestValue) highestValue = dataPoint;
        }
        return highestValue > threshold;
    }

    public bool ProcessThresholdValueFrequencyTrigger(List<float> data, float globalThreshold)
    {
        float valueThreshold = 0.4f * globalThreshold;
        int percentageThreshold = 4;
        int frequency = 0;
        foreach (float dataPoint in data)
        {
            if (dataPoint >= valueThreshold) frequency++;
        }
        float percentage = ((frequency + 0.0f) / data.Count) * 100;
        return percentage >= percentageThreshold;
    }

    public bool ProcessSlopeTrigger(List<float> data, float globalThreshold)
    {
        float valueThreshold = 0.0048f * globalThreshold;
        float peakValue = data[0];
        float peakIndex = 0;
        for(int i = 0; i<data.Count; i++)
        {
            if (data[i] > peakValue) 
            { 
                peakValue = data[i];
                peakIndex = i;

            }
        }
        float a = (peakValue - data[0]) / (peakIndex);
        return a > valueThreshold;
    }

    public bool ProcessPeakAmountTrigger(List<float> data, float globalThreshold) 
    {
        float[] highestPeaks = findPeaks(data);

        float peakValueThreshold = 0.36f * globalThreshold;
        int numberOfPeaks = 0;
        for (int i = 0; i < highestPeaks.Length; i++)
        {
            if (highestPeaks[i] >= peakValueThreshold) numberOfPeaks++;
        }

        int peakAmountThreshold = 4;
        return numberOfPeaks >= peakAmountThreshold;
    }

    public bool ProcessDistanceBetweenPeaksTrigger(List<float> data, float globalThreshold)
    {
        int kernelSize = 10;
        int operations = (int)Math.Ceiling((data.Count + 0f) / kernelSize);
        float[] highestPeaks = new float[operations];
        int[] highestPeaksIndex = new int[operations];
        highestPeaksIndex[0] = 0;
        for (int i = 0; i < operations; i++)
        {
            for (int j = i * kernelSize; j < i * kernelSize + kernelSize && j < data.Count; j++)
            {
                if (highestPeaks[i] < data[j])
                {
                    highestPeaks[i] = data[j];
                    highestPeaksIndex[i] = j;
                }
            }
        }
        int total = 0;
        int previousValue = highestPeaksIndex[0];
        foreach (int val in highestPeaksIndex)
        {
            total += (val - previousValue);
            previousValue = val;
        }
        float avg = total / highestPeaksIndex.Length;
        float thresholdValue = 8f / globalThreshold;
        //Debug.Log(avg);
        return avg < thresholdValue;
    }


    /*public class HighestRecordedValueTrigger : BCITrigger
    {

        public HighestRecordedValueTrigger(List<float> data) : base(data) { }

        public override List<float> Process()
        {
            throw new System.NotImplementedException();
        }

        public override bool Benchmark(List<float> processedData)
        {
            throw new System.NotImplementedException();
        }

    }*/

}
