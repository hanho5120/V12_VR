using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using gv;

public class SPLCodeEditor : MonoBehaviour
{
    //######################################################################
    //내부적으로 사용되는 글로벌 전역변수
    //######################################################################

    //각 스테이지로 실행했다가 다시 되돌아 올 때 이전 상태를 유지한다.
    public static string _EDITOR_TYPE = "BLOCK";

    //######################################################################
    //공통 변수
    //######################################################################

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

    public Texture2D[] _HEADER_TEXTURES;
    public Texture2D[] _ICON_TEXTURES;

    public TextAsset[] _MENU_TEXT_ASSETS;
    public Texture2D[] _COLOR_BTN_TEXTURES;

    public TextAsset[] _SCRIPT_TEXT_ASSETS;
    //######################################################################


    //######################################################################
    //GUI 처리 전역변수
    //######################################################################
    private Rect[] _bg_rects = new Rect[10];
    private Rect[] _button_rects = new Rect[20];
    private GUIStyle[] _text_styles = new GUIStyle[30];
    private GUIStyle[] _btn_styles = new GUIStyle[20];
    private GUIStyle[] _gui_styles = new GUIStyle[20];

    //명령어 컬러 셋을 배열로 생성해 놓는다.
    private Color[] _cmd_color_list = new Color[20];

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

    //######################################################################
    //버튼의 이름
    //######################################################################
    private string _BTN_TITLE_YES = "Yes";
    private string _BTN_TITLE_NO = "No";


    //화면 페이이인 아웃을 처리하기 위한 젼역 객체
    //##########################################
    FadeScreenLibrary _fadeScreenLibrary = new FadeScreenLibrary();
    //##########################################




    //######################################################################
    //CodeEditor 전용 변수
    //######################################################################

    string _selected_coding_type = string.Empty;

    private bool _first_set_flag = false;

    private List<string> _left_cmd_list = new List<string>();
    private List<string> _right_pin_list = new List<string>();

    //NOT USED
    //private List<Code_Item> _code_list = new List<Code_Item>();

    private List<string> _cmd_group_list = new List<string>();
    private Dictionary<string, List<string>> _cmd_group_dic = new Dictionary<string, List<string>>();
    private Dictionary<string, List<string>> _cmd_option_dic = new Dictionary<string, List<string>>();


    //##################################################################
    //아두이노 스케치 문법을 트리 객체로 구현한다.
    //##################################################################
    Code_Item _code_btn_selected_item = null;
    Rect _code_btn_dragging_rect = new Rect(0, 0, 0, 0);
    Code_Item _code_btn_root_item = null;


    //######################################################################
    //GUI Styles
    //######################################################################
    private GUIStyle _gui_box_white_bg_styles;
    private GUIStyle[] _header_btn_styles = new GUIStyle[5];

    //왼쪽 메뉴 아이템들에 대한 컬러 버튼
    private GUIStyle[] _left_btn_color_styles = new GUIStyle[20];
    private GUIStyle _selected_white_bar_style;


    //##################################################################
    //다양한 색상의 배경 텍스처를 프로그램으로 생성해 놓는다.
    //##################################################################
    Texture2D[] _texture_list = new Texture2D[40];

    //##################################################################
    //다양한 색상의 버튼 스타일을 미리 생성해 놓는다.
    //##################################################################
    GUIStyle[] _guistyle_btn_list = new GUIStyle[40];



    //######################################################################
    //Header Region
    //######################################################################
    private Rect _header_region_rects;
    private Rect[] _header_btn_rects = new Rect[5];

    private string _SELECTED_SCENARIO_NO = "";
    private string _SELECTED_SCENARIO_NAME = "";

    private string _FILE_NAME = "";
    private string _STAGE_NAME = "";

    private Rect _header_title_rects;
    private Rect _header_stage_name_rects;


    //######################################################################
    //Left Menu Region
    //######################################################################
    private Rect _left_menu_region_rects;
    private Rect _left_menu_title_rects;

    private Rect _left_menu_scroll_view_rect;
    private Rect _left_menu_scroll_content_rect;

    private Rect _left_icon_rect;
    private Rect _left_btn_rect;

    //Touch
    private Rect _left_menu_scroll_check_rect = new Rect(0, 0, 0, 0);


    //######################################################################
    //Center Code Region
    //######################################################################
    private Rect _center_code_region_rects;
    private Rect _center_code_block_title_rects;
    private Rect _center_code_text_title_rects;

    private Rect _rect_center_code_scroll_view;
    private Rect _rect_center_code_scroll_content;

    //Touch
    private Rect _center_code_scroll_check_rect = new Rect(0, 0, 0, 0);


    //블록 코드 그림을 그리기 위한 영역
    Rect _rect_code_group_box;
    Rect _rect_code_group_circle;
    Rect _dumy_rect_code_box_white_bg;
    Rect _last_code_rect_code_item;
    Rect _dumy_rect_code_item;
    Rect _dumy_rect_select_item;

    //블록 명령어 우측에 삭제 아이콘 표시
    Rect _block_remove_icon_rect;
    Rect _block_selected_icon_rect;


    //######################################################################
    //Right Pin Region
    //######################################################################
    private Rect _right_pin_region_rects;
    private Rect _right_pin_title_rects;

    private Rect _right_pin_scroll_view_rect;
    private Rect _right_pin_scroll_content_rect;

    private Rect _right_icon_rect;
    private Rect _right_btn_rect;

    //Touch
    private Rect _right_pin_scroll_check_rect = new Rect(0, 0, 0, 0);

    //######################################################################
    //Right Property Region
    //######################################################################
    private Rect _right_property_region_rects;
    private Rect _right_property_title_rects;

    private GUIStyle[] _prop_btn_color_styles = new GUIStyle[20];

    GUIStyle _GuiStyle_font_text_field = null;
    GUIStyle _GuiStyle_font_text_field_center = null;

    Rect _Dumy_Rect = new Rect(0, 0, 0, 0);

    //######################################################################
    //Right Button Region
    //######################################################################
    private Rect _right_button_region_rects;
    private Rect _right_button_icon_rects;
    private GUIStyle _execute_btn_styles;


    //##################################################################
    //스크롤 박스 영역의 스크롤러 위치를 저장하는 전역변수이다.
    //##################################################################
    Vector2 _scroll_pos_left_menu = new Vector2(0, 0);
    Vector2 _scroll_pos_center_code = new Vector2(0, 0);
    Vector2 _scroll_pos_right_pin = new Vector2(0, 0);
    Vector2 _scroll_pos_right_property = new Vector2(0, 0);
    Vector2 _scroll_pos_dialog = new Vector2(0, 0);



    //##################################################################
    //이전 메뉴 목록에서 작성된 정보를 저장한다.
    //##################################################################
    string _target_file_name = string.Empty;


    //##################################################################
    //명령어 생성과 관련된 변수를 선언한다.
    //##################################################################
    Dictionary<string, int> _cmd_seq_num_list = new Dictionary<string, int>();
    string _LAST_VARIABLE_NAME = string.Empty;
    string _LAST_OBJECT_NAME = string.Empty;


    //##################################################################
    //드래그 앤 드랍 기능을 구현한다.
    //##################################################################
    bool _block_cmd_dragging_mode = false;
    Rect _dragged_block_rect = new Rect(0, 0, 0, 0);
    Rect _dragged_contains_rect = new Rect(0, 0, 0, 0);
    Rect _dragged_contains_rect_up = new Rect(0, 0, 0, 0);
    Rect _dragged_contains_rect_down = new Rect(0, 0, 0, 0);
    Rect _remove_contains_rect_down = new Rect(0, 0, 0, 0);
    string _dragged_cmd_from = string.Empty;
    string _dragged_cmd_item_type = string.Empty;
    string _dragged_item_name = string.Empty;
    string _dragging_source_id = string.Empty;
    string _dragging_target_id = string.Empty;
    string _drag_taget_up_down = string.Empty;
    Code_Item _dragged_code_item = null;
    GUIStyle _dragged_box_style;

    //삭제 아이콘 클릭
    bool _remove_icon_clicked = false;
    int _remove_clicked_skip_count = 0;
    string _remove_clicked_id = string.Empty;


    //##################################################################
    //속성 창 객체
    //##################################################################
    Rect propNameRect;
    Rect propValueRect;

    Rect propVariableRect;

    Rect propVXNameRect;
    Rect propVXBtn1Rect;
    Rect propVXBtn2Rect;
    Rect propVXValueRect;

    Rect propVXRemoveRect;

    Rect propDumyRect;

    float VXBtn1_Pressed_time = 0;
    float VXBtn2_Pressed_time = 0;
    float VYBtn1_Pressed_time = 0;
    float VYBtn2_Pressed_time = 0;
    float VZBtn1_Pressed_time = 0;
    float VZBtn2_Pressed_time = 0;
    float VABtn1_Pressed_time = 0;
    float VABtn2_Pressed_time = 0;



    //속성
    string propInputText = "";
    string propInputText_X = "";
    string propInputText_Y = "";
    string propInputText_Z = "";
    string propInputText_A = "";

    //속성
    string propInputVariable = "";


    //######################################################################
    // 테스트 편집기 화면
    //######################################################################


    private GUIStyle _GuiStyle_font_text_area_editor = null;

    //현재 사용자가 입력한 코드를 텍스트 모드일 때 텍스트로 저장한다.
    private string _CURRENT_SCRIPT_TEXT = string.Empty;



    //스크립트를 이전 상태로 되돌릴 때 사용한다.
    private List<string> _SCRIPT_HISTORY_LIST = new List<string>();


    //######################################################################



    //######################################################################
    //File Save
    //######################################################################

    private List<string> _sel_script_list = new List<string>();

    private string _USER_ID = string.Empty;
    private string _dialog_msg = string.Empty;

    private GUIStyle _font_text_field_style;

    private Rect _download_input_bg_rect = new Rect(0, 0, 0, 0);
    private Rect _download_input_title_rect = new Rect(0, 0, 0, 0);
    private Rect _download_dialog_title_rect = new Rect(0, 0, 0, 0);

    private Rect _download_email_lable_rect = new Rect(0, 0, 0, 0);
    private Rect _download_email_box_rect = new Rect(0, 0, 0, 0);
    private Rect _download_file_lable_rect = new Rect(0, 0, 0, 0);
    private Rect _download_file_box_rect = new Rect(0, 0, 0, 0);



    void Start()
    {

        #region Start

        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            Screen.fullScreen = false;
        }


        Application.runInBackground = true;


        _screen_width = Screen.width;
        _screen_height = Screen.height;
        _dumy_audio_source = GameObject.Find("sound_dumy");


        _USER_ID = PlayerPrefs.GetString("_USER_ID");


        //저장되어 있는 시나리오 이름을 읽어온다.
        //##########################################################################
        _selected_coding_type = GlobalVariables.SelectedCodingType;



        //왼쪽 메뉴 명령어 리스트를 불러온다.
        LoadMenuScript();


        //오른쪽 아두이노 핀 리스트를 불러온다.
        LoadArduinoPinInfo();


        //전역 객체들을 초기화 한다.
        SetupWorks();
        SetupMenuButtons();
        SetupTextStyle();
        SetupGUIStyle();


        if (_selected_coding_type == "TrafficSignal")
        {
            _SELECTED_SCENARIO_NO = "01";
            _SELECTED_SCENARIO_NAME = "TRAFFIC SIGNAL";
        }
        else if (_selected_coding_type == "ButtonLight")
        {
            _SELECTED_SCENARIO_NO = "02";
            _SELECTED_SCENARIO_NAME = "BUTTON LIGHT";
        }
        else if (_selected_coding_type == "PlaySound")
        {
            _SELECTED_SCENARIO_NO = "03";
            _SELECTED_SCENARIO_NAME = "PLAY SOUND";
        }
        else if (_selected_coding_type == "AutoLight")
        {
            _SELECTED_SCENARIO_NO = "04";
            _SELECTED_SCENARIO_NAME = "AUTO LIGHT";
        }
        else if (_selected_coding_type == "DetectMovingObject")
        {
            _SELECTED_SCENARIO_NO = "05";
            _SELECTED_SCENARIO_NAME = "DETECT MOVING OBJECT";
        }
        else if (_selected_coding_type == "SecurityAlert")
        {
            _SELECTED_SCENARIO_NO = "06";
            _SELECTED_SCENARIO_NAME = "SECURITY ALERT";
        }
        else if (_selected_coding_type == "RearDistanceSensing")
        {
            _SELECTED_SCENARIO_NO = "07";
            _SELECTED_SCENARIO_NAME = "REAR DISTANCE SENSING";
        }
        else if (_selected_coding_type == "SimpleRobotDriving")
        {
            _SELECTED_SCENARIO_NO = "08";
            _SELECTED_SCENARIO_NAME = "SIMPLE ROBOT DRIVING";
        }
        else if (_selected_coding_type == "SCurveDriving")
        {
            _SELECTED_SCENARIO_NO = "09";
            _SELECTED_SCENARIO_NAME = "S CURVE DRIVING";
        }
        else if (_selected_coding_type == "ReactionRobot")
        {
            _SELECTED_SCENARIO_NO = "10";
            _SELECTED_SCENARIO_NAME = "REACTION ROBOT";
        }
        else if (_selected_coding_type == "AvoidObstacle")
        {
            _SELECTED_SCENARIO_NO = "11";
            _SELECTED_SCENARIO_NAME = "AVOID OBSTACLE";
        }
        else if (_selected_coding_type == "AutoParking")
        {
            _SELECTED_SCENARIO_NO = "12";
            _SELECTED_SCENARIO_NAME = "AUTO PARKING";
        }
        else if (_selected_coding_type == "DetectEdge")
        {
            _SELECTED_SCENARIO_NO = "13";
            _SELECTED_SCENARIO_NAME = "DETECT EDGE";
        }
        else if (_selected_coding_type == "LineTracer")
        {
            _SELECTED_SCENARIO_NO = "14";
            _SELECTED_SCENARIO_NAME = "LINE TRACER";
        }
        else if (_selected_coding_type == "MazeExplorer")
        {
            _SELECTED_SCENARIO_NO = "15";
            _SELECTED_SCENARIO_NAME = "MAZE EXPLORER";
        }
        else if (_selected_coding_type == "SecurityAlertVR")
        {
            _SELECTED_SCENARIO_NO = "16";
            _SELECTED_SCENARIO_NAME = "SECURITY ALERT VR";
        }
        else
        {
            _SELECTED_SCENARIO_NO = "00";
            _SELECTED_SCENARIO_NAME = "NEW FILE";
        }


        _STAGE_NAME = _SELECTED_SCENARIO_NO + " " + _SELECTED_SCENARIO_NAME;
        _FILE_NAME = GlobalVariables.CurrentFileName;

        _dialog_name = "CodeEditor";


        //###############################################################
        //비동기 적으로 코드를 불러온다.
        //###############################################################
        StartCoroutine(AsyncStartWorks());
        //###############################################################


        //화면을 어두운 상태에서 점점 밝게 한다.
        //##########################################
        _fadeScreenLibrary.StartFadeIn(false);
        //##########################################

