using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using SPL.Common;
using eval = SPL.Evaluator;

public class ExecuteCmdLineHelper
{

    public delegate void LogInfoHandler(string log);
    public LogInfoHandler EvalLogInfoHandler = null;

    public Dictionary<object, object> _procedureTreeInstanceList = null;
    public Dictionary<object, object> _EventProcedureMappingList = null;

    public ArrayList _variableTypeNameList = null;
    public Dictionary<object, object> _GlobalVariables = null;
    public ArrayList _ProcedureList = new ArrayList();

    public Dictionary<object, object> _stringToIntMapping = new Dictionary<object, object>();



    public ExecuteCmdLineHelper()
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



    public bool CheckSetVariable(string line)
    {
        bool res = false;

        try
        {

            if ((line.IndexOf("=") >= 0 || line.IndexOf("++") >= 0 || line.IndexOf("--") >= 0 || line.IndexOf("+=") >= 0 || line.IndexOf("-=") >= 0) && line.IndexOf("if") != 0)
                res = true;

            else if (line.IndexOf(".") >= 0 && line.IndexOf("(") >= 0 && line.IndexOf(")") >= 0)
                res = true;
        }
        catch (Exception ex)
        {
            LogInfo("[CheckSetVariable] " + ex.ToString());
        }

        return res;
    }



    public bool CheckExecutableTargetType(string targetType)
    {
        try
        {
            switch (targetType)
            {
                case "delay": return true;
                case "defvariable": return true;
                case "setvariable": return true;
                case "script": return true;
                case "call": return true;
                case "start": return true;
                case "startloop": return true;

                case "spl_command": return true;
                case "arduino_command": return true;

                case "if": return true;
                case "else": return true;
                case "while": return true;
                case "for": return true;
                case "switch": return true;
                case "case": return true;
                case "default": return true;
                case "break": return true;
                case "continue": return true;
                case "print": return true;

                default: return false;
            }
        }
        catch (Exception ex)
        {
            LogInfo("[CheckExecutableTargetType] " + ex.ToString());
        }

        return false;
    }



