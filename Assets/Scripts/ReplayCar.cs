using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ReplayCar : MonoBehaviour
{
    [SerializeField]
    int id;
    [SerializeField]
    string folderPath;
    private List<float[]> csvData = new List<float[]>();
    [SerializeField]
    int step = 0;
    [SerializeField]
    int endStepTime = 100;
    [SerializeField]
    bool crashCar;
    [SerializeField]
    ColorController frame;

    [SerializeField]
    ReplaySetting setting;

    float[] rayDirections = //ray
    {
        25f,     // right forward
        0f,      // forward
        -25f,    // left forward
        155f,    // right backward
        180f,    // backward
        -155f,   // left backward
        90f,     // right side
        -90f     // left side
    };

    // Start is called before the first frame update
    void Start()
    {
        LoadCSV();
        SettingParameter();
        if (endStepTime == -1) endStepTime = csvData.Count-1;
        if (crashCar) frame.CrashCarColor();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (step < endStepTime + 1) {
            transform.localPosition = new Vector3(csvData[step][2], csvData[step][3], csvData[step][4]);
            transform.localEulerAngles = new Vector3(csvData[step][5], csvData[step][6], csvData[step][7]);
        }

        Vector3 position = transform.localPosition;
        foreach (var angle in rayDirections)
        {
            var eulerAngle = Quaternion.Euler(0, angle, 0f);
            var dir = eulerAngle * transform.forward;
            Ray ray = new Ray(position, dir);
            Debug.DrawRay(ray.origin, ray.direction * 5, Color.red);
        }
        step++;
        if (step == endStepTime+120) {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    void LoadCSV()
    {
        string filePath = Path.Combine(folderPath, $"output_{id}.csv");

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            for (int lineIndex = 1; lineIndex < lines.Length; lineIndex++)
            {
                string[] strValues = lines[lineIndex].Split(',');
                float[] floatValues = Array.ConvertAll(strValues, float.Parse);
                csvData.Add(floatValues);
            }
        }
        else
        {
            Debug.LogWarning($"CSV file not found: {filePath}");
        }
    }

    void SettingParameter()
    {
        step = setting.endStep - 500;
        endStepTime = setting.endStep;
        crashCar = setting.crashCarsId.Contains(id);
        folderPath = setting.folderPath;
    }
}
