using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneCtrl : MonoBehaviour
{
    bool isNotEnd = true;
    public Rigidbody Rigidbody;
    public MoveDogCtrlVR MoveDogCtrlVR;
    // Use this for initialization
    void Awake()
    {
       
        MoveDogCtrlVR = GameObject.Find("Dog_Parent").GetComponent< MoveDogCtrlVR>();
     //   
    }

    // Update is called once per frame
    void Update()
    {

    }

    void StartCheck()
    {
        StartCoroutine(CheckSpeed());
    }

    IEnumerator CheckSpeed()
    {
        while (isNotEnd)
        {
            float speed = Rigidbody.velocity.magnitude;
            if (speed < 0.01f)
            {
               isNotEnd = false;
                MoveDogCtrlVR.SendMessage("SetGoal", transform.position);

            }
            yield return null;
        }
    }
}
