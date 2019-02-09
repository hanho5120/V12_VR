using UnityEngine;
using System.Collections;

public class ShakeObjectAlongZ : MonoBehaviour {

	public float Speed = 1.0f;

	float cur_deg  = 0;
	int dir = 0;

	void Start () {
	
	}


	// Update is called once per frame
	void Update () {

		if (dir == 0)
		{
			cur_deg = cur_deg + (Speed);

			if (transform.localEulerAngles.z > 10 && transform.localEulerAngles.z < 180)
			{
				dir = 1;
			}
		}
		else
		{
			cur_deg = cur_deg - Speed;

			if (transform.localEulerAngles.z < 350 && transform.localEulerAngles.z > 180)
			{
				dir = 0;
			}
		}

		transform.localEulerAngles = new Vector3(0, 0, cur_deg);
	}
}
