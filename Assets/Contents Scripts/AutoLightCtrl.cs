using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SPL.Common;

public class AutoLightCtrl : MonoBehaviour
{
    public List<Light> MainLights;    
    public SPLEngine SPLEng;
    public Light CoreLight;
    public List<float> LightValue = new List<float>();
    public Light P11_OverHeadLight; //천장
    public GameObject D12_StandLight; //스탠드

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < MainLights.Count; i++)
        {
            LightValue.Add(CoreLight.intensity / MainLights[i].intensity);
        }
    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < MainLights.Count; i++)
        {
            MainLights[i].intensity = CoreLight.intensity / LightValue[i];
        }

        if (SPLEng._GlobalVariables.Count > 0)
        {
            int d11 = Util.ToInt(SPLEng._GlobalVariables["_DIGITAL_11"]);
            int p11 = Util.ToInt(SPLEng._GlobalVariables["_PWM_11"]);

            if (d11 == 1 && p11 > 0)
            {

            }
            else if (d11 == 1) // 디지털
            {
                P11_OverHeadLight.gameObject.SetActive(true);
                P11_OverHeadLight.intensity = 2.55f;
            }
            else if (p11 > 0)//아날로그
            {
                P11_OverHeadLight.gameObject.SetActive(true);
                P11_OverHeadLight.intensity = p11 / 100f;
            }
            else
            {
                P11_OverHeadLight.gameObject.SetActive(false);
                P11_OverHeadLight.intensity = 0;
            }

          


            int d12 = Util.ToInt(SPLEng._GlobalVariables["_DIGITAL_12"]);
            if (d12 == 1)
            {
                D12_StandLight.SetActive(true);
            }
            else
            {
                D12_StandLight.SetActive(false);
            }
        }
    }
}
