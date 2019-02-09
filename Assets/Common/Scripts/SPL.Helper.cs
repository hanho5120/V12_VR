using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


namespace SPL.Common
{
    //public class GlobalObject
    //{
    //    public delegate void LogInfoHandler(string msg);
    //    public LogInfoHandler LogInfoEventHandler = null;
    //}

    public class UtilInstance
    {
        public UtilInstance()
        {
        }
    }

    public class MathInstance
    {
        public MathInstance()
        {
        }
    }

    public class ConvertInstance
    {
        public ConvertInstance()
        {
        }
    }

    public class RandomInstance
    {
        public RandomInstance()
        {
        }
    }

    public class ScreenInstance
    {
        public ScreenInstance()
        {
        }
    }

    class EvalEngineParam
    {
        public string _token = string.Empty;
        public int _tokType = 0;
        public string _exp = string.Empty;
        public int _expIdx = 0;
        public object _lastObject = null;
    }

    public class LocalVariableItem
    {
        public Dictionary<object, object> LocalVariables = null;

        public ArrayList TempLocalVariableList = null;

        public LocalVariableItem Clone()
        {
            LocalVariableItem newInstance = new LocalVariableItem();

            foreach (object key in this.LocalVariables.Keys)
            {
                newInstance.LocalVariables.Add(key, this.LocalVariables[key]);
            }


            newInstance.TempLocalVariableList = new ArrayList();

            if (this.TempLocalVariableList != null)
            {
                foreach (string item in this.TempLocalVariableList)
                    newInstance.TempLocalVariableList.Add(item);
            }

            return newInstance;
        }
    }

    /*
    public class Vector2
    {
        public double X = 0;
        public double Y = 0;

        public Vector2()
        {
        }

        public Vector2(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    public class Vector3
    {
        public double X = 0;
        public double Y = 0;
        public double Z = 0;

        public Vector3()
        {
        }

        public Vector3(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }

    public class Vector4
    {
        public double X = 0;
        public double Y = 0;
        public double Z = 0;
        public double W = 0;

        public Vector4()
        {
        }

        public Vector4(double x, double y, double z, double w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }
    }
	*/

    public class SPLHelper
    {
        public static GameObject CurrentLight = null;

