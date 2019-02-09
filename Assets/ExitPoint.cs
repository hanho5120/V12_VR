using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ExitPoint : MonoBehaviour
{
    private Vector3 lookAtPosition = Vector3.zero;
    private MeshRenderer markerMesh;
    private MeshRenderer switchSceneIcon;
    private MeshRenderer moveLocationIcon;
    private MeshRenderer lockedIcon;
    private MeshRenderer pointIcon;
    private Transform lookAtJointTransform;

    private Text titleText;

    // Use this for initialization
    void Start()
    {

        GetRelevantComponents();
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isPlaying)
        {
            //lookAtPosition.x = player.hmdTransform.position.x;
            //lookAtPosition.y = lookAtJointTransform.position.y;
            //lookAtPosition.z = player.hmdTransform.position.z;

           // lookAtJointTransform.LookAt(lookAtPosition);
        }


    }

    public void GetRelevantComponents()
    {
        //markerMesh = transform.Find("teleport_marker_mesh").GetComponent<MeshRenderer>();
        //switchSceneIcon = transform.Find("teleport_marker_lookat_joint/teleport_marker_icons/switch_scenes_icon").GetComponent<MeshRenderer>();
        //moveLocationIcon = transform.Find("teleport_marker_lookat_joint/teleport_marker_icons/move_location_icon").GetComponent<MeshRenderer>();
        //lockedIcon = transform.Find("teleport_marker_lookat_joint/teleport_marker_icons/locked_icon").GetComponent<MeshRenderer>();
        lookAtJointTransform = transform.Find("teleport_marker_lookat_joint");

        titleText = transform.Find("teleport_marker_lookat_joint/teleport_marker_canvas/teleport_marker_canvas_text").GetComponent<Text>();

        //gotReleventComponents = true;
    }



}
