using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents.Actuators;
using System.IO;
class Evaluator{
    public int ThroughCars{
        get;
        private set;
    }
    private static Evaluator instance = null;
    private StreamWriter logFullData, logTest;
    public List<float[]> fullData = new List<float[]>();
    public int fullDataNum;

    private Evaluator(){
        ThroughCars = 0;
        Time = 0;
        try {
            Init();
        }

        catch {}
    }

    public void Init() {
        //Must be changed for each environment.
        logFullData = new StreamWriter("logFullData.csv");
        logFullData.Write("step,id,pX,pY,pZ,rX,rY,rZ,speed,horizontal,verticle,");
        for(int i = 0; i < 44; i++){
            logFullData.Write("x" + i.ToString() + ",");
        }
        logFullData.WriteLine("movingForwardTile,movingBackwardTile,movingPreviousTile,stayingSameTile,angle,crash");
        logFullData.Close();
        fullData = new List<float[]>();
        logTest = new StreamWriter("logTest.csv");
        logTest.WriteLine("number,throghtput,crash2wall,crash2car");
        logTest.Close();
    }

    public static Evaluator getInstance(){
        if(instance == null){ instance = new Evaluator(); }
        return instance;
    }

    public double Time{
        get;
        private set;
    }

    public int AddFullData(float[] data, int ind=-1) {
        if (ind == -1) fullData.Add(data);
        else fullData[ind] = data;
        return fullData.Count;
    }

    public void WriteFullData(){
        logFullData = new StreamWriter("logFullData.csv", true);
        foreach (float[] data in fullData) {
            for (int i=0; i<fullDataNum-1; i++) {
                logFullData.Write(data[i].ToString() + ",");
            }
            logFullData.WriteLine(data[fullDataNum-1]);
        }
        logFullData.Close();
    }

    public void addTestData(int testNum, int throughput, int crash2wall, int crash2car)
    {
        logTest = new StreamWriter("logTest.csv", true);
        logTest.WriteLine(testNum.ToString() + "," + throughput.ToString() + "," + crash2wall.ToString() + "," + crash2car.ToString());
        logTest.Close();
    }
}