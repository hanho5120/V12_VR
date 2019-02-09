using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SPL.Common;

public class RearSensingCtrl : MonoBehaviour
{
    public SPLEngine SPLEng;
    public RobotControl RobotCtrl;

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


}





