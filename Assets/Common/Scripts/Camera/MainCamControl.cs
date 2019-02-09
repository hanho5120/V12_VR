using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using gv;

public class MainCamControl : MonoBehaviour
{
    string a="";

    public float Rotation_Speed = 5.0f;

    private float Translate_Speed_Hor = 30.0f;
    private float Translate_Speed_Ver = 30.0f;

    private float Rotation_Speed_Hor = 5000.0f;
    private float Rotation_Speed_Ver = 5000.0f;


    //GUI
    float _w_ratio = 1.0f;
    float _h_ratio = 1.0f;
    float _s_ratio = 1.0f;

    //JoyStick
    //######################################################################
    public Texture2D[] JoystickTextureList = null;
    bool _JoystickUse = true;

    Rect GUI_Out_Ring_Left_Rect = new Rect(0, 0, 256, 256);
    Rect GUI_Out_Ring_Right_Rect = new Rect(0, 0, 256, 256);

    Rect GUI_Center_Ring_Left_Rect = new Rect(0, 0, 180, 180);
    Rect GUI_Center_Ring_Right_Rect = new Rect(0, 0, 180, 180);

    Rect GUI_Center_Ring_Dumy_Rect = new Rect(0, 0, 180, 180);

    Rect GUI_Ext_Ring_Left_Rect = new Rect(0, 0, 512, 512);
    Rect GUI_Ext_Ring_Right_Rect = new Rect(0, 0, 512, 512);


    Rect GUI_Btn_Ring_Left_Rect = new Rect(0, 0, 80, 80);
    Rect GUI_Btn_Ring_Right_Rect = new Rect(0, 0, 80, 80);
    //Rect GUI_Btn_Ring_Left_Rect = new Rect(0, 0, 160, 60);
    //Rect GUI_Btn_Ring_Right_Rect = new Rect(0, 0, 160, 60);


    float _left_ring_center_x = 0;
    float _left_ring_center_y = 0;
    float _right_ring_center_x = 0;
    float _right_ring_center_y = 0;

    float _left_ring_offset_x = 0;
    float _left_ring_offset_y = 0;
    float _right_ring_offset_x = 0;
    float _right_ring_offset_y = 0;


    //2017.11.19
    //#####################################
    bool _OnLeftJoystickClick_Used = false;
    bool _OnRightJoystickClick_Used = false;

    bool _OnLeftJoystickPressed_Used = false;
    bool _OnRightJoystickPressed_Used = false;
    //#####################################

    bool _OnLeftButtonClick_Used = false;
    bool _OnRightButtonClick_Used = false;

    bool _OnLeftButtonPressed_Used = false;
    bool _OnRightButtonPressed_Used = false;

    bool _OnLeftJoystick_Used = false;
    bool _OnRightJoystick_Used = false;

    //######################################################################


    GUIStyle _GuiStyle_Log = new GUIStyle();



    //2017.11.19
    //######################################################################
    //외부에서 카메라 각도 설정시 에니메이션으로 이동시킨다.
    //GUI 함수에서 1도씩 값을 빼준다.
    float _target_container_y_deg = 0;
    float _target_container_x_deg = 0;
    float _target_container_z_distance = 0;
    bool _smooth_rotate_request = false;

    //######################################################################


    //Font
    public Font[] Fonts;
    private GUIStyle _GuiStyle_joystick_btn_text = null;

    //2017.11.19
    private string _selected_coding_type = string.Empty;

    //조이스틱 이동 위치 계산
    private Vector2 _joystic_dumy_v2 = new Vector2();
    private float _MAX_JOYSTIC_DISTANCE = 150;



    //######################################################################

    private GameObject _CONTAINER_Y;
    private GameObject _CONTAINER_X;
    private GameObject _OFFSET_FROM_CENTER;
    private GameObject _MAIN_CAMERA;

    //######################################################################


