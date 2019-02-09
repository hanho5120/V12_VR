using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SPL.Common;

public class ButtonLightCtrl : MonoBehaviour
{
    public SPLEngine SPLEng;
    public GameObject StandLight;
    public GameObject Fan;
    public List<GameObject> MainLight;
    public bool FanOnoff = false;
    public float MaxFanSpeed = 0.0f;
    public float FanSpeed = 0.0f;

    // Use this for initialization
    void Start()
    {
        foreach (GameObject obj in MainLight)
        {
            obj.SetActive(false);
        }

        StartCoroutine(RotateFan());
    }

    // Update is called once per frame
    void Update()
    {
        if (SPLEng._GlobalVariables.Count > 0)
        {
            //11번 선풍기
            int d11 = Util.ToInt(SPLEng._GlobalVariables["_DIGITAL_11"]);
            int p11 = Util.ToInt(SPLEng._GlobalVariables["_PWM_11"]);

            if (d11 == 1 && p11 > 0)
            {

            }
            else if (d11 == 1) // 디지털
            {
                MaxFanSpeed = 1020;
                FanOnoff = true;
            }
            else if (p11 > 0)//아날로그
            {
                MaxFanSpeed = p11 * 4;
                FanOnoff = true;
            }
            else
            {
                MaxFanSpeed = 0;
                FanOnoff = false;
            }


            int d12 = Util.ToInt(SPLEng._GlobalVariables["_DIGITAL_12"]);
            if (d12 == 1)
            {
                StandLight.SetActive(true);
            }
            else
            {
                StandLight.SetActive(false);
            }

            int d13 = Util.ToInt(SPLEng._GlobalVariables["_DIGITAL_13"]);
            if (d13 == 1)
            {
                foreach (GameObject obj in MainLight)
                {
                    obj.SetActive(true);
                }
            }
            else
            {
                foreach (GameObject obj in MainLight)
                {
                    obj.SetActive(false);
                }
            }

        }

    }

    IEnumerator RotateFan()
    {
        while(true)
        {
            if (FanOnoff)
            {
                if (FanSpeed < MaxFanSpeed)
                {
                    FanSpeed = FanSpeed + 10f;
                }
                else
                {
                    FanSpeed = MaxFanSpeed;
                }

                Fan.transform.Rotate(new Vector3(0, 0, FanSpeed * Time.deltaTime));

            }
            else
            {
                if (FanSpeed > 0)
                {
                    FanSpeed = FanSpeed - 10f;
                }
                else
                {
                    FanSpeed = 0;
                }
                Fan.transform.Rotate(new Vector3(0, 0, FanSpeed * Time.deltaTime));
            }

            yield return null;
        } 
        
    }
}
