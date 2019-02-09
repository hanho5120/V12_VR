using UnityEngine;
using System.Collections;

public class HASmoothFollow : MonoBehaviour {

	public GameObject Target;
	public float Distance = 10.0f;
	public float Height = 5.0f;
	public bool FollowingEnable = false;

	Vector3 _pre_position = new Vector3(0, 0, 0);

	void Setup()
	{
		_pre_position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
	}

	void LateUpdate () {
	
		if (FollowingEnable)
		{
			if (!Target)
				return;

			float distance_between = Vector3.Distance(_pre_position, Target.transform.position);

			if (distance_between > 0)
			{
				Vector3 dir_v3 = _pre_position - Target.transform.position;
				dir_v3.Normalize();
				dir_v3 = dir_v3 * Distance;

				float wantedHeight = Target.transform.position.y + Height;
				float currentHeight = gameObject.transform.position.y;
				float newHeight = Mathf.Lerp (currentHeight, wantedHeight, 0.9f);

				Vector3 tobe_pos = Target.transform.position + new Vector3(dir_v3.x, newHeight, dir_v3.z);

				gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, tobe_pos, 0.1f);

				_pre_position = new Vector3(Target.transform.position.x, Target.transform.position.y, Target.transform.position.z);
			}

			gameObject.transform.LookAt(Target.transform.position);
		}

	}
}