    void Start()
    {

        _w_ratio = Screen.width / 1280.0f;
        _h_ratio = Screen.height / 720.0f;

        _s_ratio = _w_ratio;

        if (_h_ratio < _w_ratio)
            _s_ratio = _h_ratio;


        //##########################################################################
        _CONTAINER_Y = transform.Find("Container_Y").gameObject;
        _CONTAINER_X = transform.Find("Container_Y/Container_X").gameObject;
        _OFFSET_FROM_CENTER = transform.Find("Container_Y/Container_X/Offset_From_Center").gameObject;
        _MAIN_CAMERA = transform.Find("Container_Y/Container_X/Offset_From_Center/MainCamera").gameObject;
        //##########################################################################



        //2017.11.19
        //##########################################################################
        _selected_coding_type = GlobalVariables.SelectedCodingType;
        //##########################################################################


        _GuiStyle_Log.fontSize = (int)(28 * _h_ratio);
        _GuiStyle_Log.normal.textColor = new Color(1, 1, 1);


        //Joystick
        //#############################################################################################
        //Left Ring
        //#############################################################################################
        GUI_Out_Ring_Left_Rect.width = GUI_Out_Ring_Left_Rect.width * _h_ratio;
        GUI_Out_Ring_Left_Rect.height = GUI_Out_Ring_Left_Rect.height * _h_ratio;
        GUI_Out_Ring_Left_Rect.x = 50 * _h_ratio;
        GUI_Out_Ring_Left_Rect.y = Screen.height - (40 * _h_ratio) - GUI_Out_Ring_Left_Rect.height;

        _left_ring_center_x = GUI_Out_Ring_Left_Rect.x + GUI_Out_Ring_Left_Rect.width * 0.5f;
        _left_ring_center_y = GUI_Out_Ring_Left_Rect.y + GUI_Out_Ring_Left_Rect.height * 0.5f;


        //Left Ext Ring
        GUI_Ext_Ring_Left_Rect.width = GUI_Ext_Ring_Left_Rect.width * _h_ratio;
        GUI_Ext_Ring_Left_Rect.height = GUI_Ext_Ring_Left_Rect.height * _h_ratio;
        GUI_Ext_Ring_Left_Rect.x = _left_ring_center_x - (GUI_Ext_Ring_Left_Rect.width * 0.5f);
        GUI_Ext_Ring_Left_Rect.y = _left_ring_center_y - (GUI_Ext_Ring_Left_Rect.height * 0.5f);


        //Left Center Ring
        GUI_Center_Ring_Left_Rect.width = GUI_Center_Ring_Left_Rect.width * _h_ratio;
        GUI_Center_Ring_Left_Rect.height = GUI_Center_Ring_Left_Rect.height * _h_ratio;
        GUI_Center_Ring_Left_Rect.x = _left_ring_center_x - (GUI_Center_Ring_Left_Rect.width * 0.5f);
        GUI_Center_Ring_Left_Rect.y = _left_ring_center_y - (GUI_Center_Ring_Left_Rect.height * 0.5f);

        GUI_Center_Ring_Dumy_Rect.width = GUI_Center_Ring_Left_Rect.width;
        GUI_Center_Ring_Dumy_Rect.height = GUI_Center_Ring_Left_Rect.height;


        //Left Button
        GUI_Btn_Ring_Left_Rect.width = GUI_Btn_Ring_Left_Rect.width * _h_ratio;
        GUI_Btn_Ring_Left_Rect.height = GUI_Btn_Ring_Left_Rect.height * _h_ratio;
        GUI_Btn_Ring_Left_Rect.x = _left_ring_center_x + GUI_Center_Ring_Left_Rect.width;
        GUI_Btn_Ring_Left_Rect.y = _left_ring_center_y;


        //Right Ring
        //#############################################################################################
        GUI_Out_Ring_Right_Rect.width = GUI_Out_Ring_Right_Rect.width * _h_ratio;
        GUI_Out_Ring_Right_Rect.height = GUI_Out_Ring_Right_Rect.height * _h_ratio;
        GUI_Out_Ring_Right_Rect.x = Screen.width - (50 * _h_ratio) - GUI_Out_Ring_Right_Rect.width;
        GUI_Out_Ring_Right_Rect.y = Screen.height - (40 * _h_ratio) - GUI_Out_Ring_Right_Rect.height;

        _right_ring_center_x = GUI_Out_Ring_Right_Rect.x + GUI_Out_Ring_Right_Rect.width * 0.5f;
        _right_ring_center_y = GUI_Out_Ring_Right_Rect.y + GUI_Out_Ring_Right_Rect.height * 0.5f;


        //Right Ext Ring
        GUI_Ext_Ring_Right_Rect.width = GUI_Ext_Ring_Right_Rect.width * _h_ratio;
        GUI_Ext_Ring_Right_Rect.height = GUI_Ext_Ring_Right_Rect.height * _h_ratio;
        GUI_Ext_Ring_Right_Rect.x = _right_ring_center_x - (GUI_Ext_Ring_Right_Rect.width * 0.5f);
        GUI_Ext_Ring_Right_Rect.y = _right_ring_center_y - (GUI_Ext_Ring_Right_Rect.height * 0.5f);


        //Right Center Ring
        GUI_Center_Ring_Right_Rect.width = GUI_Center_Ring_Right_Rect.width * _h_ratio;
        GUI_Center_Ring_Right_Rect.height = GUI_Center_Ring_Right_Rect.height * _h_ratio;
        GUI_Center_Ring_Right_Rect.x = _right_ring_center_x - (GUI_Center_Ring_Right_Rect.width * 0.5f);
        GUI_Center_Ring_Right_Rect.y = _right_ring_center_y - (GUI_Center_Ring_Right_Rect.height * 0.5f);


        //Right Button
        GUI_Btn_Ring_Right_Rect.width = GUI_Btn_Ring_Right_Rect.width * _h_ratio;
        GUI_Btn_Ring_Right_Rect.height = GUI_Btn_Ring_Right_Rect.height * _h_ratio;
        GUI_Btn_Ring_Right_Rect.x = _right_ring_center_x - GUI_Center_Ring_Right_Rect.width - GUI_Btn_Ring_Right_Rect.width;
        GUI_Btn_Ring_Right_Rect.y = _right_ring_center_y;



        //조이스틱의 최대 이동 반경
        _MAX_JOYSTIC_DISTANCE = 150 * _s_ratio;

        //#############################################################################################

    }


