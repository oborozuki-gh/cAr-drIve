using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using Unity.Barracuda;

using System.IO;

[DefaultExecutionOrder(-100)]
public class Test : MonoBehaviour
{
    public CarInformation carInformation;

    [Header("TEST PARAMETER")]
    public int stopTime = 600;
    private float startTime = 0;
    public float timer = 0;
    public int testNum = 10;

    private static int resetCount = 0;
    private Logger logger;
    private CarAgent[] carAgents;

    [SerializeField]
    private NNModel[] models;
    private static int modelIndex = 0;
    public bool writeLog = true;
    void Start()
    {
        carAgents = GetComponentsInChildren<CarAgent>();
        SetModel();
        startTime = Time.realtimeSinceStartup;
        logger = new Logger(carInformation);
    }

    void FixedUpdate()
    {
        timer = Time.realtimeSinceStartup - startTime;
        if ((timer >= stopTime) && (stopTime != 0))
        {
            // logger.PrintLog();
            logger.WriteLog(resetCount+1);
            ResetScene();
        }
    }

    public void ResetScene()
    {
        resetCount++;
        if (resetCount == testNum)
        {
            logger.WriteFullData();
            if (writeLog) MoveModelFile(models[modelIndex].name);
            if (modelIndex < models.Length - 1)
            {
                resetCount = 0;
                modelIndex++;
                logger.ResetEvaluator();
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else Debug.Break();
        }
        else SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void SetModel() {
        foreach (CarAgent carAgent in carAgents) {
            var behaviorParameters = carAgent.GetComponent<BehaviorParameters>();
            behaviorParameters.Model = models[modelIndex];
        }
    }

    void MoveModelFile(string modelName) {
        string logFullData = "logFullData.csv";
        string logTest = "logTest.csv";
        string modelResultsPath = "results/" + modelName + "/";
        Debug.Log("Model results path: " + modelResultsPath);

        if (File.Exists(modelResultsPath + logFullData))
        {
            File.Delete(modelResultsPath + logFullData);
        }
        if (!File.Exists(modelResultsPath))
        {
            File.Move(logFullData, modelResultsPath + logFullData);
            Debug.Log("Full data log moved to: " + modelResultsPath + logFullData);
        }
        if (File.Exists(modelResultsPath+logTest)) {
            File.Delete(modelResultsPath+logTest);
        }
        if (!File.Exists(modelResultsPath))
        {
            File.Move(logTest, modelResultsPath + logTest);
            Debug.Log("Test log moved to: " + modelResultsPath + logTest);
        }
    }
}