using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using System.Net;
using SPL.Common;
using gv;

public class MainMenu : MonoBehaviour
{

    private char _RECORD_SEPARATER = (char)0x1E;
    private char _FIELD_SEPARATER = (char)0x1C;

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

    public Texture2D[] _BG_TEXTURES;
    public Texture2D[] _THUMB_TEXTURES;
    public Texture2D[] _HEADER_TEXTURES;
    public Texture2D[] _ICON_TEXTURES;

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
    //Header Region
    //######################################################################
    private Rect _header_region_rects;
    private Rect[] _header_btn_rects = new Rect[5];

    private string _SELECTED_SCENARIO_NO = "";
    private string _SELECTED_SCENARIO_NAME = "";

    private string _FILE_NAME = "";
    private string _TITLE_NAME = "";

    private Rect _header_title_rects;
    private Rect _header_file_name_rects;

    private GUIStyle[] _header_btn_styles = new GUIStyle[5];



    //######################################################################
    //메인메뉴 처리를 위한 전역변수
    //######################################################################


    private bool _first_set_flag = false;

    private string _selected_coding_type = string.Empty;
    private int _CUR_BG_TEXTURE_NUM = 0;
    private int _PRE_BG_TEXTURE_NUM = 0;
    private int _CUR_THUMB_TEXTURE_NUM = 0;


    private Vector2[] _thumb_slide_pos = new Vector2[20];
    private Vector2[] _thumb_slide_scale = new Vector2[20];

    private Vector2[] _pre_slide_pos = new Vector2[20];
    private Vector2[] _pre_slide_scale = new Vector2[20];

    private Vector2[] _target_slide_pos = new Vector2[7];   //left 3, center 1, right 3
    private Vector2[] _target_slide_scale = new Vector2[7];


    private Vector2 _thumb_start_pos_v2 = new Vector2(0, 0);
    private Vector2 _thumb_end_pos_v2 = new Vector2(0, 0);
    private Vector2 _thumb_dumy_pos_v2 = new Vector2(0, 0);

    private Rect _stage_dumy_rect = new Rect(0, 0, 0, 0);
    private Rect _stage_region_rect = new Rect(0, 0, 0, 0);

    private Rect _pre_arrow_rect = new Rect(0, 0, 0, 0);
    private Rect _next_arrow_rect = new Rect(0, 0, 0, 0);
    private Rect _stage_bottom_title_rect = new Rect(0, 0, 0, 0);

    private GUIStyle[] _slide_btn_styles = new GUIStyle[2];



    //######################################################################
    //Small Bubble
    //######################################################################

    private Rect _small_bubble_region_rect = new Rect(0, 0, 0, 0);
    private Rect _small_bubble_dumy_rect = new Rect(0, 0, 0, 0);

    //스테이지별 설명 문구
    private List<string> _stage_desc_list = new List<string>();

    //######################################################################
    //스크립크 목록
    //######################################################################
    private Rect _bubble_right_rect = new Rect(0, 0, 0, 0);

    private Rect _create_script_btn_rect = new Rect(0, 0, 0, 0);
    private Rect _create_icon_rect = new Rect(0, 0, 0, 0);


    private List<string> _sel_script_list = new List<string>();
    private List<string> _download_script_list = new List<string>();
    private List<string> _download_num_list = new List<string>();


    //######################################################################
    // 리스트 스크롤 처리
    //######################################################################
    private Rect _script_list_scroll_view_rect;
    private Rect _script_list_scroll_content_rect;

    private Rect _script_list_btn_rect;
    private Rect _script_delte_btn_rect = new Rect(0, 0, 0, 0);

    //Touch
    private Rect _script_list_scroll_check_rect = new Rect(0, 0, 0, 0);

    private bool _remove_icon_clicked = false;
    private int _sel_delete_num = -1;



    //##################################################################
    //스크롤 박스 영역의 스크롤러 위치를 저장하는 전역변수이다.
    //##################################################################
    Vector2 _scroll_pos_script_list = new Vector2(0, 0);
    Vector2 _scroll_pos_list_download = new Vector2(0, 0);



    //######################################################################
    //Download
    //######################################################################
    private GUIStyle _gui_box_white_bg_styles;
    private GUIStyle _font_text_field_style;

    private Rect _download_input_bg_rect = new Rect(0, 0, 0, 0);
    private Rect _download_input_title_rect = new Rect(0, 0, 0, 0);
    private Rect _download_dialog_title_rect = new Rect(0, 0, 0, 0);
    private Rect _download_list_title_rect = new Rect(0, 0, 0, 0);
    private Rect _download_input_box_rect = new Rect(0, 0, 0, 0);
    private Rect _download_list_bg_rect = new Rect(0, 0, 0, 0);
    private Rect _download_list_scroll_view_rect = new Rect(0, 0, 0, 0);
    private Rect _download_list_scroll_content_rect = new Rect(0, 0, 0, 0);

    private Rect _download_list_btn_rect;

    //Touch
    private Rect _download_list_scroll_check_rect = new Rect(0, 0, 0, 0);


    private string _USER_ID = string.Empty;
    private string _dialog_msg = string.Empty;


    void Start()
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



        _USER_ID = PlayerPrefs.GetString("_USER_ID");


        //사용자 선택 정보. 0 ~ 14
        _selected_coding_type = GlobalVariables.SelectedCodingType;

        if (string.IsNullOrEmpty(_selected_coding_type))
        {
            _selected_coding_type = "TrafficSignal";
            GlobalVariables.SelectedCodingType = "TrafficSignal";
        }


        SetCurBGNum();

        SetCurStageTitle();


        //전역 객체들을 초기화 한다.
        SetupWorks();
        SetupMenuButtons();
        SetupTextStyle();
        SetupGUIStyle();


        _dialog_name = "MainMenu";


        LoadStageScriptList();


        Start_Calc_Thumb();


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


        //OK
        _button_rects[2] = new Rect(0, 0, btn_width, btn_height);
        _button_rects[2].x = screen_center_x - (btn_width * 0.5f);
        _button_rects[2].y = screen_center_y - (20 * _h_ratio);





        //각 영역별로 기본 크기를 계산해 놓는다.
        //개별 항목들은 큰 리전 그룹의 값을 이용해서 상대적으로 계산한다.
        //######################################################################
        //기본 영역별 큰 위치 계산 미리 해 놓음
        //######################################################################
        float single_padding = 8 * _s_ratio;
        float double_padding = single_padding * 2;


        //######################################################################
        //Header Region
        //######################################################################

        float header_btn_size = 100 * _h_ratio;

        //Back Button
        _header_btn_rects[0] = new Rect(0, 0, header_btn_size, header_btn_size);

        //Download Button        
        _header_btn_rects[1] = new Rect(_screen_width - _header_btn_rects[0].width, 0, header_btn_size, header_btn_size);


        _header_title_rects = new Rect(
            _header_btn_rects[0].x + _header_btn_rects[0].width + double_padding,
            0,
            _screen_width,
            header_btn_size);

        _header_file_name_rects = new Rect(
            _header_btn_rects[3].x - (300 * _w_ratio),
            0,
            _screen_width,
            header_btn_size);



        //######################################################################
        //Common Variable
        //######################################################################
        float region_x = 0;
        float region_y = 0;
        float region_width = 0;
        float region_height = 0;



        //###############################################################
        //Thumb Slide
        //###############################################################

        for (int i = 0; i < 16; i++)
        {
            _thumb_slide_pos[i] = new Vector2(0, 0);
            _thumb_slide_scale[i] = new Vector2(200 * _s_ratio, 200 * _s_ratio);

            _pre_slide_pos[i] = new Vector2(0, 0);
            _pre_slide_scale[i] = new Vector2(200 * _s_ratio, 200 * _s_ratio);
        }


        _thumb_start_pos_v2.x = _screen_width * 0.6f;
        _thumb_start_pos_v2.y = _screen_height * 0.2f;

        _thumb_end_pos_v2.x = -200 * _s_ratio;
        _thumb_end_pos_v2.y = _screen_height - (150 * _s_ratio);



