using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using SPL.Common;
using eval = SPL.Evaluator;


//public class GUICmdItem
//{
//    public string Name = "";
//    public string GUICmd = "";
//    public Rect Pos_Rect = new Rect(100, 100, 300, 50);
//    public string Text = "";
//    public bool Visible = true;
//    public string OnClick = "";
//    public string OnPressed = "";
//    public string OnKeyPressed = "";
//    public string OnKeyDown = "";
//    public string OnHorizontalKey = "";
//    public string OnVerticalKey = "";

//    //Joystict
//    public string OnLeftButtonClick = "";
//    public string OnRightButtonClick = "";
//    public string OnLeftButtonPressed = "";
//    public string OnRightButtonPressed = "";
//    public string OnLeftJoystick = "";
//    public string OnRightJoystick = "";

//    //2017.11.19
//    public string OnLeftJoystickClick = "";
//    public string OnRightJoystickClick = "";
//    public string OnLeftJoystickPressed = "";
//    public string OnRightJoystickPressed = "";



//    public Texture TargetTexture = null;

//    public string BackgroundColorStr = "";
//    public Texture2D BackgroundColor = null;
//    public Color TextColor = Color.white;
//    public int TextSize = 24;
//    public FontStyle TextFontStyle;
//    public TextAnchor Alignment;
//    public bool WordWrap = false;

//    public GUICmdItem(string name, string guiCmd, Vector2 pos, Vector2 size, string text, string action_event)
//    {
//        Name = name;
//        GUICmd = guiCmd;
//        Pos_Rect.x = pos.x;
//        Pos_Rect.y = pos.y;
//        Pos_Rect.width = size.x;
//        Pos_Rect.height = size.y;

//        Text = text;

//        if (guiCmd == "addbutton")
//            OnClick = action_event;
//    }

//    public void SetVisible(bool visible)
//    {
//        this.Visible = visible;
//    }
//}


public class EngineHelper : MonoBehaviour
{


    public delegate void LogInfoHandler(string log);
    public LogInfoHandler EvalLogInfoHandler = null;

    public Dictionary<object, object> _procedureTreeInstanceList = null;
    public Dictionary<object, object> _EventProcedureMappingList = null;

    public ArrayList _variableTypeNameList = null;
    public Dictionary<object, object> _GlobalVariables = null;
    public ArrayList _ProcedureList = new ArrayList();

    public Dictionary<object, object> _stringToIntMapping = new Dictionary<object, object>();


    public EngineHelper()
    {

    }


    public void LogInfo(string log)
    {
        if (log == null)
            return;


        if (EvalLogInfoHandler != null)
        {
            EvalLogInfoHandler(log);
        }
    }



    //public void ReplaceWithVariablesForParams(CommandItem cmdItem_Clone, Dictionary<object, object> localVariables, eval.Evaluator evaluator)
    //{
    //    foreach (string paramKey in cmdItem_Clone.Params.Keys)
    //    {
    //        string paramData = cmdItem_Clone.Params[paramKey].ToString();

    //        try
    //        {
    //            string replacedStr = ReplaceWithVariables(localVariables, paramData, evaluator);

    //            if (replacedStr != paramData)
    //                cmdItem_Clone.Params[paramKey] = replacedStr;
    //        }
    //        catch { }
    //    }
    //}

    public IEnumerator ReplaceWithVariables(Dictionary<object, object> localVariables, string srcText, eval.Evaluator evaluator)
    {
        string resStr = srcText;



        int startIndexOfVariable = resStr.IndexOf('{');
        int endIndexOfVariable = resStr.IndexOf('}');

        while (startIndexOfVariable >= 0 && endIndexOfVariable >= 0 && endIndexOfVariable > (startIndexOfVariable + 1))
        {
            string varStr = resStr.Substring(startIndexOfVariable + 1, endIndexOfVariable - startIndexOfVariable - 1);
            string srcStr = "{" + varStr + "}";

            yield return StartCoroutine(ExcuteConditionExprWithLocal(varStr, localVariables, evaluator));
            string targetStr = gv.GlobalVariables.res.ToString();
            //string targetStr = ExcuteConditionExprWithLocal(varStr, localVariables, evaluator).ToString();

            resStr = Util.Replace(resStr, srcStr, targetStr);

            startIndexOfVariable = resStr.IndexOf('{');
            endIndexOfVariable = resStr.IndexOf('}');
        }


        //return resStr;
        gv.GlobalVariables.res = resStr;
    }

    public string ReplaceWithVariables_Origin(Dictionary<object, object> localVariables, string srcText, eval.Evaluator evaluator)
    {
        string resStr = srcText;

        try
        {

            int startIndexOfVariable = resStr.IndexOf('{');
            int endIndexOfVariable = resStr.IndexOf('}');

            while (startIndexOfVariable >= 0 && endIndexOfVariable >= 0 && endIndexOfVariable > (startIndexOfVariable + 1))
            {
                string varStr = resStr.Substring(startIndexOfVariable + 1, endIndexOfVariable - startIndexOfVariable - 1);
                string srcStr = "{" + varStr + "}";

                string targetStr = ExcuteConditionExprWithLocal(varStr, localVariables, evaluator).ToString();

                resStr = Util.Replace(resStr, srcStr, targetStr);

                startIndexOfVariable = resStr.IndexOf('{');
                endIndexOfVariable = resStr.IndexOf('}');
            }
        }
        catch (Exception ex)
        {
            LogInfo("[ReplaceWithVariables] " + ex.ToString());
        }

        return resStr;
    }




    #region ParseDefVariableLineWithLocal

