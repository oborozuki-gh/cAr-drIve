using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackRecognition
{
    private CarAgent carAgent;
    private CarInformation carInformation;
    private Evaluator evaluator = Evaluator.getInstance();

    public TrackRecognition(CarAgent carAgent)
    {
        this.carAgent = carAgent;
        this.carInformation = carAgent.carInformation;
    }

    public void TrackRecognize()
    {
        carAgent.movingPreviousTile = false;
        carAgent.movingForwardTile = false;
        carAgent.movingBackwardTile = false;
        carAgent.stayingSameTile = false;

        var carCenter = carAgent.transform.position + Vector3.up;

        if (Physics.Raycast(carCenter, Vector3.down, out var hit, 2f))
        {
            var newHitTile = hit.transform;

            if(carAgent.currentTrack == null)
            {
                carAgent.previousTrack = carAgent.currentTrack;
                carAgent.currentTrack = newHitTile;
                carAgent.tracksDir = hit.collider.gameObject.GetComponent<TrackInfomation>().GetTracksDir();
            }
            // move another tile
            else if (newHitTile != carAgent.currentTrack)
            {
                var relativePosition = carAgent.transform.position - newHitTile.position;
                carAgent.tracksDir = hit.collider.gameObject.GetComponent<TrackInfomation>().GetTracksDir();

                // move previous tile
                if(newHitTile == carAgent.previousTrack)
                {
                    carAgent.movingPreviousTile = true;
                }

                // moving forward
                else
                {
                    float angle = Vector3.Angle(carAgent.currentTrack.forward, newHitTile.position - carAgent.currentTrack.position);
                    if (angle < 90f)
                    {
                        carAgent.movingForwardTile = true;

                        // if the tile's tag id "CheckPoint"
                        if (hit.collider.tag == "CheckPoint")
                        {
                            carInformation.throughCarNum++;
                        }

                        // if the tile's tag id "endTile"
                        if (hit.collider.tag == "endTile")
                        {
                            carAgent.transform.localPosition = new Vector3(carAgent.transform.localPosition.x,0, 0);
                            if (carAgent.changeSpeed)
                            {
                                carAgent.speed = Random.Range(carAgent.minSpeed, carAgent.maxSpeed+1);
                                carAgent.frame.GetComponent<ColorController>().ChangeColor(carAgent.speed, carAgent.maxSpeed, carAgent.minSpeed);
                            }
                        }
                    }

                    // moving backward
                    else
                    {
                        carAgent.movingBackwardTile = true;
                    }
                }
                carAgent.previousTrack = carAgent.currentTrack;
                carAgent.currentTrack = newHitTile;
            }
            else
            {
                carAgent.stayingSameTile = true;
            }
        }
    }
}