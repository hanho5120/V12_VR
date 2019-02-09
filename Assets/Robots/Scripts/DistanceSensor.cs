using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DistanceSensor : MonoBehaviour
{

    public int SensorNum = 0;
    //public bool Connected = false;

    //SerialRead
    //SerialRead _SerialReadInstance = null;


    public static int DistanceSensor0 = 0;
    public static int DistanceSensor1 = 0;
    public static int DistanceSensor2 = 0;
    public static int DistanceSensor3 = 0;
    public static int DistanceSensor4 = 0;
    public static int DistanceSensor5 = 0;
    public Color OriginalColor;

    public float pulseSpeed = 1.5f;
    public float noiseSize = 1.0f;
    public float maxWidth = 0.5f;
    public float minWidth = 0.2f;
    public LineRenderer lRenderer;
    Vector3 RendererPos;

    public Transform CarParent;
    public float ParentScale;

    //2017.11.21
    public string TargetTagStr = string.Empty;
    int LayerMask = (-1)-(1 << 9);
    Scene m_Scene;

    void Start()
    {
        m_Scene = m_Scene = SceneManager.GetActiveScene();
        lRenderer = transform.Find("LineRenderer").GetComponent<LineRenderer>();
        if (lRenderer)
        {
            RendererPos = lRenderer.GetPosition(1);
        }
        CarParent = transform.parent;
        ParentScale = CarParent.localScale.x;
        //_SerialReadInstance = GameObject.Find("GameControlObject").GetComponent<SerialRead>();



    }

    float _last_check_time = 0;

    void Update()
    {


        if (lRenderer)
        {
            float aniFactor = Mathf.PingPong(Time.time * pulseSpeed, 1.0f);
            aniFactor = Mathf.Max(minWidth, aniFactor) * maxWidth;
            lRenderer.startWidth = aniFactor;
            lRenderer.endWidth = aniFactor;
        }



        //if (Connected == false)
        //    return;

        //Check every 100 msec
        if ((Time.fixedTime - _last_check_time) > 0.1f)
        {
            _last_check_time = Time.fixedTime;

            RaycastHit hit = new RaycastHit();

            bool hit_res = false;

            if (m_Scene.name == "Scene_ReactionCar")
            {
                if (SensorNum == 0)
                    hit_res = Physics.Raycast(transform.position, transform.right, out hit, 100, LayerMask);
                else if (SensorNum == 1)
                    hit_res = Physics.Raycast(transform.position, -transform.right, out hit, 100, LayerMask);
                else if (SensorNum == 2)
                    hit_res = Physics.Raycast(transform.position, transform.forward, out hit, 100, LayerMask);
                else if (SensorNum == 3)
                    hit_res = Physics.Raycast(transform.position, -transform.forward, out hit, 100, LayerMask);
                else if (SensorNum == 4)
                    hit_res = Physics.Raycast(transform.position, transform.up, out hit, 100, LayerMask);
                else if (SensorNum == 5)
                    hit_res = Physics.Raycast(transform.position, -transform.up, out hit, 100, LayerMask);
            }
            else
            {
                if (SensorNum == 0)
                    hit_res = Physics.Raycast(transform.position, transform.right, out hit, 100);
                else if (SensorNum == 1)
                    hit_res = Physics.Raycast(transform.position, -transform.right, out hit, 100);
                else if (SensorNum == 2)
                    hit_res = Physics.Raycast(transform.position, transform.forward, out hit, 100);
                else if (SensorNum == 3)
                    hit_res = Physics.Raycast(transform.position, -transform.forward, out hit, 100);
                else if (SensorNum == 4)
                    hit_res = Physics.Raycast(transform.position, transform.up, out hit, 100);
                else if (SensorNum == 5)
                    hit_res = Physics.Raycast(transform.position, -transform.up, out hit, 100);
            }

            //Debug.Log("DistanceSensor: " + hit.distance.ToString());
            //Debug.Log("DistanceSensor: " + hit.transform.gameObject.name);

            //2017.11.20
            if (hit_res)
            {
                if (lRenderer)
                {
                    Vector3 Pos = RendererPos.normalized;
                    lRenderer.SetPosition(1, hit.distance * Pos / ParentScale);
                    if (m_Scene.name == "Scene_SecurityAlertVR")
                    {
                       // SecurityAlertCtrlVR.isCheck = true;
                    }
                }

                Debug.Log(hit.transform.name);
                //Debug.Log("DistanceSensor: " + hit.distance.ToString());

                /*
                int dist = (int)(hit.distance * 10);

                if (dist < 0)
                    dist = 0;

                int dist1 = dist / 100;
                if (dist1 > 255)
                    dist1 = 255;

                int dist2 = dist - (dist1 * 100);
                

                byte[] buf = new byte[6];
                buf[0] = (byte)'<';
                buf[1] = (byte)'S';
                buf[2] = (byte)SensorNum;
                buf[3] = (byte)dist1;
                buf[4] = (byte)dist2;
                buf[5] = (byte)'>';

                //Debug.Log(buf[3] + " / " + buf[4]);
                */

                //0 ~ 255
                //int dist = (int)(hit.distance * 80);

                //2016.11.06
                //5 ~ 10 -> 500 ~ 1000
                //int dist = (int)(hit.distance * 100);
                int dist = (int)(hit.distance * 120);

                if (dist == 0)
                    dist = 1000;

                dist = (int)SPL.Common.Util.map(dist, 10, 1000, 1023, 0);

                if (dist < 0)
                    dist = 0;
                else if (dist > 1023)
                    dist = 1023;

                //if (SensorNum == 0)
                //    Debug.Log(hit.distance + " : " + dist);


                if (SensorNum == 0)
                    DistanceSensor.DistanceSensor0 = dist;
                else if (SensorNum == 1)
                    DistanceSensor.DistanceSensor1 = dist;
                else if (SensorNum == 2)
                    DistanceSensor.DistanceSensor2 = dist;
                else if (SensorNum == 3)
                    DistanceSensor.DistanceSensor3 = dist;
                else if (SensorNum == 4)
                    DistanceSensor.DistanceSensor4 = dist;
                else if (SensorNum == 5)
                    DistanceSensor.DistanceSensor5 = dist;



                //2017.11.21
                if (!string.IsNullOrEmpty(TargetTagStr))
                {
                    if (hit.transform.tag.ToString() == TargetTagStr)
                    {
                        if (SensorNum == 0)
                            DistanceSensor.DistanceSensor0 = 1023;
                        else if (SensorNum == 1)
                            DistanceSensor.DistanceSensor1 = 1023;
                        else if (SensorNum == 2)
                            DistanceSensor.DistanceSensor2 = 1023;
                        else if (SensorNum == 3)
                            DistanceSensor.DistanceSensor3 = 1023;
                        else if (SensorNum == 4)
                            DistanceSensor.DistanceSensor4 = 1023;
                        else if (SensorNum == 5)
                            DistanceSensor.DistanceSensor5 = 1023;
                    }
                    else
                    {
                        if (SensorNum == 0)
                            DistanceSensor.DistanceSensor0 = 0;
                        else if (SensorNum == 1)
                            DistanceSensor.DistanceSensor1 = 0;
                        else if (SensorNum == 2)
                            DistanceSensor.DistanceSensor2 = 0;
                        else if (SensorNum == 3)
                            DistanceSensor.DistanceSensor3 = 0;
                        else if (SensorNum == 4)
                            DistanceSensor.DistanceSensor4 = 0;
                        else if (SensorNum == 5)
                            DistanceSensor.DistanceSensor5 = 0;
                    }
                }




                /*
                byte[] buf = new byte[5];
                buf[0] = (byte)'<';
                buf[1] = (byte)'S';
                buf[2] = (byte)SensorNum;
                buf[3] = (byte)dist;
                buf[4] = (byte)'>';

                Debug.Log(buf[3]);
                */

                //_SerialReadInstance.SendSerialData(buf);                
            }
            else
            {
                if (lRenderer)
                {
                    lRenderer.SetPosition(1, RendererPos);
                }


                if (SensorNum == 0)
                    DistanceSensor.DistanceSensor0 = 0;
                else if (SensorNum == 1)
                    DistanceSensor.DistanceSensor1 = 0;
                else if (SensorNum == 2)
                    DistanceSensor.DistanceSensor2 = 0;
                else if (SensorNum == 3)
                    DistanceSensor.DistanceSensor3 = 0;
                else if (SensorNum == 4)
                    DistanceSensor.DistanceSensor4 = 0;
                else if (SensorNum == 5)
                    DistanceSensor.DistanceSensor5 = 0;
            }
        }

    }
}