    public Vector3 GetCameraOrientation()
    {
        Vector3 res = new Vector3();

        res.x = _CONTAINER_X.transform.localEulerAngles.x;
        res.y = _CONTAINER_Y.transform.localEulerAngles.y;

        return res;
    }


    public void EnableJoystick()
    {
        _JoystickUse = true;
    }

    public void DisableJoystick()
    {
        _JoystickUse = false;
    }


    public Vector2 GetLeftJoystick()
    {
        _OnLeftJoystick_Used = true;
        return (_left_joystick_offset_pos * _h_ratio);
    }

    public Vector2 GetRightJoystick()
    {
        _OnRightJoystick_Used = true;
        return (_right_joystick_offset_pos * _h_ratio);
    }



    public bool GetLeftJoystickPressed()
    {
        _OnLeftJoystickPressed_Used = true;

        return _left_joystick_touched;
    }

    public bool GetRightJoystickPressed()
    {
        _OnRightJoystickPressed_Used = true;

        return _right_joystick_touched;
    }


    bool _left_joystick_down_check = false;

    public bool GetLeftJoystickClick()
    {
        _OnLeftJoystickClick_Used = true;

        if (_left_joystick_touched)
        {
            if (_left_joystick_down_check == false)
            {
                _left_joystick_down_check = true;
                return true;
            }
            else
                return false;
        }
        else
        {
            _left_joystick_down_check = false;
            return false;
        }
    }

    bool _right_joystick_down_check = false;

    public bool GetRightJoystickClick()
    {
        _OnRightJoystickClick_Used = true;

        if (_right_joystick_touched)
        {
            if (_right_joystick_down_check == false)
            {
                _right_joystick_down_check = true;
                return true;
            }
            else
                return false;
        }
        else
        {
            _right_joystick_down_check = false;
            return false;
        }
    }


    public void SetCameraPosWithAnimation(Vector3 target_pos)
    {
        _smooth_rotate_request = true;
        _target_container_y_deg = target_pos.x;
        _target_container_x_deg = target_pos.y;
        _target_container_z_distance = target_pos.z;
    }


    //###########################################################


    public bool GetLeftButtonPressed()
    {
        _OnLeftButtonPressed_Used = true;

        return _left_button_touched;
    }

    public bool GetRightButtonPressed()
    {
        _OnRightButtonPressed_Used = true;

        return _right_button_touched;
    }


    bool _left_button_down_check = false;

    public bool GetLeftButtonClick()
    {
        _OnLeftButtonClick_Used = true;

        if (_left_button_touched)
        {
            if (_left_button_down_check == false)
            {
                _left_button_down_check = true;
                return true;
            }
            else
                return false;
        }
        else
        {
            _left_button_down_check = false;
            return false;
        }
    }

    bool _right_button_down_check = false;

    public bool GetRightButtonClick()
    {
        _OnRightButtonClick_Used = true;

        if (_right_button_touched)
        {
            if (_right_button_down_check == false)
            {
                _right_button_down_check = true;
                return true;
            }
            else
                return false;
        }
        else
        {
            _right_button_down_check = false;
            return false;
        }
    }


    string _log_msg = string.Empty;

