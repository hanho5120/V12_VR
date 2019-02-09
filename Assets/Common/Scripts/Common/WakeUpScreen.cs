using UnityEngine;
using System.Collections;

public class WakeUpScreen : MonoBehaviour {

	void Start () {

		#if UNITY_ANDROID
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		#endif
	}
	
	void Update () {
	
	}
}
