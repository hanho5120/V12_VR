using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveCharacterCtrl : MonoBehaviour
{
    public MainCamControl _MainCamControlInstance = null;
    bool walk_flag = false;
    Rigidbody rigid;
    public Vector3 vel;
    public Vector3 PreValue;
    public Animator Anim;
    CharacterController cc;
    Scene m_Scene;

    // Use this for initialization
    void Start()
    {
        //PreValue = 0.0f;
        cc = GetComponent<CharacterController>();
        rigid = GetComponent<Rigidbody>();
        m_Scene = SceneManager.GetActiveScene();
    }

    // Update is called once per frame
    void Update()
    {
        //ConMovePos.y -= 1000.0f * Time.deltaTime;
        cc.Move(new Vector3(0f,  -1000f * Time.deltaTime, 0f));

        if (transform.name == "Dog_Parent")
        {
            bool event_res_left = _MainCamControlInstance.GetLeftJoystickPressed();
            if (event_res_left)
            {
                if (transform.name == "Character_Parent")
                {
                    Vector2 Movepos = _MainCamControlInstance.GetLeftJoystick();
                    Vector3 ConMovePos = new Vector3(Movepos.x, 0, -Movepos.y);
                    ConMovePos.Normalize();


                    Movement(ConMovePos);
                }
            }

            //강아지 이동
            bool event_res_right = _MainCamControlInstance.GetRightJoystickPressed();
            if (event_res_right)
            {

                Vector2 Movepos = _MainCamControlInstance.GetRightJoystick();
                Vector3 ConMovePos = new Vector3(Movepos.x, 0, -Movepos.y);
                ConMovePos.Normalize();

                Movement(ConMovePos);
                Anim.SetBool("isRun", true);
            }
            else
            {
                Anim.SetBool("isRun", false);
            }
        }


        if (transform.name == "Character_Parent")
        {
            //캐릭터 이동
            bool event_res_left = _MainCamControlInstance.GetLeftJoystickPressed();
            bool isJump = false;

            if (event_res_left)
            {
                Vector2 Movepos = _MainCamControlInstance.GetLeftJoystick();
                
                
                if (m_Scene.name == "Scene_ReactionCar")
                {

                    Vector3 ConMovePos = new Vector3(Movepos.x, 0, -Movepos.y);
                    ConMovePos.Normalize();
                    Quaternion v3Rotation = Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f);
                    //이동할 벡터를 돌린다.
                    ConMovePos = v3Rotation * ConMovePos;
                    

                    RaycastHit hit = new RaycastHit();

                    bool hit_res = false;
                    
                    hit_res = Physics.Raycast(transform.position, ConMovePos, out hit, 100);

                    Vector3 cur_pos = transform.localPosition;



                    if (hit_res && hit.transform.name == "ChildBlock" && hit.distance < 0.5f)
                    {
                      
                         isJump = true;
                        
                    }
                    else
                    {

                        Vector3 ConMovePos1 = new Vector3(Movepos.x, 0, -Movepos.y);
                        ConMovePos1.Normalize();

                        Movement(ConMovePos1);

                    }
                }
                else
                {
                   
                    Vector3 ConMovePos = new Vector3(Movepos.x, 0, -Movepos.y);
                    ConMovePos.Normalize();


                    Movement(ConMovePos);
                }

                if (!isJump && Movepos.y != 0)
                {
                    Anim.SetBool("isRun", true);
                    Anim.SetBool("isJump", false);
                }
                else if (isJump && Movepos.y != 0)
                {
                    Anim.SetBool("isRun", false);
                    Anim.SetBool("isJump", true);
                }
                else
                {
                    Anim.SetBool("isRun", false);
                    Anim.SetBool("isJump", false);
                }
                
            }
            else
            {
                Anim.SetBool("isJump", false);
                Anim.SetBool("isRun", false);
            }

            bool event_res_right = _MainCamControlInstance.GetRightJoystickPressed();
            if (event_res_right)
            {

            }
        }
    }

    public Vector3 Movement(Vector3 ConMovePos)
    {
        Quaternion v3Rotation = Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f);
        //이동할 벡터를 돌린다.
        ConMovePos = v3Rotation * ConMovePos;


        transform.GetChild(0).transform.eulerAngles = new Vector3(0, GetRot(ConMovePos), 0);
        //transform.transform.Translate(ConMovePos.x * Time.deltaTime, 0, ConMovePos.z * Time.deltaTime, Space.Self);

        //ConMovePos.y -= 1000.0f * Time.deltaTime;
        if (m_Scene.name == "Scene_ReactionCar")
        {
            cc.Move(ConMovePos * 1.5f * Time.deltaTime);
        }
        else
        {
            cc.Move(ConMovePos * 2f * Time.deltaTime);
        }
            
        //            transform.transform.Translate(ConMovePos.x * Time.deltaTime, 0, ConMovePos.z * Time.deltaTime, Space.Self);

        //anim.Play("walk");
        walk_flag = true;

        return ConMovePos;

    }

    public void MovementCar(Vector3 ConMovePos, float Length)
    {

    }

    float GetRot(Vector3 ConMovePos)
    {
        float dx = transform.localPosition.x + ConMovePos.x - transform.localPosition.x;

        float dz = transform.localPosition.z + ConMovePos.z - transform.localPosition.z;

        float rad = Mathf.Atan2(dx, dz);

        float degree = rad * Mathf.Rad2Deg;

        return degree;
    }

}
