using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crash : MonoBehaviour
{
    private CarAgent carAgent;
    private Evaluator evaluator = Evaluator.getInstance();
    private CarInformation carInformation;

    // Initialize member variables
    public void Initialize(CarAgent carAgent)
    {
        this.carAgent = carAgent;
        this.carInformation = carAgent.carInformation;
    }

    public void CrashProcess(Collision other)
    {
        if (carAgent.isCrash) return;
        // If car have an accident
        if (other.gameObject.CompareTag("wall") || other.gameObject.CompareTag("car"))
        {
            var carCenter = carAgent.transform.position + Vector3.up;

            // If the collision was a car
            if (other.gameObject.CompareTag("car"))
            {

                var otherAgent = (CarAgent)other.gameObject.GetComponent(typeof(CarAgent));
                if (!(otherAgent.survivalTime == 0 || otherAgent.survivalTime == 1) && !(carAgent.survivalTime == 0 || carAgent.survivalTime == 1))
                {
                    carAgent.isCrash = true;
                    carAgent.rewardCalculation.setCrashReward(carAgent.crashReward);
                    carAgent.AddFullData();
                    carInformation.crash2car++;

                    otherAgent.isCrash = true;
                    otherAgent.rewardCalculation.setCrashReward(carAgent.crashReward);
                    otherAgent.AddFullData();
                    carInformation.crash2car++;
                }
                carAgent.EndEpisode();

                if ((carAgent.id < otherAgent.id) && IsNotErasedId(otherAgent.id))
                {
                    if (Physics.Raycast(carCenter, Vector3.down, out var hit, 2f))
                    {
                        if (carAgent.generateNew)
                        {
                            Destroy(other.gameObject);
                            carInformation.currentCarNum--;
                        }
                    }
                }
            }

            else
            {
                if (!(carAgent.survivalTime == 0 || carAgent.survivalTime == 1))
                {
                    carAgent.isCrash = true;
                    carAgent.rewardCalculation.setCrashReward(carAgent.crashReward);
                    carAgent.AddFullData();
                    carInformation.crash2wall++;
                }

                carAgent.EndEpisode();
            }
        }
    }

    // Prevent both cars from disappearing when cars with id0 and id1 collide
    private bool IsNotErasedId(int otherAgentId)
    {
        List<int> isNotErasedIdList = new List<int>();
        for(int i = 0; i < carAgent.startCarNum; i++)
        {
            isNotErasedIdList.Add(i);
        }
        if (isNotErasedIdList.Contains(carAgent.id) && isNotErasedIdList.Contains(otherAgentId))
        {
            return false;
        }
        return true;
    }
}