        for (int i = 0; i < 7; i++)
        {

            _target_slide_pos[i] = new Vector2(0, 0);
            _target_slide_scale[i] = new Vector2(200 * _s_ratio, 200 * _s_ratio);

            if (i == 3)
            {
                _thumb_dumy_pos_v2 = Vector2.Lerp(_thumb_start_pos_v2, _thumb_end_pos_v2, 0.5f);

                _target_slide_scale[i].x = 500 * _s_ratio;
                _target_slide_scale[i].y = 500 * _s_ratio;
                _target_slide_pos[i].x = _thumb_dumy_pos_v2.x - (150 * _s_ratio);
                _target_slide_pos[i].y = _thumb_dumy_pos_v2.y - (225 * _s_ratio);
            }
            else if (i < 3)
            {
                _thumb_dumy_pos_v2 = Vector2.Lerp(_thumb_end_pos_v2, _thumb_start_pos_v2, i / 8.0f);

                _target_slide_scale[i].x = 200 * _s_ratio;
                _target_slide_scale[i].y = 200 * _s_ratio;
                _target_slide_pos[i].x = _thumb_dumy_pos_v2.x;
                _target_slide_pos[i].y = _thumb_dumy_pos_v2.y;
            }
            else if (i > 3)
            {
                _thumb_dumy_pos_v2 = Vector2.Lerp(_thumb_end_pos_v2, _thumb_start_pos_v2, (i + 2) / 8.0f);

                _target_slide_scale[i].x = 200 * _s_ratio;
                _target_slide_scale[i].y = 200 * _s_ratio;
                _target_slide_pos[i].x = _thumb_dumy_pos_v2.x;
                _target_slide_pos[i].y = _thumb_dumy_pos_v2.y;
            }
        }


        //마우스 드래그를 위한 영역 설정
        _stage_region_rect.width = _screen_width * 0.5f;
        _stage_region_rect.height = _screen_height * 0.8f;
        _stage_region_rect.x = 0;
        _stage_region_rect.y = _screen_height * 0.1f;



        //######################################################################
        //Small Bubble
        //######################################################################


        //Stage 01 ~ 15 순서대로 설명 입력
        _stage_desc_list.Add("Learn arduino digital\ncommand\n");
        _stage_desc_list.Add("Learn arduino digital\ncommand\n");
        _stage_desc_list.Add("Learn arduino digital\ncommand\n");
        _stage_desc_list.Add("Learn arduino digital\ncommand\n");
        _stage_desc_list.Add("Learn arduino digital\ncommand\n");
        _stage_desc_list.Add("Learn arduino digital\ncommand\n");
        _stage_desc_list.Add("Learn arduino digital\ncommand\n");
        _stage_desc_list.Add("Learn arduino digital\ncommand\n");
        _stage_desc_list.Add("Learn arduino digital\ncommand\n");
        _stage_desc_list.Add("Learn arduino digital\ncommand\n");
        _stage_desc_list.Add("Learn arduino digital\ncommand\n");
        _stage_desc_list.Add("Learn arduino digital\ncommand\n");
        _stage_desc_list.Add("Learn arduino digital\ncommand\n");
        _stage_desc_list.Add("Learn arduino digital\ncommand\n");
        _stage_desc_list.Add("Learn arduino digital\ncommand\n");




        //###############################################################
        //Slide Button
        //###############################################################

        float slide_main_x = _target_slide_pos[3].x + (_target_slide_scale[3].x * 0.5f);
        float slide_main_y = _target_slide_pos[3].y + _target_slide_scale[3].y;


        float slide_offset_x = _target_slide_scale[3].x * 0.1f;

        _pre_arrow_rect.width = 40 * _s_ratio;
        _pre_arrow_rect.height = 30 * _s_ratio;
        _pre_arrow_rect.x = slide_main_x - (_target_slide_scale[3].x * 0.5f) + slide_offset_x;
        _pre_arrow_rect.y = slide_main_y + (50 * _h_ratio);


        _next_arrow_rect.width = 40 * _s_ratio;
        _next_arrow_rect.height = 30 * _s_ratio;
        _next_arrow_rect.x = slide_main_x + (_target_slide_scale[3].x * 0.5f) - _next_arrow_rect.width + slide_offset_x;
        _next_arrow_rect.y = slide_main_y + (50 * _h_ratio);


        _stage_bottom_title_rect.width = _target_slide_scale[3].x;
        _stage_bottom_title_rect.height = 30 * _h_ratio;
        _stage_bottom_title_rect.x = _target_slide_pos[3].x + slide_offset_x;
        _stage_bottom_title_rect.y = slide_main_y + (50 * _h_ratio);



        _slide_btn_styles[0] = new GUIStyle();
        _slide_btn_styles[0].normal.background = _ICON_TEXTURES[4];
        _slide_btn_styles[0].hover.background = _ICON_TEXTURES[4];
        _slide_btn_styles[0].active.background = _ICON_TEXTURES[5];

        _slide_btn_styles[1] = new GUIStyle();
        _slide_btn_styles[1].normal.background = _ICON_TEXTURES[6];
        _slide_btn_styles[1].hover.background = _ICON_TEXTURES[6];
        _slide_btn_styles[1].active.background = _ICON_TEXTURES[7];


        //###############################################################
        //스크립트 리스트
        //###############################################################
        _bubble_right_rect.width = Screen.width * 0.45f;
        _bubble_right_rect.height = Screen.height - ((_header_btn_rects[1].height + 5 * _h_ratio) * 2);
        _bubble_right_rect.x = _screen_width - _bubble_right_rect.width - (5 * _w_ratio);
        _bubble_right_rect.y = _header_btn_rects[1].y + _header_btn_rects[1].height - (5 * _h_ratio);


        _create_script_btn_rect.width = _bubble_right_rect.width - (100 * _w_ratio);
        _create_script_btn_rect.height = 40 * _h_ratio;
        _create_script_btn_rect.x = _bubble_right_rect.x + (80 * _w_ratio);
        _create_script_btn_rect.y = _bubble_right_rect.y + (20 * _h_ratio);


        _create_icon_rect.width = 20 * _h_ratio;
        _create_icon_rect.height = 20 * _h_ratio;
        _create_icon_rect.x = _bubble_right_rect.x + _bubble_right_rect.width - (60 * _w_ratio);
        _create_icon_rect.y = _bubble_right_rect.y + (30 * _h_ratio);



        //######################################################################
        //Script LIst Region
        //######################################################################
        region_x = _bubble_right_rect.x;
        region_y = _bubble_right_rect.y;
        region_width = _bubble_right_rect.width;
        region_height = _bubble_right_rect.height;


        //Scroll View
        _script_list_scroll_view_rect = new Rect();
        _script_list_scroll_view_rect.x = region_x + (40 * _w_ratio);
        _script_list_scroll_view_rect.y = region_y + (70 * _h_ratio);
        _script_list_scroll_view_rect.width = region_width - (75 * _w_ratio);
        _script_list_scroll_view_rect.height = region_height - (90 * _h_ratio);

        _script_list_scroll_content_rect = new Rect(0, 0, 100, 100);
        _script_list_scroll_content_rect.width = region_width - (95 * _w_ratio);
        _script_list_scroll_content_rect.height = _sel_script_list.Count * (50 * _h_ratio) + (60 * _h_ratio);


        //스크롤 기능 구현을 위해 추가
        _script_list_scroll_check_rect.x = _script_list_scroll_view_rect.x;
        _script_list_scroll_check_rect.y = _script_list_scroll_view_rect.y;
        _script_list_scroll_check_rect.width = _script_list_scroll_view_rect.width - (40 * _w_ratio);
        _script_list_scroll_check_rect.height = _script_list_scroll_view_rect.height;


        //임시 더미 Rect  객체
        _script_list_btn_rect = new Rect();
        _script_list_btn_rect.x = 40 * _w_ratio;
        _script_list_btn_rect.y = 0;
        _script_list_btn_rect.width = _script_list_scroll_content_rect.width - (60 * _w_ratio);
        _script_list_btn_rect.height = 40 * _h_ratio;



        _script_delte_btn_rect.x = _script_list_btn_rect.width + _script_list_btn_rect.x;
        _script_delte_btn_rect.y = 0;
        _script_delte_btn_rect.width = 25 * _w_ratio;
        _script_delte_btn_rect.height = 25 * _h_ratio;



        //#################################################################################
        //Download
        //#################################################################################

        _download_input_bg_rect = new Rect(Screen.width * 0.15f, half_height * 0.25f, Screen.width * 0.7f, half_height);


