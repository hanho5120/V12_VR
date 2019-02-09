using UnityEngine;
using System.Collections;

public class RotateMainCamForActivation : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//transform.Rotate(0.05f, 0.1f, 0);

        //2017.11.14
        transform.Rotate(0.5f * Time.fixedDeltaTime, 1.0f * Time.fixedDeltaTime, 0);
	}
}
