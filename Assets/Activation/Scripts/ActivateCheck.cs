using UnityEngine;
using System.Collections;

using System.Net;

public class ActivateCheck : MonoBehaviour
{

    //전역상수는 대문자로 표시
    //전역변수는 소문자 앞에 _붙임
    //외부 전역 변수는 _로 시작하며, 대문자 사용

    private int _screen_width = 1280;
    private int _screen_height = 800;

    //다양한 화면 해상도에서 크기나 위치를 가변적으로 계산하기 위해 비율 변수 선언
    private float _w_ratio = 1.0f;
    private float _h_ratio = 1.0f;
    private float _s_ratio = 1.0f;

    //제품 활성화 이후 이동해야할 씬 이름 및 현재의 콘텐츠 ID
    string _MAIN_SCENE_NAME = "Scene_MainMenu";
    string _CONTENT_ID = "BIGRENSE_ARDUINO";	//10bytes

    //######################################################################
    //외부로 부터 받을 전역 객체를 선언한다.
    //######################################################################
    //공통적으로 사용할 폰트를 외부로 부터 받는다.
    public Font[] _FONTS;
    public Texture2D[] _TEXTURES;
    public Texture2D[] _BTN_TEXTURES;
    public AudioClip[] _GAME_AUDIO_CLIPS;
    //######################################################################


    //######################################################################
    //GUI 처리 전역변수
    //######################################################################
    private Rect[] _bg_rects = new Rect[10];
    private Rect[] _button_rects = new Rect[20];
    private GUIStyle[] _text_styles = new GUIStyle[10];
    private GUIStyle[] _btn_styles = new GUIStyle[20];
    private GUIStyle[] _gui_styles = new GUIStyle[10];
    //######################################################################


    //######################################################################
    //기능 처리를 위한 전역 변수
    //######################################################################
    private GameObject _dumy_audio_source;

    private bool _dialog_mode = false;
    private string _dialog_name = "";
    private string _pre_dialog_name = "";

    //KeyEvent	
    private string _key_down_code = string.Empty;

    //장치별 고유 식별 아이디를 저장함
    private string _device_id = "";


    string _remained_count = "";


    //화면에 기다리라는 메세지 표시하기 위한 용도
    private bool _loading_doing = false;
    private string _error_msg = "";


