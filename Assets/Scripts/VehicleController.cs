using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    SteeringWheelController steeringWheelController;

    Rigidbody rb;

    [Header("Speed Vars")]
    public float kmh;
    public float currentMaxSpeed;

    [Header("Motor Vars")]
    public float rpm;
    public float rpmAccelerationMultiplier;
    public float rpmDecelerateMultiplier = 1000;
    public float maxRpm = 7000;
    public float minimumRpm = 200;
    public float maxRpmAccelerationMultiplier = 163;
    public float oneToOneRpmAccelerationMultiplier;
    public float idleRpm = 750;

    public bool engineOn = true;
    public float throttleValue;
    public float throttleDeadzone = 0.1f;

    [Header("Clutch Vars")]
    public float clutchValue; // 0.0f to 1.0f
    public float clutchDeadzone = 0.1f; // The minimum value that the clutchValue can be without killing the car

    public float pullPositionValue; // 0.0f to 1.0f

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
    public float brakeValue;
    public float brakeDeadzone = 0.1f;
    public bool handbrakeOn = false;

    [Header("Wheels Vars")]
    public float maxAngle = 30;
    private GameObject wheelColsParent;
    public WheelCollider[] wheelCols;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        steeringWheelController = GetComponentInParent<SteeringWheelController>();

        wheelColsParent = GameObject.Find("WheelCols");
        wheelCols = wheelColsParent.GetComponentsInChildren<WheelCollider>();

        oneToOneRpmAccelerationMultiplier = maxRpmAccelerationMultiplier / gearRatio[1];
    }

    void Update()
    {
        kmh = rb.velocity.magnitude / 1000 * 2 * 60 * 60;
        
        // Check Inputs
        float angle = maxAngle * Input.GetAxis("Horizontal"); // Check steering value from steering wheel and keyboard

        float throttle = Input.GetAxis("Throttle") + 0.5f; // Check throttle value from steering wheel and keyboard
        if (Input.GetKey(KeyCode.W))
        {
            throttleValue = 1;
        } else
        {
            throttleValue = throttle;
        }

        float clutch = Input.GetAxis("Clutch") + 0.5f; // Check clutch value from steering wheel and keyboard
        if (Input.GetKey(KeyCode.C))
        {
            clutchValue = 1;
        } else
        {
            clutchValue = clutch;
        }

        float brake = Input.GetAxis("Brake") + 0.5f; // Check brake value from steering wheel and keyboard
        if (Input.GetKey(KeyCode.S))
        {
            brakeValue = 1;
        } else
        {
            brakeValue = brake;
        }


        if (Input.GetKey(KeyCode.I) && engineOn == false && whichGear == 0)
        {
            engineOn = true;
            rpm = idleRpm + 100;
        }
        else if (Input.GetKey(KeyCode.I) && engineOn == false && whichGear != 0)
        {
            Debug.Log("Try starting the engine with neutral gear!");
        }

        if (throttleValue > throttleDeadzone && engineOn == true)
        {
            if (rpm < maxRpm)
            {
                switch (whichGear)
                {
                    case -1: // If reverse
                        rpm = rpm + rpmAccelerationMultiplier * gearRatio[0] * throttleValue * 10 * Time.deltaTime;
                        break;
                    case 0: // If neutral
                        rpm = rpm + maxRpmAccelerationMultiplier * throttleValue * 10 * Time.deltaTime;
                        break;
                    default: // If gear is from 1 to highest gear
                        rpm = rpm + rpmAccelerationMultiplier * gearRatio[whichGear] * throttleValue * 10 * Time.deltaTime;
                        break;
                }
            }
        }
        else if (engineOn == true)
        {
            if (rpm > idleRpm)
            {
                rpm = rpm - rpmDecelerateMultiplier * Time.deltaTime;
            }
        }

        checkClutchValues();

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
                if (brakeValue > brakeDeadzone && handbrakeOn == true) // If footBrake and handbrake is activated, then add up their torques
                {
                    wheelCol.brakeTorque = brakeTorque + handbrakeTorque;
                }
                else if (brakeValue > brakeDeadzone)
                {
                    wheelCol.brakeTorque = brakeTorque;
                }
                else if (handbrakeOn == true)
                {
                    wheelCol.brakeTorque = handbrakeTorque;
                }
                else if (throttleValue < throttleDeadzone)
                {
                    wheelCol.brakeTorque = motorBrakeTorque;
                }
                else
                {
                    wheelCol.brakeTorque = 0;
                }

                if (kmh < currentMaxSpeed && clutchValue < clutchDeadzone)
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

    public void checkClutchValues()
    {
        if (whichGear != 0 && engineOn == true) // If the engine is on and the current gear is not neutral
        {
            if (clutchValue < clutchDeadzone && rpm < idleRpm) // If the clutchValue is less than minimumClutchValue and the rpm is to low...
            {
                rpm = rpm - rpmDecelerateMultiplier * Time.deltaTime;    // then decrease the rpm even more...
            }
            else if (clutchValue >= clutchDeadzone && rpm < idleRpm && rpm > minimumRpm)
            {
                rpm = idleRpm;
            }

            if (rpm < minimumRpm)
            {
                engineOn = false;
                rpm = 0;
            }
        }
    }
}
