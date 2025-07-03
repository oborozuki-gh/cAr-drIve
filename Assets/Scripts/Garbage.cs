using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garbage : MonoBehaviour
{
    public bool exstenceCar = false;

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("car")) {
            exstenceCar = true;
        }
    }

    void OnCollisionExit(Collision collision) {
        if (collision.gameObject.CompareTag("car")) {
            exstenceCar = false;
        }
    }
}
