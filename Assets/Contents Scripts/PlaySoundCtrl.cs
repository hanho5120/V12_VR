using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SPL.Common;
using SWS;

public class PlaySoundCtrl : MonoBehaviour
{
    public SPLEngine SPLEng;
    public AudioSource Audio;
    public Animator Anim;
    public GameObject StandLight;    
    public List<splineMove> Woofers;
    // Use this for initialization
    void Start()
    {
        StartCoroutine(PlayandPausePath());
    }

    // Update is called once per frame
    void Update()
    {
        if (Audio.isPlaying)
        {
            Anim.SetBool("isPlay", true);
        }
        else
        {
            Anim.SetBool("isPlay", false);
        }

        if (SPLEng._GlobalVariables.Count > 0)
        {
            int d13 = Util.ToInt(SPLEng._GlobalVariables["_DIGITAL_13"]);
            if (d13 == 1)
            {
                StandLight.SetActive(true);
            }
            else
            {
                StandLight.SetActive(false);
            }
        }
    }

    IEnumerator PlayandPausePath()
    {
        while (true)
        {
            if (Audio.isPlaying)
            {
                foreach (splineMove sm in Woofers)
                {
                    sm.Resume();
                }
            }
            else
            {
                foreach (splineMove sm in Woofers)
                {
                    sm.Pause();
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
