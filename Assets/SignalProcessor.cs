using System.Collections.Generic;

public struct BCIOutput
{

    bool[] results;

    public BCIOutput(params bool[] results)
    {
        this.results = results;
    }

}

public static class SignalProcessor
{

    // ...

    public static BCIOutput Process(List<float> data)
    {
        // ...

        return new BCIOutput(process1(data) /* add more comma-separated functions here... */ );
    }

    private static bool process1(List<float> data)
    {
        // ex amount of peaks...

        return true;
    }

}
