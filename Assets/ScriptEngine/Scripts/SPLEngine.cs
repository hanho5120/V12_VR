using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using SPL.Common;
using eval = SPL.Evaluator;
using gv;


public class SPLEngine : MonoBehaviour
{

    //GlobalVariables.SelectedCodingType 값은 메인 메뉴에서 해당 시나리오 선택시 설정된다.



    private int _screen_width = 1280;
    private int _screen_height = 800;

    //다양한 화면 해상도에서 크기나 위치를 가변적으로 계산하기 위해 비율 변수 선언
    private float _w_ratio = 1.0f;
    private float _h_ratio = 1.0f;
    private float _s_ratio = 1.0f;


    //######################################################################
    //외부로 부터 받을 전역 객체를 선언한다.
    //######################################################################
    //공통적으로 사용할 폰트를 외부로 부터 받는다.
    public Font[] _FONTS;
    public Texture2D[] _TEXTURES;
    public Texture2D[] _BTN_TEXTURES;
    public AudioClip[] _GAME_AUDIO_CLIPS;

    public TextAsset[] _SCRIPT_TEXT_ASSETS;

    public FlareLayer _MAIN_CAMERA_FLARELAYER;
    //######################################################################


    //######################################################################
    //GUI 처리 전역변수
    //######################################################################
    private Rect[] _bg_rects = new Rect[5];
    private Rect[] _button_rects = new Rect[20];
    private GUIStyle[] _text_styles = new GUIStyle[10];
    private GUIStyle[] _btn_styles = new GUIStyle[20];
    private GUIStyle[] _gui_styles = new GUIStyle[10];
    //######################################################################


    //######################################################################
    //기능 처리를 위한 전역 변수
    //######################################################################
    private GameObject _dumy_audio_source;

    //KeyEvent	
    private string _key_down_code = string.Empty;


    //화면 페이이인 아웃을 처리하기 위한 젼역 객체
    //##########################################
    FadeScreenLibrary _fadeScreenLibrary = new FadeScreenLibrary();
    //##########################################



    //######################################################################
    //스크립트 엔진과 관련된 전역 변수
    //######################################################################

    bool _QUIT_REQUEST = false;

    //콘솔 명령어 출력 기능
    List<string> _console_list = new List<string>();
    string _console_lines = "";
    int _console_max_count = 0;
    int _console_line_spacing = 22;
    bool _log_on_flag = true;
    Rect Console_Rect = new Rect(0, 0, 0, 0);


    //스크립트 엔진 코드가 너무 길기 때문에 여러 파일로 분리하여 실행함
    //전처리 단계에서는 아래의 코드로 분리되어 있음
    EnginePreProcessor _enginePreProcessor = null;
    eval.Evaluator g_evaluator = null;
    EngineHelper _engineHelper = null;
    ExecuteCmdLineHelper _executeCmdLineHelper = null;
    ProcedureExecutorHelper _procedureExecutorHelper = null;


    Dictionary<object, object> _procedureTreeInstanceList = new Dictionary<object, object>();
    Dictionary<object, object> _EventProcedureMappingList = new Dictionary<object, object>();

    ArrayList _variableTypeNameList = new ArrayList();
    public Dictionary<object, object> _GlobalVariables = new Dictionary<object, object>();
    ArrayList _ProcedureList = new ArrayList();

    public Dictionary<string, int> _stringToIntMapping = new Dictionary<string, int>();
    Dictionary<object, object> _ExternalInstanceList = new Dictionary<object, object>();

    bool _ProcedureStopState = false;
    bool _StartLoopRunFlag = false;



    //######################################################################
    //아두이노 명령어 처리와 관련된 전역 변수
    //######################################################################

    string _selected_coding_type = string.Empty;

    //외부의 기본 라이트 객체 매핑
    public Light _Directional_Light_Instance = null;

    //LED Spot lights
    public Light[] _LED_Light_Instances = null;

    //각 시나리오별로 외부에 받을 객체 리스트를 저장한다.
    public GameObject[] _ControlObjectList = null;


    //##########################################
    // TrafficSignal 시나리오에만 적용되는 변수들
    //##########################################
    public bool _traffic_signal_red_light = false;
    private List<Vector3> _car_pos_list = new List<Vector3>();
    private List<int> _cross_pos_car_list = new List<int>();
    //##########################################


    //##########################################
    //Main Cemara Control Instance
    //##########################################
    public MainCamControl _MainCamControlInstance = null;
    //##########################################

    private int FramesPerSec;
    private float frequency = 1.0f;
    private string fps;

    //로그패널
    public UnityEngine.UI.Text log;
    public GameObject OpenBtn;
    public GameObject PopUpLogPanel;






    IEnumerator Start()
    {
       
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            Screen.fullScreen = false;
        }


        //이걸 않하면 유니티 프로그램이 뒤로 이동했을 대 실행이 멈춤
        Application.runInBackground = true;


        _screen_width = Screen.width;
        _screen_height = Screen.height;
        _dumy_audio_source = GameObject.Find("sound_dumy");


        //전역 객체들을 초기화 한다.
        SetupWorks();
        SetupMenuButtons();
        SetupTextStyle();


        //##########################################
        Setup_Arduino_Scenario();
        //##########################################


        //##########################################
        //스크립트를 별도의 스레드로 실행시킨다.
        //##########################################
        yield return StartCoroutine(SPLScriptEngineInitialize());