    public void ExecuteCmdLine(CommandTree parsedTree)
    {
        CommandItem cmdItem = new CommandItem();

        string firstToken = parsedTree.FirstTokenOfLine;

        cmdItem.FirstToken = firstToken;


        //####################################################
        //Debug.Log("[ExecuteCmdLine] " + parsedTree.Name);
        //####################################################


        string[] arr;
        arr = parsedTree.Name.Split(new Char[] { '/' });

        ArrayList newArr = new ArrayList();

        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i].Trim() != string.Empty)
                newArr.Add(arr[i].Trim());
        }


        #region switch (firstToken)


        cmdItem.TargetType = firstToken;

        cmdItem.Target = ParsingHelper.GetFirstTargetParam(firstToken, newArr);



        if (firstToken == "wait")
        {
            cmdItem.TargetType = "delay";
            cmdItem.Action = string.Empty;

            cmdItem.Target = ParsingHelper.GetFirstTargetParam(firstToken, newArr);
        }
        else if (firstToken == "delay")
        {
            cmdItem.TargetType = "delay";
            cmdItem.Action = string.Empty;

            cmdItem.Target = ParsingHelper.GetFirstTargetParam(firstToken, newArr);
            cmdItem.ConditionExpr = ParsingHelper.RestoreSlashWithoutTrim(ParsingHelper.GetConditionExpression(parsedTree.Name));

            if (!string.IsNullOrEmpty(cmdItem.ConditionExpr))
                cmdItem.Target = cmdItem.ConditionExpr;
        }
        else if (firstToken == "call" || firstToken == "start" || firstToken == "startloop")
        {
            #region call

            cmdItem.TargetType = "call";

            //2017.05.15
            cmdItem.Target = ParsingHelper.GetFirstTargetParam(firstToken, newArr);


            Dictionary<object, object> tempList = ParsingHelper.GetTokenListWithHashtable(parsedTree.Name);


            if (firstToken == "call")
                cmdItem.RunMode = "seq";
            else if (firstToken == "start")
                cmdItem.RunMode = "concur";
            else if (firstToken == "startloop")
                cmdItem.RunMode = "loop";


            //2017.05.15
            //2017.11.17
            if (string.IsNullOrEmpty(cmdItem.Target))
                cmdItem.Target = ParsingHelper.RestoreSlashWithoutTrim(ParsingHelper.GetConditionExpression(parsedTree.Name));
            else
            {

                //2017.05.15
                //2017.11.17
                string paramStr = ParsingHelper.RestoreSlashWithoutTrim(ParsingHelper.GetConditionExpression(parsedTree.Name));


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
                            cmdItem.ProcedureCallDataList.Add(tokenStr);

                            tokenStr = string.Empty;
                        }
                    }
                }

                tokenStr = tokenStr.Trim();

                if (tokenStr != string.Empty)
                {
                    cmdItem.ProcedureCallDataList.Add(tokenStr);

                    tokenStr = string.Empty;
                }
            }

            #endregion call
        }
        else if (firstToken == "procedure")
        {
            cmdItem.TargetType = "procedure";
            cmdItem.RunMode = string.Empty;


            //Modified when 2016.07.04 for Arduino Simulation
            //cmdItem.Name = ParsingHelper.GetFirstTargetParam(firstToken, newArr);
            cmdItem.Name = ParsingHelper.GetProcedureName(firstToken, parsedTree.Name);

            //LogInfo(parsedTree.Name + " / " + cmdItem.Name);

            string paramStr = ParsingHelper.RestoreSlashWithoutTrim(ParsingHelper.GetConditionExpression(parsedTree.Name));

            if (paramStr != string.Empty)
            {
                string[] paramArr;
                paramArr = paramStr.Split(new Char[] { ',' });

                for (int pInd = 0; pInd < paramArr.Length; pInd++)
                {
                    //Just add last argument name only
                    //Ignore data type
                    //string p = paramArr[pInd].Trim();

                    //2016.07.04 : processing general C-Stype function having argument
                    string[] arg_arr = paramArr[pInd].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (arg_arr != null)
                    {
                        string p = arg_arr[arg_arr.Length - 1];

                        if (p != string.Empty)
                            cmdItem.ProcedureParamList.Add(p);
                    }
                }
            }
        }
        else if (firstToken == "if")
        {
            cmdItem.TargetType = "if";
            cmdItem.Action = ParsingHelper.RestoreSlashWithoutTrim(parsedTree.Name);
            cmdItem.ConditionExpr = ParsingHelper.RestoreSlashWithoutTrim(ParsingHelper.GetConditionExpression(parsedTree.Name));
        }
        else if (firstToken == "else")
        {
            cmdItem.TargetType = "else";
            cmdItem.Action = ParsingHelper.RestoreSlashWithoutTrim(parsedTree.Name);
        }
        else if (firstToken == "while")
        {
            cmdItem.TargetType = "while";
            cmdItem.Action = ParsingHelper.RestoreSlashWithoutTrim(parsedTree.Name);
            cmdItem.ConditionExpr = ParsingHelper.RestoreSlashWithoutTrim(ParsingHelper.GetConditionExpression(parsedTree.Name));
        }
        else if (firstToken == "for")
        {
            #region for

            string conditionExpr = ParsingHelper.RestoreSlashWithoutTrim(ParsingHelper.GetConditionExpression(parsedTree.Name));

            string[] tagArr;
            tagArr = conditionExpr.Split(new Char[] { ';' });

            if (tagArr.Length == 3)
            {
                if (tagArr[0].Trim() != string.Empty && tagArr[1].Trim() != string.Empty && tagArr[2].Trim() != string.Empty)
                {
                    cmdItem.TargetType = "for";
                    cmdItem.Action = ParsingHelper.RestoreSlashWithoutTrim(parsedTree.Name);
                    cmdItem.ForStartExpr = tagArr[0].Trim();
                    cmdItem.ForStartExpr_FirstToken = ParsingHelper.GetFirstToken(cmdItem.ForStartExpr);
                    cmdItem.ForEndCondition = tagArr[1].Trim();
                    cmdItem.ForStepExpr = tagArr[2].Trim();

                    int rInd = cmdItem.ForStepExpr.IndexOf("++");
                    if (rInd > 0)
                    {
                        string varName = cmdItem.ForStepExpr.Substring(0, rInd);
                        cmdItem.ForStepExpr = varName + " = " + varName + " + 1";
                    }

                    rInd = cmdItem.ForStepExpr.IndexOf("--");
                    if (rInd > 0)
                    {
                        string varName = cmdItem.ForStepExpr.Substring(0, rInd);
                        cmdItem.ForStepExpr = varName + " = " + varName + " - 1";
                    }

                    rInd = cmdItem.ForStepExpr.IndexOf("+=");
                    if (rInd > 0 && cmdItem.ForStepExpr.Length > (rInd + 2))
                    {
                        string varName1 = cmdItem.ForStepExpr.Substring(0, rInd);
                        string varName2 = cmdItem.ForStepExpr.Substring(rInd + 2, (cmdItem.ForStepExpr.Length - rInd - 2));
                        cmdItem.ForStepExpr = varName1 + " = " + varName1 + " + " + varName2;
                    }

                    rInd = cmdItem.ForStepExpr.IndexOf("-=");
                    if (rInd > 0 && cmdItem.ForStepExpr.Length > (rInd + 2))
                    {
                        string varName1 = cmdItem.ForStepExpr.Substring(0, rInd);
                        string varName2 = cmdItem.ForStepExpr.Substring(rInd + 2, (cmdItem.ForStepExpr.Length - rInd - 2));
                        cmdItem.ForStepExpr = varName1 + " = " + varName1 + " - " + varName2;
                    }

                }
            }

            #endregion for
        }
        else if (firstToken == "switch")
        {
            cmdItem.TargetType = "switch";
            cmdItem.Action = ParsingHelper.RestoreSlashWithoutTrim(parsedTree.Name);
            cmdItem.ConditionExpr = ParsingHelper.RestoreSlashWithoutTrim(ParsingHelper.GetConditionExpression(parsedTree.Name));
        }
        else if (firstToken == "case")
        {
            cmdItem.TargetType = "case";
            cmdItem.Action = ParsingHelper.RestoreSlashWithoutTrim(parsedTree.Name);
            cmdItem.CaseExpr = ParsingHelper.RestoreSlashWithoutTrim(parsedTree.Name.Trim());

            if (cmdItem.CaseExpr.Length > 4)
            {
                cmdItem.CaseExpr = cmdItem.CaseExpr.Substring(4, cmdItem.CaseExpr.Length - 4);
                cmdItem.CaseExpr = cmdItem.CaseExpr.Trim();
                cmdItem.CaseExpr = cmdItem.CaseExpr.TrimEnd(':');
                cmdItem.CaseExpr = cmdItem.CaseExpr.Trim();
                cmdItem.CaseExpr = cmdItem.CaseExpr.Trim('"');
            }
        }
        else if (firstToken == "default")
        {
            cmdItem.TargetType = "default";
            cmdItem.Action = ParsingHelper.RestoreSlashWithoutTrim(parsedTree.Name);
        }
        else if (firstToken == "return")
        {
            cmdItem.TargetType = "setvariable";
            cmdItem.Target = firstToken;
            cmdItem.Action = ParsingHelper.RestoreSlashWithoutTrim(parsedTree.Name);
        }
        else if (firstToken == "break")
        {
            cmdItem.TargetType = "break";
            cmdItem.Target = firstToken;
            cmdItem.Action = ParsingHelper.RestoreSlashWithoutTrim(parsedTree.Name);
        }
        else if (firstToken == "continue")
        {
            cmdItem.TargetType = "continue";
            cmdItem.Target = firstToken;
            cmdItem.Action = ParsingHelper.RestoreSlashWithoutTrim(parsedTree.Name);
        }
        else if (firstToken == "print" || firstToken == "serial.print")
        {
            cmdItem.TargetType = "print";
            cmdItem.Target = firstToken;

            parsedTree.Name = parsedTree.Name.Trim();

            if (parsedTree.Name.Length > 5)
            {
                if (firstToken == "serial.print")
                    cmdItem.Action = parsedTree.Name.Substring(12, parsedTree.Name.Length - 12);
                else
                    cmdItem.Action = parsedTree.Name.Substring(5, parsedTree.Name.Length - 5);

                cmdItem.Action = ParsingHelper.RestoreSlashWithoutTrim(cmdItem.Action.Trim());
            }
            else
                cmdItem.Action = string.Empty;
        }
        else if (firstToken == "printline" || firstToken == "serial.println")
        {
            cmdItem.TargetType = "print";
            cmdItem.Target = firstToken;

            parsedTree.Name = parsedTree.Name.Trim();

            if (parsedTree.Name.Length > 9)
            {
                if (firstToken == "serial.println")
                    cmdItem.Action = parsedTree.Name.Substring(14, parsedTree.Name.Length - 14);
                else
                    cmdItem.Action = parsedTree.Name.Substring(9, parsedTree.Name.Length - 9);

                cmdItem.Action = ParsingHelper.RestoreSlashWithoutTrim(cmdItem.Action.Trim());
            }
            else
                cmdItem.Action = string.Empty;
        }
        else if (firstToken == "digitalwrite" || firstToken == "analogwrite" || firstToken == "tone" || firstToken == "servowrite" ||
            firstToken == "motor1write" || firstToken == "motor2write" || firstToken == "drivewrite")
        {
            //2017.11.17 통합
            cmdItem.TargetType = "arduino_command";
            cmdItem.Action = firstToken;

            string conditionExpr = ParsingHelper.RestoreSlashWithoutTrim(ParsingHelper.GetConditionExpression(parsedTree.Name));
            cmdItem.Target = conditionExpr;
        }
        else
        {
            #region default

            if (_variableTypeNameList.Contains(firstToken))
            {

                cmdItem.TargetType = "defvariable";
                cmdItem.Target = firstToken;
                cmdItem.Action = ParsingHelper.RestoreSlashWithoutTrim(parsedTree.Name);

            }
            else if (_GlobalVariables.ContainsKey(firstToken))
            {

                cmdItem.TargetType = "setvariable";
                cmdItem.Target = firstToken;
                cmdItem.Action = ParsingHelper.RestoreSlashWithoutTrim(parsedTree.Name);

            }
            else if ((parsedTree.Name.IndexOf("=") >= 0 || parsedTree.Name.IndexOf("++") >= 0 || parsedTree.Name.IndexOf("--") >= 0 || parsedTree.Name.IndexOf("+=") >= 0 || parsedTree.Name.IndexOf("-=") >= 0) && firstToken != "if")
            {

                cmdItem.TargetType = "setvariable";
                cmdItem.Target = firstToken;
                cmdItem.Action = ParsingHelper.RestoreSlashWithoutTrim(parsedTree.Name);

            }
            else if (parsedTree.Name.IndexOf(".") >= 0 && parsedTree.Name.IndexOf("(") >= 0 && parsedTree.Name.IndexOf(")") >= 0)
            {
                cmdItem.TargetType = "setvariable";
                cmdItem.Target = firstToken;
                cmdItem.Action = ParsingHelper.RestoreSlashWithoutTrim(parsedTree.Name);
            }
            else if (parsedTree.Name.IndexOf("(") >= 0 && parsedTree.Name.IndexOf(")") >= 0)
            {
                //2016.09.25  프로시져 목록에 없어도 함수로 처리함. 프로시저가 프로그램 아래에서 나중에 정의되기 때문임
                cmdItem.TargetType = "setvariable";
                cmdItem.Target = firstToken;
                cmdItem.Action = ParsingHelper.RestoreSlashWithoutTrim(parsedTree.Name);
            }
            #endregion default
        }


        #endregion switch (firstToken)


        foreach (string tag in newArr)
        {
            string[] tagArr;
            tagArr = tag.Split(new Char[] { ':' });

            if (tagArr.Length == 2 && tagArr[1] != string.Empty)
            {
                tagArr[1] = ParsingHelper.RestoreSlashString(tagArr[1]);


                string paramKey = tagArr[0].Trim().ToLower();


                try
                {
                    if (!cmdItem.Params.ContainsKey(paramKey))
                        cmdItem.Params.Add(paramKey, tagArr[1].Trim());
                }
                catch (Exception ex)
                {
                    LogInfo("[ExecuteCmdLine] " + ex.ToString());
                }


                #region switch (tagArr[0].ToLower())

                switch (tagArr[0].ToLower())
                {
                    case "name":
                        cmdItem.Name = tagArr[1].Trim();
                        break;

                    case "targetname":
                        {
                            if (cmdItem.TargetType.ToLower() == "procedure")
                                cmdItem.Target = tagArr[1].Trim();
                        }
                        break;
                }

                #endregion switch (tagArr[0].ToLower())
            }
        }

        parsedTree.commandItem = cmdItem;

        parsedTree.IsSetVariable = CheckSetVariable(parsedTree.Name);
        parsedTree.IsExecutableTargetType = CheckExecutableTargetType(cmdItem.TargetType);


        if (firstToken == "procedure" && cmdItem.Name != string.Empty)
        {
            if (!_procedureTreeInstanceList.ContainsKey(cmdItem.Name))
            {
                _procedureTreeInstanceList.Add(cmdItem.Name, parsedTree);

                _ProcedureList.Add(cmdItem.Name);
            }
            else
            {
                _procedureTreeInstanceList[cmdItem.Name] = parsedTree;

                LogInfo("[ExecuteCmdLine] Old procedure was replace with new. -> " + cmdItem.Name);
            }
        }
    }


}
