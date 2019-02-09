using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

public class RobotControl : MonoBehaviour
{

    public GameObject FrontLeftWheel = null;
    public GameObject FrontRightWheel = null;
    public GameObject RearLeftWheel = null;
    public GameObject RearRightWheel = null;

    ArrayList _ColorTextureList = new ArrayList();


    float pre_left_power = 0f;
    float pre_right_power = 0f;

    float speed = 2.5F;
    float rotationSpeed = 50.0F;

    //GUI
    float _w_ratio = 1.0f;
    float _h_ratio = 1.0f;


    //Font
    public Font[] Fonts;

    public Texture2D[] _ButtonTextures;

    //Btn
    Rect[] _btn_rects = new Rect[20];
    GUIStyle _GuiStyle_game_btn = new GUIStyle();
    GUIStyle _GuiStyle_num_btn = new GUIStyle();


    //Sensor On Off
    bool[] _Sensor_On_Off = new bool[4] { true, false, true, false };

    public GameObject[] HeadLight = new GameObject[3];
    public GameObject BreakLight;

    public Color OriginalColor;
    public List<Transform> Sensors;

    public TrailRenderer TR;

    void Start()
    {
        //OriginalColor = GetComponent<MeshRenderer>().material.color;
        StartCoroutine(FlickerItem());

    }


    public static string LEFT_POWER = string.Empty;
    public static string RIGHT_POWER = string.Empty;


    IEnumerator FlickerItem()
    {
        while (true)
        {
            foreach (Transform obj in Sensors)
            {
                obj.GetComponent<MeshRenderer>().material.color = Color.red;
            }
            yield return new WaitForSeconds(0.5f);

            foreach (Transform obj in Sensors)
            {
                obj.GetComponent<MeshRenderer>().material.color = Color.white;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    void Update()
    {


        float right_power = 0;
        float left_power = 0;


        if (string.IsNullOrEmpty(LEFT_POWER) && string.IsNullOrEmpty(RIGHT_POWER))
        {
  
            
            //float translation = Input.GetAxis("Vertical") * speed;
            //float rotation = Input.GetAxis("Horizontal") * rotationSpeed;

            //Debug.Log(Input.GetAxis("Vertical"));
            //Debug.Log(Input.GetAxis("Horizontal"));

            //translation *= Time.fixedDeltaTime;
            //rotation *= Time.fixedDeltaTime * 2.0f;

            //transform.Translate(0, 0, translation);
            //transform.Rotate(0, rotation, 0);

            //right_power = translation - (rotation * 0.1f);
            //left_power = translation + (rotation * 0.1f);
        }
        else
        {
           
            
            float left_power_val = float.Parse(LEFT_POWER) / 255.0f;
            float right_power_val = float.Parse(RIGHT_POWER) / 255.0f;

            //if (TR)
            //{
            //    if (left_power_val == 0 && right_power_val == 0)
            //    {
            //        TR.time = 0;
            //    }
            //    else
            //    {
            //        TR.time = 99999;

            //    }
            //}

            float vertical_input = (left_power_val + right_power_val) / 5.0f;
            float rotation_input = (left_power_val - right_power_val) / 5.0f;


            float translation = vertical_input * speed;
            float rotation = rotation_input * rotationSpeed;


            translation *= Time.deltaTime;
            rotation *= Time.deltaTime * 2.0f;

            transform.Translate(0, 0, translation);
            transform.Rotate(0, rotation, 0);


            if (!string.IsNullOrEmpty(LEFT_POWER))
                left_power = left_power_val * 2.0f * Time.fixedDeltaTime;

            if (!string.IsNullOrEmpty(RIGHT_POWER))
                right_power = right_power_val * 2.0f * Time.fixedDeltaTime;

            left_power = (left_power + pre_left_power) / 2.0f;
            right_power = (right_power + pre_right_power) / 2.0f;

            pre_left_power = left_power;
            pre_right_power = right_power;
        }

        //-0.33 ~ +0.33 forward
        //-0.16 ~ +0.16 rotate
        //Debug.Log(left_power + " / " + right_power);

        //FrontRightWheel.transform.Rotate(right_power * 150.0f, 0, 0);
        //      RearRightWheel.transform.Rotate(right_power * 150.0f, 0, 0);

        //      FrontLeftWheel.transform.Rotate(-left_power * 150.0f, 0, 0);
        //      RearLeftWheel.transform.Rotate(-left_power * 150.0f, 0, 0);


        FrontRightWheel.transform.Rotate( right_power * 150.0f, 0,0);
        RearRightWheel.transform.Rotate(right_power * 150.0f,0, 0);

        FrontLeftWheel.transform.Rotate( left_power * 150.0f,0, 0);
        RearLeftWheel.transform.Rotate(left_power * 150.0f,0, 0);

      

    }


    void ResetRobot()
    {
        transform.position = new Vector3(0, 4, 0);
        transform.rotation = Quaternion.identity;
    }

    void HideRobot()
    {
        transform.position = new Vector3(0, -2000, 0);
        transform.rotation = Quaternion.identity;
    }

    public void JoystickMove(Vector3 MovePos)
    {
        float right_power = 0;
        float left_power = 0;

        float translation = MovePos.z * speed;
        float rotation = MovePos.x * rotationSpeed;

        translation *= Time.fixedDeltaTime;
        rotation *= Time.fixedDeltaTime ;

  
        right_power = translation - (rotation * 0.1f);
        left_power = translation + (rotation * 0.1f);

        FrontRightWheel.transform.Rotate(right_power * 150.0f, 0, 0);
        RearRightWheel.transform.Rotate(right_power * 150.0f, 0, 0);

        FrontLeftWheel.transform.Rotate(left_power * 150.0f, 0, 0);
        RearLeftWheel.transform.Rotate(left_power * 150.0f, 0, 0);

      
    }


    //####################################################################################################

}
