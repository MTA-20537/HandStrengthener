using System.Collections.Generic;
using System;
using UnityEngine;

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
    public bool isProcessed;
    private bool[] result;

    private GameManager gm;
    private InputWindowState previousInputWindowState;

    private void Start()
    {
        this.isProcessed = false;
        this.result = new bool[] { };

        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        previousInputWindowState = InputWindowState.Closed;
    }

    private void Update()
    {
        Debug.Log(gm.inputWindow == InputWindowState.Open && previousInputWindowState == InputWindowState.Closed);
        if (gm.inputWindow == InputWindowState.Open && previousInputWindowState == InputWindowState.Closed) this.Reset();
        previousInputWindowState = gm.inputWindow;
    }

    public static bool[] ProcessAll(List<float> data)
    {
        return new bool[]
        {
            ProcessHighestRecordedValueTrigger(data),
            ProcessThresholdValueFrequencyTrigger(data),
            ProcessSlopeTrigger(data),
            ProcessPeakAmountTrigger(data)
        };
    }

    public bool[] ProcessAllOnce(List<float> data)
    {
        if (data.Count > 0 && !this.isProcessed && gm.inputWindow == InputWindowState.Closed)
        {
            this.result = ProcessAll(data);
            this.isProcessed = true;
            return this.result;
        }
        return this.result;
    }

    public void Reset()
    {
        this.isProcessed = false;
    }

    private static float[] findPeaks(List<float> data)
    {
        // not an input algo, just a helper method for finding peaks
        int kernelSize = 10;
        int operations = (int)Math.Ceiling((data.Count + 0f) % kernelSize);
        float[] highestPeaks = new float[operations];
        for (int i = 0; i < operations; i++)
        {
            for (int j = i; j < i + kernelSize || j < data.Count; j++)
            {
                if (highestPeaks[i] < data[j]) highestPeaks[i] = data[j];
            }
        }
        return highestPeaks;
    }

    public static bool ProcessHighestRecordedValueTrigger(List<float> data)
    {
        float threshold = 0.7f;
        float highestValue = data[0];
        foreach (float dataPoint in data)
        {
            if (dataPoint > highestValue) highestValue = dataPoint;
        }
        return highestValue > threshold;
    }

    public static bool ProcessThresholdValueFrequencyTrigger(List<float> data)
    {
        float valueThreshold = 0.45f;
        int percentageThreshold = 45;
        int frequency = 0;
        foreach (float dataPoint in data)
        {
            if (dataPoint >= valueThreshold) frequency++;
        }
        float percentage = ((frequency + 0.0f) / data.Count) * 100;
        Debug.Log("percentage: " + percentage + ", percentageThreshold: " + percentageThreshold);
        return percentage >= percentageThreshold;
    }

    public static bool ProcessSlopeTrigger(List<float> data)
    {
        float valueThreshold = 0.003f;
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

    public static bool ProcessPeakAmountTrigger(List<float> data) 
    {
        float[] highestPeaks = findPeaks(data);

        float peakValueThreshold = 0.4f;
        int numberOfPeaks = 0;
        for (int i = 0; i < highestPeaks.Length; i++)
        {
            if (highestPeaks[i] >= peakValueThreshold) numberOfPeaks++;
        }

        int peakAmountThreshold = 4;
        return numberOfPeaks >= peakAmountThreshold;
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
