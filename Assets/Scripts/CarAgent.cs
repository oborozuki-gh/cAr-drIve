using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

[DefaultExecutionOrder(100)]
public class CarAgent : Agent
{
    private Initialization initialization;
    [HideInInspector]
    public Movement movement;
    [HideInInspector]
    public RewardCalculation rewardCalculation;
    [HideInInspector]
    public TrackRecognition trackRecognition;
    [HideInInspector]
    public Communication communication;
    private Crash crash;
    private AddObservations addObservations;
    private Action action;

    [Header("CAR PARAMETER")]
    public float speed = 10f;
    public float minSpeed = 5;
    public float maxSpeed = 15;
    public float torque = 1f;
    public float previousVertical = 0f;
    public float previousHorizontal = 0f;
    public int id = 0;
    public float noise = 0.1f;
    public float rayDistance = 5f;
    public float communicateDistance;
    public int communicationCarsNum;
    public float delayTime = 600f;
    [Space(2)]
    [Header("REWARD")]
    public int movingForwardTileReward = 1;
    public float crashReward = -10f;
    public float movingPreviousTileReward = -1f;
    public float movingBackwardTileReward = -1f;
    public float stayingSameTileReward = -0.01f;
    public float distancePenalty = -0.1f;

    [Space(2)]
    [Header("GENERATE CAR")]
    public bool generateNew = true;
    public int time = 0;
    public int generateInterval = 300;
    [Space(2)]
    [Header("ENVIRONMENT PARAMETER")]
    public float roadOccupancyRate = 1;
    public int limitCarNum = 300;
    [Space(2)]
    [Header("SWITCH")]
    public bool resetOnCollision = true;
    public bool changeColor = true;
    public bool changeSpeed = true;
    public bool writeFullData = true;
    [Space(2)]
    [Header("GAME OBJECT")]
    public GameObject frame;
    public CarInformation carInformation;

    private Evaluator evaluator = Evaluator.getInstance();
    [SerializeField]
    private Garbage garbage;

    [HideInInspector]
    public List<float> previousObservations;
    [HideInInspector]
    public int new_id = 0;
    [HideInInspector]
    public Transform currentTrack, previousTrack;
    [HideInInspector]
    public Vector3 _initPosition;
    [HideInInspector]
    public Quaternion _initRotation;
    [HideInInspector]
    public bool foundCarForward, foundCarBackward, foundCarSide;
    [HideInInspector]
    public bool movingPreviousTile, movingForwardTile, movingBackwardTile, stayingSameTile;
    [HideInInspector]
    public int startCarNum;
    [HideInInspector]
    public List<int> tracksDir;
    [HideInInspector]
    public static List<float[]> fullData = new List<float[]>();
    [HideInInspector]
    public int dataIndexNum;
    [HideInInspector]
    public static int dataNum;
    [HideInInspector]
    public bool isCrash;
    private int step = 0;
    [HideInInspector]
    public float dt = 0.02f;
    [HideInInspector]
    public int survivalTime = 0;
    [HideInInspector]
    public int sameTileTime = 0;

    public override void Initialize()
    {
        initialization = new Initialization(this);
        movement = new Movement(this);
        trackRecognition = new TrackRecognition(this);
        rewardCalculation = new RewardCalculation(this);
        crash = gameObject.AddComponent(typeof(Crash)) as Crash;
        crash.Initialize(this);
        addObservations = new AddObservations(this);
        action = new Action(this);
        communication = new Communication(this);
        initialization.Initialize();
        startCarNum = carInformation.startCarNum;
    }

    void Update()
    {
        if (!generateNew || id > startCarNum - 1) return;
        if (Time.realtimeSinceStartup < delayTime) return;
        if (garbage != null && garbage.exstenceCar) return;
        if (time > generateInterval && carInformation.currentCarNum < limitCarNum)
        {
            var gameObject = Instantiate(this, _initPosition, _initRotation);
            new_id += startCarNum;
            gameObject.id = new_id;
            gameObject.transform.parent = this.transform.parent.gameObject.transform;
            gameObject.transform.localPosition = _initPosition;
            gameObject.transform.localRotation = _initRotation;
            gameObject.speed = Random.Range(minSpeed, maxSpeed);
            gameObject.frame.GetComponent<ColorController>().ChangeColor(gameObject.speed, maxSpeed, minSpeed);
            carInformation.currentCarNum++;
            time = 0;
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        step++;
        action.ActionProcess(actionBuffers);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    public override void CollectObservations(VectorSensor vectorSensor)
    {
        List<float> observations = addObservations.MakeObservationsList();
        foreach (var v in observations)
        {
            vectorSensor.AddObservation(v);
        }
        previousObservations = observations;
    }

    public override void OnEpisodeBegin()
    {
        if (resetOnCollision)
        {
            transform.localPosition = _initPosition;
            transform.localRotation = _initRotation;
            this.speed = Random.Range(minSpeed, maxSpeed);
            limitCarNum = (int)(carInformation.roadArea * 6 * roadOccupancyRate);
            isCrash = false;
            if (changeColor)
            {
                frame.GetComponent<ColorController>().ChangeColor(speed, maxSpeed, minSpeed);
            }
            survivalTime = 0;
            sameTileTime = 0;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        crash.CrashProcess(other);
    }

    public void AddFullData()
    {
        if (!writeFullData) return;
        if (!isCrash) dataIndexNum = -1;
        float[] tempData = new float[100];
        tempData[0] = step;
        tempData[1] = id;
        tempData[2] = transform.localPosition.x;
        tempData[3] = transform.localPosition.y;
        tempData[4] = transform.localPosition.z;
        tempData[5] = transform.localEulerAngles.x;
        tempData[6] = transform.localEulerAngles.y;
        tempData[7] = transform.localEulerAngles.z;
        tempData[8] = speed;
        tempData[9] = previousHorizontal;
        tempData[10] = previousVertical;
        int ind = 11;
        foreach (float observation in previousObservations)
        {
            tempData[ind] = observation;
            ind++;
        }
        foreach (float reward in rewardCalculation.rewardRecorder.getFullReward())
        {
            tempData[ind] = reward;
            ind++;
        }
        evaluator.fullDataNum = ind;
        dataIndexNum = evaluator.AddFullData(tempData);
    }
}