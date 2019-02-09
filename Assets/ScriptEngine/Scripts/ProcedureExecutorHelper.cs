using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using SPL.Common;
using eval=SPL.Evaluator;

public class ProcedureExecutorHelper {

	public delegate void LogInfoHandler(string log);
	public LogInfoHandler EvalLogInfoHandler = null;
	
	public Dictionary<object, object> _procedureTreeInstanceList = null;
	public Dictionary<object, object> _EventProcedureMappingList = null;
	
	public ArrayList _variableTypeNameList = null;
	public Dictionary<object, object> _GlobalVariables = null;
	public ArrayList _ProcedureList = new ArrayList();
	
	public Dictionary<object, object> _stringToIntMapping = new Dictionary<object, object>();
	
	
	public ProcedureExecutorHelper()
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


	public void Proc_CopyVariables(CommandTree parsedTree, CommandItem procItem, eval.Evaluator _evaluator, ArrayList tempLocalVariableList)
	{
		if (procItem.ProcedureRunningInfo != null)
		{
			procItem.ProcedureRunningInfo.ProcedureRunStart = true;
			
			procItem.ProcedureRunningInfo.RunningProcedureCount++;
		}
		
		
		if (procItem.ProcedureParamList.Count == procItem.ProcedureCallObjectList.Count)
		{
			for (int i = 0; i < procItem.ProcedureCallObjectList.Count; i++)
			{
				try
				{
					if (!procItem.LocalVariables.ContainsKey(procItem.ProcedureParamList[i]))
						procItem.LocalVariables.Add(procItem.ProcedureParamList[i], procItem.ProcedureCallObjectList[i]);
					
					if (!tempLocalVariableList.Contains(procItem.ProcedureParamList[i]))
						tempLocalVariableList.Add(procItem.ProcedureParamList[i]);
				}
				catch { }
			}
		}
		else
		{
			return;
		}
		
		
		foreach (string key in procItem.Params.Keys)
		{
			string paramData = procItem.Params[key].ToString();
			
			try
			{
				if (!procItem.LocalVariables.ContainsKey(key))
					procItem.LocalVariables.Add(key, paramData);
				
				if (!tempLocalVariableList.Contains(key))
					tempLocalVariableList.Add(key);
			}
			catch (Exception ex)
			{
				LogInfo("[ProcedureExecutor] " + ex.ToString());
			}
		}
	}


	public void Proc_ContineAndReturn(CommandTree parsedTree, CommandItem procItem, eval.Evaluator _evaluator, ArrayList tempLocalVariableList)
	{

		#region Break 와 Continue문 처리
		
		if (procItem.IsBreakContinueCalled.Count > 0)
		{
			if (((BreakContinueItem)procItem.IsBreakContinueCalled.Peek()).IsBreakCalled || ((BreakContinueItem)procItem.IsBreakContinueCalled.Peek()).IsContinueCalled)
			{
				if (procItem.Name != "maindefault")
				{
					foreach (string key in tempLocalVariableList)
					{
						try
						{
							procItem.LocalVariables.Remove(key);
						}
						catch (Exception ex)
						{
							LogInfo("[ProcedureExecutor] " + ex.ToString());
						}
					}
				}
				
				return;
			}
		}
		
		#endregion 
		
		
		
		#region Return문장의 처리
		
		if (procItem.ProcedureReturn != null)
		{
			if (procItem.ProcedureReturn.IsReturnCalled)
			{
				if (procItem.Name != "maindefault")
				{
					foreach (string key in tempLocalVariableList)
					{
						try
						{
							procItem.LocalVariables.Remove(key);
						}
						catch (Exception ex)
						{
							LogInfo("[ProcedureExecutor] " + ex.ToString());
						}
					}
				}
				
				if (procItem.Name == procItem.ProcedureReturn.ProcedureName)
				{
					if (procItem.FirstToken == "procedure")
					{
						if (procItem.ProcedureRunningInfo != null)
						{
							procItem.ProcedureRunningInfo.RunningProcedureCount--;
						}
					}
					
					procItem.ProcedureReturn.IsReturnCalled = false;
					
					return;
				}
				else
				{
					return;
				}
			}
		}
		
		#endregion 

	}


}
