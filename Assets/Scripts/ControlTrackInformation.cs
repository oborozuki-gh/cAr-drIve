using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-50)]
public class ControlTrackInformation : MonoBehaviour
{
    public int GetTrackDirNum = 2;
    TrackInfomation[] tracks;
    public CarInformation carInformation;
    public int trackNum;

    void Start()
    {
        tracks = GetComponentsInChildren<TrackInfomation>();
        trackNum = tracks.Length;
        carInformation.roadArea = trackNum;
        foreach (TrackInfomation track in tracks)
        {
            track.Initialize(GetTrackDirNum);
        }

        int count = 0;
        for (int i = 0; i < GetTrackDirNum; i++)
        {
            count++;
            foreach (TrackInfomation track in tracks)
            {
                track.GetNextTrackInfomation(count);
            }
        }
        foreach (TrackInfomation track in tracks)
        {
            track.tracksDir.RemoveAt(0);
        }
    }
}