using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using SPL.Common;

public class EnginePreProcessor {

    //######################################################################
    //스크립트 엔진 전처리에 필요한 전역변수
    //######################################################################
    public delegate void LogInfoHandler(string log);
    public LogInfoHandler EvalLogInfoHandler = null;

    public Dictionary<object, object> _procedureTreeInstanceList = null;
    public Dictionary<object, object> _EventProcedureMappingList = null;

    public ArrayList _variableTypeNameList = null;
    public Dictionary<object, object> _GlobalVariables = null;
    public ArrayList _ProcedureList = new ArrayList();

    public Dictionary<string, int> _stringToIntMapping = null;


    //######################################################################
	public EnginePreProcessor()
	{
	}


    //######################################################################
	public void LogInfo(string log)
	{		
		if (log == null)
			return;
		
		
		if (EvalLogInfoHandler != null)
		{
			EvalLogInfoHandler(log);
		}
	}


	public void InitializeWorks()
	{
		try
		{
            //Arduino Pin Value
            _GlobalVariables.Add("_DIGITAL_0", 0);
            _GlobalVariables.Add("_DIGITAL_1", 0);
            _GlobalVariables.Add("_DIGITAL_2", 0);
            _GlobalVariables.Add("_DIGITAL_3", 0);
            _GlobalVariables.Add("_DIGITAL_4", 0);
            _GlobalVariables.Add("_DIGITAL_5", 0);
            _GlobalVariables.Add("_DIGITAL_6", 0);
            _GlobalVariables.Add("_DIGITAL_7", 0);
            _GlobalVariables.Add("_DIGITAL_8", 0);
            _GlobalVariables.Add("_DIGITAL_9", 0);
            _GlobalVariables.Add("_DIGITAL_10", 0);
            _GlobalVariables.Add("_DIGITAL_11", 0);
            _GlobalVariables.Add("_DIGITAL_12", 0);
            _GlobalVariables.Add("_DIGITAL_13", 0);


            _GlobalVariables.Add("_PWM_0", 0);
            _GlobalVariables.Add("_PWM_1", 0);
            _GlobalVariables.Add("_PWM_2", 0);
            _GlobalVariables.Add("_PWM_3", 0);
            _GlobalVariables.Add("_PWM_4", 0);
            _GlobalVariables.Add("_PWM_5", 0);
            _GlobalVariables.Add("_PWM_6", 0);
            _GlobalVariables.Add("_PWM_7", 0);
            _GlobalVariables.Add("_PWM_8", 0);
            _GlobalVariables.Add("_PWM_9", 0);
            _GlobalVariables.Add("_PWM_10", 0);
            _GlobalVariables.Add("_PWM_11", 0);
            _GlobalVariables.Add("_PWM_12", 0);
            _GlobalVariables.Add("_PWM_13", 0);


            _GlobalVariables.Add("_SERVO_0", 0);
            _GlobalVariables.Add("_SERVO_1", 0);
            _GlobalVariables.Add("_SERVO_2", 0);
            _GlobalVariables.Add("_SERVO_3", 0);
            _GlobalVariables.Add("_SERVO_4", 0);
            _GlobalVariables.Add("_SERVO_5", 0);
            _GlobalVariables.Add("_SERVO_6", 0);
            _GlobalVariables.Add("_SERVO_7", 0);
            _GlobalVariables.Add("_SERVO_8", 0);
            _GlobalVariables.Add("_SERVO_9", 0);
            _GlobalVariables.Add("_SERVO_10", 0);
            _GlobalVariables.Add("_SERVO_11", 0);
            _GlobalVariables.Add("_SERVO_12", 0);
            _GlobalVariables.Add("_SERVO_13", 0);



            _GlobalVariables.Add("_ANALOG_0", 0.0f);
            _GlobalVariables.Add("_ANALOG_1", 0.0f);
            _GlobalVariables.Add("_ANALOG_2", 0.0f);
            _GlobalVariables.Add("_ANALOG_3", 0.0f);
            _GlobalVariables.Add("_ANALOG_4", 0.0f);
            _GlobalVariables.Add("_ANALOG_5", 800.0f);
            _GlobalVariables.Add("_ANALOG_6", 0.0f);

            _GlobalVariables.Add("_MOTOR1_POWER", 0);
            _GlobalVariables.Add("_MOTOR2_POWER", 0);

            _GlobalVariables.Add("HIGH", 1);
            _GlobalVariables.Add("LOW", 0);


            _GlobalVariables.Add("LED_OFF", 0);
            _GlobalVariables.Add("LED_RED", 1);
            _GlobalVariables.Add("LED_YELLOW", 2);
            _GlobalVariables.Add("LED_GREEN", 3);


            _GlobalVariables.Add("null", null);
            _GlobalVariables.Add("true", true);
            _GlobalVariables.Add("false", false);
            _GlobalVariables.Add("True", true);
            _GlobalVariables.Add("False", false);


            _variableTypeNameList.Add("unsigned");
            _variableTypeNameList.Add("Boolean");
            _variableTypeNameList.Add("boolean");

			_variableTypeNameList.Add("null");
			_variableTypeNameList.Add("char");
			_variableTypeNameList.Add("sbyte");
			_variableTypeNameList.Add("byte");
			_variableTypeNameList.Add("short");
			_variableTypeNameList.Add("ushort");
			_variableTypeNameList.Add("int");
			_variableTypeNameList.Add("uint");
			_variableTypeNameList.Add("long");
			_variableTypeNameList.Add("ulong");
			_variableTypeNameList.Add("float");
			_variableTypeNameList.Add("double");
			_variableTypeNameList.Add("decimal");
			_variableTypeNameList.Add("datetime");
			_variableTypeNameList.Add("string");
            _variableTypeNameList.Add("String");
        }
		catch (Exception ex)
		{
			LogInfo("[InitializeWorks] " + ex.ToString());
		}
		
	}

	
	public List<string> FileToList(string script_text, ArrayList lines)
	{
		List<string> CheckRequiredCmdList = new List<string>();

        if (string.IsNullOrEmpty(script_text))
            return CheckRequiredCmdList;
		
		try
		{			
			ArrayList tmpLines = new ArrayList();			
			string[] readlines = null;

            readlines = script_text.Split(new Char[] { '\r', '\n' });

            if (readlines == null)
                return CheckRequiredCmdList;

			
			int lastInd = 0;
			int endOfLine = 0;
			
            //명령어들의 앞뒤 공백이나 탭 문자를 제거한다.
			for (int i = 0; i < readlines.Length; i++)
			{
				readlines[i] = readlines[i].Trim(' ', '\t');				
				readlines[i] = readlines[i].Replace('\t', ' ');
				
				endOfLine = readlines[i].IndexOf(" //");
				if (endOfLine >= 0)
				{
					readlines[i] = readlines[i].Substring(0, endOfLine);
				}
				
				endOfLine = readlines[i].IndexOf("\t//");
				if (endOfLine >= 0)
				{
					readlines[i] = readlines[i].Substring(0, endOfLine);
				}
			}


            //공백 라인을 제거한다.
            for (int i = 0; i < readlines.Length; i++)
            {
                if (readlines[i].Trim() == string.Empty)
                {
                    // just skip
                }
                else
                {
                    tmpLines.Add(readlines[i].Trim());
                }
            }



            foreach (string singleline in tmpLines)
            {
                string line = SPL.Common.ParsingHelper.ReplaceSlashString(singleline);
                string line_lower = line.ToLower();
                string first_tocken = SPL.Common.ParsingHelper.GetFirstToken(singleline.ToLower());

                #region foreach (string singleline in tmpLines)

                if (first_tocken == "armode")
                {
                    CheckRequiredCmdList.Add(line_lower);
                }
                else if (first_tocken == "void")
                {
                    string procedure_name = line.Substring(5).Trim();
                    string[] proc_arr = line.Split(new char[] { ' ', '\t', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);

                    if (proc_arr != null && proc_arr.Length >= 2 && proc_arr[1] == "loop")
                    {
                        lines.Add("Procedure loop");
                        CheckRequiredCmdList.Add("ProcedureLoop");
                    }
                    else if (proc_arr != null && proc_arr.Length >= 2 && proc_arr[1] == "setup")
                    {
                        lines.Add("Procedure setup");
                        CheckRequiredCmdList.Add("ProcedureSetup");
                    }
                    else
                        lines.Add("Procedure " + procedure_name);
                }
                else if (SPL.Common.ParsingHelper.IsTypeCmd(first_tocken) && !line.Contains("=") && SPL.Common.ParsingHelper.IsFunctionDefinition2(line) != null)
                {
                    //2016.07.04 Modify if condition
                    //void가 아닌 함수 선언을 찾아냄
                    lines.Add("Procedure " + line);
                }
                else
                    lines.Add(line);

                #endregion foreach (string singleline in tmpLines)
            }
		}
		catch (Exception ex)
		{
			LogInfo("[FileToList] " + ex.ToString());
		}

		return CheckRequiredCmdList;


	}



    public string GetStringWithUniquePostfix(string name)
    {
        string newName = name;

        try
        {
            int lastNo = 0;

            if (_stringToIntMapping.ContainsKey(name))
            {
                lastNo = _stringToIntMapping[name];
                lastNo = lastNo + 1;
                _stringToIntMapping[name] = lastNo;
            }
            else
            {
                _stringToIntMapping.Add(name, 1);
                lastNo = 1;
            }

            newName = name + lastNo.ToString();
        }
        catch { }

        return newName;
    }


    public string GetProcedureName(string id)
    {
        string proc_name = string.Empty;

        if (_EventProcedureMappingList.ContainsKey(id))
            proc_name = _EventProcedureMappingList[id].ToString();

        return proc_name;
    }



    public CommandTree SearchProcedureWithName(string procedureName)
    {
        try
        {
            //2016.07.05 Add if
            if (_procedureTreeInstanceList.ContainsKey(procedureName))
                return (CommandTree)_procedureTreeInstanceList[procedureName];
            else
            {
                //LogInfo("\"" + procedureName + "\" function not exist");
                return null;
            }
        }
        catch (Exception ex)
        {
            LogInfo("[SearchProcedureWithName] " + ex.ToString());
        }

        return null;
    }


    public bool CheckRegisteredGlobalVariable(string firstToken, string line)
    {
        bool res = false;

        try
        {
            string tmpLine = ParsingHelper.GetDoubleQuoteTrimedString(line);

            if (_variableTypeNameList.Contains(firstToken))
            {
                res = true;
            }

            else if (_GlobalVariables.ContainsKey(firstToken))
            {
                res = true;
            }

            else if ((tmpLine.IndexOf("=") >= 0 || tmpLine.IndexOf("++") >= 0 || tmpLine.IndexOf("--") >= 0 || tmpLine.IndexOf("+=") >= 0 || tmpLine.IndexOf("-=") >= 0) && firstToken != "if")
            {
                res = true;
            }

            else if (tmpLine.IndexOf(".") >= 0 && tmpLine.IndexOf("(") >= 0 && tmpLine.IndexOf(")") >= 0)
            {
                res = true;
            }
        }
        catch (Exception ex)
        {
            LogInfo("[CheckRegisteredGlobalVariable] " + ex.ToString());
        }

        return res;
    }



    public void AddToEventProcedureMappingList(string id, string proc_name)
    {
        try
        {
            if (!_EventProcedureMappingList.ContainsKey(id))
            {
                _EventProcedureMappingList.Add(id, proc_name);

                LogInfo("[AddToEventProcedureMappingList] -> " + id + " / " + proc_name + " was bound.");
            }
            else
            {
                LogInfo("[AddToEventProcedureMappingList] -> " + id + " / " + proc_name + " exists already.");
            }
        }
        catch (Exception ex)
        {
            LogInfo("[AddToEventProcedureMappingList] " + ex.ToString());
        }
    }



}
