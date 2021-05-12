using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class FileManager : MonoBehaviour
{
    static string basePath = "Assets/Resources/";
    static string sourceFile = "bci_recording.txt";

    [MenuItem("Tools/Read file")]
    public static IEnumerator<float> getDataLineByLine(string fileName = null)
    {
        if (fileName == null) fileName = basePath + sourceFile;
        else fileName = basePath + fileName;
        using (StreamReader sr = new StreamReader(fileName))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                yield return float.Parse(line);
            }
        }
        yield break;
    }

    public static List<float> getDataAsArray(string fileName = null)
    {
        List<float> dataArray = new List<float>();
        IEnumerator<float> dataEnumerator = getDataLineByLine(fileName);
        while (dataEnumerator.MoveNext())
        {
            dataArray.Add(dataEnumerator.Current);
        }
        return dataArray;
    }

    [MenuItem("Tools/Read file")]
    public static void saveDataFromArray(List<float> input, string fileName = null)
    {
        if (fileName == null) fileName = basePath + sourceFile;
        else fileName = basePath + fileName;
        Debug.LogWarning("Saving to " + fileName);
        File.WriteAllLines(fileName, input.Select(value => value + ""));
    }

    /*public bool isCorrectSeries(List<float> input)
    {
        List<float> source = getDataAsArray();
        int sourceIndex = -1;
        for (int i = 1; i < input.Count; i++)
        {
            for (int j = 1; j < source.Count; j++)
            {
                if (input[i] == source[j] && input[i - 1] == source[j - 1])
                {
                    sourceIndex = j;
                    break;
                }
            }
            if (sourceIndex == -1) return false;
            if (input[i - 1])
        }
        int sourceIndex = -1;
        for (int i = 0; i < input.Count; i++)
        {
            if (sourceIndex == -1)
            {
                int j = source.IndexOf(input[i]);
                if (j != -1 && source.IndexOf(input[i + 1]) == j + 1) sourceIndex = j;
            }
            else if (input[i] != source[sou])
        }
        if (input.Count < source.Count)
            Debug.LogWarning("Input list had " + (source.Count - input.Count) + " less entries than source!");
        return true;
    }*/
}