        region_x = _download_input_bg_rect.x;
        region_y = _download_input_bg_rect.y;
        region_width = _download_input_bg_rect.width;
        region_height = _download_input_bg_rect.height;

        //Title
        _download_input_title_rect.width = region_width;
        _download_input_title_rect.height = 50 * _h_ratio;
        _download_input_title_rect.x = region_x;
        _download_input_title_rect.y = region_y + 30 * _h_ratio;


        //Dialog Title
        _download_dialog_title_rect.width = region_width;
        _download_dialog_title_rect.height = 50 * _h_ratio;
        _download_dialog_title_rect.x = region_x;
        _download_dialog_title_rect.y = region_y + 80 * _h_ratio;


        //Input Box
        _download_input_box_rect.width = region_width * 0.8f;
        _download_input_box_rect.height = 50 * _h_ratio;
        _download_input_box_rect.x = region_x + (region_width * 0.1f);
        _download_input_box_rect.y = region_y + (120 * _h_ratio);


        //Download List Bg Box
        _download_list_bg_rect = new Rect(Screen.width * 0.15f, _screen_height * 0.1f, Screen.width * 0.7f, _screen_height * 0.8f);

        region_x = _download_list_bg_rect.x;
        region_y = _download_list_bg_rect.y;
        region_width = _download_list_bg_rect.width;
        region_height = _download_list_bg_rect.height;


        //list title
        _download_list_title_rect.width = _download_list_bg_rect.width;
        _download_list_title_rect.height = 50 * _h_ratio;
        _download_list_title_rect.x = _download_list_bg_rect.x;
        _download_list_title_rect.y = _download_list_bg_rect.y + 30 * _h_ratio;


        //Scroll View
        _download_list_scroll_view_rect = new Rect();
        _download_list_scroll_view_rect.x = region_x + (40 * _w_ratio);
        _download_list_scroll_view_rect.y = region_y + (90 * _h_ratio);
        _download_list_scroll_view_rect.width = region_width - (75 * _w_ratio);
        _download_list_scroll_view_rect.height = region_height - (110 * _h_ratio);

        _download_list_scroll_content_rect = new Rect(0, 0, 100, 100);
        _download_list_scroll_content_rect.width = region_width - (90 * _w_ratio);
        _download_list_scroll_content_rect.height = _sel_script_list.Count * (50 * _h_ratio) + (60 * _h_ratio);


        //스크롤 기능 구현을 위해 추가
        _download_list_scroll_check_rect.x = _download_list_scroll_view_rect.x;
        _download_list_scroll_check_rect.y = _download_list_scroll_view_rect.y;
        _download_list_scroll_check_rect.width = _download_list_scroll_view_rect.width - (40 * _w_ratio);
        _download_list_scroll_check_rect.height = _download_list_scroll_view_rect.height;


