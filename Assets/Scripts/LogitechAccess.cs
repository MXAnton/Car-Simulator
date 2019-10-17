using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogitechAccess : MonoBehaviour
{
    //LogitechGSDK.LogiControllerPropertiesData logitechProperties;

    //public float xAxes, gasInput, breakInput, clutchInput;

    //public int currentGear;

    //void Start()
    //{
    //    print(LogitechGSDK.LogiSteeringInitialize(false));
    //}

    //void Update()
    //{
    //    if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
    //    {
    //        LogitechGSDK.DIJOYSTATE2ENGINES rec;
    //        rec = LogitechGSDK.LogiGetStateUnity(0);

    //        xAxes = rec.lX / 32768f; // -1 0 1

    //        if (rec.lY > 0)
    //        {
    //            gasInput = 0;
    //        } else if (rec.lY < 2)
    //        {
    //            gasInput = rec.lY / -32768f;
    //        }
    //    }
    //}
}
