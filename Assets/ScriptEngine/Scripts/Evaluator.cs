using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using SPL.Common;

namespace SPL.Evaluator
{
    //class EvalEngineParam
    //{
    //    public string _token = string.Empty;
    //    public int _tokType = 0;
    //    public string _exp = string.Empty;
    //    public int _expIdx = 0;
    //    public object _lastObject = null;
    //}

    //public class UtilInstance
    //{
    //    public UtilInstance()
    //    {
    //    }
    //}

    //public class MathInstance
    //{
    //    public MathInstance()
    //    {
    //    }
    //}

    //public class ConvertInstance
    //{
    //    public ConvertInstance()
    //    {
    //    }
    //}

    //public class RandomInstance
    //{
    //    public RandomInstance()
    //    {
    //    }
    //}

    //public class ScreenInstance
    //{
    //    public ScreenInstance()
    //    {
    //    }
    //}


    public class Evaluator : MonoBehaviour
    {
        #region const

        const int NONE = 0;
        const int DELIMITER = 1;
        const int VARIABLE = 2;
        const int NUMBER = 3;
        const int STRING = 4;
        const int CHARACTER = 5;
        const int METHOD = 6;
        const int SYNTAX = 0;
        const int UNBALPARENS = 1;
        const int NOEXP = 2;
        const int DIVBYZERO = 3;
        const string EOE = "\0";

        #endregion const 


        public delegate void SocketProcedureHandler(string targetName, byte[] data);
        public SocketProcedureHandler EvalSocketProcedureHandler = null;

        public delegate object CallProcedureHandler(string procedureName, string paramStr, Dictionary<object, object> localVariables);
        public CallProcedureHandler EvalCallProcedureHandler = null;

        public delegate void LogInfoHandler(string log);
        public LogInfoHandler EvalLogInfoHandler = null;

        public delegate object UnityRequestHandler(GameObject target_object, string cmdName, string paramStr);
        public UnityRequestHandler EvalUnityRequestHandler = null;


        Dictionary<object, object> _CommonVariables = new Dictionary<object, object>();
        Dictionary<object, object> _GlobalVariables = new Dictionary<object, object>();
        Dictionary<object, object> _LocalVariables = new Dictionary<object, object>();

        ArrayList _ProcedureList = new ArrayList();

        GameObject _MainCameraContainerInstance = null;
        MainCamControl _MainCamControlInstance = null;

        public Evaluator()
        {
            _CommonVariables.Add("null", null);
            _CommonVariables.Add("true", true);
            _CommonVariables.Add("false", false);
            _CommonVariables.Add("True", true);
            _CommonVariables.Add("False", false);
            _CommonVariables.Add("Util", new UtilInstance());
            _CommonVariables.Add("Math", new MathInstance());
        }

        public Dictionary<object, object> GlobalVariables
        {
            get { return _GlobalVariables; }
            set { _GlobalVariables = value; }
        }

        public Dictionary<object, object> LocalVariables
        {
            get { return _LocalVariables; }
            set { _LocalVariables = value; }
        }

