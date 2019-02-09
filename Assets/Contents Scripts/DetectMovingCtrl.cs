using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SPL.Common;

public class DetectMovingCtrl : MonoBehaviour
{
    public GameObject OverLight; //13
    public GameObject StandLight; //12
    public SPLEngine SPLEng;
    public List<Transform> Sensors;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(FlickerItem());
    }

    // Update is called once per frame
    void Update()
    {
        if (SPLEng._GlobalVariables.Count > 0)
        {
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
                OverLight.SetActive(true);
            }
            else
            {
                OverLight.SetActive(false);
            }
        }
    }

    IEnumerator FlickerItem()
    {
        while (true)
        {
            foreach (Transform obj in Sensors)
            {
                obj.GetComponent<MeshRenderer>().material.color = Color.red;
            }
            yield return new WaitForSeconds(0.5f);

            foreach (Transform obj in Sensors)
            {
                obj.GetComponent<MeshRenderer>().material.color = Color.white;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}
