using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaySetting : MonoBehaviour
{
    [SerializeField]
    public List<int> crashCarsId;
    public int startStep, endStep;
    public string folderPath;
}
