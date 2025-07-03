using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logger
{
    private CarInformation carInformation;
    private Evaluator evaluator = Evaluator.getInstance();

    public Logger(CarInformation carInformation)
    {
        this.carInformation = carInformation;
    }

    public void PrintLog()
    {
        Debug.Log("throughput : " + carInformation.throughCarNum);
        Debug.Log("crash to car : " + (carInformation.crash2car));
        Debug.Log("crash to wall : " + (carInformation.crash2wall));
        Debug.Log("-------------------------------");
    }

    public void WriteLog(int couter)
    {
        evaluator.addTestData(couter, carInformation.throughCarNum, carInformation.crash2wall, carInformation.crash2car);
    }

    public void WriteFullData()
    {
        evaluator.WriteFullData();
    }

    public void ResetEvaluator() {
        evaluator.Init();
    }
}
