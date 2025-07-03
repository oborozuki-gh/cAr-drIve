using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddObservations
{
    private CarAgent carAgent;
    private CarInformation carInformation;
    private RewardCalculation rewardCalculation;

    (float, float, float)[] rayDirections = //ray
    {
        (1.5f, .5f, 25f),     // right forward
        (1.5f, 0f, 0f),       // forward
        (1.5f, -.5f, -25f),   // left forward
        (-1.5f, .5f, 155f),   // right backward
        (-1.5f, 0f, 180f),    // backward
        (-1.5f, -.5f, -155f), // left backward
        (0f, .5f, 90f),       // right side
        (0f, -.5f, -90f)      // left side
    };

    public AddObservations(CarAgent carAgent)
    {
        this.carAgent = carAgent;
        this.carInformation = carAgent.carInformation;
        this.rewardCalculation = carAgent.rewardCalculation;
    }

    public List<float> MakeObservationsList()
    {
        List<float> observations = new List<float>();
        float angle = Vector3.SignedAngle(carAgent.currentTrack.forward, carAgent.transform.forward, Vector3.up);
        carAgent.foundCarBackward = false;
        carAgent.foundCarForward = false;
        carAgent.foundCarSide = false;

        observations.Add(angle / 180f);

        string tag;
        float distanceObservedObject;
        Vector3 relativeSpeed;

        for (int i = 0; i < rayDirections.Length; i++)
        {
            distanceObservedObject = ObserveRay(rayDirections[i].Item1, rayDirections[i].Item2, rayDirections[i].Item3, out tag, out relativeSpeed);
            observations.Add(distanceObservedObject);
            observations.Add(tag == "car" ? 1 : 0);
            observations.Add(tag == "wall" ? 1 : 0);
            observations.Add(relativeSpeed.x);
            observations.Add(relativeSpeed.z);

            carAgent.AddReward(carAgent.rewardCalculation.CalculateDistanceReward(distanceObservedObject, carAgent.distancePenalty, carAgent.maxSpeed*carAgent.dt));
        }

        carAgent.communication.CommunicationCars(ref observations);

        observations.Add(carAgent.speed);
        observations.Add(carAgent.torque);

        foreach(int trackDir in carAgent.tracksDir)
        {
            observations.Add(trackDir);
        }

        return observations;
    }

    private float ObserveRay(float z, float x, float angle, out string tag, out Vector3 relativeSpeed)
    {
        tag = "none";
        var tf = carAgent.transform;
        relativeSpeed =Vector3.zero;

        // Get the start position of the ray
        var raySource = tf.position + Vector3.up / 2f;
        var position = raySource + tf.forward * z + tf.right * x;

        // Get the angle of the ray
        var eulerAngle = Quaternion.Euler(0, angle, 0f);
        var dir = eulerAngle * tf.forward;
        RaycastHit hit;

        // laser visualization
        Ray ray = new Ray(position, dir);
        Debug.DrawRay(ray.origin, ray.direction*carAgent.rayDistance, Color.red);

        // See if there is a hit in the given direction
        var rayHit = Physics.Raycast(position, dir, out hit, carAgent.rayDistance);
        if (rayHit)
        {
            tag = hit.collider.tag;
            if (tag == "car") {
                CarAgent otherAgent = hit.collider.gameObject.GetComponent(typeof(CarAgent)) as CarAgent;
                var selfDir = Quaternion.Euler(0, carAgent.torque * carAgent.previousHorizontal * 90f, 0) * (carAgent.transform.forward * carAgent.previousVertical * carAgent.speed);
                var agentDir = Quaternion.Euler(0, otherAgent.torque * otherAgent.previousHorizontal * 90f, 0) * (otherAgent.transform.forward * otherAgent.previousVertical * otherAgent.speed);
                relativeSpeed = agentDir - selfDir;
            }
        }
        return hit.distance > 0 ? Mathf.Clamp(hit.distance / carAgent.rayDistance * Random.Range(1-carAgent.noise, 1+carAgent.noise), 0f, 1f) : 1f;
    }
}