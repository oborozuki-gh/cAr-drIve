﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarInformation : MonoBehaviour
{
    public int currentCarNum = 0;
    public int startCarNum = 0;
    public int throughCarNum = 0;
    public int rewardTime = 0;
    public float commonReward = 0.0f;
    public int receivedCommonRewardCarNum = 0;
    private Logger logger;
    public int crash2car = 0;
    public int crash2wall = 0;
    public float roadArea;

    void Start()
    {
        logger = new Logger(this);
    }
}