        #endregion Start
    }


    void SetupWorks()
    {
        #region SetupWorks

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



        //Cancel
        _button_rects[2] = new Rect(0, 0, btn_width, btn_height);
        _button_rects[2].x = screen_center_x - btn_width - (10 * _w_ratio);
        _button_rects[2].y = screen_center_y + (50 * _h_ratio);

        //Save
        _button_rects[3] = new Rect(0, 0, btn_width, btn_height);
        _button_rects[3].x = screen_center_x + (10 * _w_ratio);
        _button_rects[3].y = screen_center_y + (50 * _h_ratio);


        //OK
        _button_rects[4] = new Rect(0, 0, btn_width, btn_height);
        _button_rects[4].x = screen_center_x - (btn_width * 0.5f);
        _button_rects[4].y = screen_center_y + (50 * _h_ratio);


        //DeleteButton
        //_button_rects[5] = new Rect(0, 0, _w_ratio, btn_height);
        //_button_rects[5].x = screen_center_x - (btn_width * 0.1f);
        //_button_rects[5].y = screen_center_y + (10 * _h_ratio);



        //각 영역별로 기본 크기를 계산해 놓는다.
        //개별 항목들은 큰 리전 그룹의 값을 이용해서 상대적으로 계산한다.
        //######################################################################
        //기본 영역별 큰 위치 계산 미리 해 놓음
        //######################################################################
        float single_padding = 8 * _s_ratio;
        float double_padding = single_padding * 2;
        float left_region_width = 300 * _w_ratio;
        float right_region_width = 300 * _w_ratio;

        _header_region_rects = new Rect(
            single_padding,
            single_padding,
            (_screen_width - double_padding),
            (100 * _h_ratio - single_padding));

        _left_menu_region_rects = new Rect(
            single_padding,
            _header_region_rects.height,
            (left_region_width - double_padding),
            _screen_height - _header_region_rects.height - single_padding);

        _right_pin_region_rects = new Rect(
            _screen_width - right_region_width + single_padding,
            _left_menu_region_rects.y,
            right_region_width - double_padding,
            (_screen_height * 0.3f) - single_padding);

        _right_button_region_rects = new Rect(
            _right_pin_region_rects.x,
            _screen_height - (80 * _h_ratio),
            _right_pin_region_rects.width,
            80 * _h_ratio - single_padding);


        _right_property_region_rects = new Rect(
            _right_pin_region_rects.x,
            _right_pin_region_rects.y + _right_pin_region_rects.height + single_padding,
            _right_pin_region_rects.width,
            (_screen_height - _header_region_rects.height - _right_pin_region_rects.height - _right_button_region_rects.height - double_padding - single_padding));


        _center_code_region_rects = new Rect(
            _left_menu_region_rects.x + _left_menu_region_rects.width + single_padding,
            _left_menu_region_rects.y,
            _screen_width - _left_menu_region_rects.width - _right_pin_region_rects.width - double_padding - double_padding,
            _left_menu_region_rects.height);


        //######################################################################
        //Header Region
        //######################################################################

        float header_btn_size = 100 * _h_ratio;

        //Home Button        
        _header_btn_rects[0] = new Rect(0, 0, header_btn_size, header_btn_size);

        //Share Button        
        _header_btn_rects[1] = new Rect(_screen_width - _header_btn_rects[0].width, 0, header_btn_size, header_btn_size);

        //Download Button        
        _header_btn_rects[2] = new Rect(_header_btn_rects[1].x - _header_btn_rects[0].width + double_padding, 0, header_btn_size, header_btn_size);

        //Undo Button        
        _header_btn_rects[3] = new Rect(_header_btn_rects[2].x - _header_btn_rects[0].width + double_padding, 0, header_btn_size, header_btn_size);


        _header_title_rects = new Rect(
            _header_btn_rects[0].x + _header_btn_rects[0].width + double_padding,
            0,
            _screen_width,
            header_btn_size * 0.9f);

        _header_stage_name_rects = new Rect(
            _header_btn_rects[3].x - (320 * _w_ratio),
            header_btn_size - (40 * _h_ratio),
            300 * _w_ratio,
            header_btn_size);


        //######################################################################
        //Common Variable
        //######################################################################
        float region_x = 0;
        float region_y = 0;
        float region_width = 0;
        float region_height = 0;


        //######################################################################
        //Left Menu Region
        //######################################################################
        region_x = _left_menu_region_rects.x;
        region_y = _left_menu_region_rects.y;
        region_width = _left_menu_region_rects.width;
        region_height = _left_menu_region_rects.height;

        _left_menu_title_rects = new Rect(
            region_x + double_padding,
            region_y + double_padding,
            region_width - double_padding,
            80 * _h_ratio);


        //Scroll View
        _left_menu_scroll_view_rect = new Rect();
        _left_menu_scroll_view_rect.x = region_x + (10 * _w_ratio);
        _left_menu_scroll_view_rect.y = region_y + (40 * _h_ratio);
        _left_menu_scroll_view_rect.width = region_width - (20 * _w_ratio);
        _left_menu_scroll_view_rect.height = region_height - (50 * _h_ratio);

        _left_menu_scroll_content_rect = new Rect(0, 0, 100, 100);
        //_left_menu_scroll_content_rect.width = region_width - (65 * _w_ratio);
        _left_menu_scroll_content_rect.width = region_width - (55 * _w_ratio);
        _left_menu_scroll_content_rect.height = _left_cmd_list.Count * (50 * _h_ratio) + (60 * _h_ratio);

        //스크롤 기능 구현을 위해 추가
        _left_menu_scroll_check_rect.x = _left_menu_scroll_view_rect.x;
        _left_menu_scroll_check_rect.y = _left_menu_scroll_view_rect.y;
        _left_menu_scroll_check_rect.width = _left_menu_scroll_view_rect.width - (40 * _w_ratio);
        _left_menu_scroll_check_rect.height = _left_menu_scroll_view_rect.height;


        //임시 더미 Rect  객체
        _left_icon_rect = new Rect();
        _left_icon_rect.x = 10 * _w_ratio;
        _left_icon_rect.y = 0;
        _left_icon_rect.width = 25 * _h_ratio;
        _left_icon_rect.height = 25 * _h_ratio;

        _left_btn_rect = new Rect();
        _left_btn_rect.x = 10 * _w_ratio;
        _left_btn_rect.y = 0;
        _left_btn_rect.width = _left_menu_scroll_content_rect.width;
        _left_btn_rect.height = 40 * _h_ratio;

        //######################################################################
        //드래깅 되는 명령어의 크기를 초기화 해준다.
        //######################################################################
        _dragged_block_rect.width = _left_menu_scroll_content_rect.width;
        _dragged_block_rect.height = 40 * _h_ratio;

        //해당 명령어의 마우스 위치 포함 여부 테스트용
        _dragged_contains_rect.width = _left_menu_scroll_content_rect.width;
        _dragged_contains_rect.height = 40 * _h_ratio;

        //위 아래 위치 감지용으로서 20 으로 높이 계산
        _dragged_contains_rect_up.width = _left_menu_scroll_content_rect.width;
        _dragged_contains_rect_up.height = 20 * _h_ratio;

        _dragged_contains_rect_down.width = _left_menu_scroll_content_rect.width;
        _dragged_contains_rect_down.height = 20 * _h_ratio;

        _remove_contains_rect_down.width = 20 * _h_ratio;
        _remove_contains_rect_down.height = 20 * _h_ratio;




        //######################################################################
        //Center Code Region
        //######################################################################
        region_x = _center_code_region_rects.x;
        region_y = _center_code_region_rects.y;
        region_width = _center_code_region_rects.width;
        region_height = _center_code_region_rects.height;

        _center_code_block_title_rects = new Rect(
            region_x + double_padding,
            region_y + double_padding,
            (100 * _w_ratio),
            30 * _h_ratio);


        _center_code_text_title_rects = new Rect(
            region_x + double_padding + _center_code_block_title_rects.width + (30 * _w_ratio),
            region_y + double_padding,
            (100 * _w_ratio),
            30 * _h_ratio);

        //region_width - double_padding

        //Scroll View
        _rect_center_code_scroll_view = new Rect();
        _rect_center_code_scroll_view.x = region_x + (10 * _w_ratio);
        _rect_center_code_scroll_view.y = region_y + (40 * _h_ratio);
        _rect_center_code_scroll_view.width = region_width - (20 * _w_ratio);
        _rect_center_code_scroll_view.height = region_height - (50 * _h_ratio);

        _rect_center_code_scroll_content = new Rect(0, 0, 100, 100);
        _rect_center_code_scroll_content.width = region_width - (65 * _w_ratio);
        _rect_center_code_scroll_content.height = region_height - (50 * _h_ratio);



        //스크롤 기능 구현을 위해 추가
        _center_code_scroll_check_rect.x = _rect_center_code_scroll_view.x;
        _center_code_scroll_check_rect.y = _rect_center_code_scroll_view.y;
        _center_code_scroll_check_rect.width = _rect_center_code_scroll_view.width - (40 * _w_ratio);
        _center_code_scroll_check_rect.height = _rect_center_code_scroll_view.height;


        //마지막 그려진 명령어의 위치를 저장한다.
        //명령어 그릴 때 마지막 위치를 기준으로 그 다음 명령어의 위치를 계산한다.
        _last_code_rect_code_item = new Rect();


        //임시 더비 Rect  객체
        _dumy_rect_code_item = new Rect();
        _dumy_rect_code_item.x = 10 * _w_ratio;
        _dumy_rect_code_item.y = 0;
        _dumy_rect_code_item.width = 320 * _w_ratio;
        _dumy_rect_code_item.height = 40 * _h_ratio;

        _dumy_rect_select_item = new Rect();
        _dumy_rect_select_item.width = 50 * _w_ratio;
        _dumy_rect_select_item.height = 20 * _h_ratio;


        //command box group1
        _rect_code_group_box = new Rect();
        _rect_code_group_box.width = _rect_center_code_scroll_content.width - (30 * _w_ratio);
        _rect_code_group_box.x = 10 * _w_ratio;

        _rect_code_group_circle = new Rect();
        _rect_code_group_circle.width = (30 * _s_ratio);
        _rect_code_group_circle.height = (30 * _s_ratio);
        _rect_code_group_circle.x = 30 * _w_ratio;

        _dumy_rect_code_box_white_bg = new Rect();
        _dumy_rect_code_box_white_bg.width = _rect_code_group_box.width - 10;


        //명령어 그룹의 우측에 삭제 아이콘 표시
        _block_remove_icon_rect = new Rect(0, 0, 12 * _s_ratio, 12 * _s_ratio);
        _block_selected_icon_rect = new Rect(0, 0, 16 * _s_ratio, 16 * _s_ratio);



        //######################################################################
        //Right Pin Region
        //######################################################################
        region_x = _right_pin_region_rects.x;
        region_y = _right_pin_region_rects.y;
        region_width = _right_pin_region_rects.width;
        region_height = _right_pin_region_rects.height;

        _right_pin_title_rects = new Rect(
            region_x + double_padding,
            region_y + double_padding,
            region_width - double_padding,
            80 * _h_ratio);



        //Scroll View
        _right_pin_scroll_view_rect = new Rect();
        _right_pin_scroll_view_rect.x = region_x + (10 * _w_ratio);
        _right_pin_scroll_view_rect.y = region_y + (40 * _h_ratio);
        _right_pin_scroll_view_rect.width = region_width - (20 * _w_ratio);
        _right_pin_scroll_view_rect.height = region_height - (50 * _h_ratio);

        _right_pin_scroll_content_rect = new Rect(0, 0, 100, 100);
        _right_pin_scroll_content_rect.width = region_width - (65 * _w_ratio);
        _right_pin_scroll_content_rect.height = _right_pin_list.Count * (50 * _h_ratio) + (60 * _h_ratio);

        //스크롤 기능 구현을 위해 추가
        _right_pin_scroll_check_rect.x = _right_pin_scroll_view_rect.x;
        _right_pin_scroll_check_rect.y = _right_pin_scroll_view_rect.y;
        _right_pin_scroll_check_rect.width = _right_pin_scroll_view_rect.width - (40 * _w_ratio);
        _right_pin_scroll_check_rect.height = _right_pin_scroll_view_rect.height;


        //임시 더미 Rect  객체
        _right_icon_rect = new Rect();
        //_right_icon_rect.x = 10 * _w_ratio;
        _right_icon_rect.x = 0;
        _right_icon_rect.y = 0;
        _right_icon_rect.width = 25 * _h_ratio;
        _right_icon_rect.height = 25 * _h_ratio;

        _right_btn_rect = new Rect();
        _right_btn_rect.x = 10 * _w_ratio;
        _right_btn_rect.y = 0;
        _right_btn_rect.width = _right_pin_scroll_content_rect.width - (30 * _w_ratio);
        _right_btn_rect.height = 40 * _h_ratio;


        //######################################################################
        //Right Property Region
        //######################################################################
        region_x = _right_property_region_rects.x;
        region_y = _right_property_region_rects.y;
        region_width = _right_property_region_rects.width;
        region_height = _right_property_region_rects.height;

        _right_property_title_rects = new Rect(
            region_x + double_padding,
            region_y + double_padding,
            region_width - double_padding,
            80 * _h_ratio);



        //속성 값
        propNameRect = new Rect(_right_property_region_rects.x + 20 * _w_ratio, _right_property_region_rects.y + 40 * _h_ratio, 230 * _w_ratio, 40 * _h_ratio);
        propValueRect = new Rect(_right_property_region_rects.x + 20 * _w_ratio, _right_property_region_rects.y + 85 * _h_ratio, 180 * _w_ratio, 40 * _h_ratio);
        propVariableRect = new Rect(_right_property_region_rects.x + 20 * _w_ratio, _right_property_region_rects.y + 120 * _h_ratio, 230 * _w_ratio, 40 * _h_ratio);


        //10씩 왼쪽으로 이동
        propVXNameRect = new Rect(_right_property_region_rects.x + 10 * _w_ratio, _right_property_region_rects.y + 85 * _h_ratio, 40 * _w_ratio, 40 * _h_ratio);
        propVXBtn1Rect = new Rect(_right_property_region_rects.x + 60 * _w_ratio, _right_property_region_rects.y + 85 * _h_ratio, 40 * _w_ratio, 40 * _h_ratio);
        propVXValueRect = new Rect(_right_property_region_rects.x + 110 * _w_ratio, _right_property_region_rects.y + 85 * _h_ratio, 60 * _w_ratio, 40 * _h_ratio);
        propVXBtn2Rect = new Rect(_right_property_region_rects.x + 180 * _w_ratio, _right_property_region_rects.y + 85 * _h_ratio, 40 * _w_ratio, 40 * _h_ratio);


        propVXRemoveRect = new Rect(_right_property_region_rects.x + 230 * _w_ratio, _right_property_region_rects.y + 85 * _h_ratio, 40 * _w_ratio, 40 * _h_ratio);
        propDumyRect = new Rect(0, 0, 2, 2);



        //######################################################################
        //Right Button Region
        //######################################################################
        region_x = _right_button_region_rects.x;
        region_y = _right_button_region_rects.y;
        region_width = _right_button_region_rects.width;
        region_height = _right_button_region_rects.height;

        _right_button_icon_rects = new Rect(
            region_x + 53 * _w_ratio,
            region_y + 24 * _h_ratio,
            25 * _w_ratio,
            25 * _h_ratio);




        //#################################################################################
        //File Save
        //#################################################################################

        _download_input_bg_rect = new Rect(Screen.width * 0.15f, half_height * 0.25f, Screen.width * 0.7f, half_height * 1.2f);


        region_x = _download_input_bg_rect.x;
        region_y = _download_input_bg_rect.y;
        region_width = _download_input_bg_rect.width;
        region_height = _download_input_bg_rect.height;


        //File Save Title
        _download_input_title_rect.width = region_width;
        _download_input_title_rect.height = 50 * _h_ratio;
        _download_input_title_rect.x = region_x;
        _download_input_title_rect.y = region_y + 30 * _h_ratio;


        //Dialog Title
        _download_dialog_title_rect.width = region_width;
        _download_dialog_title_rect.height = 50 * _h_ratio;
        _download_dialog_title_rect.x = region_x;
        _download_dialog_title_rect.y = region_y + 80 * _h_ratio;




        //Email Label
        _download_email_lable_rect.width = region_width * 0.8f;
        _download_email_lable_rect.height = 50 * _h_ratio;
        _download_email_lable_rect.x = region_x + (region_width * 0.1f);
        _download_email_lable_rect.y = region_y + 80 * _h_ratio;


        //Email Box
        _download_email_box_rect.width = region_width * 0.8f;
        _download_email_box_rect.height = 50 * _h_ratio;
        _download_email_box_rect.x = region_x + (region_width * 0.1f);
        _download_email_box_rect.y = region_y + (120 * _h_ratio);


        //File Name Label
        _download_file_lable_rect.width = region_width * 0.8f;
        _download_file_lable_rect.height = 50 * _h_ratio;
        _download_file_lable_rect.x = region_x + (region_width * 0.1f);
        _download_file_lable_rect.y = region_y + 180 * _h_ratio;


        //File Name Input Box
        _download_file_box_rect.width = region_width * 0.8f;
        _download_file_box_rect.height = 50 * _h_ratio;
        _download_file_box_rect.x = region_x + (region_width * 0.1f);
        _download_file_box_rect.y = region_y + (220 * _h_ratio);


        #endregion SetupWorks
    }


    void SetupMenuButtons()
    {

        #region SetupMenuButtons

        //투명 버튼
        _btn_styles[0] = new GUIStyle();


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


        //저장 다이얼로그 창
        //회색 No 버튼 스타일
        _btn_styles[5] = new GUIStyle();
        _btn_styles[5].normal.background = _BTN_TEXTURES[0];
        _btn_styles[5].hover.background = _BTN_TEXTURES[0];
        _btn_styles[5].active.background = _BTN_TEXTURES[1];
        _btn_styles[5].alignment = TextAnchor.MiddleCenter;
        _btn_styles[5].font = _FONTS[0];
        _btn_styles[5].fontStyle = FontStyle.Bold;
        _btn_styles[5].normal.textColor = new Color(1, 1, 1);
        _btn_styles[5].fontSize = (int)(26 * _w_ratio);


        //오렌지색 Yes 버튼 스타일
        _btn_styles[6] = new GUIStyle();
        _btn_styles[6].normal.background = _BTN_TEXTURES[2];
        _btn_styles[6].hover.background = _BTN_TEXTURES[2];
        _btn_styles[6].active.background = _BTN_TEXTURES[3];
        _btn_styles[6].alignment = TextAnchor.MiddleCenter;
        _btn_styles[6].font = _FONTS[0];
        _btn_styles[6].fontStyle = FontStyle.Bold;
        _btn_styles[6].normal.textColor = new Color(1, 1, 1);
        _btn_styles[6].fontSize = (int)(26 * _w_ratio);

        _btn_styles[7] = new GUIStyle();
        _btn_styles[7].normal.background = _ICON_TEXTURES[0];
        _btn_styles[7].hover.background = _ICON_TEXTURES[0];
        _btn_styles[7].active.background = _ICON_TEXTURES[16];



        #endregion SetupMenuButtons
    }



    void SetupTextStyle()
    {
        #region SetupTextStyle

        //Dialog Message Text
        _text_styles[0] = new GUIStyle();
        _text_styles[0].font = _FONTS[0];
        _text_styles[0].fontStyle = FontStyle.Bold;
        _text_styles[0].normal.textColor = Color.black;
        _text_styles[0].fontSize = (int)(38 * _s_ratio);
        _text_styles[0].alignment = TextAnchor.UpperCenter;
        _text_styles[0].contentOffset = new Vector2(0, 0);

        //Header Scenario Title
        _text_styles[1] = new GUIStyle();
        _text_styles[1].font = _FONTS[0];
        _text_styles[1].fontStyle = FontStyle.Bold;
        _text_styles[1].normal.textColor = V2.Utils.ToFloatColor(122, 150, 198);
        _text_styles[1].fontSize = (int)(36 * _s_ratio);
        _text_styles[1].alignment = TextAnchor.MiddleLeft;
        _text_styles[1].contentOffset = new Vector2(0, 0);

        //Header Stage Name
        _text_styles[2] = new GUIStyle();
        _text_styles[2].font = _FONTS[0];
        _text_styles[2].fontStyle = FontStyle.Bold;
        _text_styles[2].normal.textColor = V2.Utils.ToFloatColor(122, 150, 198);
        _text_styles[2].fontSize = (int)(18 * _s_ratio);
        //_text_styles[2].alignment = TextAnchor.MiddleLeft;
        _text_styles[2].alignment = TextAnchor.UpperRight;
        _text_styles[2].contentOffset = new Vector2(0, 0);


        //Group Title
        _text_styles[3] = new GUIStyle();
        _text_styles[3].font = _FONTS[0];
        _text_styles[3].fontStyle = FontStyle.Bold;
        _text_styles[3].normal.textColor = V2.Utils.ToFloatColor(110, 110, 110);
        _text_styles[3].fontSize = (int)(18 * _s_ratio);
        _text_styles[3].alignment = TextAnchor.UpperLeft;
        _text_styles[3].contentOffset = new Vector2(0, 0);


        //Left Menu Group
        _text_styles[4] = new GUIStyle();
        _text_styles[4].font = _FONTS[0];
        _text_styles[4].fontStyle = FontStyle.Bold;
        _text_styles[4].normal.textColor = V2.Utils.ToFloatColor(50, 50, 50);
        _text_styles[4].fontSize = (int)(22 * _s_ratio);
        _text_styles[4].alignment = TextAnchor.MiddleLeft;
        _text_styles[4].contentOffset = new Vector2(0, 0);


        //Right Pin Item Name
        _text_styles[5] = new GUIStyle();
        _text_styles[5].font = _FONTS[0];
        _text_styles[5].fontStyle = FontStyle.Bold;
        _text_styles[5].normal.background = V2.Utils.MakeTex(4, 4, Color.white, false);
        _text_styles[5].hover.background = V2.Utils.MakeTex(4, 4, Color.white, false);
        _text_styles[5].active.background = V2.Utils.MakeTex(4, 4, Color.white, false);
        _text_styles[5].normal.textColor = V2.Utils.ToFloatColor(50, 50, 50);
        _text_styles[5].hover.textColor = V2.Utils.ToFloatColor(100, 100, 100);
        _text_styles[5].active.textColor = V2.Utils.ToFloatColor(100, 50, 0);
        _text_styles[5].fontSize = (int)(18 * _s_ratio);
        _text_styles[5].alignment = TextAnchor.MiddleLeft;
        _text_styles[5].contentOffset = new Vector2(0, 0);



        //오른쪽 속성 이름
        _text_styles[6] = new GUIStyle();
        _text_styles[6].font = _FONTS[0];
        _text_styles[6].fontStyle = FontStyle.Bold;
        _text_styles[6].normal.textColor = new Color(0.2f, 0.2f, 0.2f);
        _text_styles[6].fontSize = (int)(22 * _s_ratio);
        _text_styles[6].alignment = TextAnchor.MiddleLeft;
        _text_styles[6].contentOffset = new Vector2(0, 0);


        //속성에서 벡터3 항목 이름
        _text_styles[7] = new GUIStyle();
        _text_styles[7].font = _FONTS[0];
        _text_styles[7].fontSize = (int)(18 * _s_ratio);
        //_text_styles[7].normal.textColor = new Color(1, 1, 1);
        _text_styles[7].normal.textColor = new Color(0.2f, 0.2f, 0.2f);
        _text_styles[7].alignment = TextAnchor.MiddleLeft;


        //속성의 제목 이름
        //2017.12.10
        _text_styles[8] = new GUIStyle();
        _text_styles[8].font = _FONTS[0];
        _text_styles[8].fontSize = (int)(22 * _s_ratio);
        _text_styles[8].normal.textColor = new Color(0.9f, 0.9f, 0.9f);
        _text_styles[8].alignment = TextAnchor.LowerLeft;



        //블록 텍스트 모드 활성화 상태 버튼 
        _text_styles[9] = new GUIStyle();
        _text_styles[9].font = _FONTS[0];
        _text_styles[9].fontStyle = FontStyle.Bold;
        _text_styles[9].normal.textColor = V2.Utils.ToFloatColor(110, 110, 110);
        _text_styles[9].fontSize = (int)(18 * _s_ratio);
        _text_styles[9].alignment = TextAnchor.UpperCenter;
        _text_styles[9].contentOffset = new Vector2(0, 0);

        //블록 텍스트 모드 활성화 상태 버튼 
        _text_styles[20] = new GUIStyle();
        _text_styles[20].font = _FONTS[0];
        _text_styles[20].fontStyle = FontStyle.Bold;
        _text_styles[20].normal.background = V2.Utils.MakeTex(4, 4, Color.white, false);
        _text_styles[20].hover.background = V2.Utils.MakeTex(4, 4, Color.white, false);
        _text_styles[20].active.background = V2.Utils.MakeTex(4, 4, Color.white, false);
        _text_styles[20].normal.textColor = V2.Utils.ToFloatColor(110, 110, 110);
        _text_styles[20].hover.textColor = V2.Utils.ToFloatColor(255, 128, 0);
        _text_styles[20].active.textColor = V2.Utils.ToFloatColor(100, 50, 0);
        _text_styles[20].fontSize = (int)(18 * _s_ratio);
        _text_styles[20].alignment = TextAnchor.UpperCenter;
        _text_styles[20].contentOffset = new Vector2(0, 0);


        //블록 텍스트 모드 비활성화 상태 버튼 
        _text_styles[21] = new GUIStyle();
        _text_styles[21].font = _FONTS[0];
        _text_styles[21].fontStyle = FontStyle.Bold;
        _text_styles[21].normal.background = V2.Utils.MakeTex(4, 4, Color.white, false);
        _text_styles[21].hover.background = V2.Utils.MakeTex(4, 4, Color.white, false);
        _text_styles[21].active.background = V2.Utils.MakeTex(4, 4, Color.white, false);
        _text_styles[21].normal.textColor = V2.Utils.ToFloatColor(190, 190, 190);
        _text_styles[21].hover.textColor = V2.Utils.ToFloatColor(255, 128, 0);
        _text_styles[21].active.textColor = V2.Utils.ToFloatColor(100, 50, 0);
        _text_styles[21].fontSize = (int)(18 * _s_ratio);
        _text_styles[21].alignment = TextAnchor.UpperCenter;
        _text_styles[21].contentOffset = new Vector2(0, 0);


        //Download Title
        _text_styles[22] = new GUIStyle();
        _text_styles[22].font = _FONTS[0];
        _text_styles[22].fontStyle = FontStyle.Bold;
        _text_styles[22].normal.textColor = Color.black;
        _text_styles[22].fontSize = (int)(38 * _s_ratio);
        _text_styles[22].alignment = TextAnchor.UpperCenter;
        _text_styles[22].contentOffset = new Vector2(0, 0);


        //Download Label
        _text_styles[23] = new GUIStyle();
        _text_styles[23].font = _FONTS[0];
        _text_styles[23].fontStyle = FontStyle.Bold;
        _text_styles[23].normal.textColor = V2.Utils.ToFloatColor(160, 160, 160);
        _text_styles[23].fontSize = (int)(20 * _s_ratio);
        _text_styles[23].alignment = TextAnchor.MiddleLeft;
        _text_styles[23].contentOffset = new Vector2(0, 0);





        //명령어 색상 목록 
        _cmd_color_list[0] = V2.Utils.ToFloatColor(255, 203, 43);
        _cmd_color_list[1] = V2.Utils.ToFloatColor(255, 137, 54);
        _cmd_color_list[2] = V2.Utils.ToFloatColor(255, 97, 151);
        _cmd_color_list[3] = V2.Utils.ToFloatColor(62, 189, 198);
        _cmd_color_list[4] = V2.Utils.ToFloatColor(72, 99, 146);
        _cmd_color_list[5] = V2.Utils.ToFloatColor(14, 190, 18);


        //오른쪽 속성 이름
        for (int i = 0; i < 6; i++)
        {
            _text_styles[i + 10] = new GUIStyle();
            _text_styles[i + 10].font = _FONTS[0];
            _text_styles[i + 10].fontStyle = FontStyle.Bold;
            _text_styles[i + 10].normal.textColor = _cmd_color_list[i];
            _text_styles[i + 10].fontSize = (int)(22 * _s_ratio);
            _text_styles[i + 10].alignment = TextAnchor.MiddleLeft;
            _text_styles[i + 10].contentOffset = new Vector2(0, 0);
        }



        #endregion SetupTextStyle
    }



    void SetupGUIStyle()
    {
        #region SetupGUIStyle

        _gui_box_white_bg_styles = new GUIStyle();
        _gui_box_white_bg_styles.border = new RectOffset(50, 50, 50, 50);
        _gui_box_white_bg_styles.normal.background = _TEXTURES[3];

        //Header Home Button
        _header_btn_styles[0] = new GUIStyle();
        _header_btn_styles[0].normal.background = _HEADER_TEXTURES[2];
        _header_btn_styles[0].hover.background = _HEADER_TEXTURES[2];
        _header_btn_styles[0].active.background = _HEADER_TEXTURES[3];

        //Header Share Button
        _header_btn_styles[1] = new GUIStyle();
        _header_btn_styles[1].normal.background = _HEADER_TEXTURES[4];
        _header_btn_styles[1].hover.background = _HEADER_TEXTURES[4];
        _header_btn_styles[1].active.background = _HEADER_TEXTURES[5];

        //Header Download Button
        _header_btn_styles[2] = new GUIStyle();
        _header_btn_styles[2].normal.background = _HEADER_TEXTURES[0];
        _header_btn_styles[2].hover.background = _HEADER_TEXTURES[0];
        _header_btn_styles[2].active.background = _HEADER_TEXTURES[1];

        //Header Undo Button
        _header_btn_styles[3] = new GUIStyle();
        _header_btn_styles[3].normal.background = _HEADER_TEXTURES[6];
        _header_btn_styles[3].hover.background = _HEADER_TEXTURES[6];
        _header_btn_styles[3].active.background = _HEADER_TEXTURES[7];

        //Right Button - Execute Button
        _execute_btn_styles = new GUIStyle();
        _execute_btn_styles.normal.background = _BTN_TEXTURES[4];
        _execute_btn_styles.hover.background = _BTN_TEXTURES[4];
        _execute_btn_styles.active.background = _BTN_TEXTURES[5];
        _execute_btn_styles.font = _FONTS[0];
        _execute_btn_styles.fontStyle = FontStyle.Bold;
        _execute_btn_styles.normal.textColor = V2.Utils.ToFloatColor(255, 255, 255);
        _execute_btn_styles.fontSize = (int)(32 * _s_ratio);
        _execute_btn_styles.alignment = TextAnchor.MiddleCenter;
        _execute_btn_styles.contentOffset = new Vector2(10 * _w_ratio, 0);



        for (int i = 0; i < 7; i++)
        {
            _left_btn_color_styles[i] = new GUIStyle();
            _left_btn_color_styles[i].normal.background = _COLOR_BTN_TEXTURES[i];
            _left_btn_color_styles[i].hover.background = _COLOR_BTN_TEXTURES[i];
            _left_btn_color_styles[i].active.background = _COLOR_BTN_TEXTURES[i];
            _left_btn_color_styles[i].font = _FONTS[0];
            _left_btn_color_styles[i].fontStyle = FontStyle.Bold;
            _left_btn_color_styles[i].normal.textColor = V2.Utils.ToFloatColor(255, 255, 255);
            _left_btn_color_styles[i].hover.textColor = V2.Utils.ToFloatColor(0, 0, 0);
            _left_btn_color_styles[i].active.textColor = V2.Utils.ToFloatColor(200, 200, 200);
            _left_btn_color_styles[i].fontSize = (int)(18 * _s_ratio);
            _left_btn_color_styles[i].alignment = TextAnchor.MiddleLeft;
            _left_btn_color_styles[i].contentOffset = new Vector2(20 * _w_ratio, 0);


            //속성창에서 값 감소 및 증가 버튼시 사용
            _prop_btn_color_styles[i] = new GUIStyle();
            _prop_btn_color_styles[i].normal.background = _COLOR_BTN_TEXTURES[i];
            _prop_btn_color_styles[i].hover.background = _COLOR_BTN_TEXTURES[i];
            _prop_btn_color_styles[i].active.background = _COLOR_BTN_TEXTURES[i];
            _prop_btn_color_styles[i].font = _FONTS[0];
            _prop_btn_color_styles[i].fontStyle = FontStyle.Bold;
            _prop_btn_color_styles[i].normal.textColor = V2.Utils.ToFloatColor(255, 255, 255);
            _prop_btn_color_styles[i].hover.textColor = V2.Utils.ToFloatColor(0, 0, 0);
            _prop_btn_color_styles[i].active.textColor = V2.Utils.ToFloatColor(200, 200, 200);
            _prop_btn_color_styles[i].fontSize = (int)(22 * _s_ratio);
            _prop_btn_color_styles[i].alignment = TextAnchor.MiddleCenter;
        }


        //드래깅 명령어 박스
        _dragged_box_style = new GUIStyle();
        _dragged_box_style.normal.background = _COLOR_BTN_TEXTURES[12];
        _dragged_box_style.hover.background = _COLOR_BTN_TEXTURES[12];
        _dragged_box_style.active.background = _COLOR_BTN_TEXTURES[12];
        _dragged_box_style.font = _FONTS[0];
        _dragged_box_style.fontStyle = FontStyle.Bold;
        _dragged_box_style.normal.textColor = V2.Utils.ToFloatColor(255, 255, 255);
        _dragged_box_style.fontSize = (int)(18 * _s_ratio);
        _dragged_box_style.alignment = TextAnchor.MiddleLeft;
        _dragged_box_style.contentOffset = new Vector2(20 * _w_ratio, 0);



        //드래깅 타겟 영역 표시 - 반투명 검정색 바
        _selected_white_bar_style = new GUIStyle();
        _selected_white_bar_style.normal.background = _COLOR_BTN_TEXTURES[13];
        _selected_white_bar_style.border = new RectOffset(5, 5, 0, 0);



        //##################################################################
        //다양한 색상의 배경 텍스처를 프로그램으로 생성해 놓는다.
        //##################################################################

        _texture_list[0] = V2.Utils.MakeTex(10, 10, new Color(1, 1, 1, 0.5f), false);
        _texture_list[1] = V2.Utils.MakeTex(10, 10, new Color(0.9f, 0.9f, 0.9f, 0.5f), false);

        _texture_list[2] = V2.Utils.MakeTex(10, 10, new Color(0.3f, 0.3f, 0.3f, 0.5f), false);
        _texture_list[3] = V2.Utils.MakeTex(10, 10, new Color(0.2f, 0.2f, 0.2f, 0.5f), false);

        _texture_list[4] = V2.Utils.MakeTex(10, 10, new Color(0.5f, 0.5f, 0.5f, 0.5f), false);
        _texture_list[5] = V2.Utils.MakeTex(10, 10, new Color(0.4f, 0.4f, 0.4f, 0.5f), false);

        _texture_list[6] = V2.Utils.MakeTex(10, 10, new Color(0.7f, 0.7f, 0.7f, 0.5f), false);
        _texture_list[7] = V2.Utils.MakeTex(10, 10, new Color(0.6f, 0.6f, 0.6f, 0.5f), false);

        _texture_list[10] = V2.Utils.MakeTex(10, 10, V2.Utils.ToFloatColor(70, 157, 140), false);
        _texture_list[11] = V2.Utils.MakeTex(10, 10, V2.Utils.ToFloatColor(50, 137, 120), false);

        _texture_list[12] = V2.Utils.MakeTex(10, 10, V2.Utils.ToFloatColor(236, 112, 99), false);
        _texture_list[13] = V2.Utils.MakeTex(10, 10, V2.Utils.ToFloatColor(216, 92, 79), false);

        _texture_list[14] = V2.Utils.MakeTex(10, 10, V2.Utils.ToFloatColor(243, 156, 18), false);
        _texture_list[15] = V2.Utils.MakeTex(10, 10, V2.Utils.ToFloatColor(223, 136, 0), false);

        _texture_list[16] = V2.Utils.MakeTex(10, 10, V2.Utils.ToFloatColor(65, 91, 118), false);
        _texture_list[17] = V2.Utils.MakeTex(10, 10, V2.Utils.ToFloatColor(45, 71, 98), false);

        _texture_list[18] = V2.Utils.MakeTex(10, 10, V2.Utils.ToFloatColor(52, 152, 219), false);
        _texture_list[19] = V2.Utils.MakeTex(10, 10, V2.Utils.ToFloatColor(32, 132, 199), false);

        //밝은 초록 계열
        _texture_list[20] = V2.Utils.MakeTex(10, 10, V2.Utils.ToFloatColor(46, 204, 113), false);
        _texture_list[21] = V2.Utils.MakeTex(10, 10, V2.Utils.ToFloatColor(26, 194, 93), false);

        //완전 흰색
        _texture_list[22] = V2.Utils.MakeTex(10, 10, new Color(1, 1, 1), false);
        _texture_list[23] = V2.Utils.MakeTex(10, 10, new Color(0.9f, 0.9f, 0.9f), false);





        //##################################################################
        //코드 블록에서 사용되는 다양한 색상의 버튼 스타일을 미리 생성해 놓는다.
        //##################################################################

        if (_guistyle_btn_list[0] == null)
        {
            //초록색 계열
            _guistyle_btn_list[0] = new GUIStyle();
            _guistyle_btn_list[0].fontSize = (int)(18 * _s_ratio);
            _guistyle_btn_list[0].normal.textColor = new Color(1, 1, 1);
            _guistyle_btn_list[0].normal.background = _texture_list[10];
            _guistyle_btn_list[0].active.background = _texture_list[11];
            _guistyle_btn_list[0].alignment = TextAnchor.MiddleLeft;
            _guistyle_btn_list[0].padding.left = (int)(10 * _w_ratio);
        }

        if (_guistyle_btn_list[1] == null)
        {
            //빨강 주홍색 계열
            _guistyle_btn_list[1] = new GUIStyle();
            _guistyle_btn_list[1].fontSize = (int)(18 * _s_ratio);
            _guistyle_btn_list[1].normal.textColor = new Color(1, 1, 1);
            _guistyle_btn_list[1].normal.background = _texture_list[12];
            _guistyle_btn_list[1].active.background = _texture_list[13];
            _guistyle_btn_list[1].alignment = TextAnchor.MiddleLeft;
            _guistyle_btn_list[1].padding.left = (int)(10 * _w_ratio);
        }

        if (_guistyle_btn_list[2] == null)
        {
            //오렌지색 계열
            _guistyle_btn_list[2] = new GUIStyle();
            _guistyle_btn_list[2].fontSize = (int)(18 * _s_ratio);
            _guistyle_btn_list[2].normal.textColor = new Color(1, 1, 1);
            _guistyle_btn_list[2].normal.background = _texture_list[14];
            _guistyle_btn_list[2].active.background = _texture_list[15];
            _guistyle_btn_list[2].alignment = TextAnchor.MiddleLeft;
            _guistyle_btn_list[2].padding.left = (int)(10 * _w_ratio);
        }

        if (_guistyle_btn_list[3] == null)
        {
            //쥐회색 계열
            _guistyle_btn_list[3] = new GUIStyle();
            _guistyle_btn_list[3].fontSize = (int)(18 * _s_ratio);
            _guistyle_btn_list[3].normal.textColor = new Color(1, 1, 1);
            _guistyle_btn_list[3].normal.background = _texture_list[16];
            _guistyle_btn_list[3].active.background = _texture_list[17];
            _guistyle_btn_list[3].alignment = TextAnchor.MiddleLeft;
            _guistyle_btn_list[3].padding.left = (int)(10 * _w_ratio);
        }

        if (_guistyle_btn_list[4] == null)
        {
            //하늘색 계열
            _guistyle_btn_list[4] = new GUIStyle();
            _guistyle_btn_list[4].fontSize = (int)(18 * _s_ratio);
            _guistyle_btn_list[4].normal.textColor = new Color(1, 1, 1);
            _guistyle_btn_list[4].normal.background = _texture_list[18];
            _guistyle_btn_list[4].active.background = _texture_list[19];
            _guistyle_btn_list[4].alignment = TextAnchor.MiddleLeft;
            _guistyle_btn_list[4].padding.left = (int)(10 * _w_ratio);
        }

        if (_guistyle_btn_list[5] == null)
        {
            //짙은 회색 계열
            _guistyle_btn_list[5] = new GUIStyle();
            _guistyle_btn_list[5].fontSize = (int)(18 * _s_ratio);
            _guistyle_btn_list[5].normal.textColor = new Color(1, 1, 1);
            _guistyle_btn_list[5].normal.background = _texture_list[4];    //0.5, 0.5, 0.5
            _guistyle_btn_list[5].active.background = _texture_list[5];
            _guistyle_btn_list[5].alignment = TextAnchor.MiddleLeft;
            _guistyle_btn_list[5].padding.left = (int)(10 * _w_ratio);
        }

        if (_guistyle_btn_list[6] == null)
        {
            //투명 버튼
            _guistyle_btn_list[6] = new GUIStyle();
        }

        if (_guistyle_btn_list[7] == null)
        {
            //밝은 초록 계열
            _guistyle_btn_list[7] = new GUIStyle();
            _guistyle_btn_list[7].fontSize = (int)(18 * _s_ratio);
            _guistyle_btn_list[7].normal.textColor = new Color(1, 1, 1);
            _guistyle_btn_list[7].normal.background = _texture_list[20];
            _guistyle_btn_list[7].active.background = _texture_list[21];
            _guistyle_btn_list[7].alignment = TextAnchor.MiddleLeft;
            _guistyle_btn_list[7].padding.left = (int)(10 * _w_ratio);
        }



        #endregion SetupGUIStyle
    }



    //###############################################################
    //비동기 적으로 시작되어야 하는 기능들을 호출한다
    //###############################################################
    private IEnumerator AsyncStartWorks()
    {

        //#####################################################
        //스크립트 코드를 불러온다.
        //#####################################################
        LoadCodeScript();

        //종료한다.
        yield break;
    }




    #region LoadCodeScript

    //###############################################################
    //스크립트 코드를 불러온다.
    //###############################################################
    private void LoadCodeScript()
    {

        string script_text = string.Empty;


        string saved_target_file_dir = Application.persistentDataPath + "/" + GlobalVariables.SelectedCodingType;

        if (!Directory.Exists(saved_target_file_dir))
        {
            Directory.CreateDirectory(saved_target_file_dir);
        }


        string saved_target_file_name = Application.persistentDataPath + "/" + GlobalVariables.SelectedCodingType + "/" + GlobalVariables.CurrentFileName + ".txt";

        if (File.Exists(saved_target_file_name))
        {
            script_text = File.ReadAllText(saved_target_file_name);
        }

        string[] lines = script_text.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        LoadCodeScript_Internal(lines);


        //Text 스크립트도 저장해 놓는다.
        _CURRENT_SCRIPT_TEXT = ExportScriptAsText();
    }



    private void LoadCodeScript_Internal(string[] all_code_lines)
    {
        //트리 명령어 초기화
        _code_btn_root_item = null;
        _code_btn_selected_item = null;


        if (all_code_lines != null && all_code_lines.Length > 0)
        {
            #region if

            Code_Item cur_code_item = null;


            for (int i = 0; i < all_code_lines.Length; i++)
            {
                #region for

                string single_line = all_code_lines[i].Trim();


                if (string.IsNullOrEmpty(single_line))
                    continue;

                if (single_line.StartsWith("//"))
                    continue;

                int indentation_level = 0;
                while (single_line.StartsWith("\t") && single_line.Length >= 2)
                {
                    single_line = single_line.Substring(1);
                    indentation_level = indentation_level + 1;
                }


                string cmd_type = "EXPR";
                string cmd_name = "Expression";
                string cmd_value = single_line;
                string cmd_variable = "";


                if (single_line.StartsWith("if(") || single_line.StartsWith("if "))
                {
                    cmd_type = "LOGIC";
                    cmd_name = "if";
                    cmd_value = SPL.Common.ParsingHelper.GetConditionExpression(single_line);
                }
                else if (single_line.StartsWith("else if(") || single_line.StartsWith("else if "))
                {
                    cmd_type = "LOGIC";
                    cmd_name = "else if";
                    cmd_value = SPL.Common.ParsingHelper.GetConditionExpression(single_line);
                }
                else if (single_line.StartsWith("else"))
                {
                    cmd_type = "LOGIC";
                    cmd_name = "else";
                    cmd_value = SPL.Common.ParsingHelper.GetConditionExpression(single_line);
                }
                else if (single_line.StartsWith("for(") || single_line.StartsWith("for "))
                {
                    cmd_type = "LOGIC";
                    cmd_name = "for";
                    cmd_value = SPL.Common.ParsingHelper.GetConditionExpression(single_line);
                }
                else if (single_line.StartsWith("while(") || single_line.StartsWith("while "))
                {
                    cmd_type = "LOGIC";
                    cmd_name = "while";
                    cmd_value = SPL.Common.ParsingHelper.GetConditionExpression(single_line);
                }
                else
                {

                    #region Arduino
                    if (single_line.StartsWith("delay(") || single_line.StartsWith("delay ") || single_line.StartsWith("Delay(") || single_line.StartsWith("Delay "))
                    {
                        cmd_type = "CMD";
                        cmd_name = "delay";
                        cmd_value = SPL.Common.ParsingHelper.GetConditionExpression(single_line);
                    }
                    else if (single_line.StartsWith("Serial.print(") || single_line.StartsWith("Serial.print ") || single_line.StartsWith("Print(") || single_line.StartsWith("Print "))
                    {
                        cmd_type = "EXPR";
                        cmd_name = "Serial.print";
                        cmd_value = SPL.Common.ParsingHelper.GetConditionExpression(single_line);
                    }
                    else if (single_line.StartsWith("Serial.println(") || single_line.StartsWith("Serial.println ") || single_line.StartsWith("PrintLine(") || single_line.StartsWith("PrintLine "))
                    {
                        cmd_type = "EXPR";
                        cmd_name = "Serial.println";
                        cmd_value = SPL.Common.ParsingHelper.GetConditionExpression(single_line);
                    }
                    else if (single_line.StartsWith("void loop(") || single_line.StartsWith("void loop "))
                    {
                        cmd_type = "LOGIC";
                        cmd_name = "loop function";
                    }
                    else if (single_line.StartsWith("void setup(") || single_line.StartsWith("void setup "))
                    {
                        cmd_type = "LOGIC";
                        cmd_name = "setup function";
                    }
                    else if (single_line.StartsWith("void") && single_line.Contains("(") && single_line.Contains(")") && !single_line.Contains("="))
                    {
                        //2016.10.12
                        cmd_type = "LOGIC";
                        cmd_name = "function";
                        cmd_value = single_line;
                    }

                    else if (
                        (single_line.StartsWith("int") ||
                        single_line.StartsWith("float") ||
                        single_line.StartsWith("string") ||
                        single_line.StartsWith("char") ||
                        single_line.StartsWith("double") ||
                        single_line.StartsWith("bool") ||
                        single_line.StartsWith("long")
                        )
                        && single_line.Contains("(") && single_line.Contains(")") && !single_line.Contains("="))
                    {
                        //2016.10.12
                        cmd_type = "LOGIC";
                        cmd_name = "function";
                        cmd_value = single_line;
                    }
                    else if (single_line.StartsWith("{"))
                    {
                        cmd_type = "LOGIC";
                        cmd_name = "{";
                    }
                    else if (single_line.StartsWith("}"))
                    {
                        cmd_type = "LOGIC";
                        cmd_name = "}";
                    }
                    else if (single_line.StartsWith("digitalWrite(") || single_line.StartsWith("digitalWrite ") || single_line.StartsWith("DigitalWrite(") || single_line.StartsWith("DigitalWrite "))
                    {
                        cmd_type = "CMD";
                        cmd_name = "digitalWrite";
                        cmd_value = SPL.Common.ParsingHelper.GetConditionExpression(single_line);
                    }
                    else if (single_line.StartsWith("analogWrite(") || single_line.StartsWith("analogWrite ") || single_line.StartsWith("AnalogWrite(") || single_line.StartsWith("AnalogWrite "))
                    {
                        cmd_type = "CMD";
                        cmd_name = "analogWrite";
                        cmd_value = SPL.Common.ParsingHelper.GetConditionExpression(single_line);
                    }
                    else if (single_line.StartsWith("tone(") || single_line.StartsWith("tone ") || single_line.StartsWith("Tone(") || single_line.StartsWith("Tone "))
                    {
                        cmd_type = "CMD";
                        cmd_name = "tone";
                        cmd_value = SPL.Common.ParsingHelper.GetConditionExpression(single_line);
                    }
                    else if (single_line.StartsWith("driveWrite(") || single_line.StartsWith("driveWrite ") || single_line.StartsWith("DriveWrite(") || single_line.StartsWith("DriveWrite "))
                    {
                        cmd_type = "CMD";
                        cmd_name = "driveWrite";
                        cmd_value = SPL.Common.ParsingHelper.GetConditionExpression(single_line);
                    }
                    else if (single_line.StartsWith("servoWrite(") || single_line.StartsWith("servoWrite ") || single_line.StartsWith("servoWrite(") || single_line.StartsWith("servoWrite "))
                    {
                        cmd_type = "CMD";
                        cmd_name = "servoWrite";
                        cmd_value = SPL.Common.ParsingHelper.GetConditionExpression(single_line);
                    }
                    else
                    {
                        //2016.10.12
                        cmd_type = "EXPR";
                        cmd_name = "Expression";
                        cmd_value = single_line;


                        int eq_ind = single_line.IndexOf('=');

                        if (eq_ind > 0)
                        {
                            string left_line = single_line.Substring(0, eq_ind).Trim();
                            string right_line = single_line.Substring(eq_ind + 1).Trim();

                            if (right_line.StartsWith("digitalRead(") || right_line.StartsWith("digitalRead ") || right_line.StartsWith("DigitalRead(") || right_line.StartsWith("DigitalRead "))
                            {
                                cmd_type = "CMD";
                                cmd_name = "digitalRead";
                                cmd_value = SPL.Common.ParsingHelper.GetConditionExpression(right_line);
                                cmd_variable = left_line;
                            }
                            else if (right_line.StartsWith("analogRead(") || right_line.StartsWith("analogRead ") || right_line.StartsWith("AnalogRead(") || right_line.StartsWith("AnalogRead "))
                            {
                                cmd_type = "CMD";
                                cmd_name = "analogRead";
                                cmd_value = SPL.Common.ParsingHelper.GetConditionExpression(right_line);
                                cmd_variable = left_line;
                            }
                            else if (right_line.StartsWith("map(") || right_line.StartsWith("map ") || right_line.StartsWith("Map(") || right_line.StartsWith("Map "))
                            {
                                cmd_type = "CMD";
                                cmd_name = "map";
                                cmd_value = SPL.Common.ParsingHelper.GetConditionExpression(right_line);
                                cmd_variable = left_line;
                            }
                        }
                    }

                    #endregion Arduino

                }


                cur_code_item = CreateItem(cmd_type, cmd_name, cmd_value, cmd_variable, true);

                #endregion for
            }


            //#########################################################################
            //코드 스크롤 영역의 크기를 계산한다.
            //#########################################################################

            UpdateCodeScrollSize();

            //#########################################################################


            #endregion if
        }
        else
        {
            //CreateItem("LOGIC", "setup function", string.Empty, string.Empty, true);
            //CreateItem("LOGIC", "{", string.Empty, string.Empty, true);
            //CreateItem("LOGIC", "}", string.Empty, string.Empty, true);

            //CreateItem("LOGIC", "loop function", string.Empty, string.Empty, false);
            //CreateItem("LOGIC", "{", string.Empty, string.Empty, true);
            //CreateItem("LOGIC", "}", string.Empty, string.Empty, true);
        }
    }


    #endregion LoadCodeScript



    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

    #region LoadMenuScript

    private void LoadMenuScript()
    {
        string menu_str = _MENU_TEXT_ASSETS[0].text;
        menu_str = System.Text.Encoding.Default.GetString(_MENU_TEXT_ASSETS[0].bytes);

        if (string.IsNullOrEmpty(menu_str))
            return;

        string[] menu_str_arr = menu_str.Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

        if (menu_str == null)
            return;

        string cur_group_name = string.Empty;
        string cur_cmd_name = string.Empty;

        List<string> cur_cmd_list = null;
        List<string> cur_option_list = null;

        for (int i = 0; i < menu_str_arr.Length; i++)
        {
            #region for each line

            string trim_line = menu_str_arr[i].Trim();

            if (string.IsNullOrEmpty(trim_line))
                continue;

            trim_line = trim_line.TrimStart('\t');

            if (trim_line.StartsWith("//"))
                continue;

            trim_line = ConvDirChar(trim_line);

            string[] items = trim_line.Split(new char[] { '/' }, System.StringSplitOptions.RemoveEmptyEntries);

            if (items == null || items.Length == 0)
                continue;


            //##############################################################################
            #region name value parsing

            Dictionary<string, string> name_value_pair = new Dictionary<string, string>();

            for (int k = 0; k < items.Length; k++)
            {
                string[] sub_arr = items[k].Trim().Split(new char[] { ':' }, System.StringSplitOptions.RemoveEmptyEntries);

                if (sub_arr != null && sub_arr.Length >= 2)
                {
                    if (!name_value_pair.ContainsKey(sub_arr[0].Trim()))
                        name_value_pair.Add(sub_arr[0].Trim(), sub_arr[1].Trim());
                }
            }

            #endregion name value parsing
            //##############################################################################


            if (items[0].StartsWith("addrootnode"))
            {
                #region addrootnode

                //현재 쌓여있는 것 처리
                if (!string.IsNullOrEmpty(cur_group_name))
                    _cmd_group_list.Add(cur_group_name);

                if (!string.IsNullOrEmpty(cur_cmd_name) && cur_cmd_list != null)
                    cur_cmd_list.Add(cur_cmd_name);

                if (!string.IsNullOrEmpty(cur_group_name) && cur_cmd_list != null)
                {
                    if (!_cmd_group_dic.ContainsKey(cur_group_name))
                        _cmd_group_dic.Add(cur_group_name, cur_cmd_list);
                }

                if (!string.IsNullOrEmpty(cur_cmd_name) && cur_option_list != null)
                {
                    if (!_cmd_option_dic.ContainsKey(cur_cmd_name))
                        _cmd_option_dic.Add(cur_cmd_name, cur_option_list);
                }


                //새로운 것 처리
                if (!name_value_pair.ContainsKey("name"))
                {
                    Debug.Log("None of name -> " + items[0] + " / " + trim_line);
                    continue;
                }

                cur_group_name = name_value_pair["name"];
                cur_group_name = cur_group_name.Trim('\"').TrimStart('[').TrimEnd(']');


                _left_cmd_list.Add("G" + cur_group_name);


                cur_cmd_name = string.Empty;
                cur_cmd_list = new List<string>();
                cur_option_list = new List<string>();

                #endregion addrootnode
            }
            else if (items[0].StartsWith("addcmdnode"))
            {
                #region addcmdnode

                //현재 쌓여있는 것 처리
                if (!string.IsNullOrEmpty(cur_cmd_name) && cur_cmd_list != null)
                    cur_cmd_list.Add(cur_cmd_name);

                if (!string.IsNullOrEmpty(cur_cmd_name) && cur_option_list != null)
                {
                    if (!_cmd_option_dic.ContainsKey(cur_cmd_name))
                        _cmd_option_dic.Add(cur_cmd_name, cur_option_list);
                }

                //새로운 것 처리
                if (!name_value_pair.ContainsKey("name"))
                {
                    Debug.Log("None of name -> " + items[0] + " / " + trim_line);
                    continue;
                }

                cur_cmd_name = name_value_pair["name"];
                cur_cmd_name = cur_cmd_name.Trim('\"');

                _left_cmd_list.Add("C" + cur_cmd_name);

                cur_option_list = new List<string>();

                #endregion addcmdnode
            }
            else if (items[0].StartsWith("addoptionnode"))
            {
                #region addoptionnode

                if (!name_value_pair.ContainsKey("name"))
                {
                    Debug.Log("None of name -> " + items[0] + " / " + trim_line);
                    continue;
                }

                string option_name = name_value_pair["name"];
                option_name = option_name.Trim('\"').TrimStart('!');
                if (cur_option_list != null)
                    cur_option_list.Add(option_name);

                #endregion addoptionnode
            }

            #endregion for each line
        }

        //현재 쌓여있는 것 처리
        if (!string.IsNullOrEmpty(cur_group_name))
            _cmd_group_list.Add(cur_group_name);

        if (!string.IsNullOrEmpty(cur_cmd_name) && cur_cmd_list != null)
            cur_cmd_list.Add(cur_cmd_name);

        if (!string.IsNullOrEmpty(cur_group_name) && cur_cmd_list != null)
        {
            if (!_cmd_group_dic.ContainsKey(cur_group_name))
                _cmd_group_dic.Add(cur_group_name, cur_cmd_list);
        }

        if (!string.IsNullOrEmpty(cur_cmd_name) && cur_option_list != null)
        {
            if (!_cmd_option_dic.ContainsKey(cur_cmd_name))
                _cmd_option_dic.Add(cur_cmd_name, cur_option_list);
        }

    }


    #endregion LoadMenuScript



    private string ConvDirChar(string line)
    {
        string res = string.Empty;

        bool open_flag = false;

        if (string.IsNullOrEmpty(line))
            return string.Empty;

        for (int i = 0; i < line.Length; i++)
        {
            if (line[i] == '^' && open_flag == false)
            {
                open_flag = true;
            }
            else if (line[i] == '^' && open_flag == true)
            {
                open_flag = false;
            }
            else if (line[i] == '/' && open_flag == true)
            {
                res = res + '|';
            }
            else
                res = res + line[i];
        }

        return res;
    }

    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

    #region LoadArduinoPinInfo

    private void LoadArduinoPinInfo()
    {
        _right_pin_list.Clear();


        if (_selected_coding_type == "TrafficSignal")
        {
            _right_pin_list.Add("(D02)  Speaker");
            _right_pin_list.Add("(D03)  Button Sensor");
            _right_pin_list.Add("(D04)  Button Sensor");
            _right_pin_list.Add("(D05)  Button Sensor");
            _right_pin_list.Add("(D06)  Button Sensor");
            _right_pin_list.Add("(D11)  LED Green");
            _right_pin_list.Add("(D12)  LED Yellow");
            _right_pin_list.Add("(D13)  LED Red");
        }
        else if (_selected_coding_type == "ButtonLight")
        {
            _right_pin_list.Add("(D03)  Button Sensor");
            _right_pin_list.Add("(D04)  Button Sensor");
            _right_pin_list.Add("(D05)  Button Sensor");
            _right_pin_list.Add("(D06)  Button Sensor");
            _right_pin_list.Add("(D11)  Fan Motor");
            _right_pin_list.Add("(D12)  Stand Light");
            _right_pin_list.Add("(D13)  Ceiling Light");

        }
        else if (_selected_coding_type == "AutoLight")
        {
            _right_pin_list.Add("(D11)  Ceiling Light");
            _right_pin_list.Add("(D12)  Stand Light");

            _right_pin_list.Add("(A0)  Brightness Sensor");
        }
        else if (_selected_coding_type == "DetectMovingObject")
        {
            _right_pin_list.Add("(D12)  Stand Light");
            _right_pin_list.Add("(D13)  Ceiling Light");

            _right_pin_list.Add("(A1)  Distance Sensor");
        }
        else if (_selected_coding_type == "SecurityAlert")
        {
            _right_pin_list.Add("(D02)  Speaker");
            _right_pin_list.Add("(D03)  Button Sensor");
            _right_pin_list.Add("(D13)  LED Red");

            _right_pin_list.Add("(A1)  Distance Sensor");
        }
        else if (_selected_coding_type == "PlaySound")
        {
            _right_pin_list.Add("(D02)  Speaker");
            _right_pin_list.Add("(D03)  Button Sensor");
            _right_pin_list.Add("(D04)  Button Sensor");
            _right_pin_list.Add("(D05)  Button Sensor");
            _right_pin_list.Add("(D06)  Button Sensor");
            _right_pin_list.Add("(D13)  Stand Light");

        }
        else if (_selected_coding_type == "RearDistanceSensing")
        {
            _right_pin_list.Add("(D02)  Speaker");
            _right_pin_list.Add("(D03)  Button Sensor");
            _right_pin_list.Add("(D11)  Head Light");
            _right_pin_list.Add("(D12)  Brake Light");

            _right_pin_list.Add("(A1)  Rear \nDistance Sensor");
            _right_pin_list.Add("(A2)  Front \nDistance Sensor");
        }
        else if (_selected_coding_type == "SimpleRobotDriving")
        {
            _right_pin_list.Add("(D03)  Button Sensor");
            _right_pin_list.Add("(D04)  Button Sensor");
            _right_pin_list.Add("(D05)  Button Sensor");
            _right_pin_list.Add("(D06)  Button Sensor");
            _right_pin_list.Add("(D11)  Head Light");
            _right_pin_list.Add("(D12)  Brake Light");

            _right_pin_list.Add("(A1)  Rear \nDistance Sensor");
            _right_pin_list.Add("(A2)  Front \nDistance Sensor");
        }
        else if (_selected_coding_type == "SCurveDriving")
        {
            _right_pin_list.Add("(D11)  Head Light");
            _right_pin_list.Add("(D12)  Brake Light");
        }
        else if (_selected_coding_type == "ReactionRobot")
        {
            _right_pin_list.Add("(D02)  Speaker");
            _right_pin_list.Add("(D03)  Button Sensor");
            _right_pin_list.Add("(D11)  Head Light");
            _right_pin_list.Add("(D12)  Brake Light");

            _right_pin_list.Add("(A1)  Left \nDistance Sensor");
            _right_pin_list.Add("(A2)  Front \nDistance Sensor");
        }
        else if (_selected_coding_type == "AvoidObstacle")
        {
            _right_pin_list.Add("(D02)  Speaker");
            _right_pin_list.Add("(D03)  Button Sensor");
            _right_pin_list.Add("(D11)  Head Light");
            _right_pin_list.Add("(D12)  Brake Light");

            _right_pin_list.Add("(A1)  Rear \nDistance Sensor");
            _right_pin_list.Add("(A2)  Front \nDistance Sensor");
        }
        else if (_selected_coding_type == "AutoParking")
        {
            _right_pin_list.Add("(D02)  Speaker");
            _right_pin_list.Add("(D03)  Button Sensor");
            _right_pin_list.Add("(D11)  Head Light");
            _right_pin_list.Add("(D12)  Brake Light");

            _right_pin_list.Add("(A1)  Rear \nDistance Sensor");
            _right_pin_list.Add("(A2)  Left \nDistance Sensor");
        }
        else if (_selected_coding_type == "DetectEdge")
        {
            _right_pin_list.Add("(D11)  Head Light");
            _right_pin_list.Add("(D12)  Brake Light");

            _right_pin_list.Add("(A1)  Right Bottom \nDistance Sensor");
            _right_pin_list.Add("(A2)  Left Bottom \nDistance Sensor");
        }
        else if (_selected_coding_type == "LineTracer")
        {
            _right_pin_list.Add("(D11)  Head Light");
            _right_pin_list.Add("(D12)  Brake Light");

            _right_pin_list.Add("(A1)  Right Bottom \nIR Sensor");
            _right_pin_list.Add("(A2)  Left Bottom \nIR Sensor");
        }
        else if (_selected_coding_type == "MazeExplorer")
        {
            _right_pin_list.Add("(D11)  Head Light");
            _right_pin_list.Add("(D12)  Brake Light");

            _right_pin_list.Add("(A1)  Front \nDistance Sensor");
            _right_pin_list.Add("(A2)  Left IR Sensor");
        }
        else if (_selected_coding_type == "SecurityAlertVR")
        {
            _right_pin_list.Add("(D02)  Speaker");
            _right_pin_list.Add("(D13)  LED Red");

            _right_pin_list.Add("(A1)  Distance Sensor");
        }
        else
        {
            _right_pin_list.Add("Brightness Sensor");
            _right_pin_list.Add("Button Sensor");
            _right_pin_list.Add("Distance Sensor");
            _right_pin_list.Add("IR Sensor");
            _right_pin_list.Add("LED Green");
            _right_pin_list.Add("LED Red");
            _right_pin_list.Add("LED Yellow");
            _right_pin_list.Add("Light");
            _right_pin_list.Add("Speaker");
            _right_pin_list.Add("Joystick X");
            _right_pin_list.Add("Joystick Y");
        }
    }

    #endregion LoadArduinoPinInfo

    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
            _key_down_code = "Space";
        else if (Input.GetKeyDown(KeyCode.Escape))
            _key_down_code = "Escape";

        //화면 해상도가 변경되면 전체 크기 변수들을 다시 계산해 준다.
        if (Screen.width != _screen_width || Screen.height != _screen_height)
        {
            _screen_width = Screen.width;
            _screen_height = Screen.height;

            SetupWorks();
        }
    }

    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@



    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    void OnGUI()
    {
        if (_first_set_flag == false)
        {
            //GUI 설정
            GUI.skin.verticalScrollbar.fixedWidth = 10 * _w_ratio;
            GUI.skin.verticalScrollbarThumb.fixedWidth = 10 * _w_ratio;
            GUI.skin.verticalScrollbarThumb.fixedHeight = 20 * _h_ratio;

            GUI.skin.verticalScrollbar.normal.background = V2.Utils.MakeTex(10, 10, V2.Utils.ToFloatColor(200, 200, 200), false);
            GUI.skin.verticalScrollbarThumb.normal.background = V2.Utils.MakeTex(10, 10, V2.Utils.ToFloatColor(0, 0, 0), false);

            _first_set_flag = true;
        }


        //속성 가운데 정렬 문자
        if (_GuiStyle_font_text_field_center == null)
        {
            _GuiStyle_font_text_field_center = new GUIStyle(GUI.skin.textField);
            _GuiStyle_font_text_field_center.font = _FONTS[0];
            _GuiStyle_font_text_field_center.fontSize = (int)(18 * _s_ratio);
            _GuiStyle_font_text_field_center.normal.textColor = new Color(1, 1, 1);
            _GuiStyle_font_text_field_center.alignment = TextAnchor.MiddleCenter;
        }

        //코드 편집기 스타일
        if (_GuiStyle_font_text_area_editor == null)
        {
            _GuiStyle_font_text_area_editor = new GUIStyle(GUI.skin.textArea);
            _GuiStyle_font_text_area_editor.font = _FONTS[0];
            _GuiStyle_font_text_area_editor.fontSize = (int)(22 * _s_ratio);
            _GuiStyle_font_text_area_editor.normal.textColor = new Color(1, 1, 1);
            _GuiStyle_font_text_area_editor.hover.textColor = new Color(1, 1, 1);
            _GuiStyle_font_text_area_editor.active.textColor = new Color(1, 1, 1);
            _GuiStyle_font_text_area_editor.alignment = TextAnchor.UpperLeft;

            _GuiStyle_font_text_area_editor.padding.left = (int)(10 * _w_ratio);
            _GuiStyle_font_text_area_editor.padding.right = (int)(10 * _w_ratio);
        }


        //속성값 폰트
        if (_GuiStyle_font_text_field == null)
        {
            _GuiStyle_font_text_field = new GUIStyle(GUI.skin.textField);
            _GuiStyle_font_text_field.font = _FONTS[0];
            _GuiStyle_font_text_field.fontSize = (int)(20 * _s_ratio);
            _GuiStyle_font_text_field.normal.textColor = new Color(1, 1, 1);
            _GuiStyle_font_text_field.alignment = TextAnchor.MiddleLeft;

            _GuiStyle_font_text_field.padding.left = (int)(10 * _w_ratio);
            _GuiStyle_font_text_field.padding.right = (int)(10 * _w_ratio);
        }


        //속성값 폰트
        if (_font_text_field_style == null)
        {
            _font_text_field_style = new GUIStyle(GUI.skin.textField);
            _font_text_field_style.font = _FONTS[0];
            _font_text_field_style.fontSize = (int)(28 * _s_ratio);
            _font_text_field_style.normal.textColor = new Color(1, 1, 1);
            _font_text_field_style.alignment = TextAnchor.MiddleLeft;

            _font_text_field_style.padding.left = (int)(10 * _w_ratio);
            _font_text_field_style.padding.right = (int)(10 * _w_ratio);
        }



        //################################################
        //화면 터치 스크롤 기능 구현
        //################################################
        if (_dialog_name == "CodeEditor")
            CheckTouchScroll();
        //################################################


        if (_dialog_name == "CodeEditor")
        {
            GUI_CodeEditor();
        }
        else if (_dialog_name == "FileSave")
        {
            GUI_FileSave();
        }
        else if (_dialog_name == "ServerSave")
        {
            GUI_ServerSave();
        }

        //################################################
        //################################################
        //드래그 앤 드랍 명령어를 그려준다.
        //################################################
        //################################################

        //if (_block_cmd_dragging_mode && !string.IsNullOrEmpty(_dragged_item_name))
        if (_block_cmd_dragging_mode && !string.IsNullOrEmpty(_dragged_cmd_item_type))
        {
            GUI.Label(_dragged_block_rect, _dragged_item_name, _dragged_box_style);
        }

        //################################################
        //################################################




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
            else if (_dialog_name == "FileSave" || _dialog_name == "ServerSave")
            {
                //이전의 화면으로 돌아간다,

                //##########################################            
                _fadeScreenLibrary.RequestFadeOut("CodeEditor", true);
                //##########################################                
            }
            else
            {
                //MainMenu 화면으로 이동한다.

                //##########################################            
                _fadeScreenLibrary.RequestFadeOut("Back", true);
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
        if (fade_res == "CodeEditor")
        {
            _dialog_name = "CodeEditor";
            _dialog_msg = string.Empty;

            //다시 화면을 밝게 처리한다.
            _fadeScreenLibrary.StartFadeIn(true);
        }
        else if (fade_res == "FileSave")
        {
            _dialog_name = "FileSave";
            _dialog_msg = string.Empty;

            //다시 화면을 밝게 처리한다.
            _fadeScreenLibrary.StartFadeIn(true);
        }
        else if (fade_res == "ServerSave")
        {
            _dialog_name = "ServerSave";
            _dialog_msg = string.Empty;

            //다시 화면을 밝게 처리한다.
            _fadeScreenLibrary.StartFadeIn(true);
        }
        else if (fade_res == "Execute")
        {
            _dialog_name = "";
            _dialog_msg = string.Empty;

            if (GlobalVariables.SelectedCodingType == "TrafficSignal")
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Scene_TrafficSignal");
            }
            else if (GlobalVariables.SelectedCodingType == "ButtonLight")
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Scene_ButtonLight");
            }
            else if (GlobalVariables.SelectedCodingType == "AutoLight")
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Scene_AutoLight");
            }
            else if (GlobalVariables.SelectedCodingType == "DetectMovingObject")
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Scene_DetectMovingObject");
            }
            else if (GlobalVariables.SelectedCodingType == "SecurityAlert")
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Scene_SecurityAlert");
            }
            else if (GlobalVariables.SelectedCodingType == "PlaySound")
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Scene_PlaySound");
            }
            else if (GlobalVariables.SelectedCodingType == "RearDistanceSensing")
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Scene_RearDistanceSensing");
            }
            else if (GlobalVariables.SelectedCodingType == "SimpleRobotDriving")
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Scene_SimpleCarDriving");
            }
            else if (GlobalVariables.SelectedCodingType == "SCurveDriving")
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Scene_SCurveDriving");
            }
            else if (GlobalVariables.SelectedCodingType == "ReactionRobot")
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Scene_ReactionCar");
            }
            else if (GlobalVariables.SelectedCodingType == "AvoidObstacle")
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Scene_AvoidObstacle");
            }
            else if (GlobalVariables.SelectedCodingType == "AutoParking")
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Scene_AutoParking");
            }
            else if (GlobalVariables.SelectedCodingType == "DetectEdge")
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Scene_DetectEdge");
                //UnityEngine.SceneManagement.SceneManager.LoadScene("Scene_DetectEdge2");
            }
            else if (GlobalVariables.SelectedCodingType == "LineTracer")
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Scene_LineTracer");
            }
            else if (GlobalVariables.SelectedCodingType == "MazeExplorer")
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Scene_MazeExplorer");
            }
            else if (GlobalVariables.SelectedCodingType == "SecurityAlertVR")
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Scene_SecurityAlertVR");
            }

        }
        else if (fade_res == "Back")
        {
            _dialog_name = "";
            UnityEngine.SceneManagement.SceneManager.LoadScene("Scene_MainMenu");
        }
    }

    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@



    //#############################################################################################
    //#############################################################################################
    //    Touch Scroll
    //#############################################################################################
    //#############################################################################################


    #region CheckTouchScroll()

    bool _TOUCH_DOWN_FLAG = false;
    bool _TOUCH_UP_FLAG = false;
    bool _IS_DRAGGING_MODE = false;
    Vector2 _FIRST_POS = new Vector2(0, 0);
    Vector2 _PRE_POS = new Vector2(0, 0);
    Vector2 _CUR_POS = new Vector2(0, 0);
    Vector2 _CUR_MOUSE_POS = new Vector2(0, 0);
    Vector2 _TOUCH_OFFSET_POS = new Vector2(0, 0);
    string _TOUCH_REGION_NAME = string.Empty;

    //###########################################
    float _TOUCH_MOVE_MOMENTUM_Y = 0;
    string _MOMENTUM_REGION_NAME = string.Empty;
    //###########################################

    void CheckTouchScroll()
    {
        //########################################################################
        // 드래깅 시 마우스 위치 Code
        //########################################################################
        if (_block_cmd_dragging_mode)
        {
            _CUR_MOUSE_POS.x = Input.mousePosition.x;
            _CUR_MOUSE_POS.y = Screen.height - Input.mousePosition.y;
            _dragged_block_rect.x = _CUR_MOUSE_POS.x - (_dragged_block_rect.width * 0.5f);
            _dragged_block_rect.y = _CUR_MOUSE_POS.y - (_dragged_block_rect.height * 0.5f);
        }
        //########################################################################
        if (Input.GetMouseButtonUp(0))
        {
            _TOUCH_UP_FLAG = true;

            _FIRST_POS.x = Input.mousePosition.x;
            _FIRST_POS.y = Screen.height - Input.mousePosition.y;
        }

        if (Input.GetMouseButton(0))
        {
            #region

            if (_TOUCH_DOWN_FLAG == false)
            {
                #region

                //눌려 지는 순간
                //_FIRST_POS = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
                //_PRE_POS = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);

                _FIRST_POS.x = Input.mousePosition.x;
                _FIRST_POS.y = Screen.height - Input.mousePosition.y;

                _PRE_POS.x = Input.mousePosition.x;
                _PRE_POS.y = Screen.height - Input.mousePosition.y;


                //왼쪽 메뉴 리스트 먼저 체크
                if (_left_menu_scroll_check_rect.Contains(_FIRST_POS))
                {
                    _TOUCH_REGION_NAME = "LEFT";
                }
                else if (_right_pin_scroll_check_rect.Contains(_FIRST_POS))
                {
                    _TOUCH_REGION_NAME = "RIGHT";
                }
                else if (_center_code_scroll_check_rect.Contains(_FIRST_POS))
                {
                    _TOUCH_REGION_NAME = "CODE";
                }

                #endregion
            }
            else
            {
                if (!string.IsNullOrEmpty(_TOUCH_REGION_NAME))
                {
                    #region

                    //드래그 하는 순간
                    //_CUR_POS = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
                    _CUR_POS.x = Input.mousePosition.x;
                    _CUR_POS.y = Screen.height - Input.mousePosition.y;


                    _TOUCH_OFFSET_POS.x = _CUR_POS.x - _PRE_POS.x;
                    _TOUCH_OFFSET_POS.y = _CUR_POS.y - _PRE_POS.y;

                    float diff_x_abs = UnityEngine.Mathf.Abs(_TOUCH_OFFSET_POS.x);
                    float diff_y_abs = UnityEngine.Mathf.Abs(_TOUCH_OFFSET_POS.y);


                    if (diff_y_abs > diff_x_abs && _block_cmd_dragging_mode == false)
                    {
                        if (_TOUCH_REGION_NAME == "LEFT")
                        {
                            _scroll_pos_left_menu.y = _scroll_pos_left_menu.y - _TOUCH_OFFSET_POS.y;
                        }
                        else if (_TOUCH_REGION_NAME == "RIGHT")
                        {
                            _scroll_pos_right_pin.y = _scroll_pos_right_pin.y - _TOUCH_OFFSET_POS.y;
                        }
                        else if (_TOUCH_REGION_NAME == "CODE")
                        {
                            _scroll_pos_center_code.y = _scroll_pos_center_code.y - _TOUCH_OFFSET_POS.y;
                        }
                    }
                    else if (diff_x_abs > diff_y_abs && diff_x_abs > (10 * _w_ratio))
                    {
                        //옆으로 이동한 경우 명령어를 드래그 하는 경우임
                        if (_TOUCH_REGION_NAME == "LEFT")
                        {
                            _block_cmd_dragging_mode = true;
                            _dragged_cmd_from = _TOUCH_REGION_NAME;
                        }
                        else if (_TOUCH_REGION_NAME == "CODE")
                        {
                            _block_cmd_dragging_mode = true;
                            _dragged_cmd_from = _TOUCH_REGION_NAME;
                        }
                    }


                    //차이를 없에기 위해 마지막 위치를 저장
                    _PRE_POS = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);

                    #endregion
                }
            }

            _TOUCH_DOWN_FLAG = true;

            #endregion
        }
        else
        {
            if (_TOUCH_DOWN_FLAG)
            {
                #region


                //눌렀다가 떼는 순간
                //_CUR_POS = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
                _CUR_POS.x = Input.mousePosition.x;
                _CUR_POS.y = Screen.height - Input.mousePosition.y;

                _TOUCH_OFFSET_POS.x = _CUR_POS.x - _FIRST_POS.x;
                _TOUCH_OFFSET_POS.y = _CUR_POS.y - _FIRST_POS.y;

                float diff_x_abs = UnityEngine.Mathf.Abs(_TOUCH_OFFSET_POS.x);
                float diff_y_abs = UnityEngine.Mathf.Abs(_TOUCH_OFFSET_POS.y);

                //#########################################################################################
                //아래 코드는 드래그 앤 드랍이 끝났을 때의 실제 명령어를 처리한다.
                //드랍된 결과물에 대한 실제 액션 (생성 또는 이동)을 이곳에서 처리한다.
                //#########################################################################################

                if (_block_cmd_dragging_mode)
                {
                    #region _block_cmd_dragging_mode

                    //드랍 시킨 영역이 어디인지 우선 체크한다
                    //왼쪽 명령어에 드랍한 경우는 제외시키기 위함암
                    //왼쪽 메뉴 리스트 먼저 체크

                    string dropped_region_name = string.Empty;

                    if (_left_menu_scroll_check_rect.Contains(_CUR_POS))
                        dropped_region_name = "LEFT";
                    else if (_right_pin_scroll_check_rect.Contains(_CUR_POS))
                        dropped_region_name = "RIGHT";
                    else if (_center_code_scroll_check_rect.Contains(_CUR_POS))
                        dropped_region_name = "CODE";


                    if (dropped_region_name == "CODE" && string.IsNullOrEmpty(_drag_taget_up_down))
                    {
                        #region CODE Empty Region

                        //코드 영역의 빈 영역에 드랍시킨 경우는 Root에 추가한다.

                        if (_code_btn_root_item == null)
                        {
                            //Root 객체는 반드시 존재해야 한다.
                            _code_btn_selected_item = new Code_Item("ROOT", "ROOT", string.Empty, null);
                            _code_btn_root_item = _code_btn_selected_item;
                        }

                        //현재 루트 형제 명령어의 맨 마지막 명령어로 이동
                        _code_btn_selected_item = GetLastSiblingItem(_code_btn_root_item);

                        //블록 끝 영역을 선택영역으로 함
                        if (_code_btn_selected_item.BlockYN && _code_btn_selected_item.BlockEndItem != null)
                            _code_btn_selected_item = _code_btn_selected_item.BlockEndItem;

                        #endregion CODE Empty Region
                    }


                    if (_dragged_cmd_from == "LEFT")
                    {
                        //왼쪽 명령어 목록에서 드래그 해서 Code 영역에 드래그한 경우 처리

                        #region LEFT

                        //왼쪽 명령어 리스트에 드랍하는 경우를 제외하고 처리한다.
                        if (dropped_region_name == "CODE")
                        {
                            //추가하기 전에 현재 코드를 되돌리기 리스트에 추가한다.
                            BackupScriptHistryList();



                            if (_dragged_cmd_item_type != "" && _dragged_item_name != "")
                            {
                                //새로운 명령어를 추가함
                                _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);
                                CreateItem(_dragged_cmd_item_type, _dragged_item_name, string.Empty, string.Empty, false);
                            }



                            //#########################################################################
                            //코드 스크롤 영역의 크기를 계산한다.
                            //#########################################################################
                            UpdateCodeScrollSize();
                            //#########################################################################
                        }

                        #endregion LEFT
                    }
                    else if (_dragged_cmd_from == "CODE")
                    {

                        //코드 영역에서 드래그 해서 자신의 Code 영역에 드래그한 경우 처리
                        //이동이나 복사의 경우

                        #region CODE

                        //코드 영역에 정상적으로 블럭을 드랍시킨 경우를 처리한다.

                        if (dropped_region_name == "CODE")
                        {
                            //명령어를 복사하거나 이동시켜 준다.
                            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                            {
                                //시프트 키를 누르고 드래그한 경우는 블록 명령어를 복사해 준다

                                Code_Item src_instance = FindCodeItem(_dragging_source_id);

                                if (src_instance != null)
                                {
                                    //추가하기 전에 현재 코드를 되돌리기 리스트에 추가한다.
                                    BackupScriptHistryList();


                                    Code_Item new_instance = src_instance.Clone();

                                    if (new_instance != null)
                                        AddNewCodeTree(new_instance, new_instance.BlockYN, false, false);
                                }
                            }
                            else
                            {
                                //그렇지 않은 경우는 그냥 명령어를 이동시켜 준다.
                                //맨 아래 항목은 이동되면 오류 발생. 아래는 이 오류 방지 코드용
                                Code_Item last_item = GetLastRootSiblingItem();

                                if (last_item == null)
                                {
                                    //Do nothing;
                                }
                                else if (string.IsNullOrEmpty(_dragging_target_id))
                                {
                                    //공백 영역에 드랍한 경우 맨 마지막 항목을 선택한 경우로 한다.
                                    if (last_item.BlockYN && last_item.BlockEndItem != null)
                                        last_item = last_item.BlockEndItem;

                                    _dragging_target_id = last_item.Id;
                                    _drag_taget_up_down = "DOWN";
                                }


                                if (_dragging_source_id != _dragging_target_id)
                                {
                                    Code_Item src_instance = FindCodeItem(_dragging_source_id);

                                    if (src_instance != null)
                                    {
                                        //이동시키기 전에 현재 코드를 되돌리기 리스트에 추가한다.
                                        BackupScriptHistryList();

                                        //명령어를 이동시켜 준다.
                                        RemoveFromCodeTree(src_instance);
                                        AddNewCodeTree(src_instance, src_instance.BlockYN, false, false);
                                    }
                                }

                            }

                            //#########################################################################
                            //코드 스크롤 영역의 크기를 계산한다.
                            //#########################################################################
                            UpdateCodeScrollSize();
                            //#########################################################################
                        }

                        #endregion CODE
                    }

                    #endregion _block_cmd_dragging_mode
                }


                if (_remove_icon_clicked)
                {
                    //명령어 삭제 기능을 처리한다.
                    //Do Nothing
                }
                else
                {
                    #region else

                    //#########################################################################################
                    //아래 코드는 움직임 모멘텀을 만들거나 작은 움직임을 무시하는 용도로만 사용한다.
                    //#########################################################################################

                    if (diff_y_abs > diff_x_abs && diff_y_abs > (10 * _h_ratio))
                    {
                        if (_TOUCH_REGION_NAME == "LEFT" || _TOUCH_REGION_NAME == "RIGHT" || _TOUCH_REGION_NAME == "CODE")
                        {
                            _IS_DRAGGING_MODE = true;

                            //#################################################
                            //다시 계산
                            _TOUCH_OFFSET_POS.x = _CUR_POS.x - _PRE_POS.x;
                            _TOUCH_OFFSET_POS.y = _CUR_POS.y - _PRE_POS.y;

                            _TOUCH_MOVE_MOMENTUM_Y = _TOUCH_OFFSET_POS.y;
                            _MOMENTUM_REGION_NAME = _TOUCH_REGION_NAME;
                            //#################################################
                        }
                    }
                    else if (diff_x_abs > diff_y_abs && diff_x_abs > (10 * _w_ratio))
                    {
                        if (_TOUCH_REGION_NAME == "LEFT" || _TOUCH_REGION_NAME == "RIGHT" || _TOUCH_REGION_NAME == "CODE")
                        {
                            _IS_DRAGGING_MODE = true;
                            //작은 움직임 무시
                        }
                    }
                    else
                    {
                        //2018.04.03 버그 수정
                        //######################################
                        //움직임이 작으면 그냥 버튼 클릭으로 처리
                        _IS_DRAGGING_MODE = false;
                        //######################################
                    }

                    #endregion else
                }
                //#########################################################################################





                #endregion
            }
            else
            {
                #region

                //2018.04.03
                //######################################################################
                //터치가 않된 정상적인 경우
                //만약 이전에 이동 모멘텀이 남아 있다면 이동을 시켜준다.

                float diff_y_abs = UnityEngine.Mathf.Abs(_TOUCH_MOVE_MOMENTUM_Y);


                if (diff_y_abs < 0.1f)
                {
                    _TOUCH_MOVE_MOMENTUM_Y = 0;
                    _MOMENTUM_REGION_NAME = string.Empty;
                }
                else
                {
                    if (_MOMENTUM_REGION_NAME == "LEFT")
                    {
                        _scroll_pos_left_menu.y = _scroll_pos_left_menu.y - _TOUCH_MOVE_MOMENTUM_Y;
                    }
                    else if (_MOMENTUM_REGION_NAME == "RIGHT")
                    {
                        _scroll_pos_right_pin.y = _scroll_pos_right_pin.y - _TOUCH_MOVE_MOMENTUM_Y;
                    }
                    else if (_MOMENTUM_REGION_NAME == "CODE")
                    {
                        _scroll_pos_center_code.y = _scroll_pos_center_code.y - _TOUCH_MOVE_MOMENTUM_Y;
                    }

                    float redu_ratio = 0.95f - Time.deltaTime;
                    _TOUCH_MOVE_MOMENTUM_Y = _TOUCH_MOVE_MOMENTUM_Y * redu_ratio;
                }

                //######################################################################

                #endregion
            }

            _TOUCH_DOWN_FLAG = false;
            _TOUCH_REGION_NAME = string.Empty;
            //_TOUCH_UP_FLAG = false;

            //명령어 드래깅 여부
            _block_cmd_dragging_mode = false;
            _dragged_cmd_from = string.Empty;
            _dragged_cmd_item_type = string.Empty;
            _dragged_item_name = string.Empty;
            _dragged_code_item = null;
            _dragging_source_id = string.Empty;
            _dragging_target_id = string.Empty;
        }
    }


    #endregion CheckTouchScroll()




    void GUI_CodeEditor()
    {
        GUI.DrawTexture(_bg_rects[0], _TEXTURES[0]);

        Draw_Header();

        Draw_LeftMenu();


        string cur_item_name = string.Empty;
        string disp_item_name = cur_item_name;


        if (_code_btn_selected_item != null && _code_btn_selected_item.ValueYN)
        {
            cur_item_name = GetCurSelectedItemName();
            disp_item_name = cur_item_name;
        }


        Draw_CenterCode();

        Draw_RightPins();

        Draw_RightProperty(cur_item_name, disp_item_name);

        Draw_RightButton();

        if (_remove_icon_clicked)
        {
            Draw_Delete();
        }
    }


    void Draw_Header()
    {

        //Title
        GUI.Label(_header_title_rects, _FILE_NAME, _text_styles[1]);

        //Stage Name
        GUI.Label(_header_stage_name_rects, _STAGE_NAME, _text_styles[2]);


        //Home Button
        if (GUI.Button(_header_btn_rects[0], "", _header_btn_styles[0]))
        {
            _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);	//button sound

            //##########################################            
            _fadeScreenLibrary.RequestFadeOut("Back", true);
            //##########################################            
        }

        //ServerSave Button
        if (GUI.Button(_header_btn_rects[1], "", _header_btn_styles[1]))
        {
            _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);	//button sound

            _USER_ID = PlayerPrefs.GetString("_USER_ID");
            _dialog_msg = string.Empty;

            //##########################################            
            _fadeScreenLibrary.RequestFadeOut("ServerSave", true);
            //##########################################            
        }

        //FileSave Button
        if (GUI.Button(_header_btn_rects[2], "", _header_btn_styles[2]))
        {
            _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);	//button sound


            _dialog_msg = string.Empty;

            //##########################################            
            _fadeScreenLibrary.RequestFadeOut("FileSave", true);
            //##########################################            
        }

        //Undo Button
        if (GUI.Button(_header_btn_rects[3], "", _header_btn_styles[3]))
        {
            _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);

            RestoreFromHistryList();
        }
    }


    void Draw_LeftMenu()
    {
        GUI.Box(_left_menu_region_rects, "", _gui_box_white_bg_styles);



        string ArduinoCommandesString = "Arduino Commands";
        if (Application.systemLanguage == SystemLanguage.Korean)
        {
            _text_styles[3].font = _FONTS[1];
            ArduinoCommandesString = "아두이노 명령어";
        }
        //Group Title
        GUI.Label(_left_menu_title_rects, ArduinoCommandesString, _text_styles[3]);


        //#################
        //스크롤 영역 시작
        //#################

        _scroll_pos_left_menu = GUI.BeginScrollView(_left_menu_scroll_view_rect, _scroll_pos_left_menu, _left_menu_scroll_content_rect, false, true);


        for (int i = 0; i < _left_cmd_list.Count; i++)
        {
            _left_icon_rect.y = (16 + (i * 45)) * _h_ratio;
            _left_btn_rect.y = (10 + (i * 45)) * _h_ratio;

            string item_name = _left_cmd_list[i];
            char cmd_type = item_name[0];
            item_name = item_name.Substring(1);

            if (cmd_type == 'G')
            {
                _left_btn_rect.x = 50 * _w_ratio;

                if (item_name.StartsWith("Logic"))
                {
                    GUI.DrawTexture(_left_icon_rect, _ICON_TEXTURES[2]);
                    if (Application.systemLanguage == SystemLanguage.Korean)
                    {
                        _text_styles[4].font = _FONTS[1];
                        GUI.Label(_left_btn_rect, "논리", _text_styles[4]);
                    }
                    else if (Application.systemLanguage == SystemLanguage.English)
                    {
                        GUI.Label(_left_btn_rect, item_name, _text_styles[4]);
                    }

                }
                else if (item_name.StartsWith("Arduino"))
                {
                    GUI.DrawTexture(_left_icon_rect, _ICON_TEXTURES[3]);

                    if (Application.systemLanguage == SystemLanguage.Korean)
                    {
                        _text_styles[4].font = _FONTS[1];
                        GUI.Label(_left_btn_rect, "아두이노", _text_styles[4]);
                    }
                    else if (Application.systemLanguage == SystemLanguage.English)
                    {
                        GUI.Label(_left_btn_rect, item_name, _text_styles[4]);
                    }
                }
            }
            else
            {
                _left_btn_rect.x = 10 * _w_ratio;

                int btn_color_style_ind = 0;

                string cmd_item_type = "CMD";

                if (item_name == "Expression")
                {
                    cmd_item_type = "EXPR";
                    btn_color_style_ind = 5;
                }
                else if (item_name == "Print" || item_name == "PrintLine")
                {
                    cmd_item_type = "EXPR";
                    btn_color_style_ind = 0;
                }
                else if (item_name == "Serial.print" || item_name == "Serial.println")
                {
                    cmd_item_type = "EXPR";
                    btn_color_style_ind = 0;
                }
                else if (item_name == "if" || item_name == "else" || item_name == "else if" || item_name == "for" || item_name == "while")
                {
                    cmd_item_type = "LOGIC";
                    btn_color_style_ind = 1;
                }
                else if (item_name == "{" || item_name == "}")
                {
                    cmd_item_type = "LOGIC";
                    btn_color_style_ind = 2;
                }
                else if (item_name == "Setup Function" || item_name == "Loop Function" || item_name == "Function")
                {
                    cmd_item_type = "LOGIC";
                    btn_color_style_ind = 2;
                }
                else if (item_name == "setup function" || item_name == "loop function" || item_name == "function")
                {
                    cmd_item_type = "LOGIC";
                    btn_color_style_ind = 2;
                }
                else if (item_name == "tone" || item_name == "driveWrite" || item_name == "servoWrite" || item_name == "map")
                {
                    cmd_item_type = "CMD";
                    btn_color_style_ind = 4;
                }
                else
                    btn_color_style_ind = 3;


                if (GUI.Button(_left_btn_rect, item_name, _left_btn_color_styles[btn_color_style_ind]))
                {
                    if (_IS_DRAGGING_MODE)
                        _IS_DRAGGING_MODE = false;
                    else
                    {
                        if (_remove_icon_clicked == false)
                        {
                            //새로운 명령어를 추가함
                            _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);
                            CreateItem(cmd_item_type, item_name, string.Empty, string.Empty, false);
                        }
                    }

                }


                //명령어 드래깅 상태를 체크함
                if (_block_cmd_dragging_mode && string.IsNullOrEmpty(_dragged_item_name))
                {
                    _dragged_contains_rect.x = _left_menu_scroll_view_rect.x + _left_btn_rect.x + _scroll_pos_left_menu.x;
                    _dragged_contains_rect.y = _left_menu_scroll_view_rect.y + _left_btn_rect.y - _scroll_pos_left_menu.y;
                    _dragged_contains_rect.width = _left_btn_rect.width;

                    //맨 처음 마우스를 클릭한 위치의 명령어를 찾는다.
                    if (_dragged_contains_rect.Contains(_FIRST_POS))
                    {
                        _dragged_cmd_item_type = cmd_item_type;
                        _dragged_item_name = item_name;
                    }

                }

            }
        }


        //#################
        //스크롤 영역 끝
        //#################
        GUI.EndScrollView();
    }


    //######################################################################




    void Draw_CenterCode()
    {
        GUI.Box(_center_code_region_rects, "", _gui_box_white_bg_styles);

        GUIStyle block_style = _text_styles[20];
        GUIStyle text_style = _text_styles[21];

        if (SPLCodeEditor._EDITOR_TYPE == "TEXT")
        {
            block_style = _text_styles[21];
            text_style = _text_styles[20];
        }

        string BlockModeString = "Block Mode";
        string TextModeString = "Text Mode";
        if (Application.systemLanguage == SystemLanguage.Korean)
        {
            block_style.font = _FONTS[1];
            text_style.font = _FONTS[1];
            BlockModeString = "블록 모드";
            TextModeString = "텍스트 모드";
        }

        if (GUI.Button(_center_code_block_title_rects, BlockModeString, block_style))
        {
            if (SPLCodeEditor._EDITOR_TYPE == "TEXT")
            {
                SPLCodeEditor._EDITOR_TYPE = "BLOCK";

                string[] lines = _CURRENT_SCRIPT_TEXT.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
                LoadCodeScript_Internal(lines);
            }
        }

        if (GUI.Button(_center_code_text_title_rects, TextModeString, text_style))
        {
            if (SPLCodeEditor._EDITOR_TYPE == "BLOCK")
            {
                SPLCodeEditor._EDITOR_TYPE = "TEXT";
                _CURRENT_SCRIPT_TEXT = ExportScriptAsText();
            }
        }



        if (SPLCodeEditor._EDITOR_TYPE == "BLOCK")
            Draw_CenterCode_Block();
        else
            Draw_CenterCode_Text();

    }


    void Draw_CenterCode_Block()
    {
        //#################
        //스크롤 영역 시작
        //#################
        _scroll_pos_center_code = GUI.BeginScrollView(_rect_center_code_scroll_view, _scroll_pos_center_code, _rect_center_code_scroll_content, false, true);

        #region BeginScrollView


        //########################################################
        // 모든 블록 명령어들에 대해 드래킹 체크하기 전에 일단 초기화 해 놓는다.
        // 아래 명령어들을 모두 끝난 후에 만약 드래깅 대상이 있는 경우는 아래 변수에 id가 저장되어 있어야 한다,
        //########################################################
        _drag_taget_up_down = string.Empty;
        _dragging_target_id = string.Empty;


        //###########################################################
        //_CODE_LIST의 코드 목록을 반복문을 통해 출력한다.
        //###########################################################

        _dumy_rect_code_item.x = 10 * _w_ratio;

        //_draw_code_line_cnt = -1;

        //마지막 그려진 명령어를 저장하기 위한 위치값을 초기화 한다.
        _last_code_rect_code_item.x = 0;
        _last_code_rect_code_item.y = 10 * _h_ratio;


        if (_code_btn_root_item != null && _code_btn_root_item.NextCmdItem != null)
            Draw_CodeList(_code_btn_root_item.NextCmdItem, 0);


        #endregion EndScrollView

        //#################
        //스크롤 영역 끝
        //#################
        GUI.EndScrollView();
    }


    Vector2 scrollPosition = Vector2.zero;

    void Draw_CenterCode_Text()
    {
        float scroll_height_limit = _rect_center_code_scroll_view.height - (50 * _h_ratio);

        GUILayout.BeginArea(_rect_center_code_scroll_view);

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.ExpandHeight(true));

        Event ev = Event.current;
        _CURRENT_SCRIPT_TEXT = GUILayout.TextArea(_CURRENT_SCRIPT_TEXT, _GuiStyle_font_text_area_editor, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);

        //탭키 입력을 위한 기능임
        if (GUIUtility.keyboardControl == editor.controlID && ev.Equals(Event.KeyboardEvent("tab")) && editor.cursorIndex >= 0)
        {
            _CURRENT_SCRIPT_TEXT = _CURRENT_SCRIPT_TEXT.Insert(editor.cursorIndex, "\t");
            editor.Insert('\t');
        }


        if (ev.isKey)
        {
            //키입력이 있으면 현재의 코드를 저정한다
            //변경하기 전에 현재 코드를 되돌리기 리스트에 추가한다.
            BackupScriptHistryList();
        }


        GUILayout.EndScrollView();

        GUILayout.EndArea();


        //float cursor_scroll_pos = editor.graphicalCursorPos.y - scrollPosition.y;

        //if (cursor_scroll_pos > scroll_height_limit)
        //{
        //    scrollPosition.y = scrollPosition.y + scroll_height_limit;
        //}
        //else if (cursor_scroll_pos < 0)
        //{
        //    scrollPosition.y = scrollPosition.y + cursor_scroll_pos;
        //}
    }




    private void Draw_CodeList(Code_Item code_item, int level_num)
    {
        #region Draw_CodeList

        //########################################################
        // 명령어가 표시되는 블록의 위치를 로컬 변수에 저장한다.
        //########################################################

        Rect temp_top_rect = new Rect();
        temp_top_rect.x = _last_code_rect_code_item.x;
        temp_top_rect.y = _last_code_rect_code_item.y;
        temp_top_rect.width = _last_code_rect_code_item.width;
        temp_top_rect.height = _last_code_rect_code_item.height;


        //########################################################
        // 먼저 실제 명령어가 표시되는 블록을 그려준다.
        //########################################################
        if (code_item != null)
        {
            Draw_CodeItemLine(code_item, level_num, temp_top_rect);

        }


        //########################################################
        // 자식이 있으면 자식을 그려준다.
        //########################################################
        if (code_item.BlockStartItem != null && code_item.BlockStartItem.NextCmdItem != null)
        {
            Draw_CodeList(code_item.BlockStartItem.NextCmdItem, level_num + 1);
        }


        //########################################################
        // BLOCK_END
        // end block을 그려준다.
        //########################################################        
        if (code_item.BlockEndItem != null)
        {
            //end block 그릴 때 옆쪽 수직 바를 같이 그려준다.
            //맨 위에서 부모 명령어 그릴 때 부모의 위치를 저장해 놓았기 때문에
            //맨 아래 그릴 때 높이를 계산할 수 있다.
            Draw_CodeItemLine(code_item.BlockEndItem, level_num, temp_top_rect);

            _TOUCH_UP_FLAG = false;
        }


        //########################################################
        // Next
        // 그 다음 명령어가 있으면 그 다음 명령어를 그려준다.
        //########################################################
        if (code_item.NextCmdItem != null)
        {
            Draw_CodeList(code_item.NextCmdItem, level_num);
        }


        #endregion Draw_CodeList
    }


    private void Draw_CodeItemLine(Code_Item code_item, int level_num, Rect vertical_top_rect)
    {
        #region Draw_CodeItemLine


        //버튼 위치 및 크기 계산
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        _dumy_rect_code_item.y = _last_code_rect_code_item.y;
        _dumy_rect_code_item.x = (code_item.IndentationLevel * 40 * _w_ratio) + (10 * _w_ratio) + (21 * _w_ratio * level_num);
        _dumy_rect_code_item.width = code_item.Width;
        _dumy_rect_code_item.height = 40 * _h_ratio;
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        if (code_item.CmdType == "BLOCK_END")
        {
            //블록 아래쪽은 좁게 그려준다.
            _dumy_rect_code_item.height = 20 * _h_ratio;
        }


        //삭제 아이콘 위치 계산
        _block_remove_icon_rect.x = _dumy_rect_code_item.x + _dumy_rect_code_item.width - (28 * _s_ratio);
        _block_remove_icon_rect.y = _dumy_rect_code_item.y + (14 * _h_ratio);

        //선택 여부 아이콘 위치 계산
        _block_selected_icon_rect.x = _dumy_rect_code_item.x + _dumy_rect_code_item.width - (52 * _s_ratio);
        _block_selected_icon_rect.y = _dumy_rect_code_item.y + (12 * _h_ratio);


        int btn_style_ind = 0;

        if (code_item.CmdName == "Expression")
            btn_style_ind = 5;
        else if (code_item.CmdName == "Print" || code_item.CmdName == "PrintLine")
            btn_style_ind = 0;
        else if (code_item.CmdName == "Serial.print" || code_item.CmdName == "Serial.println")
            btn_style_ind = 0;
        else if (code_item.CmdName == "if" || code_item.CmdName == "else" || code_item.CmdName == "else if")
            btn_style_ind = 1;
        else if (code_item.CmdName == "for" || code_item.CmdName == "while")
            btn_style_ind = 1;
        else if (code_item.CmdType == "BLOCK_END" && code_item.ParentItem.CmdName == "if")
            btn_style_ind = 1;
        else if (code_item.CmdType == "BLOCK_END" && code_item.ParentItem.CmdName == "else")
            btn_style_ind = 1;
        else if (code_item.CmdType == "BLOCK_END" && code_item.ParentItem.CmdName == "else if")
            btn_style_ind = 1;
        else if (code_item.CmdType == "BLOCK_END" && code_item.ParentItem.CmdName == "for")
            btn_style_ind = 1;
        else if (code_item.CmdType == "BLOCK_END" && code_item.ParentItem.CmdName == "while")
            btn_style_ind = 1;
        else if (code_item.CmdName == "setup function" || code_item.CmdName == "Setup Function")
            btn_style_ind = 2;
        else if (code_item.CmdType == "BLOCK_END" && code_item.ParentItem.CmdName == "Setup Function")
            btn_style_ind = 2;
        else if (code_item.CmdType == "BLOCK_END" && code_item.ParentItem.CmdName == "setup function")
            btn_style_ind = 2;
        else if (code_item.CmdName == "loop function" || code_item.CmdName == "Loop Function")
            btn_style_ind = 2;
        else if (code_item.CmdType == "BLOCK_END" && code_item.ParentItem.CmdName == "loop function")
            btn_style_ind = 2;
        else if (code_item.CmdType == "BLOCK_END" && code_item.ParentItem.CmdName == "Loop Function")
            btn_style_ind = 2;
        else if (code_item.CmdName == "Function" || code_item.CmdName == "function")
            btn_style_ind = 2;
        else if (code_item.CmdType == "BLOCK_END" && code_item.ParentItem.CmdName == "function")
            btn_style_ind = 2;
        else if (code_item.CmdType == "BLOCK_END" && code_item.ParentItem.CmdName == "Function")
            btn_style_ind = 2;
        else if (code_item.CmdName == "digitalWrite" || code_item.CmdName == "DigitalWrite")
            btn_style_ind = 3;
        else if (code_item.CmdName == "digitalRead" || code_item.CmdName == "DigitalRead")
            btn_style_ind = 3;
        else if (code_item.CmdName == "analogRead" || code_item.CmdName == "AnalogRead")
            btn_style_ind = 3;
        else if (code_item.CmdName == "analogWrite" || code_item.CmdName == "AnalogWrite")
            btn_style_ind = 3;
        else if (code_item.CmdName == "driveWrite" || code_item.CmdName == "DriveWrite")
            btn_style_ind = 4;
        else if (code_item.CmdName == "servoWrite" || code_item.CmdName == "ServoWrite")
            btn_style_ind = 4;
        else if (code_item.CmdName == "tone" || code_item.CmdName == "Tone")
            btn_style_ind = 4;
        else if (code_item.CmdName == "map" || code_item.CmdName == "Map")
            btn_style_ind = 4;
        else if (code_item.CmdName == "Delay" || code_item.CmdName == "delay")
            btn_style_ind = 3;


        //명령어 표시할 때의 백그라운드 색상 인덱스
        code_item.Bg_Color_Index = btn_style_ind;

        GUIStyle btn_style = _left_btn_color_styles[btn_style_ind];

        bool selected_yn = false;

        if (_code_btn_selected_item != null && code_item.Id == _code_btn_selected_item.Id)
        {
            selected_yn = true;
        }


        //####################################################################
        //드래깅 타겟 영역 위치 테스트
        //####################################################################
        _code_btn_dragging_rect.height = 8 * _h_ratio;
        _code_btn_dragging_rect.width = _dumy_rect_code_item.width - (40 * _w_ratio);
        _code_btn_dragging_rect.x = _dumy_rect_code_item.x + (20 * _w_ratio);
        _code_btn_dragging_rect.y = _dumy_rect_code_item.y + (2 * _h_ratio);

        string local_drag_taget_up_down = string.Empty;
        //####################################################################



        #region Button Command Display


        //#######################################################################################################
        //#######################################################################################################


        if (code_item.CmdType == "BLOCK_END")
        {
            //실제 블록의 아래쪽은 여기서 그린다.

            #region BLOCK_END

            //#########################################################
            //블록 명령어일 경우 왼쪽에 막대를 표시한다.
            //#########################################################

            Rect vertical_bar_rect = new Rect();
            vertical_bar_rect.x = _dumy_rect_code_item.x;

            //아래 쪽으로 5픽셀 내려준다. 겹치지 안도록.
            vertical_bar_rect.y = vertical_top_rect.y + (5 * _s_ratio);

            vertical_bar_rect.width = 15 * _h_ratio;
            vertical_bar_rect.height = _dumy_rect_code_item.y - vertical_top_rect.y;

            GUI.DrawTexture(vertical_bar_rect, _COLOR_BTN_TEXTURES[btn_style_ind]);

            #endregion BLOCK_END
        }


        if (_remove_icon_clicked)
        {
            //삭제 다이얼로그 창에서는 Lable로 그려준다.
            GUI.Label(_dumy_rect_code_item, GetCmdLineText(code_item), btn_style);

        }
        else
        {
            if (GUI.Button(_dumy_rect_code_item, GetCmdLineText(code_item), btn_style))
            {
                if (_IS_DRAGGING_MODE)
                    _IS_DRAGGING_MODE = false;
                else
                {
                    if (_remove_icon_clicked == false)
                    {
                        _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);
                        _code_btn_selected_item = code_item;

                        ShowOptionList();


                        //####################################################
                        // 삭제 아이콘 클릭 여부를 체크함
                        //####################################################
                        _remove_contains_rect_down.x = _rect_center_code_scroll_view.x + _dumy_rect_code_item.x + _scroll_pos_center_code.x + _dumy_rect_code_item.width - (28 * _s_ratio);
                        _remove_contains_rect_down.y = _rect_center_code_scroll_view.y + _dumy_rect_code_item.y - _scroll_pos_center_code.y + (14 * _h_ratio);




                        //삭제 아이콘 클릭 여부를 체크한다
                        if (string.IsNullOrEmpty(_dragging_source_id) && _remove_icon_clicked == false && _remove_contains_rect_down.Contains(_FIRST_POS))
                        {

                            _remove_icon_clicked = true;
                            _remove_clicked_id = code_item.Id;

                            //마우스 버튼 클릭 이벤트를 소진시키기 위해 dumy를 5번 정도 gui 함수가 그냥 돌도록 함
                            //다이얼로그 창에서 저절로 버튼 클릭이 되는 현상을 방지하기 위함임
                            _remove_clicked_skip_count = 5;
                        }
                    }
                }
            }
        }





        //####################################################
        //명령어 드래깅 상태를 체크함
        //####################################################
        if (_block_cmd_dragging_mode && _remove_icon_clicked == false)
        {
            _dragged_contains_rect.x = _rect_center_code_scroll_view.x + _dumy_rect_code_item.x + _scroll_pos_center_code.x;
            _dragged_contains_rect.y = _rect_center_code_scroll_view.y + _dumy_rect_code_item.y - _scroll_pos_center_code.y;
            _dragged_contains_rect.width = _dumy_rect_code_item.width;

            _dragged_contains_rect_up.x = _rect_center_code_scroll_view.x + _dumy_rect_code_item.x + _scroll_pos_center_code.x;
            _dragged_contains_rect_up.y = _rect_center_code_scroll_view.y + _dumy_rect_code_item.y - _scroll_pos_center_code.y;
            _dragged_contains_rect_up.width = _dumy_rect_code_item.width;
            _dragged_contains_rect_up.height = _dumy_rect_code_item.height;

            _dragged_contains_rect_down.x = _rect_center_code_scroll_view.x + _dumy_rect_code_item.x + _scroll_pos_center_code.x;
            _dragged_contains_rect_down.y = _rect_center_code_scroll_view.y + _dumy_rect_code_item.y - _scroll_pos_center_code.y + (20 * _h_ratio);
            _dragged_contains_rect_down.height = _dumy_rect_code_item.height;


            if (string.IsNullOrEmpty(_dragging_source_id) && _dragged_contains_rect.Contains(_FIRST_POS) && code_item.CmdType != "BLOCK_END")
            {
                //맨 처음 마우스를 클릭한 위치의 명령어를 찾는다.
                //이곳에서 명령어를 이동하거나 복제할 원본 대상을 저장한다.
                _dragging_source_id = code_item.Id;
                _dragged_cmd_item_type = code_item.CmdType;
                _dragged_item_name = GetCmdLineText(code_item);
            }
            else if ((!string.IsNullOrEmpty(_dragging_source_id) || !string.IsNullOrEmpty(_dragged_item_name)) && _dragged_contains_rect_up.Contains(_CUR_MOUSE_POS))
            {
                local_drag_taget_up_down = "UP";
                _drag_taget_up_down = local_drag_taget_up_down;
                _dragging_target_id = code_item.Id;
            }
            else if ((!string.IsNullOrEmpty(_dragging_source_id) || !string.IsNullOrEmpty(_dragged_item_name)) && _dragged_contains_rect_down.Contains(_CUR_MOUSE_POS))
            {
                local_drag_taget_up_down = "DOWN";
                _drag_taget_up_down = local_drag_taget_up_down;
                _dragging_target_id = code_item.Id;
                _code_btn_dragging_rect.y = _dumy_rect_code_item.y + _dumy_rect_code_item.height - (10 * _h_ratio);
            }
        }
        //####################################################


        if (code_item.CmdType != "BLOCK_END")
        {
            //명령어 삭제 아이콘을 그려준다.
            GUI.DrawTexture(_block_remove_icon_rect, _ICON_TEXTURES[0]);
            //GUI.Button(_block_remove_icon_rect, _ICON_TEXTURES[0]);
            //if (GUI.Button(_block_remove_icon_rect, "", _btn_styles[7]))
            //{
            //    _remove_icon_clicked = true;
            //    _remove_clicked_id = code_item.Id;
            //    Draw_Delete();
            //}

            //현재 명령어가 선택되었는 지 선택 여부 아이콘 표시
            if (selected_yn)
                GUI.DrawTexture(_block_selected_icon_rect, _ICON_TEXTURES[1]);
        }


        //##################################################################################
        //드래킹 타겟 영역을 반투명 검정 바로 표시한다.
        //##################################################################################
        if (!string.IsNullOrEmpty(local_drag_taget_up_down))
        {
            GUI.Label(_code_btn_dragging_rect, "", _selected_white_bar_style);
        }
        //##################################################################################


        //마지막에 그린 명령어의 위치 값을 저장한다.
        _last_code_rect_code_item.x = _dumy_rect_code_item.x;
        _last_code_rect_code_item.y = _dumy_rect_code_item.y + _dumy_rect_code_item.height + (4 * _h_ratio);
        _last_code_rect_code_item.width = _dumy_rect_code_item.width;
        _last_code_rect_code_item.height = _dumy_rect_code_item.height;



        if (code_item.CmdType == "BLOCK_END" && level_num == 0)
        {
            //일반 함수 영역의 경우 아래쪽에 빈 공간을 20피셀 정도 추가해 준다.
            //그 다음 명령어의 시작 위치가 20 픽셀 아래가 되도록 한다.
            _last_code_rect_code_item.y = _last_code_rect_code_item.y + 20 * _h_ratio;
        }


        if (code_item.BlockYN)
        {
            //자식이 없으면 기본 공백 라인을 추가해 준다.

            if (code_item.BlockStartItem == null || code_item.BlockStartItem.NextCmdItem == null)
            {
                _last_code_rect_code_item.y = _last_code_rect_code_item.y + 20 * _h_ratio;
            }
        }



        //#######################################################################################################
        //#######################################################################################################


        #endregion Button Command Display


        #endregion Draw_CodeItemLine
    }


    //##########################################################################################################################



    //###############################################################
    //오른쪽 옵션 리스트 내용을 그려준다.
    //###############################################################

    void ShowOptionList()
    {

    }


    void Draw_RightPins()
    {
        GUI.Box(_right_pin_region_rects, "", _gui_box_white_bg_styles);

        string ConnectedPinsString = "Connected Pins";
        if (Application.systemLanguage == SystemLanguage.Korean)
        {
            ConnectedPinsString = "연결된 핀";
        }
        //Group Title
        GUI.Label(_right_pin_title_rects, ConnectedPinsString, _text_styles[3]);


        //높이를 다시 계산
        _right_pin_scroll_content_rect.height = _right_pin_list.Count * (50 * _h_ratio) + (60 * _h_ratio);

        _scroll_pos_right_pin = GUI.BeginScrollView(_right_pin_scroll_view_rect, _scroll_pos_right_pin, _right_pin_scroll_content_rect, false, true);

        for (int i = 0; i < _right_pin_list.Count; i++)
        {
            _right_icon_rect.y = (10 + (i * 45)) * _h_ratio;
            _right_btn_rect.y = (1 + (i * 45)) * _h_ratio;

            //_right_btn_rect.x = 60 * _w_ratio;
            _right_btn_rect.x = 40 * _w_ratio;


            string pin_name = _right_pin_list[i];

            int icon_ind = 4;

            if (pin_name.Contains("Brightness"))
                icon_ind = 4;
            else if (pin_name.Contains("Button"))
                icon_ind = 5;
            else if (pin_name.Contains("Distance"))
                icon_ind = 6;
            else if (pin_name.Contains("IR"))
                icon_ind = 7;
            else if (pin_name.Contains("LED Green"))
                icon_ind = 8;
            else if (pin_name.Contains("LED Red"))
                icon_ind = 9;
            else if (pin_name.Contains("LED Yellow"))
                icon_ind = 10;
            else if (pin_name.Contains("Light"))
                icon_ind = 11;
            else if (pin_name.Contains("Speaker"))
                icon_ind = 12;
            else if (pin_name.Contains("Joystick X"))
                icon_ind = 13;
            else if (pin_name.Contains("Joystick Y"))
                icon_ind = 14;
            else if (pin_name.Contains("Motor"))
                icon_ind = 15;


            GUI.DrawTexture(_right_icon_rect, _ICON_TEXTURES[icon_ind]);

            if (GUI.Button(_right_btn_rect, pin_name, _text_styles[5]))
            {
                if (_IS_DRAGGING_MODE)
                    _IS_DRAGGING_MODE = false;
                else
                {
                    _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);
                }
            }
        }

        GUI.EndScrollView();


    }



    void Draw_RightProperty(string cur_item_name, string disp_item_name)
    {
        GUI.Box(_right_property_region_rects, "", _gui_box_white_bg_styles);

        string PropertiesString = "Properties";
        if (Application.systemLanguage == SystemLanguage.Korean)
        {
            PropertiesString = "속성";
        }
        //Group Title
        GUI.Label(_right_property_title_rects, PropertiesString, _text_styles[3]);


        if (_code_btn_selected_item != null && cur_item_name != "{" && cur_item_name != "}")
        {
            cur_item_name = GetCurSelectedItemName();
            disp_item_name = cur_item_name;


            //속성 이름. 폰트 색상은 10번째 부터 시작
            GUI.Label(propNameRect, disp_item_name, _text_styles[_code_btn_selected_item.Bg_Color_Index + 10]);



            //#################################################################################################
            //변수 표시
            //#################################################################################################
            if (_code_btn_selected_item != null && _code_btn_selected_item.VariableYN)
            {
                string cur_variable_name = GetCurSelectedItemVariable();

                //변수 제목
                GUI.Label(propVXNameRect, "Variable Name", _text_styles[7]);

                //변수 이름
                propInputVariable = GUI.TextField(propVariableRect, cur_variable_name, _GuiStyle_font_text_field);
                SetCurSelectedItemVariable(propInputVariable);
            }
            //#################################################################################################




            if (!string.IsNullOrEmpty(cur_item_name))
            {
                string cur_value_name = GetCurSelectedItemValue();

                GUIStyle btn_style = _prop_btn_color_styles[_code_btn_selected_item.Bg_Color_Index];

                if (cur_item_name == "setup function" || cur_item_name == "loop function" || cur_item_name == "else")
                {
                    //Do nothing
                }
                else if (IsNeededValueChangeButton(cur_item_name))
                {

                    #region 값 분리

                    string[] val_arr = cur_value_name.Split(new char[] { ' ', ',', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);

                    if (val_arr.Length == 1)
                    {
                        propInputText_X = val_arr[0];
                        propInputText_Y = "0";
                        propInputText_Z = "0";
                        propInputText_A = "0";
                    }
                    else if (val_arr.Length == 2)
                    {
                        propInputText_X = val_arr[0];
                        propInputText_Y = val_arr[1];
                        propInputText_Z = "0";
                        propInputText_A = "0";
                    }
                    else if (val_arr.Length == 3)
                    {
                        propInputText_X = val_arr[0];
                        propInputText_Y = val_arr[1];
                        propInputText_Z = val_arr[2];
                        propInputText_A = "0";
                    }
                    else if (val_arr.Length == 4)
                    {
                        propInputText_X = val_arr[0];
                        propInputText_Y = val_arr[1];
                        propInputText_Z = val_arr[2];
                        propInputText_A = val_arr[3];
                    }

                    #endregion 값 분리


                    //###############################################################################
                    //첫번째 값 표시
                    //###############################################################################

                    #region 첫번째 값 표시


                    if (cur_item_name == "digitalWrite" || cur_item_name == "analogWrite" || cur_item_name == "tone")
                        GUI.Label(propVXNameRect, "Pin", _text_styles[7]);
                    else if (cur_item_name == "driveWrite")
                        GUI.Label(propVXNameRect, "Left", _text_styles[7]);
                    else if (cur_item_name == "servoWrite")
                        GUI.Label(propVXNameRect, "Pin", _text_styles[7]);
                    else if (cur_item_name == "delay")
                        GUI.Label(propVXNameRect, "ms", _text_styles[7]);


                    //Button -
                    propDumyRect.x = propVXBtn1Rect.x;
                    propDumyRect.y = propVXBtn1Rect.y;
                    propDumyRect.width = propVXBtn1Rect.width;
                    propDumyRect.height = propVXBtn1Rect.height;


                    if (GUI.Button(propDumyRect, "-", btn_style) && !string.IsNullOrEmpty(propInputText_X))
                    {
                        Do_Property_Value_Change_V1_Minus(cur_item_name);
                    }
                    else if (Input.GetMouseButton(0) && propDumyRect.Contains(GetReversePosV2(Input.mousePosition)) && !string.IsNullOrEmpty(propInputText_X))
                    {
                        VXBtn1_Pressed_time = VXBtn1_Pressed_time + Time.deltaTime;

                        if (VXBtn1_Pressed_time > 0.5f)
                        {
                            Do_Property_Value_Change_V1_Minus(cur_item_name);
                        }
                    }
                    else
                        VXBtn1_Pressed_time = 0;



                    //Button +
                    propDumyRect.x = propVXBtn2Rect.x;
                    propDumyRect.y = propVXBtn2Rect.y;
                    propDumyRect.width = propVXBtn2Rect.width;
                    propDumyRect.height = propVXBtn2Rect.height;


                    if (GUI.Button(propDumyRect, "+", btn_style) && !string.IsNullOrEmpty(propInputText_X))
                    {
                        Do_Property_Value_Change_V1_Plus(cur_item_name);
                    }
                    else if (Input.GetMouseButton(0) && propDumyRect.Contains(GetReversePosV2(Input.mousePosition)) && !string.IsNullOrEmpty(propInputText_X))
                    {
                        VXBtn2_Pressed_time = VXBtn2_Pressed_time + Time.deltaTime;

                        if (VXBtn2_Pressed_time > 0.5f)
                        {
                            Do_Property_Value_Change_V1_Plus(cur_item_name);
                        }
                    }
                    else
                        VXBtn2_Pressed_time = 0;



                    //TextField
                    propDumyRect.x = propVXValueRect.x;
                    propDumyRect.y = propVXValueRect.y;
                    propDumyRect.width = propVXValueRect.width;
                    propDumyRect.height = propVXValueRect.height;


                    propInputText_X = GUI.TextField(propDumyRect, propInputText_X, _GuiStyle_font_text_field_center);


                    //값을 지우는 버튼
                    if (GUI.Button(propVXRemoveRect, "x", _prop_btn_color_styles[6]))
                    {
                        propInputText_X = string.Empty;
                    }


                    #endregion 첫번째 값 표시


                    //###############################################################################
                    //두번째 값 표시
                    //###############################################################################

                    if (cur_item_name == "delay")
                    {
                        //do nothing
                    }
                    else
                    {

                        #region 두번째 값 표시

                        //Y
                        propDumyRect.x = propVXNameRect.x;
                        propDumyRect.y = propVXNameRect.y + (45 * _h_ratio);
                        propDumyRect.width = propVXNameRect.width;
                        propDumyRect.height = propVXNameRect.height;


                        if (cur_item_name == "digitalWrite" || cur_item_name == "analogWrite")
                            GUI.Label(propDumyRect, "Val", _text_styles[7]);
                        else if (cur_item_name == "driveWrite")
                            GUI.Label(propDumyRect, "Right", _text_styles[7]);
                        else if (cur_item_name == "servoWrite")
                            GUI.Label(propDumyRect, "Deg", _text_styles[7]);
                        else if (cur_item_name == "tone")
                            GUI.Label(propDumyRect, "Freq", _text_styles[7]);


                        propDumyRect.x = propVXBtn1Rect.x;
                        propDumyRect.y = propVXBtn1Rect.y + (45 * _h_ratio);
                        propDumyRect.width = propVXBtn1Rect.width;
                        propDumyRect.height = propVXBtn1Rect.height;


                        if (GUI.Button(propDumyRect, "-", btn_style) && !string.IsNullOrEmpty(propInputText_Y))
                        {
                            Do_Property_Value_Change_V2_Minus(cur_item_name);
                        }
                        else if (Input.GetMouseButton(0) && propDumyRect.Contains(GetReversePosV2(Input.mousePosition)) && !string.IsNullOrEmpty(propInputText_Y))
                        {
                            VYBtn1_Pressed_time = VYBtn1_Pressed_time + Time.deltaTime;

                            if (VYBtn1_Pressed_time > 0.5f)
                            {
                                Do_Property_Value_Change_V2_Minus(cur_item_name);
                            }
                        }
                        else
                            VYBtn1_Pressed_time = 0;


                        propDumyRect.x = propVXBtn2Rect.x;
                        propDumyRect.y = propVXBtn2Rect.y + (45 * _h_ratio);
                        propDumyRect.width = propVXBtn2Rect.width;
                        propDumyRect.height = propVXBtn2Rect.height;



                        if (GUI.Button(propDumyRect, "+", btn_style) && !string.IsNullOrEmpty(propInputText_Y))
                        {
                            Do_Property_Value_Change_V2_Plus(cur_item_name);
                        }
                        else if (Input.GetMouseButton(0) && propDumyRect.Contains(GetReversePosV2(Input.mousePosition)) && !string.IsNullOrEmpty(propInputText_Y))
                        {
                            VYBtn2_Pressed_time = VYBtn2_Pressed_time + Time.deltaTime;

                            if (VYBtn2_Pressed_time > 0.5f)
                            {
                                Do_Property_Value_Change_V2_Plus(cur_item_name);
                            }
                        }
                        else
                            VYBtn2_Pressed_time = 0;


                        propDumyRect.x = propVXValueRect.x;
                        propDumyRect.y = propVXValueRect.y + (45 * _h_ratio);
                        propDumyRect.width = propVXValueRect.width;
                        propDumyRect.height = propVXValueRect.height;



                        propInputText_Y = GUI.TextField(propDumyRect, propInputText_Y, _GuiStyle_font_text_field_center);



                        //값을 지우는 버튼
                        propDumyRect.x = propVXRemoveRect.x;
                        propDumyRect.y = propVXRemoveRect.y + (45 * _h_ratio);
                        propDumyRect.width = propVXRemoveRect.width;
                        propDumyRect.height = propVXRemoveRect.height;

                        if (GUI.Button(propDumyRect, "x", _prop_btn_color_styles[6]))
                        {
                            propInputText_Y = string.Empty;
                        }

                        #endregion 두번째 값 표시

                    }

                    //###############################################################################
                    //세번째 값 표시
                    //###############################################################################

                    if (cur_item_name == "delay" || cur_item_name == "digitalWrite" || cur_item_name == "analogWrite"
                        || cur_item_name == "driveWrite" || cur_item_name == "servoWrite")
                    {
                        //do nothing
                    }
                    else
                    {

                        #region 세번째 값 표시

                        //Z
                        propDumyRect.x = propVXNameRect.x;
                        propDumyRect.y = propVXNameRect.y + (90 * _h_ratio);
                        propDumyRect.width = propVXNameRect.width;
                        propDumyRect.height = propVXNameRect.height;


                        if (cur_item_name == "tone")
                            GUI.Label(propDumyRect, "Dur", _text_styles[7]);


                        propDumyRect.x = propVXBtn1Rect.x;
                        propDumyRect.y = propVXBtn1Rect.y + (90 * _h_ratio);
                        propDumyRect.width = propVXBtn1Rect.width;
                        propDumyRect.height = propVXBtn1Rect.height;



                        if (GUI.Button(propDumyRect, "-", btn_style) && !string.IsNullOrEmpty(propInputText_Z))
                        {
                            Do_Property_Value_Change_V3_Minus(cur_item_name);
                        }
                        else if (Input.GetMouseButton(0) && propDumyRect.Contains(GetReversePosV2(Input.mousePosition)) && !string.IsNullOrEmpty(propInputText_Z))
                        {
                            VZBtn1_Pressed_time = VZBtn1_Pressed_time + Time.deltaTime;

                            if (VZBtn1_Pressed_time > 0.5f)
                            {
                                Do_Property_Value_Change_V3_Minus(cur_item_name);
                            }
                        }
                        else
                            VZBtn1_Pressed_time = 0;

                        propDumyRect.x = propVXBtn2Rect.x;
                        propDumyRect.y = propVXBtn2Rect.y + (90 * _h_ratio);
                        propDumyRect.width = propVXBtn2Rect.width;
                        propDumyRect.height = propVXBtn2Rect.height;


                        if (GUI.Button(propDumyRect, "+", btn_style) && !string.IsNullOrEmpty(propInputText_Z))
                        {
                            Do_Property_Value_Change_V3_Plus(cur_item_name);
                        }
                        else if (Input.GetMouseButton(0) && propDumyRect.Contains(GetReversePosV2(Input.mousePosition)) && !string.IsNullOrEmpty(propInputText_Z))
                        {
                            VZBtn2_Pressed_time = VZBtn2_Pressed_time + Time.deltaTime;

                            if (VZBtn2_Pressed_time > 0.5f)
                            {
                                Do_Property_Value_Change_V3_Plus(cur_item_name);
                            }
                        }
                        else
                            VZBtn2_Pressed_time = 0;

                        propDumyRect.x = propVXValueRect.x;
                        propDumyRect.y = propVXValueRect.y + (90 * _h_ratio);
                        propDumyRect.width = propVXValueRect.width;
                        propDumyRect.height = propVXValueRect.height;


                        propInputText_Z = GUI.TextField(propDumyRect, propInputText_Z, _GuiStyle_font_text_field_center);


                        //값을 지우는 버튼

                        propDumyRect.x = propVXRemoveRect.x;
                        propDumyRect.y = propVXRemoveRect.y + (90 * _h_ratio);
                        propDumyRect.width = propVXRemoveRect.width;
                        propDumyRect.height = propVXRemoveRect.height;

                        if (GUI.Button(propDumyRect, "x", _prop_btn_color_styles[6]))
                        {
                            propInputText_Z = string.Empty;
                        }

                        #endregion 세번째 값 표시

                    }

                    //###############################################################################
                    //값 반영시키기
                    //###############################################################################


                    if (cur_item_name == "delay")
                    {
                        if (string.IsNullOrEmpty(propInputText_X))
                            propInputText_X = "1000";

                        SetCurSelectedItemValue(propInputText_X);
                    }
                    else if (cur_item_name == "digitalWrite")
                    {
                        if (string.IsNullOrEmpty(propInputText_X))
                            propInputText_X = "2";

                        if (string.IsNullOrEmpty(propInputText_Y))
                            propInputText_Y = "HIGH";

                        SetCurSelectedItemValue(propInputText_X + ", " + propInputText_Y);
                    }
                    else if (cur_item_name == "analogWrite")
                    {
                        if (string.IsNullOrEmpty(propInputText_X))
                            propInputText_X = "11";

                        if (string.IsNullOrEmpty(propInputText_Y))
                            propInputText_Y = "0";

                        SetCurSelectedItemValue(propInputText_X + ", " + propInputText_Y);
                    }
                    else if (cur_item_name == "driveWrite")
                    {
                        if (string.IsNullOrEmpty(propInputText_X))
                            propInputText_X = "0";

                        if (string.IsNullOrEmpty(propInputText_Y))
                            propInputText_Y = "0";

                        SetCurSelectedItemValue(propInputText_X + ", " + propInputText_Y);
                    }
                    else if (cur_item_name == "servoWrite")
                    {
                        if (string.IsNullOrEmpty(propInputText_X))
                            propInputText_X = "2";

                        if (string.IsNullOrEmpty(propInputText_Y))
                            propInputText_Y = "0";

                        SetCurSelectedItemValue(propInputText_X + ", " + propInputText_Y);
                    }
                    else if (cur_item_name == "tone")
                    {
                        if (string.IsNullOrEmpty(propInputText_X))
                            propInputText_X = "2";

                        if (string.IsNullOrEmpty(propInputText_Y))
                            propInputText_Y = "0";

                        if (string.IsNullOrEmpty(propInputText_Z))
                            propInputText_Z = "100";

                        SetCurSelectedItemValue(propInputText_X + ", " + propInputText_Y + ", " + propInputText_Z);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(propInputText_Y) && string.IsNullOrEmpty(propInputText_Z))
                            SetCurSelectedItemValue(propInputText_X);
                        else if (string.IsNullOrEmpty(propInputText_Z))
                            SetCurSelectedItemValue(propInputText_X + "  " + propInputText_Y);
                        else
                            SetCurSelectedItemValue(propInputText_X + "  " + propInputText_Y + "  " + propInputText_Z);
                    }


                }
                else if (cur_item_name == "for")
                {

                    #region for

                    string ex1 = propInputText;
                    string ex2 = propInputText;
                    string ex3 = propInputText;

                    if (!string.IsNullOrEmpty(cur_value_name))
                    {
                        string[] val_arr = cur_value_name.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries);

                        if (val_arr != null && val_arr.Length == 3)
                        {
                            ex1 = val_arr[0].Trim();
                            ex2 = val_arr[1].Trim();
                            ex3 = val_arr[2].Trim();


                            _Dumy_Rect.width = propValueRect.width;
                            _Dumy_Rect.height = propValueRect.height;
                            _Dumy_Rect.x = propValueRect.x;
                            _Dumy_Rect.y = propVXNameRect.y + 15 * _h_ratio;


                            ex1 = GUI.TextField(_Dumy_Rect, ex1, _GuiStyle_font_text_field);

                            _Dumy_Rect.width = 40 * _w_ratio;
                            _Dumy_Rect.height = propValueRect.height;
                            _Dumy_Rect.x = _Dumy_Rect.x + propValueRect.width + 15 * _w_ratio;
                            _Dumy_Rect.y = _Dumy_Rect.y;


                            if (GUI.Button(_Dumy_Rect, "x", _prop_btn_color_styles[6]))
                                ex1 = string.Empty;

                            //###########################################################################
                            _Dumy_Rect.width = propValueRect.width;
                            _Dumy_Rect.x = propValueRect.x;
                            _Dumy_Rect.y = _Dumy_Rect.y + propValueRect.height + 15 * _h_ratio;

                            ex2 = GUI.TextField(_Dumy_Rect, ex2, _GuiStyle_font_text_field);

                            _Dumy_Rect.width = 40 * _w_ratio;
                            _Dumy_Rect.x = propValueRect.x + propValueRect.width + 15 * _w_ratio;

                            if (GUI.Button(_Dumy_Rect, "x", _prop_btn_color_styles[6]))
                                ex2 = string.Empty;


                            //###########################################################################
                            _Dumy_Rect.width = propValueRect.width;
                            _Dumy_Rect.x = propValueRect.x;
                            _Dumy_Rect.y = _Dumy_Rect.y + propValueRect.height + 15 * _h_ratio;

                            ex3 = GUI.TextField(_Dumy_Rect, ex3, _GuiStyle_font_text_field);

                            _Dumy_Rect.width = 40 * _w_ratio;
                            _Dumy_Rect.x = propValueRect.x + propValueRect.width + 15 * _w_ratio;

                            if (GUI.Button(_Dumy_Rect, "x", _prop_btn_color_styles[6]))
                                ex3 = string.Empty;


                            SetCurSelectedItemValue(ex1 + "; " + ex2 + "; " + ex3);
                        }
                    }

                    #endregion for

                }
                else
                {

                    if (cur_item_name == "digitalRead" || cur_item_name == "analogRead")
                    {
                        #region DigitalRead

                        //아래로 일정한 높이 만큼 내려 준다.
                        //Y 값들이 다른 항목들과 차이가 있다.
                        float offset_h = 100 * _h_ratio;

                        propDumyRect.x = propVXNameRect.x;
                        propDumyRect.y = propVXNameRect.y + offset_h;
                        propDumyRect.width = propVXNameRect.width;
                        propDumyRect.height = propVXNameRect.height;


                        GUI.Label(propDumyRect, "Pin", _text_styles[7]);


                        //#####################################################################################


                        //Button -
                        propDumyRect.x = propVXBtn1Rect.x;
                        propDumyRect.y = propVXBtn1Rect.y + offset_h;
                        propDumyRect.width = propVXBtn1Rect.width;
                        propDumyRect.height = propVXBtn1Rect.height;


                        propInputText_X = cur_value_name;


                        if (GUI.Button(propDumyRect, "-", btn_style) && !string.IsNullOrEmpty(propInputText_X))
                        {
                            float v = float.Parse(propInputText_X);

                            v = v - 1;
                            if (v < 0)
                                v = 0;

                            propInputText_X = v.ToString();
                        }
                        else if (Input.GetMouseButton(0) && propDumyRect.Contains(GetReversePosV2(Input.mousePosition)) && !string.IsNullOrEmpty(propInputText_X))
                        {
                            VXBtn1_Pressed_time = VXBtn1_Pressed_time + Time.deltaTime;

                            if (VXBtn1_Pressed_time > 0.5f)
                            {
                                float v = float.Parse(propInputText_X);

                                v = v - 1;
                                if (v < 0)
                                    v = 0;

                                propInputText_X = v.ToString();
                            }
                        }
                        else
                            VXBtn1_Pressed_time = 0;



                        //Button +
                        propDumyRect.x = propVXBtn2Rect.x;
                        propDumyRect.y = propVXBtn2Rect.y + offset_h;
                        propDumyRect.width = propVXBtn2Rect.width;
                        propDumyRect.height = propVXBtn2Rect.height;



                        if (GUI.Button(propDumyRect, "+", btn_style) && !string.IsNullOrEmpty(propInputText_X))
                        {
                            float v = float.Parse(propInputText_X);

                            v = v + 1;
                            if (v > 13)
                                v = 13;

                            propInputText_X = v.ToString();

                        }
                        else if (Input.GetMouseButton(0) && propDumyRect.Contains(GetReversePosV2(Input.mousePosition)) && !string.IsNullOrEmpty(propInputText_X))
                        {
                            VXBtn2_Pressed_time = VXBtn2_Pressed_time + Time.deltaTime;

                            if (VXBtn2_Pressed_time > 0.5f)
                            {
                                float v = float.Parse(propInputText_X);

                                v = v + 1;
                                if (v > 13)
                                    v = 13;

                                propInputText_X = v.ToString();
                            }
                        }
                        else
                            VXBtn2_Pressed_time = 0;



                        //TextField
                        propDumyRect.x = propVXValueRect.x;
                        propDumyRect.y = propVXValueRect.y + offset_h;
                        propDumyRect.width = propVXValueRect.width;
                        propDumyRect.height = propVXValueRect.height;

                        propInputText_X = GUI.TextField(propDumyRect, propInputText_X, _GuiStyle_font_text_field_center);

                        SetCurSelectedItemValue(propInputText_X);


                        #endregion DigitalRead

                        //#####################################################################################
                    }
                    else if (cur_item_name == "map")
                    {
                        //아래로 일정한 높이 만큼 내려 준다.
                        //Y 값들이 다른 항목들과 차이가 있다.
                        float offset_h = 100 * _h_ratio;


                        propDumyRect.x = propValueRect.x;
                        propDumyRect.y = propValueRect.y + offset_h;

                        //아래 크기 수정하였음
                        propDumyRect.width = propVariableRect.width;
                        propDumyRect.height = propVariableRect.height;


                        //기본적인 값 입력창을 표시한다.
                        propInputText = GUI.TextField(propDumyRect, cur_value_name, _GuiStyle_font_text_field);
                        SetCurSelectedItemValue(propInputText);


                    }
                    else
                    {
                        //기본적인 값 입력창을 표시한다.
                        propInputText = GUI.TextField(propValueRect, cur_value_name, _GuiStyle_font_text_field);
                        SetCurSelectedItemValue(propInputText);


                        //내용 삭제시키는 버튼 추가
                        _Dumy_Rect.width = 40 * _w_ratio;
                        _Dumy_Rect.height = propValueRect.height;
                        _Dumy_Rect.x = propValueRect.x + propValueRect.width + 15 * _w_ratio;
                        _Dumy_Rect.y = propValueRect.y;

                        if (GUI.Button(_Dumy_Rect, "x", _prop_btn_color_styles[6]))
                        {
                            propInputText = string.Empty;
                            SetCurSelectedItemValue(propInputText);
                        }
                    }
                }

            }
        }
    }



    void Do_Property_Value_Change_V1_Minus(string cur_item_name)
    {

        #region Do_Property_Value_Change_V1_Minus


        float v = float.Parse(propInputText_X);

        if (cur_item_name == "delay")
        {
            v = v - 100;
            if (v < 0)
                v = 0;
        }
        else if (cur_item_name == "digitalWrite" || cur_item_name == "analogWrite" || cur_item_name == "servoWrite" || cur_item_name == "tone")
        {
            v = v - 1;
            if (v < 0)
                v = 0;
        }
        else if (cur_item_name == "driveWrite")
        {
            v = v - 10;
            if (v < -250)
                v = -250;
        }
        else
            v = v - 0.5f;

        propInputText_X = v.ToString();

        #endregion Do_Property_Value_Change_V1_Minus
    }


    void Do_Property_Value_Change_V1_Plus(string cur_item_name)
    {

        #region Do_Property_Value_Change_V1_Plus

        float v = float.Parse(propInputText_X);

        if (cur_item_name == "delay")
        {
            v = v + 100;
        }
        else if (cur_item_name == "digitalWrite" || cur_item_name == "analogWrite" || cur_item_name == "servoWrite" || cur_item_name == "tone")
        {
            v = v + 1;
            if (v > 13)
                v = 13;
        }
        else if (cur_item_name == "driveWrite")
        {
            v = v + 10;
            if (v > 250)
                v = 250;
        }
        else
            v = v + 0.5f;

        propInputText_X = v.ToString();

        #endregion Do_Property_Value_Change_V1_Plus
    }


    void Do_Property_Value_Change_V2_Minus(string cur_item_name)
    {
        #region Do_Property_Value_Change_V2_Minus

        float v = 0;

        if (cur_item_name != "digitalWrite")
            v = float.Parse(propInputText_Y);

        if (cur_item_name == "digitalWrite")
        {
            if (propInputText_Y == "HIGH")
                propInputText_Y = "LOW";
            else if (propInputText_Y == "LOW")
                propInputText_Y = "HIGH";
            else if (propInputText_Y == "1")
                propInputText_Y = "0";
            else if (propInputText_Y == "0")
                propInputText_Y = "1";
        }
        else if (cur_item_name == "analogWrite")
        {
            v = v - 1;
            if (v < 0)
                v = 0;

            propInputText_Y = v.ToString();
        }
        else if (cur_item_name == "driveWrite")
        {
            v = v - 10;
            if (v < -250)
                v = -250;

            propInputText_Y = v.ToString();
        }
        else if (cur_item_name == "servoWrite")
        {
            v = v - 10;
            if (v < 0)
                v = 0;

            propInputText_Y = v.ToString();
        }
        else if (cur_item_name == "tone")
        {
            v = v - 1f;
            if (v < 0)
                v = 0;

            propInputText_Y = v.ToString();
        }
        else
        {
            v = v - 0.5f;

            propInputText_Y = v.ToString();
        }

        #endregion Do_Property_Value_Change_V2_Minus
    }


    void Do_Property_Value_Change_V2_Plus(string cur_item_name)
    {
        #region Do_Property_Value_Change_V2_Plus

        float v = 0;

        if (cur_item_name != "digitalWrite")
            v = float.Parse(propInputText_Y);

        if (cur_item_name == "digitalWrite")
        {
            if (propInputText_Y == "HIGH")
                propInputText_Y = "LOW";
            else if (propInputText_Y == "LOW")
                propInputText_Y = "HIGH";
            else if (propInputText_Y == "1")
                propInputText_Y = "0";
            else if (propInputText_Y == "0")
                propInputText_Y = "1";
        }
        else if (cur_item_name == "analogWrite")
        {
            v = v + 1;
            if (v > 255)
                v = 255;

            propInputText_Y = v.ToString();
        }
        else if (cur_item_name == "driveWrite")
        {
            v = v + 10;
            if (v > 250)
                v = 250;

            propInputText_Y = v.ToString();
        }
        else if (cur_item_name == "servoWrite")
        {
            v = v + 10;
            if (v > 180)
                v = 180;

            propInputText_Y = v.ToString();
        }
        else if (cur_item_name == "tone")
        {
            v = v + 1;

            propInputText_Y = v.ToString();
        }
        else
        {
            v = v + 0.5f;

            propInputText_Y = v.ToString();
        }

        #endregion Do_Property_Value_Change_V2_Plus
    }


    void Do_Property_Value_Change_V3_Minus(string cur_item_name)
    {
        #region Do_Property_Value_Change_V3_Minus

        float v = float.Parse(propInputText_Z);

        if (cur_item_name == "tone")
        {
            v = v - 1f;
            if (v < 0)
                v = 0;
        }
        else
            v = v - 0.5f;

        propInputText_Z = v.ToString();

        #endregion Do_Property_Value_Change_V3_Minus
    }


    void Do_Property_Value_Change_V3_Plus(string cur_item_name)
    {
        #region Do_Property_Value_Change_V3_Plus

        float v = float.Parse(propInputText_Z);

        if (cur_item_name == "tone")
        {
            v = v + 1f;
        }
        else
            v = v + 0.5f;

        propInputText_Z = v.ToString();

        #endregion Do_Property_Value_Change_V3_Plus
    }



    void Draw_RightButton()
    {
        string ExecuteString = "Execute";
        if (Application.systemLanguage == SystemLanguage.Korean)
        {
            _execute_btn_styles.font = _FONTS[1];
            ExecuteString = "실행";
        }
        //Execute Button
        if (GUI.Button(_right_button_region_rects, ExecuteString, _execute_btn_styles))
        {
            _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);


            string target_script = string.Empty;

            if (SPLCodeEditor._EDITOR_TYPE == "TEXT")
                target_script = _CURRENT_SCRIPT_TEXT;
            else
                target_script = ExportScriptAsText();


            //사용자 코드에 저장한다.
            string saved_target_dir = Application.persistentDataPath + "/" + GlobalVariables.SelectedCodingType;
            string saved_target_file_name = Application.persistentDataPath + "/" + GlobalVariables.SelectedCodingType + "/" + GlobalVariables.CurrentFileName + ".txt";

            if (!Directory.Exists(saved_target_dir))
            {
                Directory.CreateDirectory(saved_target_dir);
            }


            if (File.Exists(saved_target_file_name))
            {
                File.Delete(saved_target_file_name);
            }


            File.WriteAllText(saved_target_file_name, target_script);



            //스크립트 엔진에 넘겨줄 파일에 저장한다.

            string script_file_name = Application.persistentDataPath + "/ArduinoScript.txt";

            if (File.Exists(script_file_name))
            {
                File.Delete(script_file_name);
            }


            File.WriteAllText(script_file_name, target_script);

            //##########################################            
            _fadeScreenLibrary.RequestFadeOut("Execute", true);
            //##########################################      
        }

        GUI.DrawTexture(_right_button_icon_rects, _BTN_TEXTURES[6]);
    }



    void Draw_Delete()
    {
        ////반투명 BG
        //GUI.DrawTexture(_bg_rects[0], _TEXTURES[2]);

        ////메세지
        //GUI.Label(_bg_rects[1], "Are you sure you want to delete?", _text_styles[0]);


        if (_remove_clicked_skip_count > 0)
        {
            //맨 처음 표시할 때 아래 다이얼로그 창의 YES 버튼이 자동으로 눌려진 것으로 처리되는 것을 방지하기 위해
            //첫번째 화면을 이러한 마우스 눌림 이벤트를 소진시키기 위한 목적임
            _remove_clicked_skip_count = _remove_clicked_skip_count - 1;


        }
        else
        {
            ////No
            //if (GUI.Button(_button_rects[0], _BTN_TITLE_NO, _btn_styles[1]))
            //{
            //    _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);

            //    _remove_icon_clicked = false;
            //    _remove_clicked_id = string.Empty;
            //    _remove_clicked_skip_count = 0;
            //}


            ////Yes
            //if (GUI.Button(_button_rects[1], _BTN_TITLE_YES, _btn_styles[2]))
            //{
            //    _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);

            //    if (!string.IsNullOrEmpty(_remove_clicked_id))
            //    {
            //        Code_Item target_item = FindCodeItem(_remove_clicked_id);

            //        if (target_item != null)
            //        {
            //            //삭제 전에 현재 코드를 되돌리기 리스트에 추가한다.
            //            BackupScriptHistryList();

            //            //명령어를 삭제한다
            //            RemoveFromCodeTree(target_item);
            //        }
            //    }

            //    _remove_clicked_skip_count = 0;
            //    _remove_icon_clicked = false;
            //    _remove_clicked_id = string.Empty;
            //}

            _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);

            if (!string.IsNullOrEmpty(_remove_clicked_id))
            {
                Code_Item target_item = FindCodeItem(_remove_clicked_id);

                if (target_item != null)
                {
                    //삭제 전에 현재 코드를 되돌리기 리스트에 추가한다.
                    BackupScriptHistryList();

                    Debug.Log(target_item.CmdName);
                    //명령어를 삭제한다
                    RemoveFromCodeTree(target_item);
                }
            }

            _remove_clicked_skip_count = 0;
            _remove_icon_clicked = false;
            _remove_clicked_id = string.Empty;

        }


        //_dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);

        //if (!string.IsNullOrEmpty(_remove_clicked_id))
        //{
        //    Code_Item target_item = FindCodeItem(_remove_clicked_id);

        //    if (target_item != null)
        //    {
        //        //삭제 전에 현재 코드를 되돌리기 리스트에 추가한다.
        //        BackupScriptHistryList();

        //        //명령어를 삭제한다
        //        RemoveFromCodeTree(target_item);
        //    }
        //}

        //_remove_clicked_skip_count = 0;
        //_remove_icon_clicked = false;
        //_remove_clicked_id = string.Empty;

    }




    void GUI_FileSave()
    {
        //배경
        GUI.DrawTexture(_bg_rects[0], _TEXTURES[1]);


        //반 투명 BG Box 처리
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 0.8f);

        GUI.Box(_download_input_bg_rect, "", _gui_box_white_bg_styles);

        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 1.0f);


        if (string.IsNullOrEmpty(_dialog_msg))
        {
            #region _dialog_msg normal

            if (Application.systemLanguage == SystemLanguage.Korean)
            {
                _text_styles[22].font = _FONTS[1];
                GUI.Label(_download_input_title_rect, "파일 이름을 입력하세요.", _text_styles[22]);
            }
            else if (Application.systemLanguage == SystemLanguage.English)
            {
                GUI.Label(_download_input_title_rect, "Please input file name.", _text_styles[22]);
            }




            //레이블
            if (Application.systemLanguage == SystemLanguage.Korean)
            {
                _text_styles[23].font = _FONTS[1];
                GUI.Label(_download_email_lable_rect, "파일명", _text_styles[23]);
            }
            else if (Application.systemLanguage == SystemLanguage.English)
            {
                GUI.Label(_download_email_lable_rect, "File Name", _text_styles[23]);
            }



            //입력창
            GlobalVariables.CurrentFileName = GUI.TextField(_download_email_box_rect, GlobalVariables.CurrentFileName, _font_text_field_style);

            string CancelString = "Cancel";
            if (Application.systemLanguage == SystemLanguage.Korean)
            {
                _btn_styles[5].font = _FONTS[1];
                CancelString = "취소";
            }

            //No
            if (GUI.Button(_button_rects[2], CancelString, _btn_styles[5]))
            {
                _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);	//button sound

                //##########################################            
                _fadeScreenLibrary.RequestFadeOut("CodeEditor", true);
                //##########################################            
            }

            string SaveString = "Save";
            if (Application.systemLanguage == SystemLanguage.Korean)
            {
                _btn_styles[6].font = _FONTS[1];
                SaveString = "저장";
            }

            if (GUI.Button(_button_rects[3], SaveString, _btn_styles[6]))
            {
                _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);	//button sound


                if (!string.IsNullOrEmpty(GlobalVariables.CurrentFileName))
                {

                    //현재 표시 이름을 변경한다
                    _FILE_NAME = GlobalVariables.CurrentFileName;


                    string target_script = string.Empty;

                    if (SPLCodeEditor._EDITOR_TYPE == "TEXT")
                        target_script = _CURRENT_SCRIPT_TEXT;
                    else
                        target_script = ExportScriptAsText();


                    //사용자 코드에 저장한다.
                    string saved_target_dir = Application.persistentDataPath + "/" + GlobalVariables.SelectedCodingType;
                    string saved_target_file_name = Application.persistentDataPath + "/" + GlobalVariables.SelectedCodingType + "/" + GlobalVariables.CurrentFileName + ".txt";

                    if (!Directory.Exists(saved_target_dir))
                    {
                        Directory.CreateDirectory(saved_target_dir);
                    }


                    if (File.Exists(saved_target_file_name))
                    {
                        File.Delete(saved_target_file_name);
                    }


                    File.WriteAllText(saved_target_file_name, target_script);

                    LoadStageScriptList();
                    UpdateUserScriptList();

                    //##########################################            
                    _fadeScreenLibrary.RequestFadeOut("CodeEditor", true);
                    //##########################################         
                }
                else
                {
                    _dialog_msg = "Type file name!";
                    if (Application.systemLanguage == SystemLanguage.Korean)
                    {
                        _btn_styles[6].font = _FONTS[1];
                        _dialog_msg = "파일 이름을 입력하세요.";
                    }
                }

            }

            #endregion _dialog_msg normal
        }
        else
        {
            GUI.Label(_download_dialog_title_rect, _dialog_msg, _text_styles[22]);


            if (Application.systemLanguage == SystemLanguage.Korean)
            {
                if (_dialog_msg != "잠시만 기다려주세요.")
                {
                    //OK
                    if (GUI.Button(_button_rects[4], "OK", _btn_styles[6]))
                    {
                        _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f); //button sound

                        _dialog_msg = string.Empty;
                    }
                }
            }
            else if (Application.systemLanguage == SystemLanguage.English)
            {
                if (_dialog_msg != "Please wait...")
                {
                    //OK
                    if (GUI.Button(_button_rects[4], "OK", _btn_styles[6]))
                    {
                        _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f); //button sound

                        _dialog_msg = string.Empty;
                    }
                }
            }
        }


    }




    void GUI_ServerSave()
    {
        //배경
        GUI.DrawTexture(_bg_rects[0], _TEXTURES[1]);


        //반 투명 BG Box 처리
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 0.8f);

        GUI.Box(_download_input_bg_rect, "", _gui_box_white_bg_styles);

        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 1.0f);


        if (string.IsNullOrEmpty(_dialog_msg))
        {
            #region _dialog_msg normal

            string InputEmailString = "Please input your email and file name";
            if (Application.systemLanguage == SystemLanguage.Korean)
            {
                _text_styles[22].font = _FONTS[1];
                InputEmailString = "Email과 파일명을 입력해주세요.";
            }
            GUI.Label(_download_input_title_rect, InputEmailString, _text_styles[22]);


            string EmailString = "Email Address";
            if (Application.systemLanguage == SystemLanguage.Korean)
            {
                _text_styles[23].font = _FONTS[1];
                EmailString = "Email 주소";
            }
            //레이블
            GUI.Label(_download_email_lable_rect, EmailString, _text_styles[23]);

            //입력창
            _USER_ID = GUI.TextField(_download_email_box_rect, _USER_ID, _font_text_field_style);


            string FileNameString = "File Name";
            if (Application.systemLanguage == SystemLanguage.Korean)
            {
                _text_styles[23].font = _FONTS[1];
                FileNameString = "파일명";
            }
            //레이블
            GUI.Label(_download_file_lable_rect, FileNameString, _text_styles[23]);


            //입력창
            GlobalVariables.CurrentFileName = GUI.TextField(_download_file_box_rect, GlobalVariables.CurrentFileName, _font_text_field_style);



            string CancelString = "Cancel";
            if (Application.systemLanguage == SystemLanguage.Korean)
            {
                _btn_styles[5].font = _FONTS[1];
                CancelString = "취소";
            }
            //No
            if (GUI.Button(_button_rects[2], CancelString, _btn_styles[5]))
            {
                _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);	//button sound

                //##########################################            
                _fadeScreenLibrary.RequestFadeOut("CodeEditor", true);
                //##########################################            
            }

            string SaveServerString = "Save on server";
            if (Application.systemLanguage == SystemLanguage.Korean)
            {
                _btn_styles[6].font = _FONTS[1];
                SaveServerString = "서버에 저장";
            }
            if (GUI.Button(_button_rects[3], SaveServerString, _btn_styles[6]))
            {
                _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);	//button sound

                PlayerPrefs.SetString("_USER_ID", _USER_ID);

                if (!string.IsNullOrEmpty(_USER_ID) && !string.IsNullOrEmpty(GlobalVariables.CurrentFileName))
                {

                    //현재 표시 이름을 변경한다
                    _FILE_NAME = GlobalVariables.CurrentFileName;


                    string target_script = string.Empty;

                    if (SPLCodeEditor._EDITOR_TYPE == "TEXT")
                        target_script = _CURRENT_SCRIPT_TEXT;
                    else
                        target_script = ExportScriptAsText();


                    //사용자 코드에 저장한다.
                    string saved_target_dir = Application.persistentDataPath + "/" + GlobalVariables.SelectedCodingType;
                    string saved_target_file_name = Application.persistentDataPath + "/" + GlobalVariables.SelectedCodingType + "/" + GlobalVariables.CurrentFileName + ".txt";

                    if (!Directory.Exists(saved_target_dir))
                    {
                        Directory.CreateDirectory(saved_target_dir);
                    }


                    if (File.Exists(saved_target_file_name))
                    {
                        File.Delete(saved_target_file_name);
                    }


                    File.WriteAllText(saved_target_file_name, target_script);

                    LoadStageScriptList();
                    UpdateUserScriptList();



                    //서버에 저장
                    PlayerPrefs.SetString("USER_ID", _USER_ID);

                    _dialog_msg = "Please wait...";
                    if (Application.systemLanguage == SystemLanguage.Korean)
                    {
                        _dialog_msg = "잠시만 기다려주세요.";
                    }



                    string url = "http://bigrense.azurewebsites.net/api/upload_script_big.aspx";

                    WWWForm form = new WWWForm();
                    form.AddField("user_id", _USER_ID);
                    form.AddField("prod_type", "VIRTUAL_CODING");
                    form.AddField("cont_type", GlobalVariables.SelectedCodingType);
                    form.AddField("title", GlobalVariables.CurrentFileName);
                    form.AddField("user_script", target_script);
                    WWW www = new WWW(url, form);

                    StartCoroutine(WaitFor_Upload(www));
                }
                else
                {
                    _dialog_msg = "Type email address and file name!";
                    if (Application.systemLanguage == SystemLanguage.Korean)
                    {
                        _dialog_msg = "Email주소와 파일명을 입력해주세요.";
                    }
                }

            }

            #endregion _dialog_msg normal
        }
        else
        {
            GUI.Label(_download_dialog_title_rect, _dialog_msg, _text_styles[22]);

            if (Application.systemLanguage == SystemLanguage.Korean)
            {
                if (_dialog_msg != "잠시만 기다려주세요.")
                {
                    //OK
                    if (GUI.Button(_button_rects[4], "OK", _btn_styles[6]))
                    {
                        _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f); //button sound

                        _dialog_msg = string.Empty;
                    }
                }
            }
            else if (Application.systemLanguage == SystemLanguage.English)
            {
                if (_dialog_msg != "Please wait...")
                {
                    //OK
                    if (GUI.Button(_button_rects[4], "OK", _btn_styles[6]))
                    {
                        _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f); //button sound

                        _dialog_msg = string.Empty;
                    }
                }
            }

        }



    }




    IEnumerator WaitFor_Upload(WWW www)
    {
        yield return www;

        if (www.error == null)
        {
            if (!string.IsNullOrEmpty(www.text))
            {
                string res = www.text;

                if (res.StartsWith("OK"))
                {
                    //##########################################            
                    _fadeScreenLibrary.RequestFadeOut("CodeEditor", true);
                    //##########################################     
                }
                else
                {
                    _dialog_msg = "Fail to server upload!";
                    if (Application.systemLanguage == SystemLanguage.Korean)
                    {
                        _dialog_msg = "서버에 업로드를 실패하였습니다.";
                    }
                }
            }
            else
            {
                _dialog_msg = "Fail to server upload!";
                if (Application.systemLanguage == SystemLanguage.Korean)
                {
                    _dialog_msg = "서버에 업로드를 실패하였습니다.";
                }
            }
        }
        else
        {
            _dialog_msg = "Fail to server upload!";
            if (Application.systemLanguage == SystemLanguage.Korean)
            {
                _dialog_msg = "서버에 업로드를 실패하였습니다.";
            }
        }
    }






    private void LoadStageScriptList()
    {
        _sel_script_list.Clear();


        string saved_target_list_path = Application.persistentDataPath + "/" + GlobalVariables.SelectedCodingType + "/SCRIPT_LIST.txt";

        if (File.Exists(saved_target_list_path))
        {
            string[] title_list = File.ReadAllLines(saved_target_list_path);

            if (title_list != null)
            {
                for (int i = 0; i < title_list.Length; i++)
                {
                    _sel_script_list.Add(title_list[i]);
                }
            }
        }

    }


    void UpdateUserScriptList()
    {
        if (string.IsNullOrEmpty(GlobalVariables.CurrentFileName))
            return;

        if (_sel_script_list.Contains(GlobalVariables.CurrentFileName))
            _sel_script_list.Remove(GlobalVariables.CurrentFileName);

        _sel_script_list.Insert(0, GlobalVariables.CurrentFileName);


        //최대 60개까지만 저장
        while (_sel_script_list.Count > 60)
        {
            //60개가 넘어가면 오래된 것은 삭제
            string remove_target_file = _sel_script_list[_sel_script_list.Count - 1];

            string user_script_path = Application.persistentDataPath + "/" + GlobalVariables.SelectedCodingType + "/" + remove_target_file + ".txt";

            if (File.Exists(user_script_path))
                File.Delete(user_script_path);

            _sel_script_list.RemoveAt(_sel_script_list.Count - 1);
        }


        string saved_target_list_dir = Application.persistentDataPath + "/" + GlobalVariables.SelectedCodingType;
        string saved_target_list_name = Application.persistentDataPath + "/" + GlobalVariables.SelectedCodingType + "/SCRIPT_LIST.txt";

        if (!Directory.Exists(saved_target_list_dir))
        {
            Directory.CreateDirectory(saved_target_list_dir);
        }

        if (File.Exists(saved_target_list_name))
        {
            File.Delete(saved_target_list_name);
        }

        File.WriteAllLines(saved_target_list_name, _sel_script_list.ToArray());
    }






    void BackupScriptHistryList()
    {
        //삭제 전에 현재 코드를 되돌리기 리스트에 추가한다.
        if (SPLCodeEditor._EDITOR_TYPE == "BLOCK")
            _CURRENT_SCRIPT_TEXT = ExportScriptAsText();

        _SCRIPT_HISTORY_LIST.Insert(0, _CURRENT_SCRIPT_TEXT);

        //10개 이상이면 삭제함
        while (_SCRIPT_HISTORY_LIST.Count > 10)
            _SCRIPT_HISTORY_LIST.RemoveAt(_SCRIPT_HISTORY_LIST.Count - 1);
    }



    void RestoreFromHistryList()
    {
        if (_SCRIPT_HISTORY_LIST.Count == 0)
            return;

        if (SPLCodeEditor._EDITOR_TYPE == "BLOCK")
        {
            _CURRENT_SCRIPT_TEXT = _SCRIPT_HISTORY_LIST[0];
            _SCRIPT_HISTORY_LIST.RemoveAt(0);

            string[] lines = _CURRENT_SCRIPT_TEXT.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            LoadCodeScript_Internal(lines);
        }
        else
        {
            _CURRENT_SCRIPT_TEXT = _SCRIPT_HISTORY_LIST[0];
            _SCRIPT_HISTORY_LIST.RemoveAt(0);
        }

    }




    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

    #region GetCmdLineText

    private string GetCmdLineText(Code_Item code_item)
    {
        string line = string.Empty;

        if (code_item.CmdType == "CMD")
        {
            if (code_item.CmdName == "digitalRead" || code_item.CmdName == "analogRead"
                || code_item.CmdName == "map")
            {
                line = code_item.CmdVariable + " = " + code_item.CmdName + "(" + code_item.CmdValue + ")";
            }
            else if (code_item.CmdName == "delay" || code_item.CmdName == "digitalWrite" || code_item.CmdName == "analogWrite"
                || code_item.CmdName == "servoDrive" || code_item.CmdName == "driveWrite" || code_item.CmdName == "servoWrite"
                || code_item.CmdName == "tone")
            {
                line = code_item.CmdName + "(" + code_item.CmdValue + ")";
            }
            else
            {
                line = code_item.CmdName + " " + code_item.CmdValue;
            }
        }
        else if (code_item.CmdType == "EXPR")
        {
            if (code_item.CmdName == "Print" || code_item.CmdName == "PrintLine")
            {
                line = code_item.CmdName + "(" + code_item.CmdValue + ")";
            }
            else if (code_item.CmdName == "Serial.print" || code_item.CmdName == "Serial.println")
            {
                line = code_item.CmdName + "(" + code_item.CmdValue + ")";
            }
            else
            {
                line = code_item.CmdValue;
            }
        }
        else if (code_item.CmdType == "LOGIC")
        {
            if (code_item.CmdName == "if" || code_item.CmdName == "else if")
                line = code_item.CmdName + " (" + code_item.CmdValue + ")";
            else if (code_item.CmdName == "else" || code_item.CmdName == "{" || code_item.CmdName == "}")
                line = code_item.CmdName;
            else if (code_item.CmdName == "for")
                line = code_item.CmdName + " (" + code_item.CmdValue + ")";
            else if (code_item.CmdName == "while")
                line = code_item.CmdName + " (" + code_item.CmdValue + ")";
            else if (code_item.CmdName == "setup function")
            {
                line = "void setup()";
            }
            else if (code_item.CmdName == "loop function")
            {
                line = "void loop()";
            }
            else if (code_item.CmdName == "function")
            {
                line = code_item.CmdValue;
            }
        }
        else if (code_item.CmdType == "BLOCK_END")
        {
            line = string.Empty;
        }
        else
            line = code_item.CmdName;

        return line;
    }


    private string GetCmdLineTextForExport(Code_Item code_item)
    {
        string indentation_tab_string = string.Empty;
        string line = string.Empty;

        for (int i = 0; i < code_item.IndentationLevel; i++)
            indentation_tab_string = indentation_tab_string + "\t";


        if (code_item.CmdType == "CMD")
        {
            if (code_item.CmdName == "digitalRead" || code_item.CmdName == "analogRead" || code_item.CmdName == "map")
                line = code_item.CmdVariable + " = " + code_item.CmdName + "(" + code_item.CmdValue + ")";
            else if (code_item.CmdName == "delay" || code_item.CmdName == "digitalWrite" || code_item.CmdName == "analogWrite"
                || code_item.CmdName == "servoDrive" || code_item.CmdName == "driveWrite" || code_item.CmdName == "servoWrite"
                || code_item.CmdName == "tone")
                line = code_item.CmdName + "(" + code_item.CmdValue + ")";
            else
                line = code_item.CmdName + " " + code_item.CmdValue;
        }
        else if (code_item.CmdType == "EXPR")
        {
            if (code_item.CmdName == "Print" || code_item.CmdName == "PrintLine")
                line = code_item.CmdName + "(" + code_item.CmdValue + ")";
            else if (code_item.CmdName == "Serial.print" || code_item.CmdName == "Serial.println")
                line = code_item.CmdName + "(" + code_item.CmdValue + ")";
            else
                line = code_item.CmdValue;
        }
        else if (code_item.CmdType == "LOGIC")
        {
            if (code_item.CmdName == "if" || code_item.CmdName == "else if")
                line = code_item.CmdName + " (" + code_item.CmdValue + ")";
            else if (code_item.CmdName == "else" || code_item.CmdName == "{" || code_item.CmdName == "}")
                line = code_item.CmdName;
            else if (code_item.CmdName == "for")
                line = code_item.CmdName + " (" + code_item.CmdValue + ")";
            else if (code_item.CmdName == "while")
                line = code_item.CmdName + " (" + code_item.CmdValue + ")";
            else if (code_item.CmdName == "setup function")
                line = "void setup()";
            else if (code_item.CmdName == "loop function")
                line = "void loop()";
            else if (code_item.CmdName == "function")
                line = code_item.CmdValue;
        }

        return indentation_tab_string + line;
    }


    #endregion GetCmdLineText


    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@


    #region CreateItem


    private Code_Item GetNearestOpenedBlockItem(Code_Item item_instance)
    {
        Code_Item cur_item = item_instance;

        if (cur_item.BracketOpenFlag == true && cur_item.BracketCloseFlag == false)
            return cur_item;
        else
        {
            while (cur_item.ParentItem != null)
            {
                cur_item = cur_item.ParentItem;

                if (cur_item.BracketOpenFlag == true && cur_item.BracketCloseFlag == false)
                    return cur_item;
            }
        }

        return null;
    }


    //#######################################################################################
    //드래그 앤 드랍 명령어에 대해 신규로 명령어를 추가하고 명령어 링크를 만들어 준다.
    //#######################################################################################

    private void AddNewCodeTree(Code_Item code_item, bool block_cmd_yn, bool new_create_yn, bool load_flag)
    {

        //#########################################################################
        //    Root 객체를 처리한다.
        //#########################################################################
        if (_code_btn_root_item == null)
        {
            //Root 객체는 반드시 존재해야 한다.
            _code_btn_selected_item = new Code_Item("ROOT", "ROOT", string.Empty, null);
            _code_btn_root_item = _code_btn_selected_item;
        }

        if (_code_btn_selected_item == null)
        {
            _code_btn_selected_item = GetLastSiblingItem(_code_btn_root_item);
        }


        string new_link_up_down = "DOWN";

        if (!string.IsNullOrEmpty(_dragging_target_id))
        {
            Code_Item Dragging_Target_Item = FindCodeItem(_dragging_target_id);
            _code_btn_selected_item = Dragging_Target_Item;
            new_link_up_down = _drag_taget_up_down;
        }


        if (block_cmd_yn && new_create_yn)
        {
            code_item.BlockYN = true;

            code_item.BlockStartItem = new Code_Item("BLOCK_START", "{", string.Empty, null);
            code_item.BlockStartItem.ParentItem = code_item;

            code_item.BlockEndItem = new Code_Item("BLOCK_END", "}", string.Empty, null);
            code_item.BlockEndItem.ParentItem = code_item;
        }


        if (_code_btn_selected_item.CmdType == "BLOCK_END")
        {
            if (new_link_up_down == "DOWN")
            {
                _code_btn_selected_item = _code_btn_selected_item.ParentItem;
            }
            else if (new_link_up_down == "UP")
            {
                //현재 속해 있는 블록 명령어 안에 있는 가장 마지막 명령어로 이동한다.
                _code_btn_selected_item = GetLastSiblingItem(_code_btn_selected_item.ParentItem.BlockStartItem);

                //이전 명령어의 아래에 추가해야 하므로 DOWN으로 모드를 변경한다.
                new_link_up_down = "DOWN";
            }
        }
        else if (_code_btn_selected_item.BlockYN)
        {
            //블록의 경우에는 BlockStartItem으로 선택해 주어야 그 아래에 명령어가 추가된다.
            if (new_link_up_down == "DOWN")
            {
                if (_code_btn_selected_item.BlockStartItem != null)
                    _code_btn_selected_item = _code_btn_selected_item.BlockStartItem;
            }
        }


        //중간에 추가하기 전에 이전 연결을 백업한다.
        Code_Item pre_item_backup = _code_btn_selected_item.PreCmdItem;
        Code_Item next_item_backup = _code_btn_selected_item.NextCmdItem;


        if (new_link_up_down == "DOWN")
        {
            _code_btn_selected_item.NextCmdItem = code_item;
            code_item.PreCmdItem = _code_btn_selected_item;
            code_item.ParentItem = _code_btn_selected_item.ParentItem;


            //다음 연결 복구
            code_item.NextCmdItem = next_item_backup;

            if (next_item_backup != null)
                next_item_backup.PreCmdItem = code_item;
        }
        else if (new_link_up_down == "UP")
        {
            _code_btn_selected_item.PreCmdItem = code_item;
            code_item.NextCmdItem = _code_btn_selected_item;
            code_item.ParentItem = _code_btn_selected_item.ParentItem;


            //이전 연결 복구
            code_item.PreCmdItem = pre_item_backup;

            if (pre_item_backup != null)
                pre_item_backup.NextCmdItem = code_item;
        }


        //#############################################################################
        //현재 추가한 블록 명령어를 최종 선택된 명령어로 처리한다.
        //#############################################################################

        if (block_cmd_yn)
        {
            //_code_btn_selected_item = code_item.BlockStartItem;
            _code_btn_selected_item = code_item;
        }
        else if (_code_btn_selected_item.CmdType == "BLOCK_START")
        {
            //블록 안에 있는 첫번째 명령어로서 중괄호가 없는 경우는 블록 다음 명령어로 이동해야 한다. (if 명령어에 하나의 자식 명령어만 있는 경우
            if (_code_btn_selected_item.ParentItem != null && _code_btn_selected_item.ParentItem.BracketOpenFlag == false)
            {
                if (_code_btn_selected_item.ParentItem.BlockEndItem == null)
                    _code_btn_selected_item.ParentItem.BlockEndItem = new Code_Item("BLOCK_END", "}", string.Empty, null);

                _code_btn_selected_item = _code_btn_selected_item.ParentItem.BlockEndItem;
            }
            else
                _code_btn_selected_item = code_item;
        }
        else
            _code_btn_selected_item = code_item;

        //#############################################################################
    }



    private void RemoveFromCodeTree(Code_Item code_item)
    {
        //중간에 추가하기 전에 이전 연결을 백업한다.
        Code_Item pre_item_backup = null;
        Code_Item next_item_backup = null;

        if (code_item != null && code_item.PreCmdItem != null)
            pre_item_backup = code_item.PreCmdItem;

        if (code_item != null && code_item.NextCmdItem != null)
            next_item_backup = code_item.NextCmdItem;

        if (pre_item_backup != null)
            pre_item_backup.NextCmdItem = next_item_backup;

        if (next_item_backup != null)
            next_item_backup.PreCmdItem = pre_item_backup;
    }


    private Code_Item CreateItem(string cmd_item_type, string item_name, string item_value, string item_variable, bool load_flag)
    {
        //#########################################################################
        //    마지막 추가한 변수 이름과 형식 처리
        //#########################################################################

        string item_value_str = item_value;
        string item_variable_str = item_variable;

        if (string.IsNullOrEmpty(item_value_str) && load_flag == false)
        {
            item_value_str = GetNewTargetName(item_name);

            if (cmd_item_type == "CMD")
            {
                if (string.IsNullOrEmpty(_LAST_VARIABLE_NAME))
                    _LAST_VARIABLE_NAME = item_value_str;
                else
                    _LAST_OBJECT_NAME = item_value_str;
            }
        }


        //#########################################################################
        //    Root 객체를 처리한다.
        //#########################################################################

        if (_code_btn_selected_item == null)
        {
            //Root 객체는 반드시 존재해야 한다.
            _code_btn_selected_item = new Code_Item("ROOT", "ROOT", string.Empty, null);
            _code_btn_root_item = _code_btn_selected_item;
        }



        //#########################################################################
        //    명령어별 처리
        //#########################################################################

        //명령어를 추가한다.
        Code_Item code_item = new Code_Item(cmd_item_type, item_name, item_value_str, null);
        code_item.Width = 400 * _h_ratio;
        code_item.Height = 40 * _h_ratio;


        if (item_name == "{")
        {
            //이 명령어는 이제 블록 편집기에서는 입력이 않되고 스크립트에서만 입력 가능
        }
        else if (item_name == "}")
        {
            //이 명령어는 이제 블록 편집기에서는 입력이 않되고 스크립트에서만 입력 가능
        }
        else if (item_name == "digitalRead" || item_name == "DigitalRead")
        {
            code_item.VariableYN = true;

            if (string.IsNullOrEmpty(item_variable_str))
                code_item.CmdVariable = "d";
            else
                code_item.CmdVariable = item_variable_str;
        }
        else if (item_name == "analogRead" || item_name == "AnalogRead")
        {
            code_item.VariableYN = true;

            if (string.IsNullOrEmpty(item_variable_str))
                code_item.CmdVariable = "a";
            else
                code_item.CmdVariable = item_variable_str;
        }
        else if (item_name == "map" || item_name == "Map")
        {
            code_item.VariableYN = true;

            if (string.IsNullOrEmpty(item_variable_str))
                code_item.CmdVariable = "a";
            else
                code_item.CmdVariable = item_variable_str;
        }
        else if (item_name == "digitalWriteHigh" || item_name == "digitalWriteLow" || item_name == "DigitalWriteHigh" || item_name == "DigitalWriteLow")
        {
            code_item.CmdName = "digitalWrite";
        }
        else if (item_name == "if" || item_name == "else" || item_name == "else if" || item_name == "for" || item_name == "while")
        {
            code_item.ValueYN = true;
            code_item.OptionYN = false;
            code_item.VariableYN = false;
        }
        else if (item_name == "setup function" || item_name == "loop function" || item_name == "Setup Function" || item_name == "Loop Function")
        {
            code_item.ValueYN = false;
            code_item.OptionYN = false;
            code_item.VariableYN = false;
        }
        else if (item_name == "function" || item_name == "Function")
        {
            code_item.ValueYN = true;   //has function name
            code_item.OptionYN = false;
            code_item.VariableYN = false;
        }


        //블록 명령어 자체가 선택되어 있는 경우, 블록 내부의 첫번째 항목으로 이동시킨다.
        //###########################################################################################
        if (_code_btn_selected_item.BlockYN && _code_btn_selected_item.BlockStartItem != null)
        {
            _code_btn_selected_item = _code_btn_selected_item.BlockStartItem;

            //마지막으로 이동하려면 아래의 주석을 해제
            //while (_code_btn_selected_item.NextCmdItem != null)
            //    _code_btn_selected_item = _code_btn_selected_item.NextCmdItem;
        }
        //###########################################################################################


        if (item_name == "if" || item_name == "else" || item_name == "else if" || item_name == "for" || item_name == "while"
            || item_name == "setup function" || item_name == "loop function" || item_name == "function"
            || item_name == "Setup Function" || item_name == "Loop Function" || item_name == "Function")
        {
            AddNewCodeTree(code_item, true, true, load_flag);
        }
        else if (item_name == "{")
        {
            if (_code_btn_selected_item.CmdType == "BLOCK_START")
            {
                if (_code_btn_selected_item.ParentItem != null)
                {
                    _code_btn_selected_item.ParentItem.BracketOpenFlag = true;
                    _code_btn_selected_item.ParentItem.BracketCloseFlag = false;
                }
            }
        }
        else if (item_name == "}")
        {
            Code_Item nearest_opened_item = GetNearestOpenedBlockItem(_code_btn_selected_item);

            if (nearest_opened_item != null)
            {
                nearest_opened_item.BracketCloseFlag = true;

                if (nearest_opened_item.BlockEndItem == null)
                    nearest_opened_item.BlockEndItem = new Code_Item("BLOCK_END", "}", string.Empty, null);

                _code_btn_selected_item = nearest_opened_item.BlockEndItem;
            }
        }
        else
        {
            //#######################################################################################
            //드래그 앤 드랍 명령어에 대해 신규로 명령어를 추가하고 명령어 링크를 만들어 준다.
            //#######################################################################################
            AddNewCodeTree(code_item, false, true, load_flag);
        }


        if (load_flag == false)
        {
            _code_btn_root_item.NextCmdItem.CalcChildCmdCount(true);

            _rect_center_code_scroll_content.height = (_code_btn_root_item.NextCmdItem.ChildBlockCmdCount + _code_btn_root_item.NextCmdItem.ChildSingleLineCmdCount) * 50 * _h_ratio;

            if (_code_btn_root_item.NextCmdItem.ChildBlockCmdCount > 0)
            {
                _rect_center_code_scroll_content.height = _rect_center_code_scroll_content.height + (_code_btn_root_item.NextCmdItem.ChildBlockCmdCount - 1) * 50 * _h_ratio;
                _rect_center_code_scroll_content.height = _rect_center_code_scroll_content.height - (_code_btn_root_item.NextCmdItem.ChildBlockCmdCount - 1) * 20 * _h_ratio;
            }

            _rect_center_code_scroll_content.height = _rect_center_code_scroll_content.height + 100 * _h_ratio;
        }


        if (load_flag == false)
            ShowOptionList();

        return code_item;
    }


    #endregion CreateItem


    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@



    string _last_target_name = string.Empty;

    private string GetNewTargetName(string cmd_name)
    {
        string res = string.Empty;
        int last_num = 0;

        if (_cmd_seq_num_list.ContainsKey(cmd_name))
        {
            last_num = _cmd_seq_num_list[cmd_name];
            last_num++;
            _cmd_seq_num_list[cmd_name] = last_num;
        }
        else
        {
            last_num = 1;
            _cmd_seq_num_list.Add(cmd_name, last_num);
        }


        if (cmd_name == "if" || cmd_name == "else if")
            res = "true";
        else if (cmd_name == "for")
            res = "i = 0; i < 10; i++";
        else if (cmd_name == "while")
            res = "true";
        else if (cmd_name == "Serial.println" || cmd_name == "PrintLine")
            res = "a";
        else if (cmd_name == "delay" || cmd_name == "Delay")
            res = "1000";
        else if (cmd_name == "digitalRead" || cmd_name == "DigitalRead")
            res = "2";
        else if (cmd_name == "digitalWrite" || cmd_name == "DigitalWrite")
            res = "13, HIGH";
        else if (cmd_name == "digitalWriteHigh" || cmd_name == "DigitalWriteHigh")
            res = "13, HIGH";
        else if (cmd_name == "digitalWriteLow" || cmd_name == "DigitalWriteLow")
            res = "13, LOW";
        else if (cmd_name == "analogRead" || cmd_name == "AnalogRead")
            res = "0";
        else if (cmd_name == "analogWrite" || cmd_name == "AnalogWrite")
            res = "3, 255";
        else if (cmd_name == "servoDrive" || cmd_name == "ServoDrive")
            res = "90, 90";
        else if (cmd_name == "servoWrite" || cmd_name == "ServoWrite")
            res = "2, 90";
        else if (cmd_name == "driveWrite" || cmd_name == "DriveWrite")
            res = "200, 200";
        else if (cmd_name == "tone" || cmd_name == "Tone")
            res = "9, 1000, 1000";
        else if (cmd_name == "map" || cmd_name == "Map")
            res = "a, 0, 1023, 0, 255";
        else if (cmd_name == "function")
            res = "void f" + last_num.ToString() + "()";

        return res;
    }


    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@




    void UpdateCodeScrollSize()
    {
        //#########################################################################
        //    Root 객체를 처리한다.
        //#########################################################################

        if (_code_btn_root_item == null)
        {
            //Root 객체는 반드시 존재해야 한다.
            _code_btn_selected_item = new Code_Item("ROOT", "ROOT", string.Empty, null);
            _code_btn_root_item = _code_btn_selected_item;
        }


        //#########################################################################
        //스크롤 영역의 크기를 계산한다.
        //#########################################################################

        if (_code_btn_root_item.NextCmdItem != null)
        {
            _code_btn_root_item.NextCmdItem.CalcChildCmdCount(true);


            _rect_center_code_scroll_content.height = (_code_btn_root_item.NextCmdItem.ChildBlockCmdCount + _code_btn_root_item.NextCmdItem.ChildSingleLineCmdCount) * 50 * _h_ratio;

            //블록인 경우에 기본적으로 40 픽셀의 빈 영역과 20픽셀의 하단부 영역 블록이 추가되기 때문에 블록 명령어 갯수에 60 픽셀을 추가로 곱해 주어야 한다.
            _rect_center_code_scroll_content.height = _rect_center_code_scroll_content.height + (_code_btn_root_item.NextCmdItem.ChildBlockCmdCount * 60 * _h_ratio);



            if (_code_btn_root_item.NextCmdItem.ChildBlockCmdCount > 0)
            {
                //옵션이 있는 경우에 적용. 아두이노에서는 사용되지 않음
                _rect_center_code_scroll_content.height = _rect_center_code_scroll_content.height + (_code_btn_root_item.NextCmdItem.ChildBlockCmdCount - 1) * 50 * _h_ratio;
                _rect_center_code_scroll_content.height = _rect_center_code_scroll_content.height - (_code_btn_root_item.NextCmdItem.ChildBlockCmdCount - 1) * 20 * _h_ratio;
            }
        }

        _rect_center_code_scroll_content.height = _rect_center_code_scroll_content.height + 100 * _h_ratio;
        //#########################################################################
    }


    Code_Item GetLastSiblingItem(Code_Item cur_item)
    {
        if (cur_item == null)
            return null;
        else if (cur_item.NextCmdItem == null)
            return cur_item;
        else
        {
            Code_Item next_item = cur_item.NextCmdItem;
            while (next_item != null)
            {
                if (next_item.NextCmdItem == null)
                    return next_item;
                else
                    next_item = next_item.NextCmdItem;
            }
        }

        return cur_item;
    }


    Code_Item GetLastRootSiblingItem()
    {
        //#########################################################################
        //    Root 객체를 처리한다.
        //#########################################################################

        if (_code_btn_root_item == null)
        {
            //Root 객체는 반드시 존재해야 한다.
            _code_btn_root_item = new Code_Item("ROOT", "ROOT", string.Empty, null);
            _code_btn_selected_item = _code_btn_root_item;
        }

        if (_code_btn_root_item.NextCmdItem == null)
            return _code_btn_root_item;

        Code_Item next_item = _code_btn_root_item.NextCmdItem;
        while (next_item != null)
        {
            if (next_item.NextCmdItem == null)
                return next_item;
            else
                next_item = next_item.NextCmdItem;
        }

        return next_item;
    }



    string ExportScriptAsText()
    {
        List<string> lines = ExportScript();

        string target_script = string.Empty;

        for (int i = 0; i < lines.Count; i++)
        {
            target_script = target_script + lines[i] + System.Environment.NewLine;
        }

        return target_script;
    }



    List<string> ExportScript()
    {
        UpdateCodeScrollSize();

        List<string> lines = new List<string>();

        if (_code_btn_root_item != null && _code_btn_root_item.NextCmdItem != null)
        {
            ExportScriptInternal(_code_btn_root_item.NextCmdItem, lines, 0);
        }

        return lines;
    }


    void ExportScriptInternal(Code_Item code_item, List<string> lines, int indent_num)
    {
        //########################################################
        // 먼저 실제 명령어가 표시되는 블록을 체크한다.
        //########################################################


        string indentation_str = string.Empty;

        for (int i = 0; i < indent_num; i++)
            indentation_str = indentation_str + "    ";

        if (code_item != null)
        {
            lines.Add(indentation_str + GetCmdLineTextForExport(code_item));
        }


        //########################################################
        // 자식이 있으면 자식에 대해서 호출해 준다.
        //########################################################
        if (code_item.BlockStartItem != null)
        {
            lines.Add(indentation_str + "{");

            if (code_item.BlockStartItem.NextCmdItem != null)
                ExportScriptInternal(code_item.BlockStartItem.NextCmdItem, lines, indent_num + 1);
        }


        //########################################################
        // BLOCK_END
        // end block을 그려준다.
        //########################################################        
        if (code_item.BlockEndItem != null)
        {
            lines.Add(indentation_str + "}");

            //빈 공백 라인을 추가해 준다.
            lines.Add("");
        }


        //########################################################
        // Next
        // 그 다음 명령어가 있으면 그 다음 명령어를 처리한다.
        //########################################################
        if (code_item.NextCmdItem != null)
        {
            ExportScriptInternal(code_item.NextCmdItem, lines, indent_num);
        }

    }




    Code_Item FindCodeItem(string id_str)
    {
        Code_Item res = null;

        if (_code_btn_root_item != null && _code_btn_root_item.NextCmdItem != null)
        {
            res = FindCodeItemInternal(_code_btn_root_item.NextCmdItem, id_str);
        }

        return res;
    }


    Code_Item FindCodeItemInternal(Code_Item code_item, string id_str)
    {
        Code_Item res = null;

        //########################################################
        // 먼저 실제 명령어가 표시되는 블록을 체크한다.
        //########################################################
        if (code_item != null && code_item.Id == id_str)
        {
            return code_item;
        }


        //########################################################
        // 자식이 있으면 자식에 대해서 호출해 준다.
        //########################################################
        if (code_item.BlockStartItem != null && code_item.BlockStartItem.NextCmdItem != null)
        {
            Code_Item temp_res = FindCodeItemInternal(code_item.BlockStartItem.NextCmdItem, id_str);

            if (temp_res != null)
                return temp_res;
        }


        //########################################################
        // BLOCK_END
        // end block을 그려준다.
        //########################################################        
        if (code_item.BlockEndItem != null)
        {
            Code_Item temp_res = FindCodeItemInternal(code_item.BlockEndItem, id_str);

            if (temp_res != null)
                return temp_res;
        }


        //########################################################
        // Next
        // 그 다음 명령어가 있으면 그 다음 명령어를 처리한다.
        //########################################################
        if (code_item.NextCmdItem != null)
        {
            return FindCodeItemInternal(code_item.NextCmdItem, id_str);
        }

        return res;
    }


    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@


    private Vector2 GetReversePosV2(Vector2 v2)
    {
        return new Vector2(v2.x, Screen.height - v2.y);
    }


    private string GetCurSelectedItemName()
    {
        if (_code_btn_selected_item == null)
            return string.Empty;
        else
            return _code_btn_selected_item.CmdName;
    }

    private string GetCurSelectedItemValue()
    {
        if (_code_btn_selected_item == null)
            return string.Empty;
        else
        {
            return _code_btn_selected_item.CmdValue;
        }
    }

    private void SetCurSelectedItemValue(string new_val)
    {
        if (_code_btn_selected_item == null)
            return;
        else
        {
            _code_btn_selected_item.CmdValue = new_val;
        }
    }


    private string GetCurSelectedItemVariable()
    {
        if (_code_btn_selected_item == null)
            return string.Empty;
        else
        {
            return _code_btn_selected_item.CmdVariable;
        }
    }

    private void SetCurSelectedItemVariable(string new_val)
    {
        if (_code_btn_selected_item == null)
            return;
        else
        {
            _code_btn_selected_item.CmdVariable = new_val;
        }
    }


    bool IsNeededValueChangeButton(string cur_item_name)
    {
        if (cur_item_name == "delay")
            return true;
        else if (cur_item_name == "digitalWrite" || cur_item_name == "analogWrite" || cur_item_name == "driveWrite" || cur_item_name == "tone")
            return true;
        else if (cur_item_name == "driveWrite" || cur_item_name == "servoWrite")
            return true;
        else
            return false;
    }



    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

}



