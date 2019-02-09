using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCarCtrl : MonoBehaviour
{
    public MainCamControl _MainCamControlInstance = null;
    public RobotControl RobotCtrl;


    // Use this for initialization
    void Start()
    {
        RobotCtrl = GetComponent<RobotControl>();
    }

    // Update is called once per frame
    void Update()
    {
        bool event_res_left = _MainCamControlInstance.GetLeftJoystickPressed();
        if (event_res_left)
        {
            Vector2 Movepos = _MainCamControlInstance.GetLeftJoystick();
            Vector3 ConMovePos = new Vector3(Movepos.x, 0, -Movepos.y);
            float MovePosLength = ConMovePos.magnitude;


            MovementCar(ConMovePos, MovePosLength);
        }

    }

    public void MovementCar(Vector3 ConMovePos, float Length)
    {

     

        ConMovePos.Normalize();

        if (Mathf.Abs(ConMovePos.z) < 0.1)
        {
            if (ConMovePos.z >= 0 && ConMovePos.z < 0.1f)
            {
                ConMovePos.z = 0.15f;
            }
            else
            {
                ConMovePos.z = -0.15f;
            }
        }

        if (ConMovePos.z >= 0)
        {
            transform.Rotate(new Vector3(0f, ConMovePos.x * 30 * Time.deltaTime, 0f));

            Vector3 movement = new Vector3(0.0f, 0.0f, ConMovePos.z * 1.5f * Time.deltaTime);
            transform.Translate(movement);
        }
        else
        {
            transform.Rotate(new Vector3(0f, -ConMovePos.x * 30 * Time.deltaTime, 0f));

            Vector3 movement = new Vector3(0.0f, 0.0f, ConMovePos.z * 1.5f * Time.deltaTime);
            transform.Translate(movement);
        }

        if (RobotCtrl)
        {
            RobotCtrl.JoystickMove(ConMovePos);
        }


    }
}
