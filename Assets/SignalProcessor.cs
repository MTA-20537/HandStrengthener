using System.Collections.Generic;

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
        return new bool[] { ProcessHighestRecordedValueTrigger(data), ProcessThresholdValueFrequencyTrigger(data) };
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
