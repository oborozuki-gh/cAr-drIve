using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Communication
{
    private CarAgent carAgent;
    Color myColor;
    public Communication(CarAgent carAgent)
    {
        this.carAgent = carAgent;
        this.myColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1.0f);
    }

    public void CommunicationCars(ref List<float> observations)
    {
        Vector3 centerPosition = carAgent.transform.localPosition+Vector3.up;
        Collider[] hits = Physics.OverlapSphere(centerPosition, carAgent.communicateDistance);

        int maxCars = carAgent.communicationCarsNum;

        float[,] carData = new float[maxCars, 6];

        for (int i = 0; i < maxCars; i++)
        {
            carData[i, 0] = carAgent.communicateDistance+1;
        }


        foreach (Collider hit in hits)
        {
            if (hit.tag != "car") continue;
            CarAgent otherAgent = hit.gameObject.GetComponent(typeof(CarAgent)) as CarAgent;
            if (otherAgent.id == carAgent.id) continue;

            float distance = Vector3.Distance(carAgent.transform.position, otherAgent.transform.position);
            Vector3 relativePosition = otherAgent.transform.localPosition - carAgent.transform.localPosition;
            float theta = Mathf.Atan2(relativePosition.z, relativePosition.x) * Mathf.Rad2Deg;

            var selfDir = Quaternion.Euler(0, carAgent.torque * carAgent.previousHorizontal * 90f, 0) * (carAgent.transform.forward * carAgent.previousVertical * carAgent.speed);
            var agentDir = Quaternion.Euler(0, otherAgent.torque * otherAgent.previousHorizontal * 90f, 0) * (otherAgent.transform.forward * otherAgent.previousVertical * otherAgent.speed);
            Vector3 relativeSpeed = agentDir - selfDir;

            for (int i = 0; i < maxCars; i++)
            {
                if (distance < carData[i, 0])
                {
                    for (int j = maxCars - 1; j > i; j--)
                    {
                        for (int k = 0; k < 6; k++)
                        {
                            carData[j, k] = carData[j - 1, k];
                        }
                    }

                    carData[i, 0] = distance;
                    carData[i, 1] = theta;
                    carData[i, 2] = relativeSpeed.x;
                    carData[i, 3] = relativeSpeed.z;
                    carData[i, 4] = otherAgent.speed;
                    carData[i, 5] = otherAgent.torque;
                    break;
                }
            }
        }

        for (int i = 0; i < maxCars; i++)
        {
            if (carData[i, 0] == carAgent.communicateDistance+1)
            {
                observations.Add(carAgent.communicateDistance+1);
                observations.Add(0);
                observations.Add(-carAgent.communicateDistance);
                observations.Add(-carAgent.communicateDistance);
                observations.Add(0);
                observations.Add(0);
            }
            else
            {
                Debug.DrawLine(centerPosition, centerPosition - new Vector3(carData[i, 2], 0, carData[i, 3]), myColor, 0.1f);
                for (int j = 0; j < 6; j++)
                {
                    observations.Add(carData[i, j]);
                }
            }
        }
    }
}