        //화면을 어두운 상태에서 점점 밝게 한다.
        //##########################################
        _fadeScreenLibrary.StartFadeIn(false);
        //##########################################
    }



    void SetupWorks()
    {
        //화면의 비율을 정함
        _w_ratio = Screen.width / 1280.0f;
        _h_ratio = Screen.height / 720.0f;

        //아래 변수는 w_ratio와 h_ratio 중에서 더 작은 값을 저장한다.
        //폰트 크기를 화면 해상도에 따라 변경할 때 사용한다.
        _s_ratio = _w_ratio;

        if (_h_ratio < _w_ratio)
            _s_ratio = _h_ratio;


        float half_width = Screen.width / 2;
        float half_height = Screen.height / 2;


        //##############################################################################
        //버튼 기준값
        //##############################################################################

        float screen_center_x = Screen.width / 2;
        float screen_center_y = Screen.height / 2;

        float btn_width = 240 * _w_ratio;
        float btn_height = 70 * _h_ratio;

        //##############################################################################


        //##########################################################################
        _console_line_spacing = (int)(_console_line_spacing * _h_ratio);
        //##########################################################################



        //전체 배경 화면을 그리기 위한 rect
        _bg_rects[0] = new Rect(0, 0, Screen.width, Screen.height);


        //첫화면 다이얼로그 메세지 표시를 위한 rect
        _bg_rects[1] = new Rect(0, 280 * _h_ratio, Screen.width, half_height);


        //################################################################################
        //버튼 크기 및 위치 설정
        //################################################################################

        //No
        _button_rects[0] = new Rect(0, 0, btn_width, btn_height);
        _button_rects[0].x = screen_center_x - btn_width - (10 * _w_ratio);
        _button_rects[0].y = screen_center_y - (20 * _h_ratio);

        //Yes
        _button_rects[1] = new Rect(0, 0, btn_width, btn_height);
        _button_rects[1].x = screen_center_x + (10 * _w_ratio);
        _button_rects[1].y = screen_center_y - (20 * _h_ratio);



        //################################################################################
        //로그 메세지 영역
        //################################################################################
        Console_Rect.x = 20 * _h_ratio;
        Console_Rect.y = 20 * _h_ratio;
        Console_Rect.width = Screen.width - (40 * _h_ratio);
        Console_Rect.height = Screen.height - (40 * _h_ratio);
        _console_max_count = 26;

    }


    void SetupMenuButtons()
    {
        //투명 버튼
        _btn_styles[0] = new GUIStyle();
        
        /*
        //회색 No 버튼 스타일
        _btn_styles[1] = new GUIStyle();
        _btn_styles[1].normal.background = _BTN_TEXTURES[0];
        _btn_styles[1].hover.background = _BTN_TEXTURES[0];
        _btn_styles[1].active.background = _BTN_TEXTURES[1];
        _btn_styles[1].alignment = TextAnchor.MiddleCenter;
        _btn_styles[1].font = _FONTS[0];
        _btn_styles[1].fontStyle = FontStyle.Bold;
        _btn_styles[1].normal.textColor = new Color(1, 1, 1);
        _btn_styles[1].fontSize = (int)(32 * _w_ratio);


        //오렌지색 Yes 버튼 스타일
        _btn_styles[2] = new GUIStyle();
        _btn_styles[2].normal.background = _BTN_TEXTURES[2];
        _btn_styles[2].hover.background = _BTN_TEXTURES[2];
        _btn_styles[2].active.background = _BTN_TEXTURES[3];
        _btn_styles[2].alignment = TextAnchor.MiddleCenter;
        _btn_styles[2].font = _FONTS[0];
        _btn_styles[2].fontStyle = FontStyle.Bold;
        _btn_styles[2].normal.textColor = new Color(1, 1, 1);
        _btn_styles[2].fontSize = (int)(32 * _w_ratio);
        */
    }



    void SetupTextStyle()
    {
        //Log 메세지
        _text_styles[0] = new GUIStyle();
        _text_styles[0].font = _FONTS[0];
        _text_styles[0].fontStyle = FontStyle.Normal;
        _text_styles[0].normal.textColor = Color.white;
        _text_styles[0].fontSize = (int)(24 * _s_ratio);
        _text_styles[0].alignment = TextAnchor.UpperLeft;
        _text_styles[0].contentOffset = new Vector2(0, 0);
    }

    public void OnClickLogPanel(int Num)
    {
        if (Num == 1)
        {
            OpenBtn.SetActive(false);
            PopUpLogPanel.SetActive(true);
        }
        else
        {
            OpenBtn.SetActive(true);
            PopUpLogPanel.SetActive(false);
        }
    }


    //#####################################################################
    //아두이노 시나리오 Setup
    //#####################################################################

    #region Setup_Arduino_Scenario

    private void Setup_Arduino_Scenario()
    {
        //저장되어 있는 시나리오 이름을 읽어온다.
        //##########################################################################
        _selected_coding_type = GlobalVariables.SelectedCodingType;


        //첫번째 신호등 시나리오에서는 자동차의 위치를 초기화 시킨다.
        if (_selected_coding_type == "TrafficSignal")
        {
            for (int i = 0; i < _ControlObjectList.Length; i++)
            {
                _car_pos_list.Add(new Vector3(_ControlObjectList[i].transform.localPosition.x, _ControlObjectList[i].transform.localPosition.y, _ControlObjectList[i].transform.localPosition.z));
            }
        }
    }

    #endregion Setup_Arduino_Scenario


    //#####################################################################


    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
            _key_down_code = "Escape";

        //화면 해상도가 변경되면 전체 크기 변수들을 다시 계산해 준다.
        if (Screen.width != _screen_width || Screen.height != _screen_height)
        {
            _screen_width = Screen.width;
            _screen_height = Screen.height;

            SetupWorks();
        }

        //##############################################
        //TrafficSignal 시나리오에서 뒷면으로 회전시 신호등이 작동하지 않도록 한다.
        //##############################################
        if (_selected_coding_type == "TrafficSignal")
        {
            if (_MainCamControlInstance != null)
            {
                Vector3 deg = _MainCamControlInstance.GetCameraOrientation();

                if ((deg.y >= 240 && deg.y < 320) && (deg.x <= 20 || deg.x >= 340))
                    _MAIN_CAMERA_FLARELAYER.enabled = false;
                else
                    _MAIN_CAMERA_FLARELAYER.enabled = true;
            }
        }
    }

    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    void OnGUI()
    {

        //###########################################################################################
        //아두이노 명령어 처리
        //###########################################################################################
        DoArduinoCommand();

        //###########################################################################################




        //###########################################################################################
        //로그 메시지를 출력한다.
        //###########################################################################################
        if (_log_on_flag)
        {
            log.text = _console_lines;
            //GUI.Label(Console_Rect, _console_lines, _text_styles[0]);
        }
            
        //###########################################################################################



        //#######################################################################
        //키보드 처리를 한다
        //#######################################################################

        if (_key_down_code == "Escape")
        {
            g_evaluator.StopAllCoroutines();
            _QUIT_REQUEST = true;
            ResetEngine();
            
            //##########################################            
            _fadeScreenLibrary.RequestFadeOut("Close", true);
            //##########################################     

            _key_down_code = "";
        }


        //#######################################################################
        //화면 페이드인 아웃을 그려준다.
        //#######################################################################
        _fadeScreenLibrary.DoFadeGUI();

        //화면 페이드 처리가 끝났으면 그 때 해당 씬이나 화면으로 이동한다,
        string fade_res = _fadeScreenLibrary.CheckFadeOutCompleted();

        //페이드 처리 요청받은 화면의 이름을 비교하여 해당 기능 수행
        if (fade_res == "Close")
        {
            _key_down_code = "";
            UnityEngine.SceneManagement.SceneManager.LoadScene("Scene_CodeEditor");
        }

        
    }



    //###########################################################################################


    void DoArduinoCommand()
    {
        if (_QUIT_REQUEST)
            return;


        //D03 버튼
        if (_selected_coding_type == "TrafficSignal" || _selected_coding_type == "ButtonLight" || _selected_coding_type == "PlaySound" || _selected_coding_type == "SimpleRobotDriving")
        {
            bool event_res = _MainCamControlInstance.GetLeftJoystickPressed();

            if (event_res)
                _GlobalVariables["_DIGITAL_3"] = 1;
            else
                _GlobalVariables["_DIGITAL_3"] = 0;
        }

        //D04 버튼
        if (_selected_coding_type == "TrafficSignal" || _selected_coding_type == "ButtonLight" || _selected_coding_type == "PlaySound" || _selected_coding_type == "SimpleRobotDriving")
        {
            bool event_res = _MainCamControlInstance.GetLeftButtonPressed();

            if (event_res)
                _GlobalVariables["_DIGITAL_4"] = 1;
            else
                _GlobalVariables["_DIGITAL_4"] = 0;
        }

        //D05 버튼
        if (_selected_coding_type == "TrafficSignal" || _selected_coding_type == "ButtonLight" || _selected_coding_type == "PlaySound" || _selected_coding_type == "SimpleRobotDriving")
        {
            bool event_res = _MainCamControlInstance.GetRightButtonPressed();

            if (event_res)
                _GlobalVariables["_DIGITAL_5"] = 1;
            else
                _GlobalVariables["_DIGITAL_5"] = 0;
        }

        //D06 버튼
        if (_selected_coding_type == "TrafficSignal" || _selected_coding_type == "ButtonLight" || _selected_coding_type == "PlaySound" || _selected_coding_type == "SimpleRobotDriving")
        {
            bool event_res = _MainCamControlInstance.GetRightJoystickPressed();

            if (event_res)
                _GlobalVariables["_DIGITAL_6"] = 1;
            else
                _GlobalVariables["_DIGITAL_6"] = 0;
        }


        if (_selected_coding_type == "TrafficSignal" || _selected_coding_type == "ButtonLight")
        {
            //D11 GREEN (PWM)
            int d11 = Util.ToInt(_GlobalVariables["_DIGITAL_11"]);
            int p11 = Util.ToInt(_GlobalVariables["_PWM_11"]);

            if (p11 == 0)
            {
                if (d11 == 0)
                {
                    _LED_Light_Instances[0].intensity = 0;
                }
                else
                {
                    _LED_Light_Instances[0].intensity = 2;
                    _traffic_signal_red_light = false;
                }
            }
            else
            {
                _LED_Light_Instances[0].intensity = p11 * 0.01f;
            }
        }


        if (_selected_coding_type == "TrafficSignal" || _selected_coding_type == "ButtonLight")
        {
            //D12 YELLOW
            int d12 = Util.ToInt(_GlobalVariables["_DIGITAL_12"]);
            if (d12 == 0)
            {
                _LED_Light_Instances[1].intensity = 0;
            }
            else
            {
                if (_selected_coding_type == "ButtonLight")
                {
                    _LED_Light_Instances[1].intensity = 0.5f;
                }
                else
                {
                    _LED_Light_Instances[1].intensity = 2;
                }

                _traffic_signal_red_light = true;
            }
        }


        if (_selected_coding_type == "TrafficSignal" || _selected_coding_type == "ButtonLight")
        {
            //D13 RED
            int d13 = Util.ToInt(_GlobalVariables["_DIGITAL_13"]);
            if (d13 == 0)
            {
                _LED_Light_Instances[2].intensity = 0;
            }
            else
            {
                _LED_Light_Instances[2].intensity = 2;
                _traffic_signal_red_light = true;
            }
        }


        if (_selected_coding_type == "AutoLight")
        {
            //밝기 조절

            bool event_res_left = _MainCamControlInstance.GetLeftButtonPressed();

            int a = Util.ToInt(_GlobalVariables["_ANALOG_0"]);

            if (a == 0)
                _GlobalVariables["_ANALOG_0"] = (int)(_Directional_Light_Instance.intensity / 0.001f);


            if (event_res_left)
            {
                float a0 = _Directional_Light_Instance.intensity / 0.001f;

                a0 = a0 + 5;
                if (a0 > 1023)
                    a0 = 1023;

                _Directional_Light_Instance.intensity = a0 * 0.001f;
                _GlobalVariables["_ANALOG_0"] = (int)a0;
            }



            bool event_res_right = _MainCamControlInstance.GetRightButtonPressed();

            if (event_res_right)
            {
                float a0 = _Directional_Light_Instance.intensity / 0.001f;

                a0 = a0 - 5;
                if (a0 < 0)
                    a0 = 0;

                _Directional_Light_Instance.intensity = a0 * 0.001f;
                _GlobalVariables["_ANALOG_0"] = (int)a0;

            }

        }
        else if (_selected_coding_type == "DetectMovingObject" || _selected_coding_type == "SecurityAlert" || _selected_coding_type == "SecurityAlertVR")
        {
            //거리 센서 방향
            if (_selected_coding_type == "DetectMovingObject")
            {
                _GlobalVariables["_ANALOG_1"] = DistanceSensor.DistanceSensor4;
            }
            else if (_selected_coding_type == "SecurityAlert" || _selected_coding_type == "SecurityAlertVR")
            {
                bool event_res = _MainCamControlInstance.GetLeftButtonPressed();

                if (event_res)
                    _GlobalVariables["_DIGITAL_3"] = 1;
                else
                    _GlobalVariables["_DIGITAL_3"] = 0;

                _GlobalVariables["_ANALOG_1"] = DistanceSensor.DistanceSensor4;
            }
        }
        else if (_selected_coding_type == "AutoParking")
        {
            bool event_res = _MainCamControlInstance.GetLeftJoystickPressed();

            if (event_res)
                _GlobalVariables["_DIGITAL_3"] = 1;
            else
                _GlobalVariables["_DIGITAL_3"] = 0;

            //거리 센서
            _GlobalVariables["_ANALOG_1"] = DistanceSensor.DistanceSensor4;//Rear
            _GlobalVariables["_ANALOG_2"] = DistanceSensor.DistanceSensor5;//Left
        }
        else if (_selected_coding_type == "RearDistanceSensing" || _selected_coding_type == "AvoidObstacle" || _selected_coding_type == "SimpleRobotDriving" || _selected_coding_type == "ReactionRobot")
        {
            if (_selected_coding_type == "RearDistanceSensing")
            {
                bool event_res = _MainCamControlInstance.GetLeftButtonPressed();

                if (event_res)
                    _GlobalVariables["_DIGITAL_3"] = 1;
                else
                    _GlobalVariables["_DIGITAL_3"] = 0;

                //거리 센서
                _GlobalVariables["_ANALOG_1"] = DistanceSensor.DistanceSensor4;//Rear
                _GlobalVariables["_ANALOG_2"] = DistanceSensor.DistanceSensor5;//Front
            }
            else if (_selected_coding_type == "SimpleRobotDriving")
            {
                //거리 센서
                _GlobalVariables["_ANALOG_1"] = DistanceSensor.DistanceSensor4;//Rear
                _GlobalVariables["_ANALOG_2"] = DistanceSensor.DistanceSensor5;//Front
            }
            else if (_selected_coding_type == "AvoidObstacle")
            {
                bool event_res = _MainCamControlInstance.GetLeftButtonPressed();

                if (event_res)
                    _GlobalVariables["_DIGITAL_3"] = 1;
                else
                    _GlobalVariables["_DIGITAL_3"] = 0;

                //거리 센서
                _GlobalVariables["_ANALOG_1"] = DistanceSensor.DistanceSensor4;//Rear
                _GlobalVariables["_ANALOG_2"] = DistanceSensor.DistanceSensor5;//Front
            }
            else if (_selected_coding_type == "ReactionRobot")
            {
                bool event_res = _MainCamControlInstance.GetLeftButtonPressed();

                if (event_res)
                    _GlobalVariables["_DIGITAL_3"] = 1;
                else
                    _GlobalVariables["_DIGITAL_3"] = 0;

                _GlobalVariables["_ANALOG_1"] = DistanceSensor.DistanceSensor5;//Left
                _GlobalVariables["_ANALOG_2"] = DistanceSensor.DistanceSensor4;//front
            }
        }
        else if (_selected_coding_type == "MazeExplorer" )
        {
            //거리 센서
            _GlobalVariables["_ANALOG_1"] = DistanceSensor.DistanceSensor4; //Front
            _GlobalVariables["_ANALOG_2"] = DistanceSensor.DistanceSensor1; //Left
        }
        else if (_selected_coding_type == "DetectEdge" || _selected_coding_type == "LineTracer")
        {
            //거리 센서
            _GlobalVariables["_ANALOG_1"] = DistanceSensor.DistanceSensor4;//FrontR
            _GlobalVariables["_ANALOG_2"] = DistanceSensor.DistanceSensor5;//FrontL
        }


    }

    //###########################################################################################



    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@



    //###################################################################################
    // 아두이노 화면 프린트 명령어
    //###################################################################################



    public void ResetEngine()
    {
        try
        {
            //Start Loop
            _StartLoopRunFlag = false;

            _ProcedureStopState = false;


            //2017.11.20
            //###############################################
            RobotControl.LEFT_POWER = string.Empty;
            RobotControl.RIGHT_POWER = string.Empty;
            //###############################################


            if (_GlobalVariables != null)
                _GlobalVariables.Clear();

            if (_EventProcedureMappingList != null)
                _EventProcedureMappingList.Clear();

            if (_ProcedureList != null)
                _ProcedureList.Clear();


            if (_enginePreProcessor != null && _enginePreProcessor._stringToIntMapping != null)
                _enginePreProcessor._stringToIntMapping.Clear();

            if (g_evaluator != null)
            {
                if (g_evaluator.LocalVariables != null)
                    g_evaluator.LocalVariables.Clear();
            }


            if (_procedureTreeInstanceList != null)
                _procedureTreeInstanceList.Clear();


            foreach (object key in _ExternalInstanceList.Keys)
            {
                string name = key.ToString();

                _GlobalVariables.Add(name, _ExternalInstanceList[name]);
            }


            _console_list.Clear();
            _console_lines = "";


            //Reset Camera
            //if (_MainCamControlInstance != null)
            //    _MainCamControlInstance.ResetMainCamera();
        }
        catch (Exception ex)
        {
            LogInfo("[Reset] " + ex.ToString());
        }
    }



    private IEnumerator SPLScriptEngineInitialize()
    {
        //Debug.Log("[SPLScriptEngineInitialize]");


        //############################################################
        //이곳에서 스크립트 엔진에 필요한 초기화 작업을 수행한다.
        //############################################################


        ResetEngine();


        //##########################################
        //스크립트 실행 엔진 소스 코드를 여러 코드로 분리시켜 실행시킨다.
        //기능별로 분리 시켜 놓음
        //##########################################
        _enginePreProcessor = new EnginePreProcessor();
        _enginePreProcessor.EvalLogInfoHandler = new EnginePreProcessor.LogInfoHandler(this.LogInfo);
        _enginePreProcessor._procedureTreeInstanceList = _procedureTreeInstanceList;
        _enginePreProcessor._EventProcedureMappingList = _EventProcedureMappingList;
        _enginePreProcessor._variableTypeNameList = _variableTypeNameList;
        _enginePreProcessor._GlobalVariables = _GlobalVariables;
        _enginePreProcessor._ProcedureList = _ProcedureList;
        _enginePreProcessor._stringToIntMapping = _stringToIntMapping;

        //##########################################
        _enginePreProcessor.InitializeWorks();
        //##########################################



        //_engineHelper = new EngineHelper();
        _engineHelper = GetComponent<EngineHelper>();
        _engineHelper.EvalLogInfoHandler = new EngineHelper.LogInfoHandler(this.LogInfo);
        _engineHelper._procedureTreeInstanceList = _procedureTreeInstanceList;
        _engineHelper._EventProcedureMappingList = _EventProcedureMappingList;
        _engineHelper._variableTypeNameList = _variableTypeNameList;
        _engineHelper._GlobalVariables = _GlobalVariables;
        _engineHelper._ProcedureList = _ProcedureList;

        _executeCmdLineHelper = new ExecuteCmdLineHelper();
        _executeCmdLineHelper.EvalLogInfoHandler = new ExecuteCmdLineHelper.LogInfoHandler(this.LogInfo);
        _executeCmdLineHelper._procedureTreeInstanceList = _procedureTreeInstanceList;
        _executeCmdLineHelper._EventProcedureMappingList = _EventProcedureMappingList;
        _executeCmdLineHelper._variableTypeNameList = _variableTypeNameList;
        _executeCmdLineHelper._GlobalVariables = _GlobalVariables;
        _executeCmdLineHelper._ProcedureList = _ProcedureList;


        _procedureExecutorHelper = new ProcedureExecutorHelper();
        _procedureExecutorHelper.EvalLogInfoHandler = new ProcedureExecutorHelper.LogInfoHandler(this.LogInfo);
        _procedureExecutorHelper._procedureTreeInstanceList = _procedureTreeInstanceList;
        _procedureExecutorHelper._EventProcedureMappingList = _EventProcedureMappingList;
        _procedureExecutorHelper._variableTypeNameList = _variableTypeNameList;
        _procedureExecutorHelper._GlobalVariables = _GlobalVariables;
        _procedureExecutorHelper._ProcedureList = _ProcedureList;


        //##########################################
        //수식 실행기를 초기화 한다.
        //##########################################
        //g_evaluator = new eval.Evaluator();
        g_evaluator = GetComponent<eval.Evaluator>();
        g_evaluator.GlobalVariables = _GlobalVariables;
        g_evaluator.ProcedureList = _ProcedureList;
        g_evaluator.EvalCallProcedureHandler = new eval.Evaluator.CallProcedureHandler(this.CallProcedure2);
        g_evaluator.EvalLogInfoHandler = new eval.Evaluator.LogInfoHandler(this.LogInfo);
        g_evaluator.EvalUnityRequestHandler = new eval.Evaluator.UnityRequestHandler(this.UnityRequestProc);


        //스크립트를 처리한다.
        yield return StartCoroutine(ExecuteScriptFile());

        yield break;
    }


    private IEnumerator ExecuteScriptFile()
    {
        //Debug.Log("[ExecuteScriptFile]");

     
            string script_text = string.Empty;

            //테스트 시에는 유니티 소스에 저장된 텍스트 파일을 이용한다.
            if (GlobalVariables.DEBUG_MODE)
            {
                //아래에서 2번째 코드로 실제 실행. 첫번째 명령은 예전 명령어
                script_text = _SCRIPT_TEXT_ASSETS[0].text;
                script_text = System.Text.Encoding.Default.GetString(_SCRIPT_TEXT_ASSETS[0].bytes);
            }
            else
            {
                //임시
                string script_file_name = Application.persistentDataPath + "/ArduinoScript.txt";
                //string script_file_name = Application.dataPath + "/ArduinoScript.txt";

                if (File.Exists(script_file_name))
                {
                    script_text = File.ReadAllText(script_file_name);
                }
                else
                    LogInfo("Target script not found!");
            }


            //Debug.Log(script_text);


            ArrayList lines = new ArrayList();
            List<string> CheckRequiredCmdList = _enginePreProcessor.FileToList(script_text, lines);


            foreach (string check_item in CheckRequiredCmdList)
            {
                if (!_GlobalVariables.ContainsKey(check_item))
                    _GlobalVariables.Add(check_item, check_item);
            }



            //X : Container_X 각도
            //Y : Container_Y 각도
            //Z : Center_From_Distace 거리

            //각 시나리오 별로 카메라 위치를 설정한다.
            //카메라를 에니메이션으로 천천히 이동시킨다.
            //if (_selected_coding_type == "TrafficSignal" || _selected_coding_type == "ButtonLight" || _selected_coding_type == "AutoLight" || 
            //    _selected_coding_type == "DetectMoving" ||_selected_coding_type ==  "SecurityAlert" ||
            //    _selected_coding_type == "PlaySound" || _selected_coding_type == "RearSensing" ||
            //    _selected_coding_type == "SimpleDriving" || _selected_coding_type == "SCurveDriving"
            //    )
            _MainCamControlInstance.SetCameraPosWithAnimation(new Vector3(150, 20, -30));



            //###################################################################                
            //Add Engine Start Time for millis function
            DateTime curTime = DateTime.Now;
            _GlobalVariables.Add("_ENGINE_START_TIME_", curTime);
            //###################################################################



            //###################################################################
            //2016.07.04 Added to execute main function
            //###################################################################
            if (CheckRequiredCmdList.Contains("ProcedureMain"))
                lines.Add("call main");
            else if (CheckRequiredCmdList.Contains("ProcedureSetup") && !CheckRequiredCmdList.Contains("ProcedureLoop"))
                lines.Add("call setup");
            else if (CheckRequiredCmdList.Contains("ProcedureSetup") && CheckRequiredCmdList.Contains("ProcedureLoop"))
            {
                lines.Add("call setup");
                lines.Add("while(true)");
                lines.Add("{");
                lines.Add("call loop");
                lines.Add("}");
            }
            //2016.10.03 setup이 없는 경우에는 loop만 실행 추가
            else if (!CheckRequiredCmdList.Contains("ProcedureSetup") && CheckRequiredCmdList.Contains("ProcedureLoop"))
            {
                lines.Add("while(true)");
                lines.Add("{");
                lines.Add("call loop");
                lines.Add("}");
            }
            //###################################################################


            //###################################################################
            //메인 함수 호출을 위한 코드 실행을 시작한다.
            //###################################################################
            yield return StartCoroutine(ParseScript(lines));
            //###################################################################
     
            //LogInfo("[ExecuteScriptFile] " + ex.ToString());
        
    }



    private IEnumerator ParseScript(ArrayList lines)
    {

        //Debug.Log("[ParseScript]");
        //for (int i = 0; i < lines.Count; i++)
        //    Debug.Log(lines[i].ToString());

      
            CommandParsingTree parsedTree = ParsingHelper.GetCommandTreeFromScriptList(lines);

            if (parsedTree != null)
            {
               yield return  StartCoroutine(ExecuteTree(parsedTree));
            }
       

        yield break;
    }



    public IEnumerator ExecuteTree(CommandParsingTree parsedTreeInstance)
    {
        _ProcedureStopState = false;

        if (parsedTreeInstance == null)
        {
            LogInfo("[ExecuteTree] parsedTreeInstance is null");
            yield break;
        }

        yield return StartCoroutine(ExecuteParsedTree(parsedTreeInstance.GetRootNode()));


        //Debug.Log("[Main Ready]");


        CommandItem proc_Item = new CommandItem();
        proc_Item.Name = "maindefault";
        proc_Item.RunMode = "seq";
        proc_Item.FirstToken = "procedure";

        ProcedureReturnItem procReturnItem = new ProcedureReturnItem();
        procReturnItem.IsReturnCalled = false;
        procReturnItem.ProcedureName = proc_Item.Name;

        proc_Item.ProcedureReturn = procReturnItem;

        CommandTree root_node = parsedTreeInstance.GetRootNode();

        //Debug.Log("[ProcedureExecutor Before]");

        yield return StartCoroutine(ProcedureExecutor(root_node, proc_Item, g_evaluator));

        yield break;
    }



    IEnumerator ExecuteParsedTree(CommandTree parsedTree)
    {
        if (parsedTree == null)
            yield break;

        if (parsedTree.Name == null)
            yield break;


        yield return StartCoroutine(ExecuteSingleScript(parsedTree));

        if (parsedTree.Childs != null)
        {
            foreach (CommandTree child in parsedTree.Childs)
            {
                yield return StartCoroutine(ExecuteParsedTree(child));
            }
        }

        yield break;
    }


    IEnumerator ExecuteSingleScript(CommandTree parsedTree)
    {
        parsedTree.Name = ParsingHelper.ReplaceSlashString(parsedTree.Name);

        yield return StartCoroutine(ExecuteCmdLine(parsedTree));

        yield break;
    }



    IEnumerator ExecuteCmdLine(CommandTree parsedTree)
    {
        _executeCmdLineHelper.ExecuteCmdLine(parsedTree);

        yield break;
    }





    //###################################################################################



    //###################################################################################
    // 아두이노 화면 프린트 명령어
    //###################################################################################
    public void LogPrint(string msg)
    {
        string[] lines = msg.Split(new char[] { '\n' });

        if (lines == null)
            return;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            if (!string.IsNullOrEmpty(line))
            {
                if (i == 0)
                {
                    if (_console_list.Count > 0)
                    {
                        _console_list[_console_list.Count - 1] = _console_list[_console_list.Count - 1] + line;
                    }
                    else
                    {
                        _console_list.Add(line);
                    }
                }
                else
                {
                    _console_list.Add(line);
                }
            }
            else
            {
                if (i == 0)
                {
                    if (_console_list.Count == 0)
                    {
                        _console_list.Add("");
                    }
                }
                else
                {
                    _console_list.Add("");
                }
            }
        }

        while (_console_list.Count > _console_max_count)
            _console_list.RemoveAt(0);

        _console_lines = "";

        for (int i = 0; i < _console_list.Count; i++)
            _console_lines += _console_list[i] + System.Environment.NewLine;
    }


    public void LogPrintLine(string msg)
    {
        string[] lines = msg.Split(new char[] { '\n' });

        if (lines == null)
            return;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            if (!string.IsNullOrEmpty(line))
            {
                if (_console_list.Count > 0)
                {
                    _console_list[_console_list.Count - 1] = _console_list[_console_list.Count - 1] + line;
                }
                else
                {
                    _console_list.Add(line);
                }

                _console_list.Add("");
            }
            else
            {
                _console_list.Add("");
            }
        }

        while (_console_list.Count > _console_max_count)
            _console_list.RemoveAt(0);

        _console_lines = "";

        for (int i = 0; i < _console_list.Count; i++)
            _console_lines += _console_list[i] + System.Environment.NewLine;
    }



    public void LogInfo(string msg)
    {
        LogInfo(msg, true);
    }

    public void LogInfo(string msg, bool new_line_flag)
    {
        string[] lines = msg.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);


        foreach (string line in lines)
        {
            if (!string.IsNullOrEmpty(line))
            {
                string line2 = line;

                //100문자씩 잘라서 표시한다.
                while (line2.Length > 100)
                {
                    if (new_line_flag || _console_list.Count == 0)
                        _console_list.Add(line2.Substring(0, 100));
                    else
                    {
                        _console_list[_console_list.Count - 1] = _console_list[_console_list.Count - 1] + line2.Substring(0, 100);
                    }

                    line2 = line2.Substring(100);
                }

                if (new_line_flag || _console_list.Count == 0)
                    _console_list.Add(line2);
                else
                {
                    _console_list[_console_list.Count - 1] = _console_list[_console_list.Count - 1] + line2;
                }
            }
        }


        while (_console_list.Count > _console_max_count)
            _console_list.RemoveAt(0);

        _console_lines = "";

        for (int i = 0; i < _console_list.Count; i++)
            _console_lines += _console_list[i] + System.Environment.NewLine;
    }

    //###################################################################################




    //###################################################################################
    //함수 호출과 관련된 모듈 모음
    //###################################################################################


    //Called from Eval Delegation
    public object UnityRequestProc(GameObject target_object, string cmdName, string paramStr)
    {
        object res = null;

        //Do something

        return res;
    }



    public object CallProcedure(string procedureName, string paramStr, Dictionary<object, object> localVariables)
    {
        CommandTree procItem_Tree = _enginePreProcessor.SearchProcedureWithName(procedureName);

        object res = null;

        try
        {

            if (procItem_Tree != null)
            {
                CommandItem procItem_Clone = procItem_Tree.commandItem.Clone();

                ProcedureReturnItem procReturnItem = new ProcedureReturnItem();
                procReturnItem.IsReturnCalled = false;
                procReturnItem.ProcedureName = procItem_Clone.Name;
                procItem_Clone.ProcedureReturn = procReturnItem;

                procItem_Clone.ProcedureCallDataList.Clear();


                bool openFlag = false;
                string tokenStr = string.Empty;

                for (int pInd = 0; pInd < paramStr.Length; pInd++)
                {
                    if (paramStr[pInd] != ',' && paramStr[pInd] != '"')
                    {
                        tokenStr += paramStr[pInd];
                    }
                    else if (paramStr[pInd] == '"' && openFlag)
                    {
                        tokenStr += paramStr[pInd];
                        openFlag = false;
                    }
                    else if (paramStr[pInd] == '"' && !openFlag)
                    {
                        tokenStr += paramStr[pInd];
                        openFlag = true;
                    }
                    else if (paramStr[pInd] == ',' && openFlag)
                    {
                        tokenStr += paramStr[pInd];
                    }
                    else if (paramStr[pInd] == ',' && !openFlag)
                    {
                        tokenStr = tokenStr.Trim();

                        if (tokenStr != string.Empty)
                        {
                            procItem_Clone.ProcedureCallDataList.Add(tokenStr);

                            tokenStr = string.Empty;
                        }
                    }
                }

                tokenStr = tokenStr.Trim();

                if (tokenStr != string.Empty)
                {
                    procItem_Clone.ProcedureCallDataList.Add(tokenStr);

                    tokenStr = string.Empty;
                }


                for (int pInd = 0; pInd < procItem_Clone.ProcedureCallDataList.Count; pInd++)
                {
                    object resObj = _engineHelper.ExcuteConditionExprWithLocal(procItem_Clone.ProcedureCallDataList[pInd].ToString(), localVariables, g_evaluator);
                    procItem_Clone.ProcedureCallObjectList.Add(resObj);
                }

                

                StartCoroutine(ProcedureExecutor(procItem_Tree, procItem_Clone, g_evaluator));

                //ProcedureExecutorNotCo(procItem_Tree, procItem_Clone, g_evaluator);
                //2016.09.25 주석 오류 해제
                if (_GlobalVariables.ContainsKey(procedureName + "_return_value"))
                {
                    res = _engineHelper.ExcuteConditionExprWithLocal(procedureName + "_return_value", localVariables, g_evaluator);


                    //System.change
                    if (procItem_Clone.Target == "string")
                    {
                        if (res is string)
                        {
                            Debug.Log("형변환 : " + res);
                        }
                        else
                        {
                            Debug.Log("형변환실패");
                        }
                    }

                    Type t = res.GetType();


                    
                }
                    
            }
            else
            {
                //LogInfo("[CallProcedure] " + "Procedure " + procedureName + " is not exist.");

                if (procedureName != "main")
                    LogInfo("Error CP: \"" + procedureName + "\" function not exist");
            }
        }
        catch (Exception ex)
        {
            LogInfo("[CallProcedure] " + ex.ToString());
        }

        return res;
    }



    public IEnumerator CallProcedure2(string procedureName, string paramStr, Dictionary<object, object> localVariables)
    {
        CommandTree procItem_Tree = _enginePreProcessor.SearchProcedureWithName(procedureName);

        object res = null;


        if (procItem_Tree != null)
        {
            CommandItem procItem_Clone = procItem_Tree.commandItem.Clone();

            ProcedureReturnItem procReturnItem = new ProcedureReturnItem();
            procReturnItem.IsReturnCalled = false;
            procReturnItem.ProcedureName = procItem_Clone.Name;
            procItem_Clone.ProcedureReturn = procReturnItem;

            procItem_Clone.ProcedureCallDataList.Clear();


            bool openFlag = false;
            string tokenStr = string.Empty;

            for (int pInd = 0; pInd < paramStr.Length; pInd++)
            {
                if (paramStr[pInd] != ',' && paramStr[pInd] != '"')
                {
                    tokenStr += paramStr[pInd];
                }
                else if (paramStr[pInd] == '"' && openFlag)
                {
                    tokenStr += paramStr[pInd];
                    openFlag = false;
                }
                else if (paramStr[pInd] == '"' && !openFlag)
                {
                    tokenStr += paramStr[pInd];
                    openFlag = true;
                }
                else if (paramStr[pInd] == ',' && openFlag)
                {
                    tokenStr += paramStr[pInd];
                }
                else if (paramStr[pInd] == ',' && !openFlag)
                {
                    tokenStr = tokenStr.Trim();

                    if (tokenStr != string.Empty)
                    {
                        procItem_Clone.ProcedureCallDataList.Add(tokenStr);

                        tokenStr = string.Empty;
                    }
                }
            }

            tokenStr = tokenStr.Trim();

            if (tokenStr != string.Empty)
            {
                procItem_Clone.ProcedureCallDataList.Add(tokenStr);

                tokenStr = string.Empty;
            }


            for (int pInd = 0; pInd < procItem_Clone.ProcedureCallDataList.Count; pInd++)
            {
                object resObj = _engineHelper.ExcuteConditionExprWithLocal(procItem_Clone.ProcedureCallDataList[pInd].ToString(), localVariables, g_evaluator);
                procItem_Clone.ProcedureCallObjectList.Add(resObj);
            }

            yield return StartCoroutine(ProcedureExecutor(procItem_Tree, procItem_Clone, g_evaluator));

            //ProcedureExecutorNotCo(procItem_Tree, procItem_Clone, g_evaluator);
            //2016.09.25 주석 오류 해제
            if (_GlobalVariables.ContainsKey(procedureName + "_return_value"))
            {
                //res = _engineHelper.ExcuteConditionExprWithLocal(procedureName + "_return_value", localVariables, g_evaluator);
                yield return StartCoroutine(_engineHelper.ExcuteConditionExprWithLocal(procedureName + "_return_value", localVariables, g_evaluator));
                res = gv.GlobalVariables.res;

                //System.change
                if (procItem_Clone.Target == "string")
                {
                    if (res is string)
                    {
                        Debug.Log("형변환 : " + res);
                    }
                    else
                    {
                        Debug.Log("형변환실패");
                    }
                }

                Type t = res.GetType();



            }

        }
        else
        {
            //LogInfo("[CallProcedure] " + "Procedure " + procedureName + " is not exist.");

            if (procedureName != "main")
                LogInfo("Error CP: \"" + procedureName + "\" function not exist");
        }
    }
      
        //return res;
    



    public void CallAssignedProcedureFromId(string id, object state, bool cuncurMode)
    {
        try
        {
            string proc_name = _enginePreProcessor.GetProcedureName(id);

            if (proc_name == string.Empty)
                return;

            StartCoroutine(CallAssignedProcedure(proc_name, state, "value", cuncurMode));
        }
        catch (Exception ex)
        {
            LogInfo("[CallAssignedProcedureFromId] " + ex.ToString());
        }
    }


    public IEnumerator CallAssignedProcedure(string procedureName, object newInstance, string alternativeVariableName, bool cuncurMode)
    {
        yield return StartCoroutine(CallAssignedProcedure(procedureName, newInstance, alternativeVariableName, cuncurMode, "", null, "", null, "", null, "", null));
    }

    public IEnumerator CallAssignedProcedure(string procedureName, object newInstance, string alternativeVariableName, bool cuncurMode, string var_name1, object var_value1)
    {
        yield return StartCoroutine(CallAssignedProcedure(procedureName, newInstance, alternativeVariableName, cuncurMode, var_name1, var_value1, "", null, "", null, "", null));
    }

    public IEnumerator CallAssignedProcedure(string procedureName, object newInstance, string alternativeVariableName, bool cuncurMode, string var_name1, object var_value1, string var_name2, object var_value2)
    {
        yield return StartCoroutine(CallAssignedProcedure(procedureName, newInstance, alternativeVariableName, cuncurMode, var_name1, var_value1, var_name2, var_value2, "", null, "", null));
    }

    public IEnumerator CallAssignedProcedure(string procedureName, object newInstance, string alternativeVariableName, bool cuncurMode, string var_name1, object var_value1, string var_name2, object var_value2, string var_name3, object var_value3)
    {
        yield return StartCoroutine(CallAssignedProcedure(procedureName, newInstance, alternativeVariableName, cuncurMode, var_name1, var_value1, var_name2, var_value2, var_name3, var_value3, "", null));
    }

    public IEnumerator CallAssignedProcedure(string procedureName, object newInstance, string alternativeVariableName, bool cuncurMode, string var_name1, object var_value1, string var_name2, object var_value2, string var_name3, object var_value3, string var_name4, object var_value4)
    {

        if (procedureName == string.Empty)
        {
            LogInfo("[CallAssignedProcedure] Procedure name is empty");
            yield break;
        }


        CommandTree procItem_Tree = _enginePreProcessor.SearchProcedureWithName(procedureName);

        if (procItem_Tree == null)
        {
            //LogInfo("[CallAssignedProcedure] Can't find procedure from the list -> " + procedureName);
            if (procedureName != "main")
                LogInfo("Error AP: \"" + procedureName + "\" function not exist");

            yield break;
        }


        CommandItem procItem_Clone = procItem_Tree.commandItem.Clone();

        ProcedureReturnItem procReturnItem = new ProcedureReturnItem();
        procReturnItem.IsReturnCalled = false;
        procReturnItem.ProcedureName = procItem_Clone.Name;

        procItem_Clone.ProcedureReturn = procReturnItem;


        Dictionary<object, object> newLocalVariables = new Dictionary<object, object>();

        if (alternativeVariableName != string.Empty)
            newLocalVariables.Add(alternativeVariableName, newInstance);

        if (!string.IsNullOrEmpty(var_name1))
            newLocalVariables.Add(var_name1, var_value1);

        if (!string.IsNullOrEmpty(var_name2))
            newLocalVariables.Add(var_name2, var_value2);

        if (!string.IsNullOrEmpty(var_name3))
            newLocalVariables.Add(var_name3, var_value3);

        if (!string.IsNullOrEmpty(var_name4))
            newLocalVariables.Add(var_name4, var_value4);

        procItem_Clone.LocalVariables = newLocalVariables;


        if (cuncurMode)
        {
            eval.Evaluator evaluator = new eval.Evaluator();

            evaluator.GlobalVariables = _GlobalVariables;
            evaluator.ProcedureList = _ProcedureList;
            evaluator.EvalCallProcedureHandler = new eval.Evaluator.CallProcedureHandler(this.CallProcedure);
            evaluator.EvalLogInfoHandler = new eval.Evaluator.LogInfoHandler(this.LogInfo);
            evaluator.EvalUnityRequestHandler = new eval.Evaluator.UnityRequestHandler(this.UnityRequestProc);

            StartCoroutine(ProcedureExecutor(procItem_Tree, procItem_Clone, evaluator));
        }
        else
        {
            yield return StartCoroutine(ProcedureExecutor(procItem_Tree, procItem_Clone, g_evaluator));
        }

        yield break;
    }



    //###################################################################################



    //###################################################################################
    // 프로시져 실행 모듈
    //###################################################################################


    private IEnumerator ProcedureExecutorLoop(CommandTree parsedTree, CommandItem procItem, eval.Evaluator _evaluator)
    {
        while (_StartLoopRunFlag)
        {
            //Call sequential
            yield return StartCoroutine(ProcedureExecutor(parsedTree, procItem, _evaluator));
        }

        yield break;
    }

  

    //private void ProcedureExecutorNotCo(CommandTree parsedTree, CommandItem procItem, eval.Evaluator _evaluator)
    //{
    //    if (_ProcedureStopState)
    //    {
    //        return;
    //    }


    //    ArrayList tempLocalVariableList = new ArrayList();

    //    bool ifConditionResult = true;
    //    bool CaseRunFlag = false;


    //    #region Pre processing


    //    #region if (procItem.FirstToken == "procedure")

    //    if (procItem.FirstToken == "procedure")
    //    {
    //        _procedureExecutorHelper.Proc_CopyVariables(parsedTree, procItem, _evaluator, tempLocalVariableList);
    //    }

    //    #endregion if (procItem.FirstToken == "procedure")


    //    if (procItem == null)
    //    {
    //        return;
    //    }

    //    #endregion


    //    #region foreach


    //    foreach (CommandTree child in parsedTree.Childs)
    //    {

    //        if (_ProcedureStopState)
    //        {

    //            foreach (string key in tempLocalVariableList)
    //            {
    //                try
    //                {
    //                    procItem.LocalVariables.Remove(key);
    //                }
    //                catch (Exception ex)
    //                {
    //                    LogInfo("[ProcedureExecutor] " + ex.ToString());
    //                }
    //            }

    //            break;
    //        }


    //        #region foreach

    //        _procedureExecutorHelper.Proc_ContineAndReturn(parsedTree, procItem, _evaluator, tempLocalVariableList);

    //        #endregion foreach


    //        if (child.commandItem != null)
    //        {
    //            #region foreach inner

    //            bool doFlag = false;


    //            if (procItem.LocalVariables.ContainsKey(child.FirstTokenOfLine))
    //                doFlag = true;

    //            if (child.IsSetVariable)
    //                doFlag = true;


    //            if (child.IsExecutableTargetType && child.FirstTokenOfLine != "procedure" && child.FirstTokenOfLine != "end")
    //                doFlag = true;


    //            if (doFlag)
    //            {

    //                #region doFlag

    //                CommandItem cmdItem = child.commandItem;


    //                //##################################################################################################################################
    //                //LogInfo("[foreach inner] " + cmdItem.FirstToken + " / " + cmdItem.Action + " / " + cmdItem.Target + " / " + cmdItem.TargetType);
    //                Debug.Log("[foreach inner] " + cmdItem.FirstToken + " / " + cmdItem.Action + " / " + cmdItem.Target + " / " + cmdItem.TargetType);
    //                //##################################################################################################################################


    //                if (cmdItem.TargetType == "arduino_command")
    //                {
    //                    #region arduino_command

    //                    //LogInfo("[arduino_command] " + cmdItem.FirstToken + " / " + cmdItem.Action + " / " + cmdItem.Target);
    //                    Debug.Log("[arduino_command] " + cmdItem.FirstToken + " / " + cmdItem.Action + " / " + cmdItem.Target);

    //                    CommandItem cmdItem_Clone = cmdItem.Clone();

    //                    cmdItem_Clone.Target = _engineHelper.ReplaceWithVariables(procItem.LocalVariables, cmdItem_Clone.Target, _evaluator);

    //                    if (cmdItem.FirstToken == "digitalwrite" && !string.IsNullOrEmpty(cmdItem_Clone.Target))
    //                    {
    //                        ArrayList str_list = SPLHelper.GetStringList(cmdItem_Clone.Target);

    //                        if (str_list != null && str_list.Count == 2)
    //                        {
    //                            object eval_res1 = _engineHelper.ExcuteConditionExprWithLocal(str_list[0].ToString(), procItem.LocalVariables, _evaluator);
    //                            object eval_res2 = _engineHelper.ExcuteConditionExprWithLocal(str_list[1].ToString(), procItem.LocalVariables, _evaluator);


    //                            if (eval_res1 != null && eval_res2 != null)
    //                            {
    //                                int pin = Util.ToInt(eval_res1);
    //                                int v = Util.ToInt(eval_res2);

    //                                _GlobalVariables["_DIGITAL_" + pin.ToString()] = v;
    //                            }
    //                        }
    //                    }
    //                    else if (cmdItem.FirstToken == "analogwrite" && !string.IsNullOrEmpty(cmdItem_Clone.Target))
    //                    {
    //                        ArrayList str_list = SPLHelper.GetStringList(cmdItem_Clone.Target);

    //                        if (str_list != null && str_list.Count == 2)
    //                        {
    //                            object eval_res1 = _engineHelper.ExcuteConditionExprWithLocal(str_list[0].ToString(), procItem.LocalVariables, _evaluator);
    //                            object eval_res2 = _engineHelper.ExcuteConditionExprWithLocal(str_list[1].ToString(), procItem.LocalVariables, _evaluator);


    //                            if (eval_res1 != null && eval_res2 != null)
    //                            {
    //                                int pin = Util.ToInt(eval_res1);
    //                                int v = Util.ToInt(eval_res2);

    //                                if (v < 0)
    //                                    v = 0;
    //                                else if (v > 255)
    //                                    v = 255;

    //                                //PWM not analog
    //                                _GlobalVariables["_PWM_" + pin.ToString()] = v;
    //                            }
    //                        }
    //                    }
    //                    else if (cmdItem.FirstToken == "tone" && !string.IsNullOrEmpty(cmdItem_Clone.Target))
    //                    {
    //                        ArrayList str_list = SPLHelper.GetStringList(cmdItem_Clone.Target);

    //                        if (str_list != null && str_list.Count == 3)
    //                        {
    //                            object eval_res1 = _engineHelper.ExcuteConditionExprWithLocal(str_list[0].ToString(), procItem.LocalVariables, _evaluator);
    //                            object eval_res2 = _engineHelper.ExcuteConditionExprWithLocal(str_list[1].ToString(), procItem.LocalVariables, _evaluator);
    //                            object eval_res3 = _engineHelper.ExcuteConditionExprWithLocal(str_list[2].ToString(), procItem.LocalVariables, _evaluator);


    //                            if (eval_res1 != null && eval_res2 != null && eval_res3 != null)
    //                            {
    //                                int pin = Util.ToInt(eval_res1);
    //                                int fr = Util.ToInt(eval_res2);
    //                                float du = Util.ToFloat(eval_res3);

    //                                //PlayOrGenerate(PCSpeakerEffects.Tone(fr, speaker.sampleRate), du * 0.001f);
    //                                if (pin == 2)
    //                                {
    //                                    Tone_Internal(fr, du * 0.001f);
    //                                }

    //                            }
    //                        }
    //                    }
    //                    else if (cmdItem.FirstToken == "servowrite" && !string.IsNullOrEmpty(cmdItem_Clone.Target))
    //                    {
    //                        ArrayList str_list = SPLHelper.GetStringList(cmdItem_Clone.Target);

    //                        if (str_list != null && str_list.Count == 2)
    //                        {
    //                            object eval_res1 = _engineHelper.ExcuteConditionExprWithLocal(str_list[0].ToString(), procItem.LocalVariables, _evaluator);
    //                            object eval_res2 = _engineHelper.ExcuteConditionExprWithLocal(str_list[1].ToString(), procItem.LocalVariables, _evaluator);


    //                            if (eval_res1 != null && eval_res2 != null)
    //                            {
    //                                int pin = Util.ToInt(eval_res1);
    //                                int v = Util.ToInt(eval_res2);

    //                                if (v < 0)
    //                                    v = 0;

    //                                if (v > 180)
    //                                    v = 180;

    //                                _GlobalVariables["_SERVO_" + pin.ToString()] = v;

    //                                //if (Bar_Ring_Object != null)
    //                                //{
    //                                //    Bar_Ring_Object.transform.localEulerAngles = new Vector3(0, 0, -v);
    //                                //}
    //                            }
    //                        }
    //                    }
    //                    else if (cmdItem.FirstToken == "drivewrite" && !string.IsNullOrEmpty(cmdItem_Clone.Target))
    //                    {
    //                        ArrayList str_list = SPLHelper.GetStringList(cmdItem_Clone.Target);

    //                        if (str_list != null && str_list.Count == 2)
    //                        {
    //                            object eval_res1 = _engineHelper.ExcuteConditionExprWithLocal(str_list[0].ToString(), procItem.LocalVariables, _evaluator);
    //                            object eval_res2 = _engineHelper.ExcuteConditionExprWithLocal(str_list[1].ToString(), procItem.LocalVariables, _evaluator);


    //                            if (eval_res1 != null && eval_res2 != null)
    //                            {
    //                                int left = Util.ToInt(eval_res1);
    //                                int right = Util.ToInt(eval_res2);

    //                                _GlobalVariables["_MOTOR1_POWER"] = left;
    //                                _GlobalVariables["_MOTOR2_POWER"] = right;

    //                                RobotControl.LEFT_POWER = left.ToString();
    //                                RobotControl.RIGHT_POWER = right.ToString();
    //                            }
    //                        }
    //                    }
    //                    else if (cmdItem.FirstToken == "motor1write" && !string.IsNullOrEmpty(cmdItem_Clone.Target))
    //                    {
    //                        ArrayList str_list = SPLHelper.GetStringList(cmdItem_Clone.Target);

    //                        if (str_list != null && str_list.Count == 1)
    //                        {
    //                            object eval_res1 = _engineHelper.ExcuteConditionExprWithLocal(str_list[0].ToString(), procItem.LocalVariables, _evaluator);


    //                            if (eval_res1 != null)
    //                            {
    //                                int power = Util.ToInt(eval_res1);

    //                                _GlobalVariables["_MOTOR1_POWER"] = power;
    //                                RobotControl.LEFT_POWER = power.ToString();
    //                            }
    //                        }
    //                    }
    //                    else if (cmdItem.FirstToken == "motor2write" && !string.IsNullOrEmpty(cmdItem_Clone.Target))
    //                    {
    //                        ArrayList str_list = SPLHelper.GetStringList(cmdItem_Clone.Target);

    //                        if (str_list != null && str_list.Count == 1)
    //                        {
    //                            object eval_res1 = _engineHelper.ExcuteConditionExprWithLocal(str_list[0].ToString(), procItem.LocalVariables, _evaluator);


    //                            if (eval_res1 != null)
    //                            {
    //                                int power = Util.ToInt(eval_res1);

    //                                _GlobalVariables["_MOTOR2_POWER"] = power;
    //                                RobotControl.RIGHT_POWER = power.ToString();
    //                            }
    //                        }
    //                    }

    //                    #endregion arduino_command
    //                }
    //                else if (cmdItem.TargetType == "delay")
    //                {
    //                    #region delay

    //                    CommandItem cmdItem_Clone = cmdItem.Clone();

    //                    cmdItem_Clone.Target = _engineHelper.ReplaceWithVariables(procItem.LocalVariables, cmdItem_Clone.Target, _evaluator);


    //                    object eval_res = _engineHelper.ExcuteConditionExprWithLocal(cmdItem_Clone.Target, procItem.LocalVariables, _evaluator);

    //                    if (eval_res != null)
    //                    {
    //                        cmdItem_Clone.Target = eval_res.ToString();

    //                        int delaytime = 0;

    //                        try
    //                        {
    //                            delaytime = int.Parse(cmdItem_Clone.Target);
    //                        }
    //                        catch (Exception ex)
    //                        {
    //                            LogInfo("[ProcedureExecutor] " + ex.ToString());
    //                        }

    //                        if (delaytime > 0)
    //                        {

    //                            //Invoke("", delaytime / 1000.0f);
    //                            // yield return new WaitForSeconds(delaytime / 1000.0f);

    //                            CheckTimer = true;
    //                            CheckRunning = true;

    //                            while (CheckTimer)
    //                            {
    //                                if (CheckRunning)
    //                                {
    //                                    StartCoroutine(TimerInFunc(delaytime));
    //                                    CheckRunning = false;
    //                                }
                                    
    //                            }


    //                            //System.Threading.Timer tm = new System.Threading.Timer();
    //                            //tm.


    //                        }

    //                    }
    //                    else
    //                    {
    //                        LogInfo("[ProcedureExecutor] " + "Delay data is null.");
    //                    }

    //                    #endregion
    //                }
    //                else if (cmdItem.TargetType == "defvariable")
    //                {
    //                    #region defvariable

    //                    CommandItem cmdItem_Clone = cmdItem.Clone();

    //                    LocalVariableItem localVariableItem = new LocalVariableItem();

    //                    localVariableItem.LocalVariables = procItem.LocalVariables;
    //                    localVariableItem.TempLocalVariableList = tempLocalVariableList;

    //                    //LogInfo("[defvariable] " + cmdItem_Clone.FirstToken + " / " + cmdItem_Clone.Action + " / " + cmdItem_Clone.Target);

    //                    _engineHelper.ParseDefVariableLineWithLocal(cmdItem_Clone.Target, cmdItem_Clone.Action, localVariableItem, procItem.Name, _evaluator);

    //                    #endregion
    //                }
    //                else if (cmdItem.TargetType == "setvariable")
    //                {
    //                    #region setvariable

    //                    CommandItem cmdItem_Clone = cmdItem.Clone();

    //                    if (cmdItem_Clone.Action.IndexOf("{") >= 0)
    //                    {
    //                        cmdItem_Clone.Action = _engineHelper.ReplaceWithVariables(procItem.LocalVariables, cmdItem_Clone.Action, _evaluator);
    //                    }

    //                    //LogInfo("[setvariable] " + cmdItem_Clone.FirstToken + " / " + cmdItem_Clone.Action + " / " + cmdItem_Clone.Target);

    //                    _engineHelper.ParseSetVariableLineWithLocal(cmdItem_Clone.Action, procItem.LocalVariables, procItem.Name, _evaluator, procItem.Target);



    //                    if (cmdItem.Target == "return")
    //                    {
    //                        if (procItem.ProcedureReturn != null)
    //                            procItem.ProcedureReturn.IsReturnCalled = true;

    //                        break;
    //                    }

    //                    #endregion
    //                }
    //                else if (cmdItem.TargetType == "break")
    //                {
    //                    #region break

    //                    if (procItem.IsBreakContinueCalled.Count > 0)
    //                    {
    //                        if (procItem.Name == "if" || procItem.Name == "else" || procItem.Name == "for" || procItem.Name == "while" || procItem.Name == "case" || procItem.Name == "default")
    //                        {
    //                            BreakContinueItem breakItem = (BreakContinueItem)procItem.IsBreakContinueCalled.Peek();
    //                            breakItem.IsBreakCalled = true;
    //                        }
    //                    }

    //                    break;

    //                    #endregion
    //                }
    //                else if (cmdItem.TargetType == "continue")
    //                {
    //                    #region break

    //                    if (procItem.IsBreakContinueCalled.Count > 0)
    //                    {
    //                        if (procItem.Name == "if" || procItem.Name == "else" || procItem.Name == "for" || procItem.Name == "while")
    //                        {
    //                            BreakContinueItem breakItem = (BreakContinueItem)procItem.IsBreakContinueCalled.Peek();
    //                            breakItem.IsContinueCalled = true;
    //                        }
    //                    }

    //                    break;

    //                    #endregion
    //                }
    //                else if (cmdItem.TargetType == "if")
    //                {
    //                    #region if

    //                    CommandItem cmdItem_Clone = cmdItem.Clone();

    //                    cmdItem_Clone.LocalVariables = procItem.LocalVariables;
    //                    cmdItem_Clone.IsBreakContinueCalled = procItem.IsBreakContinueCalled;
    //                    cmdItem_Clone.ProcedureReturn = procItem.ProcedureReturn;
    //                    cmdItem_Clone.ProcedureRunningInfo = procItem.ProcedureRunningInfo;
    //                    cmdItem_Clone.RunMode = "seq";


    //                    object eval_res = _engineHelper.CheckConditionExprWithLocal(cmdItem_Clone.ConditionExpr, procItem.LocalVariables, _evaluator);

    //                    if (eval_res != null)
    //                    {
    //                        ifConditionResult = Util.ToBool(eval_res);

    //                        cmdItem_Clone.Name = "if";

    //                        if (ifConditionResult)
    //                        {
    //                            //yield return StartCoroutine(ProcedureExecutor(child, cmdItem_Clone, _evaluator));\
    //                            ProcedureExecutorNotCo(child, cmdItem_Clone, _evaluator);
    //                        }
    //                    }
    //                    else
    //                    {
    //                        LogInfo("[ProcedureExecutor] " + "If's condition expr is not boolean type. / " + cmdItem_Clone.ConditionExpr);
    //                    }

    //                    #endregion
    //                }
    //                else if (cmdItem.TargetType == "else")
    //                {
    //                    #region else

    //                    CommandItem cmdItem_Clone = cmdItem.Clone();

    //                    cmdItem_Clone.LocalVariables = procItem.LocalVariables;
    //                    cmdItem_Clone.IsBreakContinueCalled = procItem.IsBreakContinueCalled;
    //                    cmdItem_Clone.ProcedureReturn = procItem.ProcedureReturn;
    //                    cmdItem_Clone.ProcedureRunningInfo = procItem.ProcedureRunningInfo;
    //                    cmdItem_Clone.RunMode = "seq";

    //                    cmdItem_Clone.Name = "else";

    //                    if (!ifConditionResult)
    //                    {
    //                        //yield return StartCoroutine(ProcedureExecutor(child, cmdItem_Clone, _evaluator));
    //                        ProcedureExecutorNotCo(child, cmdItem_Clone, _evaluator);
    //                    }

    //                    ifConditionResult = true;

    //                    #endregion
    //                }
    //                else if (cmdItem.TargetType == "while")
    //                {
    //                    #region while

    //                    CommandItem cmdItem_Clone = cmdItem.Clone();

    //                    cmdItem_Clone.LocalVariables = procItem.LocalVariables;
    //                    cmdItem_Clone.IsBreakContinueCalled = procItem.IsBreakContinueCalled;
    //                    cmdItem_Clone.ProcedureReturn = procItem.ProcedureReturn;
    //                    cmdItem_Clone.ProcedureRunningInfo = procItem.ProcedureRunningInfo;
    //                    cmdItem_Clone.RunMode = "seq";


    //                    BreakContinueItem breakItem = new BreakContinueItem();
    //                    breakItem.IsBreakCalled = false;
    //                    breakItem.IsContinueCalled = false;

    //                    procItem.IsBreakContinueCalled.Push(breakItem);

    //                    cmdItem_Clone.Name = "while";


    //                    bool whileDoFlag = true;

    //                    object eval_res = _engineHelper.CheckConditionExprWithLocal(cmdItem_Clone.ConditionExpr, procItem.LocalVariables, _evaluator);

    //                    if (eval_res == null)
    //                    {
    //                        whileDoFlag = false;
    //                        LogInfo("[ProcedureExecutor] " + "While's condition expr is null. / " + cmdItem_Clone.ConditionExpr);
    //                    }

    //                    while (Util.ToBool(eval_res) && whileDoFlag)
    //                    {

    //                        if (_ProcedureStopState)
    //                        {
    //                            try
    //                            {
    //                                foreach (string key in tempLocalVariableList)
    //                                {
    //                                    try
    //                                    {
    //                                        procItem.LocalVariables.Remove(key);
    //                                    }
    //                                    catch { }
    //                                }
    //                            }
    //                            catch { }

    //                            break;
    //                        }


    //                        ProcedureExecutorNotCo(child, cmdItem_Clone, _evaluator);

    //                        if (breakItem.IsBreakCalled)
    //                        {
    //                            breakItem.IsBreakCalled = false;
    //                            break;
    //                        }

    //                        if (breakItem.IsContinueCalled)
    //                        {
    //                            breakItem.IsContinueCalled = false;
    //                        }


    //                        eval_res = _engineHelper.CheckConditionExprWithLocal(cmdItem_Clone.ConditionExpr, procItem.LocalVariables, _evaluator);

    //                        if (eval_res == null)
    //                        {
    //                            whileDoFlag = false;
    //                            LogInfo("[ProcedureExecutor] " + "While's condition expr is null. / " + cmdItem_Clone.ConditionExpr);
    //                        }
    //                    }

    //                    procItem.IsBreakContinueCalled.Pop();

    //                    #endregion
    //                }
    //                else if (cmdItem.TargetType == "for")
    //                {
    //                    #region for

    //                    CommandItem cmdItem_Clone = cmdItem.Clone();

    //                    cmdItem_Clone.LocalVariables = procItem.LocalVariables;
    //                    cmdItem_Clone.IsBreakContinueCalled = procItem.IsBreakContinueCalled;
    //                    cmdItem_Clone.ProcedureReturn = procItem.ProcedureReturn;
    //                    cmdItem_Clone.ProcedureRunningInfo = procItem.ProcedureRunningInfo;
    //                    cmdItem_Clone.RunMode = "seq";

    //                    LocalVariableItem localVariableItem = new LocalVariableItem();

    //                    localVariableItem.LocalVariables = procItem.LocalVariables;
    //                    localVariableItem.TempLocalVariableList = new ArrayList();


    //                    if (_variableTypeNameList.Contains(cmdItem_Clone.ForStartExpr_FirstToken))
    //                    {
    //                        _engineHelper.ParseDefVariableLineWithLocal(cmdItem_Clone.ForStartExpr_FirstToken, cmdItem_Clone.ForStartExpr, localVariableItem, procItem.Name, _evaluator);
    //                    }
    //                    else
    //                    {
    //                        _engineHelper.ParseSetVariableLineWithLocal(cmdItem_Clone.ForStartExpr, procItem.LocalVariables, procItem.Name, _evaluator);
    //                    }


    //                    BreakContinueItem breakItem = new BreakContinueItem();
    //                    breakItem.IsBreakCalled = false;
    //                    breakItem.IsContinueCalled = false;

    //                    procItem.IsBreakContinueCalled.Push(breakItem);

    //                    cmdItem_Clone.Name = "for";


    //                    bool whileDoFlag = true;

    //                    object eval_res = _engineHelper.CheckConditionExprWithLocal(cmdItem_Clone.ForEndCondition, procItem.LocalVariables, _evaluator);

    //                    if (eval_res == null)
    //                    {
    //                        whileDoFlag = false;
    //                        LogInfo("[ProcedureExecutor] " + "For's condition expr is null. / " + cmdItem_Clone.ForEndCondition);
    //                    }

    //                    while (Util.ToBool(eval_res) && whileDoFlag)
    //                    {

    //                        if (_ProcedureStopState)
    //                        {
    //                            foreach (string key in tempLocalVariableList)
    //                            {
    //                                try
    //                                {
    //                                    procItem.LocalVariables.Remove(key);
    //                                }
    //                                catch (Exception ex)
    //                                {
    //                                    LogInfo("[ProcedureExecutor] " + ex.ToString());
    //                                }
    //                            }

    //                            break;
    //                        }

    //                        Debug.Log("전 : " + cmdItem_Clone.Name);
    //                        ProcedureExecutorNotCo(child, cmdItem_Clone, _evaluator);
    //                        Debug.Log("후 : " + cmdItem_Clone.Name);
    //                        if (breakItem.IsBreakCalled)
    //                        {
    //                            breakItem.IsBreakCalled = false;
    //                            break;
    //                        }

    //                        if (breakItem.IsContinueCalled)
    //                        {
    //                            breakItem.IsContinueCalled = false;
    //                        }

    //                        _engineHelper.ParseSetVariableLineWithLocal(cmdItem_Clone.ForStepExpr, procItem.LocalVariables, procItem.Name, _evaluator);


    //                        eval_res = _engineHelper.CheckConditionExprWithLocal(cmdItem_Clone.ForEndCondition, procItem.LocalVariables, _evaluator);

    //                        if (eval_res == null)
    //                        {
    //                            whileDoFlag = false;
    //                            LogInfo("[ProcedureExecutor] " + "For's condition expr is null. / " + cmdItem_Clone.ForEndCondition);
    //                        }
    //                    }

    //                    procItem.IsBreakContinueCalled.Pop();

    //                    foreach (string key in localVariableItem.TempLocalVariableList)
    //                        procItem.LocalVariables.Remove(key);

    //                    #endregion
    //                }
    //                else if (cmdItem.TargetType == "switch")
    //                {
    //                    #region switch

    //                    CommandItem cmdItem_Clone = cmdItem.Clone();

    //                    cmdItem_Clone.LocalVariables = procItem.LocalVariables;
    //                    cmdItem_Clone.IsBreakContinueCalled = procItem.IsBreakContinueCalled;
    //                    cmdItem_Clone.ProcedureReturn = procItem.ProcedureReturn;
    //                    cmdItem_Clone.ProcedureRunningInfo = procItem.ProcedureRunningInfo;
    //                    cmdItem_Clone.RunMode = "seq";

    //                    cmdItem_Clone.SwitchExpr = _engineHelper.ExcuteConditionExprWithLocal(cmdItem_Clone.ConditionExpr, procItem.LocalVariables, _evaluator).ToString();

    //                    cmdItem_Clone.Name = "switch";

    //                    ProcedureExecutorNotCo(child, cmdItem_Clone, _evaluator);
    //                    #endregion
    //                }
    //                else if (cmdItem.TargetType == "case")
    //                {
    //                    #region case

    //                    CommandItem cmdItem_Clone = cmdItem.Clone();

    //                    cmdItem_Clone.LocalVariables = procItem.LocalVariables;
    //                    cmdItem_Clone.IsBreakContinueCalled = procItem.IsBreakContinueCalled;
    //                    cmdItem_Clone.ProcedureReturn = procItem.ProcedureReturn;
    //                    cmdItem_Clone.ProcedureRunningInfo = procItem.ProcedureRunningInfo;
    //                    cmdItem_Clone.RunMode = "seq";

    //                    if (procItem.SwitchExpr == cmdItem_Clone.CaseExpr)
    //                    {

    //                        BreakContinueItem breakItem = new BreakContinueItem();
    //                        breakItem.IsBreakCalled = false;
    //                        breakItem.IsContinueCalled = false;

    //                        procItem.IsBreakContinueCalled.Push(breakItem);


    //                        cmdItem_Clone.Name = "case";

    //                        ProcedureExecutorNotCo(child, cmdItem_Clone, _evaluator);

    //                        CaseRunFlag = true;

    //                        procItem.IsBreakContinueCalled.Pop();
    //                    }

    //                    #endregion
    //                }
    //                else if (cmdItem.TargetType == "default")
    //                {
    //                    #region default

    //                    if (!CaseRunFlag)
    //                    {

    //                        CommandItem cmdItem_Clone = cmdItem.Clone();

    //                        cmdItem_Clone.LocalVariables = procItem.LocalVariables;
    //                        cmdItem_Clone.IsBreakContinueCalled = procItem.IsBreakContinueCalled;
    //                        cmdItem_Clone.ProcedureReturn = procItem.ProcedureReturn;
    //                        cmdItem_Clone.ProcedureRunningInfo = procItem.ProcedureRunningInfo;
    //                        cmdItem_Clone.RunMode = "seq";


    //                        BreakContinueItem breakItem = new BreakContinueItem();
    //                        breakItem.IsBreakCalled = false;
    //                        breakItem.IsContinueCalled = false;

    //                        procItem.IsBreakContinueCalled.Push(breakItem);


    //                        cmdItem_Clone.Name = "default";

    //                        ProcedureExecutorNotCo(child, cmdItem_Clone, _evaluator);


    //                        procItem.IsBreakContinueCalled.Pop();
    //                    }

    //                    #endregion
    //                }
    //                else if (cmdItem.TargetType == "call")
    //                {
    //                    #region procedure

    //                    CommandItem cmdItem_Clone = cmdItem.Clone();

    //                    //LogInfo(cmdItem.Target);

    //                    CommandTree procItem_Tree = _enginePreProcessor.SearchProcedureWithName(cmdItem_Clone.Target);

    //                    if (procItem_Tree != null)
    //                    {
    //                        CommandItem procItem_Clone = procItem_Tree.commandItem.Clone();


    //                        ProcedureReturnItem procReturnItem = new ProcedureReturnItem();
    //                        procReturnItem.IsReturnCalled = false;
    //                        procReturnItem.ProcedureName = procItem_Clone.Name;

    //                        procItem_Clone.ProcedureReturn = procReturnItem;

    //                        procItem_Clone.ProcedureCallDataList = cmdItem_Clone.ProcedureCallDataList;

    //                        for (int pInd = 0; pInd < procItem_Clone.ProcedureCallDataList.Count; pInd++)
    //                        {
    //                            object resObj = _engineHelper.ExcuteConditionExprWithLocal(procItem_Clone.ProcedureCallDataList[pInd].ToString(), procItem.LocalVariables, _evaluator);
    //                            procItem_Clone.ProcedureCallObjectList.Add(resObj);

    //                        }


    //                        if (cmdItem_Clone.RunMode != string.Empty)
    //                            procItem_Clone.RunMode = cmdItem_Clone.RunMode;


    //                        procItem_Clone.ProcedureRunningInfo = procItem.ProcedureRunningInfo;


    //                        if (procItem_Clone.RunMode == "concur")
    //                        {
    //                            eval.Evaluator evaluator = new eval.Evaluator();

    //                            evaluator.GlobalVariables = _GlobalVariables;
    //                            evaluator.ProcedureList = _ProcedureList;
    //                            evaluator.EvalCallProcedureHandler = new eval.Evaluator.CallProcedureHandler(this.CallProcedure);
    //                            evaluator.EvalLogInfoHandler = new eval.Evaluator.LogInfoHandler(this.LogInfo);
    //                            evaluator.EvalUnityRequestHandler = new eval.Evaluator.UnityRequestHandler(this.UnityRequestProc);

    //                            ProcedureExecutorNotCo(procItem_Tree, procItem_Clone, evaluator);
    //                        }
    //                        else if (procItem_Clone.RunMode == "loop")
    //                        {
    //                            _StartLoopRunFlag = true;

    //                            eval.Evaluator evaluator = new eval.Evaluator();
    //                            evaluator.GlobalVariables = _GlobalVariables;
    //                            evaluator.ProcedureList = _ProcedureList;
    //                            evaluator.EvalCallProcedureHandler = new eval.Evaluator.CallProcedureHandler(this.CallProcedure);
    //                            evaluator.EvalLogInfoHandler = new eval.Evaluator.LogInfoHandler(this.LogInfo);
    //                            evaluator.EvalUnityRequestHandler = new eval.Evaluator.UnityRequestHandler(this.UnityRequestProc);

    //                            ProcedureExecutorLoop(procItem_Tree, procItem_Clone, evaluator);
    //                        }
    //                        else
    //                        {
    //                            //Call sequential
    //                            ProcedureExecutorNotCo(procItem_Tree, procItem_Clone, _evaluator);
    //                        }
    //                    }
    //                    else
    //                    {
    //                        //LogInfo("[ProcedureExecutor] " + "Procedure " + cmdItem_Clone.Target + " is not exist.");
    //                        if (cmdItem_Clone.Target != "main")
    //                        {
    //                            //2017.11.19
    //                            if (_QUIT_REQUEST == false)
    //                                LogInfo("Error EP: \"" + cmdItem_Clone.Target + "\" function not exist");
    //                        }
    //                    }

    //                    #endregion
    //                }
    //                else if (cmdItem.TargetType == "print")
    //                {
    //                    #region print

    //                    CommandItem cmdItem_Clone = cmdItem.Clone();

    //                    //2016.09.25
    //                    cmdItem_Clone.Action = ParsingHelper.RestoreSlashWithoutTrim(cmdItem_Clone.Action);

                        
    //                    object eval_res = _engineHelper.ExcuteConditionExprWithLocal(cmdItem_Clone.Action, procItem.LocalVariables, _evaluator);


    //                    if (eval_res != null)
    //                    {
    //                        string printStr = eval_res.ToString();

    //                        if (printStr != string.Empty)
    //                        {
    //                            if (cmdItem.Target == "print" || cmdItem.Target == "serial.print")
    //                                LogPrint(printStr);
    //                            else
    //                            {
    //                                //println, printline
    //                                LogPrintLine(printStr);
    //                            }
    //                        }
    //                    }
    //                    else
    //                    {
    //                        LogInfo("[ProcedureExecutor] " + "Print data is null. ");
    //                    }

    //                    #endregion print
    //                }

    //                #endregion doFlag
    //            }

    //            #endregion foreach

    //        }
    //    }

    //    #endregion foreach

    //    if (procItem.FirstToken == "procedure")
    //    {
    //        if (procItem.ProcedureRunningInfo != null)
    //        {
    //            procItem.ProcedureRunningInfo.RunningProcedureCount--;
    //        }
    //    }

    //    if (procItem.Name != "maindefault")
    //    {
    //        foreach (string key in tempLocalVariableList)
    //            procItem.LocalVariables.Remove(key);
    //    }
    //    else
    //    {
    //        //if (!_logInfo_called)
    //        //	LogInfo(" ");  
    //    }


    //    // break;
    //}


    private IEnumerator ProcedureExecutor(CommandTree parsedTree, CommandItem procItem, eval.Evaluator _evaluator)
    {
        if (_ProcedureStopState)
            yield break;

        ArrayList tempLocalVariableList = new ArrayList();

        bool ifConditionResult = true;
        bool CaseRunFlag = false;


        #region Pre processing


        #region if (procItem.FirstToken == "procedure")

        if (procItem.FirstToken == "procedure")
        {
            _procedureExecutorHelper.Proc_CopyVariables(parsedTree, procItem, _evaluator, tempLocalVariableList);
        }

        #endregion if (procItem.FirstToken == "procedure")


        if (procItem == null)
        {
            yield break;
        }

        #endregion


        #region foreach


        foreach (CommandTree child in parsedTree.Childs)
        {

            if (_ProcedureStopState)
            {

                foreach (string key in tempLocalVariableList)
                {
                    try
                    {
                        procItem.LocalVariables.Remove(key);
                    }
                    catch (Exception ex)
                    {
                        LogInfo("[ProcedureExecutor] " + ex.ToString());
                    }
                }

                yield break;
            }


            #region foreach

            _procedureExecutorHelper.Proc_ContineAndReturn(parsedTree, procItem, _evaluator, tempLocalVariableList);

            #endregion foreach


            if (child.commandItem != null)
            {
                #region foreach inner

                bool doFlag = false;


                if (procItem.LocalVariables.ContainsKey(child.FirstTokenOfLine))
                    doFlag = true;

                if (child.IsSetVariable)
                    doFlag = true;


                if (child.IsExecutableTargetType && child.FirstTokenOfLine != "procedure" && child.FirstTokenOfLine != "end")
                    doFlag = true;


                if (doFlag)
                {

                    #region doFlag

                    CommandItem cmdItem = child.commandItem;


                    //##################################################################################################################################
                    //LogInfo("[foreach inner] " + cmdItem.FirstToken + " / " + cmdItem.Action + " / " + cmdItem.Target + " / " + cmdItem.TargetType);
                    //Debug.Log("[foreach inner] " + cmdItem.FirstToken + " / " + cmdItem.Action + " / " + cmdItem.Target + " / " + cmdItem.TargetType);
                    //##################################################################################################################################


                    if (cmdItem.TargetType == "arduino_command")
                    {
                        #region arduino_command

                        //LogInfo("[arduino_command] " + cmdItem.FirstToken + " / " + cmdItem.Action + " / " + cmdItem.Target);
                        Debug.Log("[arduino_command] " + cmdItem.FirstToken + " / " + cmdItem.Action + " / " + cmdItem.Target);

                        CommandItem cmdItem_Clone = cmdItem.Clone();

                        yield return StartCoroutine(_engineHelper.ReplaceWithVariables(procItem.LocalVariables, cmdItem_Clone.Target, _evaluator));
                        cmdItem_Clone.Target = gv.GlobalVariables.res.ToString();
                        //cmdItem_Clone.Target = _engineHelper.ReplaceWithVariables(procItem.LocalVariables, cmdItem_Clone.Target, _evaluator);

                        if (cmdItem.FirstToken == "digitalwrite" && !string.IsNullOrEmpty(cmdItem_Clone.Target))
                        {
                            ArrayList str_list = SPLHelper.GetStringList(cmdItem_Clone.Target);

                            if (str_list != null && str_list.Count == 2)
                            {
                                yield return StartCoroutine(_engineHelper.ExcuteConditionExprWithLocal(str_list[0].ToString(), procItem.LocalVariables, _evaluator));
                                object eval_res1 = gv.GlobalVariables.res;
                                yield return StartCoroutine(_engineHelper.ExcuteConditionExprWithLocal(str_list[1].ToString(), procItem.LocalVariables, _evaluator));
                                object eval_res2 = gv.GlobalVariables.res;

                                //object eval_res1 = _engineHelper.ExcuteConditionExprWithLocal(str_list[0].ToString(), procItem.LocalVariables, _evaluator);
                                //object eval_res2 = _engineHelper.ExcuteConditionExprWithLocal(str_list[1].ToString(), procItem.LocalVariables, _evaluator);


                                if (eval_res1 != null && eval_res2 != null)
                                {
                                    int pin = Util.ToInt(eval_res1);
                                    int v = Util.ToInt(eval_res2);

                                    _GlobalVariables["_DIGITAL_" + pin.ToString()] = v;
                                }
                            }
                        }
                        else if (cmdItem.FirstToken == "analogwrite" && !string.IsNullOrEmpty(cmdItem_Clone.Target))
                        {
                            ArrayList str_list = SPLHelper.GetStringList(cmdItem_Clone.Target);

                            if (str_list != null && str_list.Count == 2)
                            {

                                yield return StartCoroutine(_engineHelper.ExcuteConditionExprWithLocal(str_list[0].ToString(), procItem.LocalVariables, _evaluator));
                                object eval_res1 = gv.GlobalVariables.res;
                                yield return StartCoroutine(_engineHelper.ExcuteConditionExprWithLocal(str_list[1].ToString(), procItem.LocalVariables, _evaluator));
                                object eval_res2 = gv.GlobalVariables.res;

                                //object eval_res1 = _engineHelper.ExcuteConditionExprWithLocal(str_list[0].ToString(), procItem.LocalVariables, _evaluator);
                                //object eval_res2 = _engineHelper.ExcuteConditionExprWithLocal(str_list[1].ToString(), procItem.LocalVariables, _evaluator);


                                if (eval_res1 != null && eval_res2 != null)
                                {
                                    int pin = Util.ToInt(eval_res1);
                                    int v = Util.ToInt(eval_res2);

                                    if (v < 0)
                                        v = 0;
                                    else if (v > 255)
                                        v = 255;

                                    //PWM not analog
                                    _GlobalVariables["_PWM_" + pin.ToString()] = v;
                                }
                            }
                        }
                        else if (cmdItem.FirstToken == "tone" && !string.IsNullOrEmpty(cmdItem_Clone.Target))
                        {
                            ArrayList str_list = SPLHelper.GetStringList(cmdItem_Clone.Target);

                            if (str_list != null && str_list.Count == 3)
                            {

                                yield return StartCoroutine(_engineHelper.ExcuteConditionExprWithLocal(str_list[0].ToString(), procItem.LocalVariables, _evaluator));
                                object eval_res1 = gv.GlobalVariables.res;
                                yield return StartCoroutine(_engineHelper.ExcuteConditionExprWithLocal(str_list[1].ToString(), procItem.LocalVariables, _evaluator));
                                object eval_res2 = gv.GlobalVariables.res;
                                yield return StartCoroutine(_engineHelper.ExcuteConditionExprWithLocal(str_list[2].ToString(), procItem.LocalVariables, _evaluator));
                                object eval_res3 = gv.GlobalVariables.res;


                                //object eval_res1 = _engineHelper.ExcuteConditionExprWithLocal(str_list[0].ToString(), procItem.LocalVariables, _evaluator);
                                //object eval_res2 = _engineHelper.ExcuteConditionExprWithLocal(str_list[1].ToString(), procItem.LocalVariables, _evaluator);
                                //object eval_res3 = _engineHelper.ExcuteConditionExprWithLocal(str_list[2].ToString(), procItem.LocalVariables, _evaluator);


                                if (eval_res1 != null && eval_res2 != null && eval_res3 != null)
                                {
                                    int pin = Util.ToInt(eval_res1);
                                    int fr = Util.ToInt(eval_res2);
                                    float du = Util.ToFloat(eval_res3);

                                    //PlayOrGenerate(PCSpeakerEffects.Tone(fr, speaker.sampleRate), du * 0.001f);
                                    if (pin == 2)
                                    {
                                        Tone_Internal(fr, du * 0.001f);
                                    }
                                    
                                }
                            }
                        }
                        else if (cmdItem.FirstToken == "servowrite" && !string.IsNullOrEmpty(cmdItem_Clone.Target))
                        {
                            ArrayList str_list = SPLHelper.GetStringList(cmdItem_Clone.Target);

                            if (str_list != null && str_list.Count == 2)
                            {
                                yield return StartCoroutine(_engineHelper.ExcuteConditionExprWithLocal(str_list[0].ToString(), procItem.LocalVariables, _evaluator));
                                object eval_res1 = gv.GlobalVariables.res;
                                yield return StartCoroutine(_engineHelper.ExcuteConditionExprWithLocal(str_list[1].ToString(), procItem.LocalVariables, _evaluator));
                                object eval_res2 = gv.GlobalVariables.res;

                                //object eval_res1 = _engineHelper.ExcuteConditionExprWithLocal(str_list[0].ToString(), procItem.LocalVariables, _evaluator);
                                //object eval_res2 = _engineHelper.ExcuteConditionExprWithLocal(str_list[1].ToString(), procItem.LocalVariables, _evaluator);


                                if (eval_res1 != null && eval_res2 != null)
                                {
                                    int pin = Util.ToInt(eval_res1);
                                    int v = Util.ToInt(eval_res2);

                                    if (v < 0)
                                        v = 0;

                                    if (v > 180)
                                        v = 180;

                                    _GlobalVariables["_SERVO_" + pin.ToString()] = v;

                                    //if (Bar_Ring_Object != null)
                                    //{
                                    //    Bar_Ring_Object.transform.localEulerAngles = new Vector3(0, 0, -v);
                                    //}
                                }
                            }
                        }
                        else if (cmdItem.FirstToken == "drivewrite" && !string.IsNullOrEmpty(cmdItem_Clone.Target))
                        {
                            ArrayList str_list = SPLHelper.GetStringList(cmdItem_Clone.Target);

                            if (str_list != null && str_list.Count == 2)
                            {
                                yield return StartCoroutine(_engineHelper.ExcuteConditionExprWithLocal(str_list[0].ToString(), procItem.LocalVariables, _evaluator));
                                object eval_res1 = gv.GlobalVariables.res;
                                yield return StartCoroutine(_engineHelper.ExcuteConditionExprWithLocal(str_list[1].ToString(), procItem.LocalVariables, _evaluator));
                                object eval_res2 = gv.GlobalVariables.res;

                                //object eval_res1 = _engineHelper.ExcuteConditionExprWithLocal(str_list[0].ToString(), procItem.LocalVariables, _evaluator);
                                //object eval_res2 = _engineHelper.ExcuteConditionExprWithLocal(str_list[1].ToString(), procItem.LocalVariables, _evaluator);


                                if (eval_res1 != null && eval_res2 != null)
                                {
                                    int left = Util.ToInt(eval_res1);
                                    int right = Util.ToInt(eval_res2);

                                    _GlobalVariables["_MOTOR1_POWER"] = left;
                                    _GlobalVariables["_MOTOR2_POWER"] = right;

                                    RobotControl.LEFT_POWER = left.ToString();
                                    RobotControl.RIGHT_POWER = right.ToString();
                                }
                            }
                        }
                        else if (cmdItem.FirstToken == "motor1write" && !string.IsNullOrEmpty(cmdItem_Clone.Target))
                        {
                            ArrayList str_list = SPLHelper.GetStringList(cmdItem_Clone.Target);

                            if (str_list != null && str_list.Count == 1)
                            {
                                yield return StartCoroutine(_engineHelper.ExcuteConditionExprWithLocal(str_list[0].ToString(), procItem.LocalVariables, _evaluator));
                                object eval_res1 = gv.GlobalVariables.res;

                                //object eval_res1 = _engineHelper.ExcuteConditionExprWithLocal(str_list[0].ToString(), procItem.LocalVariables, _evaluator);


                                if (eval_res1 != null)
                                {
                                    int power = Util.ToInt(eval_res1);

                                    _GlobalVariables["_MOTOR1_POWER"] = power;
                                    RobotControl.LEFT_POWER = power.ToString();
                                }
                            }
                        }
                        else if (cmdItem.FirstToken == "motor2write" && !string.IsNullOrEmpty(cmdItem_Clone.Target))
                        {
                            ArrayList str_list = SPLHelper.GetStringList(cmdItem_Clone.Target);

                            if (str_list != null && str_list.Count == 1)
                            {
                                yield return StartCoroutine(_engineHelper.ExcuteConditionExprWithLocal(str_list[0].ToString(), procItem.LocalVariables, _evaluator));
                                object eval_res1 = gv.GlobalVariables.res;

                                //object eval_res1 = _engineHelper.ExcuteConditionExprWithLocal(str_list[0].ToString(), procItem.LocalVariables, _evaluator);


                                if (eval_res1 != null)
                                {
                                    int power = Util.ToInt(eval_res1);

                                    _GlobalVariables["_MOTOR2_POWER"] = power;
                                    RobotControl.RIGHT_POWER = power.ToString();
                                }
                            }
                        }

                        #endregion arduino_command
                    }
                    else if (cmdItem.TargetType == "delay")
                    {
                        #region delay

                        CommandItem cmdItem_Clone = cmdItem.Clone();

                        yield return StartCoroutine(_engineHelper.ReplaceWithVariables(procItem.LocalVariables, cmdItem_Clone.Target, _evaluator));
                        cmdItem_Clone.Target = gv.GlobalVariables.res.ToString();
                        //cmdItem_Clone.Target = _engineHelper.ReplaceWithVariables(procItem.LocalVariables, cmdItem_Clone.Target, _evaluator);

                        yield return StartCoroutine(_engineHelper.ExcuteConditionExprWithLocal(cmdItem_Clone.Target, procItem.LocalVariables, _evaluator));
                        object eval_res = gv.GlobalVariables.res;
                        //object eval_res = _engineHelper.ExcuteConditionExprWithLocal(cmdItem_Clone.Target, procItem.LocalVariables, _evaluator);
                        if (eval_res != null)
                        {
                            cmdItem_Clone.Target = eval_res.ToString();

                            int delaytime = 0;

                            try
                            {
                                delaytime = int.Parse(cmdItem_Clone.Target);
                            }
                            catch (Exception ex)
                            {
                                LogInfo("[ProcedureExecutor] " + ex.ToString());
                            }

                            if (delaytime > 0)
                            {
                                yield return new WaitForSeconds(delaytime / 1000.0f);
                            }

                        }
                        else
                        {
                            LogInfo("[ProcedureExecutor] " + "Delay data is null.");
                        }

                        #endregion
                    }
                    else if (cmdItem.TargetType == "defvariable")
                    {
                        #region defvariable

                        CommandItem cmdItem_Clone = cmdItem.Clone();

                        LocalVariableItem localVariableItem = new LocalVariableItem();

                        localVariableItem.LocalVariables = procItem.LocalVariables;
                        localVariableItem.TempLocalVariableList = tempLocalVariableList;

                        //LogInfo("[defvariable] " + cmdItem_Clone.FirstToken + " / " + cmdItem_Clone.Action + " / " + cmdItem_Clone.Target);

                        yield return StartCoroutine(_engineHelper.ParseDefVariableLineWithLocal(cmdItem_Clone.Target, cmdItem_Clone.Action, localVariableItem, procItem.Name, _evaluator));
                        //object eval_res1 = gv.GlobalVariables.res;


                        //_engineHelper.ParseDefVariableLineWithLocal(cmdItem_Clone.Target, cmdItem_Clone.Action, localVariableItem, procItem.Name, _evaluator);

                        #endregion
                    }
                    else if (cmdItem.TargetType == "setvariable")
                    {
                        #region setvariable

                        CommandItem cmdItem_Clone = cmdItem.Clone();

                        if (cmdItem_Clone.Action.IndexOf("{") >= 0)
                        {
                            yield return StartCoroutine(_engineHelper.ReplaceWithVariables(procItem.LocalVariables, cmdItem_Clone.Action, _evaluator));
                            cmdItem_Clone.Action = gv.GlobalVariables.res.ToString();
                           // cmdItem_Clone.Action = _engineHelper.ReplaceWithVariables(procItem.LocalVariables, cmdItem_Clone.Action, _evaluator);
                        }

                        //LogInfo("[setvariable] " + cmdItem_Clone.FirstToken + " / " + cmdItem_Clone.Action + " / " + cmdItem_Clone.Target);

                        //_engineHelper.ParseSetVariableLineWithLocal(cmdItem_Clone.Action, procItem.LocalVariables, procItem.Name, _evaluator, procItem.Target);
                        yield return StartCoroutine(_engineHelper.ParseSetVariableLineWithLocal(cmdItem_Clone.Action, procItem.LocalVariables, procItem.Name, _evaluator, procItem.Target));
                        object eval_res = gv.GlobalVariables.res;


                        if (cmdItem.Target == "return")
                        {
                            if (procItem.ProcedureReturn != null)
                                procItem.ProcedureReturn.IsReturnCalled = true;

                            yield break;
                        }

                        #endregion
                    }
                    else if (cmdItem.TargetType == "break")
                    {
                        #region break

                        if (procItem.IsBreakContinueCalled.Count > 0)
                        {
                            if (procItem.Name == "if" || procItem.Name == "else" || procItem.Name == "for" || procItem.Name == "while" || procItem.Name == "case" || procItem.Name == "default")
                            {
                                BreakContinueItem breakItem = (BreakContinueItem)procItem.IsBreakContinueCalled.Peek();
                                breakItem.IsBreakCalled = true;
                            }
                        }

                        yield break;

                        #endregion
                    }
                    else if (cmdItem.TargetType == "continue")
                    {
                        #region break

                        if (procItem.IsBreakContinueCalled.Count > 0)
                        {
                            if (procItem.Name == "if" || procItem.Name == "else" || procItem.Name == "for" || procItem.Name == "while")
                            {
                                BreakContinueItem breakItem = (BreakContinueItem)procItem.IsBreakContinueCalled.Peek();
                                breakItem.IsContinueCalled = true;
                            }
                        }

                        yield break;

                        #endregion
                    }
                    else if (cmdItem.TargetType == "if")
                    {
                        #region if

                        CommandItem cmdItem_Clone = cmdItem.Clone();

                        cmdItem_Clone.LocalVariables = procItem.LocalVariables;
                        cmdItem_Clone.IsBreakContinueCalled = procItem.IsBreakContinueCalled;
                        cmdItem_Clone.ProcedureReturn = procItem.ProcedureReturn;
                        cmdItem_Clone.ProcedureRunningInfo = procItem.ProcedureRunningInfo;
                        cmdItem_Clone.RunMode = "seq";


                        //object eval_res = _engineHelper.CheckConditionExprWithLocal(cmdItem_Clone.ConditionExpr, procItem.LocalVariables, _evaluator);
                        yield return StartCoroutine(_engineHelper.CheckConditionExprWithLocal(cmdItem_Clone.ConditionExpr, procItem.LocalVariables, _evaluator));
                        object eval_res = gv.GlobalVariables.res;

                        if (eval_res != null)
                        {
                            ifConditionResult = Util.ToBool(eval_res);

                            cmdItem_Clone.Name = "if";

                            if (ifConditionResult)
                            {
                                yield return StartCoroutine(ProcedureExecutor(child, cmdItem_Clone, _evaluator));
                            }
                        }
                        else
                        {
                            LogInfo("[ProcedureExecutor] " + "If's condition expr is not boolean type. / " + cmdItem_Clone.ConditionExpr);
                        }

                        #endregion
                    }
                    else if (cmdItem.TargetType == "else")
                    {
                        #region else

                        CommandItem cmdItem_Clone = cmdItem.Clone();

                        cmdItem_Clone.LocalVariables = procItem.LocalVariables;
                        cmdItem_Clone.IsBreakContinueCalled = procItem.IsBreakContinueCalled;
                        cmdItem_Clone.ProcedureReturn = procItem.ProcedureReturn;
                        cmdItem_Clone.ProcedureRunningInfo = procItem.ProcedureRunningInfo;
                        cmdItem_Clone.RunMode = "seq";

                        cmdItem_Clone.Name = "else";

                        if (!ifConditionResult)
                        {
                            yield return StartCoroutine(ProcedureExecutor(child, cmdItem_Clone, _evaluator));
                        }

                        ifConditionResult = true;

                        #endregion
                    }
                    else if (cmdItem.TargetType == "while")
                    {
                        #region while

                        CommandItem cmdItem_Clone = cmdItem.Clone();

                        cmdItem_Clone.LocalVariables = procItem.LocalVariables;
                        cmdItem_Clone.IsBreakContinueCalled = procItem.IsBreakContinueCalled;
                        cmdItem_Clone.ProcedureReturn = procItem.ProcedureReturn;
                        cmdItem_Clone.ProcedureRunningInfo = procItem.ProcedureRunningInfo;
                        cmdItem_Clone.RunMode = "seq";


                        BreakContinueItem breakItem = new BreakContinueItem();
                        breakItem.IsBreakCalled = false;
                        breakItem.IsContinueCalled = false;

                        procItem.IsBreakContinueCalled.Push(breakItem);

                        cmdItem_Clone.Name = "while";


                        bool whileDoFlag = true;

                        //object eval_res = _engineHelper.CheckConditionExprWithLocal(cmdItem_Clone.ConditionExpr, procItem.LocalVariables, _evaluator);
                        yield return StartCoroutine(_engineHelper.CheckConditionExprWithLocal(cmdItem_Clone.ConditionExpr, procItem.LocalVariables, _evaluator));
                        object eval_res = gv.GlobalVariables.res;

                        if (eval_res == null)
                        {
                            whileDoFlag = false;
                            LogInfo("[ProcedureExecutor] " + "While's condition expr is null. / " + cmdItem_Clone.ConditionExpr);
                        }

                        while (Util.ToBool(eval_res) && whileDoFlag)
                        {

                            if (_ProcedureStopState)
                            {
                                try
                                {
                                    foreach (string key in tempLocalVariableList)
                                    {
                                        try
                                        {
                                            procItem.LocalVariables.Remove(key);
                                        }
                                        catch { }
                                    }
                                }
                                catch { }

                                yield break;
                            }


                            yield return StartCoroutine(ProcedureExecutor(child, cmdItem_Clone, _evaluator));

                            if (breakItem.IsBreakCalled)
                            {
                                breakItem.IsBreakCalled = false;
                                break;
                            }

                            if (breakItem.IsContinueCalled)
                            {
                                breakItem.IsContinueCalled = false;
                            }

                            yield return StartCoroutine(_engineHelper.CheckConditionExprWithLocal(cmdItem_Clone.ConditionExpr, procItem.LocalVariables, _evaluator));
                            //eval_res = _engineHelper.CheckConditionExprWithLocal(cmdItem_Clone.ConditionExpr, procItem.LocalVariables, _evaluator);
                            eval_res = gv.GlobalVariables.res;

                            if (eval_res == null)
                            {
                                whileDoFlag = false;
                                LogInfo("[ProcedureExecutor] " + "While's condition expr is null. / " + cmdItem_Clone.ConditionExpr);
                            }
                        }

                        procItem.IsBreakContinueCalled.Pop();

                        #endregion
                    }
                    else if (cmdItem.TargetType == "for")
                    {
                        #region for

                        CommandItem cmdItem_Clone = cmdItem.Clone();

                        cmdItem_Clone.LocalVariables = procItem.LocalVariables;
                        cmdItem_Clone.IsBreakContinueCalled = procItem.IsBreakContinueCalled;
                        cmdItem_Clone.ProcedureReturn = procItem.ProcedureReturn;
                        cmdItem_Clone.ProcedureRunningInfo = procItem.ProcedureRunningInfo;
                        cmdItem_Clone.RunMode = "seq";

                        LocalVariableItem localVariableItem = new LocalVariableItem();

                        localVariableItem.LocalVariables = procItem.LocalVariables;
                        localVariableItem.TempLocalVariableList = new ArrayList();


                        if (_variableTypeNameList.Contains(cmdItem_Clone.ForStartExpr_FirstToken))
                        {
                            yield return StartCoroutine(_engineHelper.ParseDefVariableLineWithLocal(cmdItem_Clone.ForStartExpr_FirstToken, cmdItem_Clone.ForStartExpr, localVariableItem, procItem.Name, _evaluator));
                        }
                        else
                        {
                            yield return StartCoroutine(_engineHelper.ParseSetVariableLineWithLocal(cmdItem_Clone.ForStartExpr, procItem.LocalVariables, procItem.Name, _evaluator));
                        }


                        BreakContinueItem breakItem = new BreakContinueItem();
                        breakItem.IsBreakCalled = false;
                        breakItem.IsContinueCalled = false;

                        procItem.IsBreakContinueCalled.Push(breakItem);

                        cmdItem_Clone.Name = "for";


                        bool whileDoFlag = true;

                        //object eval_res = _engineHelper.CheckConditionExprWithLocal(cmdItem_Clone.ForEndCondition, procItem.LocalVariables, _evaluator);
                        yield return StartCoroutine(_engineHelper.CheckConditionExprWithLocal(cmdItem_Clone.ForEndCondition, procItem.LocalVariables, _evaluator));
                        object eval_res = gv.GlobalVariables.res;

                        if (eval_res == null)
                        {
                            whileDoFlag = false;
                            LogInfo("[ProcedureExecutor] " + "For's condition expr is null. / " + cmdItem_Clone.ForEndCondition);
                        }

                        while (Util.ToBool(eval_res) && whileDoFlag)
                        {

                            if (_ProcedureStopState)
                            {
                                foreach (string key in tempLocalVariableList)
                                {
                                    try
                                    {
                                        procItem.LocalVariables.Remove(key);
                                    }
                                    catch (Exception ex)
                                    {
                                        LogInfo("[ProcedureExecutor] " + ex.ToString());
                                    }
                                }

                                yield break;
                            }

                            //Debug.Log("전 : " + cmdItem_Clone.Name);
                            yield return StartCoroutine(ProcedureExecutor(child, cmdItem_Clone, _evaluator));
                            //Debug.Log("후 : " + cmdItem_Clone.Name);
                            if (breakItem.IsBreakCalled)
                            {
                                breakItem.IsBreakCalled = false;
                                break;
                            }

                            if (breakItem.IsContinueCalled)
                            {
                                breakItem.IsContinueCalled = false;
                            }

                            yield return StartCoroutine(_engineHelper.ParseSetVariableLineWithLocal(cmdItem_Clone.ForStepExpr, procItem.LocalVariables, procItem.Name, _evaluator));


                            yield return StartCoroutine(_engineHelper.CheckConditionExprWithLocal(cmdItem_Clone.ForEndCondition, procItem.LocalVariables, _evaluator));
                            eval_res = gv.GlobalVariables.res;
                            //eval_res = _engineHelper.CheckConditionExprWithLocal(cmdItem_Clone.ForEndCondition, procItem.LocalVariables, _evaluator);

                            if (eval_res == null)
                            {
                                whileDoFlag = false;
                                LogInfo("[ProcedureExecutor] " + "For's condition expr is null. / " + cmdItem_Clone.ForEndCondition);
                            }
                        }

                        procItem.IsBreakContinueCalled.Pop();

                        foreach (string key in localVariableItem.TempLocalVariableList)
                            procItem.LocalVariables.Remove(key);

                        #endregion
                    }
                    else if (cmdItem.TargetType == "switch")
                    {
                        #region switch

                        CommandItem cmdItem_Clone = cmdItem.Clone();

                        cmdItem_Clone.LocalVariables = procItem.LocalVariables;
                        cmdItem_Clone.IsBreakContinueCalled = procItem.IsBreakContinueCalled;
                        cmdItem_Clone.ProcedureReturn = procItem.ProcedureReturn;
                        cmdItem_Clone.ProcedureRunningInfo = procItem.ProcedureRunningInfo;
                        cmdItem_Clone.RunMode = "seq";

                        yield return StartCoroutine(_engineHelper.ExcuteConditionExprWithLocal(cmdItem_Clone.ConditionExpr, procItem.LocalVariables, _evaluator));
                        cmdItem_Clone.SwitchExpr = gv.GlobalVariables.res.ToString();
                        //cmdItem_Clone.SwitchExpr = _engineHelper.ExcuteConditionExprWithLocal(cmdItem_Clone.ConditionExpr, procItem.LocalVariables, _evaluator).ToString();

                        cmdItem_Clone.Name = "switch";

                        yield return StartCoroutine(ProcedureExecutor(child, cmdItem_Clone, _evaluator));
                        #endregion
                    }
                    else if (cmdItem.TargetType == "case")
                    {
                        #region case

                        CommandItem cmdItem_Clone = cmdItem.Clone();

                        cmdItem_Clone.LocalVariables = procItem.LocalVariables;
                        cmdItem_Clone.IsBreakContinueCalled = procItem.IsBreakContinueCalled;
                        cmdItem_Clone.ProcedureReturn = procItem.ProcedureReturn;
                        cmdItem_Clone.ProcedureRunningInfo = procItem.ProcedureRunningInfo;
                        cmdItem_Clone.RunMode = "seq";

                        if (procItem.SwitchExpr == cmdItem_Clone.CaseExpr)
                        {

                            BreakContinueItem breakItem = new BreakContinueItem();
                            breakItem.IsBreakCalled = false;
                            breakItem.IsContinueCalled = false;

                            procItem.IsBreakContinueCalled.Push(breakItem);


                            cmdItem_Clone.Name = "case";

                            yield return StartCoroutine(ProcedureExecutor(child, cmdItem_Clone, _evaluator));

                            CaseRunFlag = true;

                            procItem.IsBreakContinueCalled.Pop();
                        }

                        #endregion
                    }
                    else if (cmdItem.TargetType == "default")
                    {
                        #region default

                        if (!CaseRunFlag)
                        {

                            CommandItem cmdItem_Clone = cmdItem.Clone();

                            cmdItem_Clone.LocalVariables = procItem.LocalVariables;
                            cmdItem_Clone.IsBreakContinueCalled = procItem.IsBreakContinueCalled;
                            cmdItem_Clone.ProcedureReturn = procItem.ProcedureReturn;
                            cmdItem_Clone.ProcedureRunningInfo = procItem.ProcedureRunningInfo;
                            cmdItem_Clone.RunMode = "seq";


                            BreakContinueItem breakItem = new BreakContinueItem();
                            breakItem.IsBreakCalled = false;
                            breakItem.IsContinueCalled = false;

                            procItem.IsBreakContinueCalled.Push(breakItem);


                            cmdItem_Clone.Name = "default";

                            yield return StartCoroutine(ProcedureExecutor(child, cmdItem_Clone, _evaluator));


                            procItem.IsBreakContinueCalled.Pop();
                        }

                        #endregion
                    }
                    else if (cmdItem.TargetType == "call")
                    {
                        #region procedure

                        CommandItem cmdItem_Clone = cmdItem.Clone();

                        //LogInfo(cmdItem.Target);

                        CommandTree procItem_Tree = _enginePreProcessor.SearchProcedureWithName(cmdItem_Clone.Target);

                        if (procItem_Tree != null)
                        {
                            CommandItem procItem_Clone = procItem_Tree.commandItem.Clone();


                            ProcedureReturnItem procReturnItem = new ProcedureReturnItem();
                            procReturnItem.IsReturnCalled = false;
                            procReturnItem.ProcedureName = procItem_Clone.Name;

                            procItem_Clone.ProcedureReturn = procReturnItem;

                            procItem_Clone.ProcedureCallDataList = cmdItem_Clone.ProcedureCallDataList;

                            for (int pInd = 0; pInd < procItem_Clone.ProcedureCallDataList.Count; pInd++)
                            {
                                //object resObj = _engineHelper.ExcuteConditionExprWithLocal(procItem_Clone.ProcedureCallDataList[pInd].ToString(), procItem.LocalVariables, _evaluator);\
                                yield return StartCoroutine(_engineHelper.ExcuteConditionExprWithLocal(procItem_Clone.ProcedureCallDataList[pInd].ToString(), procItem.LocalVariables, _evaluator));
                                object resObj = gv.GlobalVariables.res;
                                procItem_Clone.ProcedureCallObjectList.Add(resObj);

                            }


                            if (cmdItem_Clone.RunMode != string.Empty)
                                procItem_Clone.RunMode = cmdItem_Clone.RunMode;


                            procItem_Clone.ProcedureRunningInfo = procItem.ProcedureRunningInfo;


                            if (procItem_Clone.RunMode == "concur")
                            {
                                eval.Evaluator evaluator = new eval.Evaluator();

                                evaluator.GlobalVariables = _GlobalVariables;
                                evaluator.ProcedureList = _ProcedureList;
                                evaluator.EvalCallProcedureHandler = new eval.Evaluator.CallProcedureHandler(this.CallProcedure);
                                evaluator.EvalLogInfoHandler = new eval.Evaluator.LogInfoHandler(this.LogInfo);
                                evaluator.EvalUnityRequestHandler = new eval.Evaluator.UnityRequestHandler(this.UnityRequestProc);

                                StartCoroutine(ProcedureExecutor(procItem_Tree, procItem_Clone, evaluator));
                            }
                            else if (procItem_Clone.RunMode == "loop")
                            {
                                _StartLoopRunFlag = true;

                                eval.Evaluator evaluator = new eval.Evaluator();
                                evaluator.GlobalVariables = _GlobalVariables;
                                evaluator.ProcedureList = _ProcedureList;
                                evaluator.EvalCallProcedureHandler = new eval.Evaluator.CallProcedureHandler(this.CallProcedure);
                                evaluator.EvalLogInfoHandler = new eval.Evaluator.LogInfoHandler(this.LogInfo);
                                evaluator.EvalUnityRequestHandler = new eval.Evaluator.UnityRequestHandler(this.UnityRequestProc);

                                StartCoroutine(ProcedureExecutorLoop(procItem_Tree, procItem_Clone, evaluator));
                            }
                            else
                            {
                                //Call sequential
                                yield return StartCoroutine(ProcedureExecutor(procItem_Tree, procItem_Clone, _evaluator));
                            }
                        }
                        else
                        {
                            //LogInfo("[ProcedureExecutor] " + "Procedure " + cmdItem_Clone.Target + " is not exist.");
                            if (cmdItem_Clone.Target != "main")
                            {
                                //2017.11.19
                                if (_QUIT_REQUEST == false)
                                    LogInfo("Error EP: \"" + cmdItem_Clone.Target + "\" function not exist");
                            }
                        }

                        #endregion
                    }
                    else if (cmdItem.TargetType == "print")
                    {
                        #region print

                        CommandItem cmdItem_Clone = cmdItem.Clone();

                        //2016.09.25
                        cmdItem_Clone.Action = ParsingHelper.RestoreSlashWithoutTrim(cmdItem_Clone.Action);

                        yield return StartCoroutine(_engineHelper.ExcuteConditionExprWithLocal(cmdItem_Clone.Action, procItem.LocalVariables, _evaluator));
                        object eval_res = gv.GlobalVariables.res;
                        //object eval_res = _engineHelper.ExcuteConditionExprWithLocal(cmdItem_Clone.Action, procItem.LocalVariables, _evaluator);


                        if (eval_res != null)
                        {
                            string printStr = eval_res.ToString();

                            if (printStr != string.Empty)
                            {
                                if (cmdItem.Target == "print" || cmdItem.Target == "serial.print")
                                    LogPrint(printStr);
                                else
                                {
                                    //println, printline
                                    LogPrintLine(printStr);
                                }
                            }
                        }
                        else
                        {
                            LogInfo("[ProcedureExecutor] " + "Print data is null. ");
                        }

                        #endregion print
                    }

                    #endregion doFlag
                }

                #endregion foreach
                
            }
        }

        #endregion foreach

        if (procItem.FirstToken == "procedure")
        {
            if (procItem.ProcedureRunningInfo != null)
            {
                procItem.ProcedureRunningInfo.RunningProcedureCount--;
            }
        }

        if (procItem.Name != "maindefault")
        {
            foreach (string key in tempLocalVariableList)
                procItem.LocalVariables.Remove(key);
        }
        else
        {
            //if (!_logInfo_called)
            //	LogInfo(" ");  
        }

        
        yield break;
    }



    //###################################################################################



    //###################################################################################
    //Play Tone 명령어 함수
    //###################################################################################

    private int _tone_position = 0;
    private int _tone_samplerate = 44100;
    private float _tone_frequency = 440;


    void Tone_Internal(float freq, float sec)
    {
        AudioSource audio_source = _dumy_audio_source.GetComponent<AudioSource>();

        if (freq == 0 || sec == 0)
        {
            if (audio_source.isPlaying)
                audio_source.Stop();
        }
        else
        {
            _tone_frequency = freq;

            audio_source.clip = AudioClip.Create("MySinusoid", (int)(_tone_samplerate * sec), 1, _tone_samplerate, true, OnAudioRead, OnAudioSetPosition);

            if (audio_source.isPlaying)
                audio_source.Stop();

            audio_source.Play();
        }

    }

    void OnAudioRead(float[] data)
    {
        int count = 0;
        while (count < data.Length)
        {
            data[count] = Mathf.Sign(Mathf.Sin(2 * Mathf.PI * _tone_frequency * _tone_position / _tone_samplerate));
            _tone_position++;
            count++;
        }
    }

    void OnAudioSetPosition(int newPosition)
    {
        _tone_position = newPosition;
    }

    //###################################################################################


}


