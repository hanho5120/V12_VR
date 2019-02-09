using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class MoveDogCtrlVR : MonoBehaviour
{
    public MainCamControl _MainCamControlInstance = null;
    bool walk_flag = false;
    Rigidbody rigid;
    public Vector3 vel;
    public Vector3 PreValue;
    public Animator Anim;
    CharacterController cc;
    Scene m_Scene;
    NavMeshAgent agent;
    public GameObject DogPasswordPaper;
    public GameObject PasswordPaper;

    // Use this for initialization
    void Start()
    {
        //PreValue = 0.0f;
        cc = GetComponent<CharacterController>();
        rigid = GetComponent<Rigidbody>();
        m_Scene = SceneManager.GetActiveScene();
        agent = GetComponent<NavMeshAgent>();
        DogPasswordPaper.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {



        //강아지 이동
        //bool event_res_right = _MainCamControlInstance.GetRightJoystickPressed();
        //if (event_res_right)
        //{

        //    Vector2 Movepos = _MainCamControlInstance.GetRightJoystick();
        //    Vector3 ConMovePos = new Vector3(Movepos.x, 0, -Movepos.y);
        //    ConMovePos.Normalize();

        //    Movement(ConMovePos);
        //    Anim.SetBool("isRun", true);
        //}
        //else
        //{
        //    Anim.SetBool("isRun", false);
        //}

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            Anim.SetBool("isRun", false);
        }







    }

    void SetGoal(Vector3 Pos)
    {
        agent.destination = Pos;
        Anim.SetBool("isRun", true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "PasswordBox_MainCtrl")
        {
            DogPasswordPaper.SetActive(true);
            PasswordPaper.SetActive(false);
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
