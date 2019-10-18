using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringWheelController : MonoBehaviour
{
    public float throttle, brake, clutch;

    void Update()
    {
        throttle = Input.GetAxis("Throttle");
        brake = Input.GetAxis("Brake");
        clutch = Input.GetAxis("Clutch");        
    }
}
