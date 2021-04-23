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

public static class SignalProcessor
{

    //public static BCITrigger[] inputTriggers;
    static private bool isProcessed = false;
    static private bool[] result = new bool[] { };

    public static bool[] ProcessAll(List<float> data)
    {
        SignalProcessor.isProcessed = true;
        return new bool[]
        {
            ProcessHighestRecordedValueTrigger(data),
            ProcessThresholdValueFrequencyTrigger(data),
            ProcessSlope(data),
            ProcessPeakAmount(data)
        };
    }

    public static bool[] ProcessAllOnce(List<float> data)
    {
        if (!SignalProcessor.isProcessed) return SignalProcessor.ProcessAll(data);
        return SignalProcessor.result;
    }

    public static void Reset()
    {
        SignalProcessor.isProcessed = false;
    }

    public static bool ProcessHighestRecordedValueTrigger(List<float> data)
    {
        float threshold = 0.8f;
        float highestValue = data[0];
        foreach (float dataPoint in data)
        {
            if (dataPoint > highestValue) highestValue = dataPoint;
        }
        return highestValue > threshold;
    }

    public static bool ProcessThresholdValueFrequencyTrigger(List<float> data)
    {
        float valueThreshold = 0.3f;
        float percentageThreshold = 0.3f;
        int frequency = 0;
        foreach (float dataPoint in data)
        {
            if (dataPoint >= valueThreshold) frequency++;
        }
        float percentage = (frequency / 100) * data.Count;
        return percentage >= percentageThreshold;
    }

    public static bool ProcessSlope(List<float> data)
    {
        float valueThreshold = 0.002f;
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

    public static bool ProcessPeakAmount(List<float> data) 
    {
        int kernelSize = 10;
        int operations = (int) Math.Ceiling((data.Count + 0f) % kernelSize);
        float[] highestPeaks = new float[operations];
        for (int i = 0; i < operations; i++)
        {
            for (int j = i; j < i + kernelSize || j < data.Count; j++)
            {
                if (highestPeaks[i] != 0.0 || highestPeaks[i] < data[j]) highestPeaks[i] = data[j];
            }
        }

        float peakValueThreshold = 0.5f;
        int numberOfPeaks = 0;
        for (int i = 0; i < highestPeaks.Length; i++)
        {
            if (highestPeaks[i] >= peakValueThreshold) numberOfPeaks++;
        }

        int peakAmounThreshold = 5;
        return numberOfPeaks >= peakAmounThreshold;
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
