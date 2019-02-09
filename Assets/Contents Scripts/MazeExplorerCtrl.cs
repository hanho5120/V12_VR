﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SPL.Common;


public class MazeExplorerCtrl : MonoBehaviour
{
    public SPLEngine SPLEng;
    public RobotControl RobotCtrl;
    public List<GameObject> Fireworks;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (SPLEng._GlobalVariables.Count > 0)
        {
            int d11 = Util.ToInt(SPLEng._GlobalVariables["_DIGITAL_11"]);
            if (d11 == 1)
            {
                foreach (GameObject obj in RobotCtrl.HeadLight)
                {
                    obj.SetActive(true);
                }
            }
            else
            {
                foreach (GameObject obj in RobotCtrl.HeadLight)
                {
                    obj.SetActive(false);
                }
            }

            int d12 = Util.ToInt(SPLEng._GlobalVariables["_DIGITAL_12"]);
            if (d12 == 1)
            {
                RobotCtrl.BreakLight.SetActive(true);
            }
            else
            {
                RobotCtrl.BreakLight.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root == RobotCtrl.transform)
        {
            foreach (GameObject obj in Fireworks)
            {
                obj.SetActive(true);
            }
            RobotCtrl.enabled = false;
        }
    }
}