    void Update()
    {

        

        //#####################################################################################################
        //화면 터치 값을 받아서 화면을 회전시켜 준다.
        //#####################################################################################################

        Mouse_Drag_Calc_Items();

#if UNITY_ANDROID

        if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            if (deltaMagnitudeDiff < 0)
            {
                _OFFSET_FROM_CENTER.transform.Translate(new Vector3(0, 0, 0.1f * 2.0f * Translate_Speed_Ver * Time.deltaTime), Space.Self);
            }

            if (deltaMagnitudeDiff > 0)
            {
                _OFFSET_FROM_CENTER.transform.Translate(new Vector3(0, 0, 0.1f * -2.0f * Translate_Speed_Ver * Time.deltaTime), Space.Self);
            }
        }


        //#####################################################################################################
        //키보드 값을 받아서 화면을 회전시켜 준다.
        //#####################################################################################################
        float dir1 = Input.GetAxis("Horizontal");

        if (dir1 != 0)
        {
            _CONTAINER_Y.transform.Rotate(0, dir1 * Translate_Speed_Hor * Time.deltaTime * -2.0f, 0);
        }

        float dir2 = Input.GetAxis("Vertical");

        if (dir2 != 0)
        {
            _OFFSET_FROM_CENTER.transform.Translate(new Vector3(0, 0, 0.1f * dir2 * Translate_Speed_Ver * Time.deltaTime), Space.Self);
        }

        //#####################################################################################################
        //마우스 휠 값을 받아서 화면을 앞 뒤로 이동시켜 준다.
        //#####################################################################################################
        if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
        {
            _OFFSET_FROM_CENTER.transform.Translate(new Vector3(0, 0, 0.1f * -2.0f * Translate_Speed_Ver * Time.deltaTime), Space.Self);
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
        {
            _OFFSET_FROM_CENTER.transform.Translate(new Vector3(0, 0, 0.1f * 2.0f * Translate_Speed_Ver * Time.deltaTime), Space.Self);
        }

#endif
    }



