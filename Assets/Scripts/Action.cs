using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

public class Action
{
    private CarAgent carAgent;
    private Evaluator evaluator = Evaluator.getInstance();
    private CarInformation carInformation;
    private RewardCalculation rewardCalculation;

    public Action(CarAgent carAgent)
    {
        this.carAgent = carAgent;
        this.carInformation = carAgent.carInformation;
        this.rewardCalculation = carAgent.rewardCalculation;
    }

    public void ActionProcess(ActionBuffers actionBuffers)
    {
        carAgent.survivalTime++;
        carAgent.rewardCalculation.rewardRecorder.resetReward();
        var lastPos = carAgent.transform.position;

        if (carAgent.generateNew)
        {
            carAgent.time++;
        }

        float horizontal = actionBuffers.ContinuousActions[0];
        float vertical = actionBuffers.ContinuousActions[1];
        vertical = Mathf.Clamp(vertical, -1.0f, 1.0f);
        horizontal = Mathf.Clamp(horizontal, -1.0f, 1.0f);

        carAgent.movement.MoveCar(horizontal, vertical, Time.fixedDeltaTime);

        float individualReward = rewardCalculation.CalculateIndividualReward();

        var moveVec = carAgent.transform.position - lastPos;
        float angle = Vector3.Angle(moveVec, carAgent.currentTrack.forward);
        float angleReward = rewardCalculation.CalculateAngleReward(moveVec, angle, vertical);

        carAgent.AddReward(individualReward + angleReward);
        if (carAgent.movingForwardTile) carAgent.sameTileTime = 0;
        else carAgent.sameTileTime++;
        if (carAgent.sameTileTime > 10000) {
            carAgent.rewardCalculation.setCrashReward(carAgent.crashReward);
            carAgent.EndEpisode();
        }
        carAgent.AddFullData();
    }
}