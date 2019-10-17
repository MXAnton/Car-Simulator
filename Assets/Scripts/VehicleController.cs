using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    

    Rigidbody rb;

    [Header("Speed Vars")]
    public float kmh;
    public float currentMaxSpeed;

    [Header("Motor Vars")]
    public float rpm;
    public float rpmAccelerationMultiplier;
    public float rpmDecelerateMultiplier = 1000;
    public float maxRpm = 7000;
    public float maxRpmAccelerationMultiplier = 163;
    public float oneToOneRpmAccelerationMultiplier;
    public float idleRpm = 750;

    public bool engineOn = true;
    public bool isThrottling = false;

    [Header("Gear Vars")]
    public int amountOfGears; // With reverse || 0 = reverse | 1 = gear 1 | 2 = gear 2...
    public float[] gearRatio; // With reverse || 0 = reverse | 1 = gear 1 | 2 = gear 2...
    public float oneToOneRatioMaxSpeed = 180;

    public int whichGear; //  -1 = reverse | 0 = natural | 1 = gear 1...

    [Header("Brake Vars")]
    public float motorBrakeTorque;
    public float minimumMotorBrakeTorque = 1000;

    public float brakeTorque = 8000;
    public float handbrakeTorque = 16_000;
    public bool isBraking = false;
    public bool handbrakeOn = false;

    [Header("Wheels Vars")]
    public float maxAngle = 30;
    private GameObject wheelColsParent;
    public WheelCollider[] wheelCols;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        wheelColsParent = GameObject.Find("WheelCols");
        wheelCols = wheelColsParent.GetComponentsInChildren<WheelCollider>();

        oneToOneRpmAccelerationMultiplier = maxRpmAccelerationMultiplier / gearRatio[1];
    }

    void Update()
    {
        kmh = rb.velocity.magnitude / 1000 * 2 * 60 * 60;
        
        float angle = maxAngle * Input.GetAxis("Horizontal");

        if (Input.GetKey(KeyCode.W) && engineOn == true)
        {
            isThrottling = true;
            if (rpm < maxRpm)
            {
                switch (whichGear)
                {
                    case -1: // If reverse
                        rpm = rpm + rpmAccelerationMultiplier * gearRatio[0] * Time.deltaTime;
                        break;
                    case 0: // If neutral
                        rpm = rpm + maxRpmAccelerationMultiplier * Time.deltaTime;
                        break;
                    default: // If gear is from 1 to highest gear
                        rpm = rpm + rpmAccelerationMultiplier * gearRatio[whichGear] * Time.deltaTime;
                        break;
                }
            }
        }
        else if (engineOn == true)
        {
            isThrottling = false;
            if (rpm > idleRpm)
            {
                rpm = rpm - rpmDecelerateMultiplier * Time.deltaTime;
            }
            else
            {
                rpm = idleRpm;
            }
        }
        else
        {
            isThrottling = false;
        }

        if (Input.GetKey(KeyCode.S))
        {
            isBraking = true;
        } else
        {
            isBraking = false;
        } // Check if player wants to brake. If so, then brake...

        if (Input.GetKeyDown(KeyCode.G) && whichGear < amountOfGears)
        {
            whichGear += 1;
            applyNewGear();
        } // Gear up
        else if (Input.GetKeyDown(KeyCode.B) && whichGear > -1)
        {
            whichGear -= 1;
            applyNewGear();
        } // Gear down

        foreach (WheelCollider wheelCol in wheelCols)
        {
            // A simple car where front wheels steer while rear ones drive.
            if (wheelCol.transform.localPosition.z > 0)
            {
                wheelCol.steerAngle = angle;
            }
            // Checks if backwheel. If then put handBrake value on wheel.brakeTorque and wheel.motorTorque to torque
            if (wheelCol.transform.localPosition.z < 0)
            {
                if (isBraking == true && handbrakeOn == true) // If footBrake and handbrake is activated, then add up their torques
                {
                    wheelCol.brakeTorque = brakeTorque + handbrakeTorque;
                }
                else if (isBraking == true)
                {
                    wheelCol.brakeTorque = brakeTorque;
                }
                else if (handbrakeOn == true)
                {
                    wheelCol.brakeTorque = handbrakeTorque;
                }
                else if (isThrottling == false)
                {
                    wheelCol.brakeTorque = motorBrakeTorque;
                }
                else
                {
                    wheelCol.brakeTorque = 0;
                }

                if (kmh < currentMaxSpeed)
                {
                    if (whichGear == -1)
                    {
                        wheelCol.motorTorque = -rpm;
                    }
                    else
                    {
                        wheelCol.motorTorque = rpm;
                    }
                }
                else
                {
                    wheelCol.motorTorque = 0;
                }
            }
        }
    }

    public void applyNewGear()
    {
        switch (whichGear)
        {
            case -1: // If reverse
                rpmAccelerationMultiplier = oneToOneRpmAccelerationMultiplier * gearRatio[0]; // Set how fast the car will accelerate depending on which gear
                currentMaxSpeed = oneToOneRatioMaxSpeed / gearRatio[0];
                motorBrakeTorque = minimumMotorBrakeTorque * gearRatio[0];
                break;
            case 0: // If neutral
                rpmAccelerationMultiplier = maxRpmAccelerationMultiplier; // Set how fast the car will accelerate depending on which gear
                currentMaxSpeed = 0;
                motorBrakeTorque = 0;
                break;
            default: // If gear is from 1 to highest gear
                rpmAccelerationMultiplier = oneToOneRpmAccelerationMultiplier * gearRatio[whichGear]; // Set how fast the car will accelerate depending on which gear
                currentMaxSpeed = oneToOneRatioMaxSpeed / gearRatio[whichGear];
                motorBrakeTorque = minimumMotorBrakeTorque * gearRatio[whichGear];
                break;
        }
    }
}
