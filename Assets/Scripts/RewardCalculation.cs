using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using System.Linq;

public class RewardCalculation
{
    private CarAgent carAgent;
    private CarInformation carInformation;
    public RewardRecorder rewardRecorder = new RewardRecorder();

    public RewardCalculation(CarAgent carAgent)
    {
        this.carAgent = carAgent;
        this.carInformation = carAgent.carInformation;
    }

    public float CalculateIndividualReward()
    {
        float individualReward = 0.0f;
        carAgent.trackRecognition.TrackRecognize();
        if (carAgent.movingPreviousTile)
        {
            individualReward = carAgent.movingPreviousTileReward;
            rewardRecorder.movingPreviousTileReward = individualReward;
        }
        else if (carAgent.movingForwardTile)
        {
            individualReward = carAgent.movingForwardTileReward;
            rewardRecorder.movingForwardTileReward = individualReward;
        }
        else if (carAgent.movingBackwardTile)
        {
            individualReward = carAgent.movingBackwardTileReward;
            rewardRecorder.movingBackwardTileReward = individualReward;
        }
        else if (carAgent.stayingSameTile)
        {
            individualReward = carAgent.stayingSameTileReward;
            rewardRecorder.stayingSameTileReward = individualReward;
        }
        return individualReward;
    }

    public float CalculateAngleReward(Vector3 moveVec, float angle, float vertical)
    {
        float angleReward = ((1f - angle / 90f) * Mathf.Clamp01(Mathf.Max(0, vertical)) + Mathf.Min(0, vertical)) * Time.fixedDeltaTime;
        rewardRecorder.angleReward = angleReward;
        return angleReward;
    }

    public float CalculateDistanceReward(float distance, float distanceReward, float distanceThreshold)
    {
        if (distance < distanceThreshold)
        {
            return distanceReward;
        }
        return -distanceReward;
    }

    public void setCrashReward(float crashReward)
    {
        rewardRecorder.resetReward();
        rewardRecorder.crashReward = crashReward;
        carAgent.SetReward(crashReward);
    }
}

public class RewardRecorder
{
    public float movingPreviousTileReward = 0;
    public float movingForwardTileReward = 0;
    public float movingBackwardTileReward = 0;
    public float stayingSameTileReward = 0;
    public float commonReward = 0;
    public float angleReward = 0;
    public float crashReward = 0;

    public void resetReward()
    {
        movingPreviousTileReward = 0;
        movingForwardTileReward = 0;
        movingBackwardTileReward = 0;
        stayingSameTileReward = 0;
        commonReward = 0;
        angleReward = 0;
        crashReward = 0;
    }

    public List<float> getFullReward()
    {
        return new List<float>{ movingForwardTileReward,
        movingBackwardTileReward, movingPreviousTileReward, stayingSameTileReward, angleReward, crashReward};
    }

    public float SumReward()
    {
        float sumReward = getFullReward().Sum();
        return sumReward;
    }
}