        //임시 더미 Rect  객체
        _download_list_btn_rect = new Rect();
        _download_list_btn_rect.x = 40 * _w_ratio;
        _download_list_btn_rect.y = 0;
        _download_list_btn_rect.width = _download_list_scroll_content_rect.width - (60 * _w_ratio);
        _download_list_btn_rect.height = 40 * _h_ratio;
    }


    void SetupMenuButtons()
    {
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


        //흰색 바탕 버튼 오렌지 폰트 스타일
        _btn_styles[3] = new GUIStyle();
        //_btn_styles[3].normal.background = V2.Utils.MakeTex(2, 2, new Color(1, 1, 1), false);
        //_btn_styles[3].hover.background = _ICON_TEXTURES[2];
        //_btn_styles[3].active.background = _ICON_TEXTURES[3];
        _btn_styles[3].alignment = TextAnchor.MiddleLeft;
        _btn_styles[3].font = _FONTS[0];
        _btn_styles[3].fontStyle = FontStyle.Bold;
        _btn_styles[3].normal.textColor = V2.Utils.ToFloatColor(233, 113, 66);
        _btn_styles[3].fontSize = (int)(24 * _w_ratio);


        //스크립트 추가 아이콘
        _btn_styles[4] = new GUIStyle();
        _btn_styles[4].normal.background = _ICON_TEXTURES[2];
        _btn_styles[4].hover.background = _ICON_TEXTURES[2];
        _btn_styles[4].active.background = _ICON_TEXTURES[3];



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

        //스크립트 delte 버튼
        _btn_styles[7] = new GUIStyle();
        _btn_styles[7].normal.background = _ICON_TEXTURES[8];
        _btn_styles[7].hover.background = _ICON_TEXTURES[8];
        _btn_styles[7].active.background = _ICON_TEXTURES[9];

    }



    void SetupTextStyle()
    {
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
        _text_styles[1].fontSize = (int)(38 * _s_ratio);
        _text_styles[1].alignment = TextAnchor.MiddleLeft;
        _text_styles[1].contentOffset = new Vector2(0, 0);


        //bottom stage Title
        _text_styles[2] = new GUIStyle();
        _text_styles[2].font = _FONTS[0];
        _text_styles[2].fontStyle = FontStyle.Bold;
        _text_styles[2].normal.textColor = V2.Utils.ToFloatColor(255, 255, 255);
        _text_styles[2].fontSize = (int)(26 * _s_ratio);
        _text_styles[2].alignment = TextAnchor.MiddleCenter;
        _text_styles[2].contentOffset = new Vector2(0, 0);


        //small bubble
        _text_styles[3] = new GUIStyle();
        _text_styles[3].font = _FONTS[0];
        _text_styles[3].fontStyle = FontStyle.Bold;
        _text_styles[3].normal.textColor = V2.Utils.ToFloatColor(255, 255, 255);
        _text_styles[3].fontSize = (int)(18 * _s_ratio);
        _text_styles[3].alignment = TextAnchor.MiddleCenter;
        _text_styles[3].contentOffset = new Vector2(0, 0);


        //Script list Item Name
        _text_styles[4] = new GUIStyle();
        _text_styles[4].font = _FONTS[0];
        _text_styles[4].fontStyle = FontStyle.Normal;
        _text_styles[4].normal.background = V2.Utils.MakeTex(4, 4, Color.white, false);
        _text_styles[4].hover.background = V2.Utils.MakeTex(4, 4, Color.white, false);
        _text_styles[4].active.background = V2.Utils.MakeTex(4, 4, Color.white, false);
        _text_styles[4].normal.textColor = V2.Utils.ToFloatColor(120, 120, 120);
        _text_styles[4].hover.textColor = V2.Utils.ToFloatColor(150, 150, 150);
        _text_styles[4].active.textColor = V2.Utils.ToFloatColor(150, 100, 0);
        _text_styles[4].fontSize = (int)(18 * _s_ratio);
        _text_styles[4].alignment = TextAnchor.MiddleLeft;
        _text_styles[4].contentOffset = new Vector2(0, 0);


        //Download Title
        _text_styles[5] = new GUIStyle();
        _text_styles[5].font = _FONTS[0];
        _text_styles[5].fontStyle = FontStyle.Bold;
        _text_styles[5].normal.textColor = Color.black;
        _text_styles[5].fontSize = (int)(38 * _s_ratio);
        _text_styles[5].alignment = TextAnchor.UpperCenter;
        _text_styles[5].contentOffset = new Vector2(0, 0);
    }



    void SetupGUIStyle()
    {
        #region SetupGUIStyle

        //Header BACK Button
        _header_btn_styles[0] = new GUIStyle();
        _header_btn_styles[0].normal.background = _HEADER_TEXTURES[0];
        _header_btn_styles[0].hover.background = _HEADER_TEXTURES[0];
        _header_btn_styles[0].active.background = _HEADER_TEXTURES[1];

        //Header Download Button
        _header_btn_styles[1] = new GUIStyle();
        _header_btn_styles[1].normal.background = _HEADER_TEXTURES[2];
        _header_btn_styles[1].hover.background = _HEADER_TEXTURES[2];
        _header_btn_styles[1].active.background = _HEADER_TEXTURES[3];



        _gui_box_white_bg_styles = new GUIStyle();
        _gui_box_white_bg_styles.border = new RectOffset(50, 50, 50, 50);
        _gui_box_white_bg_styles.normal.background = _TEXTURES[8];


        #endregion SetupGUIStyle
    }



    void SetCurBGNum()
    {
        if (_selected_coding_type == "TrafficSignal")
        {
            _CUR_THUMB_TEXTURE_NUM = 0;
        }
        else if (_selected_coding_type == "ButtonLight")
        {
            _CUR_THUMB_TEXTURE_NUM = 1;
        }
        else if (_selected_coding_type == "PlaySound")
        {
            _CUR_THUMB_TEXTURE_NUM = 2;
        }
        else if (_selected_coding_type == "AutoLight")
        {
            _CUR_THUMB_TEXTURE_NUM = 3;
        }
        else if (_selected_coding_type == "DetectMovingObject")
        {
            _CUR_THUMB_TEXTURE_NUM = 4;
        }
        else if (_selected_coding_type == "SecurityAlert")
        {
            _CUR_THUMB_TEXTURE_NUM = 5;
        }
        else if (_selected_coding_type == "RearDistanceSensing")
        {
            _CUR_THUMB_TEXTURE_NUM = 6;
        }
        else if (_selected_coding_type == "SimpleRobotDriving")
        {
            _CUR_THUMB_TEXTURE_NUM = 7;
        }
        else if (_selected_coding_type == "SCurveDriving")
        {
            _CUR_THUMB_TEXTURE_NUM = 8;
        }
        else if (_selected_coding_type == "ReactionRobot")
        {
            _CUR_THUMB_TEXTURE_NUM = 9;
        }
        else if (_selected_coding_type == "AvoidObstacle")
        {
            _CUR_THUMB_TEXTURE_NUM = 10;
        }
        else if (_selected_coding_type == "AutoParking")
        {
            _CUR_THUMB_TEXTURE_NUM = 11;
        }
        else if (_selected_coding_type == "DetectEdge")
        {
            _CUR_THUMB_TEXTURE_NUM = 12;
        }
        else if (_selected_coding_type == "LineTracer")
        {
            _CUR_THUMB_TEXTURE_NUM = 13;
        }
        else if (_selected_coding_type == "MazeExplorer")
        {
            _CUR_THUMB_TEXTURE_NUM = 14;
        }
        else if (_selected_coding_type == "SecurityAlertVR")
        {
            _CUR_THUMB_TEXTURE_NUM = 15;
        }
        else
        {
            _CUR_THUMB_TEXTURE_NUM = 0;
        }
    }



    void SetCurStageTitle()
    {
        _PRE_BG_TEXTURE_NUM = _CUR_BG_TEXTURE_NUM;

        //배경 그림을 5개 단위로 반복해 준다.
        if (_CUR_THUMB_TEXTURE_NUM == 0 || _CUR_THUMB_TEXTURE_NUM == 5 || _CUR_THUMB_TEXTURE_NUM == 10)
            _CUR_BG_TEXTURE_NUM = 0;
        else if (_CUR_THUMB_TEXTURE_NUM == 1 || _CUR_THUMB_TEXTURE_NUM == 6 || _CUR_THUMB_TEXTURE_NUM == 11)
            _CUR_BG_TEXTURE_NUM = 1;
        else if (_CUR_THUMB_TEXTURE_NUM == 2 || _CUR_THUMB_TEXTURE_NUM == 7 || _CUR_THUMB_TEXTURE_NUM == 12)
            _CUR_BG_TEXTURE_NUM = 2;
        else if (_CUR_THUMB_TEXTURE_NUM == 3 || _CUR_THUMB_TEXTURE_NUM == 8 || _CUR_THUMB_TEXTURE_NUM == 13)
            _CUR_BG_TEXTURE_NUM = 3;
        else if (_CUR_THUMB_TEXTURE_NUM == 4 || _CUR_THUMB_TEXTURE_NUM == 9 || _CUR_THUMB_TEXTURE_NUM == 14 || _CUR_THUMB_TEXTURE_NUM == 15)
            _CUR_BG_TEXTURE_NUM = 4;



        if (_CUR_THUMB_TEXTURE_NUM == 0)
        {
            _SELECTED_SCENARIO_NO = "01";
            _SELECTED_SCENARIO_NAME = "TRAFFIC SIGNAL";
        }
        else if (_CUR_THUMB_TEXTURE_NUM == 1)
        {
            _SELECTED_SCENARIO_NO = "02";
            _SELECTED_SCENARIO_NAME = "BUTTON LIGHT";
        }
        else if (_CUR_THUMB_TEXTURE_NUM == 2)
        {
            _SELECTED_SCENARIO_NO = "03";
            _SELECTED_SCENARIO_NAME = "PLAY SOUND";
        }
        else if (_CUR_THUMB_TEXTURE_NUM == 3)
        {
            _SELECTED_SCENARIO_NO = "04";
            _SELECTED_SCENARIO_NAME = "AUTO LIGHT";
        }
        else if (_CUR_THUMB_TEXTURE_NUM == 4)
        {
            _SELECTED_SCENARIO_NO = "05";
            _SELECTED_SCENARIO_NAME = "DETECT MOVING OBJECT";
        }
        else if (_CUR_THUMB_TEXTURE_NUM == 5)
        {
            _SELECTED_SCENARIO_NO = "06";
            _SELECTED_SCENARIO_NAME = "SECURITY ALERT";
        }
        else if (_CUR_THUMB_TEXTURE_NUM == 6)
        {
            _SELECTED_SCENARIO_NO = "07";
            _SELECTED_SCENARIO_NAME = "REAR DISTANCE SENSING";
        }
        else if (_CUR_THUMB_TEXTURE_NUM == 7)
        {
            _SELECTED_SCENARIO_NO = "08";
            _SELECTED_SCENARIO_NAME = "SIMPLE ROBOT DRIVING";
        }
        else if (_CUR_THUMB_TEXTURE_NUM == 8)
        {
            _SELECTED_SCENARIO_NO = "09";
            _SELECTED_SCENARIO_NAME = "S CURVE DRIVING";
        }
        else if (_CUR_THUMB_TEXTURE_NUM == 9)
        {
            _SELECTED_SCENARIO_NO = "10";
            _SELECTED_SCENARIO_NAME = "REACTION ROBOT";
        }
        else if (_CUR_THUMB_TEXTURE_NUM == 10)
        {
            _SELECTED_SCENARIO_NO = "11";
            _SELECTED_SCENARIO_NAME = "AVOID OBSTACLE";
        }
        else if (_CUR_THUMB_TEXTURE_NUM == 11)
        {
            _SELECTED_SCENARIO_NO = "12";
            _SELECTED_SCENARIO_NAME = "AUTO PARKING";
        }
        else if (_CUR_THUMB_TEXTURE_NUM == 12)
        {
            _SELECTED_SCENARIO_NO = "13";
            _SELECTED_SCENARIO_NAME = "DETECT EDGE";
        }
        else if (_CUR_THUMB_TEXTURE_NUM == 13)
        {
            _SELECTED_SCENARIO_NO = "14";
            _SELECTED_SCENARIO_NAME = "LINE TRACER";
        }
        else if (_CUR_THUMB_TEXTURE_NUM == 14)
        {
            _SELECTED_SCENARIO_NO = "15";
            _SELECTED_SCENARIO_NAME = "MAZE EXPLORER";
        }
        else if (_CUR_THUMB_TEXTURE_NUM == 15)
        {
            _SELECTED_SCENARIO_NO = "16";
            _SELECTED_SCENARIO_NAME = "SECURITY ALERT VR";
        }
        else
        {
            _SELECTED_SCENARIO_NO = "00";
            _SELECTED_SCENARIO_NAME = "NEW FILE";
        }


        LoadStageScriptList();
    }



    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

    private void LoadStageScriptList()
    {
        //스크롤바 위치를 초기화해 준다.
        _scroll_pos_script_list.x = 0;
        _scroll_pos_script_list.y = 0;


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



    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
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


        //############################
        //############################
        Calc_Thumb();
        //############################

    }


    void Start_Calc_Thumb()
    {
        Backup_Thumb_Slide();
        _calc_time = 0;
        _calc_flag = true;
    }

    void Backup_Thumb_Slide()
    {
        for (int i = 0; i < 16; i++)
        {
            _pre_slide_pos[i] = _thumb_slide_pos[i];
            _pre_slide_scale[i] = _thumb_slide_scale[i];
        }
    }


    private bool _calc_flag = false;
    private float _calc_time = 0;

    void Calc_Thumb()
    {
        if (_calc_flag)
        {
            _calc_time = _calc_time + Time.deltaTime * 1.5f ;

            if (_calc_time > 1)
                _calc_time = 1.0f;

            for (int i = 0; i <= 6; i++)
            {
                int t_ind = _CUR_THUMB_TEXTURE_NUM + i - 3;

                if (t_ind < 0)
                    t_ind = 16 + t_ind;

                if (t_ind >= 16)
                    t_ind = t_ind - 16;


                _thumb_slide_pos[t_ind] = Vector2.Lerp(_pre_slide_pos[t_ind], _target_slide_pos[i], _calc_time);
                _thumb_slide_scale[t_ind] = Vector2.Lerp(_pre_slide_scale[t_ind], _target_slide_scale[i], _calc_time);
            }

            if (_calc_time > 0.99f)
            {
                _calc_flag = false;
                _calc_time = 1.0f;
            }
        }
    }



    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    void OnGUI()
    {

        if (_first_set_flag == false)
        {
            //GUI 설정
            GUI.skin.verticalScrollbar.fixedWidth = 10 * _w_ratio;
            GUI.skin.verticalScrollbarThumb.fixedWidth = 10 * _w_ratio;
            GUI.skin.verticalScrollbarThumb.fixedHeight = 20 * _h_ratio;

            GUI.skin.verticalScrollbar.normal.background = V2.Utils.MakeTex(10, 10, V2.Utils.ToFloatColor(200, 200, 200), false);
            GUI.skin.verticalScrollbarThumb.normal.background = V2.Utils.MakeTex(10, 10, V2.Utils.ToFloatColor(180, 180, 180), false);

            _first_set_flag = true;
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



        if (_dialog_name == "MainMenu")
        {
            GUI_MainMenu();
        }
        else if (_dialog_name == "Close")
        {
            GUI_Close();
        }
        else if (_dialog_name == "Download")
        {
            GUI_Download();
        }
        else if (_dialog_name == "ListDownload")
        {
            GUI_ListDownload();
        }



        //################################################
        //화면 터치 스크롤 기능 구현
        //################################################
        if (_dialog_name == "MainMenu" || _dialog_name == "ListDownload")
            CheckTouchScroll();
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
            else if (_dialog_name == "Download")
            {
                //이전의 화면으로 돌아간다,

                //##########################################            
                _fadeScreenLibrary.RequestFadeOut("MainMenu", true);
                //##########################################                
            }
            else if (_dialog_name == "ListDownload")
            {
                //이전의 화면으로 돌아간다,

                //##########################################            
                _fadeScreenLibrary.RequestFadeOut("MainMenu", true);
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
        if (fade_res == "MainMenu")
        {
            _dialog_name = "MainMenu";

            //다시 화면을 밝게 처리한다.
            _fadeScreenLibrary.StartFadeIn(true);
        }
        else if (fade_res == "Scene_CodeEditor")
        {
            _dialog_name = string.Empty;
            _dialog_msg = string.Empty;

            UnityEngine.SceneManagement.SceneManager.LoadScene("Scene_CodeEditor");
        }
        else if (fade_res == "Download")
        {
            _dialog_name = "Download";

            //다시 화면을 밝게 처리한다.
            _fadeScreenLibrary.StartFadeIn(true);
        }
        else if (fade_res == "ListDownload")
        {
            _dialog_name = "ListDownload";

            _dialog_msg = string.Empty;

            //다시 화면을 밝게 처리한다.
            _fadeScreenLibrary.StartFadeIn(true);
        }
        else if (fade_res == "Close")
        {
            _dialog_name = "Close";

            //다시 화면을 밝게 처리한다.
            _fadeScreenLibrary.StartFadeIn(true);
        }
        else if (fade_res == "Quit")
        {
            _dialog_name = "";
            _key_down_code = "";
            Application.Quit();
        }
    }

    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@



    //#############################################################################################
    //#############################################################################################
    //    Touch Scroll
    //#############################################################################################
    //#############################################################################################


    #region CheckTouchScroll()

    bool _TOUCH_DOWN_FLAG = false;
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
        //현재 애니메이션 중일 때에는 처리하지 않음
        if (_calc_flag)
            return;


        if (Input.GetMouseButton(0))
        {

            #region

            if (_TOUCH_DOWN_FLAG == false)
            {
                #region

                //눌려 지는 순간
                _FIRST_POS.x = Input.mousePosition.x;
                _FIRST_POS.y = Screen.height - Input.mousePosition.y;

                _PRE_POS.x = Input.mousePosition.x;
                _PRE_POS.y = Screen.height - Input.mousePosition.y;


                //스크립트 영역인지 먼저 체크
                if (_dialog_name != "ListDownload" && _script_list_scroll_check_rect.Contains(_FIRST_POS))
                {
                    _TOUCH_REGION_NAME = "SCRIPT_LIST";
                }
                else if (_dialog_name == "ListDownload" && _download_list_scroll_check_rect.Contains(_FIRST_POS))
                {
                    _TOUCH_REGION_NAME = "LIST_DOWNLOAD";
                }
                else if (_dialog_name != "ListDownload" && _stage_region_rect.Contains(_FIRST_POS))
                {
                    _TOUCH_REGION_NAME = "SCREEN";
                }

                #endregion
            }
            else
            {
                if (!string.IsNullOrEmpty(_TOUCH_REGION_NAME))
                {
                    #region

                    //드래그 하는 순간
                    _CUR_POS.x = Input.mousePosition.x;
                    _CUR_POS.y = Screen.height - Input.mousePosition.y;

                    _TOUCH_OFFSET_POS.x = _CUR_POS.x - _PRE_POS.x;
                    _TOUCH_OFFSET_POS.y = _CUR_POS.y - _PRE_POS.y;

                    float diff_x_abs = UnityEngine.Mathf.Abs(_TOUCH_OFFSET_POS.x);
                    float diff_y_abs = UnityEngine.Mathf.Abs(_TOUCH_OFFSET_POS.y);


                    if (diff_y_abs > diff_x_abs)
                    {
                        if (_TOUCH_REGION_NAME == "SCRIPT_LIST")
                        {
                            _scroll_pos_script_list.y = _scroll_pos_script_list.y - _TOUCH_OFFSET_POS.y;
                        }
                        else if (_TOUCH_REGION_NAME == "LIST_DOWNLOAD")
                        {
                            _scroll_pos_list_download.y = _scroll_pos_list_download.y - _TOUCH_OFFSET_POS.y;
                        }
                    }
                    else if (diff_x_abs > diff_y_abs && diff_x_abs > (10 * _w_ratio))
                    {
                        //옆으로 이동한 경우 
                        if (_TOUCH_REGION_NAME == "SCREEN")
                        {

                            if (_TOUCH_OFFSET_POS.x < 0)
                            {
                                //Next
                                _CUR_THUMB_TEXTURE_NUM = _CUR_THUMB_TEXTURE_NUM + 1;
                                if (_CUR_THUMB_TEXTURE_NUM >= 16)
                                    _CUR_THUMB_TEXTURE_NUM = 0;

                                SetCurStageTitle();

                                SaveUserSelStage();
                                LoadStageScriptList();

                                Start_Calc_Thumb();
                            }
                            else
                            {
                                //Pre
                                _CUR_THUMB_TEXTURE_NUM = _CUR_THUMB_TEXTURE_NUM - 1;
                                if (_CUR_THUMB_TEXTURE_NUM < 0)
                                    _CUR_THUMB_TEXTURE_NUM = 15;

                                SetCurStageTitle();

                                SaveUserSelStage();
                                LoadStageScriptList();

                                Start_Calc_Thumb();
                            }

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
                _CUR_POS.x = Input.mousePosition.x;
                _CUR_POS.y = Screen.height - Input.mousePosition.y;

                _TOUCH_OFFSET_POS.x = _CUR_POS.x - _FIRST_POS.x;
                _TOUCH_OFFSET_POS.y = _CUR_POS.y - _FIRST_POS.y;

                float diff_x_abs = UnityEngine.Mathf.Abs(_TOUCH_OFFSET_POS.x);
                float diff_y_abs = UnityEngine.Mathf.Abs(_TOUCH_OFFSET_POS.y);


                //#########################################################################################
                //아래 코드는 움직임 모멘텀을 만들거나 작은 움직임을 무시하는 용도로만 사용한다.
                //#########################################################################################

                if (diff_y_abs > diff_x_abs && diff_y_abs > (10 * _h_ratio))
                {

                    if (_TOUCH_REGION_NAME == "SCRIPT_LIST" || _TOUCH_REGION_NAME == "LIST_DOWNLOAD")
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
                    if (_TOUCH_REGION_NAME == "SCRIPT_LIST" || _TOUCH_REGION_NAME == "LIST_DOWNLOAD")
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

                #endregion
            }
            else
            {
                #region

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
                    if (_MOMENTUM_REGION_NAME == "SCRIPT_LIST")
                    {
                        _scroll_pos_script_list.y = _scroll_pos_script_list.y - _TOUCH_MOVE_MOMENTUM_Y;
                    }
                    else if (_MOMENTUM_REGION_NAME == "LIST_DOWNLOAD")
                    {
                        _scroll_pos_list_download.y = _scroll_pos_list_download.y - _TOUCH_MOVE_MOMENTUM_Y;
                    }

                    float redu_ratio = 0.95f - Time.deltaTime;
                    _TOUCH_MOVE_MOMENTUM_Y = _TOUCH_MOVE_MOMENTUM_Y * redu_ratio;
                }

                //######################################################################

                #endregion
            }

            _TOUCH_DOWN_FLAG = false;
            _TOUCH_REGION_NAME = string.Empty;
        }
    }


    #endregion CheckTouchScroll()



    //#################################################################################



    void GUI_Close()
    {
        GUI.DrawTexture(_bg_rects[0], _TEXTURES[0]);
        GUI.DrawTexture(_bg_rects[0], _TEXTURES[1]);

        GUI.Label(_bg_rects[1], "Are you sure you want to close?", _text_styles[0]);


        //No
        if (GUI.Button(_button_rects[0], _BTN_TITLE_NO, _btn_styles[1]))
        {
            _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);	//button sound

            //##########################################            
            _fadeScreenLibrary.RequestFadeOut(_pre_dialog_name, true);
            //##########################################            
        }


        //Yes
        if (GUI.Button(_button_rects[1], _BTN_TITLE_YES, _btn_styles[2]))
        {
            _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);	//button sound

            //##########################################            
            _fadeScreenLibrary.RequestFadeOut("Quit", true);
            //##########################################            
        }
    }



    void GUI_MainMenu()
    {
        if (_calc_time < 1f)
        {
            //이전 배경과 오버랩 시켜 준다.
            //먼저 이전 상태의 배경을 그려준다.
            GUI.DrawTexture(_bg_rects[0], _BG_TEXTURES[_PRE_BG_TEXTURE_NUM]);

            //계산시 반투명 처리
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, _calc_time);
            GUI.DrawTexture(_bg_rects[0], _BG_TEXTURES[_CUR_BG_TEXTURE_NUM]);
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 1.0f);
        }
        else
            GUI.DrawTexture(_bg_rects[0], _BG_TEXTURES[_CUR_BG_TEXTURE_NUM]);

        Draw_Header();

        Draw_Thumb();


        Draw_Script_List();

        if (_calc_flag == false)
            Draw_Slide_Button();

    }



    void Draw_Header()
    {

        if (Application.systemLanguage == SystemLanguage.Korean)
        {
            _text_styles[1].font = _FONTS[1];
            GUI.Label(_header_title_rects, "스테이지를 선택하세요", _text_styles[1]);
        }
        else if (Application.systemLanguage == SystemLanguage.English)
        {
            //Title
            GUI.Label(_header_title_rects, "PLEASE SELECT STAGE", _text_styles[1]);
        }
        


        //BACK Button
        if (GUI.Button(_header_btn_rects[0], "", _header_btn_styles[0]))
        {
            _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f); //button sound

            //현재의 화면 이름을 저장한다.
            _pre_dialog_name = _dialog_name;

            //##########################################            
            _fadeScreenLibrary.RequestFadeOut("Close", true);
            //##########################################     

            ////##########################################            
            //_fadeScreenLibrary.RequestFadeOut("Back", true);
            //    //##########################################            
        }

        //Download Button
        if (GUI.Button(_header_btn_rects[1], "", _header_btn_styles[1]))
        {
            _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);	//button sound

            //현재의 화면 이름을 저장한다.
            _pre_dialog_name = _dialog_name;

            _USER_ID = PlayerPrefs.GetString("_USER_ID");

            //##########################################            
            _fadeScreenLibrary.RequestFadeOut("Download", true);
            //##########################################            
        }

    }


    void Draw_Thumb()
    {
        for (int i = 5; i >= 1; i--)
        {
            int t_ind = _CUR_THUMB_TEXTURE_NUM + i - 3;

            if (t_ind < 0)
                t_ind = 16 + t_ind;

            if (t_ind >= 16)
                t_ind = t_ind - 16;

            _stage_dumy_rect.x = _thumb_slide_pos[t_ind].x;
            _stage_dumy_rect.y = _thumb_slide_pos[t_ind].y;
            _stage_dumy_rect.width = _thumb_slide_scale[t_ind].x;
            _stage_dumy_rect.height = _thumb_slide_scale[t_ind].y;

            GUI.DrawTexture(_stage_dumy_rect, _THUMB_TEXTURES[t_ind]); 
        }


        //Small Bubble

        //_small_bubble_region_rect.width = 250 * _w_ratio * _calc_time;
        //_small_bubble_region_rect.height = 160 * _h_ratio * _calc_time;
        //_small_bubble_region_rect.x = _thumb_slide_pos[_CUR_THUMB_TEXTURE_NUM].x - (50 * _w_ratio);
        //_small_bubble_region_rect.y = _thumb_slide_pos[_CUR_THUMB_TEXTURE_NUM].y - (50 * _h_ratio) + ((1 - _calc_time) * 100 * _h_ratio);

        //_text_styles[3].fontSize = (int)(18 * _s_ratio * _calc_time);


        //GUI.DrawTexture(_small_bubble_region_rect, _TEXTURES[3 + _CUR_BG_TEXTURE_NUM]);


        //if (_calc_time > 0.9f)
        //{
        //    //현재 스테이지의 설명을 표시한다
        //    GUI.Label(_small_bubble_region_rect, _stage_desc_list[_CUR_THUMB_TEXTURE_NUM], _text_styles[3]);            
        //}

    }


    void Draw_Slide_Button()
    {

        GUI.Label(_stage_bottom_title_rect, _SELECTED_SCENARIO_NO + ". " + _SELECTED_SCENARIO_NAME, _text_styles[2]);


        //Pre Button
        if (GUI.Button(_pre_arrow_rect, "", _slide_btn_styles[0]))
        {
            _CUR_THUMB_TEXTURE_NUM = _CUR_THUMB_TEXTURE_NUM - 1;
            if (_CUR_THUMB_TEXTURE_NUM < 0)
                _CUR_THUMB_TEXTURE_NUM = 15;

            SetCurStageTitle();

            SaveUserSelStage();
            LoadStageScriptList();

            Start_Calc_Thumb();
        }


        //Next Button
        if (GUI.Button(_next_arrow_rect, "", _slide_btn_styles[1]))
        {
            _CUR_THUMB_TEXTURE_NUM = _CUR_THUMB_TEXTURE_NUM + 1;
            if (_CUR_THUMB_TEXTURE_NUM >= 16)
                _CUR_THUMB_TEXTURE_NUM = 0;

            SetCurStageTitle();

            SaveUserSelStage();
            LoadStageScriptList();

            Start_Calc_Thumb();
        }

    }


    void Draw_Script_List()
    {
        GUI.DrawTexture(_bubble_right_rect, _TEXTURES[2]);


        //계산시 투명 처리
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, _calc_time);



        string CreateScriptName= "Create Script";

        if (Application.systemLanguage == SystemLanguage.Korean)
        {
            _btn_styles[3].font = _FONTS[1];
            CreateScriptName = "스크립트 생성";
        }
        

        if (GUI.Button(_create_script_btn_rect, CreateScriptName, _btn_styles[3]))
        {
            _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);	//button sound

            SaveUserSelStage();

            Create_NewScriptName();

            //##########################################            
            _fadeScreenLibrary.RequestFadeOut("Scene_CodeEditor", true);
            //##########################################
        }

        if (GUI.Button(_create_icon_rect, "", _btn_styles[4]))
        {
            _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);	//button sound

            SaveUserSelStage();

            Create_NewScriptName();

            //##########################################            
            _fadeScreenLibrary.RequestFadeOut("Scene_CodeEditor", true);
            //##########################################
        }


        //######################################################################

        //높이를 다시 계산
        _script_list_scroll_content_rect.height = _sel_script_list.Count * (50 * _h_ratio) + (60 * _h_ratio);

        _scroll_pos_script_list = GUI.BeginScrollView(_script_list_scroll_view_rect, _scroll_pos_script_list, _script_list_scroll_content_rect, false, true);

        for (int i = 0; i < _sel_script_list.Count; i++)
        {
            _script_list_btn_rect.y = (1 + (i * 45)) * _h_ratio;
            _script_delte_btn_rect.y = (10 + (i * 45)) * _h_ratio;

            string user_script_name = _sel_script_list[i];


            if (_remove_icon_clicked)
            {
                GUI.Label(_script_list_btn_rect, user_script_name, _text_styles[4]);
            }
            else
            {
                if (GUI.Button(_script_list_btn_rect, user_script_name, _text_styles[4]) && !_remove_icon_clicked)
                {
                    if (_IS_DRAGGING_MODE)
                        _IS_DRAGGING_MODE = false;
                    else
                    {
                        _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);

                        SaveUserSelStage();
                        GlobalVariables.CurrentFileName = user_script_name;
                        UpdateUserScriptList();

                        //##########################################            
                        _fadeScreenLibrary.RequestFadeOut("Scene_CodeEditor", true);
                        //##########################################
                    }
                }
            }


            if (GUI.Button(_script_delte_btn_rect, "", _btn_styles[7]) && !_remove_icon_clicked)
            {
                _sel_delete_num = i;
                _remove_icon_clicked = true;
            }
        }

        GUI.EndScrollView();

        if (_remove_icon_clicked)
        {
            Draw_Delete(_sel_delete_num);
        }

        //######################################################################



        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 1.0f);

    }

    void Draw_Delete(int SelNum)
    {
        //반투명 BG
        GUI.DrawTexture(_bg_rects[0], _TEXTURES[1]);

        //메세지
        GUI.Label(_bg_rects[1], "Are you sure you want to delete?", _text_styles[0]);

        //No
        GUI.Label(_button_rects[0], _BTN_TITLE_NO, _btn_styles[1]);

        //Yes
        GUI.Label(_button_rects[1], _BTN_TITLE_YES, _btn_styles[2]);


        //No
        if (GUI.Button(_button_rects[0], _BTN_TITLE_NO, _btn_styles[1]))
        {
            _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);


            _sel_delete_num = -1;
            _remove_icon_clicked = false;
        }


        //Yes
        if (GUI.Button(_button_rects[1], _BTN_TITLE_YES, _btn_styles[2]))
        {
            _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);

            //실제스크립트 지우기
            string remove_target_file = _sel_script_list[_sel_delete_num];

            string user_script_path = Application.persistentDataPath + "/" + GlobalVariables.SelectedCodingType + "/" + remove_target_file + ".txt";

            if (File.Exists(user_script_path))
                File.Delete(user_script_path);

            _sel_script_list.RemoveAt(SelNum);


            //스크립트 리스트 지우고 다시쓰기
            string saved_target_list_name = Application.persistentDataPath + "/" + GlobalVariables.SelectedCodingType + "/SCRIPT_LIST.txt";

            if (File.Exists(saved_target_list_name))
            {
                File.Delete(saved_target_list_name);
            }

            File.WriteAllLines(saved_target_list_name, _sel_script_list.ToArray());



            LoadStageScriptList();

            _sel_delete_num = -1;
            _remove_icon_clicked = false;

        }

    }


    void GUI_Download()
    {
        GUI.DrawTexture(_bg_rects[0], _BG_TEXTURES[_CUR_BG_TEXTURE_NUM]);

        Draw_Thumb();



        //반 투명 BG Box 처리
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 0.8f);

        GUI.Box(_download_input_bg_rect, "", _gui_box_white_bg_styles);

        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 1.0f);



        if (string.IsNullOrEmpty(_dialog_msg))
        {
            #region _dialog_msg normal           

            if (Application.systemLanguage == SystemLanguage.Korean)
            {
                _text_styles[5].font = _FONTS[1];
                GUI.Label(_download_input_title_rect, "Email 주소를 입력해주세요.", _text_styles[5]);
            }
            else if (Application.systemLanguage == SystemLanguage.English)
            {
                //Title
                GUI.Label(_download_input_title_rect, "Please input your email.", _text_styles[5]);
            }

            


            _USER_ID = GUI.TextField(_download_input_box_rect, _USER_ID, _font_text_field_style);

            string CancelText = "Cancel";
            if (Application.systemLanguage == SystemLanguage.Korean)
            {
                _btn_styles[5].font = _FONTS[1];
                CancelText = "취소";
            }

            //No
            if (GUI.Button(_button_rects[0], CancelText, _btn_styles[5]))
            {
                _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);	//button sound

                //##########################################            
                _fadeScreenLibrary.RequestFadeOut(_pre_dialog_name, true);
                //##########################################            
            }

            string ListDownloadText = "List Download";
            if (Application.systemLanguage == SystemLanguage.Korean)
            {
                _btn_styles[6].font = _FONTS[1];
                ListDownloadText = "리스트 다운로드";
            }

            if (GUI.Button(_button_rects[1], ListDownloadText, _btn_styles[6]))
            {
                _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);	//button sound

                PlayerPrefs.SetString("_USER_ID", _USER_ID);

                if (!string.IsNullOrEmpty(_USER_ID))
                {
                    PlayerPrefs.SetString("USER_ID", _USER_ID);

                    _dialog_msg = "Please wait...";
                    if (Application.systemLanguage == SystemLanguage.Korean)
                    {
                        _text_styles[5].font = _FONTS[1];
                        _dialog_msg = "잠시만 기다려주세요...";
                    }

                    string url = "http://bigrense.azurewebsites.net/api/get_script_big.aspx";

                    WWWForm form = new WWWForm();
                    form.AddField("user_id", _USER_ID);
                    form.AddField("prod_type", "VIRTUAL_CODING");
                    form.AddField("cont_type", GlobalVariables.SelectedCodingType);
                    WWW www = new WWW(url, form);

                    StartCoroutine(WaitFor_ListDownload(www));
                }
                else
                {
                    _dialog_msg = "Type email address!";
                    if (Application.systemLanguage == SystemLanguage.Korean)
                    {
                        _text_styles[5].font = _FONTS[1];
                        _dialog_msg = "email 주소를 입력해주세요.";
                    }
                }
                    
            }

            #endregion _dialog_msg normal
        }
        else
        {
            GUI.Label(_download_dialog_title_rect, _dialog_msg, _text_styles[5]);


            if (Application.systemLanguage == SystemLanguage.Korean)
            {
                if (_dialog_msg != "잠시만 기다려주세요...")
                {
                    //OK
                    if (GUI.Button(_button_rects[2], "OK", _btn_styles[6]))
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
                    if (GUI.Button(_button_rects[2], "OK", _btn_styles[6]))
                    {
                        _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f); //button sound

                        _dialog_msg = string.Empty;
                    }
                }
            }
        }
    }



    void GUI_ListDownload()
    {
        GUI.DrawTexture(_bg_rects[0], _BG_TEXTURES[_CUR_BG_TEXTURE_NUM]);

        Draw_Thumb();



        //반 투명 BG Box 처리
        GUI.Box(_download_list_bg_rect, "", _gui_box_white_bg_styles);


        if (string.IsNullOrEmpty(_dialog_msg))
        {
            #region _dialog_msg normal

            string SelectStageText = "Select a file";
            if (Application.systemLanguage == SystemLanguage.Korean)
            {
                _text_styles[5].font = _FONTS[1];
                SelectStageText = "파일을 선택해주세요.";
            }

            GUI.Label(_download_list_title_rect, SelectStageText, _text_styles[5]);

            if (GUI.Button(_header_btn_rects[0], "", _header_btn_styles[0]))
            {
                _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f); //button sound
                _fadeScreenLibrary.RequestFadeOut("MainMenu", true);

            }
            //######################################################################

            //높이를 다시 계산
            _download_list_scroll_content_rect.height = _download_script_list.Count * (50 * _h_ratio) + (60 * _h_ratio);

            _scroll_pos_list_download = GUI.BeginScrollView(_download_list_scroll_view_rect, _scroll_pos_list_download, _download_list_scroll_content_rect, false, true);

            for (int i = 0; i < _download_script_list.Count; i++)
            {
                _download_list_btn_rect.y = (1 + (i * 45)) * _h_ratio;

                string user_script_name = _download_script_list[i];
                string seqno_str = _download_num_list[i];

                if (GUI.Button(_download_list_btn_rect, user_script_name, _text_styles[4]))
                {
                    if (_IS_DRAGGING_MODE)
                        _IS_DRAGGING_MODE = false;
                    else
                    {
                        _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);


                        if (!string.IsNullOrEmpty(seqno_str))
                        {
                            PlayerPrefs.SetString("Uploaded_Script_Number", seqno_str);
                            PlayerPrefs.SetString("NewSavedCodeFileName", user_script_name);

                            _dialog_msg = "Please wait...";
                            if (Application.systemLanguage == SystemLanguage.Korean)
                            {
                                _dialog_msg = "잠시만 기다려주세요.";
                            }

                            string url = "http://bigrense.azurewebsites.net/api/get_script_big.aspx";

                            WWWForm form = new WWWForm();
                            form.AddField("seq_no", seqno_str);
                            WWW www = new WWW(url, form);

                            StartCoroutine(WaitForRequest(www));
                        }
                        else
                        {
                            _dialog_msg = "Can't read  target file";
                            if (Application.systemLanguage == SystemLanguage.Korean)
                            {
                                _dialog_msg = "파일을 읽을 수 없습니다.";
                            }
                        }
                            
                    }
                }
            }

            GUI.EndScrollView();

            //######################################################################

            #endregion _dialog_msg normal
        }
        else
        {
            GUI.Label(_download_dialog_title_rect, _dialog_msg, _text_styles[5]);


            if (_dialog_msg != "Please wait...")
            {
                //OK
                if (GUI.Button(_button_rects[2], "OK", _btn_styles[6]))
                {
                    _dumy_audio_source.GetComponent<AudioSource>().PlayOneShot(_GAME_AUDIO_CLIPS[0], 1.0f);	//button sound

                    _dialog_msg = string.Empty;
                }
            }
        }
    }



    void SaveUserSelStage()
    {
        if (_CUR_THUMB_TEXTURE_NUM == 0)
        {
            GlobalVariables.SelectedCodingType = "TrafficSignal";
        }
        else if (_CUR_THUMB_TEXTURE_NUM == 1)
        {
            GlobalVariables.SelectedCodingType = "ButtonLight";
        }
        else if (_CUR_THUMB_TEXTURE_NUM == 2)
        {
            GlobalVariables.SelectedCodingType = "PlaySound";
        }
        else if (_CUR_THUMB_TEXTURE_NUM == 3)
        {
            GlobalVariables.SelectedCodingType = "AutoLight";
        }
        else if (_CUR_THUMB_TEXTURE_NUM == 4)
        {
            GlobalVariables.SelectedCodingType = "DetectMovingObject";
        }
        else if (_CUR_THUMB_TEXTURE_NUM == 5)
        {
            GlobalVariables.SelectedCodingType = "SecurityAlert";
        }
        else if (_CUR_THUMB_TEXTURE_NUM == 6)
        {
            GlobalVariables.SelectedCodingType = "RearDistanceSensing";
        }
        else if (_CUR_THUMB_TEXTURE_NUM == 7)
        {
            GlobalVariables.SelectedCodingType = "SimpleRobotDriving";
        }
        else if (_CUR_THUMB_TEXTURE_NUM == 8)
        {
            GlobalVariables.SelectedCodingType = "SCurveDriving";
        }
        else if (_CUR_THUMB_TEXTURE_NUM == 9)
        {
            GlobalVariables.SelectedCodingType = "ReactionRobot";
        }
        else if (_CUR_THUMB_TEXTURE_NUM == 10)
        {
            GlobalVariables.SelectedCodingType = "AvoidObstacle";
        }
        else if (_CUR_THUMB_TEXTURE_NUM == 11)
        {
            GlobalVariables.SelectedCodingType = "AutoParking";
        }
        else if (_CUR_THUMB_TEXTURE_NUM == 12)
        {
            GlobalVariables.SelectedCodingType = "DetectEdge";
        }
        else if (_CUR_THUMB_TEXTURE_NUM == 13)
        {
            GlobalVariables.SelectedCodingType = "LineTracer";
        }
        else if (_CUR_THUMB_TEXTURE_NUM == 14)
        {
            GlobalVariables.SelectedCodingType = "MazeExplorer";
        }
        else if (_CUR_THUMB_TEXTURE_NUM == 15)
        {
            GlobalVariables.SelectedCodingType = "SecurityAlertVR";
        }
        else
        {
            GlobalVariables.SelectedCodingType = "TrafficSignal";
        }

    }



    void Create_NewScriptName()
    {
        DateTime cur_dt = DateTime.Now;

        string new_file_name = GlobalVariables.SelectedCodingType + " (";

        new_file_name = new_file_name + cur_dt.Year.ToString();
        new_file_name = new_file_name + "-" + cur_dt.Month.ToString().PadLeft(2, '0');
        new_file_name = new_file_name + "-" + cur_dt.Day.ToString().PadLeft(2, '0');
        new_file_name = new_file_name + " " + cur_dt.Hour.ToString().PadLeft(2, '0');
        new_file_name = new_file_name + "." + cur_dt.Minute.ToString().PadLeft(2, '0');
        new_file_name = new_file_name + "." + cur_dt.Second.ToString().PadLeft(2, '0');

        new_file_name = new_file_name + ")";

        GlobalVariables.CurrentFileName = new_file_name;
        UpdateUserScriptList();
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




    IEnumerator WaitFor_ListDownload(WWW www)
    {
        yield return www;

        if (www.error == null)
        {
            if (!string.IsNullOrEmpty(www.text))
            {
                string title_lines = SPL.Common.Util.RestoreSingleQuatString(www.text);

                if (!string.IsNullOrEmpty(title_lines))
                {
                    string[] title_list = title_lines.Split(new char[] { _RECORD_SEPARATER }, StringSplitOptions.RemoveEmptyEntries);

                    if (title_list != null)
                    {
                        _download_script_list.Clear();
                        _download_num_list.Clear();

                        for (int i = 0; i < title_list.Length; i++)
                        {
                            string[] items = title_list[i].Split(new char[] { _FIELD_SEPARATER }, StringSplitOptions.RemoveEmptyEntries);

                            if (items != null && items.Length == 2)
                            {
                                _download_script_list.Add(items[0]);
                                _download_num_list.Add(items[1]);
                            }
                        }
                    }

                    File.WriteAllText(Application.persistentDataPath + "/" + GlobalVariables.SelectedCodingType + "/DownloadList.txt", title_lines);
                }


                //##########################################            
                _fadeScreenLibrary.RequestFadeOut("ListDownload", true);
                //##########################################      
            }
            else
            {
                _dialog_msg = "None of data";
                if (Application.systemLanguage == SystemLanguage.Korean)
                {
                    _dialog_msg = "데이터가 없습니다.";
                }
            }
        }
        else
        {
            _dialog_msg = "Check internet connection!";
            if (Application.systemLanguage == SystemLanguage.Korean)
            {
                _dialog_msg = "인터넷 연결을 확인해주세요.";
            }

        }
    }



    IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

        if (www.error == null)
        {
            if (!string.IsNullOrEmpty(www.text))
            {
                //코드 목록에 추가한다.
                string user_script_name = PlayerPrefs.GetString("NewSavedCodeFileName");


                string restored_str = SPL.Common.Util.RestoreSingleQuatString(www.text);

                string target_file_name_path = Application.persistentDataPath + "/" + GlobalVariables.SelectedCodingType + "/" + user_script_name + ".txt";



                if (File.Exists(target_file_name_path))
                    File.Delete(target_file_name_path);

                File.WriteAllText(target_file_name_path, restored_str);


                SaveUserSelStage();
                GlobalVariables.CurrentFileName = user_script_name;
                UpdateUserScriptList();

                //##########################################            
                _fadeScreenLibrary.RequestFadeOut("Scene_CodeEditor", true);
                //##########################################

            }
            else
                _dialog_msg = "None of data";
        }
        else
        {
            _dialog_msg = "Check innternet connection!";
        }
    }






}