        public ArrayList ProcedureList
        {
            get { return _ProcedureList; }
            set { _ProcedureList = value; }
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

        public IEnumerator Evaluate(string line)
        {
            object res = null;

            EvalEngineParam evalParam = new EvalEngineParam();

            evalParam._exp = line;
            evalParam._expIdx = 0;

            GetToken(evalParam);

            if (evalParam._token == EOE)
            {
                //return null;
                gv.GlobalVariables.res = null;
                yield break;
            }

            yield return StartCoroutine(EvalLogicalOp(evalParam));
            res = gv.GlobalVariables.res;

        }

        public object Evaluate_Origin(string line)
        {
            object res = null;

            try
            {
                EvalEngineParam evalParam = new EvalEngineParam();

                evalParam._exp = line;
                evalParam._expIdx = 0;

                GetToken(evalParam);

                if (evalParam._token == EOE)
                    return null;

                res = EvalLogicalOp_Origin(evalParam);
            }
            catch (Exception ex)
            {
                LogInfo("[Evaluate] " + ex.ToString());
            }

            return res;
        }

        private IEnumerator EvalLogicalOp(EvalEngineParam evalParam)
        {
            object res = null;
            object l_temp = null;
            object r_temp = null;


            yield return StartCoroutine(EvalCompare(evalParam));
            res = gv.GlobalVariables.res;

            if (evalParam._token == EOE)
            {
                //return res;
                gv.GlobalVariables.res = res;
                yield break;
            }


            string op = evalParam._token;

            if (op == "&&" || op == "||")
            {
                l_temp = res;

                GetToken(evalParam);

                yield return StartCoroutine(EvalLogicalOp(evalParam));
                r_temp = gv.GlobalVariables.res;

                if (l_temp == null || r_temp == null)
                {
                    LogInfo("[EvalSign] Can't apply Logical operation for the null object");
                    gv.GlobalVariables.res = null;
                    yield break;
                }

                switch (op)
                {
                    case "&&":

                        if (IsBool(l_temp) && IsBool(r_temp))
                        {
                            res = Util.ToBool(l_temp) && Util.ToBool(r_temp);
                        }
                        else
                            LogInfo("[EvalLogicalOp] " + "Not executable -> " + l_temp.ToString() + " && " + r_temp.ToString());

                        break;

                    case "||":

                        if (IsBool(l_temp) && IsBool(r_temp))
                        {
                            res = Util.ToBool(l_temp) || Util.ToBool(r_temp);
                        }
                        else
                            LogInfo("[EvalLogicalOp] " + "Not executable -> " + l_temp.ToString() + " || " + r_temp.ToString());

                        break;
                }
            }
            gv.GlobalVariables.res = res;
        }

        private object EvalLogicalOp_Origin(EvalEngineParam evalParam)
        {
            object res = null;
            object l_temp = null;
            object r_temp = null;

            try
            {
                res = EvalCompare_Origin(evalParam);

                if (evalParam._token == EOE)
                    return res;

                string op = evalParam._token;

                if (op == "&&" || op == "||")
                {
                    l_temp = res;

                    GetToken(evalParam);

                    r_temp = EvalLogicalOp_Origin(evalParam);

                    if (l_temp == null || r_temp == null)
                    {
                        LogInfo("[EvalSign] Can't apply Logical operation for the null object");
                        return null;
                    }

                    switch (op)
                    {
                        case "&&":

                            if (IsBool(l_temp) && IsBool(r_temp))
                            {
                                res = Util.ToBool(l_temp) && Util.ToBool(r_temp);
                            }
                            else
                                LogInfo("[EvalLogicalOp] " + "Not executable -> " + l_temp.ToString() + " && " + r_temp.ToString());

                            break;

                        case "||":

                            if (IsBool(l_temp) && IsBool(r_temp))
                            {
                                res = Util.ToBool(l_temp) || Util.ToBool(r_temp);
                            }
                            else
                                LogInfo("[EvalLogicalOp] " + "Not executable -> " + l_temp.ToString() + " || " + r_temp.ToString());

                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogInfo("[EvalLogicalOp] " + ex.ToString());
            }

            return res;
        }

        private IEnumerator EvalCompare(EvalEngineParam evalParam)
        {
            object res = null;
            object l_temp = null;
            object r_temp = null;


            //res = EvalBitOp(evalParam);
            yield return StartCoroutine(EvalBitOp(evalParam));
            res = gv.GlobalVariables.res;

            if (evalParam._token == EOE)
            {
                gv.GlobalVariables.res = res;
                yield break;
                //return res;
            }


            string op = evalParam._token;

            if ((op == "<") || (op == "<=") || (op == ">") || (op == ">=") ||
                (op == "==") || (op == "!="))
            {
                l_temp = res;

                GetToken(evalParam);

                yield return StartCoroutine(EvalCompare(evalParam));
                r_temp = gv.GlobalVariables.res;


                if (l_temp == null || r_temp == null)
                {
                    LogInfo("[EvalSign] Can't apply Compare operation for the null object");
                    gv.GlobalVariables.res = null;
                    yield break;
                }

                switch (op)
                {
                    case "<":

                        if (IsNumeric(l_temp) && IsNumeric(r_temp))
                        {
                            if (Util.ToDouble(l_temp) < Util.ToDouble(r_temp))
                                res = true;
                            else
                                res = false;
                        }
                        else
                            LogInfo("[EvalCompare] " + "Not executable -> " + l_temp.ToString() + " < " + r_temp.ToString());

                        break;

                    case "<=":

                        if (IsNumeric(l_temp) && IsNumeric(r_temp))
                        {
                            if (Util.ToDouble(l_temp) <= Util.ToDouble(r_temp))
                                res = true;
                            else
                                res = false;
                        }
                        else
                            LogInfo("[EvalCompare] " + "Not executable -> " + l_temp.ToString() + " <= " + r_temp.ToString());

                        break;

                    case ">":

                        if (IsNumeric(l_temp) && IsNumeric(r_temp))
                        {
                            if (Util.ToDouble(l_temp) > Util.ToDouble(r_temp))
                                res = true;
                            else
                                res = false;
                        }
                        else
                            LogInfo("[EvalCompare] " + "Not executable -> " + l_temp.ToString() + " > " + r_temp.ToString());

                        break;

                    case ">=":

                        if (IsNumeric(l_temp) && IsNumeric(r_temp))
                        {
                            if (Util.ToDouble(l_temp) >= Util.ToDouble(r_temp))
                                res = true;
                            else
                                res = false;
                        }
                        else
                            LogInfo("[EvalCompare] " + "Not executable -> " + l_temp.ToString() + " >= " + r_temp.ToString());

                        break;

                    case "==":

                        if (l_temp.ToString() == r_temp.ToString())
                            res = true;
                        else
                            res = false;
                        break;

                    case "!=":

                        if (l_temp.ToString() != r_temp.ToString())
                            res = true;
                        else
                            res = false;

                        break;
                }
            }

            gv.GlobalVariables.res = res;
            // return res;
        }

        private object EvalCompare_Origin(EvalEngineParam evalParam)
        {
            object res = null;
            object l_temp = null;
            object r_temp = null;

            try
            {

                res = EvalBitOp_Origin(evalParam);

                if (evalParam._token == EOE)
                    return res;

                string op = evalParam._token;

                if ((op == "<") || (op == "<=") || (op == ">") || (op == ">=") ||
                    (op == "==") || (op == "!="))
                {
                    l_temp = res;

                    GetToken(evalParam);

                    r_temp = EvalCompare_Origin(evalParam);


                    if (l_temp == null || r_temp == null)
                    {
                        LogInfo("[EvalSign] Can't apply Compare operation for the null object");
                        return null;
                    }

                    switch (op)
                    {
                        case "<":

                            if (IsNumeric(l_temp) && IsNumeric(r_temp))
                            {
                                if (Util.ToDouble(l_temp) < Util.ToDouble(r_temp))
                                    res = true;
                                else
                                    res = false;
                            }
                            else
                                LogInfo("[EvalCompare] " + "Not executable -> " + l_temp.ToString() + " < " + r_temp.ToString());

                            break;

                        case "<=":

                            if (IsNumeric(l_temp) && IsNumeric(r_temp))
                            {
                                if (Util.ToDouble(l_temp) <= Util.ToDouble(r_temp))
                                    res = true;
                                else
                                    res = false;
                            }
                            else
                                LogInfo("[EvalCompare] " + "Not executable -> " + l_temp.ToString() + " <= " + r_temp.ToString());

                            break;

                        case ">":

                            if (IsNumeric(l_temp) && IsNumeric(r_temp))
                            {
                                if (Util.ToDouble(l_temp) > Util.ToDouble(r_temp))
                                    res = true;
                                else
                                    res = false;
                            }
                            else
                                LogInfo("[EvalCompare] " + "Not executable -> " + l_temp.ToString() + " > " + r_temp.ToString());

                            break;

                        case ">=":

                            if (IsNumeric(l_temp) && IsNumeric(r_temp))
                            {
                                if (Util.ToDouble(l_temp) >= Util.ToDouble(r_temp))
                                    res = true;
                                else
                                    res = false;
                            }
                            else
                                LogInfo("[EvalCompare] " + "Not executable -> " + l_temp.ToString() + " >= " + r_temp.ToString());

                            break;

                        case "==":

                            if (l_temp.ToString() == r_temp.ToString())
                                res = true;
                            else
                                res = false;
                            break;

                        case "!=":

                            if (l_temp.ToString() != r_temp.ToString())
                                res = true;
                            else
                                res = false;

                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogInfo("[EvalCompare] " + ex.ToString());
            }

            return res;
        }

        private IEnumerator EvalBitOp(EvalEngineParam evalParam)
        {
            object res = null;
            object pRes = null;


            yield return StartCoroutine(EvalShiftOp(evalParam));
            res = gv.GlobalVariables.res;

            //res = EvalShiftOp(evalParam);

            if (evalParam._token == EOE)
            {
                gv.GlobalVariables.res = res;
                yield break;
                //return res;
            }

            string op = evalParam._token;

            while (op == "&" || op == "|" || op == "^")
            {
                GetToken(evalParam);

                yield return StartCoroutine(EvalShiftOp(evalParam));
                pRes = gv.GlobalVariables.res;
                //pRes = EvalShiftOp(evalParam);

                if (res == null || pRes == null)
                {

                    LogInfo("[EvalSign] Can't apply Bit operation for the null object");
                    //return null;
                    gv.GlobalVariables.res = null;
                    yield break;
                }

                switch (op)
                {
                    case "&":

                        if (IsNumeric(res) && IsNumeric(pRes))
                        {
                            res = Util.ToInt(res) & Util.ToInt(pRes);
                        }
                        else
                            LogInfo("[EvalBitOp] " + "Not executable -> " + res.ToString() + " & " + pRes.ToString());

                        break;

                    case "|":

                        if (IsNumeric(res) && IsNumeric(pRes))
                        {
                            res = Util.ToInt(res) | Util.ToInt(pRes);
                        }
                        else
                            LogInfo("[EvalBitOp] " + "Not executable -> " + res.ToString() + " | " + pRes.ToString());

                        break;

                    case "^":

                        if (IsNumeric(res) && IsNumeric(pRes))
                        {
                            res = Util.ToInt(res) ^ Util.ToInt(pRes);
                        }
                        else
                            LogInfo("[EvalBitOp] " + "Not executable -> " + res.ToString() + " ^ " + pRes.ToString());

                        break;
                }

                if (evalParam._token.Length > 0)
                    op = evalParam._token;
                else
                    break;
            }

            gv.GlobalVariables.res = res;
            //return res;
        }

        private object EvalBitOp_Origin(EvalEngineParam evalParam)
        {
            object res = null;
            object pRes = null;

            try
            {

                res = EvalShiftOp_Origin(evalParam);

                if (evalParam._token == EOE)
                    return res;

                string op = evalParam._token;

                while (op == "&" || op == "|" || op == "^")
                {
                    GetToken(evalParam);

                    pRes = EvalShiftOp_Origin(evalParam);

                    if (res == null || pRes == null)
                    {
                        LogInfo("[EvalSign] Can't apply Bit operation for the null object");
                        return null;
                    }

                    switch (op)
                    {
                        case "&":

                            if (IsNumeric(res) && IsNumeric(pRes))
                            {
                                res = Util.ToInt(res) & Util.ToInt(pRes);
                            }
                            else
                                LogInfo("[EvalBitOp] " + "Not executable -> " + res.ToString() + " & " + pRes.ToString());

                            break;

                        case "|":

                            if (IsNumeric(res) && IsNumeric(pRes))
                            {
                                res = Util.ToInt(res) | Util.ToInt(pRes);
                            }
                            else
                                LogInfo("[EvalBitOp] " + "Not executable -> " + res.ToString() + " | " + pRes.ToString());

                            break;

                        case "^":

                            if (IsNumeric(res) && IsNumeric(pRes))
                            {
                                res = Util.ToInt(res) ^ Util.ToInt(pRes);
                            }
                            else
                                LogInfo("[EvalBitOp] " + "Not executable -> " + res.ToString() + " ^ " + pRes.ToString());

                            break;
                    }

                    if (evalParam._token.Length > 0)
                        op = evalParam._token;
                    else
                        break;
                }
            }
            catch (Exception ex)
            {
                LogInfo("[EvalBitOp] " + ex.ToString());
            }

            return res;
        }

        private IEnumerator EvalShiftOp(EvalEngineParam evalParam)
        {
            object res = null;
            object pRes = null;

            yield return StartCoroutine(EvalAddOrSub(evalParam));
            res = gv.GlobalVariables.res;
            //res = EvalAddOrSub(evalParam);

            if (evalParam._token == EOE)
            {
                res = gv.GlobalVariables.res;
                yield break;
                //return res;
            }

            string op = evalParam._token;

            while (op == "<<" || op == ">>")
            {
                GetToken(evalParam);

                yield return StartCoroutine(EvalAddOrSub(evalParam));
                pRes = gv.GlobalVariables.res;
                //pRes = EvalAddOrSub(evalParam);

                if (res == null || pRes == null)
                {
                    LogInfo("[EvalSign] Can't apply Shift operation for the null object");
                    gv.GlobalVariables.res = null;
                    yield break;
                    //return null;
                }

                switch (op)
                {
                    case "<<":

                        if (IsNumeric(res) && IsNumeric(pRes))
                        {
                            res = Util.ToInt(res) << Util.ToInt(pRes);
                        }
                        else
                            LogInfo("[EvalShiftOp] " + "Not executable -> " + res.ToString() + " << " + pRes.ToString());

                        break;

                    case ">>":

                        if (IsNumeric(res) && IsNumeric(pRes))
                        {
                            res = Util.ToInt(res) >> Util.ToInt(pRes);
                        }
                        else
                            LogInfo("[EvalShiftOp] " + "Not executable -> " + res.ToString() + " >> " + pRes.ToString());

                        break;
                }

                if (evalParam._token.Length > 0)
                    op = evalParam._token;
                else
                    break;
            }

            //return res;
            gv.GlobalVariables.res = res;
        }

        private object EvalShiftOp_Origin(EvalEngineParam evalParam)
        {
            object res = null;
            object pRes = null;

            try
            {

                res = EvalAddOrSub_Origin(evalParam);

                if (evalParam._token == EOE)
                    return res;

                string op = evalParam._token;

                while (op == "<<" || op == ">>")
                {
                    GetToken(evalParam);

                    pRes = EvalAddOrSub_Origin(evalParam);

                    if (res == null || pRes == null)
                    {
                        LogInfo("[EvalSign] Can't apply Shift operation for the null object");
                        return null;
                    }

                    switch (op)
                    {
                        case "<<":

                            if (IsNumeric(res) && IsNumeric(pRes))
                            {
                                res = Util.ToInt(res) << Util.ToInt(pRes);
                            }
                            else
                                LogInfo("[EvalShiftOp] " + "Not executable -> " + res.ToString() + " << " + pRes.ToString());

                            break;

                        case ">>":

                            if (IsNumeric(res) && IsNumeric(pRes))
                            {
                                res = Util.ToInt(res) >> Util.ToInt(pRes);
                            }
                            else
                                LogInfo("[EvalShiftOp] " + "Not executable -> " + res.ToString() + " >> " + pRes.ToString());

                            break;
                    }

                    if (evalParam._token.Length > 0)
                        op = evalParam._token;
                    else
                        break;
                }
            }
            catch (Exception ex)
            {
                LogInfo("[EvalShiftOp] " + ex.ToString());
            }

            return res;
        }

        private IEnumerator EvalAddOrSub(EvalEngineParam evalParam)
        {
            object res = null;
            object pRes = null;

            yield return StartCoroutine(EvalMulOrDiv(evalParam));
            res = gv.GlobalVariables.res;
            //res = EvalMulOrDiv(evalParam);

            if (evalParam._token == EOE)
            {
                gv.GlobalVariables.res = res;
                yield break;
                //return res;
            }


            string op = evalParam._token;

            while (op == "+" || op == "-")
            {
                GetToken(evalParam);

                yield return StartCoroutine(EvalMulOrDiv(evalParam));
                pRes = gv.GlobalVariables.res;
                //pRes = ;

                if (res == null || pRes == null)
                {
                    LogInfo("[EvalSign] Can't apply AddOrSub operation for the null object");
                    gv.GlobalVariables.res = null;
                    yield break;
                    //return null;
                }

                switch (op)
                {
                    case "-":

                        if (IsNumeric(res) && IsNumeric(pRes))
                        {
                            res = Util.ToDouble(res) - Util.ToDouble(pRes);
                        }
                        else
                            LogInfo("[EvalAddOrSub] " + "Not executable -> " + res.ToString() + " - " + pRes.ToString());

                        break;

                    case "+":

                        if (IsNumeric(res) && IsNumeric(pRes))
                        {
                            res = Util.ToDouble(res) + Util.ToDouble(pRes);
                        }
                        else
                        {
                            res = res.ToString() + pRes.ToString();
                        }

                        break;
                }

                if (evalParam._token.Length > 0)
                    op = evalParam._token;
                else
                    break;
            }
            gv.GlobalVariables.res = res;
            //return res;
        }

        private object EvalAddOrSub_Origin(EvalEngineParam evalParam)
        {
            object res = null;
            object pRes = null;

            try
            {

                res = EvalMulOrDiv_Origin(evalParam);

                if (evalParam._token == EOE)
                    return res;

                string op = evalParam._token;

                while (op == "+" || op == "-")
                {
                    GetToken(evalParam);

                    pRes = EvalMulOrDiv_Origin(evalParam);

                    if (res == null || pRes == null)
                    {
                        LogInfo("[EvalSign] Can't apply AddOrSub operation for the null object");
                        return null;
                    }

                    switch (op)
                    {
                        case "-":

                            if (IsNumeric(res) && IsNumeric(pRes))
                            {
                                res = Util.ToDouble(res) - Util.ToDouble(pRes);
                            }
                            else
                                LogInfo("[EvalAddOrSub] " + "Not executable -> " + res.ToString() + " - " + pRes.ToString());

                            break;

                        case "+":

                            if (IsNumeric(res) && IsNumeric(pRes))
                            {
                                res = Util.ToDouble(res) + Util.ToDouble(pRes);
                            }
                            else
                            {
                                res = res.ToString() + pRes.ToString();
                            }

                            break;
                    }

                    if (evalParam._token.Length > 0)
                        op = evalParam._token;
                    else
                        break;
                }
            }
            catch (Exception ex)
            {
                LogInfo("[EvalAddOrSub] " + ex.ToString());
            }

            return res;
        }

        private IEnumerator EvalMulOrDiv(EvalEngineParam evalParam)
        {
            object res = null;
            object pRes = null;


            //res = EvalSign(evalParam);
            yield return StartCoroutine(EvalSign(evalParam));
            res = gv.GlobalVariables.res;

            if (evalParam._token == EOE)
            {
                gv.GlobalVariables.res = res;
                yield break;
                //return res;
            }


            string op = evalParam._token;

            while (op == "*" || op == "/" || op == "%")
            {
                GetToken(evalParam);

                yield return StartCoroutine(EvalSign(evalParam));
                pRes = gv.GlobalVariables.res;
                //pRes = EvalSign(evalParam);

                if (res == null || pRes == null)
                {
                    LogInfo("[EvalSign #1] Can't apply MulOrDiv operation for the null object");
                    gv.GlobalVariables.res = null;
                    yield break;
                    //return null;
                }


                switch (op)
                {
                    case "*":

                        if (IsNumeric(res) && IsNumeric(pRes))
                        {
                            res = Util.ToDouble(res) * Util.ToDouble(pRes);
                        }
                        else
                            LogInfo("[EvalMulOrDiv] " + "Not executable -> " + res.ToString() + " * " + pRes.ToString());

                        break;

                    case "/":

                        if (IsNumeric(res) && IsNumeric(pRes))
                        {
                            if (Util.ToDouble(pRes) == 0.0)
                                LogInfo("[EvalMulOrDiv] " + "DIV BY ZERO" + " / " + res.ToString() + ", " + pRes.ToString());
                            else
                                res = Util.ToDouble(res) / Util.ToDouble(pRes);
                        }
                        else
                            LogInfo("[EvalMulOrDiv] " + "Not executable -> " + res.ToString() + " / " + pRes.ToString());

                        break;

                    case "%":

                        if (IsNumeric(res) && IsNumeric(pRes))
                        {
                            if (Util.ToDouble(pRes) == 0.0)
                                LogInfo("[EvalMulOrDiv] " + "DIV BY ZERO" + " / " + res.ToString() + ", " + pRes.ToString());
                            else
                                res = Util.ToDouble(res) % Util.ToDouble(pRes);
                        }
                        else
                            LogInfo("[EvalMulOrDiv] " + "Not executable -> " + res.ToString() + " % " + pRes.ToString());

                        break;
                }

                if (evalParam._token.Length > 0)
                    op = evalParam._token;
                else
                    break;
            }
            //return res;
            gv.GlobalVariables.res = res;
        }

        private object EvalMulOrDiv_Origin(EvalEngineParam evalParam)
        {
            object res = null;
            object pRes = null;

            try
            {
                res = EvalSign_Origin(evalParam);

                if (evalParam._token == EOE)
                    return res;

                string op = evalParam._token;

                while (op == "*" || op == "/" || op == "%")
                {
                    GetToken(evalParam);

                    pRes = EvalSign_Origin(evalParam);

                    if (res == null || pRes == null)
                    {
                        LogInfo("[EvalSign #1] Can't apply MulOrDiv operation for the null object");
                        return null;
                    }


                    switch (op)
                    {
                        case "*":

                            if (IsNumeric(res) && IsNumeric(pRes))
                            {
                                res = Util.ToDouble(res) * Util.ToDouble(pRes);
                            }
                            else
                                LogInfo("[EvalMulOrDiv] " + "Not executable -> " + res.ToString() + " * " + pRes.ToString());

                            break;

                        case "/":

                            if (IsNumeric(res) && IsNumeric(pRes))
                            {
                                if (Util.ToDouble(pRes) == 0.0)
                                    LogInfo("[EvalMulOrDiv] " + "DIV BY ZERO" + " / " + res.ToString() + ", " + pRes.ToString());
                                else
                                    res = Util.ToDouble(res) / Util.ToDouble(pRes);
                            }
                            else
                                LogInfo("[EvalMulOrDiv] " + "Not executable -> " + res.ToString() + " / " + pRes.ToString());

                            break;

                        case "%":

                            if (IsNumeric(res) && IsNumeric(pRes))
                            {
                                if (Util.ToDouble(pRes) == 0.0)
                                    LogInfo("[EvalMulOrDiv] " + "DIV BY ZERO" + " / " + res.ToString() + ", " + pRes.ToString());
                                else
                                    res = Util.ToDouble(res) % Util.ToDouble(pRes);
                            }
                            else
                                LogInfo("[EvalMulOrDiv] " + "Not executable -> " + res.ToString() + " % " + pRes.ToString());

                            break;
                    }

                    if (evalParam._token.Length > 0)
                        op = evalParam._token;
                    else
                        break;
                }
            }
            catch (Exception ex)
            {
                LogInfo("[EvalMulOrDiv] " + ex.ToString());
            }

            return res;
        }

        private IEnumerator EvalSign(EvalEngineParam evalParam)
        {
            object res = null;
            string op = string.Empty;



            if (evalParam._tokType == DELIMITER && (evalParam._token == "+" || evalParam._token == "-" || evalParam._token == "!" || evalParam._token == "~"))
            {
                op = evalParam._token;
                GetToken(evalParam);
            }

            yield return StartCoroutine(EvalParen(evalParam));
            res = gv.GlobalVariables.res;
            //res = EvalParen(evalParam);                

            if (op == "-")
            {
                if (res == null)
                {
                    LogInfo("[EvalSign] Can't apply - operation for the null object");
                    gv.GlobalVariables.res = null;
                    yield break;
                    //return null;
                }

                if (IsNumeric(res))
                {
                    res = -1 * Util.ToDouble(res);
                }
            }
            else if (op == "!")
            {
                if (res == null)
                {
                    LogInfo("[EvalSign] Can't apply ! operation for the null object");
                    gv.GlobalVariables.res = null;
                    yield break;
                    //return null;
                }

                if (IsBool(res))
                {
                    res = !Util.ToBool(res);
                }
            }
            else if (op == "~")
            {
                if (res == null)
                {
                    LogInfo("[EvalSign] Can't apply ~ operation for the null object");
                    gv.GlobalVariables.res = null;
                    yield break;
                    //return null;
                }

                if (IsNumeric(res))
                {
                    res = ~Util.ToInt(res);
                }
            }


            //return res;
            gv.GlobalVariables.res = res;
        }

        private object EvalSign_Origin(EvalEngineParam evalParam)
        {
            object res = null;
            string op = string.Empty;

            try
            {

                if (evalParam._tokType == DELIMITER && (evalParam._token == "+" || evalParam._token == "-" || evalParam._token == "!" || evalParam._token == "~"))
                {
                    op = evalParam._token;
                    GetToken(evalParam);
                }

                res = EvalParen_Origin(evalParam);

                if (op == "-")
                {
                    if (res == null)
                    {
                        LogInfo("[EvalSign] Can't apply - operation for the null object");
                        return null;
                    }

                    if (IsNumeric(res))
                    {
                        res = -1 * Util.ToDouble(res);
                    }
                }
                else if (op == "!")
                {
                    if (res == null)
                    {
                        LogInfo("[EvalSign] Can't apply ! operation for the null object");
                        return null;
                    }

                    if (IsBool(res))
                    {
                        res = !Util.ToBool(res);
                    }
                }
                else if (op == "~")
                {
                    if (res == null)
                    {
                        LogInfo("[EvalSign] Can't apply ~ operation for the null object");
                        return null;
                    }

                    if (IsNumeric(res))
                    {
                        res = ~Util.ToInt(res);
                    }
                }
            }
            catch (Exception ex)
            {
                LogInfo("[EvalSign] " + ex.ToString());
            }

            return res;
        }

        private IEnumerator EvalParen(EvalEngineParam evalParam)
        {
            //LogInfo("[EvalParen] " + evalParam._token);

            object res = null;

            //마지막
            if (evalParam._token == "(")
            {
                GetToken(evalParam);

                yield return StartCoroutine(EvalLogicalOp(evalParam));
                res = gv.GlobalVariables.res;
                //res = EvalLogicalOp(evalParam);

                if (evalParam._token != ")")
                {
                    LogInfo("[EvalParen] " + "Need closing paren");
                }

                GetToken(evalParam);
            }
            else
            {
                yield return StartCoroutine(TokenToInstance(evalParam));
                res = gv.GlobalVariables.res;
                //res = TokenToInstance(evalParam);
            }

            gv.GlobalVariables.res = res;
            //return res;
        }

        private object EvalParen_Origin(EvalEngineParam evalParam)
        {
            //LogInfo("[EvalParen] " + evalParam._token);

            object res = null;

            try
            {
                if (evalParam._token == "(")
                {
                    GetToken(evalParam);

                    res = EvalLogicalOp_Origin(evalParam);

                    if (evalParam._token != ")")
                    {
                        LogInfo("[EvalParen] " + "Need closing paren");
                    }

                    GetToken(evalParam);
                }
                else
                    res = TokenToInstance(evalParam);
            }
            catch (Exception ex)
            {
                LogInfo("[EvalParen] " + ex.ToString());
            }

            return res;
        }

        #region TokenToInstance()

        private IEnumerator TokenToInstance(EvalEngineParam evalParam)
        {
            object res = null;


            switch (evalParam._tokType)
            {
                case NUMBER:
                    try
                    {
                        if (evalParam._token.IndexOf("0x") == 0)
                        {
                            //hex
                            if (evalParam._token.Length == 4)
                                res = Util.HexToByte(evalParam._token);
                            else if (evalParam._token.Length == 6)
                                res = Util.HexToShort(evalParam._token);
                            else if (evalParam._token.Length == 10)
                                res = Util.HexToInt(evalParam._token);
                            else
                                LogInfo("[TokenToInstance NUMBER #1] " + "Can't conver from hex to number -> " + evalParam._token);
                        }
                        else
                        {
                            double val = double.Parse(evalParam._token);

                            res = val;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogInfo("[TokenToInstance NUMBER #2] " + ex.ToString() + " => " + evalParam._token);
                    }

                    GetToken(evalParam);

                    break;

                case STRING:
                    try
                    {
                        res = evalParam._token.ToString();
                    }
                    catch (Exception ex)
                    {
                        LogInfo("[TokenToInstance STRING #1] " + ex.ToString());
                    }

                    GetToken(evalParam);

                    break;

                case CHARACTER:
                    try
                    {
                        if (evalParam._token.Length == 1)
                        {
                            res = (char)evalParam._token[0];
                        }
                        else
                            res = evalParam._token.ToString();
                    }
                    catch (Exception ex)
                    {
                        LogInfo("[TokenToInstance CHAR #1] " + ex.ToString());
                    }

                    GetToken(evalParam);

                    break;

                case VARIABLE:

                    if (evalParam._token.IndexOf(".") >= 0)
                    {
                        if (evalParam._token.IndexOf("(") >= 0 && evalParam._token.IndexOf(")") >= 0)
                        {
                            //method
                            int dotPos = evalParam._token.IndexOf(".");

                            string objectName = evalParam._token.Substring(0, dotPos);
                            string methodName = evalParam._token.Substring(dotPos);

                            int parenStartPos = methodName.IndexOf("(");

                            string arguments = methodName.Substring(parenStartPos);

                            if (arguments[0] == '(')
                                arguments = arguments.Substring(1);

                            if (arguments[arguments.Length - 1] == ')')
                                arguments = arguments.Substring(0, arguments.Length - 1);

                            methodName = methodName.Substring(0, parenStartPos);

                            yield return StartCoroutine(ExecuteMethod(objectName, methodName, arguments));
                            res = gv.GlobalVariables.res;
                           //res = ExecuteMethod(objectName, methodName, arguments);
                        }
                        else if (evalParam._token.IndexOf("(") < 0 && evalParam._token.IndexOf(")") < 0)
                        {
                            //property
                            int dotPos = evalParam._token.IndexOf(".");
                            string objectName = evalParam._token.Substring(0, dotPos);
                            string properyName = evalParam._token.Substring(dotPos);

                            res = GetProperty(objectName, properyName);
                        }
                        else
                            LogInfo("[TokenToInstance VAR #1] " + "Can't find variable " + evalParam._token);
                    }
                    else if (evalParam._token.IndexOf("(") >= 0 && evalParam._token.IndexOf(")") >= 0)
                    {
                        //###################################################################
                        //2015.12.27
                        //내부 함수는 이곳에서 정의해 추가해 준다.
                        //###################################################################

                        //2017.11.20 Arduino 함수 추가

                        if (evalParam._token.StartsWith("map("))
                        {
                            yield return StartCoroutine(CallInternalMethod(evalParam._token, "map"));
                            res = gv.GlobalVariables.res;
                            //res = CallInternalMethod(evalParam._token, "map");
                        }
                        else if (evalParam._token.StartsWith("digitalRead") || evalParam._token.StartsWith("DigitalRead"))
                        {
                            //2017.11.20 Arduino 함수 추가
                            yield return StartCoroutine(CallInternalMethod(evalParam._token, "digitalread"));
                            res = gv.GlobalVariables.res;
                            //res = CallInternalMethod(evalParam._token, "digitalread");
                        }
                        else if (evalParam._token.StartsWith("analogRead") || evalParam._token.StartsWith("AnalogRead"))
                        {
                            //2017.11.20 Arduino 함수 추가
                            yield return StartCoroutine(CallInternalMethod(evalParam._token, "analogread"));
                            res = gv.GlobalVariables.res;
                            //res = CallInternalMethod(evalParam._token, "analogread");
                        }
                        else if (evalParam._token.StartsWith("dist("))
                        {
                            yield return StartCoroutine(CallInternalMethod(evalParam._token, "analogread"));
                            res = gv.GlobalVariables.res;
                            //res = CallInternalMethod(evalParam._token, "dist");
                        }
                        else if (evalParam._token.StartsWith("millis("))
                        {
                            yield return StartCoroutine(CallInternalMethod(evalParam._token, "millis"));
                            res = gv.GlobalVariables.res;
                            //res = CallInternalMethod(evalParam._token, "millis");
                        }
                        else if (evalParam._token.StartsWith("random("))
                        {
                            yield return StartCoroutine(CallInternalMethod(evalParam._token, "random"));
                            res = gv.GlobalVariables.res;
                            //res = CallInternalMethod(evalParam._token, "random");
                        }
                        else if (evalParam._token.StartsWith("loadImage(") || evalParam._token.StartsWith("LoadImage(") || evalParam._token.StartsWith("loadimage("))
                        {
                            yield return StartCoroutine(CallInternalMethod(evalParam._token, "loadimage"));
                            res = gv.GlobalVariables.res;
                            //res = CallInternalMethod(evalParam._token, "loadimage");
                        }
                        else if (evalParam._token.StartsWith("SerialOpen(") || evalParam._token.StartsWith("serialopen("))
                        {
                            yield return StartCoroutine(CallInternalMethod(evalParam._token, "serialopen"));
                            res = gv.GlobalVariables.res;
                            //res = CallInternalMethod(evalParam._token, "serialopen");
                        }
                        else if (evalParam._token.StartsWith("SerialList(") || evalParam._token.StartsWith("seriallist("))
                        {
                            yield return StartCoroutine(CallInternalMethod(evalParam._token, "seriallist"));
                            res = gv.GlobalVariables.res;
                            //res = CallInternalMethod(evalParam._token, "seriallist");
                        }
                        else if (evalParam._token.StartsWith("SerialClose(") || evalParam._token.StartsWith("serialclose("))
                        {
                            yield return StartCoroutine(CallInternalMethod(evalParam._token, "serialclose"));
                            res = gv.GlobalVariables.res;
                            //res = CallInternalMethod(evalParam._token, "serialclose");
                        }
                        else if (evalParam._token.StartsWith("SerialReadString(") || evalParam._token.StartsWith("serialreadstring("))
                        {
                            yield return StartCoroutine(CallInternalMethod(evalParam._token, "serialreadstring"));
                            res = gv.GlobalVariables.res;
                            //res = CallInternalMethod(evalParam._token, "serialreadstring");
                        }
                        else if (evalParam._token.StartsWith("SerialAvailable(") || evalParam._token.StartsWith("serialavailable("))
                        {
                            yield return StartCoroutine(CallInternalMethod(evalParam._token, "serialavailable"));
                            res = gv.GlobalVariables.res;
                            //res = CallInternalMethod(evalParam._token, "serialavailable");
                        }
                        else if (evalParam._token.StartsWith("cos("))
                        {
                            yield return StartCoroutine(CallInternalMethod(evalParam._token, "cos"));
                            res = gv.GlobalVariables.res;
                            //res = CallInternalMethod(evalParam._token, "cos");
                        }
                        else if (evalParam._token.StartsWith("sin("))
                        {
                            yield return StartCoroutine(CallInternalMethod(evalParam._token, "sin"));
                            res = gv.GlobalVariables.res;
                            //res = CallInternalMethod(evalParam._token, "sin");
                        }
                        else if (evalParam._token.StartsWith("abs("))
                        {
                            yield return StartCoroutine(CallInternalMethod(evalParam._token, "abs"));
                            res = gv.GlobalVariables.res;
                            //res = CallInternalMethod(evalParam._token, "abs");
                        }
                        else if (evalParam._token.StartsWith("tan("))
                        {
                            yield return StartCoroutine(CallInternalMethod(evalParam._token, "tan"));
                            res = gv.GlobalVariables.res;
                            //res = CallInternalMethod(evalParam._token, "tan");
                        }
                        else if (evalParam._token.StartsWith("atan("))
                        {
                            yield return StartCoroutine(CallInternalMethod(evalParam._token, "atan"));
                            res = gv.GlobalVariables.res;
                            //res = CallInternalMethod(evalParam._token, "atan");
                        }
                        else if (evalParam._token.StartsWith("sqrt("))
                        {
                            yield return StartCoroutine(CallInternalMethod(evalParam._token, "sqrt"));
                            res = gv.GlobalVariables.res;
                            //res = CallInternalMethod(evalParam._token, "sqrt");
                        }
                        else if (evalParam._token.StartsWith("pow("))
                        {
                            yield return StartCoroutine(CallInternalMethod(evalParam._token, "pow"));
                            res = gv.GlobalVariables.res;
                            //res = CallInternalMethod(evalParam._token, "pow");
                        }
                        else if (evalParam._token.StartsWith("min("))
                        {
                            yield return StartCoroutine(CallInternalMethod(evalParam._token, "min"));
                            res = gv.GlobalVariables.res;
                            //res = CallInternalMethod(evalParam._token, "min");
                        }
                        else if (evalParam._token.StartsWith("max("))
                        {
                            yield return StartCoroutine(CallInternalMethod(evalParam._token, "max"));
                            res = gv.GlobalVariables.res;
                            //res = CallInternalMethod(evalParam._token, "max");
                        }
                        else if (evalParam._token.StartsWith("round("))
                        {
                            yield return StartCoroutine(CallInternalMethod(evalParam._token, "round"));
                            res = gv.GlobalVariables.res;
                            //res = CallInternalMethod(evalParam._token, "round");
                        }
                        else if (EvalCallProcedureHandler != null)
                        {
                            string methodName = evalParam._token;

                            int parenStartPos = methodName.IndexOf("(");

                            string arguments = methodName.Substring(parenStartPos);

                            if (arguments[0] == '(')
                                arguments = arguments.Substring(1);

                            if (arguments[arguments.Length - 1] == ')')
                                arguments = arguments.Substring(0, arguments.Length - 1);

                            methodName = methodName.Substring(0, parenStartPos);


                            yield return StartCoroutine((IEnumerator)EvalCallProcedureHandler(methodName, arguments, _LocalVariables));
                            res = gv.GlobalVariables.res;
                            //res = EvalCallProcedureHandler(methodName, arguments, _LocalVariables);
                            //res = gv.GlobalVariables.res;




                        }
                        else
                        {
                            LogInfo("[TokenToInstance VAR #2] EvalCallProcedureHandler is null. " + "Can't execute procedure " + evalParam._token);
                        }
                    }
                    else if (evalParam._token.IndexOf("[") >= 0 && evalParam._token.IndexOf("]") >= 0)
                    {
                        string methodName = evalParam._token;

                        int parenStartPos = methodName.IndexOf("[");

                        string arguments = methodName.Substring(parenStartPos);

                        if (arguments[0] == '[')
                            arguments = arguments.Substring(1);

                        if (arguments[arguments.Length - 1] == ']')
                            arguments = arguments.Substring(0, arguments.Length - 1);

                        methodName = methodName.Substring(0, parenStartPos);

                        object arrInstance = FindObject(methodName);


                        if (arrInstance != null)
                        {
                            if (arguments == string.Empty)
                                LogInfo("[TokenToInstance VAR #3] " + "Array need index -> " + methodName + " / " + arguments);
                            else
                            {
                                yield return StartCoroutine(Evaluate(arguments));
                                object result_index = gv.GlobalVariables.res;

                                //object result_index = Evaluate(arguments);
                                int ind = 0;

                                string tName = arrInstance.GetType().ToString();

                                if (tName.IndexOf("System.") == 0)
                                    tName = tName.Substring(7);

                                if (tName.IndexOf("Collections.") == 0)
                                    tName = tName.Substring(12);

                                if (tName.IndexOf("SPL.Script.Engine.") == 0)
                                    tName = tName.Substring(18);


                                switch (tName)
                                {
                                    case "Byte[]":
                                        byte[] arr_byte = (byte[])arrInstance;
                                        ind = Util.ToInt(result_index);
                                        res = arr_byte[ind];

                                        break;

                                    case "SByte[]":
                                        sbyte[] arr_sbyte = (sbyte[])arrInstance;
                                        ind = Util.ToInt(result_index);
                                        res = arr_sbyte[ind];
                                        break;

                                    case "Char[]":
                                        char[] arr_char = (char[])arrInstance;
                                        ind = Util.ToInt(result_index);
                                        res = arr_char[ind];
                                        break;

                                    case "Int16[]":
                                        Int16[] arr_int16 = (Int16[])arrInstance;
                                        ind = Util.ToInt(result_index);
                                        res = arr_int16[ind];
                                        break;

                                    case "Int32[]":
                                        Int32[] arr_int32 = (Int32[])arrInstance;
                                        ind = Util.ToInt(result_index);
                                        res = arr_int32[ind];
                                        break;

                                    case "Int64[]":
                                        Int64[] arr_int64 = (Int64[])arrInstance;
                                        ind = Util.ToInt(result_index);
                                        res = arr_int64[ind];
                                        break;

                                    case "UInt16[]":
                                        UInt16[] arr_Uint16 = (UInt16[])arrInstance;
                                        ind = Util.ToInt(result_index);
                                        res = arr_Uint16[ind];
                                        break;

                                    case "UInt32[]":
                                        UInt32[] arr_Uint32 = (UInt32[])arrInstance;
                                        ind = Util.ToInt(result_index);
                                        res = arr_Uint32[ind];
                                        break;

                                    case "UInt64[]":
                                        UInt64[] arr_Uint64 = (UInt64[])arrInstance;
                                        ind = Util.ToInt(result_index);
                                        res = arr_Uint64[ind];
                                        break;

                                    case "Double[]":
                                        Double[] arr_double = (Double[])arrInstance;
                                        ind = Util.ToInt(result_index);
                                        res = arr_double[ind];
                                        break;

                                    case "Single[]":
                                        Single[] arr_single = (Single[])arrInstance;
                                        ind = Util.ToInt(result_index);
                                        res = arr_single[ind];
                                        break;

                                    case "String[]":
                                        String[] arr_string = (String[])arrInstance;
                                        ind = Util.ToInt(result_index);
                                        res = arr_string[ind];
                                        break;

                                    case "String":
                                        //2016.09.25
                                        string string_dumy = arrInstance.ToString();
                                        ind = Util.ToInt(result_index);
                                        res = string_dumy[ind];
                                        break;

                                    case "Boolean[]":
                                        Boolean[] arr_bool = (Boolean[])arrInstance;
                                        ind = Util.ToInt(result_index);
                                        res = arr_bool[ind];
                                        break;

                                    case "Object[]":
                                        object[] arr_object = (object[])arrInstance;
                                        ind = Util.ToInt(result_index);
                                        res = arr_object[ind];
                                        break;

                                    case "ArrayList":
                                        ArrayList arrayList = (ArrayList)arrInstance;
                                        ind = Util.ToInt(result_index);
                                        res = arrayList[ind];
                                        break;

                                    case "Dictionary":
                                        Dictionary<object, object> splHashtable = (Dictionary<object, object>)arrInstance;
                                        res = splHashtable[result_index.ToString()];
                                        break;

                                    default:
                                        LogInfo("[TokenToInstance VAR #4] " + "Can't find array type " + tName);
                                        break;
                                }
                            }
                        }
                        else
                        {
                            LogInfo("[TokenToInstance VAR #5] " + "Can't find array variable " + methodName);
                        }
                    }
                    else
                    {

                        res = FindObject(evalParam._token);

                        if (res == null)
                        {
                            LogInfo("[TokenToInstance VAR #6] " + "Can't find variable " + evalParam._token + " / " + evalParam._exp);
                        }
                    }

                    GetToken(evalParam);

                    break;
                    //yield break;
            }

            evalParam._lastObject = res;


            //return res;
            gv.GlobalVariables.res = res;
        }

        //private object TokenToInstance_Origin(EvalEngineParam evalParam)
        //{
        //    object res = null;

        //    try
        //    {
        //        switch (evalParam._tokType)
        //        {
        //            case NUMBER:
        //                try
        //                {
        //                    if (evalParam._token.IndexOf("0x") == 0)
        //                    {
        //                        //hex
        //                        if (evalParam._token.Length == 4)
        //                            res = Util.HexToByte(evalParam._token);
        //                        else if (evalParam._token.Length == 6)
        //                            res = Util.HexToShort(evalParam._token);
        //                        else if (evalParam._token.Length == 10)
        //                            res = Util.HexToInt(evalParam._token);
        //                        else
        //                            LogInfo("[TokenToInstance NUMBER #1] " + "Can't conver from hex to number -> " + evalParam._token);
        //                    }
        //                    else
        //                    {
        //                        double val = double.Parse(evalParam._token);

        //                        res = val;
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    LogInfo("[TokenToInstance NUMBER #2] " + ex.ToString() + " => " + evalParam._token);
        //                }

        //                GetToken(evalParam);

        //                break;

        //            case STRING:
        //                try
        //                {
        //                    res = evalParam._token.ToString();
        //                }
        //                catch (Exception ex)
        //                {
        //                    LogInfo("[TokenToInstance STRING #1] " + ex.ToString());
        //                }

        //                GetToken(evalParam);

        //                break;

        //            case CHARACTER:
        //                try
        //                {
        //                    if (evalParam._token.Length == 1)
        //                    {
        //                        res = (char)evalParam._token[0];
        //                    }
        //                    else
        //                        res = evalParam._token.ToString();
        //                }
        //                catch (Exception ex)
        //                {
        //                    LogInfo("[TokenToInstance CHAR #1] " + ex.ToString());
        //                }

        //                GetToken(evalParam);

        //                break;

        //            case VARIABLE:

        //                try
        //                {

        //                    if (evalParam._token.IndexOf(".") >= 0)
        //                    {
        //                        if (evalParam._token.IndexOf("(") >= 0 && evalParam._token.IndexOf(")") >= 0)
        //                        {
        //                            //method
        //                            int dotPos = evalParam._token.IndexOf(".");

        //                            string objectName = evalParam._token.Substring(0, dotPos);
        //                            string methodName = evalParam._token.Substring(dotPos);

        //                            int parenStartPos = methodName.IndexOf("(");

        //                            string arguments = methodName.Substring(parenStartPos);

        //                            if (arguments[0] == '(')
        //                                arguments = arguments.Substring(1);

        //                            if (arguments[arguments.Length - 1] == ')')
        //                                arguments = arguments.Substring(0, arguments.Length - 1);

        //                            methodName = methodName.Substring(0, parenStartPos);


        //                            res = ExecuteMethod(objectName, methodName, arguments);
        //                        }
        //                        else if (evalParam._token.IndexOf("(") < 0 && evalParam._token.IndexOf(")") < 0)
        //                        {
        //                            //property
        //                            int dotPos = evalParam._token.IndexOf(".");
        //                            string objectName = evalParam._token.Substring(0, dotPos);
        //                            string properyName = evalParam._token.Substring(dotPos);

        //                            res = GetProperty(objectName, properyName);
        //                        }
        //                        else
        //                            LogInfo("[TokenToInstance VAR #1] " + "Can't find variable " + evalParam._token);
        //                    }
        //                    else if (evalParam._token.IndexOf("(") >= 0 && evalParam._token.IndexOf(")") >= 0)
        //                    {
        //                        //###################################################################
        //                        //2015.12.27
        //                        //내부 함수는 이곳에서 정의해 추가해 준다.
        //                        //###################################################################

        //                        //2017.11.20 Arduino 함수 추가

        //                        if (evalParam._token.StartsWith("map("))
        //                        {
        //                            res = CallInternalMethod(evalParam._token, "map");
        //                        }
        //                        else if (evalParam._token.StartsWith("digitalRead") || evalParam._token.StartsWith("DigitalRead"))
        //                        {
        //                            //2017.11.20 Arduino 함수 추가
        //                            res = CallInternalMethod(evalParam._token, "digitalread");
        //                        }
        //                        else if (evalParam._token.StartsWith("analogRead") || evalParam._token.StartsWith("AnalogRead"))
        //                        {
        //                            //2017.11.20 Arduino 함수 추가
        //                            res = CallInternalMethod(evalParam._token, "analogread");
        //                        }
        //                        else if (evalParam._token.StartsWith("dist("))
        //                        {
        //                            res = CallInternalMethod(evalParam._token, "dist");
        //                        }
        //                        else if (evalParam._token.StartsWith("millis("))
        //                        {
        //                            res = CallInternalMethod(evalParam._token, "millis");
        //                        }
        //                        else if (evalParam._token.StartsWith("random("))
        //                        {
        //                            res = CallInternalMethod(evalParam._token, "random");
        //                        }
        //                        else if (evalParam._token.StartsWith("loadImage(") || evalParam._token.StartsWith("LoadImage(") || evalParam._token.StartsWith("loadimage("))
        //                        {
        //                            res = CallInternalMethod(evalParam._token, "loadimage");
        //                        }
        //                        else if (evalParam._token.StartsWith("SerialOpen(") || evalParam._token.StartsWith("serialopen("))
        //                        {
        //                            res = CallInternalMethod(evalParam._token, "serialopen");
        //                        }
        //                        else if (evalParam._token.StartsWith("SerialList(") || evalParam._token.StartsWith("seriallist("))
        //                        {
        //                            res = CallInternalMethod(evalParam._token, "seriallist");
        //                        }
        //                        else if (evalParam._token.StartsWith("SerialClose(") || evalParam._token.StartsWith("serialclose("))
        //                        {
        //                            res = CallInternalMethod(evalParam._token, "serialclose");
        //                        }
        //                        else if (evalParam._token.StartsWith("SerialReadString(") || evalParam._token.StartsWith("serialreadstring("))
        //                        {
        //                            res = CallInternalMethod(evalParam._token, "serialreadstring");
        //                        }
        //                        else if (evalParam._token.StartsWith("SerialAvailable(") || evalParam._token.StartsWith("serialavailable("))
        //                        {
        //                            res = CallInternalMethod(evalParam._token, "serialavailable");
        //                        }
        //                        else if (evalParam._token.StartsWith("cos("))
        //                        {
        //                            res = CallInternalMethod(evalParam._token, "cos");
        //                        }
        //                        else if (evalParam._token.StartsWith("sin("))
        //                        {
        //                            res = CallInternalMethod(evalParam._token, "sin");
        //                        }
        //                        else if (evalParam._token.StartsWith("abs("))
        //                        {
        //                            res = CallInternalMethod(evalParam._token, "abs");
        //                        }
        //                        else if (evalParam._token.StartsWith("tan("))
        //                        {
        //                            res = CallInternalMethod(evalParam._token, "tan");
        //                        }
        //                        else if (evalParam._token.StartsWith("atan("))
        //                        {
        //                            res = CallInternalMethod(evalParam._token, "atan");
        //                        }
        //                        else if (evalParam._token.StartsWith("sqrt("))
        //                        {
        //                            res = CallInternalMethod(evalParam._token, "sqrt");
        //                        }
        //                        else if (evalParam._token.StartsWith("pow("))
        //                        {
        //                            res = CallInternalMethod(evalParam._token, "pow");
        //                        }
        //                        else if (evalParam._token.StartsWith("min("))
        //                        {
        //                            res = CallInternalMethod(evalParam._token, "min");
        //                        }
        //                        else if (evalParam._token.StartsWith("max("))
        //                        {
        //                            res = CallInternalMethod(evalParam._token, "max");
        //                        }
        //                        else if (evalParam._token.StartsWith("round("))
        //                        {
        //                            res = CallInternalMethod(evalParam._token, "round");
        //                        }
        //                        else if (EvalCallProcedureHandler != null)
        //                        {
        //                            string methodName = evalParam._token;

        //                            int parenStartPos = methodName.IndexOf("(");

        //                            string arguments = methodName.Substring(parenStartPos);

        //                            if (arguments[0] == '(')
        //                                arguments = arguments.Substring(1);

        //                            if (arguments[arguments.Length - 1] == ')')
        //                                arguments = arguments.Substring(0, arguments.Length - 1);

        //                            methodName = methodName.Substring(0, parenStartPos);


        //                            res = EvalCallProcedureHandler(methodName, arguments, _LocalVariables);
        //                            //res = gv.GlobalVariables.res;




        //                        }
        //                        else
        //                        {
        //                            LogInfo("[TokenToInstance VAR #2] EvalCallProcedureHandler is null. " + "Can't execute procedure " + evalParam._token);
        //                        }
        //                    }
        //                    else if (evalParam._token.IndexOf("[") >= 0 && evalParam._token.IndexOf("]") >= 0)
        //                    {
        //                        string methodName = evalParam._token;

        //                        int parenStartPos = methodName.IndexOf("[");

        //                        string arguments = methodName.Substring(parenStartPos);

        //                        if (arguments[0] == '[')
        //                            arguments = arguments.Substring(1);

        //                        if (arguments[arguments.Length - 1] == ']')
        //                            arguments = arguments.Substring(0, arguments.Length - 1);

        //                        methodName = methodName.Substring(0, parenStartPos);

        //                        object arrInstance = FindObject(methodName);


        //                        if (arrInstance != null)
        //                        {
        //                            if (arguments == string.Empty)
        //                                LogInfo("[TokenToInstance VAR #3] " + "Array need index -> " + methodName + " / " + arguments);
        //                            else
        //                            {
        //                                object result_index = Evaluate(arguments);
        //                                int ind = 0;

        //                                string tName = arrInstance.GetType().ToString();

        //                                if (tName.IndexOf("System.") == 0)
        //                                    tName = tName.Substring(7);

        //                                if (tName.IndexOf("Collections.") == 0)
        //                                    tName = tName.Substring(12);

        //                                if (tName.IndexOf("SPL.Script.Engine.") == 0)
        //                                    tName = tName.Substring(18);


        //                                switch (tName)
        //                                {
        //                                    case "Byte[]":
        //                                        byte[] arr_byte = (byte[])arrInstance;
        //                                        ind = Util.ToInt(result_index);
        //                                        res = arr_byte[ind];

        //                                        break;

        //                                    case "SByte[]":
        //                                        sbyte[] arr_sbyte = (sbyte[])arrInstance;
        //                                        ind = Util.ToInt(result_index);
        //                                        res = arr_sbyte[ind];
        //                                        break;

        //                                    case "Char[]":
        //                                        char[] arr_char = (char[])arrInstance;
        //                                        ind = Util.ToInt(result_index);
        //                                        res = arr_char[ind];
        //                                        break;

        //                                    case "Int16[]":
        //                                        Int16[] arr_int16 = (Int16[])arrInstance;
        //                                        ind = Util.ToInt(result_index);
        //                                        res = arr_int16[ind];
        //                                        break;

        //                                    case "Int32[]":
        //                                        Int32[] arr_int32 = (Int32[])arrInstance;
        //                                        ind = Util.ToInt(result_index);
        //                                        res = arr_int32[ind];
        //                                        break;

        //                                    case "Int64[]":
        //                                        Int64[] arr_int64 = (Int64[])arrInstance;
        //                                        ind = Util.ToInt(result_index);
        //                                        res = arr_int64[ind];
        //                                        break;

        //                                    case "UInt16[]":
        //                                        UInt16[] arr_Uint16 = (UInt16[])arrInstance;
        //                                        ind = Util.ToInt(result_index);
        //                                        res = arr_Uint16[ind];
        //                                        break;

        //                                    case "UInt32[]":
        //                                        UInt32[] arr_Uint32 = (UInt32[])arrInstance;
        //                                        ind = Util.ToInt(result_index);
        //                                        res = arr_Uint32[ind];
        //                                        break;

        //                                    case "UInt64[]":
        //                                        UInt64[] arr_Uint64 = (UInt64[])arrInstance;
        //                                        ind = Util.ToInt(result_index);
        //                                        res = arr_Uint64[ind];
        //                                        break;

        //                                    case "Double[]":
        //                                        Double[] arr_double = (Double[])arrInstance;
        //                                        ind = Util.ToInt(result_index);
        //                                        res = arr_double[ind];
        //                                        break;

        //                                    case "Single[]":
        //                                        Single[] arr_single = (Single[])arrInstance;
        //                                        ind = Util.ToInt(result_index);
        //                                        res = arr_single[ind];
        //                                        break;

        //                                    case "String[]":
        //                                        String[] arr_string = (String[])arrInstance;
        //                                        ind = Util.ToInt(result_index);
        //                                        res = arr_string[ind];
        //                                        break;

        //                                    case "String":
        //                                        //2016.09.25
        //                                        string string_dumy = arrInstance.ToString();
        //                                        ind = Util.ToInt(result_index);
        //                                        res = string_dumy[ind];
        //                                        break;

        //                                    case "Boolean[]":
        //                                        Boolean[] arr_bool = (Boolean[])arrInstance;
        //                                        ind = Util.ToInt(result_index);
        //                                        res = arr_bool[ind];
        //                                        break;

        //                                    case "Object[]":
        //                                        object[] arr_object = (object[])arrInstance;
        //                                        ind = Util.ToInt(result_index);
        //                                        res = arr_object[ind];
        //                                        break;

        //                                    case "ArrayList":
        //                                        ArrayList arrayList = (ArrayList)arrInstance;
        //                                        ind = Util.ToInt(result_index);
        //                                        res = arrayList[ind];
        //                                        break;

        //                                    case "Dictionary":
        //                                        Dictionary<object, object> splHashtable = (Dictionary<object, object>)arrInstance;
        //                                        res = splHashtable[result_index.ToString()];
        //                                        break;

        //                                    default:
        //                                        LogInfo("[TokenToInstance VAR #4] " + "Can't find array type " + tName);
        //                                        break;
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            LogInfo("[TokenToInstance VAR #5] " + "Can't find array variable " + methodName);
        //                        }
        //                    }
        //                    else
        //                    {

        //                        res = FindObject(evalParam._token);

        //                        if (res == null)
        //                        {
        //                            LogInfo("[TokenToInstance VAR #6] " + "Can't find variable " + evalParam._token + " / " + evalParam._exp);
        //                        }
        //                    }

        //                    GetToken(evalParam);
        //                }
        //                catch (Exception ex)
        //                {
        //                    LogInfo("[TokenToInstance Error #7] " + ex.ToString());
        //                }

        //                break;
        //        }

        //        evalParam._lastObject = res;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogInfo("[TokenToInstance Error #8] " + ex.ToString());
        //    }

        //    return res;
        //}

        #endregion TokenToInstance()



        //2015.12.27
        private IEnumerator CallInternalMethod(string evalParam_token, string method_name)
        {
            string objectName = method_name;
            string methodName = evalParam_token;

            int parenStartPos = methodName.IndexOf("(");

            string arguments = methodName.Substring(parenStartPos);

            if (arguments[0] == '(')
                arguments = arguments.Substring(1);

            if (arguments[arguments.Length - 1] == ')')
                arguments = arguments.Substring(0, arguments.Length - 1);

            methodName = method_name;

            yield return StartCoroutine(ExecuteMethod(objectName, methodName, arguments));
            //return ExecuteMethod(objectName, methodName, arguments);
        }



        #region GetToken()

        private void GetToken(EvalEngineParam evalParam)
        {
            evalParam._tokType = NONE;
            evalParam._token = string.Empty;
            char ch;

            try
            {

                if (evalParam._expIdx == evalParam._exp.Length)
                {
                    evalParam._token = EOE;
                    return;
                }

                while (evalParam._expIdx < evalParam._exp.Length && (evalParam._exp[evalParam._expIdx] == ' ' || evalParam._exp[evalParam._expIdx] == '\t'))
                    ++evalParam._expIdx;

                if (evalParam._expIdx == evalParam._exp.Length)
                {
                    evalParam._token = EOE;
                    return;
                }


                if (IsDelim(evalParam._exp[evalParam._expIdx]))
                {
                    evalParam._token += evalParam._exp[evalParam._expIdx];
                    evalParam._expIdx++;
                    evalParam._tokType = DELIMITER;

                    if (evalParam._expIdx >= evalParam._exp.Length)
                        return;

                    string tmpStr = evalParam._token + evalParam._exp[evalParam._expIdx];

                    if (tmpStr == "==" || tmpStr == "!=" || tmpStr == ">=" || tmpStr == "<=" || tmpStr == "&&" || tmpStr == "||" || tmpStr == "<<" || tmpStr == ">>")
                    {
                        evalParam._token += evalParam._exp[evalParam._expIdx];
                        evalParam._expIdx++;
                    }
                }
                else if (IsLetter(evalParam._exp[evalParam._expIdx]))
                {
                    bool doFlag = false;
                    bool parenOpen = false;

                    doFlag = !IsDelim(evalParam._exp[evalParam._expIdx]);

                    while (doFlag)
                    {
                        evalParam._token += evalParam._exp[evalParam._expIdx];
                        evalParam._expIdx++;

                        if (evalParam._expIdx >= evalParam._exp.Length)
                            break;

                        doFlag = !IsDelim(evalParam._exp[evalParam._expIdx]);

                        if (!doFlag)
                        {
                            if (evalParam._exp[evalParam._expIdx] == '(' && !parenOpen)
                            {
                                parenOpen = true;
                                doFlag = true;
                            }
                            else if (evalParam._exp[evalParam._expIdx] == ')' && parenOpen)
                            {
                                evalParam._token += evalParam._exp[evalParam._expIdx];
                                evalParam._expIdx++;

                                parenOpen = false;
                                doFlag = false;
                            }
                            else if (parenOpen)
                                doFlag = true;
                        }
                    }

                    evalParam._tokType = VARIABLE;
                }
                else if (evalParam._exp[evalParam._expIdx] == '.')
                {
                    LogInfo("[GetToken] " + "Do not support multiple method or propery. " + evalParam._exp);
                }
                else if (IsDigit(evalParam._exp[evalParam._expIdx]))
                {
                    while (!IsDelim(evalParam._exp[evalParam._expIdx]))
                    {
                        evalParam._token += evalParam._exp[evalParam._expIdx];
                        evalParam._expIdx++;

                        if (evalParam._expIdx >= evalParam._exp.Length)
                            break;
                    }

                    evalParam._tokType = NUMBER;
                }
                else if (evalParam._exp[evalParam._expIdx] == '\"')
                {
                    evalParam._expIdx++;
                    ch = evalParam._exp[evalParam._expIdx];

                    while (ch != '\"')
                    {
                        evalParam._token += ch;
                        evalParam._expIdx++;

                        if (evalParam._expIdx >= evalParam._exp.Length)
                            LogInfo("[GetToken] " + "Need closing double quote");

                        ch = evalParam._exp[evalParam._expIdx];
                    }

                    evalParam._expIdx++;
                    evalParam._tokType = STRING;
                }
                else if (evalParam._exp[evalParam._expIdx] == '\'')
                {
                    evalParam._expIdx++;
                    ch = evalParam._exp[evalParam._expIdx];

                    while (ch != '\'')
                    {
                        evalParam._token += ch;
                        evalParam._expIdx++;

                        if (evalParam._expIdx >= evalParam._exp.Length)
                            LogInfo("[GetToken] " + "Need closing single quote");

                        ch = evalParam._exp[evalParam._expIdx];
                    }

                    evalParam._expIdx++;
                    evalParam._tokType = CHARACTER;
                }
                else
                {
                    evalParam._token = EOE;
                    return;
                }
            }
            catch (Exception ex)
            {
                LogInfo("[GetToken] " + ex.ToString());
            }

        }


        #endregion GetToken()


        private void CheckMainCameraInstance()
        {
            if (_MainCameraContainerInstance == null)
                _MainCameraContainerInstance = GameObject.Find("MainCameraContainer");

            if (_MainCamControlInstance == null)
                _MainCamControlInstance = _MainCameraContainerInstance.GetComponent<MainCamControl>();
        }


        private object FindObject(string name)
        {
            object res = null;

            try
            {
                if (_LocalVariables.ContainsKey(name))
                    res = _LocalVariables[name];
                else if (_GlobalVariables.ContainsKey(name))
                    res = _GlobalVariables[name];
                else if (_CommonVariables.ContainsKey(name))
                    res = _CommonVariables[name];
            }
            catch (Exception ex)
            {
                LogInfo("[FindObject] " + ex.ToString());
            }

            return res;
        }



        private IEnumerator ExecuteMethod(string objName, string methodName, string arguments)
        {
            string methodName_lower = methodName.ToLower();

            //2015.12.27
            string objName_lower = objName.ToLower();

            object res = null;


            //2015.12.27
            object instance = null;

            bool need_instance_yn = true;

            //새로운 내부 함수가 추가된 경우 TokenToInstance() 모듈 안에 추가해 주어야 한다.
            //1010 라인에 있음
            //그 다음에 이곳에 추가해 주어야 한다.

            if (objName_lower == "map" || objName_lower == "millis" || objName_lower == "dist" || objName_lower == "sin" || objName_lower == "cos" ||
                objName_lower == "abs" || objName_lower == "tan" || objName_lower == "sqrt" || objName_lower == "pow" ||
                objName_lower == "min" || objName_lower == "atan" || objName_lower == "max" || objName_lower == "round" ||
                objName_lower == "loadimage" || objName_lower == "random" ||
                objName_lower == "serial" || objName_lower == "seriallist" || objName_lower == "serialavailable" || objName_lower == "serialreadstring")
                need_instance_yn = false;


            //2017.11.20
            if (objName_lower == "digitalread" || objName_lower == "analogread")
                need_instance_yn = false;


            if (need_instance_yn)
                instance = FindObject(objName);

            if (need_instance_yn && instance == null)
            {
                //2016.07.12
                //LogInfo("[ExecuteMethod] " + "Can't find method -> " + objName + " / " + methodName);
                //return res;

                //2016.07.12
                yield return StartCoroutine(Evaluate(objName));
                instance = gv.GlobalVariables.res;
                //instance = Evaluate(objName);
            }



            //2015.12.27
            string tName = "";

            if (need_instance_yn == false)
                tName = objName_lower;
            else
                tName = instance.GetType().Name;



            //2015.12.27
            if (methodName_lower == "map")
            {
                if (tName == "map")
                {
                    if (!string.IsNullOrEmpty(arguments))
                    {
                        ArrayList str_list = SPLHelper.GetStringList(arguments);

                        if (str_list != null && str_list.Count == 5)
                        {
                            yield return StartCoroutine(Evaluate(str_list[0].ToString()));
                            float x = Util.ToFloat(gv.GlobalVariables.res);

                            yield return StartCoroutine(Evaluate(str_list[1].ToString()));
                            float in_min = Util.ToFloat(gv.GlobalVariables.res);

                            yield return StartCoroutine(Evaluate(str_list[2].ToString()));
                            float in_max = Util.ToFloat(gv.GlobalVariables.res);

                            yield return StartCoroutine(Evaluate(str_list[3].ToString()));
                            float out_min = Util.ToFloat(gv.GlobalVariables.res);

                            yield return StartCoroutine(Evaluate(str_list[4].ToString()));
                            float out_max = Util.ToFloat(gv.GlobalVariables.res);

                            //float x = Util.ToFloat(Evaluate(str_list[0].ToString()));

                            //float in_min = Util.ToFloat(Evaluate(str_list[1].ToString()));
                            //float in_max = Util.ToFloat(Evaluate(str_list[2].ToString()));
                            //float out_min = Util.ToFloat(Evaluate(str_list[3].ToString()));
                            //float out_max = Util.ToFloat(Evaluate(str_list[4].ToString()));


                            gv.GlobalVariables.res = Mathf.Floor((x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min);
                            yield break;
                        }
                    }
                }
            }
            else if (methodName_lower == "digitalread")
            {
                //2017.11.20
                if (!string.IsNullOrEmpty(arguments))
                {
                    ArrayList str_list = SPLHelper.GetStringList(arguments);

                    if (str_list != null && str_list.Count == 1)
                    {
                        yield return StartCoroutine(Evaluate(str_list[0].ToString()));
                        int pin = Util.ToInt(gv.GlobalVariables.res);
                        //int pin = Util.ToInt(Evaluate(str_list[0].ToString()));
                        int v = Util.ToInt(_GlobalVariables["_DIGITAL_" + pin.ToString()]);

                        gv.GlobalVariables.res = v;
                        yield break;
                        //return v;
                    }
                }
            }
            else if (methodName_lower == "analogread")
            {
                //2017.11.20
                if (!string.IsNullOrEmpty(arguments))
                {
                    ArrayList str_list = SPLHelper.GetStringList(arguments);

                    if (str_list != null && str_list.Count == 1)
                    {
                        yield return StartCoroutine(Evaluate(str_list[0].ToString()));
                        int pin = Util.ToInt(gv.GlobalVariables.res);

                        //int pin = Util.ToInt(Evaluate(str_list[0].ToString()));
                        int v = Util.ToInt(_GlobalVariables["_ANALOG_" + pin.ToString()]);

                        gv.GlobalVariables.res = v;
                        yield break;
                        //return v;
                    }
                }
            }
            else if (methodName_lower == "cos")
            {
                if (tName == "cos")
                {
                    if (!string.IsNullOrEmpty(arguments))
                    {
                        ArrayList str_list = SPLHelper.GetStringList(arguments);

                        if (str_list != null && str_list.Count == 1)
                        {
                            yield return StartCoroutine(Evaluate(str_list[0].ToString()));
                            int x = Util.ToInt(gv.GlobalVariables.res);

                            //float x = Util.ToFloat(Evaluate(str_list[0].ToString()));

                            gv.GlobalVariables.res = x;
                            yield break;
                            //return Math.Cos(x);
                        }
                    }
                }
            }
            else if (methodName_lower == "dist")
            {
                if (tName == "dist")
                {
                    if (!string.IsNullOrEmpty(arguments))
                    {
                        ArrayList str_list = SPLHelper.GetStringList(arguments);

                        if (str_list != null && str_list.Count == 4)
                        {
                            yield return StartCoroutine(Evaluate(str_list[0].ToString()));
                            float x1 = Util.ToFloat(gv.GlobalVariables.res);

                            yield return StartCoroutine(Evaluate(str_list[1].ToString()));
                            float y1 = Util.ToFloat(gv.GlobalVariables.res);

                            yield return StartCoroutine(Evaluate(str_list[2].ToString()));
                            float x2 = Util.ToFloat(gv.GlobalVariables.res);

                            yield return StartCoroutine(Evaluate(str_list[3].ToString()));
                            float y2 = Util.ToFloat(gv.GlobalVariables.res);

                            //float x1 = Util.ToFloat(Evaluate(str_list[0].ToString()));
                            //float y1 = Util.ToFloat(Evaluate(str_list[1].ToString()));
                            //float x2 = Util.ToFloat(Evaluate(str_list[2].ToString()));
                            //float y2 = Util.ToFloat(Evaluate(str_list[3].ToString()));

                            float dx = x2 - x1;
                            float dy = y2 - y1;

                            gv.GlobalVariables.res = Math.Sqrt(dx * dx + dy * dy);
                            yield break;
                            //return Math.Sqrt(dx * dx + dy * dy);
                        }
                    }
                }
            }
            else if (methodName_lower == "millis")
            {
                if (tName == "millis")
                {
                    DateTime curTime = DateTime.Now;
                    DateTime engine_start_time = Util.ToDateTime(_GlobalVariables["_ENGINE_START_TIME_"]);
                    TimeSpan diffTime = curTime - engine_start_time;

                    gv.GlobalVariables.res = (diffTime.Ticks / 10000);
                    yield break;
                    //return (diffTime.Ticks / 10000);
                }
            }
            else if (methodName_lower == "random")
            {
                //2016.09.25
                if (tName == "random")
                {
                    if (!string.IsNullOrEmpty(arguments))
                    {
                        ArrayList str_list = SPLHelper.GetStringList(arguments);

                        if (str_list != null && str_list.Count == 1)
                        {
                            yield return StartCoroutine(Evaluate(str_list[0].ToString()));
                            int max = Util.ToInt(gv.GlobalVariables.res);
                            //int max = Util.ToInt(Evaluate(str_list[0].ToString()));

                            //return UnityEngine.Random.Range(0, max);
                            gv.GlobalVariables.res = UnityEngine.Random.Range(0, max);
                            yield break;
                        }
                        else if (str_list != null && str_list.Count == 2)
                        {
                            yield return StartCoroutine(Evaluate(str_list[0].ToString()));
                            int min = Util.ToInt(gv.GlobalVariables.res);

                            yield return StartCoroutine(Evaluate(str_list[1].ToString()));
                            int max = Util.ToInt(gv.GlobalVariables.res);

                            //int min = Util.ToInt(Evaluate(str_list[0].ToString()));
                            //int max = Util.ToInt(Evaluate(str_list[1].ToString()));

                            //return UnityEngine.Random.Range(min, max);
                            gv.GlobalVariables.res = UnityEngine.Random.Range(min, max);
                            yield break;

                        }
                    }
                }
            }
            else if (methodName_lower == "sin")
            {
                if (tName == "sin")
                {
                    if (!string.IsNullOrEmpty(arguments))
                    {
                        ArrayList str_list = SPLHelper.GetStringList(arguments);

                        if (str_list != null && str_list.Count == 1)
                        {
                            yield return StartCoroutine(Evaluate(str_list[0].ToString()));
                            float x = Util.ToFloat(gv.GlobalVariables.res);
                            //float x = Util.ToFloat(Evaluate(str_list[0].ToString()));

                            gv.GlobalVariables.res = Math.Sin(x);
                            yield break;
                            //return Math.Sin(x);
                        }
                    }
                }
            }
            else if (methodName_lower == "sqrt")
            {
                if (tName == "sqrt")
                {
                    if (!string.IsNullOrEmpty(arguments))
                    {
                        ArrayList str_list = SPLHelper.GetStringList(arguments);

                        if (str_list != null && str_list.Count == 1)
                        {
                            yield return StartCoroutine(Evaluate(str_list[0].ToString()));
                            float x = Util.ToFloat(gv.GlobalVariables.res);
                            //float x = Util.ToFloat(Evaluate(str_list[0].ToString()));

                            gv.GlobalVariables.res = Math.Sqrt(x);
                            yield break;
                            //return Math.Sqrt(x);
                        }
                    }
                }
            }
            else if (methodName_lower == "tan")
            {
                if (tName == "tan")
                {
                    if (!string.IsNullOrEmpty(arguments))
                    {
                        ArrayList str_list = SPLHelper.GetStringList(arguments);

                        if (str_list != null && str_list.Count == 1)
                        {
                            yield return StartCoroutine(Evaluate(str_list[0].ToString()));
                            float x = Util.ToFloat(gv.GlobalVariables.res);
                            //float x = Util.ToFloat(Evaluate(str_list[0].ToString()));

                            gv.GlobalVariables.res = Math.Tan(x);
                            yield break;
                            //return Math.Tan(x);
                        }
                    }
                }
            }
            else if (methodName_lower == "atan")
            {
                if (tName == "atan")
                {
                    if (!string.IsNullOrEmpty(arguments))
                    {
                        ArrayList str_list = SPLHelper.GetStringList(arguments);

                        if (str_list != null && str_list.Count == 1)
                        {
                            yield return StartCoroutine(Evaluate(str_list[0].ToString()));
                            float x = Util.ToFloat(gv.GlobalVariables.res);
                            //float x = Util.ToFloat(Evaluate(str_list[0].ToString()));

                            gv.GlobalVariables.res = Math.Atan(x);
                            yield break;
                            //return Math.Atan(x);
                        }
                    }
                }
            }
            else if (methodName_lower == "pow")
            {
                if (tName == "pow")
                {
                    if (!string.IsNullOrEmpty(arguments))
                    {
                        ArrayList str_list = SPLHelper.GetStringList(arguments);

                        if (str_list != null && str_list.Count == 2)
                        {
                            yield return StartCoroutine(Evaluate(str_list[0].ToString()));
                            float x = Util.ToFloat(gv.GlobalVariables.res);

                            yield return StartCoroutine(Evaluate(str_list[1].ToString()));
                            float y = Util.ToFloat(gv.GlobalVariables.res);

                            //float x = Util.ToFloat(Evaluate(str_list[0].ToString()));
                            //float y = Util.ToFloat(Evaluate(str_list[1].ToString()));

                            //return Math.Pow(x, y);
                            gv.GlobalVariables.res = Math.Pow(x, y);
                            yield break;
                        }
                    }
                }
            }
            else if (methodName_lower == "min")
            {
                if (tName == "min")
                {
                    if (!string.IsNullOrEmpty(arguments))
                    {
                        ArrayList str_list = SPLHelper.GetStringList(arguments);

                        if (str_list != null && str_list.Count == 2)
                        {
                            //float x = Util.ToFloat(Evaluate(str_list[0].ToString()));
                            //float y = Util.ToFloat(Evaluate(str_list[1].ToString()));

                            yield return StartCoroutine(Evaluate(str_list[0].ToString()));
                            float x = Util.ToFloat(gv.GlobalVariables.res);

                            yield return StartCoroutine(Evaluate(str_list[1].ToString()));
                            float y = Util.ToFloat(gv.GlobalVariables.res);

                            gv.GlobalVariables.res = Math.Min(x, y);
                            yield break;

                            //return Math.Min(x, y);
                        }
                    }
                }
            }
            else if (methodName_lower == "max")
            {
                if (tName == "max")
                {
                    if (!string.IsNullOrEmpty(arguments))
                    {
                        ArrayList str_list = SPLHelper.GetStringList(arguments);

                        if (str_list != null && str_list.Count == 2)
                        {
                            yield return StartCoroutine(Evaluate(str_list[0].ToString()));
                            float x = Util.ToFloat(gv.GlobalVariables.res);

                            yield return StartCoroutine(Evaluate(str_list[1].ToString()));
                            float y = Util.ToFloat(gv.GlobalVariables.res);

                            gv.GlobalVariables.res = Math.Max(x, y);
                            yield break;

                            //float x = Util.ToFloat(Evaluate(str_list[0].ToString()));
                            //float y = Util.ToFloat(Evaluate(str_list[1].ToString()));

                            //return Math.Max(x, y);
                        }
                    }
                }
            }
            else if (methodName_lower == "round")
            {
                if (tName == "round")
                {
                    if (!string.IsNullOrEmpty(arguments))
                    {
                        ArrayList str_list = SPLHelper.GetStringList(arguments);

                        if (str_list != null && str_list.Count == 1)
                        {

                            yield return StartCoroutine(Evaluate(str_list[0].ToString()));
                            float x = Util.ToFloat(gv.GlobalVariables.res);

                            gv.GlobalVariables.res = Math.Round(x);
                            yield break;

                            //float x = Util.ToFloat(Evaluate(str_list[0].ToString()));

                            //return Math.Round(x);
                        }
                    }
                }
            }


            gv.GlobalVariables.res = res;
            yield break;
            //return res;
        }



        private object GetProperty(string objName, string propName)
        {
            string propName_lower = propName.ToLower();

            object res = null;

            object instance = FindObject(objName);

            if (instance == null)
            {
                LogInfo("[GetProperty] " + "Can't find object -> " + objName);
                return res;
            }

            string tName = instance.GetType().Name;


            try
            {
                //Do Someting

            }
            catch (Exception ex)
            {
                LogInfo("[GetProperty] " + ex.ToString());
            }

            return res;
        }



        #region Check Modules

        private bool IsNumeric(object o)
        {
            string tName = o.GetType().Name;

            bool res = false;

            try
            {

                switch (tName)
                {
                    case "Double":
                        res = true;
                        break;

                    case "Single":
                        res = true;
                        break;

                    case "Int64":
                        res = true;
                        break;

                    case "Int32":
                        res = true;
                        break;

                    case "Int16":
                        res = true;
                        break;

                    case "UInt64":
                        res = true;
                        break;

                    case "UInt32":
                        res = true;
                        break;

                    case "UInt16":
                        res = true;
                        break;

                    case "Byte":
                        res = true;
                        break;

                    case "SByte":
                        res = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                LogInfo("[IsNumeric] " + ex.ToString());
            }

            return res;
        }

        private bool IsBool(object o)
        {
            string tName = o.GetType().Name;

            bool res = false;

            try
            {

                switch (tName)
                {
                    case "Boolean":
                        res = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                LogInfo("[IsBool] " + ex.ToString());
            }

            return res;
        }


        private bool IsString(object o)
        {
            try
            {
                string tName = o.GetType().Name;

                if (tName == "String")
                    return true;
            }
            catch (Exception ex)
            {
                LogInfo("[IsString] " + ex.ToString());
            }

            return false;
        }

        private bool IsDateTime(object o)
        {
            try
            {
                string tName = o.GetType().Name;

                if (tName == "DateTime")
                    return true;
            }
            catch (Exception ex)
            {
                LogInfo("[IsDateTime] " + ex.ToString());
            }

            return false;
        }



        private bool IsDelim(char c)
        {
            try
            {
                if (" \t\r,;<>+-/*%^=()!&|~".IndexOf(c) != -1)
                    return true;
            }
            catch (Exception ex)
            {
                LogInfo("[IsDelim] " + ex.ToString());
            }

            return false;
        }

        private bool IsSpaceOrTab(char c)
        {
            try
            {
                if (c == ' ' || c == '\t')
                    return true;
            }
            catch (Exception ex)
            {
                LogInfo("[IsSpaceOrTab] " + ex.ToString());
            }

            return false;
        }

        private bool IsLetter(char c)
        {
            try
            {
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                    return true;
            }
            catch (Exception ex)
            {
                LogInfo("[IsLetter] " + ex.ToString());
            }

            return false;
        }

        private bool IsDigit(char c)
        {
            try
            {
                if ((c >= '0' && c <= '9'))
                    return true;
            }
            catch (Exception ex)
            {
                LogInfo("[IsDigit] " + ex.ToString());
            }

            return false;
        }


        #endregion Check Modules


        public void ExtEvalCallProcedureHandler(string procedureName, object value)
        {
            if (_LocalVariables.ContainsKey("value"))
                _LocalVariables["value"] = value;
            else
                _LocalVariables.Add("value", value);

            EvalCallProcedureHandler(procedureName, string.Empty, _LocalVariables);
        }
    }
}
