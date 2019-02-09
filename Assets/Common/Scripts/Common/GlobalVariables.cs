using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gv
{
    public class GlobalVariables
    {

        //######################################################################
        //디버깅 또는 테스트 시에만 아래 변수를 true로 설정한다.
        //######################################################################
        public static bool DEBUG_MODE = false;


        public static string SelectedCodingType = string.Empty;
        //public static string SelectedCodingType = "TrafficSignal";
        //public static string SelectedCodingType = "ButtonLight";
        //public static string SelectedCodingType = "AutoLight";
        //public static string SelectedCodingType = "DetectMovingObject"; 
        //public static string SelectedCodingType = "SecurityAlert";
        //public static string SelectedCodingType = "PlaySound";
        //public static string SelectedCodingType = "RearDistanceSensing";
        //public static string SelectedCodingType = "SimpleRobotDriving";
        //public static string SelectedCodingType = "SCurveDriving";
        //public static string SelectedCodingType = "ReactionRobot";
        //public static string SelectedCodingType = "AvoidObstacle";
        //public static string SelectedCodingType = "AutoParking";
        //public static string SelectedCodingType = "DetectEdge";
        //public static string SelectedCodingType = "LineTracer";
        //public static string SelectedCodingType = "MazeExplorer";

        public static object res;

        public static string CurrentFileName = "2018.05.27 PM 02:30";
    }
}