    //######################################################################
    //제품키 입력 처리를 위한 전역 변수
    //######################################################################
    private Rect[] _textbox_rects = new Rect[24];
    private Rect[] _numpad_rects = new Rect[16];
    private Rect[] _bar_rects = new Rect[5];
    private string[] _numpad_values = new string[16] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "A", "B", "C", "D", "E", "F" };
    private string[] _prod_sn = new string[24];
    private int _cur_input_index = 0;

    private GUIStyle _keypad_box_bg_styles;
    //######################################################################


    private string _BTN_TITLE_YES = "Yes";
    private string _BTN_TITLE_NO = "No";
    private string _BTN_TITLE_OK = "OK";
    private string _BTN_TITLE_BACK = "Back";
    private string _BTN_TITLE_DELETE = "Delete";
    private string _BTN_TITLE_ACTIVATION = "Activation";


    //제품키 활성화 결과 상태를 저장한다.
    //#############################################################
    private string activate_status = "";
    private string _ACTIVATED_MSG = "NEW";
    private string _PROD_KEY = string.Empty;

    private string _INPUT_DIALOG_MSG = string.Empty;
    //################################################################


    //######################################################################
    //Ready
    //######################################################################

    private Rect _logo_rect = new Rect(0, 0, 0, 0);
    private Rect _ready_msg_rect = new Rect(0, 0, 0, 0);
    private Rect _ready_dumy_rect = new Rect(0, 0, 0, 0);


    //화면 페이이인 아웃을 처리하기 위한 젼역 객체
    //##########################################
    FadeScreenLibrary _fadeScreenLibrary = new FadeScreenLibrary();
    //##########################################



    void Start()
    {




        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            Screen.fullScreen = false;
        }


        //이걸 않하면 유니티 프로그램이 뒤로 이동했을 대 실행이 멈춤
        Application.runInBackground = true;


        try
        {
            _screen_width = Screen.width;
            _screen_height = Screen.height;

            _device_id = ActivateUtils.GetDeviceUniqueIdentifier();
            _dumy_audio_source = GameObject.Find("sound_dumy");


            //전역 객체들을 초기화 한다.
            SetupWorks();
            SetupMenuButtons();
            SetupTextStyle();


            //인증 테스트하 때에는 아래 주석을 해제하여 인증 상태를 초기화 해놓고 테스트
            //한 번 실행후 다시 주석처리해야 gka
            //#########################################################################################################
            //PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Activate_Status"), string.Empty);
            //PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Trial_Activate_Status"), string.Empty);
            //PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Last_Exec_Date"), string.Empty);
            //PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Activated_Date"), string.Empty);
            //PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Expired_Date"), string.Empty);
            //#########################################################################################################
            

            //Key Activation
            //#########################################################################################################
            activate_status = ActivateUtils.DecodeFromHexString(PlayerPrefs.GetString(ActivateUtils.EncodeToHexString("Activate_Status")));
            string last_exec_date = ActivateUtils.DecodeFromHexString(PlayerPrefs.GetString(ActivateUtils.EncodeToHexString("Last_Exec_Date")));
            string activated_date = ActivateUtils.DecodeFromHexString(PlayerPrefs.GetString(ActivateUtils.EncodeToHexString("Activated_Date")));
            string expired_date = ActivateUtils.DecodeFromHexString(PlayerPrefs.GetString(ActivateUtils.EncodeToHexString("Expired_Date")));
            //#########################################################################################################


            //어떠한 화면을 표시할 지를 결정
            //맨 처음 화면은 우선 제품키를 입력 받는 화면 상태 값으로 설정
            _dialog_name = "Check";

            if (!string.IsNullOrEmpty(activate_status) && activate_status == (_device_id + _CONTENT_ID))
            {
                //#############################################################################################################

                //일단 정상상태 값으로 초기화
                _ACTIVATED_MSG = "NORMAL";

                bool load_flag = false;

                //마지막 실행 일자를 기록함
                string cur_date = ActivateUtil.GetDateTime23Str();

                //사용자가 시스템 날짜를 수정할 것에 대비하여 현재 날짜가 반드시 마지막 실행 날짜 보다 커야 함
                if (cur_date.CompareTo(last_exec_date) > 0)
                {

                    //만료 일자 체크
                    if (cur_date.CompareTo(expired_date) < 0)
                        load_flag = true;
                    else
                    {
                        _ACTIVATED_MSG = "NORMAL_EXPIRED";
                    }


                    PlayerPrefs.SetString("Last_Exec_Date", ActivateUtils.EncodeToHexString(cur_date));
                }
                else
                {

                    //사용자가 시스템 날짜를 임의로 뒤로 수정한 경우임
                    //데이터를 모두 초기화 시킴
                    _ACTIVATED_MSG = "RESET";
                    PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Activate_Status"), string.Empty);
                    PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Trial_Activate_Status"), string.Empty);
                    PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Last_Exec_Date"), string.Empty);
                    PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Activated_Date"), string.Empty);
                    PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Expired_Date"), string.Empty);
                }
                //#############################################################################################################
                PlayerPrefs.Save();

                //#############################################################################################################
                if (load_flag)
                {
                    _dialog_name = "Ready";
                }
                //#############################################################################################################
            }
            else
            {
                //Do nothing
            }


            //화면을 어두운 상태에서 점점 밝게 한다.
            //##########################################
            _fadeScreenLibrary.StartFadeIn(false);
            //##########################################
        }
        catch (System.Exception ex)
        {
            _error_msg = ex.ToString();
        }
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


        //전체 배경 화면을 그리기 위한 rect
        _bg_rects[0] = new Rect(0, 0, Screen.width, Screen.height);


        //첫화면 다이얼로그 메세지 표시를 위한 rect
        _bg_rects[1] = new Rect(0, 280 * _h_ratio, Screen.width, half_height);


        //제품키 입력 화면 메세지 표시 위치
        _bg_rects[2] = new Rect(0, 180 * _h_ratio, Screen.width, half_height);


        //제품키 입력 제목 아래에 메세지 표시하는 영역. 오류 메세지나 현재 상태 정보 표시. 오렌지색
        _bg_rects[4] = new Rect(0, 240 * _h_ratio, Screen.width, half_height);


        //키보드 백그라운드 박스
        _bg_rects[3] = new Rect(
            70 * _w_ratio,
            420 * _h_ratio,
            _screen_width - (140 * _w_ratio),
            240 * _h_ratio);


        //##############################################################################
        //제품키 입력화면의 입력창 및 버튼 위치 설정
        //##############################################################################
        //TextBox_Rects
        float offset_x = 0;
        float offset_space = 0;

        float offset_x_left = 60 * _w_ratio;
        float text_box_width = 88 * _w_ratio;
        float text_box_height = 90 * _h_ratio;
        float text_box_pad_offset = 2 * _w_ratio;

        float bar_offset = 30 * _w_ratio;
        float bar_offset_x = 72 * _w_ratio;
        float bar_offset_y = 20 * _h_ratio;

        float num_pad_width = 85 * _w_ratio;
        float num_pad_height = 85 * _h_ratio;
        float num_pad_offset = 10 * _w_ratio;
        float num_pad_top = 450 * _h_ratio;


        offset_x = offset_x_left;
        offset_space = 0;

        for (int i = 0; i < 12; i++)
        {
            float p_x = i * (text_box_width + text_box_pad_offset);
            offset_space = (i / 4) * (30 * _h_ratio);

            _textbox_rects[i] = new Rect(offset_x + p_x + offset_space, 310 * _h_ratio, text_box_width, text_box_height);
        }

        bar_offset_x = (_textbox_rects[3].width / 2) + bar_offset;

        //Bar_Rects
        _bar_rects[0] = new Rect(_textbox_rects[3].x + bar_offset_x, _textbox_rects[3].y + bar_offset_y, 50 * _h_ratio, 70 * _h_ratio);
        _bar_rects[1] = new Rect(_textbox_rects[7].x + bar_offset_x, _textbox_rects[7].y + bar_offset_y, 50 * _h_ratio, 70 * _h_ratio);


        //NumPad_Rects
        offset_x = offset_x_left + 60 * _w_ratio;

        for (int i = 0; i < 10; i++)
        {
            float p_x = i * (num_pad_width + num_pad_offset);
            _numpad_rects[i] = new Rect(offset_x + p_x, num_pad_top, num_pad_width, num_pad_width);
        }

        for (int i = 0; i < 6; i++)
        {
            float p_x = i * (num_pad_width + num_pad_offset);
            _numpad_rects[i + 10] = new Rect(offset_x + p_x, num_pad_top + (20 * _h_ratio) + num_pad_height, num_pad_width, num_pad_width);
        }

        //##############################################################################


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


        //OK
        _button_rects[4] = new Rect(0, 0, btn_width, btn_height);
        _button_rects[4].x = screen_center_x - (btn_width * 0.5f);
        _button_rects[4].y = screen_center_y - (20 * _h_ratio);



        //제품키 입력 Delete
        _button_rects[2] = new Rect(0, 0, (_numpad_rects[15].width * 2) + (10 * _w_ratio), _numpad_rects[15].height);
        _button_rects[2].x = _numpad_rects[15].x + _numpad_rects[15].width + (10 * _w_ratio);
        _button_rects[2].y = _numpad_rects[15].y;

        //제품키 입력 Activation
        _button_rects[3] = new Rect(0, 0, _button_rects[2].width, _numpad_rects[15].height);
        _button_rects[3].x = _button_rects[2].x + _button_rects[2].width + (10 * _w_ratio);
        _button_rects[3].y = _button_rects[2].y;
        //##############################################################################



        //################################################################################
        //Ready
        //################################################################################

        _logo_rect.width = _screen_width * 0.2f * 1.15f;
        _logo_rect.height = _screen_width * 0.2f;

        _logo_rect.x = half_width - (_logo_rect.width * 0.5f);
        _logo_rect.y = _screen_height * 0.1f;

        _ready_msg_rect.width = _screen_width;
        _ready_msg_rect.height = 50 * _s_ratio;
        _ready_msg_rect.x = 0;
        _ready_msg_rect.y = _logo_rect.y + _logo_rect.height + (50 * _s_ratio);

        _ready_dumy_rect.width = _ready_msg_rect.width;
        _ready_dumy_rect.height = _ready_msg_rect.height;
        _ready_dumy_rect.x = _ready_msg_rect.x;
        _ready_dumy_rect.y = _ready_msg_rect.y;
    }


    void SetupMenuButtons()
    {
        //투명 버튼
        _btn_styles[0] = new GUIStyle();

        //White Bg Black Text
        _btn_styles[1] = new GUIStyle();
        _btn_styles[1].normal.background = _BTN_TEXTURES[0];
        _btn_styles[1].hover.background = _BTN_TEXTURES[0];
        _btn_styles[1].active.background = _BTN_TEXTURES[1];
        _btn_styles[1].alignment = TextAnchor.MiddleCenter;
        _btn_styles[1].font = _FONTS[0];
        _btn_styles[1].fontStyle = FontStyle.Bold;
        _btn_styles[1].normal.textColor = Color.grey;
        _btn_styles[1].fontSize = (int)(32 * _w_ratio);


        //White Bg Orange Text
        _btn_styles[2] = new GUIStyle();
        _btn_styles[2].normal.background = _BTN_TEXTURES[0];
        _btn_styles[2].hover.background = _BTN_TEXTURES[0];
        _btn_styles[2].active.background = _BTN_TEXTURES[1];
        _btn_styles[2].alignment = TextAnchor.MiddleCenter;
        _btn_styles[2].font = _FONTS[0];
        _btn_styles[2].fontStyle = FontStyle.Bold;
        _btn_styles[2].normal.textColor = new Color(1, 0.5f, 0);
        _btn_styles[2].fontSize = (int)(32 * _w_ratio);


        //제품키 기본 입력창
        _btn_styles[3] = new GUIStyle();
        _btn_styles[3].normal.background = _BTN_TEXTURES[6];
        _btn_styles[3].alignment = TextAnchor.MiddleCenter;
        _btn_styles[3].font = _FONTS[0];
        _btn_styles[3].fontStyle = FontStyle.Bold;
        _btn_styles[3].normal.textColor = new Color(1, 0.5f, 0);
        _btn_styles[3].fontSize = (int)(32 * _w_ratio);


        //제품키 값이 있는 입력창
        _btn_styles[4] = new GUIStyle();
        _btn_styles[4].normal.background = _BTN_TEXTURES[7];
        _btn_styles[4].alignment = TextAnchor.MiddleCenter;
        _btn_styles[4].font = _FONTS[0];
        _btn_styles[4].fontStyle = FontStyle.Bold;
        _btn_styles[4].normal.textColor = new Color(1, 0.5f, 0);
        _btn_styles[4].fontSize = (int)(32 * _w_ratio);



        //제품키 입력 원형 버튼
        _btn_styles[5] = new GUIStyle();
        _btn_styles[5].normal.background = _BTN_TEXTURES[8];
        _btn_styles[5].hover.background = _BTN_TEXTURES[8];
        _btn_styles[5].active.background = _BTN_TEXTURES[9];
        _btn_styles[5].alignment = TextAnchor.MiddleCenter;
        _btn_styles[5].font = _FONTS[0];
        _btn_styles[5].fontStyle = FontStyle.Bold;
        _btn_styles[5].normal.textColor = new Color(1, 1, 1);
        _btn_styles[5].fontSize = (int)(24 * _w_ratio);


        //제품키 입력 Delete 버튼
        _btn_styles[6] = new GUIStyle();
        _btn_styles[6].normal.background = _BTN_TEXTURES[10];
        _btn_styles[6].hover.background = _BTN_TEXTURES[10];
        _btn_styles[6].active.background = _BTN_TEXTURES[11];
        _btn_styles[6].alignment = TextAnchor.MiddleCenter;
        _btn_styles[6].font = _FONTS[0];
        _btn_styles[6].fontStyle = FontStyle.Bold;
        _btn_styles[6].normal.textColor = new Color(1, 1, 1);
        _btn_styles[6].fontSize = (int)(24 * _w_ratio);


        //제품키 입력 Activation 버튼
        _btn_styles[7] = new GUIStyle();
        _btn_styles[7].normal.background = _BTN_TEXTURES[12];
        _btn_styles[7].hover.background = _BTN_TEXTURES[12];
        _btn_styles[7].active.background = _BTN_TEXTURES[13];
        _btn_styles[7].alignment = TextAnchor.MiddleCenter;
        _btn_styles[7].font = _FONTS[0];
        _btn_styles[7].fontStyle = FontStyle.Bold;
        _btn_styles[7].normal.textColor = V2.Utils.ToFloatColor(54, 78, 123);
        _btn_styles[7].fontSize = (int)(24 * _w_ratio);




        //회색 No 버튼 스타일
        _btn_styles[8] = new GUIStyle();
        _btn_styles[8].normal.background = _BTN_TEXTURES[2];
        _btn_styles[8].hover.background = _BTN_TEXTURES[2];
        _btn_styles[8].active.background = _BTN_TEXTURES[3];
        _btn_styles[8].alignment = TextAnchor.MiddleCenter;
        _btn_styles[8].font = _FONTS[0];
        _btn_styles[8].fontStyle = FontStyle.Bold;
        _btn_styles[8].normal.textColor = new Color(1, 1, 1);
        _btn_styles[8].fontSize = (int)(32 * _w_ratio);


        //오렌지색 Yes 버튼 스타일
        _btn_styles[9] = new GUIStyle();
        _btn_styles[9].normal.background = _BTN_TEXTURES[4];
        _btn_styles[9].hover.background = _BTN_TEXTURES[4];
        _btn_styles[9].active.background = _BTN_TEXTURES[5];
        _btn_styles[9].alignment = TextAnchor.MiddleCenter;
        _btn_styles[9].font = _FONTS[0];
        _btn_styles[9].fontStyle = FontStyle.Bold;
        _btn_styles[9].normal.textColor = new Color(1, 1, 1);
        _btn_styles[9].fontSize = (int)(32 * _w_ratio);

        if (Application.systemLanguage == SystemLanguage.Korean)
        {
            foreach (GUIStyle style in _btn_styles)
            {
                if (style != null)
                {
                    style.font = _FONTS[1];
                }
            }

            _BTN_TITLE_YES = "예";
            _BTN_TITLE_NO = "아니오";
            _BTN_TITLE_OK = "확인";
            _BTN_TITLE_BACK = "뒤로가기";
            _BTN_TITLE_DELETE = "삭제";
            _BTN_TITLE_ACTIVATION = "활성화";
        }
    }


    void SetupTextStyle()
    {
        //화면 가운데 상단의 제목 글자
        _text_styles[0] = new GUIStyle();
        _text_styles[0].font = _FONTS[0];
        _text_styles[0].fontStyle = FontStyle.Bold;
        _text_styles[0].normal.textColor = Color.black;
        _text_styles[0].fontSize = (int)(38 * _s_ratio);
        _text_styles[0].alignment = TextAnchor.UpperCenter;
        _text_styles[0].contentOffset = new Vector2(0, 0);


        //화면 가운데 상단 제목 아래의 정보 표시용 메세지. 오렌지색
        _text_styles[1] = new GUIStyle();
        _text_styles[1].font = _FONTS[0];
        _text_styles[1].fontStyle = FontStyle.Bold;
        _text_styles[1].normal.textColor = V2.Utils.ToFloatColor(233, 113, 66);
        _text_styles[1].fontSize = (int)(28 * _s_ratio);
        _text_styles[1].alignment = TextAnchor.UpperCenter;
        _text_styles[1].contentOffset = new Vector2(0, 0);


        //Ready 화면 메세지
        _text_styles[2] = new GUIStyle();
        _text_styles[2].font = _FONTS[0];
        _text_styles[2].fontStyle = FontStyle.Bold;
        _text_styles[2].normal.textColor = V2.Utils.ToFloatColor(135, 172, 220);
        _text_styles[2].fontSize = (int)(24 * _s_ratio);
        _text_styles[2].alignment = TextAnchor.UpperCenter;
        _text_styles[2].contentOffset = new Vector2(0, 0);


        _keypad_box_bg_styles = new GUIStyle();
        _keypad_box_bg_styles.border = new RectOffset(50, 50, 50, 50);
        _keypad_box_bg_styles.normal.background = _TEXTURES[4];
    }


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
    }


    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@


    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

    void OnGUI()
    {
        if (_dialog_name == "Ready")
        {
            GUI_Ready();
        }
        else if (_dialog_name == "Close")
        {
            GUI_Close();
        }
        else if (_dialog_name == "Check")
        {
            GUI_Check();
        }
        else if (_dialog_name == "Input")
        {
            GUI_Input();
        }
        else if (_dialog_name == "Submit")
        {
            GUI_Submit();
        }
        else if (_dialog_name == "SUCCESS1")
        {
            GUI_SUCCESS1();
        }
        else if (_dialog_name == "SUCCESS2")
        {
            GUI_SUCCESS2();
        }
        else if (_dialog_name == "NoKey")
        {
            GUI_NoKey();
        }
        else if (_dialog_name == "Network")
        {
            GUI_Network();
        }
        else if (_dialog_name == "DUPLICATE")
        {
            GUI_DUPLICATE();
        }
        else if (_dialog_name == "OVER")
        {
            GUI_OVER();
        }
        else if (_dialog_name == "NOTALLOWED")
        {
            GUI_NOTALLOWED();
        }
        else if (_dialog_name == "EXPIRE")
        {
            GUI_EXPIRE();
        }
        else if (_dialog_name == "OK")
        {
            GUI_OK();
        }
        else if (_dialog_name == "Start")
        {
            _loading_doing = true;
            _dialog_name = "";
            UnityEngine.SceneManagement.SceneManager.LoadScene(_MAIN_SCENE_NAME);
        }


        //#######################################################################
        //키보드 처리를 한다
        //#######################################################################

        if (_key_down_code == "Escape")
        {
            if (_dialog_name == "Close")
            {
                //이전의 화면으로 돌아간다,

                //##########################################            
                _fadeScreenLibrary.RequestFadeOut(_pre_dialog_name, true);
                //##########################################                
            }
            else
            {
                //현재의 화면 이름을 저장한다.
                _pre_dialog_name = _dialog_name;

                //##########################################            
                _fadeScreenLibrary.RequestFadeOut("Close", true);
                //##########################################     
            }

            _key_down_code = "";
        }



        //#######################################################################
        //화면 페이드인 아웃을 그려준다.
        //#######################################################################

        _fadeScreenLibrary.DoFadeGUI();

        //화면 페이드 처리가 끝났으면 그 때 해당 씬이나 화면으로 이동한다,
        string fade_res = _fadeScreenLibrary.CheckFadeOutCompleted();

        //페이드 처리 요청받은 화면의 이름을 비교하여 해당 기능 수행
        if (fade_res == "Input")
        {
            _dialog_name = "Input";

            //다시 화면을 밝게 처리한다.
            _fadeScreenLibrary.StartFadeIn(true);
        }
        else if (fade_res == "Check")
        {
            _dialog_name = "Check";

            //다시 화면을 밝게 처리한다.
            _fadeScreenLibrary.StartFadeIn(true);
        }
        else if (fade_res == "Submit")
        {
            _dialog_name = "Submit";

            //다시 화면을 밝게 처리한다.
            _fadeScreenLibrary.StartFadeIn(true);


            string url = "http://bigrense.azurewebsites.net/api/activate_big.aspx";

            WWWForm form = new WWWForm();
            form.AddField("mode", "activate");
            form.AddField("pkey", _PROD_KEY);
            form.AddField("prod_type", "arduino");
            form.AddField("provider", "bigrense");
            form.AddField("cont_id", _CONTENT_ID);
            form.AddField("device_id", _device_id);
            WWW www = new WWW(url, form);
            

            StartCoroutine(WaitForRequest(www));

        }
        else if (fade_res == "Ready")
        {
            _dialog_name = "Ready";

            //다시 화면을 밝게 처리한다.
            _fadeScreenLibrary.StartFadeIn(true);
        }
        else if (fade_res == "Close")
        {
            _dialog_name = "Close";

            //다시 화면을 밝게 처리한다.
            _fadeScreenLibrary.StartFadeIn(true);
        }
        else if (fade_res == "Scene_MainMenu")
        {
            _dialog_name = "";
            UnityEngine.SceneManagement.SceneManager.LoadScene("Scene_MainMenu");
        }
        else if (fade_res == "Quit")
        {
            _dialog_name = "";
            _key_down_code = "";
            Application.Quit();
        }
        //#######################################################################

    }

    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@


    void GUI_Ready()
    {
        GUI.DrawTexture(_bg_rects[0], _TEXTURES[0]);

        GUI.DrawTexture(_logo_rect, _TEXTURES[5]);

        float bounce_y = EasingFunction.EaseInBounce(0, 20 * _h_ratio, Mathf.PingPong(Time.time, 2.0f));
        _ready_dumy_rect.y = _ready_msg_rect.y - bounce_y;

        string TouchScreenString = "Please touch anywhere on the screen.";
        if (Application.systemLanguage == SystemLanguage.Korean)
        {
            _text_styles[2].font = _FONTS[1];
            TouchScreenString = "화면의 아무 곳이나 터치하세요.";
        }
        GUI.Label(_ready_dumy_rect, TouchScreenString, _text_styles[2]);


        if (GUI.Button(_bg_rects[0], "", _btn_styles[0]))
        {
            _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f); //button sound

            //##########################################            
            _fadeScreenLibrary.RequestFadeOut("Scene_MainMenu", true);
            //##########################################
        }
    }


    void GUI_Check()
    {

        GUI.DrawTexture(_bg_rects[0], _TEXTURES[1]);

        string ActivationString = "This App Requires Activation.";
        if (Application.systemLanguage == SystemLanguage.Korean)
        {
            _text_styles[0].font = _FONTS[1];
            ActivationString = "이 앱은 활성화가 필요합니다.";
        }
        GUI.Label(_bg_rects[1], ActivationString, _text_styles[0]);


        //No
        if (GUI.Button(_button_rects[0], _BTN_TITLE_NO, _btn_styles[1]))
        {
            _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);	//button sound

            //현재의 화면 이름을 저장한다.
            _pre_dialog_name = _dialog_name;

            //##########################################            
            _fadeScreenLibrary.RequestFadeOut("Close", true);
            //##########################################            
        }


        //Yes
        if (GUI.Button(_button_rects[1], _BTN_TITLE_YES, _btn_styles[2]))
        {
            _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);	//button sound

            //##########################################            
            _fadeScreenLibrary.RequestFadeOut("Input", true);
            //##########################################            
        }
    }


    void GUI_Close()
    {
        GUI.DrawTexture(_bg_rects[0], _TEXTURES[1]);
        GUI.DrawTexture(_bg_rects[0], _TEXTURES[3]);

        string CloseString = "Are you sure you want to close?";
        if (Application.systemLanguage == SystemLanguage.Korean)
        {
            _text_styles[0].font = _FONTS[1];
            CloseString = "종료하시겠습니까?";
        }

        GUI.Label(_bg_rects[1], CloseString, _text_styles[0]);


        //No
        if (GUI.Button(_button_rects[0], _BTN_TITLE_NO, _btn_styles[8]))
        {
            _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);	//button sound

            //##########################################            
            _fadeScreenLibrary.RequestFadeOut(_pre_dialog_name, true);
            //##########################################            
        }


        //Yes
        if (GUI.Button(_button_rects[1], _BTN_TITLE_YES, _btn_styles[9]))
        {
            _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);	//button sound

            //##########################################            
            _fadeScreenLibrary.RequestFadeOut("Quit", true);
            //##########################################            
        }
    }


    void GUI_Input()
    {
        GUI.DrawTexture(_bg_rects[0], _TEXTURES[2]);

        string SerialString = "Please Type Serial Number";
        if (Application.systemLanguage == SystemLanguage.Korean)
        {
            _text_styles[0].font = _FONTS[1];
            SerialString = "제품키를 입력해주세요.";
        }

        GUI.Label(_bg_rects[2], SerialString, _text_styles[0]);


        //오렌지색 메세지 표시
        GUI.Label(_bg_rects[4], _INPUT_DIALOG_MSG, _text_styles[1]);


        //키보드 백그라운드 박스
        GUI.Box(_bg_rects[3], "", _keypad_box_bg_styles);


        //현재 입력된 제품키 표시 레이블
        for (int i = 0; i < 12; i++)
        {
            if (string.IsNullOrEmpty(_prod_sn[i]))
                GUI.Label(_textbox_rects[i], _prod_sn[i], _btn_styles[3]);
            else
                GUI.Label(_textbox_rects[i], _prod_sn[i], _btn_styles[4]);
        }

        //제품키 중간에 데시 기호
        for (int i = 0; i < 2; i++)
        {
            GUI.Label(_bar_rects[i], "-", _text_styles[0]);
        }


        //제품키 입력 버튼 - 16진수
        for (int i = 0; i < 16; i++)
        {
            if (GUI.Button(_numpad_rects[i], _numpad_values[i], _btn_styles[5]))
            {
                if (_cur_input_index < 12)
                {
                    _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);	//button sound
                    _prod_sn[_cur_input_index] = _numpad_values[i];
                    _cur_input_index++;

                    _INPUT_DIALOG_MSG = "";
                }
                else
                {
                    _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[1], 1.0f);	//button sound
                    _cur_input_index = 12;

                    _INPUT_DIALOG_MSG = "The length of product key should be 12";
                    if (Application.systemLanguage == SystemLanguage.Korean)
                    {
                        _text_styles[1].font = _FONTS[1];
                        _INPUT_DIALOG_MSG = "제품키는 12자리 입니다.";
                    }
                }

            }
        }


        //Delete
        if (GUI.Button(_button_rects[2], _BTN_TITLE_DELETE, _btn_styles[6]))
        {
            _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);	//button sound

            _cur_input_index--;

            if (_cur_input_index >= 0)
                _prod_sn[_cur_input_index] = "";
            else
                _cur_input_index = 0;

            _INPUT_DIALOG_MSG = "";
        }



        //Activate
        if (GUI.Button(_button_rects[3], _BTN_TITLE_ACTIVATION, _btn_styles[7]))
        {

            if (_cur_input_index >= 12)
            {
                _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);	//button sound

                //_dialog_name = "Submit";

                string prod_key = "";

                for (int i = 0; i < 12; i++)
                {
                    prod_key += _prod_sn[i];

                    if (i == 3 || i == 7)
                        prod_key += "-";
                }


                _PROD_KEY = prod_key;

                _INPUT_DIALOG_MSG = "";


                //##########################################            
                _fadeScreenLibrary.RequestFadeOut("Submit", true);
                //##########################################    

                /*
                //서버작업 전까지 임시로 적용
                //########################################################################################################
                //########################################################################################################
                PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Activate_Status"), ActivateUtils.EncodeToHexString(_device_id + _CONTENT_ID));

                string cur_date = ActivateUtil.GetDateTime23Str();
                PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Last_Exec_Date"), ActivateUtils.EncodeToHexString(cur_date));
                PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Activated_Date"), ActivateUtils.EncodeToHexString(cur_date));

                //하루 단위로 구독 확인하도록 함
                System.DateTime nn = System.DateTime.Now;
                nn = nn.AddDays(1);
                string ExpDateStr = ActivateUtil.GetDateTime23Str(nn);
                PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Expired_Date"), ActivateUtils.EncodeToHexString(ExpDateStr));

                //##########################################            
                _fadeScreenLibrary.RequestFadeOut("Ready", true);
                //##########################################
                */


                //########################################################################################################
                //########################################################################################################
            }
            else
            {
                _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[1], 1.0f); //button sound

                _INPUT_DIALOG_MSG = "The length of product key should be 12";
                if (Application.systemLanguage == SystemLanguage.Korean)
                {
                    _text_styles[1].font = _FONTS[1];
                    _INPUT_DIALOG_MSG = "제품키는 12자리 입니다.";
                }
            }
        }


    }


    void GUI_OK()
    {
        //
    }




    IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

        _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[1], 1.0f);	//button sound
        
        // check for errors
        if (www.error == null)
        {
            if (www.text.StartsWith("OK"))
            {
                _dialog_name = "SUCCESS1";

                //#############################################################################################################
                PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Activate_Status"), ActivateUtils.EncodeToHexString(_device_id + _CONTENT_ID));

                string cur_date = ActivateUtil.GetDateTime23Str();
                PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Last_Exec_Date"), ActivateUtils.EncodeToHexString(cur_date));
                PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Activated_Date"), ActivateUtils.EncodeToHexString(cur_date));

                if (_PROD_KEY.StartsWith("1D"))
                {
                    //하루 단위로 구독 확인하도록 함
                    System.DateTime nn = System.DateTime.Now;
                    nn = nn.AddDays(1);
                    string ExpDateStr = ActivateUtil.GetDateTime23Str(nn);
                    PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Expired_Date"), ActivateUtils.EncodeToHexString(ExpDateStr));
                }
                else if (_PROD_KEY.StartsWith("3D"))
                {
                    //하루 단위로 구독 확인하도록 함
                    System.DateTime nn = System.DateTime.Now;
                    nn = nn.AddDays(3);
                    string ExpDateStr = ActivateUtil.GetDateTime23Str(nn);
                    PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Expired_Date"), ActivateUtils.EncodeToHexString(ExpDateStr));
                }
                else if (_PROD_KEY.StartsWith("7D"))
                {
                    //7일 단위로 구독 확인하도록 함
                    System.DateTime nn = System.DateTime.Now;
                    nn = nn.AddDays(7);
                    string ExpDateStr = ActivateUtil.GetDateTime23Str(nn);
                    PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Expired_Date"), ActivateUtils.EncodeToHexString(ExpDateStr));
                }
                else if (_PROD_KEY.StartsWith("1A"))
                {
                    //한달 단위로 구독 확인하도록 함
                    System.DateTime nn = System.DateTime.Now;
                    nn = nn.AddMonths(1);
                    string ExpDateStr = ActivateUtil.GetDateTime23Str(nn);
                    PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Expired_Date"), ActivateUtils.EncodeToHexString(ExpDateStr));
                }
                else if (_PROD_KEY.StartsWith("3A"))
                {
                    //3개월 단위로 구독 확인하도록 함
                    System.DateTime nn = System.DateTime.Now;
                    nn = nn.AddMonths(3);
                    string ExpDateStr = ActivateUtil.GetDateTime23Str(nn);
                    PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Expired_Date"), ActivateUtils.EncodeToHexString(ExpDateStr));
                }
                else if (_PROD_KEY.StartsWith("6A"))
                {
                    //3개월 단위로 구독 확인하도록 함
                    System.DateTime nn = System.DateTime.Now;
                    nn = nn.AddMonths(6);
                    string ExpDateStr = ActivateUtil.GetDateTime23Str(nn);
                    PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Expired_Date"), ActivateUtils.EncodeToHexString(ExpDateStr));
                }
                else if (_PROD_KEY.StartsWith("12A"))
                {
                    //3개월 단위로 구독 확인하도록 함
                    System.DateTime nn = System.DateTime.Now;
                    nn = nn.AddMonths(12);
                    string ExpDateStr = ActivateUtil.GetDateTime23Str(nn);
                    PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Expired_Date"), ActivateUtils.EncodeToHexString(ExpDateStr));
                }
                else
                {
                    //한달 단위로 구독 확인하도록 함
                    System.DateTime nn = System.DateTime.Now;
                    nn = nn.AddMonths(1);
                    string ExpDateStr = ActivateUtil.GetDateTime23Str(nn);
                    PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Expired_Date"), ActivateUtils.EncodeToHexString(ExpDateStr));
                }
                //#############################################################################################################
            }
            else if (www.text.StartsWith("CNT"))
            {
                //PlayerPrefs.SetString("Activate_Status", ActivateUtils.EncodeToHexString(device_id + CONTENT_ID));

                //#############################################################################################################
                PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Activate_Status"), ActivateUtils.EncodeToHexString(_device_id + _CONTENT_ID));

                string cur_date = ActivateUtil.GetDateTime23Str();
                PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Last_Exec_Date"), ActivateUtils.EncodeToHexString(cur_date));
                PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Activated_Date"), ActivateUtils.EncodeToHexString(cur_date));

                if (_PROD_KEY.StartsWith("1D"))
                {
                    //하루 단위로 구독 확인하도록 함
                    System.DateTime nn = System.DateTime.Now;
                    nn = nn.AddDays(1);
                    string ExpDateStr = ActivateUtil.GetDateTime23Str(nn);
                    PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Expired_Date"), ActivateUtils.EncodeToHexString(ExpDateStr));
                }
                else if (_PROD_KEY.StartsWith("3D"))
                {
                    //하루 단위로 구독 확인하도록 함
                    System.DateTime nn = System.DateTime.Now;
                    nn = nn.AddDays(3);
                    string ExpDateStr = ActivateUtil.GetDateTime23Str(nn);
                    PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Expired_Date"), ActivateUtils.EncodeToHexString(ExpDateStr));
                }
                else if (_PROD_KEY.StartsWith("7D"))
                {
                    //7일 단위로 구독 확인하도록 함
                    System.DateTime nn = System.DateTime.Now;
                    nn = nn.AddDays(7);
                    string ExpDateStr = ActivateUtil.GetDateTime23Str(nn);
                    PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Expired_Date"), ActivateUtils.EncodeToHexString(ExpDateStr));
                }
                else if (_PROD_KEY.StartsWith("1A"))
                {
                    //한달 단위로 구독 확인하도록 함
                    System.DateTime nn = System.DateTime.Now;
                    nn = nn.AddMonths(1);
                    string ExpDateStr = ActivateUtil.GetDateTime23Str(nn);
                    PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Expired_Date"), ActivateUtils.EncodeToHexString(ExpDateStr));
                }
                else if (_PROD_KEY.StartsWith("3A"))
                {
                    //3개월 단위로 구독 확인하도록 함
                    System.DateTime nn = System.DateTime.Now;
                    nn = nn.AddMonths(3);
                    string ExpDateStr = ActivateUtil.GetDateTime23Str(nn);
                    PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Expired_Date"), ActivateUtils.EncodeToHexString(ExpDateStr));
                }
                else if (_PROD_KEY.StartsWith("6A"))
                {
                    //3개월 단위로 구독 확인하도록 함
                    System.DateTime nn = System.DateTime.Now;
                    nn = nn.AddMonths(6);
                    string ExpDateStr = ActivateUtil.GetDateTime23Str(nn);
                    PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Expired_Date"), ActivateUtils.EncodeToHexString(ExpDateStr));
                }
                else if (_PROD_KEY.StartsWith("12A"))
                {
                    //3개월 단위로 구독 확인하도록 함
                    System.DateTime nn = System.DateTime.Now;
                    nn = nn.AddMonths(12);
                    string ExpDateStr = ActivateUtil.GetDateTime23Str(nn);
                    PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Expired_Date"), ActivateUtils.EncodeToHexString(ExpDateStr));
                }
                else
                {
                    //한달 단위로 구독 확인하도록 함
                    System.DateTime nn = System.DateTime.Now;
                    nn = nn.AddMonths(1);
                    string ExpDateStr = ActivateUtil.GetDateTime23Str(nn);
                    PlayerPrefs.SetString(ActivateUtils.EncodeToHexString("Expired_Date"), ActivateUtils.EncodeToHexString(ExpDateStr));
                }
                //#############################################################################################################

                _remained_count = "";

                for (int i = 4; i < www.text.Length; i++)
                {
                    if (www.text[i] != ':')
                        _remained_count += www.text[i];
                    else
                        break;
                }

                _dialog_name = "SUCCESS2";
            }
            else if (www.text.StartsWith("ERROR") || www.text.StartsWith("Error"))
            {
                _dialog_name = "NoKey";
            }
            else if (www.text.StartsWith("DUPLICATE"))
            {
                _dialog_name = "DUPLICATE";
            }
            else if (www.text.StartsWith("OVER"))
            {
                _dialog_name = "OVER";
            }
            else if (www.text.StartsWith("NOTALLOWED"))
            {
                _dialog_name = "NOTALLOWED";
            }
            else if (www.text.StartsWith("EXPIRE"))
            {
                _dialog_name = "EXPIRE";
            }
        }
        else
        {
            _dialog_name = "Network";
            _error_msg = www.error;
        }

    }


    void GUI_Submit()
    {
        GUI.DrawTexture(_bg_rects[0], _TEXTURES[1]);
        GUI.DrawTexture(_bg_rects[0], _TEXTURES[3]);

        string WaitString = "Please Wait...";
        if (Application.systemLanguage == SystemLanguage.Korean)
        {
            _text_styles[0].font = _FONTS[1];
            WaitString = "잠시만 기다려주세요.";
        }
        GUI.Label(_bg_rects[1], WaitString, _text_styles[0]);
    }


    void GUI_SUCCESS1()
    {
        //###########################################################################
        string expired_date = ActivateUtils.DecodeFromHexString(PlayerPrefs.GetString(ActivateUtils.EncodeToHexString("Expired_Date")));
        System.DateTime exp_date = ActivateUtil.GetDateTimeFrom23Str(expired_date);
        int diff_days = (int)((exp_date - System.DateTime.Now).TotalDays);


        GUI.DrawTexture(_bg_rects[0], _TEXTURES[1]);
        GUI.DrawTexture(_bg_rects[0], _TEXTURES[3]);

        string successfullyString = "Your product was activated successfully! ";
        if (Application.systemLanguage == SystemLanguage.Korean)
        {
            _text_styles[0].font = _FONTS[1];
            successfullyString = "성공적으로 인증되었습니다! ";
        }
        GUI.Label(_bg_rects[2], successfullyString + diff_days.ToString() + "days left.", _text_styles[0]);
        //###########################################################################


        //OK
        if (GUI.Button(_button_rects[4], _BTN_TITLE_OK, _btn_styles[9]))
        {
            _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);	//button sound

            //##########################################            
            _fadeScreenLibrary.RequestFadeOut("Ready", true);
            //##########################################    
        }

    }

    void GUI_SUCCESS2()
    {
        //###########################################################################
        string expired_date = ActivateUtils.DecodeFromHexString(PlayerPrefs.GetString(ActivateUtils.EncodeToHexString("Expired_Date")));
        System.DateTime exp_date = ActivateUtil.GetDateTimeFrom23Str(expired_date);
        int diff_days = (int)((exp_date - System.DateTime.Now).TotalDays);

        GUI.DrawTexture(_bg_rects[0], _TEXTURES[1]);
        GUI.DrawTexture(_bg_rects[0], _TEXTURES[3]);

        string successfullyString = "Your product was activated successfully! ";
        if (Application.systemLanguage == SystemLanguage.Korean)
        {
            _text_styles[0].font = _FONTS[1];
            successfullyString = "성공적으로 인증되었습니다! ";
        }
        GUI.Label(_bg_rects[2], successfullyString + diff_days.ToString() + "days left.", _text_styles[0]);
        //###########################################################################

        //OK
        if (GUI.Button(_button_rects[4], _BTN_TITLE_OK, _btn_styles[9]))
        {
            _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f); //button sound

            //##########################################            
            _fadeScreenLibrary.RequestFadeOut("Ready", true);
            //##########################################    
        }
    }


    void GUI_NoKey()
    {
        GUI.DrawTexture(_bg_rects[0], _TEXTURES[1]);
        GUI.DrawTexture(_bg_rects[0], _TEXTURES[3]);


        string NotExistString = "Your product key does not exist!";
        if (Application.systemLanguage == SystemLanguage.Korean)
        {
            _text_styles[0].font = _FONTS[1];
            NotExistString = "제품키가 존재하지 않습니다.";
        }
        GUI.Label(_bg_rects[2], NotExistString, _text_styles[0]);


        //Previous
        if (GUI.Button(_button_rects[4], _BTN_TITLE_BACK, _btn_styles[9]))
        {
            _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);	//button sound

            //##########################################            
            _fadeScreenLibrary.RequestFadeOut("Input", true);
            //##########################################    
        }
    }


    void GUI_Network()
    {
        GUI.DrawTexture(_bg_rects[0], _TEXTURES[1]);
        GUI.DrawTexture(_bg_rects[0], _TEXTURES[3]);

        string InternetString = "Check your internet connection!";
        if (Application.systemLanguage == SystemLanguage.Korean)
        {
            _text_styles[0].font = _FONTS[1];
            InternetString = "인터넷 연결을 확인해주세요.";
        }
        GUI.Label(_bg_rects[2], InternetString, _text_styles[0]);


        //Previous
        if (GUI.Button(_button_rects[4], _BTN_TITLE_BACK, _btn_styles[9]))
        {
            _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);	//button sound

            //##########################################            
            _fadeScreenLibrary.RequestFadeOut("Input", true);
            //##########################################    
        }
    }

    void GUI_DUPLICATE()
    {
        GUI.DrawTexture(_bg_rects[0], _TEXTURES[1]);
        GUI.DrawTexture(_bg_rects[0], _TEXTURES[3]);

        string InternetString = "Your key is already used. Please type another key!";
        if (Application.systemLanguage == SystemLanguage.Korean)
        {
            _text_styles[0].font = _FONTS[1];
            InternetString = "이 제품 키는 이미 사용되었습니다. 다른 제품 키를 입력해주세요.";
        }
        GUI.Label(_bg_rects[2], InternetString, _text_styles[0]);


        //Previous
        if (GUI.Button(_button_rects[4], _BTN_TITLE_BACK, _btn_styles[9]))
        {
            _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);	//button sound

            //##########################################            
            _fadeScreenLibrary.RequestFadeOut("Input", true);
            //##########################################    
        }
    }

    void GUI_OVER()
    {
        GUI.DrawTexture(_bg_rects[0], _TEXTURES[1]);
        GUI.DrawTexture(_bg_rects[0], _TEXTURES[3]);

        string ExceedsString = "Your key is already used. Please type another key!";
        if (Application.systemLanguage == SystemLanguage.Korean)
        {
            _text_styles[0].font = _FONTS[1];
            ExceedsString = "제품 키가 허용된 수를 초과했습니다. 다른 제품 키를 입력하세요.";
        }
        GUI.Label(_bg_rects[2], ExceedsString, _text_styles[0]);


        //Previous
        if (GUI.Button(_button_rects[4], _BTN_TITLE_BACK, _btn_styles[9]))
        {
            _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);	//button sound

            //##########################################            
            _fadeScreenLibrary.RequestFadeOut("Input", true);
            //##########################################    
        }
    }


    void GUI_NOTALLOWED()
    {
        GUI.DrawTexture(_bg_rects[0], _TEXTURES[1]);
        GUI.DrawTexture(_bg_rects[0], _TEXTURES[3]);


        string NotAllowededString = "Your key does not alloweded for this product. Please type another key!";
        if (Application.systemLanguage == SystemLanguage.Korean)
        {
            _text_styles[0].font = _FONTS[1];
            NotAllowededString = "허용되지 않은 제품 키입니다. 다른 제품 키를 입력해주세요.";
        }
        GUI.Label(_bg_rects[2], NotAllowededString, _text_styles[0]);


        //Previous
        if (GUI.Button(_button_rects[4], _BTN_TITLE_BACK, _btn_styles[9]))
        {
            _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);	//button sound

            //##########################################            
            _fadeScreenLibrary.RequestFadeOut("Input", true);
            //##########################################    
        }

    }


    void GUI_EXPIRE()
    {
        GUI.DrawTexture(_bg_rects[0], _TEXTURES[1]);
        GUI.DrawTexture(_bg_rects[0], _TEXTURES[3]);

        string PeriodString = "Your key exceeds allowed period. Please type another key!";
        if (Application.systemLanguage == SystemLanguage.Korean)
        {
            _text_styles[0].font = _FONTS[1];
            PeriodString = "사용기한이 지났습니다. 다른 키를 입력해주세요.";
        }
        GUI.Label(_bg_rects[2], PeriodString, _text_styles[0]);


        if (GUI.Button(_button_rects[4], _BTN_TITLE_BACK, _btn_styles[9]))
        {
            _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);	//button sound

            //##########################################            
            _fadeScreenLibrary.RequestFadeOut("Input", true);
            //##########################################    
        }
    }



}
