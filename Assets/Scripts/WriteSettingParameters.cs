using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.MLAgents;

public class WriteSettingParameters : MonoBehaviour
{
    private StreamWriter streamWriter;
    public CarAgent carAgent;
    public CarInformation carInformation;

    void Start()
    {
        try {
            streamWriter = new StreamWriter("settingParameters.txt");
            streamWriter.WriteLine("COURSE");
            streamWriter.WriteLine("course : " + this.name);
            streamWriter.WriteLine("");

            streamWriter.WriteLine("CAR PARAMETER");
            streamWriter.WriteLine("speed : " + carAgent.speed.ToString());
            streamWriter.WriteLine("minSpeed : " + carAgent.minSpeed.ToString());
            streamWriter.WriteLine("maxSpeed : " + carAgent.maxSpeed.ToString());
            streamWriter.WriteLine("torque : " + carAgent.torque.ToString());
            streamWriter.WriteLine("id : " + carAgent.id.ToString());
            streamWriter.WriteLine("noise : " + carAgent.noise.ToString());
            streamWriter.WriteLine("rayDistance : " + carAgent.rayDistance.ToString());
            streamWriter.WriteLine("communicateDistance : " + carAgent.communicateDistance.ToString());
            streamWriter.WriteLine("communicationCarsNum : " + carAgent.communicationCarsNum.ToString());
            streamWriter.WriteLine("delayTime : " + carAgent.delayTime.ToString());
            streamWriter.WriteLine("");

            streamWriter.WriteLine("REWARD");
            streamWriter.WriteLine("movingForwardTileReward : " + carAgent.movingForwardTileReward.ToString());
            streamWriter.WriteLine("crashReward : " + carAgent.crashReward.ToString());
            streamWriter.WriteLine("movingPreviousTileReward : " + carAgent.movingPreviousTileReward.ToString());
            streamWriter.WriteLine("movingBackwardTileReward : " + carAgent.movingBackwardTileReward.ToString());
            streamWriter.WriteLine("stayingSameTileReward : " + carAgent.stayingSameTileReward.ToString());
            streamWriter.WriteLine("distancePenalty : " + carAgent.distancePenalty.ToString());
            streamWriter.WriteLine("");

            streamWriter.WriteLine("GENERATE CAR");
            streamWriter.WriteLine("generateNew : " + carAgent.generateNew.ToString());
            streamWriter.WriteLine("time : " + carAgent.time.ToString());
            streamWriter.WriteLine("generateInterval : " + carAgent.generateInterval.ToString());
            streamWriter.WriteLine("");

            streamWriter.WriteLine("ENVIRONMENT PARAMETER");
            streamWriter.WriteLine("roadOccupancyRate : " + carAgent.roadOccupancyRate.ToString());
            streamWriter.WriteLine("limitCarNum : " + carAgent.limitCarNum.ToString());
            streamWriter.WriteLine("");

            streamWriter.WriteLine("SWITCH");
            streamWriter.WriteLine("resetOnCollision : " + carAgent.resetOnCollision.ToString());
            streamWriter.WriteLine("changeColor : " + carAgent.changeColor.ToString());
            streamWriter.WriteLine("changeSpeed : " + carAgent.changeSpeed.ToString());
            streamWriter.WriteLine("writeFullData : " + carAgent.writeFullData.ToString());
            streamWriter.WriteLine("");

            streamWriter.WriteLine("currentCarNum : " + carInformation.currentCarNum.ToString());

            streamWriter.Close();
        }
        catch {}
    }
}