    void OnGUI()
    {
        //GUI.Label(new Rect(0, 0, 500, 500), Application.platform.ToString());
        //Event e = Event.current;
        //Debug.Log(e.type.ToString());
        
        //if (e.type.ToString() != "Layout" && e.type.ToString() != "Repaint")
        //{
        //    a = e.type.ToString();
        //}
        //else
        //{

        //}

        //GUI.Label(new Rect(0, 100, 500, 500), a);



        //2017.11.20
        //#####################################################################################################
        //Joystick Button Text
        if (_GuiStyle_joystick_btn_text == null)
        {
            _GuiStyle_joystick_btn_text = new GUIStyle();
            _GuiStyle_joystick_btn_text.font = Fonts[0];
            _GuiStyle_joystick_btn_text.fontSize = (int)(_s_ratio * 22);
            //_GuiStyle_joystick_btn_text.normal.textColor = new Color(140, 140, 140);
            _GuiStyle_joystick_btn_text.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
            _GuiStyle_joystick_btn_text.alignment = TextAnchor.MiddleCenter;
        }

        //#####################################################################################################







        #if (UNITY_EDITOR_WIN || UNITY_STANDALONE)
        //#####################################################################################################
        //키보드 값을 받아서 화면을 회전시켜 준다.
        //#####################################################################################################
        float dir1 = Input.GetAxis("Horizontal");

        if (dir1 != 0)
        {
            _CONTAINER_Y.transform.Rotate(0, dir1 * Translate_Speed_Hor * Time.deltaTime * -2.0f, 0);
        }

        float dir2 = Input.GetAxis("Vertical");

        if (dir2 != 0)
        {
            _OFFSET_FROM_CENTER.transform.Translate(new Vector3(0, 0, 0.1f * dir2 * Translate_Speed_Ver * Time.deltaTime), Space.Self);
        }

        //#####################################################################################################
        //마우스 휠 값을 받아서 화면을 앞 뒤로 이동시켜 준다.
        //#####################################################################################################
        if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
        {
            _OFFSET_FROM_CENTER.transform.Translate(new Vector3(0, 0, 0.1f * -2.0f * Translate_Speed_Ver * Time.deltaTime), Space.Self);
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
        {
            _OFFSET_FROM_CENTER.transform.Translate(new Vector3(0, 0, 0.1f * 2.0f * Translate_Speed_Ver * Time.deltaTime), Space.Self);
        }
#else

#endif








        //#####################################################################################################
        //조이스틱 버튼 이미지를 그려준다.
        //#####################################################################################################

        if (_JoystickUse)
        {
            //######################################################
            //왼쪽 조이스틱
            //######################################################
            //Left Out Ring
            GUI.DrawTexture(GUI_Out_Ring_Left_Rect, JoystickTextureList[0]);

            //Left Center Ring
            if (_left_joystick_touched)
            {
                GUI_Center_Ring_Dumy_Rect.x = GUI_Center_Ring_Left_Rect.x + _left_joystick_offset_pos.x;
                GUI_Center_Ring_Dumy_Rect.y = GUI_Center_Ring_Left_Rect.y + _left_joystick_offset_pos.y;

                GUI.DrawTexture(GUI_Center_Ring_Dumy_Rect, JoystickTextureList[2]);
            }
            else
                GUI.DrawTexture(GUI_Center_Ring_Left_Rect, JoystickTextureList[1]);

            //Left Button
            if (_left_button_touched)
                GUI.DrawTexture(GUI_Btn_Ring_Left_Rect, JoystickTextureList[2]);
            else
                GUI.DrawTexture(GUI_Btn_Ring_Left_Rect, JoystickTextureList[1]);


            //######################################################
            //오른쪽 조이스틱
            //######################################################
            //Right Out Ring
            GUI.DrawTexture(GUI_Out_Ring_Right_Rect, JoystickTextureList[0]);

            //Right Center Ring
            if (_right_joystick_touched)
            {
                GUI_Center_Ring_Dumy_Rect.x = GUI_Center_Ring_Right_Rect.x + _right_joystick_offset_pos.x;
                GUI_Center_Ring_Dumy_Rect.y = GUI_Center_Ring_Right_Rect.y + _right_joystick_offset_pos.y;

                GUI.DrawTexture(GUI_Center_Ring_Dumy_Rect, JoystickTextureList[2]);
            }
            else
                GUI.DrawTexture(GUI_Center_Ring_Right_Rect, JoystickTextureList[1]);

            //Right Button
            if (_right_button_touched)
                GUI.DrawTexture(GUI_Btn_Ring_Right_Rect, JoystickTextureList[2]);
            else
                GUI.DrawTexture(GUI_Btn_Ring_Right_Rect, JoystickTextureList[1]);



            //###########################################################################################
            //조이스틱 버튼 Text 표시
            //###########################################################################################
            if (_selected_coding_type == "TrafficSignal" || _selected_coding_type == "ButtonLight" || _selected_coding_type == "PlaySound" || _selected_coding_type == "SimpleRobotDriving")
            {
                GUI.Label(GUI_Out_Ring_Left_Rect, "D03\nButton", _GuiStyle_joystick_btn_text);
                GUI.Label(GUI_Btn_Ring_Left_Rect, "D04\nButton", _GuiStyle_joystick_btn_text);
                GUI.Label(GUI_Btn_Ring_Right_Rect, "D05\nButton", _GuiStyle_joystick_btn_text);
                GUI.Label(GUI_Out_Ring_Right_Rect, "D06\nButton", _GuiStyle_joystick_btn_text);
            }
            else if (_selected_coding_type == "DetectMovingObject")
            {
                GUI.Label(GUI_Out_Ring_Left_Rect, "Move\nCharacter", _GuiStyle_joystick_btn_text);
                GUI.Label(GUI_Out_Ring_Right_Rect, "Move\nDog", _GuiStyle_joystick_btn_text);

            }
            else if (_selected_coding_type == "SecurityAlert")
            {
                GUI.Label(GUI_Out_Ring_Left_Rect, "Move\nCharacter", _GuiStyle_joystick_btn_text);
                GUI.Label(GUI_Out_Ring_Right_Rect, "Move\nDog", _GuiStyle_joystick_btn_text);
                GUI.Label(GUI_Btn_Ring_Left_Rect, "D03\nButton", _GuiStyle_joystick_btn_text);
            }
            else if (_selected_coding_type == "AutoLight")
            {
                GUI.Label(GUI_Btn_Ring_Left_Rect, "+\nButton", _GuiStyle_joystick_btn_text);
                GUI.Label(GUI_Btn_Ring_Right_Rect, "-\nButton", _GuiStyle_joystick_btn_text);
            }
            else if (_selected_coding_type == "ReactionRobot")
            {
                GUI.Label(GUI_Out_Ring_Left_Rect, "Move\nCharacter", _GuiStyle_joystick_btn_text);
                GUI.Label(GUI_Btn_Ring_Left_Rect, "D03\nButton", _GuiStyle_joystick_btn_text);
            }
            else if (_selected_coding_type == "RearDistanceSensing" || _selected_coding_type == "AvoidObstacle")
            {
                GUI.Label(GUI_Out_Ring_Left_Rect, "Move\nRobot", _GuiStyle_joystick_btn_text);
                GUI.Label(GUI_Btn_Ring_Left_Rect, "D03\nButton", _GuiStyle_joystick_btn_text);
            }
            else if (_selected_coding_type == "AutoParking")
            {
                GUI.Label(GUI_Out_Ring_Left_Rect, "D03\nButton", _GuiStyle_joystick_btn_text);
            }
            //###########################################################################################

        }



        //2017.11.19
        //###################################################################
        //SPL 명령어에서 카메라 각도 조절시 부드럽게 이동시킨다.
        if (_smooth_rotate_request)
        {
            float offset_y = (_CONTAINER_Y.transform.localEulerAngles.y - _target_container_y_deg) * Time.deltaTime;
            float offset_x = (_CONTAINER_X.transform.localEulerAngles.x - _target_container_x_deg) * Time.deltaTime;
            float offset_z = (_OFFSET_FROM_CENTER.transform.localPosition.z - _target_container_z_distance) * Time.deltaTime;


            float abs_y = Mathf.Abs(offset_y);
            float abs_x = Mathf.Abs(offset_x);
            float abs_z = Mathf.Abs(offset_z);


            _CONTAINER_Y.transform.Rotate(0, -offset_y, 0);
            _CONTAINER_X.transform.Rotate(-offset_x, 0, 0);
            _OFFSET_FROM_CENTER.transform.Translate(new Vector3(0, 0, -offset_z), Space.Self);

            if (abs_x < 0.01f && abs_y < 0.01f && abs_z < 0.01f)
                _smooth_rotate_request = false;
        }

        //###################################################################


    }



    //###############################################################
    Vector2 _left_joystick_touch_pos = new Vector2(0, 0);
    Vector2 _right_joystick_touch_pos = new Vector2(0, 0);

    Vector2 _left_joystick_offset_pos = new Vector2(0, 0);
    Vector2 _right_joystick_offset_pos = new Vector2(0, 0);


    //#########################################
    //2018.04.13
    //#########################################
    bool _joystick_down_mode = false;
    //#########################################



    bool _left_joystick_touched = false;
    bool _right_joystick_touched = false;

    bool _left_button_touched = false;
    bool _right_button_touched = false;

    List<Vector2> _TouchList = new List<Vector2>();


    //#########################################
    //2018.04.15
    //가운데 화면을 통한 화면 회전
    //#########################################
    Vector2 _screen_touch_pos = new Vector2(0, 0);
    Vector2 _screen_offset_pos = new Vector2(0, 0);
    bool _screen_touched = false;



    void Mouse_Drag_Calc_Items()
    {
        _TouchList.Clear();

        for (int t = 0; t < Input.touchCount; t++)
        {
            Touch th = Input.GetTouch(t);
            Vector2 Reverse_v2 = new Vector2(th.position.x, Screen.height - th.position.y);
            _TouchList.Add(Reverse_v2);
        }

        if (Input.GetMouseButton(0))
        {
            Vector2 Reverse_v2 = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
            _TouchList.Add(Reverse_v2);
        }


        //#########################################
        //2018.04.13
        //#########################################
        if (_TouchList.Count == 0)
            _joystick_down_mode = false;
        //#########################################


        //Left and Right Joystick
        CheckLeftJoystic();
        CheckRightJoystick();

        //Left and Right Button
        CheckLeftButton();
        CheckRightButton();


        //#########################################
        //2018.04.15
        //#########################################
        CheckCameraRotation();
        //#########################################
    }


    //#########################################
    //2018.04.15
    //#########################################

    //조이스틱이 아닌 그냥 화면 터치를 통한 화면 회전
    private void CheckCameraRotation()
    {
        bool contain_flag = false;

        foreach (Vector2 Reverse_v2 in _TouchList)
        {
            //일반 화면 영역
            //if (GUI_Out_Ring_Left_Rect.Contains(Reverse_v2) && _left_joystick_touched == false)
            //if (!GUI_Out_Ring_Left_Rect.Contains(Reverse_v2) && !GUI_Out_Ring_Right_Rect.Contains(Reverse_v2) &&
            //    _screen_touched == false && _joystick_down_mode == false)
            if (!GUI_Out_Ring_Left_Rect.Contains(Reverse_v2) && !GUI_Out_Ring_Right_Rect.Contains(Reverse_v2) &&
                !GUI_Btn_Ring_Left_Rect.Contains(Reverse_v2) && !GUI_Btn_Ring_Right_Rect.Contains(Reverse_v2) &&
                _screen_touched == false && _joystick_down_mode == false  && Input.touchCount < 2)
            {
                //#########################################
                //2018.04.15
                //#########################################
                if (_joystick_down_mode == false)
                    _joystick_down_mode = true;
                //#########################################


                //Touch Down
                _screen_touched = true;
                _screen_touch_pos.x = Reverse_v2.x;
                _screen_touch_pos.y = Reverse_v2.y;

                _screen_offset_pos.x = 0;
                _screen_offset_pos.y = 0;

                contain_flag = true;
            }
            else if (_joystick_down_mode && _screen_touched == true && Input.touchCount < 2 && !_smooth_rotate_request)
            {
                //Move
                float diff_x = (Reverse_v2.x - _screen_touch_pos.x) / Screen.width;
                float diff_y = (Reverse_v2.y - _screen_touch_pos.y) / -Screen.height;
                float diff_x_abs = UnityEngine.Mathf.Abs(diff_x);
                float diff_y_abs = UnityEngine.Mathf.Abs(diff_y);

                _screen_offset_pos.x = Reverse_v2.x - _screen_touch_pos.x;
                _screen_offset_pos.y = Reverse_v2.y - _screen_touch_pos.y;


                if (diff_x_abs > diff_y_abs)
                {
                    //마우스를 좌우로 움직일 때
                    _CONTAINER_Y.transform.Rotate(0, diff_x * Rotation_Speed_Hor * Time.fixedDeltaTime, 0);
                }
                else if (diff_y_abs > diff_x_abs)
                {
                    //마우스를 위아래로 움직일 때
                    _CONTAINER_X.transform.Rotate(diff_y * -1 * Rotation_Speed_Ver * Time.fixedDeltaTime, 0, 0);
                }


                contain_flag = true;

                //2018.06.02
                //이전 위치 초기화
                _screen_touch_pos.x = Reverse_v2.x;
                _screen_touch_pos.y = Reverse_v2.y;

            }
        }

        if (contain_flag == false)
        {
            _screen_touched = false;
            _screen_offset_pos.x = 0;
            _screen_offset_pos.y = 0;
        }

    }




    private void CheckLeftJoystic()
    {
        bool contain_flag = false;

        foreach (Vector2 Reverse_v2 in _TouchList)
        {
            //Left Joystick
            //if (GUI_Out_Ring_Left_Rect.Contains(Reverse_v2) && _left_joystick_touched == false)
            if (GUI_Out_Ring_Left_Rect.Contains(Reverse_v2) && _left_joystick_touched == false && _joystick_down_mode == false)
            {
                //#########################################
                //2018.04.13
                //#########################################
                if (_joystick_down_mode == false)
                    _joystick_down_mode = true;
                //#########################################

                //Touch Down
                _left_joystick_touched = true;
                _left_joystick_touch_pos.x = Reverse_v2.x;
                _left_joystick_touch_pos.y = Reverse_v2.y;

                _left_joystick_offset_pos.x = 0;
                _left_joystick_offset_pos.y = 0;

                contain_flag = true;
            }
            //else if (GUI_Ext_Ring_Left_Rect.Contains(Reverse_v2) && _left_joystick_touched == true)
            else if (_joystick_down_mode && _left_joystick_touched == true)
            {
                //Move
                float diff_x = (Reverse_v2.x - _left_joystick_touch_pos.x) / Screen.width;
                float diff_y = (Reverse_v2.y - _left_joystick_touch_pos.y) / -Screen.height;
                float diff_x_abs = UnityEngine.Mathf.Abs(diff_x);
                float diff_y_abs = UnityEngine.Mathf.Abs(diff_y);


                //##############################################################################
                //일정한 범위 안에 있는 경우에만 조이스틱 움직임을 그려줌
                _joystic_dumy_v2.x = Reverse_v2.x - _left_joystick_touch_pos.x;
                _joystic_dumy_v2.y = Reverse_v2.y - _left_joystick_touch_pos.y;

                float distance = _joystic_dumy_v2.magnitude;

                //일정한 범위 안에 있는 경우에만 조이스틱 움직임을 그려줌
                if (distance < _MAX_JOYSTIC_DISTANCE)
                {
                    _left_joystick_offset_pos.x = Reverse_v2.x - _left_joystick_touch_pos.x;
                    _left_joystick_offset_pos.y = Reverse_v2.y - _left_joystick_touch_pos.y;

                }
                //##############################################################################


                //Left Joystick
                if (_OnLeftJoystick_Used == false)
                {
                    //
                }

                contain_flag = true;
            }
        }

        if (contain_flag == false)
        {
            _left_joystick_touched = false;
            _left_joystick_offset_pos.x = 0;
            _left_joystick_offset_pos.y = 0;
        }

    }

    private void CheckRightJoystick()
    {
        bool contain_flag = false;

        foreach (Vector2 Reverse_v2 in _TouchList)
        {
            //if (GUI_Out_Ring_Right_Rect.Contains(Reverse_v2) && _right_joystick_touched == false)
            if (GUI_Out_Ring_Right_Rect.Contains(Reverse_v2) && _right_joystick_touched == false && _joystick_down_mode == false)
            {
                //#########################################
                //2018.04.13
                //#########################################
                if (_joystick_down_mode == false)
                    _joystick_down_mode = true;
                //#########################################

                //Touch Down
                _right_joystick_touched = true;
                _right_joystick_touch_pos.x = Reverse_v2.x;
                _right_joystick_touch_pos.y = Reverse_v2.y;

                _right_joystick_offset_pos.x = 0;
                _right_joystick_offset_pos.y = 0;

                contain_flag = true;
            }
            //else if (GUI_Ext_Ring_Right_Rect.Contains(Reverse_v2) && _right_joystick_touched == true)
            else if (_joystick_down_mode && _right_joystick_touched == true)
            {
                //Move
                float diff_x = (Reverse_v2.x - _right_joystick_touch_pos.x) / Screen.width;
                float diff_y = (Reverse_v2.y - _right_joystick_touch_pos.y) / -Screen.height;
                float diff_x_abs = UnityEngine.Mathf.Abs(diff_x);
                float diff_y_abs = UnityEngine.Mathf.Abs(diff_y);


                //##############################################################################
                //일정한 범위 안에 있는 경우에만 조이스틱 움직임을 그려줌
                _joystic_dumy_v2.x = Reverse_v2.x - _right_joystick_touch_pos.x;
                _joystic_dumy_v2.y = Reverse_v2.y - _right_joystick_touch_pos.y;

                float distance = _joystic_dumy_v2.magnitude;

                //일정한 범위 안에 있는 경우에만 조이스틱 움직임을 그려줌
                if (distance < _MAX_JOYSTIC_DISTANCE)
                {
                    _right_joystick_offset_pos.x = Reverse_v2.x - _right_joystick_touch_pos.x;
                    _right_joystick_offset_pos.y = Reverse_v2.y - _right_joystick_touch_pos.y;
                }
                //##############################################################################


                //Right Joystick
                if (_OnRightJoystick_Used == false)
                {
                    //
                }

                contain_flag = true;
            }
        }

        if (contain_flag == false)
        {
            _right_joystick_touched = false;
            _right_joystick_offset_pos.x = 0;
            _right_joystick_offset_pos.y = 0;
        }
    }


    private void CheckLeftButton()
    {
        //Left Button
        if (_left_joystick_touched == false)
        {
            bool contain_flag = false;

            foreach (Vector2 Reverse_v2 in _TouchList)
            {
                //if (GUI_Btn_Ring_Left_Rect.Contains(Reverse_v2))
                if (GUI_Btn_Ring_Left_Rect.Contains(Reverse_v2) && _joystick_down_mode == false)
                {

                    if (_left_button_touched == false)
                    {
                        //Key Down
                        _left_button_touched = true;
                    }
                    else
                    {
                        //Pressed
                    }

                    contain_flag = true;
                }
            }

            if (contain_flag == false)
                _left_button_touched = false;
        }
        else
            _left_button_touched = false;
    }


    private void CheckRightButton()
    {
        //Right Button
        if (_right_joystick_touched == false)
        {
            bool contain_flag = false;

            foreach (Vector2 Reverse_v2 in _TouchList)
            {
                //if (GUI_Btn_Ring_Right_Rect.Contains(Reverse_v2))
                if (GUI_Btn_Ring_Right_Rect.Contains(Reverse_v2) && _joystick_down_mode == false)
                {


                    if (_right_button_touched == false)
                    {
                        //Key Down
                        _right_button_touched = true;
                    }
                    else
                    {
                        //Pressed
                    }

                    contain_flag = true;
                }
            }

            if (contain_flag == false)
                _right_button_touched = false;
        }
        else
            _right_button_touched = false;
    }

}
