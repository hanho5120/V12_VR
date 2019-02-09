using UnityEngine;
using System.Collections;

public class RoateTargetAroundAxis : MonoBehaviour {

	public GameObject TargetObject;
	public Vector3 TargetPosition = new Vector3(0, 0, 0);
	public string TargetAxis = "Y";
	public float RotationSpeed = 0.1f;

	void Start () {
	
	}
	

	void Update () {
	
		if (TargetObject != null && TargetObject.activeSelf)
		{
			if (TargetAxis == "X")
				TargetObject.transform.RotateAround(TargetPosition, Vector3.right, RotationSpeed);
			else if (TargetAxis == "Z")
				TargetObject.transform.RotateAround(TargetPosition, Vector3.forward, RotationSpeed);
			else
				TargetObject.transform.RotateAround(TargetPosition, Vector3.up, RotationSpeed);
		}
	}
}
