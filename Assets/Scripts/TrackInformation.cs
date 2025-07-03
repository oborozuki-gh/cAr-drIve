using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackInfomation : MonoBehaviour
{
    Vector3 foward;
    public List<int> tracksDir = new List<int>();

    public void Initialize(int GetTrackDirNum)
    {
        foward = this.transform.forward;
        for (int i=0; i < GetTrackDirNum+1; i++) tracksDir.Add(-1);
        if (foward == new Vector3(0f, 0f, 1f))  // rotarion 0
        {
            tracksDir[0] = 0;
        }

        else if (foward == new Vector3(-1f, 0f, 0f)) // -90
        {
            tracksDir[0] = 1;
        }

        else if (foward == new Vector3(0f, 0f, -1f)) // 180
        {
            tracksDir[0] = 2;
        }

        else if (foward == new Vector3(1f, 0f, 0f)) // 90
        {
            tracksDir[0] = 3;
        }
    }

    public void GetNextTrackInfomation(int count)
    {
        //Debug.Log("Test");
        Ray ray = new Ray(this.transform.position + this.transform.forward * 7f + new Vector3(0, 1, 0), Vector3.down);
        RaycastHit hitInfomation;
        var raycast = Physics.Raycast(ray, out hitInfomation, 2f);
        if (raycast)
        {
            //Debug.Log("Hit");
            var nextTrack = hitInfomation.collider.gameObject;
            if (count != 1)
            {
                this.tracksDir[count] = nextTrack.GetComponent<TrackInfomation>().GetNextTrackDir(count);
            }
            else
            {
                this.tracksDir[count] = this.tracksDir[count - 1] - nextTrack.GetComponent<TrackInfomation>().GetNextTrackDir(count);
                if (this.tracksDir[count] == 3 || this.tracksDir[count] == -3)
                {
                    this.tracksDir[count] /= -3;
                }
            }
        }
    }

    int GetNextTrackDir(int count)
    {
        return this.tracksDir[count-1];
    }

    public List<int> GetTracksDir()
    {
        return this.tracksDir;
    }

    public void DebugDir()
    {
        Debug.Log(this.name);
        Debug.Log(string.Join(",", tracksDir));
    }
}