    public IEnumerator ParseDefVariableLineWithLocal(string firstToken, string line, LocalVariableItem localVariableItem, string procedureName, eval.Evaluator _evaluator)
    {
       
            _evaluator.LocalVariables = localVariableItem.LocalVariables;

            string newLine = line;

            newLine = newLine.Substring(firstToken.Length, newLine.Length - firstToken.Length);
            newLine = newLine.Trim();


            string variableName = string.Empty;
            object assignedVal = null;

            if (newLine != string.Empty)
            {
                string[] tagArr;
                tagArr = newLine.Split(new Char[] { '=' });

                if (tagArr[0] != string.Empty)
                {
                    //2016.09.25
                    string new_first_token = firstToken;

                    if (firstToken == "unsigned")
                    {
                        string[] sub_token = tagArr[0].Trim().Split(new char[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        new_first_token = firstToken + " " + sub_token[0].Trim();
                        variableName = sub_token[1].Trim();
                    }
                    else
                        variableName = tagArr[0].Trim();

                    //LogInfo("firstToken: " + new_first_token);
                    //LogInfo("variableName: " + variableName);


                    //2016.09.25 switch -> if
                    //switch (firstToken)
                    //{
                    if (new_first_token == "hashtable")
                    {
                        assignedVal = new Dictionary<object, object>();
                    }
                    else if (new_first_token == "dictionary")
                    {
                        assignedVal = new Dictionary<object, object>();
                    }
                    else if (new_first_token == "arraylist")
                    {
                        assignedVal = new ArrayList();
                    }
                    else if (new_first_token == "queue")
                    {
                        assignedVal = new Queue();
                    }
                    else if (new_first_token == "stack")
                    {
                        assignedVal = new Stack();
                    }
                    else if (new_first_token == "stringbuilder")
                    {
                        assignedVal = new StringBuilder();
                    }
                    else if (new_first_token == "random")
                    {
                        assignedVal = new System.Random();
                    }
                    else if (new_first_token == "int")
                    {
                        assignedVal = (int)0;

                        //2016.09.25 배열 처리 추가
                        if (tagArr.Length == 1 && !variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            assignedVal = 0;
                        }
                        else if (tagArr.Length == 2 && !variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            if (tagArr[1] != string.Empty)
                            {
                                string paramVal = tagArr[1].Trim();

                                //object result = _evaluator.Evaluate(paramVal);
                                yield return StartCoroutine(_evaluator.Evaluate(paramVal));
                                object result = gv.GlobalVariables.res;

                                if (result != null)
                                    assignedVal = Util.ToInt(result);
                                else
                                    LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + paramVal + " is null");

                            }
                        }
                        else if (tagArr.Length == 1 && variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            int s_p = variableName.IndexOf("[");
                            int e_p = variableName.IndexOf("]");
                            string arr_size = variableName.Substring(s_p + 1, (e_p - s_p - 1));

                            variableName = variableName.Substring(0, variableName.IndexOf("["));

                            if (!string.IsNullOrEmpty(arr_size))
                            {
                            yield return StartCoroutine(_evaluator.Evaluate(arr_size));
                            object result = gv.GlobalVariables.res;
                            //object result = _evaluator.Evaluate(arr_size);

                                if (result != null)
                                {
                                    int[] dumy_arr = new int[Util.ToInt(result)];
                                    for (int i = 0; i < dumy_arr.Length; i++)
                                        dumy_arr[i] = 0;

                                    assignedVal = dumy_arr;
                                }
                                else
                                    LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + arr_size + " is null");
                            }
                            else
                            {
                                LogInfo("Array should have size. " + variableName);
                            }
                        }
                        else if (tagArr.Length == 2 && variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            variableName = variableName.Substring(0, variableName.IndexOf("["));

                            string paramVal = tagArr[1].Trim().Trim(';').TrimStart('{').TrimEnd('}');

                            string[] result = paramVal.Split(new char[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                            if (result != null)
                            {
                                int[] dumy_arr = new int[result.Length];

                                for (int i = 0; i < result.Length; i++)
                                    dumy_arr[i] = Util.ToInt(result[i]);

                                assignedVal = dumy_arr;
                            }
                            else
                                LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + paramVal + " is null");
                        }
                    }
                    else if (new_first_token == "char")
                    {
                        assignedVal = '\0';

                        /*
                        if (tagArr.Length == 2)
                        {
                            if (tagArr[1] != string.Empty)
                            {
                                string paramVal = tagArr[1].Trim();
								
                                object result = _evaluator.Evaluate(paramVal);
								
                                if (result != null)
                                    assignedVal = Util.ToChar(result);
                                else
                                    LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + paramVal + " is null");
								
                            }
                        }
                        */


                        //2016.09.25 배열 처리 추가
                        if (tagArr.Length == 1 && !variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            assignedVal = '\0';
                        }
                        else if (tagArr.Length == 2 && !variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            if (tagArr[1] != string.Empty)
                            {
                                string paramVal = tagArr[1].Trim();

                            yield return StartCoroutine(_evaluator.Evaluate(paramVal));
                            object result = gv.GlobalVariables.res;
                            //object result = _evaluator.Evaluate(paramVal);

                                if (result != null)
                                    assignedVal = Util.ToChar(result);
                                else
                                    LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + paramVal + " is null");

                            }
                        }
                        else if (tagArr.Length == 1 && variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            int s_p = variableName.IndexOf("[");
                            int e_p = variableName.IndexOf("]");
                            string arr_size = variableName.Substring(s_p + 1, (e_p - s_p - 1));

                            variableName = variableName.Substring(0, variableName.IndexOf("["));

                            if (!string.IsNullOrEmpty(arr_size))
                            {
                            yield return StartCoroutine(_evaluator.Evaluate(arr_size));
                            object result = gv.GlobalVariables.res;
                            //object result = _evaluator.Evaluate(arr_size);

                                if (result != null)
                                {
                                    char[] dumy_arr = new char[Util.ToInt(result)];
                                    for (int i = 0; i < dumy_arr.Length; i++)
                                        dumy_arr[i] = '\0';

                                    assignedVal = dumy_arr;
                                }
                                else
                                    LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + arr_size + " is null");
                            }
                            else
                            {
                                LogInfo("Array should have size. " + variableName);
                            }
                        }
                        else if (tagArr.Length == 2 && variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            variableName = variableName.Substring(0, variableName.IndexOf("["));

                            string paramVal = tagArr[1].Trim().Trim(';').TrimStart('{').TrimEnd('}');

                            string[] result = paramVal.Split(new char[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                            if (result != null)
                            {
                                char[] dumy_arr = new char[result.Length];

                                for (int i = 0; i < result.Length; i++)
                                    dumy_arr[i] = Util.ToChar(result[i]);

                                assignedVal = dumy_arr;
                            }
                            else
                                LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + paramVal + " is null");
                        }

                    }
                    else if (new_first_token == "byte")
                    {
                        assignedVal = (byte)0;

                        /*
                        if (tagArr.Length == 2)
                        {
                            if (tagArr[1] != string.Empty)
                            {
                                string paramVal = tagArr[1].Trim();
								
                                object result = _evaluator.Evaluate(paramVal);
								
                                if (result != null)
                                    assignedVal = Util.ToByte(result);
                                else
                                    LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + paramVal + " is null");
                            }
                        }
                        */

                        //2016.09.25 배열 처리 추가
                        if (tagArr.Length == 1 && !variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            assignedVal = (byte)0;
                        }
                        else if (tagArr.Length == 2 && !variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            if (tagArr[1] != string.Empty)
                            {
                                string paramVal = tagArr[1].Trim();

                            yield return StartCoroutine(_evaluator.Evaluate(paramVal));
                            object result = gv.GlobalVariables.res;
                            //object result = _evaluator.Evaluate(paramVal);

                                if (result != null)
                                    assignedVal = Util.ToByte(result);
                                else
                                    LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + paramVal + " is null");

                            }
                        }
                        else if (tagArr.Length == 1 && variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            int s_p = variableName.IndexOf("[");
                            int e_p = variableName.IndexOf("]");
                            string arr_size = variableName.Substring(s_p + 1, (e_p - s_p - 1));

                            variableName = variableName.Substring(0, variableName.IndexOf("["));

                            if (!string.IsNullOrEmpty(arr_size))
                            {
                            yield return StartCoroutine(_evaluator.Evaluate(arr_size));
                            object result = gv.GlobalVariables.res;
                            //object result = _evaluator.Evaluate(arr_size);

                                if (result != null)
                                {
                                    byte[] dumy_arr = new byte[Util.ToInt(result)];
                                    for (int i = 0; i < dumy_arr.Length; i++)
                                        dumy_arr[i] = 0;

                                    assignedVal = dumy_arr;
                                }
                                else
                                    LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + arr_size + " is null");
                            }
                            else
                            {
                                LogInfo("Array should have size. " + variableName);
                            }
                        }
                        else if (tagArr.Length == 2 && variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            variableName = variableName.Substring(0, variableName.IndexOf("["));

                            string paramVal = tagArr[1].Trim().Trim(';').TrimStart('{').TrimEnd('}');

                            string[] result = paramVal.Split(new char[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                            if (result != null)
                            {
                                byte[] dumy_arr = new byte[result.Length];

                                for (int i = 0; i < result.Length; i++)
                                    dumy_arr[i] = Util.ToByte(result[i]);

                                assignedVal = dumy_arr;
                            }
                            else
                                LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + paramVal + " is null");
                        }
                    }
                    else if (new_first_token == "long")
                    {
                        assignedVal = (long)0;

                        /*
                        if (tagArr.Length == 2)
                        {
                            if (tagArr[1] != string.Empty)
                            {
                                string paramVal = tagArr[1].Trim();
								
                                object result = _evaluator.Evaluate(paramVal);
								
                                if (result != null)
                                    assignedVal = Util.ToLong(result);
                                else
                                    LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + paramVal + " is null");
                            }
                        }
                        */

                        //2016.09.25 배열 처리 추가
                        if (tagArr.Length == 1 && !variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            assignedVal = (long)0;
                        }
                        else if (tagArr.Length == 2 && !variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            if (tagArr[1] != string.Empty)
                            {
                                string paramVal = tagArr[1].Trim();

                            yield return StartCoroutine(_evaluator.Evaluate(paramVal));
                            object result = gv.GlobalVariables.res;
                            //object result = _evaluator.Evaluate(paramVal);

                                if (result != null)
                                    assignedVal = Util.ToLong(result);
                                else
                                    LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + paramVal + " is null");

                            }
                        }
                        else if (tagArr.Length == 1 && variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            int s_p = variableName.IndexOf("[");
                            int e_p = variableName.IndexOf("]");
                            string arr_size = variableName.Substring(s_p + 1, (e_p - s_p - 1));

                            variableName = variableName.Substring(0, variableName.IndexOf("["));

                            if (!string.IsNullOrEmpty(arr_size))
                            {
                                //object result = _evaluator.Evaluate(arr_size);
                            yield return StartCoroutine(_evaluator.Evaluate(arr_size));
                            object result = gv.GlobalVariables.res;

                            if (result != null)
                                {
                                    long[] dumy_arr = new long[Util.ToInt(result)];
                                    for (int i = 0; i < dumy_arr.Length; i++)
                                        dumy_arr[i] = 0;

                                    assignedVal = dumy_arr;
                                }
                                else
                                    LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + arr_size + " is null");
                            }
                            else
                            {
                                LogInfo("Array should have size. " + variableName);
                            }
                        }
                        else if (tagArr.Length == 2 && variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            variableName = variableName.Substring(0, variableName.IndexOf("["));

                            string paramVal = tagArr[1].Trim().Trim(';').TrimStart('{').TrimEnd('}');

                            string[] result = paramVal.Split(new char[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                            if (result != null)
                            {
                                long[] dumy_arr = new long[result.Length];

                                for (int i = 0; i < result.Length; i++)
                                    dumy_arr[i] = Util.ToLong(result[i]);

                                assignedVal = dumy_arr;
                            }
                            else
                                LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + paramVal + " is null");
                        }
                    }
                    else if (new_first_token == "unsigned long")
                    {
                        assignedVal = (ulong)0;

                        /*
                        if (tagArr.Length == 2)
                        {
                            if (tagArr[1] != string.Empty)
                            {
                                string paramVal = tagArr[1].Trim();

                                object result = _evaluator.Evaluate(paramVal);

                                if (result != null)
                                    assignedVal = Util.ToLong(result);
                                else
                                    LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + paramVal + " is null");
                            }
                        }
                        */

                        //2016.09.25 배열 처리 추가
                        if (tagArr.Length == 1 && !variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            assignedVal = (ulong)0;
                        }
                        else if (tagArr.Length == 2 && !variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            if (tagArr[1] != string.Empty)
                            {
                                string paramVal = tagArr[1].Trim();

                            yield return StartCoroutine(_evaluator.Evaluate(paramVal));
                            object result = gv.GlobalVariables.res;
                            //object result = _evaluator.Evaluate(paramVal);

                                if (result != null)
                                    assignedVal = Util.ToULong(result);
                                else
                                    LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + paramVal + " is null");

                            }
                        }
                        else if (tagArr.Length == 1 && variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            int s_p = variableName.IndexOf("[");
                            int e_p = variableName.IndexOf("]");
                            string arr_size = variableName.Substring(s_p + 1, (e_p - s_p - 1));

                            variableName = variableName.Substring(0, variableName.IndexOf("["));

                            if (!string.IsNullOrEmpty(arr_size))
                            {
                                //object result = _evaluator.Evaluate(arr_size);
                            yield return StartCoroutine(_evaluator.Evaluate(arr_size));
                            object result = gv.GlobalVariables.res;

                            if (result != null)
                                {
                                    ulong[] dumy_arr = new ulong[Util.ToInt(result)];
                                    for (int i = 0; i < dumy_arr.Length; i++)
                                        dumy_arr[i] = (ulong)0;

                                    assignedVal = dumy_arr;
                                }
                                else
                                    LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + arr_size + " is null");
                            }
                            else
                            {
                                LogInfo("Array should have size. " + variableName);
                            }
                        }
                        else if (tagArr.Length == 2 && variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            variableName = variableName.Substring(0, variableName.IndexOf("["));

                            string paramVal = tagArr[1].Trim().Trim(';').TrimStart('{').TrimEnd('}');

                            string[] result = paramVal.Split(new char[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                            if (result != null)
                            {
                                ulong[] dumy_arr = new ulong[result.Length];

                                for (int i = 0; i < result.Length; i++)
                                    dumy_arr[i] = Util.ToULong(result[i]);

                                assignedVal = dumy_arr;
                            }
                            else
                                LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + paramVal + " is null");
                        }
                    }
                    else if (new_first_token == "float")
                    {
                        assignedVal = 0.0f;

                        /*
                        if (tagArr.Length == 2)
                        {
                            if (tagArr[1] != string.Empty)
                            {
                                string paramVal = tagArr[1].Trim();
								
                                object result = _evaluator.Evaluate(paramVal);
								
                                if (result != null)
                                    assignedVal = Util.ToFloat(result);
                                else
                                    LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + paramVal + " is null");
                            }
                        }
                        */

                        //2016.09.25 배열 처리 추가
                        if (tagArr.Length == 1 && !variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            assignedVal = 0.0f;
                        }
                        else if (tagArr.Length == 2 && !variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            if (tagArr[1] != string.Empty)
                            {
                                string paramVal = tagArr[1].Trim();

                            yield return StartCoroutine(_evaluator.Evaluate(paramVal));
                            object result = gv.GlobalVariables.res;
                            //object result = _evaluator.Evaluate(paramVal);

                                if (result != null)
                                    assignedVal = Util.ToFloat(result);
                                else
                                    LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + paramVal + " is null");

                            }
                        }
                        else if (tagArr.Length == 1 && variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            int s_p = variableName.IndexOf("[");
                            int e_p = variableName.IndexOf("]");
                            string arr_size = variableName.Substring(s_p + 1, (e_p - s_p - 1));

                            variableName = variableName.Substring(0, variableName.IndexOf("["));

                            if (!string.IsNullOrEmpty(arr_size))
                            {
                                //object result = _evaluator.Evaluate(arr_size);
                            yield return StartCoroutine(_evaluator.Evaluate(arr_size));
                            object result = gv.GlobalVariables.res;

                            if (result != null)
                                {
                                    float[] dumy_arr = new float[Util.ToInt(result)];
                                    for (int i = 0; i < dumy_arr.Length; i++)
                                        dumy_arr[i] = 0.0f;

                                    assignedVal = dumy_arr;
                                }
                                else
                                    LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + arr_size + " is null");
                            }
                            else
                            {
                                LogInfo("Array should have size. " + variableName);
                            }
                        }
                        else if (tagArr.Length == 2 && variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            variableName = variableName.Substring(0, variableName.IndexOf("["));

                            string paramVal = tagArr[1].Trim().Trim(';').TrimStart('{').TrimEnd('}');

                            string[] result = paramVal.Split(new char[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                            if (result != null)
                            {
                                float[] dumy_arr = new float[result.Length];

                                for (int i = 0; i < result.Length; i++)
                                    dumy_arr[i] = Util.ToFloat(result[i]);

                                assignedVal = dumy_arr;
                            }
                            else
                                LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + paramVal + " is null");
                        }
                    }
                    else if (new_first_token == "double")
                    {
                        assignedVal = 0.0;

                        /*
                        if (tagArr.Length == 2)
                        {
                            if (tagArr[1] != string.Empty)
                            {
                                string paramVal = tagArr[1].Trim();
								
                                object result = _evaluator.Evaluate(paramVal);
								
                                if (result != null)
                                    assignedVal = Util.ToDouble(result);
                                else
                                    LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + paramVal + " is null");
                            }
                        }
                        */

                        //2016.09.25 배열 처리 추가
                        if (tagArr.Length == 1 && !variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            assignedVal = 0.0;
                        }
                        else if (tagArr.Length == 2 && !variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            if (tagArr[1] != string.Empty)
                            {
                                string paramVal = tagArr[1].Trim();

                                //object result = _evaluator.Evaluate(paramVal);
                            yield return StartCoroutine(_evaluator.Evaluate(paramVal));
                            object result = gv.GlobalVariables.res;

                            if (result != null)
                                    assignedVal = Util.ToDouble(result);
                                else
                                    LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + paramVal + " is null");

                            }
                        }
                        else if (tagArr.Length == 1 && variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            int s_p = variableName.IndexOf("[");
                            int e_p = variableName.IndexOf("]");
                            string arr_size = variableName.Substring(s_p + 1, (e_p - s_p - 1));

                            variableName = variableName.Substring(0, variableName.IndexOf("["));

                            if (!string.IsNullOrEmpty(arr_size))
                            {
                                //object result = _evaluator.Evaluate(arr_size);
                            yield return StartCoroutine(_evaluator.Evaluate(arr_size));
                            object result = gv.GlobalVariables.res;

                            if (result != null)
                                {
                                    double[] dumy_arr = new double[Util.ToInt(result)];
                                    for (int i = 0; i < dumy_arr.Length; i++)
                                        dumy_arr[i] = 0.0;

                                    assignedVal = dumy_arr;
                                }
                                else
                                    LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + arr_size + " is null");
                            }
                            else
                            {
                                LogInfo("Array should have size. " + variableName);
                            }
                        }
                        else if (tagArr.Length == 2 && variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            variableName = variableName.Substring(0, variableName.IndexOf("["));

                            string paramVal = tagArr[1].Trim().Trim(';').TrimStart('{').TrimEnd('}');

                            string[] result = paramVal.Split(new char[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                            if (result != null)
                            {
                                double[] dumy_arr = new double[result.Length];

                                for (int i = 0; i < result.Length; i++)
                                    dumy_arr[i] = Util.ToDouble(result[i]);

                                assignedVal = dumy_arr;
                            }
                            else
                                LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + paramVal + " is null");
                        }
                    }
                    else if (new_first_token == "datetime")
                    {
                        assignedVal = new DateTime();

                        if (tagArr.Length == 2)
                        {
                            if (tagArr[1] != string.Empty)
                            {
                                string paramVal = tagArr[1].Trim();

                                paramVal = paramVal.TrimStart((char)(34));
                                paramVal = paramVal.TrimEnd((char)(34));

                            //object result = _evaluator.Evaluate(paramVal);
                            yield return StartCoroutine(_evaluator.Evaluate(paramVal));
                            object result = gv.GlobalVariables.res;

                            if (result != null)
                                    assignedVal = Util.ToDateTime(result);
                                else
                                    LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + paramVal + " is null");
                            }
                        }

                    }
                    else if (new_first_token == "string" || new_first_token == "String")
                    {
                        assignedVal = string.Empty;

                        /*
                        if (tagArr.Length == 2)
                        {
                            if (tagArr[1] != string.Empty)
                            {
                                string paramVal = tagArr[1].Trim();
								
                                object result = _evaluator.Evaluate(paramVal);
								
                                if (result != null)
                                    assignedVal = result.ToString();
                                else
                                    LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + paramVal + " is null");
                            }
                        }
                        */

                        //2016.09.25 배열 처리 추가
                        if (tagArr.Length == 1 && !variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            assignedVal = string.Empty;
                        }
                        else if (tagArr.Length == 2 && !variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            if (tagArr[1] != string.Empty)
                            {
                                string paramVal = tagArr[1].Trim();

                                //object result = _evaluator.Evaluate(paramVal);

                            yield return StartCoroutine(_evaluator.Evaluate(paramVal));
                            object result = gv.GlobalVariables.res;

                            if (result != null)
                                    assignedVal = result.ToString();
                                else
                                    LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + paramVal + " is null");

                            }
                        }
                        else if (tagArr.Length == 1 && variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            int s_p = variableName.IndexOf("[");
                            int e_p = variableName.IndexOf("]");
                            string arr_size = variableName.Substring(s_p + 1, (e_p - s_p - 1));

                            variableName = variableName.Substring(0, variableName.IndexOf("["));

                            if (!string.IsNullOrEmpty(arr_size))
                            {
                                //object result = _evaluator.Evaluate(arr_size);
                            yield return StartCoroutine(_evaluator.Evaluate(arr_size));
                            object result = gv.GlobalVariables.res;

                            if (result != null)
                                {
                                    string[] dumy_arr = new string[Util.ToInt(result)];
                                    for (int i = 0; i < dumy_arr.Length; i++)
                                        dumy_arr[i] = string.Empty;

                                    assignedVal = dumy_arr;
                                }
                                else
                                    LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + arr_size + " is null");
                            }
                            else
                            {
                                LogInfo("Array should have size. " + variableName);
                            }
                        }
                        else if (tagArr.Length == 2 && variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            variableName = variableName.Substring(0, variableName.IndexOf("["));

                            string paramVal = tagArr[1].Trim().Trim(';').TrimStart('{').TrimEnd('}');

                            string[] result = paramVal.Split(new char[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                            if (result != null)
                            {
                                string[] dumy_arr = new string[result.Length];

                                for (int i = 0; i < result.Length; i++)
                                    dumy_arr[i] = result[i].ToString();

                                assignedVal = dumy_arr;
                            }
                            else
                                LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + paramVal + " is null");
                        }
                    }
                    else if (new_first_token == "bool" || new_first_token == "boolean" || new_first_token == "Boolean")
                    {
                        assignedVal = false;

                        /*
                        if (tagArr.Length == 2)
                        {
                            if (tagArr[1] != string.Empty)
                            {
                                string paramVal = tagArr[1].Trim();
								
                                object result = _evaluator.Evaluate(paramVal);
								
                                if (result != null)
                                    assignedVal = Util.ToBool(result);
                                else
                                    LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + paramVal + " is null");
                            }
                        }
                        */

                        //2016.09.25 배열 처리 추가
                        if (tagArr.Length == 1 && !variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            assignedVal = false;
                        }
                        else if (tagArr.Length == 2 && !variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            if (tagArr[1] != string.Empty)
                            {
                                string paramVal = tagArr[1].Trim();

                            yield return StartCoroutine(_evaluator.Evaluate(paramVal));
                            object result = gv.GlobalVariables.res;
                            //object result = _evaluator.Evaluate(paramVal);

                                if (result != null)
                                    assignedVal = Util.ToBool(result);
                                else
                                    LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + paramVal + " is null");

                            }
                        }
                        else if (tagArr.Length == 1 && variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            int s_p = variableName.IndexOf("[");
                            int e_p = variableName.IndexOf("]");
                            string arr_size = variableName.Substring(s_p + 1, (e_p - s_p - 1));

                            variableName = variableName.Substring(0, variableName.IndexOf("["));

                            if (!string.IsNullOrEmpty(arr_size))
                            {
                                //object result = _evaluator.Evaluate(arr_size);
                            yield return StartCoroutine(_evaluator.Evaluate(arr_size));
                            object result = gv.GlobalVariables.res;

                            if (result != null)
                                {
                                    bool[] dumy_arr = new bool[Util.ToInt(result)];
                                    for (int i = 0; i < dumy_arr.Length; i++)
                                        dumy_arr[i] = false;

                                    assignedVal = dumy_arr;
                                }
                                else
                                    LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + arr_size + " is null");
                            }
                            else
                            {
                                LogInfo("Array should have size. " + variableName);
                            }
                        }
                        else if (tagArr.Length == 2 && variableName.Contains("["))
                        {
                            //2016.09.25 배열 처리 추가
                            variableName = variableName.Substring(0, variableName.IndexOf("["));

                            string paramVal = tagArr[1].Trim().Trim(';').TrimStart('{').TrimEnd('}');

                            string[] result = paramVal.Split(new char[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                            if (result != null)
                            {
                                bool[] dumy_arr = new bool[result.Length];

                                for (int i = 0; i < result.Length; i++)
                                    dumy_arr[i] = Util.ToBool(result[i]);

                                assignedVal = dumy_arr;
                            }
                            else
                                LogInfo("[ParseDefVariableLineWithLocal] " + "Variable Exception:" + paramVal + " is null");
                        }
                    }
                    else if (new_first_token == "object")
                    {
                        assignedVal = new object();

                        if (tagArr.Length == 2)
                        {
                            if (tagArr[1] != string.Empty)
                            {
                                string paramVal = tagArr[1].Trim();

                                //object result = _evaluator.Evaluate(paramVal);
                            yield return StartCoroutine(_evaluator.Evaluate(paramVal));
                            object result = gv.GlobalVariables.res;

                            assignedVal = result;
                            }
                        }
                    }
                    else if (new_first_token == "var")
                    {
                        assignedVal = new object();

                        if (tagArr.Length == 2)
                        {
                            if (tagArr[1] != string.Empty)
                            {
                                string paramVal = tagArr[1].Trim();

                                //object result = _evaluator.Evaluate(paramVal);
                            yield return StartCoroutine(_evaluator.Evaluate(paramVal));
                            object result = gv.GlobalVariables.res;

                            assignedVal = result;
                            }
                        }

                    }
                    //}


                    if (procedureName == "maindefault")
                    {
                        try
                        {
                            if (!_GlobalVariables.ContainsKey(variableName))
                            {
                                _GlobalVariables.Add(variableName, assignedVal);
                            }
                            else
                            {
                                _GlobalVariables[variableName] = assignedVal;

                            }
                        }
                        catch { }
                    }
                    else
                    {
                        try
                        {
                            if (!_evaluator.LocalVariables.ContainsKey(variableName))
                            {
                                Debug.Log(assignedVal.GetType().Name);
                                string a = assignedVal.GetType().Name;
                                _evaluator.LocalVariables.Add(variableName, assignedVal);

                                localVariableItem.TempLocalVariableList.Add(variableName);
                            }
                            else
                            {
                                _evaluator.LocalVariables[variableName] = assignedVal;

                            }
                        }
                        catch { }
                    }

                }
            }
     

    }


    #endregion ParseDefVariableLineWithLocal




    #region ParseSetVariableLineWithLocal


    public string GetVariableName(string line, out string dimensionExpr1, out int dimensions)
    {
        int startInd = -1;
        int endInd = -1;
        int openCount = 0;

        dimensionExpr1 = string.Empty;
        dimensions = 0;

        try
        {
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '[')
                {
                    if (startInd == -1)
                        startInd = i;

                    openCount++;
                }

                if (line[i] == ']')
                {
                    openCount--;

                    if (openCount == 0 && endInd == -1)
                        endInd = i;
                }
            }

            if (startInd >= 0 && endInd > 0 && endInd > startInd)
            {
                string arrayExpr = line.Substring(startInd + 1, endInd - startInd - 1);

                string[] tagArr = arrayExpr.Split(new Char[] { ',' });

                if (tagArr.Length > 0)
                {
                    if (tagArr[0].Trim() != string.Empty)
                    {
                        dimensionExpr1 = tagArr[0].Trim();
                        dimensions = 1;
                    }
                }
                return line.Substring(0, startInd);
            }
            else
            {
                return line;
            }
        }
        catch (Exception ex)
        {
            LogInfo("[GetVariableName] " + ex.ToString());
        }

        return string.Empty;
    }

    public IEnumerator ParseSetVariableLineWithLocal(string line, Dictionary<object, object> localVariables, string procedureName, eval.Evaluator _evaluator, string procitem_Target = "")
    {

        //LogInfo("[ParseSetVariableLineWithLocal ### " + line);

        _evaluator.LocalVariables = localVariables;


        string newLine = line;

        if (line.IndexOf("return ") == 0)
        {
            #region return

            if (newLine.Length > 6)
            {
                string returnExpr = newLine.Substring(6, newLine.Length - 6);
                returnExpr = returnExpr.Trim();


                int rInd = returnExpr.IndexOf("++");
                if (rInd > 0)
                {
                    string varName = returnExpr.Substring(0, rInd);
                    returnExpr = varName + " = " + varName + " + 1";
                }

                rInd = returnExpr.IndexOf("--");
                if (rInd > 0)
                {
                    string varName = returnExpr.Substring(0, rInd);
                    returnExpr = varName + " = " + varName + " - 1";
                }

                rInd = returnExpr.IndexOf("+=");
                if (rInd > 0 && returnExpr.Length > (rInd + 2))
                {
                    string varName1 = returnExpr.Substring(0, rInd);
                    string varName2 = returnExpr.Substring(rInd + 2, (returnExpr.Length - rInd - 2));
                    returnExpr = varName1 + " = " + varName1 + " + " + varName2;
                }

                rInd = returnExpr.IndexOf("-=");
                if (rInd > 0 && returnExpr.Length > (rInd + 2))
                {
                    string varName1 = returnExpr.Substring(0, rInd);
                    string varName2 = returnExpr.Substring(rInd + 2, (returnExpr.Length - rInd - 2));
                    returnExpr = varName1 + " = " + varName1 + " - " + varName2;
                }




                //OutResult res = new OutResult();
                object result;
                yield return StartCoroutine(_evaluator.Evaluate(returnExpr));
                result = gv.GlobalVariables.res;

                bool IsAvailable = true;
                //IsAvailable = ConvertingType(result, procitem_Target);

                if (IsAvailable)
                {
                    if (_GlobalVariables.ContainsKey(procedureName + "_return_value"))
                    {
                        _GlobalVariables[procedureName + "_return_value"] = result;

                    }
                    else
                    {
                        _GlobalVariables.Add(procedureName + "_return_value", result);

                    }
                }



            }

            #endregion return
        }
        else
        {

            int rInd = newLine.IndexOf("++");
            if (rInd > 0)
            {
                string varName = newLine.Substring(0, rInd);
                newLine = varName + " = " + varName + " + 1";
            }

            rInd = newLine.IndexOf("--");
            if (rInd > 0)
            {
                string varName = newLine.Substring(0, rInd);
                newLine = varName + " = " + varName + " - 1";
            }

            rInd = newLine.IndexOf("+=");
            if (rInd > 0 && newLine.Length > (rInd + 2))
            {
                string varName1 = newLine.Substring(0, rInd);
                string varName2 = newLine.Substring(rInd + 2, (newLine.Length - rInd - 2));
                newLine = varName1 + " = " + varName1 + " + " + varName2;
            }

            rInd = newLine.IndexOf("-=");
            if (rInd > 0 && newLine.Length > (rInd + 2))
            {
                string varName1 = newLine.Substring(0, rInd);
                string varName2 = newLine.Substring(rInd + 2, (newLine.Length - rInd - 2));
                newLine = varName1 + " = " + varName1 + " - " + varName2;
            }



            string[] tagArr;
            tagArr = newLine.Split(new Char[] { '=' });


            if (tagArr.Length == 2)
            {

                #region if (tagArr.Length == 2)

                string variableName = tagArr[0].Trim();
                string expr = tagArr[1].Trim();

                string dimensionExpr1 = string.Empty;
                int dimensions = 0;


                variableName = GetVariableName(variableName, out dimensionExpr1, out dimensions);


                int arrayIndex1 = -1;

                if (dimensionExpr1 != string.Empty)
                {
                   
                        //object result = _evaluator.Evaluate(dimensionExpr1, _brickService);
                        yield return StartCoroutine(_evaluator.Evaluate(dimensionExpr1));
                        object result = gv.GlobalVariables.res;
                        //object result = _evaluator.Evaluate(dimensionExpr1);
                        arrayIndex1 = Util.ToInt(result);
                  
                }

                if (variableName != string.Empty && expr != string.Empty)
                {
                   

                        //object result = _evaluator.Evaluate(expr, _brickService);
                        //object result = _evaluator.Evaluate(expr);
                        yield return StartCoroutine(_evaluator.Evaluate(expr));
                    object result = gv.GlobalVariables.res;


                        if (procedureName == "maindefault")
                        {
                            try
                            {
                                if (variableName.IndexOf(".") >= 0)
                                {
                                    if (variableName.IndexOf("(") < 0 && variableName.IndexOf(")") < 0)
                                    {
                                        int dotPos = variableName.IndexOf(".");
                                        string objectName = variableName.Substring(0, dotPos);
                                        string properyName = variableName.Substring(dotPos);

                                        SetProperty(objectName, properyName, localVariables, result);
                                    }
                                }
                                else
                                {
                                    if (!_GlobalVariables.ContainsKey(variableName))
                                        _GlobalVariables.Add(variableName, new object());


                                    if (dimensions == 0)
                                        _GlobalVariables[variableName] = result;
                                    else
                                    {

                                        if (dimensions == 1)
                                        {
                                            object targetArray = _GlobalVariables[variableName];
                                            Util.SetTypedArrayValue(targetArray, arrayIndex1, result);
                                        }
                                        else
                                        {
                                            LogInfo("[ParseSetVariableLineWithLocal] Do not support multi-dimension array");
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                LogInfo("[ParseSetVariableLineWithLocal] " + ex.ToString());
                            }
                        }
                        else
                        {
                            if (variableName.IndexOf(".") >= 0)
                            {
                                //Property
                                if (variableName.IndexOf("(") < 0 && variableName.IndexOf(")") < 0)
                                {
                                    //property
                                    int dotPos = variableName.IndexOf(".");
                                    string objectName = variableName.Substring(0, dotPos);
                                    string properyName = variableName.Substring(dotPos);

                                    SetProperty(objectName, properyName, localVariables, result);
                                }
                            }
                            else
                            {
                                if (_GlobalVariables.ContainsKey(variableName))
                                {
                                    if (dimensions == 0)
                                        _GlobalVariables[variableName] = result;
                                    else
                                    {

                                        if (dimensions == 1)
                                        {
                                            object targetArray = _GlobalVariables[variableName];
                                            //Util.SetTypedArrayValue(targetArray, arrayIndex1, result);

                                            //2016.09.25
                                            //#############################
                                            string tName = targetArray.GetType().Name;

                                            if (tName.EndsWith("String"))
                                            {
                                                String target = targetArray.ToString();
                                                String res = string.Empty;

                                                for (int i = 0; i < target.Length; i++)
                                                {
                                                    if (i == arrayIndex1)
                                                        res = res + result.ToString();
                                                    else
                                                        res = res + target[i];
                                                }

                                                _GlobalVariables[variableName] = res;
                                                //#############################
                                            }
                                            else
                                                Util.SetTypedArrayValue(targetArray, arrayIndex1, result);
                                        }
                                        else
                                        {
                                            LogInfo("[ParseSetVariableLineWithLocal] Do not support multi-dimension array");
                                        }
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        if (localVariables.ContainsKey(variableName))
                                        {
                                            if (dimensions == 0)
                                                localVariables[variableName] = result;
                                            else
                                            {

                                                if (dimensions == 1)
                                                {
                                                    object targetArray = localVariables[variableName];
                                                    //Util.SetTypedArrayValue(targetArray, arrayIndex1, result);

                                                    //2016.09.25
                                                    //#############################
                                                    string tName = targetArray.GetType().Name;

                                                    if (tName.EndsWith("String"))
                                                    {
                                                        String target = targetArray.ToString();
                                                        String res = string.Empty;

                                                        for (int i = 0; i < target.Length; i++)
                                                        {
                                                            if (i == arrayIndex1)
                                                                res = res + result.ToString();
                                                            else
                                                                res = res + target[i];
                                                        }

                                                        localVariables[variableName] = res;
                                                        //#############################
                                                    }
                                                    else
                                                        Util.SetTypedArrayValue(targetArray, arrayIndex1, result);
                                                }
                                                else
                                                {
                                                    LogInfo("[ParseSetVariableLineWithLocal] Do not support multi-dimension array");
                                                }
                                            }
                                        }
                                        else
                                            localVariables.Add(variableName, result);
                                    }
                                    catch (Exception ex)
                                    {
                                        LogInfo("[ParseSetVariableLineWithLocal] " + ex.ToString());
                                    }
                                }
                            }
                        }
                  

                }

                #endregion if (tagArr.Length == 2)
            }
            else
            {

                string expr = newLine;

                if (expr != string.Empty)
                {
                  
                        //object result = _evaluator.Evaluate(expr, _brickService);
                        //object result = _evaluator.Evaluate(expr);
                        yield return StartCoroutine(_evaluator.Evaluate(expr));
                        object result = gv.GlobalVariables.res;
                        //_evaluator.Evaluate(expr);
                   

                }
            }
        }

    }

    //public void ParseSetVariableLineWithLocal2(string line, Dictionary<object, object> localVariables, string procedureName, eval.Evaluator _evaluator, string procitem_Target = "")
    //{

    //    //LogInfo("[ParseSetVariableLineWithLocal ### " + line);

    //    _evaluator.LocalVariables = localVariables;

    //    try
    //    {
    //        string newLine = line;

    //        if (line.IndexOf("return ") == 0)
    //        {
    //            #region return

    //            if (newLine.Length > 6)
    //            {
    //                string returnExpr = newLine.Substring(6, newLine.Length - 6);
    //                returnExpr = returnExpr.Trim();


    //                int rInd = returnExpr.IndexOf("++");
    //                if (rInd > 0)
    //                {
    //                    string varName = returnExpr.Substring(0, rInd);
    //                    returnExpr = varName + " = " + varName + " + 1";
    //                }

    //                rInd = returnExpr.IndexOf("--");
    //                if (rInd > 0)
    //                {
    //                    string varName = returnExpr.Substring(0, rInd);
    //                    returnExpr = varName + " = " + varName + " - 1";
    //                }

    //                rInd = returnExpr.IndexOf("+=");
    //                if (rInd > 0 && returnExpr.Length > (rInd + 2))
    //                {
    //                    string varName1 = returnExpr.Substring(0, rInd);
    //                    string varName2 = returnExpr.Substring(rInd + 2, (returnExpr.Length - rInd - 2));
    //                    returnExpr = varName1 + " = " + varName1 + " + " + varName2;
    //                }

    //                rInd = returnExpr.IndexOf("-=");
    //                if (rInd > 0 && returnExpr.Length > (rInd + 2))
    //                {
    //                    string varName1 = returnExpr.Substring(0, rInd);
    //                    string varName2 = returnExpr.Substring(rInd + 2, (returnExpr.Length - rInd - 2));
    //                    returnExpr = varName1 + " = " + varName1 + " - " + varName2;
    //                }


    //                try
    //                {
    //                    object result = _evaluator.Evaluate(returnExpr);
    //                    bool IsAvailable = true;
    //                    //IsAvailable = ConvertingType(result, procitem_Target);

    //                    if (IsAvailable)
    //                    {
    //                        if (_GlobalVariables.ContainsKey(procedureName + "_return_value"))
    //                        {
    //                            _GlobalVariables[procedureName + "_return_value"] = result;

    //                        }
    //                        else
    //                        {
    //                            _GlobalVariables.Add(procedureName + "_return_value", result);

    //                        }
    //                    }
    //                    //else
    //                    //{

    //                    //}

    //                }
    //                catch (Exception ex)
    //                {
    //                    LogInfo("[ParseSetVariableLineWithLocal] " + ex.ToString());
    //                }
    //            }

    //            #endregion return
    //        }
    //        else
    //        {

    //            int rInd = newLine.IndexOf("++");
    //            if (rInd > 0)
    //            {
    //                string varName = newLine.Substring(0, rInd);
    //                newLine = varName + " = " + varName + " + 1";
    //            }

    //            rInd = newLine.IndexOf("--");
    //            if (rInd > 0)
    //            {
    //                string varName = newLine.Substring(0, rInd);
    //                newLine = varName + " = " + varName + " - 1";
    //            }

    //            rInd = newLine.IndexOf("+=");
    //            if (rInd > 0 && newLine.Length > (rInd + 2))
    //            {
    //                string varName1 = newLine.Substring(0, rInd);
    //                string varName2 = newLine.Substring(rInd + 2, (newLine.Length - rInd - 2));
    //                newLine = varName1 + " = " + varName1 + " + " + varName2;
    //            }

    //            rInd = newLine.IndexOf("-=");
    //            if (rInd > 0 && newLine.Length > (rInd + 2))
    //            {
    //                string varName1 = newLine.Substring(0, rInd);
    //                string varName2 = newLine.Substring(rInd + 2, (newLine.Length - rInd - 2));
    //                newLine = varName1 + " = " + varName1 + " - " + varName2;
    //            }



    //            string[] tagArr;
    //            tagArr = newLine.Split(new Char[] { '=' });


    //            if (tagArr.Length == 2)
    //            {

    //                #region if (tagArr.Length == 2)

    //                string variableName = tagArr[0].Trim();
    //                string expr = tagArr[1].Trim();

    //                string dimensionExpr1 = string.Empty;
    //                int dimensions = 0;


    //                variableName = GetVariableName(variableName, out dimensionExpr1, out dimensions);


    //                int arrayIndex1 = -1;

    //                if (dimensionExpr1 != string.Empty)
    //                {
    //                    try
    //                    {
    //                        //object result = _evaluator.Evaluate(dimensionExpr1, _brickService);
    //                        object result = _evaluator.Evaluate(dimensionExpr1);
    //                        arrayIndex1 = Util.ToInt(result);
    //                    }
    //                    catch (Exception ex)
    //                    {
    //                        LogInfo("[ParseSetVariableLineWithLocal] " + ex.ToString());
    //                    }
    //                }

    //                if (variableName != string.Empty && expr != string.Empty)
    //                {
    //                    try
    //                    {

    //                        //object result = _evaluator.Evaluate(expr, _brickService);
    //                        object result = _evaluator.Evaluate(expr);


    //                        if (procedureName == "maindefault")
    //                        {
    //                            try
    //                            {
    //                                if (variableName.IndexOf(".") >= 0)
    //                                {
    //                                    if (variableName.IndexOf("(") < 0 && variableName.IndexOf(")") < 0)
    //                                    {
    //                                        int dotPos = variableName.IndexOf(".");
    //                                        string objectName = variableName.Substring(0, dotPos);
    //                                        string properyName = variableName.Substring(dotPos);

    //                                        SetProperty(objectName, properyName, localVariables, result);
    //                                    }
    //                                }
    //                                else
    //                                {
    //                                    if (!_GlobalVariables.ContainsKey(variableName))
    //                                        _GlobalVariables.Add(variableName, new object());


    //                                    if (dimensions == 0)
    //                                        _GlobalVariables[variableName] = result;
    //                                    else
    //                                    {

    //                                        if (dimensions == 1)
    //                                        {
    //                                            object targetArray = _GlobalVariables[variableName];
    //                                            Util.SetTypedArrayValue(targetArray, arrayIndex1, result);
    //                                        }
    //                                        else
    //                                        {
    //                                            LogInfo("[ParseSetVariableLineWithLocal] Do not support multi-dimension array");
    //                                        }
    //                                    }
    //                                }
    //                            }
    //                            catch (Exception ex)
    //                            {
    //                                LogInfo("[ParseSetVariableLineWithLocal] " + ex.ToString());
    //                            }
    //                        }
    //                        else
    //                        {
    //                            if (variableName.IndexOf(".") >= 0)
    //                            {
    //                                //Property
    //                                if (variableName.IndexOf("(") < 0 && variableName.IndexOf(")") < 0)
    //                                {
    //                                    //property
    //                                    int dotPos = variableName.IndexOf(".");
    //                                    string objectName = variableName.Substring(0, dotPos);
    //                                    string properyName = variableName.Substring(dotPos);

    //                                    SetProperty(objectName, properyName, localVariables, result);
    //                                }
    //                            }
    //                            else
    //                            {
    //                                if (_GlobalVariables.ContainsKey(variableName))
    //                                {
    //                                    if (dimensions == 0)
    //                                        _GlobalVariables[variableName] = result;
    //                                    else
    //                                    {

    //                                        if (dimensions == 1)
    //                                        {
    //                                            object targetArray = _GlobalVariables[variableName];
    //                                            //Util.SetTypedArrayValue(targetArray, arrayIndex1, result);

    //                                            //2016.09.25
    //                                            //#############################
    //                                            string tName = targetArray.GetType().Name;

    //                                            if (tName.EndsWith("String"))
    //                                            {
    //                                                String target = targetArray.ToString();
    //                                                String res = string.Empty;

    //                                                for (int i = 0; i < target.Length; i++)
    //                                                {
    //                                                    if (i == arrayIndex1)
    //                                                        res = res + result.ToString();
    //                                                    else
    //                                                        res = res + target[i];
    //                                                }

    //                                                _GlobalVariables[variableName] = res;
    //                                                //#############################
    //                                            }
    //                                            else
    //                                                Util.SetTypedArrayValue(targetArray, arrayIndex1, result);
    //                                        }
    //                                        else
    //                                        {
    //                                            LogInfo("[ParseSetVariableLineWithLocal] Do not support multi-dimension array");
    //                                        }
    //                                    }
    //                                }
    //                                else
    //                                {
    //                                    try
    //                                    {
    //                                        if (localVariables.ContainsKey(variableName))
    //                                        {
    //                                            if (dimensions == 0)
    //                                                localVariables[variableName] = result;
    //                                            else
    //                                            {

    //                                                if (dimensions == 1)
    //                                                {
    //                                                    object targetArray = localVariables[variableName];
    //                                                    //Util.SetTypedArrayValue(targetArray, arrayIndex1, result);

    //                                                    //2016.09.25
    //                                                    //#############################
    //                                                    string tName = targetArray.GetType().Name;

    //                                                    if (tName.EndsWith("String"))
    //                                                    {
    //                                                        String target = targetArray.ToString();
    //                                                        String res = string.Empty;

    //                                                        for (int i = 0; i < target.Length; i++)
    //                                                        {
    //                                                            if (i == arrayIndex1)
    //                                                                res = res + result.ToString();
    //                                                            else
    //                                                                res = res + target[i];
    //                                                        }

    //                                                        localVariables[variableName] = res;
    //                                                        //#############################
    //                                                    }
    //                                                    else
    //                                                        Util.SetTypedArrayValue(targetArray, arrayIndex1, result);
    //                                                }
    //                                                else
    //                                                {
    //                                                    LogInfo("[ParseSetVariableLineWithLocal] Do not support multi-dimension array");
    //                                                }
    //                                            }
    //                                        }
    //                                        else
    //                                            localVariables.Add(variableName, result);
    //                                    }
    //                                    catch (Exception ex)
    //                                    {
    //                                        LogInfo("[ParseSetVariableLineWithLocal] " + ex.ToString());
    //                                    }
    //                                }
    //                            }
    //                        }
    //                    }
    //                    catch (Exception ex)
    //                    {
    //                        LogInfo("[ParseSetVariableLineWithLocal] " + ex.ToString());
    //                    }

    //                }

    //                #endregion if (tagArr.Length == 2)
    //            }
    //            else
    //            {

    //                string expr = newLine;

    //                if (expr != string.Empty)
    //                {
    //                    try
    //                    {
    //                        //object result = _evaluator.Evaluate(expr, _brickService);
    //                        //object result = _evaluator.Evaluate(expr);
    //                        _evaluator.Evaluate(expr);
    //                    }
    //                    catch (Exception ex)
    //                    {
    //                        LogInfo("[ParseSetVariableLineWithLocal] " + ex.ToString());
    //                    }

    //                }
    //            }
    //        }

    //    }
    //    catch (Exception ex)
    //    {
    //        LogInfo("[ParseSetVariableLineWithLocal] " + ex.ToString());
    //    }

    //}

    public bool ConvertingType(object OriginalType, string procitem_Target)
    {
        bool returnValue = false;

        string TypeName = OriginalType.GetType().Name;
        string fTypeName = OriginalType.GetType().FullName;
        //string fTypeName1 = OriginalType.GetType().

        //Utility

        try
        {
            if (procitem_Target == "string" || procitem_Target == "String")
            {
                if (OriginalType is string)
                {
                    returnValue = true;
                }
            }
            else if (procitem_Target == "int")
            {

                //int Result = Util.ToInt(OriginalType);
                //int Result = Convert.ToInt32(OriginalType);
                //int Result = (int)OriginalType;
                //int Result = Int32.TryParse(OriginalType);
                bool Result = OriginalType is Int32;
                returnValue = true;
            }
            else if (procitem_Target == "float")
            {
                float Result = (float)OriginalType;
                returnValue = true;
            }
            else if (procitem_Target == "double")
            {
                double Result = (double)OriginalType;
                returnValue = true;
            }
            else if (procitem_Target == "short")
            {
                short Result = (short)OriginalType;
                returnValue = true;
            }
            else if (procitem_Target == "long")
            {
                long Result = (long)OriginalType;
                returnValue = true;
            }
            else if (procitem_Target == "char")
            {
                char Result = (char)OriginalType;
                returnValue = true;
            }
            else if (procitem_Target == "byte")
            {
                byte Result = (byte)OriginalType;
                returnValue = true;
            }
            else if (procitem_Target == "byte")
            {
                byte Result = (byte)OriginalType;
                returnValue = true;
            }
            else if (procitem_Target == "bool")
            {
                bool Result = (bool)OriginalType;
                returnValue = true;
            }

        }
        catch (Exception ex)
        {
            Debug.Log("변환실패");
        }



        return returnValue;
    }

    public void SetProperty(string objName, string propName, Dictionary<object, object> localVariables, object res)
    {
        object instance = null;

        if (localVariables.ContainsKey(objName))
            instance = localVariables[objName];
        else if (_GlobalVariables.ContainsKey(objName))
            instance = _GlobalVariables[objName];

        if (instance == null)
        {
            LogInfo("[SetProperty] " + "Can't find object -> " + objName);
            return;
        }

        string tName = instance.GetType().Name;

        if (tName.IndexOf("System.") == 0)
            tName = tName.Substring(7);

        if (tName.IndexOf("Collections.") == 0)
            tName = tName.Substring(12);

        if (tName.IndexOf("SPL.Script.Engine.") == 0)
            tName = tName.Substring(18);

        //if (tName.IndexOf("Microsoft.SPOT.Presentation.Controls.") == 0)
        //    tName = tName.Substring(37);



        switch (propName)
        {
            //#//
            //case ".Active":
            //    {
            //        switch (tName)
            //        {
            //            case "TristatePort":
            //                {
            //                    TristatePort port = (TristatePort)instance;
            //                    port.Active = Util.ToBool(res);                                    
            //                }

            //                break;

            //            default:
            //                LogInfo("[SetProperty] " + "Can't find property -> " + objName + " / " + propName + " / " + tName);
            //                break;
            //        }
            //    }
            //    break;

            //case ".TextContent":
            //    {
            //        switch (tName)
            //        {
            //            case "Text":
            //                {
            //                    if (Dispatcher.CurrentDispatcher == _currentDispatcher)
            //                    {
            //                        Text txt = (Text)instance;
            //                        txt.TextContent = res.ToString();
            //                    }
            //                    else
            //                    {
            //                        System.Threading.Timer timer = new System.Threading.Timer(new System.Threading.TimerCallback(SetTextContent_Thread), new AsyncSetPropertyParam(_currentDispatcher, propName, tName, instance, res), 0, -1);
            //                    }
            //                }

            //                break;

            //            default:
            //                LogInfo("[SetProperty] " + "Can't find property -> " + objName + " / " + propName + " / " + tName);
            //                break;
            //        }
            //    }
            //    break;

            default:
                LogInfo("[SetProperty] " + "Can't find property -> " + objName + " / " + propName + " / " + tName);
                break;
        }
    }


    #endregion ParseSetVariableLineWithLocal



    #region CheckConditionExprWithLocal

    public IEnumerator CheckConditionExprWithLocal(string conditionExpr, Dictionary<object, object> localVariables, eval.Evaluator _evaluator)
    {
        object res = null;

        _evaluator.LocalVariables = localVariables;


        string expr = conditionExpr;

        if (expr != string.Empty)
        {

            //object result = _evaluator.Evaluate(expr);
            yield return StartCoroutine(_evaluator.Evaluate(expr));
            object result = gv.GlobalVariables.res;

            if (result != null)
            {
                string eval_res = result.ToString().ToLower();

                if (eval_res == "true")
                    res = true;
                else if (eval_res == "false")
                    res = false;
                else
                {
                    res = null;
                    LogInfo("[CheckConditionExprWithLocal] Condition value is not boolean type -> " + conditionExpr);
                }
            }
            else
            {
                LogInfo("[CheckConditionExprWithLocal] Condition value is null " + conditionExpr);
            }



        }


        gv.GlobalVariables.res = res;
        //return res;

    }

    //public object CheckConditionExprWithLocal_Origin(string conditionExpr, Dictionary<object, object> localVariables, eval.Evaluator _evaluator)
    //{
    //    object res = null;

    //    _evaluator.LocalVariables = localVariables;

    //    try
    //    {
    //        string expr = conditionExpr;

    //        if (expr != string.Empty)
    //        {
    //            try
    //            {
    //                object result = _evaluator.Evaluate(expr);

    //                if (result != null)
    //                {
    //                    string eval_res = result.ToString().ToLower();

    //                    if (eval_res == "true")
    //                        res = true;
    //                    else if (eval_res == "false")
    //                        res = false;
    //                    else
    //                    {
    //                        res = null;
    //                        LogInfo("[CheckConditionExprWithLocal] Condition value is not boolean type -> " + conditionExpr);
    //                    }
    //                }
    //                else
    //                {
    //                    LogInfo("[CheckConditionExprWithLocal] Condition value is null " + conditionExpr);
    //                }

    //            }
    //            catch (Exception ex)
    //            {
    //                LogInfo("[CheckConditionExprWithLocal] " + ex.ToString() + " / " + conditionExpr);
    //            }

    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        LogInfo("[CheckConditionExprWithLocal] " + ex.ToString() + " / " + conditionExpr);
    //    }

    //    return res;

    //}
    #endregion CheckConditionExprWithLocal


    #region ExcuteConditionExprWithLocal


    public IEnumerator ExcuteConditionExprWithLocal(string conditionExpr, Dictionary<object, object> localVariables, eval.Evaluator _evaluator)
    {
        object res = null;

        _evaluator.LocalVariables = localVariables;

        if (conditionExpr == string.Empty)
        {
            gv.GlobalVariables.res = res;
            yield break;
            //   return res;
        }




        string expr = conditionExpr;

        if (expr != string.Empty)
        {

            //object result = _evaluator.Evaluate(expr);
            yield return StartCoroutine(_evaluator.Evaluate(expr));
            object result = gv.GlobalVariables.res;

            if (result != null)
            {
                res = result;
            }


        }

        gv.GlobalVariables.res = res;
        //return res;

    }


    #endregion ExcuteConditionExprWithLocal


}
