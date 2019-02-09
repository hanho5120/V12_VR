using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SPL.Common;

public class DetectEdgeCtrl : MonoBehaviour
{


    public SPLEngine SPLEng;
    public RobotControl RobotCtrl;
    public List<Transform> OtherCars;
    public List<GameObject> Clouds;

    // Use this for initialization
    void Start()
    {
        //StartCoroutine(StartFall());
       // StartCoroutine(MoveToClouds());
        
    }

    IEnumerator MoveToClouds()
    {
        foreach (GameObject obj in Clouds)
        {
            float RandomMoveValue = Random.Range(0.1f, 1f);
            float RandomtimeValue = Random.Range(3f, 7f);
            iTween.MoveAdd(obj, iTween.Hash("y", -RandomMoveValue, "looptype", iTween.LoopType.pingPong, "time", RandomtimeValue, "easytype", iTween.EaseType.linear));
            yield return new WaitForSeconds(0.1f);
        }
        
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

    IEnumerator StartFall()
    {
        yield return new WaitForSeconds(2f);
        foreach (Transform obj in OtherCars)
        {
            //obj.transform.Translate(new Vector3(0, 0, 10 * Time.deltaTime));
        }
    }
}
