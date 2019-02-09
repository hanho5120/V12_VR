using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SWS;
using DG.Tweening;

using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;

public class TrafficSignal_CarMovement : MonoBehaviour
{
    public bool isCross;
    public splineMove sm;
    public SPLEngine SPLEng;
    public Transform RayStart;
    RaycastHit hit;
    

    // Use this for initialization
    void Start()
    {
        sm = gameObject.transform.GetComponent<splineMove>();
        isCross = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
       
        //Debug.DrawRay(transform.position, transform.forward,Color.red,2f);
        if (Physics.Raycast(RayStart.position, transform.forward, out hit, 0.3F))
        {
            if (hit.collider.tag == "Car")
            {
               // MinusSpeed();
                sm.Pause();
            }
            else
            {
                //PlusSpeed();
                //sm.Resume();
            }
        }
        else
        {
            if (SPLEng._traffic_signal_red_light && isCross)
            {
                sm.Pause();

            }
            else
            {
                sm.Resume();

            }
        }
    }

    void MinusSpeed()
    {
        if (sm.speed == 0)
        {
            sm.speed = 0;
            sm.Pause();
        }
        else
        {
            sm.speed = sm.speed - 1f;
        }


    }

    void PlusSpeed()
    {
        if (!sm.tween.IsPlaying())
        {
            sm.Resume();
        }
        else
        {
            if (sm.speed > 1.9)
            {
                sm.speed = 2;

            }
            else
            {
                sm.speed = sm.speed + 0.02f;
            }
        }
        

    }
    
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.name == "Crosswalk")
        {
            isCross = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isCross = false;
    }
}
