using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUIController : MonoBehaviour
{
    VehicleController vehicleController;

    public TextMeshProUGUI gearText;

    public GameObject speedometerNeedle;
    public RectTransform rpmGaugeNeedle;

    public float speedometerNeedleRotationOffset = 185;
    public float rpmGaugeNeedleRotationOffset = 80;

    void Start()
    {
        vehicleController = GetComponentInChildren<VehicleController>();
    }

    void Update()
    {
        UpdateGearText();
        UpdateRpmGaugeNeedle();
        

    }

    void UpdateGearText()
    {
        switch (vehicleController.whichGear)
        {
            case -1:
                gearText.text = "R";
                break;
            case 0:
                gearText.text = "N";
                break;
            case 1:
                gearText.text = "1";
                break;
            case 2:
                gearText.text = "2";
                break;
            case 3:
                gearText.text = "3";
                break;
            case 4:
                gearText.text = "4";
                break;
            case 5:
                gearText.text = "5";
                break;
        }
    }

    void UpdateRpmGaugeNeedle()
    {
        float mappedRpm = vehicleController.rpm;
        float x = 140 / vehicleController.maxRpm; // Map maxRpm to 140 degrees
        mappedRpm *= x;

        rpmGaugeNeedle.transform.localEulerAngles = new Vector3(0, 0, -mappedRpm + rpmGaugeNeedleRotationOffset);
    }
}