        public static Vector2 GetVector2(string inputStr)
        {
            string[] arr;
            arr = inputStr.Split(new Char[] { ' ', ',', '\t' }, 200);

            ArrayList newArr = new ArrayList();

            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i].Trim() != string.Empty)
                    newArr.Add(arr[i].Trim());
            }

            float x = float.Parse(newArr[0].ToString().Trim());
            float y = float.Parse(newArr[1].ToString().Trim());

            return new Vector2(x, y);
        }


        public static Vector3 GetVector3(string inputStr)
        {
            string[] arr;
            arr = inputStr.Split(new Char[] { ' ', ',', '\t' }, 200);

            ArrayList newArr = new ArrayList();

            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i].Trim() != string.Empty)
                    newArr.Add(arr[i].Trim());
            }

            float x = float.Parse(newArr[0].ToString().Trim());
            float y = float.Parse(newArr[1].ToString().Trim());
            float z = float.Parse(newArr[2].ToString().Trim());

            return new Vector3(x, y, z);
        }


        public static Vector4 GetVector4(string inputStr)
        {
            string[] arr;
            arr = inputStr.Split(new Char[] { ' ', ',', '\t' }, 200);

            ArrayList newArr = new ArrayList();

            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i].Trim() != string.Empty)
                    newArr.Add(arr[i].Trim());
            }

            float x = float.Parse(newArr[0].ToString().Trim());
            float y = float.Parse(newArr[1].ToString().Trim());
            float z = float.Parse(newArr[2].ToString().Trim());
            float w = float.Parse(newArr[3].ToString().Trim());

            return new Vector4(x, y, z, w);
        }


        public static ArrayList GetStringList(string inputStr)
        {
            string[] arr;
            arr = inputStr.Split(new Char[] { ' ', ',', '\t' }, 200);

            ArrayList newArr = new ArrayList();

            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] != string.Empty)
                {
                    newArr.Add(arr[i]);
                }
            }


            return newArr;
        }
    }



    public class ScriptLineItem
    {
        public string ScriptLineText = string.Empty;
        public string FirstTokenOfLine = string.Empty;
    }


    public class BreakContinueItem
    {
        public bool IsBreakCalled = false;
        public bool IsContinueCalled = false;

    }

    public class ProcedureReturnItem
    {
        public string ProcedureName = string.Empty;
        public bool IsReturnCalled = false;
    }

    public class ProcedureRunningItem
    {
        public int RunningProcedureCount = 0;
        public bool ProcedureRunStart = false;
    }


    public class CommandItem : IDisposable
    {
        public string Name = string.Empty;

        public string FirstToken = string.Empty;

        public string Line = string.Empty;

        public string TargetType = string.Empty; //entity, joint

        public string Target = string.Empty;  //in case of joint, parent name is used as target name

        public string Action = string.Empty; //method or interface name

        public string RunMode = string.Empty; //"seq", "concur"

        public Dictionary<object, object> Params = new Dictionary<object, object>();

        public string ConditionExpr = string.Empty;  //If, While, Switch에서 사용

        public string ForStartExpr = string.Empty;

        public string ForStartExpr_FirstToken = string.Empty;

        public string ForEndCondition = string.Empty;

        public string ForStepExpr = string.Empty;

        public string SwitchExpr = string.Empty;

        public string CaseExpr = string.Empty;

        public Dictionary<object, object> LocalVariables = new Dictionary<object, object>(); //프로시저 안에 정의된 IF나 For의 경우, 상위 프로시저에서 정의된 변수를 이어 받는다.

        public Stack IsBreakContinueCalled = new Stack();

        public ProcedureReturnItem ProcedureReturn = new ProcedureReturnItem();

        //프로시저 정의할 때 파라메터를 정의함. 아래 ProcedureCallDataList와 짝이 맞아야 함.
        public ArrayList ProcedureParamList = new ArrayList();

        //프로시저 호출시 데이터를 넘겨줌. Call 명령에서 생성되어 넘겨짐
        public ArrayList ProcedureCallDataList = new ArrayList();

        public ArrayList ProcedureCallObjectList = new ArrayList();

        public ProcedureRunningItem ProcedureRunningInfo = null;

        public string Tag = "";

        public string RunResult = "";

        public CommandItem()
        {
        }


        public void Dispose()
        {
            if (Params != null)
                Params.Clear();

            if (LocalVariables != null)
                LocalVariables.Clear();

            if (IsBreakContinueCalled != null)
                IsBreakContinueCalled.Clear();

            if (ProcedureParamList != null)
                ProcedureParamList.Clear();

            if (ProcedureCallDataList != null)
                ProcedureCallDataList.Clear();

            if (ProcedureCallObjectList != null)
                ProcedureCallObjectList.Clear();
        }


        public CommandItem Clone()
        {
            CommandItem newInstance = new CommandItem();

            newInstance.FirstToken = this.FirstToken;
            newInstance.Line = this.Line;

            newInstance.Name = this.Name;
            newInstance.TargetType = this.TargetType;
            newInstance.Target = this.Target;
            newInstance.Action = this.Action;

            newInstance.RunMode = this.RunMode;

            newInstance.ConditionExpr = this.ConditionExpr;
            newInstance.ForStartExpr = this.ForStartExpr;
            newInstance.ForStartExpr_FirstToken = this.ForStartExpr_FirstToken;

            newInstance.ForEndCondition = this.ForEndCondition;
            newInstance.ForStepExpr = this.ForStepExpr;
            newInstance.SwitchExpr = this.SwitchExpr;
            newInstance.CaseExpr = this.CaseExpr;

            newInstance.ProcedureReturn = this.ProcedureReturn;
            newInstance.ProcedureParamList = this.ProcedureParamList;
            newInstance.ProcedureRunningInfo = this.ProcedureRunningInfo;

            foreach (object item in this.ProcedureCallDataList)
                newInstance.ProcedureCallDataList.Add(item);


            foreach (string key in this.Params.Keys)
            {
                newInstance.Params.Add(key, this.Params[key]);
            }

            return newInstance;
        }
    }


    public class CommandTree : IDisposable
    {
        public long ID = -1;

        public string Key = string.Empty;

        public string Name = string.Empty;

        public int NodeDepth = -1;

        public string FirstTokenOfLine = string.Empty;

        public CommandTree CurrentNode = null;

        public CommandTree Parent = null;

        public ArrayList Childs = null;

        public bool OpenFlag = false;

        public bool HasBrace = false;

        public bool IsConditionKeyword = false;

        public CommandItem commandItem = null;

        public int UnbindedChildCount = 0;

        public bool IsSetVariable = false;

        public bool IsExecutableTargetType = false;

        public void Dispose()
        {
            if (CurrentNode != null)
                CurrentNode.Dispose();

            if (Parent != null)
                Parent.Dispose();

            foreach (object item in Childs)
            {
                CommandTree child = (CommandTree)item;
                child.Dispose();
            }

            if (Childs != null)
                Childs.Clear();

            if (commandItem != null)
                commandItem.Dispose();
        }

    }

    public class CommandParsingTree : IDisposable
    {
        private long SeqNo = -1;

        public CommandTree CurrentNode = null;

        public CommandTree Parent = null;

        public ArrayList Childs = null;

        public void Dispose()
        {
            if (CurrentNode != null)
                CurrentNode.Dispose();

            if (Parent != null)
                Parent.Dispose();

            if (Childs != null)
                Childs.Clear();
        }

        public void AddNode(string name, string firstToken)
        {
            SeqNo++;

            CommandTree child = new CommandTree();
            child.ID = SeqNo;
            child.Name = name;
            child.FirstTokenOfLine = firstToken;
            child.Childs = new ArrayList();

            if (CurrentNode == null)
            {
                child.NodeDepth = 0;
                child.Parent = null;

                if (this.Childs == null)
                    this.Childs = new ArrayList();

                this.Childs.Add(child);

                CurrentNode = child;
            }
            else if (CurrentNode.Parent != null)
            {
                child.NodeDepth = CurrentNode.Parent.NodeDepth + 1;
                child.Parent = CurrentNode.Parent;

                if (CurrentNode.Parent.Childs == null)
                    CurrentNode.Parent.Childs = new ArrayList();

                CurrentNode.Parent.Childs.Add(child);
                CurrentNode = child;
            }
            else
            {
                child.NodeDepth = CurrentNode.NodeDepth + 1;
                child.Parent = CurrentNode;

                if (CurrentNode.Childs == null)
                    CurrentNode.Childs = new ArrayList();

                CurrentNode.Childs.Add(child);
                CurrentNode = child;
            }
        }


        public void AddChildNode(string name, string firstToken)
        {
            SeqNo++;

            CommandTree child = new CommandTree();
            child.ID = SeqNo;
            child.Name = name;
            child.FirstTokenOfLine = firstToken;
            child.Childs = new ArrayList();

            if (CurrentNode == null)
            {
                child.NodeDepth = 0;
                child.Parent = null;

                if (this.Childs == null)
                    this.Childs = new ArrayList();

                this.Childs.Add(child);
                CurrentNode = child;
            }
            else
            {
                child.NodeDepth = CurrentNode.NodeDepth + 1;
                child.Parent = CurrentNode;

                if (CurrentNode.Childs == null)
                    CurrentNode.Childs = new ArrayList();

                CurrentNode.Childs.Add(child);
                CurrentNode = child;
            }
        }

        public CommandTree GetRootNode()
        {
            if (this.CurrentNode != null)
            {
                while (this.CurrentNode.Parent != null)
                    MoveParentNode();

                return this.CurrentNode;
            }
            else
                return null;
        }

        public void MoveParentNode()
        {
            if (CurrentNode.Parent != null)
            {
                CurrentNode = CurrentNode.Parent;
            }
        }

        public void MoveParentNode(long id)
        {
            if (CurrentNode.ID == id)
                return;
            else
            {
                while (CurrentNode.Parent != null)
                {
                    CurrentNode = CurrentNode.Parent;

                    if (CurrentNode.ID == id)
                        return;
                }
            }
        }
    }


    public class ParsingHelper
    {
        public static string ConvFileNameFromHttpUrl(string url)
        {
            string res = url;

            res = res.Replace(':', '_');
            res = res.Replace('^', '_');
            res = res.Replace('/', '_');
            res = res.Replace('\\', '_');
            res = res.Replace(' ', '_');

            return res;
        }

        public static ArrayList GetParsedText(CommandParsingTree parsedTreeInstance)
        {
            ArrayList textLineList = new ArrayList();

            GetParsedTextInternal(parsedTreeInstance.GetRootNode(), textLineList, string.Empty);

            return textLineList;
        }


        private static void GetParsedTextInternal(CommandTree parsedTree, ArrayList textLineList, string indent)
        {
            if (parsedTree == null)
                return;

            if (parsedTree.Name == null)
                return;

            textLineList.Add(indent + parsedTree.Name);

            if (parsedTree.Childs != null)
            {
                foreach (CommandTree child in parsedTree.Childs)
                {
                    GetParsedTextInternal(child, textLineList, indent + "  ");
                }
            }
        }


        public static CommandParsingTree GetCommandTreeFromScriptList(ArrayList scriptList)
        {
            Queue tempQueue = InsertBraceForSingleConditionCmd(SeparateBrace(SeparateBlock(SeparateLines(scriptList))));

            CommandParsingTree cmdTree = new CommandParsingTree();
            cmdTree.AddNode("main", "main");

            BuildTree(tempQueue, cmdTree);

            return cmdTree;
        }


        public static Queue SeparateLines(ArrayList scriptList)
        {
            Queue tmpQueue = new Queue();

            //';' 인 경우 라인 분리
            foreach (string line in scriptList)
            {
                string firstToken = GetFirstToken(line.ToLower());

                if (firstToken != "for")
                {
                    //"  " 안에 ;가 포함되면 분리하면 않됨
                    //string[] tagArr;
                    //tagArr = line.Split(new Char[] { ';' }, 1000);

                    string subStr = string.Empty;
                    bool quotOpenFlag = false;

                    int subStartIndex = 0;

                    for (int i = 0; i < line.Length; i++)
                    {
                        if (line[i] == '\"')
                        {
                            if (!quotOpenFlag)
                                quotOpenFlag = true;
                            else
                                quotOpenFlag = false;
                        }
                        else if (line[i] == ';' && !quotOpenFlag)
                        {
                            subStr = line.Substring(subStartIndex, i - subStartIndex);
                            subStartIndex = i + 1;

                            tmpQueue.Enqueue(subStr.Trim());
                        }
                    }

                    //마지막 라인 처리
                    subStr = line.Substring(subStartIndex);
                    tmpQueue.Enqueue(subStr.Trim());
                }
                else
                    tmpQueue.Enqueue(line.Trim());
            }

            return tmpQueue;
        }

        public static Queue SeparateBlock(Queue orgQueue)
        {
            bool needSeparateFlag = true;

            Queue tempQueue = new Queue();
            Queue tempQueue2 = new Queue();

            while (orgQueue.Count > 0)
                tempQueue2.Enqueue(orgQueue.Dequeue());

            while (needSeparateFlag)
            {
                needSeparateFlag = false;

                tempQueue.Clear();

                while (tempQueue2.Count > 0)
                {
                    string tempLine = tempQueue2.Dequeue().ToString();
                    string firstToken = GetFirstToken(tempLine.ToLower());

                    ArrayList subLineList = new ArrayList();

                    if (firstToken == "if" || firstToken == "for" || firstToken == "while" || firstToken == "switch")
                    {
                        subLineList.Clear();

                        int endIndex = GetConditionExpressionEndIndex(tempLine);

                        if (endIndex > 0 && endIndex < (tempLine.Length - 1))
                        {
                            string subLine1 = tempLine.Substring(0, endIndex + 1);
                            string subLine2 = tempLine.Substring(endIndex + 1, tempLine.Length - endIndex - 1);

                            subLineList.Add(subLine1);

                            subLine2 = subLine2.Trim();

                            if (subLine2 != string.Empty)
                            {
                                subLineList.Add(subLine2);

                                needSeparateFlag = true;
                            }
                        }
                        else
                            subLineList.Add(tempLine);

                        foreach (string subline in subLineList)
                            tempQueue.Enqueue(subline.Trim());
                    }
                    else if (firstToken == "case" || firstToken == "default")
                    {
                        subLineList.Clear();

                        int endIndex = tempLine.IndexOf(':');

                        if (endIndex > 0 && endIndex < (tempLine.Length - 1))
                        {
                            string subLine1 = tempLine.Substring(0, endIndex + 1);
                            string subLine2 = tempLine.Substring(endIndex + 1, tempLine.Length - endIndex - 1);

                            subLineList.Add(subLine1);

                            subLine2 = subLine2.Trim();

                            if (subLine2 != string.Empty)
                            {
                                subLineList.Add(subLine2);

                                needSeparateFlag = true;
                            }
                        }
                        else
                            subLineList.Add(tempLine);

                        foreach (string subline in subLineList)
                            tempQueue.Enqueue(subline.Trim());

                    }
                    else if (firstToken == "else")
                    {
                        subLineList.Clear();

                        if (tempLine.Length > 4)
                        {
                            string subLine1 = tempLine.Substring(0, 4);
                            string subLine2 = tempLine.Substring(4, tempLine.Length - 4);

                            subLineList.Add(subLine1);

                            subLine2 = subLine2.Trim();

                            if (subLine2 != string.Empty)
                            {
                                subLineList.Add(subLine2);

                                needSeparateFlag = true;
                            }
                        }
                        else
                            subLineList.Add(tempLine);

                        foreach (string subline in subLineList)
                            tempQueue.Enqueue(subline.Trim());
                    }
                    else
                    {
                        tempQueue.Enqueue(tempLine.Trim());
                    }

                }

                tempQueue2.Clear();

                while (tempQueue.Count > 0)
                    tempQueue2.Enqueue(tempQueue.Dequeue());
            }

            return tempQueue2;
        }


        public static Queue SeparateBrace(Queue orgQueue)
        {
            Queue tempQueue = new Queue();

            bool PreviousIsConditionKeywordFlag = false;
            //string PreviousFirstToken = string.Empty;
            //string cumulatedStr = string.Empty;
            int BlockOpenCount = 0;

            while (orgQueue.Count > 0)
            {
                string tempLine = orgQueue.Dequeue().ToString();
                //string nextLine = string.Empty;

                //if (orgQueue.Count > 0)
                //    nextLine = orgQueue.Peek().ToString();

                string firstToken = GetFirstToken(tempLine.ToLower());

                //string nextFirstToken = string.Empty;

                //if (nextLine != string.Empty)
                //    nextFirstToken = GetFirstToken(nextLine.ToLower());


                if (firstToken == "{" && PreviousIsConditionKeywordFlag)
                {
                    tempQueue.Enqueue("{");
                    BlockOpenCount = 1;

                    tempLine = tempLine.Trim();
                    tempLine = tempLine.Substring(1, tempLine.Length - 1);
                    tempLine = tempLine.Trim();

                    for (int i = 0; i < tempLine.Length; i++)
                    {
                        if (tempLine[i] == '{')
                            BlockOpenCount++;
                        else if (tempLine[i] == '}')
                        {
                            BlockOpenCount--;

                            if (BlockOpenCount == 0)
                            {
                                if (i < (tempLine.Length - 1))
                                {
                                    string str1 = tempLine.Substring(0, i);
                                    string str2 = tempLine.Substring(i + 1, (tempLine.Length - i - 1));

                                    tempQueue.Enqueue(str1);
                                    tempQueue.Enqueue("}");
                                    tempQueue.Enqueue(str2);
                                }
                            }
                        }
                    }

                    if (BlockOpenCount > 0 && tempLine != string.Empty)
                        tempQueue.Enqueue(tempLine);
                }
                else if (BlockOpenCount > 0)
                {
                    for (int i = 0; i < tempLine.Length; i++)
                    {
                        if (tempLine[i] == '{')
                            BlockOpenCount++;
                        else if (tempLine[i] == '}')
                        {
                            BlockOpenCount--;

                            if (BlockOpenCount == 0)
                            {
                                if (i < (tempLine.Length - 1))
                                {
                                    string str1 = tempLine.Substring(0, i);
                                    string str2 = tempLine.Substring(i + 1, (tempLine.Length - i - 1));

                                    tempQueue.Enqueue(str1);
                                    tempQueue.Enqueue("}");
                                    tempQueue.Enqueue(str2);
                                }
                                else
                                    tempQueue.Enqueue("}");
                            }
                        }
                    }

                    if (BlockOpenCount > 0 && tempLine != string.Empty)
                        tempQueue.Enqueue(tempLine);
                }
                else
                    tempQueue.Enqueue(tempLine);

                PreviousIsConditionKeywordFlag = CheckConditionKeyword(firstToken);
                //PreviousFirstToken = firstToken;
            }

            return tempQueue;

        }



        public static Queue InsertBraceForSingleConditionCmd(Queue orgQueue)
        {
            Queue tempQueue = new Queue();

            //bool PreviousIsConditionKeywordFlag = false;
            //string PreviousFirstToken = string.Empty;


            Stack OpenedSingleConditionCmdList = new Stack();

            while (orgQueue.Count > 0)
            {
                string tempLine = orgQueue.Dequeue().ToString();
                string nextLine = string.Empty;

                if (orgQueue.Count > 0)
                    nextLine = orgQueue.Peek().ToString();

                string firstToken = GetFirstToken(tempLine.ToLower());

                string nextFirstToken = string.Empty;

                if (nextLine != string.Empty)
                    nextFirstToken = GetFirstToken(nextLine.ToLower());



                //본처리

                if (firstToken == "{")
                {
                    OpenedSingleConditionCmdList.Push("{");

                    tempQueue.Enqueue(tempLine);
                }
                else if (firstToken == "}")
                {
                    //쌓여있는 비정상 오픈 제거
                    while (OpenedSingleConditionCmdList.Count > 0)
                    {
                        string nextOpenedStr = OpenedSingleConditionCmdList.Peek().ToString();

                        if (nextOpenedStr != "{" && nextOpenedStr != "procedure")
                            OpenedSingleConditionCmdList.Pop();
                        else
                        {
                            OpenedSingleConditionCmdList.Pop();
                            break;
                        }
                    }

                    tempQueue.Enqueue(tempLine);
                }
                else
                {
                    tempQueue.Enqueue(tempLine);
                }


                //후처리
                if (CheckConditionKeyword(firstToken) && firstToken != "case" && firstToken != "default")
                {
                    if (nextFirstToken != "{" && nextFirstToken != "}")
                    {
                        //{..}가 없는 조건문에 연결된 단일 명령문일 경우
                        if (firstToken != "procedure")
                        {
                            tempQueue.Enqueue("{");
                            OpenedSingleConditionCmdList.Push(firstToken);
                        }
                    }
                }
                else if (firstToken == "{")
                {
                }
                else if (firstToken == "}")
                {
                    if (nextFirstToken == "else")
                    {
                    }
                    else
                    {
                        while (OpenedSingleConditionCmdList.Count > 0)
                        {
                            string nextOpenedStr = OpenedSingleConditionCmdList.Peek().ToString();

                            if (nextOpenedStr != "{")
                            {
                                OpenedSingleConditionCmdList.Pop();
                                tempQueue.Enqueue("}");

                                if (nextOpenedStr == "if" && nextFirstToken == "else")
                                    break;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    while (OpenedSingleConditionCmdList.Count > 0)
                    {
                        string nextOpenedStr = OpenedSingleConditionCmdList.Peek().ToString();

                        if (nextOpenedStr != "{")
                        {
                            OpenedSingleConditionCmdList.Pop();
                            tempQueue.Enqueue("}");

                            if (nextOpenedStr == "if" && nextFirstToken == "else")
                                break;
                        }
                        else
                        {
                            break;
                        }
                    }
                }


                //PreviousIsConditionKeywordFlag = CheckConditionKeyword(firstToken);
                //PreviousFirstToken = firstToken;
            }

            return tempQueue;

        }



        public static void BuildTree(Queue orgQueue, CommandParsingTree targetTree)
        {
            bool CurrentIsConditionKeywordFlag = false;
            bool PreviousIsConditionKeywordFlag = false;
            string PreviousFirstToken = string.Empty;

            bool CaseKeywordFlag = false;
            bool CaseBraceOpenFlag = false;

            while (orgQueue.Count > 0)
            {
                string tempLine = orgQueue.Dequeue().ToString();
                string firstToken = GetFirstToken(tempLine.ToLower());

                //string nextToken = string.Empty;

                //if (orgQueue.Count > 0)
                //    nextToken = GetFirstToken(orgQueue.Peek().ToString().ToLower());


                CurrentIsConditionKeywordFlag = CheckConditionKeyword(firstToken);


                if (targetTree.CurrentNode.Parent != null)
                {
                    if (targetTree.CurrentNode.Parent.FirstTokenOfLine == "switch" && firstToken != "case" && firstToken != "default" && !CaseKeywordFlag && firstToken != "{" && firstToken != "}")
                    {
                        targetTree.MoveParentNode();
                    }
                }


                targetTree.AddNode(tempLine, firstToken);



                if (firstToken == "case" || firstToken == "default")
                    CaseKeywordFlag = true;

                if (CurrentIsConditionKeywordFlag)
                {
                    targetTree.CurrentNode.IsConditionKeyword = true;

                    if (firstToken != "procedure")
                        targetTree.CurrentNode.OpenFlag = true;


                    targetTree.AddChildNode(string.Empty, string.Empty);
                }
                else if (firstToken == "{")
                {
                    if (targetTree.CurrentNode.Parent != null)
                        targetTree.CurrentNode.Parent.HasBrace = true;

                    if (PreviousFirstToken == "case" || PreviousFirstToken == "default")
                        CaseBraceOpenFlag = true;
                }
                else if (firstToken == "end")
                {
                    targetTree.MoveParentNode();
                    targetTree.CurrentNode.OpenFlag = false;
                }
                else if (firstToken == "}" && !CaseKeywordFlag)
                {
                    targetTree.MoveParentNode();

                    targetTree.CurrentNode.OpenFlag = false;
                }
                else if (firstToken == "}" && CaseBraceOpenFlag)
                {
                    if (targetTree.CurrentNode.Parent != null)
                    {
                        if (targetTree.CurrentNode.Parent.FirstTokenOfLine == "case" || targetTree.CurrentNode.Parent.FirstTokenOfLine == "default")
                        {
                            CaseBraceOpenFlag = false;

                            if (!CaseKeywordFlag)
                                targetTree.MoveParentNode();
                        }
                    }
                }
                else if (PreviousIsConditionKeywordFlag && firstToken != "{" && !CurrentIsConditionKeywordFlag && !CaseKeywordFlag && PreviousFirstToken != "procedure")
                {
                    //단일 명령어 처리
                    targetTree.MoveParentNode();

                    targetTree.CurrentNode.OpenFlag = false;

                    while (targetTree.CurrentNode.Parent != null)
                    {
                        if (!targetTree.CurrentNode.Parent.HasBrace && targetTree.CurrentNode.Parent.OpenFlag)
                        {
                            targetTree.MoveParentNode();
                            targetTree.CurrentNode.OpenFlag = false;
                        }
                        else
                            break;
                    }
                }
                else if ((firstToken == "break" || firstToken == "return") && CaseKeywordFlag)
                {
                    CaseKeywordFlag = false;

                    if (!CaseBraceOpenFlag)
                        targetTree.MoveParentNode();
                }

                PreviousIsConditionKeywordFlag = CheckConditionKeyword(firstToken);
                PreviousFirstToken = firstToken;
            }
        }


        public static bool CheckConditionKeyword(string cmd)
        {
            if (cmd == string.Empty)
                return false;

            switch (cmd)
            {
                case "procedure":
                    return true;
                case "if":
                    return true;
                case "else":
                    return true;
                case "for":
                    return true;
                case "while":
                    return true;
                case "switch":
                    return true;
                case "case":
                    return true;
                case "default":
                    return true;
                default:
                    return false;
            }
        }

        public static string GetConditionExpression(string line)
        {
            if (line == string.Empty)
                return string.Empty;

            int startInd = -1;
            int endInd = -1;
            int openCount = 0;

            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '(')
                {
                    if (startInd == -1)
                        startInd = i;

                    openCount++;
                }

                if (line[i] == ')')
                {
                    openCount--;

                    if (openCount == 0 && endInd == -1)
                        endInd = i;
                }
            }

            if (startInd >= 0 && endInd > 0 && endInd > startInd)
            {
                return line.Substring(startInd + 1, endInd - startInd - 1);
            }
            else
                return string.Empty;
        }

        public static int GetConditionExpressionEndIndex(string line)
        {
            if (line == string.Empty)
                return -1;

            int startInd = -1;
            int endInd = -1;
            int openCount = 0;

            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '(')
                {
                    if (startInd == -1)
                        startInd = i;

                    openCount++;
                }

                if (line[i] == ')')
                {
                    openCount--;

                    if (openCount == 0 && endInd == -1)
                        endInd = i;
                }
            }

            return endInd;
        }

        public static string GetDoubleQuoteTrimedString(string line)
        {
            if (line == string.Empty)
                return string.Empty;

            int startInd = -1;
            int endInd = -1;
            bool openFlag = false;

            string tmpLine = line;

            int loopCnt = 0;

            while (loopCnt < 100)
            {
                try
                {
                    startInd = -1;
                    endInd = -1;
                    openFlag = false;

                    for (int i = 0; i < tmpLine.Length; i++)
                    {
                        if (tmpLine[i] == '\"')
                        {
                            if (openFlag)
                            {
                                if (endInd == -1)
                                {
                                    endInd = i;
                                    openFlag = false;
                                }
                            }
                            else
                            {
                                if (startInd == -1)
                                {
                                    startInd = i;
                                    openFlag = true;
                                }
                            }
                        }
                    }

                    if (startInd >= 0 && endInd > 0 && endInd > startInd)
                    {
                        tmpLine = tmpLine.Substring(0, startInd) + tmpLine.Substring(endInd + 1);
                    }
                    else
                        break;
                }
                catch { }

                loopCnt++;
            }

            return tmpLine;
        }


        static Dictionary<object, object> _stringToIntMapping = new Dictionary<object, object>();

        public static string GetStringWithUniquePostfix(string name)
        {
            if (name == string.Empty)
                return string.Empty;

            int count = 0;

            if (_stringToIntMapping.ContainsKey(name))
            {
                count = (int)_stringToIntMapping[name];
                _stringToIntMapping[name] = count + 1;
            }
            else
            {
                _stringToIntMapping[name] = 1;
            }

            return name + (count + 1).ToString();
        }


        public static Dictionary<object, object> GetTokenListWithHashtable(string line)
        {
            Dictionary<object, object> tempList = new Dictionary<object, object>();

            string[] arrWith = line.Split(new Char[] { ' ', '\t', '+', '-', '=', ':', '/' });

            for (int i = 0; i < arrWith.Length; i++)
            {
                string trimStr = arrWith[i].Trim();

                if (trimStr != string.Empty)
                {
                    if (!tempList.ContainsKey(trimStr))
                        tempList.Add(trimStr, string.Empty);
                }
            }

            return tempList;
        }

        public static ArrayList GetTokenList(string line)
        {
            ArrayList tempList = new ArrayList();

            string[] arrWith = line.Split(new Char[] { ' ', '\t', '+', '-', '=', ':', '/' });

            for (int i = 0; i < arrWith.Length; i++)
                if (arrWith[i].Trim() != string.Empty)
                    tempList.Add(arrWith[i].Trim());

            return tempList;
        }


        public static string GetFirstTargetParam(string firstToken, ArrayList newArr)
        {
            if (newArr.Count > 0 && firstToken != string.Empty)
            {
                string[] firstArr;
                firstArr = newArr[0].ToString().Split(new Char[] { ' ', '\t' });

                ArrayList tmpList = new ArrayList();
                for (int i = 0; i < firstArr.Length; i++)
                {
                    if (firstArr[i].Trim() != string.Empty)
                        tmpList.Add(firstArr[i].Trim());
                }

                if (tmpList.Count == 0)
                    return string.Empty;

                if (tmpList[0].ToString().ToLower() == firstToken && tmpList.Count >= 2)
                {
                    for (int i = 1; i < tmpList.Count; i++)
                    {
                        if (tmpList[i].ToString().IndexOf("/") < 0 && tmpList[i].ToString().Trim() != string.Empty)
                        {
                            string firstParam = tmpList[i].ToString();

                            if (firstParam.IndexOf(":") >= 0)
                            {
                                int eqInd = firstParam.IndexOf(":");

                                if (eqInd == 0)
                                    firstParam = string.Empty;
                                else
                                    firstParam = firstParam.Substring(0, eqInd);
                            }

                            if (firstParam.IndexOf("(") >= 0)
                            {
                                int eqInd = firstParam.IndexOf("(");

                                if (eqInd == 0)
                                    firstParam = string.Empty;
                                else
                                    firstParam = firstParam.Substring(0, eqInd);
                            }

                            if (firstParam.IndexOf(")") >= 0)
                            {
                                int eqInd = firstParam.IndexOf(")");

                                if (eqInd == 0)
                                    firstParam = string.Empty;
                                else
                                    firstParam = firstParam.Substring(0, eqInd);
                            }

                            if (firstParam.IndexOf("=") >= 0)
                            {
                                int eqInd = firstParam.IndexOf("=");

                                if (eqInd == 0)
                                    firstParam = string.Empty;
                                else
                                    firstParam = firstParam.Substring(0, eqInd);
                            }


                            return RestoreSlashString(firstParam);
                        }
                    }
                }
            }

            return string.Empty;
        }



        //New added 2016.07.04 for Arduino Simulation
        public static string GetProcedureName(string firstToken, string org_line)
        {
            if (string.IsNullOrEmpty(org_line))
                return string.Empty;

            string[] arr = org_line.Split(new char[] { ' ', '\t', '(', ')', '/', ':', ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (arr == null)
                return string.Empty;

            if (firstToken.ToLower() == arr[0].ToLower() && arr.Length >= 2)
            {
                if (IsTypeCmd(arr[1]) && arr.Length >= 3)
                    return arr[2];
                else
                    return arr[1];
            }

            return string.Empty;
        }



        public static string GetFirstTargetParamForFilePath(string firstToken, List<string> newArr)
        {
            if (newArr.Count > 0 && firstToken != string.Empty)
            {
                return GetFirstTargetParamForFilePath(firstToken, newArr[0].ToString());
            }

            return string.Empty;
        }

        public static string GetFirstTargetParamForFilePath(string firstToken, string line)
        {
            if (line != string.Empty && firstToken != string.Empty)
            {
                string[] firstArr;
                firstArr = line.Split(new Char[] { ' ', '\t' });

                if (firstArr[0].ToLower() == firstToken && firstArr.Length >= 2)
                {
                    for (int i = 1; i < firstArr.Length; i++)
                    {
                        if (firstArr[i].IndexOf("/") < 0 && firstArr[i].IndexOf(":") < 0 && firstArr[i].Trim() != string.Empty)
                        {
                            string firstParam = firstArr[i];

                            return RestoreSlashString(firstParam);
                        }
                    }
                }
            }

            return string.Empty;
        }


        public static string GetFirstTargetParamForWebPath(string firstToken, string line)
        {
            if (line != string.Empty && firstToken != string.Empty)
            {
                string[] firstArr;
                firstArr = line.Split(new Char[] { ' ', '\t' });

                if (firstArr[0].ToLower() == firstToken && firstArr.Length >= 2)
                {
                    for (int i = 1; i < firstArr.Length; i++)
                    {
                        if (firstArr[i].IndexOf("/") < 0 && firstArr[i].Trim() != string.Empty)
                        {
                            string firstParam = firstArr[i];

                            return RestoreSlashString(firstParam);
                        }
                    }
                }
            }

            return string.Empty;
        }


        public static string GetQutationTrimString(string src)
        {
            string res = src;
            res = res.TrimStart('\"');
            res = res.TrimEnd('\"');

            return res;
        }


        public static string ReplaceSlashString(string src)
        {
            if (src.Trim() == string.Empty)
                return string.Empty;


            char[] arr = src.ToCharArray();

            int openInd = src.IndexOf('"', 0);
            int closeInd = -1;

            while (openInd >= 0)
            {
                closeInd = src.IndexOf('"', openInd + 1);

                if (closeInd >= 0)
                {
                    for (int i = openInd; i <= closeInd; i++)
                    {
                        if (arr[i] == '/')
                            arr[i] = (char)0x01;
                        else if (arr[i] == ':')
                            arr[i] = (char)0x02;
                    }

                    openInd = src.IndexOf('"', closeInd + 1);
                }
                else
                    openInd = -1;
            }

            return new string(arr).Trim();
        }

        public static string Replace(string str, char src, char target)
        {
            return Replace(str, src.ToString(), target.ToString());
        }

        public static string Replace(string str, string src, string target)
        {
            if (str == string.Empty || src == string.Empty || target == string.Empty)
                return str;


            int src_len = src.Length;
            //int target_len = target.Length;

            while (str.IndexOf(src) >= 0)
            {
                int ind = str.IndexOf(src);
                string s1 = str.Substring(0, ind);


                int ind2 = ind + src_len;
                string s2 = string.Empty;

                if (ind2 < str.Length)
                    s2 = str.Substring(ind2);

                str = s1 + target + s2;
            }

            return str;
        }

        public static string RestoreSlashString(string str)
        {
            str = str.Trim();

            if (str == string.Empty)
                return string.Empty;

            string res = string.Empty;

            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == (char)0x01)
                    res += '/';
                else if (str[i] == (char)0x02)
                    res += ':';
                else
                    res += str[i];
            }


            if (res.IndexOf("\"") == 0)
                res = res.Substring(1);

            if (res.IndexOf("\"") == (res.Length - 1))
                res = res.Substring(0, res.Length - 1);

            return res;
        }

        //2016.09.25 ':' 조건 추가
        public static string RestoreSlashWithoutTrim(string str)
        {
            str = str.Trim();

            if (str == string.Empty)
                return string.Empty;

            string res = string.Empty;

            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == (char)0x01)
                    res += '/';
                else if (str[i] == (char)0x02)
                    res += ':';
                else
                    res += str[i];
            }

            return res;
        }

        public static string GetFirstToken(string line)
        {
            if (line == string.Empty)
                return string.Empty; ;


            string firstToken = string.Empty;

            ArrayList tokenList = SPLHelper.GetStringList(line);

            if (tokenList.Count > 0)
                firstToken = tokenList[0].ToString();

            if (firstToken.IndexOf("=") >= 0)
            {
                int eqInd = firstToken.IndexOf("=");

                if (eqInd == 0)
                    firstToken = string.Empty;
                else
                    firstToken = firstToken.Substring(0, eqInd);
            }

            if (firstToken.IndexOf("(") >= 0)
            {
                int eqInd = firstToken.IndexOf("(");

                if (eqInd == 0)
                    firstToken = string.Empty;
                else
                    firstToken = firstToken.Substring(0, eqInd);
            }

            if (firstToken.IndexOf(":") >= 0)
            {
                int eqInd = firstToken.IndexOf(":");

                if (eqInd == 0)
                    firstToken = string.Empty;
                else
                    firstToken = firstToken.Substring(0, eqInd);
            }

            if (firstToken.IndexOf("+") >= 0)
            {
                int eqInd = firstToken.IndexOf("+");

                if (eqInd == 0)
                    firstToken = string.Empty;
                else
                    firstToken = firstToken.Substring(0, eqInd);
            }

            if (firstToken.IndexOf("-") >= 0)
            {
                int eqInd = firstToken.IndexOf("-");

                if (eqInd == 0)
                    firstToken = string.Empty;
                else
                    firstToken = firstToken.Substring(0, eqInd);
            }

            if (firstToken.IndexOf("{") == 0)
                firstToken = "{";
            else if (firstToken.IndexOf("}") == 0)
                firstToken = "}";
            else
            {
                if (firstToken.IndexOf("{") >= 0)
                {
                    int eqInd = firstToken.IndexOf("{");
                    firstToken = firstToken.Substring(0, eqInd);
                }
            }

            return firstToken;
        }


        //2016.07.03

        public static string GetTrimString(string data)
        {
            if (string.IsNullOrEmpty(data))
                return string.Empty;

            string data_trim = data.TrimEnd(new char[] { ';', ' ', '\t', '{', '}', '\r', '\n' });
            data_trim = data_trim.TrimStart(new char[] { ';', ' ', '\t', '{', '}', '\r', '\n' });

            return data_trim;
        }

        public static string GetTrimSpaceTab(string data)
        {
            if (string.IsNullOrEmpty(data))
                return string.Empty;

            string data_trim = data.TrimEnd(new char[] { ' ', '\t' });

            //2015.11.24
            //Indentation 유지를 위해 앞쪽 Trim은 실행하지 않음
            //data_trim = data_trim.TrimStart(new char[] { ' ', '\t' });

            return data_trim;
        }


        //2015.11.24
        //indentation 유지를 위해 명령어 앞쭉의 공백 및 탭 문자열을 추출함
        public static string GetIndentationString(string data)
        {
            if (string.IsNullOrEmpty(data))
                return string.Empty;

            int non_space_tab_pos = 0;

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] != ' ' && data[i] != '\t')
                {
                    non_space_tab_pos = i;
                    break;
                }
            }

            if (non_space_tab_pos > 0)
                return data.Substring(0, non_space_tab_pos);
            else
                return string.Empty;
        }

        public static bool IsNumeric(string data)
        {
            bool res = false;

            try
            {
                string data_trim = GetTrimString(data);

                float d = 0;
                res = float.TryParse(data_trim, out d);
            }
            catch { }

            return res;
        }

        public static bool IsChar(string data)
        {
            bool res = false;

            try
            {
                if (string.IsNullOrEmpty(data))
                    return res;

                string data_trim = GetTrimString(data);

                if (data_trim.Length >= 2 && data_trim.StartsWith("\'") && data_trim.EndsWith("\'"))
                    res = true;
            }
            catch { }

            return res;
        }

        public static bool IsString(string data)
        {
            bool res = false;

            try
            {
                if (string.IsNullOrEmpty(data))
                    return res;

                string data_trim = GetTrimString(data);

                if (data_trim.Length >= 2 && data_trim.StartsWith("\"") && data_trim.EndsWith("\""))
                    res = true;
            }
            catch { }

            return res;
        }


        public static bool IsBool(string data)
        {
            bool res = false;

            try
            {
                if (string.IsNullOrEmpty(data))
                    return res;

                string data_trim = GetTrimString(data);
                data_trim = data_trim.ToLower();

                if (data_trim == "true" || data_trim == "false")
                    res = true;
            }
            catch { }

            return res;
        }

        public static bool IsFunctionDefinition(string line)
        {
            bool res = false;

            //함수는 반드시 타입 정의가 있어야 하며, ()를 포함하고 있어야 함. "="나 "."를 포함하고 있으면 않됨

            if (!line.Contains("=") && !line.Contains(".") && line.Contains("(") && line.Contains(")") && !line.Contains(";"))
            {
                string[] tockens = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                if (tockens != null && tockens.Length >= 2)
                {
                    string first_tocken = tockens[0];
                    string second_tocken = tockens[1];

                    //타입 정의를 맨 앞에 포함하고 있으면 함수 정의 문장이며, 이 경우 맨 뒤에 ";"를 붙이면 않됨
                    if (IsTypeCmd(first_tocken) || IsTypeCmd(first_tocken + " " + second_tocken))
                        res = true;
                }
            }

            return res;
        }


        public static string[] IsFunctionDefinition2(string line)
        {
            string trim_line = GetTrimString(line);

            if (string.IsNullOrEmpty(trim_line))
                return null;

            int open_p = line.IndexOf('(');
            int close_p = line.IndexOf(')');

            if (open_p < 0)
                return null;

            if (close_p < 0)
                return null;

            if (close_p < open_p)
                return null;

            string proc_def = trim_line.Substring(0, open_p).Trim();

            string[] res = null;

            string[] tockens = proc_def.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            if (tockens != null && tockens.Length == 2)
            {
                //if (IsTypeCmd(tockens[0]))
                res = new string[2] { tockens[0], tockens[1] };
            }
            else if (tockens != null && tockens.Length >= 3)
            {
                //if (IsTypeCmd(tockens[0] + " " + tockens[1]))
                //res = new string[2] { tockens[0] + " " + tockens[1], tockens[2] };

                string var_name = string.Empty;
                for (int i = 2; i < tockens.Length - 1; i++)
                    var_name = var_name + tockens[i];

                res = new string[2] { tockens[0] + " " + tockens[1], var_name };
            }

            return res;
        }


        public static bool IsSetupFunction(string line)
        {
            string trim_line = GetTrimString(line);

            if (string.IsNullOrEmpty(trim_line))
                return false;

            string res = string.Empty;

            for (int i = 0; i < trim_line.Length; i++)
            {
                if (trim_line[i] != ' ' && trim_line[i] != '\t')
                    res += trim_line[i];
            }

            if (res == "voidsetup()")
                return true;
            else
                return false;
        }

        public static bool IsLoopFunction(string line)
        {
            string trim_line = GetTrimString(line);

            if (string.IsNullOrEmpty(trim_line))
                return false;

            string res = string.Empty;

            for (int i = 0; i < trim_line.Length; i++)
            {
                if (trim_line[i] != ' ' && trim_line[i] != '\t')
                    res += trim_line[i];
            }

            if (res == "voidloop()")
                return true;
            else
                return false;
        }


        public static string[] IsVariableDefinition(string line)
        {
            string trim_line = GetTrimString(line);

            if (string.IsNullOrEmpty(trim_line))
                return null;

            string[] res = null;

            string[] tockens = trim_line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            if (tockens != null && tockens.Length == 2)
            {
                //if (IsTypeCmd(tockens[0]))
                res = new string[2] { tockens[0], tockens[1] };
            }
            else if (tockens != null && tockens.Length >= 3)
            {
                //if (IsTypeCmd(tockens[0] + " " + tockens[1]))
                //res = new string[2] { tockens[0] + " " + tockens[1], tockens[2] };

                string var_name = string.Empty;
                for (int i = 2; i < tockens.Length - 1; i++)
                    var_name = var_name + tockens[i];

                res = new string[2] { tockens[0] + " " + tockens[1], var_name };
            }

            return res;
        }

        public static string ReplaceReservedCharWithInData(string line, char t1, char r1)
        {
            string res = line;

            if (string.IsNullOrEmpty(line))
                return res;

            string target_str = "";
            target_str += t1;

            while (res.Contains(target_str))
            {
                bool char_open = false;
                bool str_open = false;

                int single_quote_cnt = 0;
                int single_quote_pos1 = 0;
                int single_quote_pos2 = 0;

                int double_quote_cnt = 0;
                int double_quote_pos1 = 0;
                int double_quote_pos2 = 0;

                int target_pos = res.IndexOf(t1);

                if (target_pos < 0)
                    break;

                for (int i = 0; i < res.Length; i++)
                {
                    if (res[i] == '\'')
                    {
                        single_quote_cnt++;

                        if (char_open == false)
                        {
                            single_quote_pos1 = i;
                            char_open = true;
                        }
                        else if (single_quote_pos2 == 0)
                            single_quote_pos2 = i;
                    }
                    else if (res[i] == '\"')
                    {
                        double_quote_cnt++;

                        if (str_open == false)
                        {
                            double_quote_pos1 = i;
                            str_open = true;
                        }
                        else if (double_quote_pos2 == 0)
                            double_quote_pos2 = i;
                    }
                }

                //문자열이나 문자 정의 안에 포함되어 있으면 false로 리턴함
                if (single_quote_cnt == 2 || double_quote_cnt == 2)
                {
                    if (single_quote_cnt == 2 && target_pos > single_quote_pos1 && target_pos < single_quote_pos2)
                    {
                        res = res.Substring(0, target_pos) + r1 + res.Substring(target_pos + 1);
                    }
                    else if (double_quote_cnt == 2 && target_pos > double_quote_pos1 && target_pos < double_quote_pos2)
                    {
                        res = res.Substring(0, target_pos) + r1 + res.Substring(target_pos + 1);
                    }
                    else
                    {
                        //문자열 바깥에 있으면 일단 0X04로 치환해 놓음
                        res = res.Substring(0, target_pos) + (char)0x04 + res.Substring(target_pos + 1);
                    }
                }
                else
                {
                    //문자열이 없는 경우 0x04로 치환
                    res = res.Substring(0, target_pos) + (char)0x04 + res.Substring(target_pos + 1);
                }
            }


            //0x04을 원래 값으로 대체시킴
            string res2 = string.Empty;

            for (int i = 0; i < res.Length; i++)
            {
                if (res[i] == (char)0x04)
                    res2 += t1;
                else
                    res2 += res[i];
            }

            res = res2;


            return res;
        }



        public static string GetCommentRemovedString(string line)
        {
            string res = line;

            //return res;

            if (string.IsNullOrEmpty(line))
                return res;

            string target_str = "/";

            while (res.Contains(target_str))
            {
                bool char_open = false;
                bool str_open = false;

                int single_quote_cnt = 0;
                int single_quote_pos1 = 0;
                int single_quote_pos2 = 0;

                int double_quote_cnt = 0;
                int double_quote_pos1 = 0;
                int double_quote_pos2 = 0;

                int target_pos = res.IndexOf("/");

                if (target_pos < 0)
                    break;

                for (int i = 0; i < res.Length; i++)
                {
                    if (res[i] == '\'')
                    {
                        single_quote_cnt++;

                        if (char_open == false)
                        {
                            single_quote_pos1 = i;
                            char_open = true;
                        }
                        else if (single_quote_pos2 == 0)
                            single_quote_pos2 = i;
                    }
                    else if (res[i] == '\"')
                    {
                        double_quote_cnt++;

                        if (str_open == false)
                        {
                            double_quote_pos1 = i;
                            str_open = true;
                        }
                        else if (double_quote_pos2 == 0)
                            double_quote_pos2 = i;
                    }
                }

                //문자열이나 문자 정의 안에 포함되어 있으면 false로 리턴함
                if (single_quote_cnt == 2 || double_quote_cnt == 2)
                {
                    if (single_quote_cnt == 2 && target_pos > single_quote_pos1 && target_pos < single_quote_pos2)
                    {
                        res = res.Substring(0, target_pos) + (char)0x03 + res.Substring(target_pos + 1);
                    }
                    else if (double_quote_cnt == 2 && target_pos > double_quote_pos1 && target_pos < double_quote_pos2)
                    {
                        res = res.Substring(0, target_pos) + (char)0x03 + res.Substring(target_pos + 1);
                    }
                    else
                    {
                        //문자열 바깥에 있는 것을 0x04로 치환
                        res = res.Substring(0, target_pos) + (char)0x04 + res.Substring(target_pos + 1);
                    }
                }
                else
                {
                    //문자열이 없는 경우 0x04로 치환
                    res = res.Substring(0, target_pos) + (char)0x04 + res.Substring(target_pos + 1);
                }
            }


            //만약 0x04가 연속으로 2번 포함되어 있으면 주석임


            target_str = string.Empty;
            target_str = target_str + (char)0x04 + (char)0x04;

            if (res.Contains(target_str))
            {
                int target_pos = res.IndexOf(target_str);

                //주석 앞에까지만 잘라냄
                if (target_pos >= 0)
                    res = res.Substring(0, target_pos);
            }


            //원래의 0x03 또는 0x04을 "/"로 대체시킴

            string res2 = string.Empty;

            for (int i = 0; i < res.Length; i++)
            {
                if (res[i] == (char)0x03 || res[i] == (char)0x04)
                    res2 += '/';
                else
                    res2 += res[i];
            }

            res = res2;

            return res;
        }



        public static bool IsIncludeAssignmentChar(string line)
        {
            bool res = false;

            if (string.IsNullOrEmpty(line))
                return res;

            if (line.Contains("="))
            {
                bool char_open = false;
                bool str_open = false;

                int single_quote_cnt = 0;
                int single_quote_pos1 = 0;
                int single_quote_pos2 = 0;

                int double_quote_cnt = 0;
                int double_quote_pos1 = 0;
                int double_quote_pos2 = 0;

                int target_pos = line.IndexOf('=');

                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i] == '\'')
                    {
                        single_quote_cnt++;

                        if (char_open == false)
                        {
                            single_quote_pos1 = i;
                            char_open = true;
                        }
                        else if (single_quote_pos2 == 0)
                            single_quote_pos2 = i;
                    }
                    else if (line[i] == '\"')
                    {
                        double_quote_cnt++;

                        if (str_open == false)
                        {
                            double_quote_pos1 = i;
                            str_open = true;
                        }
                        else if (double_quote_pos2 == 0)
                            double_quote_pos2 = i;
                    }
                }

                //문자열이나 문자 정의 안에 포함되어 있으면 false로 리턴함
                if (single_quote_cnt == 2 || double_quote_cnt == 2)
                {
                    if (single_quote_cnt == 2 && target_pos > single_quote_pos1 && target_pos < single_quote_pos2)
                        res = false;
                    else if (double_quote_cnt == 2 && target_pos > double_quote_pos1 && target_pos < double_quote_pos2)
                        res = false;
                    else
                        res = true;
                }
                else
                    res = true;

                return res;
            }
            else
                return res;
        }


        public static bool IsTypeCmd(string line)
        {
            bool res = false;


            string data_trim = GetTrimString(line);

            if (data_trim == "boolean")
                res = true;
            else if (data_trim == "byte")
                res = true;
            else if (data_trim == "char")
                res = true;
            else if (data_trim == "unsigned char")
                res = true;
            else if (data_trim == "int")
                res = true;
            else if (data_trim == "unsigned int")
                res = true;
            else if (data_trim == "word")
                res = true;
            else if (data_trim == "long")
                res = true;
            else if (data_trim == "unsigned long")
                res = true;
            else if (data_trim == "short")
                res = true;
            else if (data_trim == "float")
                res = true;
            else if (data_trim == "double")
                res = true;
            else if (data_trim == "string")
                res = true;
            else if (data_trim == "String")
                res = true;
            else if (data_trim == "array")
                res = true;
            else if (data_trim == "void")
                res = true;
            else if (data_trim == "LEDArray")
                res = true;
            else if (data_trim == "WS2801")
                res = true;
            else if (data_trim == "uint8_t")
                res = true;
            else if (data_trim == "uint32_t")
                res = true;

            return res;
        }

        public static string GetFirstVariable(string line)
        {
            string res = string.Empty;

            if (line.Contains("="))
            {
                string[] var_lines = line.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

                if (var_lines != null && var_lines.Length >= 2)
                {
                    string var_left = var_lines[0].Trim();
                    string var_right = var_lines[1].Trim();

                    string[] var_left_lines = var_left.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    if (var_left_lines != null && var_left_lines.Length == 1)
                    {
                        res = var_left;
                    }
                    else if (var_left_lines != null && var_left_lines.Length == 2)
                    {
                        if (IsTypeCmd(var_left_lines[0]))
                        {
                            res = var_left_lines[1];
                        }
                    }
                    else if (var_left_lines != null && var_left_lines.Length == 3)
                    {
                        //unsigned type
                        if (IsTypeCmd(var_left_lines[0] + " " + var_left_lines[1]))
                        {
                            res = var_left_lines[2];
                        }
                    }
                }
            }

            return GetTrimString(res);
        }



        public static string GetVariableTypeName(string line)
        {
            string res = string.Empty;

            if (string.IsNullOrEmpty(line))
                return res;

            int eq_ind = line.IndexOf('=');

            string line2 = line;

            if (eq_ind == 0)
                return res;
            else if (eq_ind > 0)
                line2 = line.Substring(0, eq_ind);

            string[] var_left_lines = line2.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            if (var_left_lines != null && var_left_lines.Length >= 2)
            {
                if (IsTypeCmd(var_left_lines[0]))
                {
                    res = var_left_lines[0];
                }
            }
            else if (var_left_lines != null && var_left_lines.Length >= 3)
            {
                //unsigned type
                if (IsTypeCmd(var_left_lines[0] + " " + var_left_lines[1]))
                {
                    res = var_left_lines[0] + " " + var_left_lines[1];
                }
            }

            return GetTrimString(res);
        }


        public static string GetVariableName(string line)
        {
            string res = string.Empty;

            if (string.IsNullOrEmpty(line))
                return res;

            int eq_ind = line.IndexOf('=');

            string line2 = line;

            if (eq_ind == 0)
                return res;
            else if (eq_ind > 0)
                line2 = line.Substring(0, eq_ind);


            string[] var_left_lines = line2.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            if (var_left_lines != null && var_left_lines.Length == 1)
            {
                res = var_left_lines[0];
            }
            else if (var_left_lines != null && var_left_lines.Length >= 2)
            {
                if (IsTypeCmd(var_left_lines[0]))
                {
                    res = var_left_lines[1];
                }
            }
            else if (var_left_lines != null && var_left_lines.Length >= 3)
            {
                //unsigned type
                if (IsTypeCmd(var_left_lines[0] + " " + var_left_lines[1]))
                {
                    res = var_left_lines[2];
                }
            }

            return GetTrimString(res);
        }

    }

}


