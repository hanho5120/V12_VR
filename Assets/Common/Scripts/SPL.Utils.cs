using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Win32;

namespace SPL.Common
{

	public class RegstryHelper
	{
		public static string GetSPLDuinoActivatedKey()
		{
			#if UNITY_WEBPLAYER
			return string.Empty;
			#else
			string keyStr = @"SOFTWARE\\HelloApps\SPL3D";
			
			RegistryKey MyReg = Registry.CurrentUser.OpenSubKey(keyStr);
			
			if (MyReg != null)
				return (string)MyReg.GetValue("SPL3DActivatedKey");
			else
				return string.Empty;
			#endif
		}
	}

    public class CircularQueue
    {
        #region Circular Queue

        const int CIRCLED_QUEUE_SIZE = 9084;

        byte[] _CQ_Array = new byte[CIRCLED_QUEUE_SIZE];
        int _s_array_ind = -1;
        int _e_array_ind = 0;

        public CircularQueue()
        {
        }

        public void CQ_AddBytes(byte[] bytes, int count)
        {
            if (_s_array_ind < 0)
                _s_array_ind = 0;

            for (int i = 0; i < count; i++)
            {
                _CQ_Array[_e_array_ind] = bytes[i];

                _e_array_ind++;

                if (_e_array_ind >= CIRCLED_QUEUE_SIZE)
                    _e_array_ind = 0;
            }
        }

        public int CQ_GetLength()
        {
            if (_s_array_ind < 0 || _e_array_ind < 0)
                return 0;
            else if (_e_array_ind >= _s_array_ind)
                return (_e_array_ind - _s_array_ind);
            else
            {
                //Console.WriteLine("CQ_GetLength -> " + _s_array_ind + " / " + _e_array_ind);

                int len1 = CIRCLED_QUEUE_SIZE - _s_array_ind;
                int len2 = _e_array_ind;

                return len1 + len2;
            }
        }


        public byte[] CQ_GetData(int length)
        {
            byte[] res = new byte[length];

            for (int i = 0; i < length; i++)
            {
                res[i] = _CQ_Array[_s_array_ind];

                _s_array_ind++;

                if (_s_array_ind >= CIRCLED_QUEUE_SIZE)
                    _s_array_ind = 0;
            }

            return res;
        }


        public void CQ_RemoveData(int length)
        {
            for (int i = 0; i < length; i++)
            {
                _s_array_ind++;

                if (_s_array_ind >= CIRCLED_QUEUE_SIZE)
                    _s_array_ind = 0;
            }
        }

        public void CQ_ClearData()
        {
            _s_array_ind = -1;
            _e_array_ind = 0;
        }

        #endregion Circular Queue
    }


    //public class StringBuilder
    //{
    //    ArrayList list = new ArrayList();

    //    public void Append(string str)
    //    {
    //        list.Add(str);
    //    }

    //    public void Append(char ch)
    //    {
    //        list.Add(ch.ToString());
    //    }

    //    public override string ToString()
    //    {
    //        string res = string.Empty;

    //        foreach (object item in list)
    //        {
    //            res = res + item.ToString();
    //        }

    //        return res;
    //    }

    //    public string ToText()
    //    {
    //        string res = string.Empty;

    //        foreach (object item in list)
    //        {
    //            if (res == string.Empty)
    //                res = item.ToString();
    //            else
    //                res = res + '\n' + item.ToString();
    //        }

    //        return res;
    //    }
    //}

    public class Util
    {

        public static float map(float x, float in_min, float in_max, float out_min, float out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }


		public static string RestoreSingleQuatString(string str)
		{
			if (str == string.Empty)
				return string.Empty;
			
			string res = string.Empty;
			
			for (int i = 0; i < str.Length; i++)
			{
				if (str[i] == (char)0x02)
					res += '\'';
				else
					res += str[i];
			}
			
			return res;
		}

        public static string GetType(object obj)
        {
            return obj.GetType().ToString();
        }

        public static int IndexOf(string src, string target)
        {
            return src.IndexOf(target);
        }
        

        public static bool StartWith(string src, string target)
        {
            if (src.IndexOf(target) == 0)
                return true;
            else
                return false;
        }


        public static bool EndWith(string src, string target)
        {
            bool res = false;

            int end_index = src.Length - target.Length;
            bool sameFlag = true;

            for (int t = 0; t < target.Length; t++)
            {
                if (target[t] != src[t + end_index])
                    sameFlag = false;
            }

            if (sameFlag)
                res = true;

            return res;
        }


        //0x01 -> 1
        public static byte HexToByte(string hexStr)
        {
            int res = 0;

            hexStr = hexStr.ToLower();

            if (hexStr.IndexOf("0x") == 0)
                hexStr = hexStr.Substring(2);

            if (hexStr.Length == 0)
                throw new Exception("Can't convert from hex to byte #1 -> " + hexStr);

            if (hexStr.Length != 2)
                throw new Exception("Can't convert from hex to byte #2 -> " + hexStr);

            for (int i = 0; i <= 1; i++)
            {
                int num = 0;
                int hexInd = 1 - i;

                switch (hexStr[hexInd])
                {
                    case '0':
                        num = 0;
                        break;

                    case '1':
                        num = 1;
                        break;

                    case '2':
                        num = 2;
                        break;

                    case '3':
                        num = 3;
                        break;

                    case '4':
                        num = 4;
                        break;

                    case '5':
                        num = 5;
                        break;

                    case '6':
                        num = 6;
                        break;


                    case '7':
                        num = 7;
                        break;

                    case '8':
                        num = 8;
                        break;

                    case '9':
                        num = 9;
                        break;

                    case 'a':
                        num = 10;
                        break;

                    case 'b':
                        num = 11;
                        break;

                    case 'c':
                        num = 12;
                        break;

                    case 'd':
                        num = 13;
                        break;

                    case 'e':
                        num = 14;
                        break;

                    case 'f':
                        num = 15;
                        break;

                    default:
                        throw new Exception("Can't convert from hex to byte #3 -> " + hexStr);
                }

                if (i == 0)
                    res = res + num;
                else
                    res = res + (num * 16);

            }

            return (byte)res;
        }


        public static byte[] HexToBytes(string hexStr)
        {
            byte[] res = null;

            hexStr = hexStr.ToLower();

            if (hexStr.IndexOf("0x") == 0)
                hexStr = hexStr.Substring(2);

            if (hexStr.Length == 0)
                throw new Exception("Can't convert from hex to byte -> " + hexStr);

            if ((hexStr.Length % 2) == 1)
                throw new Exception("Can't convert from hex to byte -> " + hexStr);

            res = new byte[hexStr.Length / 2];

            for (int i = 0; i < hexStr.Length; i = i + 2)
            {
                string hex = hexStr.Substring(i, 2);

                res[i / 2] = HexToByte(hex);
            }

            return res;
        }


        public static short HexToShort(string hexStr)
        {
            return (short)HexToInt(hexStr);
        }

        public static ushort HexToUShort(string hexStr)
        {
            return (ushort)HexToUInt(hexStr);
        }


        //0x0001 -> 1
        public static int HexToInt(string hexStr)
        {
            int res = 0;

            hexStr = hexStr.ToLower();

            if (hexStr.IndexOf("0x") == 0)
                hexStr = hexStr.Substring(2);

            if (hexStr.Length == 0)
                throw new Exception("Can't convert from hex to int -> " + hexStr);

            if ((hexStr.Length % 2) == 1)
                throw new Exception("Can't convert from hex to int -> " + hexStr);


            byte[] byteArr = HexToBytes(hexStr);

            for (int i = 0; i < byteArr.Length; i++)
            {
                int convByte = (int)byteArr[i];
                int shiftCnt = (byteArr.Length - 1) - i;

                for (int s = 0; s < shiftCnt; s++)
                {
                    convByte = convByte << 8;
                }

                res = res | convByte;
            }


            return res;
        }

        //0x0001 -> 1
        public static uint HexToUInt(string hexStr)
        {
            uint res = 0;

            hexStr = hexStr.ToLower();

            if (hexStr.IndexOf("0x") == 0)
                hexStr = hexStr.Substring(2);

            if (hexStr.Length == 0)
                throw new Exception("Can't convert from hex to uint -> " + hexStr);

            if ((hexStr.Length % 2) == 1)
                throw new Exception("Can't convert from hex to uint -> " + hexStr);


            byte[] byteArr = HexToBytes(hexStr);

            for (int i = 0; i < byteArr.Length; i++)
            {
                uint convByte = (uint)byteArr[i];
                int shiftCnt = (byteArr.Length - 1) - i;

                for (int s = 0; s < shiftCnt; s++)
                {
                    convByte = convByte << 8;
                }

                res = res | convByte;
            }


            return res;
        }


        public static byte[] HexStringToBytes(string hexStr)
        {
            string[] hexArr = hexStr.Split(new char[] { ' ', '\t', '\r' });

            byte[] res = new byte[hexArr.Length];

            for (int i = 0; i < hexArr.Length; i++)
            {
                res[i] = HexToByte(hexArr[i]);
            }

            return res;
        }

        public static short[] HexStringToShorts(string hexStr)
        {
            string[] hexArr = hexStr.Split(new char[] { ' ', '\t', '\r' });

            short[] res = new short[hexArr.Length];

            for (int i = 0; i < hexArr.Length; i++)
            {
                res[i] = HexToShort(hexArr[i]);
            }

            return res;
        }

        public static ushort[] HexStringToUShorts(string hexStr)
        {
            string[] hexArr = hexStr.Split(new char[] { ' ', '\t', '\r' });

            ushort[] res = new ushort[hexArr.Length];

            for (int i = 0; i < hexArr.Length; i++)
            {
                res[i] = HexToUShort(hexArr[i]);
            }

            return res;
        }

        public static int[] HexStringToInts(string hexStr)
        {
            string[] hexArr = hexStr.Split(new char[] { ' ', '\t', '\r' });

            int[] res = new int[hexArr.Length];

            for (int i = 0; i < hexArr.Length; i++)
            {
                res[i] = HexToInt(hexArr[i]);
            }

            return res;
        }


        public static uint[] HexStringToUInts(string hexStr)
        {
            string[] hexArr = hexStr.Split(new char[] { ' ', '\t', '\r' });

            uint[] res = new uint[hexArr.Length];

            for (int i = 0; i < hexArr.Length; i++)
            {
                res[i] = HexToUInt(hexArr[i]);
            }

            return res;
        }


        public static string DigitToHex(int d)
        {
            switch (d)
            {
                case 0:
                    return "0";
                case 1:
                    return "1";
                case 2:
                    return "2";
                case 3:
                    return "3";
                case 4:
                    return "4";
                case 5:
                    return "5";
                case 6:
                    return "6";
                case 7:
                    return "7";
                case 8:
                    return "8";
                case 9:
                    return "9";
                case 10:
                    return "A";
                case 11:
                    return "B";
                case 12:
                    return "C";
                case 13:
                    return "D";
                case 14:
                    return "E";
                case 15:
                    return "F";
                default:
                    throw new Exception("Can't convert to hex digit -> " + d.ToString());
            }
        }

        private static string ByteToHex_Internal(byte value)
        {
            int d1 = value / 16;
            int d2 = value % 16;

            return DigitToHex(d1) + DigitToHex(d2);
        }

        public static string ByteToHex(byte value)
        {
            return "0x" + ByteToHex_Internal(value);
        }

        
        public static string ShortToHex(short value)
        {
            byte[] data = ShortToBytes(value);

            string h1 = ByteToHex_Internal(data[1]);
            string h2 = ByteToHex_Internal(data[0]);

            return "0x" + h1 + h2;
        }

        public static string UShortToHex(ushort value)
        {
            byte[] data = UShortToBytes(value);

            string h1 = ByteToHex_Internal(data[1]);
            string h2 = ByteToHex_Internal(data[0]);

            return "0x" + h1 + h2;
        }

        public static string IntToHex(int value)
        {
            byte[] data = IntToBytes(value);

            string h1 = ByteToHex_Internal(data[1]);
            string h2 = ByteToHex_Internal(data[0]);
            string h3 = ByteToHex_Internal(data[3]);
            string h4 = ByteToHex_Internal(data[2]);

            return "0x" + h1 + h2 + h3 + h4;

        }

        public static string UIntToHex(uint value)
        {
            byte[] data = UIntToBytes(value);

            string h1 = ByteToHex_Internal(data[1]);
            string h2 = ByteToHex_Internal(data[0]);
            string h3 = ByteToHex_Internal(data[3]);
            string h4 = ByteToHex_Internal(data[2]);

            return "0x" + h1 + h2 + h3 + h4;

        }


        public static string StringToHex(string txt)
        {
            string resultHex = "";
            byte[] arr_byteStr = Encoding.UTF8.GetBytes(txt);


            foreach (byte byteStr in arr_byteStr)
            {
                resultHex += ByteToHex(byteStr) + " ";
            }
            return resultHex;
        }

        public static string BytesToHex(byte[] arr_byteStr)
        {
            return BytesToHex(arr_byteStr, arr_byteStr.Length);
        }

        public static string BytesToHex(byte[] arr_byteStr, int count)
        {
            string resultHex = "";

            for (int i = 0; i < count; i++)
            {
                byte byteStr = arr_byteStr[i];

                resultHex += ByteToHex(byteStr) + " ";
            }
            return resultHex;
        }

        public static string ShortsToHex(short[] arr_byteStr)
        {
            return ShortsToHex(arr_byteStr, arr_byteStr.Length);
        }

        public static string ShortsToHex(short[] arr_byteStr, int count)
        {
            string resultHex = "";

            for (int i = 0; i < count; i++)
            {
                short byteStr = arr_byteStr[i];

                resultHex += ShortToHex(byteStr) + " ";
            }
            return resultHex;
        }


        public static string UShortsToHex(ushort[] arr_byteStr)
        {
            return UShortsToHex(arr_byteStr, arr_byteStr.Length);
        }

        public static string UShortsToHex(ushort[] arr_byteStr, int count)
        {
            string resultHex = "";

            for (int i = 0; i < count; i++)
            {
                ushort byteStr = arr_byteStr[i];

                resultHex += UShortToHex(byteStr) + " ";
            }
            return resultHex;
        }

        public static string IntsToHex(int[] arr_byteStr)
        {
            return IntsToHex(arr_byteStr, arr_byteStr.Length);
        }

        public static string IntsToHex(int[] arr_byteStr, int count)
        {
            string resultHex = "";

            for (int i = 0; i < count; i++)
            {
                int byteStr = arr_byteStr[i];

                resultHex += IntToHex(byteStr) + " ";
            }
            return resultHex;
        }


        public static string UIntsToHex(uint[] arr_byteStr)
        {
            return UIntsToHex(arr_byteStr, arr_byteStr.Length);
        }


        public static string UIntsToHex(uint[] arr_byteStr, int count)
        {
            string resultHex = "";

            for (int i = 0; i < count; i++)
            {
                uint byteStr = arr_byteStr[i];

                resultHex += UIntToHex(byteStr) + " ";
            }
            return resultHex;
        }


        public static byte[] ShortToBytes(short data)
        {
            byte[] arr = new byte[2];

            arr[0] = (byte)(data % 256);
            arr[1] = (byte)(data / 256);

            return arr;
        }


        public static byte[] UShortToBytes(ushort data)
        {
            byte[] arr = new byte[2];

            arr[0] = (byte)(data % 256);
            arr[1] = (byte)(data / 256);

            return arr;
        }

        public static byte[] ShortsToBytes(short[] data)
        {
            int bytes_length = data.Length * 2;
            byte[] buff = new byte[bytes_length];

            for (int i = 0; i < data.Length; i++)
            {
                byte[] arr = ShortToBytes(data[i]);

                int newInd = i * 2;
                buff[newInd] = arr[0];
                buff[newInd + 1] = arr[1];
            }

            return buff;
        }

        public static byte[] UShortsToBytes(ushort[] data)
        {
            int bytes_length = data.Length * 2;
            byte[] buff = new byte[bytes_length];

            for (int i = 0; i < data.Length; i++)
            {
                byte[] arr = UShortToBytes(data[i]);

                int newInd = i * 2;
                buff[newInd] = arr[0];
                buff[newInd + 1] = arr[1];
            }

            return buff;
        }


        public static byte[] IntToBytes(int data)
        {
            byte[] arr = new byte[4];

            int a3 = data / 16777216;
            int a3_mod = data % 16777216;

            int a2 = a3_mod / 65536;
            int a2_mod = a3_mod % 65536;

            int a1 = a2_mod / 256;
            int a1_mod = a2_mod % 256;

            int a0 = a1_mod;

            arr[0] = (byte)(a0);
            arr[1] = (byte)(a1);
            arr[2] = (byte)(a2);
            arr[3] = (byte)(a3);

            return arr;
        }

        public static byte[] UIntToBytes(uint data)
        {
            byte[] arr = new byte[4];

            uint a3 = data / 16777216;
            uint a3_mod = data % 16777216;

            uint a2 = a3_mod / 65536;
            uint a2_mod = a3_mod % 65536;

            uint a1 = a2_mod / 256;
            uint a1_mod = a2_mod % 256;

            uint a0 = a1_mod;

            arr[0] = (byte)(a0);
            arr[1] = (byte)(a1);
            arr[2] = (byte)(a2);
            arr[3] = (byte)(a3);

            return arr;
        }


        public static byte[] IntsToBytes(int[] data)
        {
            int bytes_length = data.Length * 2;
            byte[] buff = new byte[bytes_length];

            for (int i = 0; i < data.Length; i++)
            {
                byte[] arr = IntToBytes(data[i]);

                int newInd = i * 2;
                buff[newInd] = arr[0];
                buff[newInd + 1] = arr[1];
            }

            return buff;
        }

        public static byte[] UIntsToBytes(uint[] data)
        {
            int bytes_length = data.Length * 2;
            byte[] buff = new byte[bytes_length];

            for (int i = 0; i < data.Length; i++)
            {
                byte[] arr = UIntToBytes(data[i]);

                int newInd = i * 2;
                buff[newInd] = arr[0];
                buff[newInd + 1] = arr[1];
            }

            return buff;
        }


        public static short BytesToShort(byte[] byteArr)
        {
            short res = (short)BytesToInt(byteArr);

            return res;
        }

        public static short[] BytesToShorts(byte[] byteArr)
        {
            short[] res = null;

            if ((byteArr.Length % 2) == 0)
            {
                res = new short[byteArr.Length / 2];

                for (int i = 0; i < (byteArr.Length / 2); i++)
                {
                    byte[] bytes = new byte[2];

                    int s = i * 2;
                    bytes[0] = byteArr[s];
                    bytes[1] = byteArr[s + 1];

                    res[i] = BytesToShort(bytes);
                }
            }
            else
            {
                throw new Exception("Can't convert from byte[] to short[] -> length of byte[] is not even.");
            }

            return res;
        }


        public static ushort BytesToUShort(byte[] byteArr)
        {
            ushort res = (ushort)BytesToUInt(byteArr);

            return res;
        }


        public static ushort[] BytesToUShorts(byte[] byteArr)
        {
            ushort[] res = null;

            if ((byteArr.Length % 2) == 0)
            {
                res = new ushort[byteArr.Length / 2];

                for (int i = 0; i < (byteArr.Length / 2); i++)
                {
                    byte[] bytes = new byte[2];

                    int s = i * 2;
                    bytes[0] = byteArr[s];
                    bytes[1] = byteArr[s + 1];

                    res[i] = BytesToUShort(bytes);
                }
            }
            else
            {
                throw new Exception("Can't convert from byte[] to ushort[] -> length of byte[] is not even.");
            }

            return res;
        }


        public static int BytesToInt(byte[] byteArr)
        {
            int res = 0;
 
            for (int i = 0; i < byteArr.Length; i++)
            {
                int convByte = (int)byteArr[i];
                int shiftCnt = i;

                for (int s = 0; s < shiftCnt; s++)
                {
                    convByte = convByte << 8;
                }

                res = res | convByte;
            }

            return res;
        }


        public static int[] BytesToInts(byte[] byteArr)
        {
            int[] res = null;

            if ((byteArr.Length % 4) == 0)
            {
                res = new int[byteArr.Length / 4];

                for (int i = 0; i < (byteArr.Length / 4); i++)
                {
                    byte[] bytes = new byte[4];

                    int s = i * 4;
                    bytes[0] = byteArr[s];
                    bytes[1] = byteArr[s + 1];
                    bytes[2] = byteArr[s + 2];
                    bytes[3] = byteArr[s + 3];

                    res[i] = BytesToInt(bytes);
                }
            }
            else
            {
                throw new Exception("Can't convert from byte[] to int[] -> length of byte[] should be 4 * x");
            }

            return res;
        }


        public static uint BytesToUInt(byte[] byteArr)
        {
            uint res = 0;

            for (int i = 0; i < byteArr.Length; i++)
            {
                uint convByte = (uint)byteArr[i];
                int shiftCnt = i;

                for (int s = 0; s < shiftCnt; s++)
                {
                    convByte = convByte << 8;
                }

                res = res | convByte;
            }

            return res;
        }


        public static uint[] BytesToUInts(byte[] byteArr)
        {
            uint[] res = null;

            if ((byteArr.Length % 4) == 0)
            {
                res = new uint[byteArr.Length / 4];

                for (int i = 0; i < (byteArr.Length / 4); i++)
                {
                    byte[] bytes = new byte[4];

                    int s = i * 4;
                    bytes[0] = byteArr[s];
                    bytes[1] = byteArr[s + 1];
                    bytes[2] = byteArr[s + 2];
                    bytes[3] = byteArr[s + 3];

                    res[i] = BytesToUInt(bytes);
                }
            }
            else
            {
                throw new Exception("Can't convert from byte[] to uint[] -> length of byte[] should be 4 * x");
            }

            return res;
        }


        public static byte[] GetBytes(string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }


        public static char[] GetChars(byte[] data)
        {
            return Encoding.UTF8.GetChars(data);
        }

        public static string GetString(byte[] data)
        {
            return new string(Encoding.UTF8.GetChars(data));
        }

        public static string GetString(char[] data)
        {
            return new string(data);
        }


        public static string Replace(string str, char src, char target)
        {
            if (str == string.Empty)
                return string.Empty;

            string res = string.Empty;

            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == src)
                    res += target;
                else
                    res += str[i];
            }

            return res;
        }

        public static string Replace(string str, string src, string target)
        {
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


        public static bool ToBool(object o)
        {
            bool res = false;

            string typeName = o.GetType().Name;

            if (typeName.IndexOf("System.") == 0)
                typeName = typeName.Substring(7);

            if (typeName == "String")
            {
                string boolStr = o.ToString().ToLower();

                if (boolStr == "true")
                    res = true;
                else if (boolStr == "false")
                    res = false;
                else
                    throw new Exception("Can't convert to bool -> " + o.ToString());  
            }
            else
            {
                switch (typeName)
                {
                    case "Boolean":
                        {
                            Boolean d = (Boolean)o;

                            res = d;

                            break;
                        }

                    default:
                        throw new Exception("Can't convert to bool -> " + o.ToString());  
                }
            }

            return res;
        }

        public static float ToFloat(object o)
        {
            float res = 0;

            string typeName = o.GetType().Name;

            if (typeName.IndexOf("System.") == 0)
                typeName = typeName.Substring(7);

            if (typeName == "String")
            {
                Double d = Convert.ToDouble(o.ToString());
                res = (float)d;
            }
            else
            {
                switch (typeName)
                {
                    case "Double":
                        {
                            Double d = (Double)o;
                            res = (float)d;
                            break;
                        }

                    case "Single":
                        {
                            Single d = (Single)o;
                            res = (float)d;
                            break;
                        }

                    case "Int64":
                        {
                            Int64 d = (Int64)o;
                            res = (float)d;
                            break;
                        }

                    case "Int32":
                        {
                            Int32 d = (Int32)o;
                            res = (float)d;
                            break;
                        }

                    case "Int16":
                        {
                            Int16 d = (Int16)o;
                            res = (float)d;
                            break;
                        }

                    case "UInt64":
                        {
                            UInt64 d = (UInt64)o;
                            res = (float)d;
                            break;
                        }

                    case "UInt32":
                        {
                            UInt32 d = (UInt32)o;
                            res = (float)d;
                            break;
                        }

                    case "UInt16":
                        {
                            UInt16 d = (UInt16)o;
                            res = (float)d;
                            break;
                        }

                    case "Byte":
                        {
                            Byte d = (Byte)o;
                            res = (float)d;
                            break;
                        }

                    case "SByte":
                        {
                            SByte d = (SByte)o;
                            res = (float)d;
                            break;
                        }

                    case "Char":
                        {
                            Char d = (Char)o;
                            res = (float)d;
                            break;
                        }

                    case "Boolean":
                        {
                            Boolean d = (Boolean)o;

                            if (d)
                                res = 1;
                            else
                                res = 0;

                            break;
                        }

                    default:
                        throw new Exception("Can't convert to float -> " + o.ToString());
                }
            }

            return res;
        }


        public static double ToDouble(object o)
        {
            double res = 0;

            string typeName = o.GetType().Name;

            if (typeName.IndexOf("System.") == 0)
                typeName = typeName.Substring(7);

            if (typeName == "String")
            {
                Double d = Convert.ToDouble(o.ToString());
                res = d;
            }
            else
            {
                switch (typeName)
                {
                    case "Double":
                        {
                            Double d = (Double)o;
                            res = (Double)d;
                            break;
                        }

                    case "Single":
                        {
                            Single d = (Single)o;
                            res = (Double)d;
                            break;
                        }

                    case "Int64":
                        {
                            Int64 d = (Int64)o;
                            res = (Double)d;
                            break;
                        }

                    case "Int32":
                        {
                            Int32 d = (Int32)o;
                            res = (Double)d;
                            break;
                        }

                    case "Int16":
                        {
                            Int16 d = (Int16)o;
                            res = (Double)d;
                            break;
                        }

                    case "UInt64":
                        {
                            UInt64 d = (UInt64)o;
                            res = (Double)d;
                            break;
                        }

                    case "UInt32":
                        {
                            UInt32 d = (UInt32)o;
                            res = (Double)d;
                            break;
                        }

                    case "UInt16":
                        {
                            UInt16 d = (UInt16)o;
                            res = (Double)d;
                            break;
                        }

                    case "Byte":
                        {
                            Byte d = (Byte)o;
                            res = (Double)d;
                            break;
                        }

                    case "SByte":
                        {
                            SByte d = (SByte)o;
                            res = (Double)d;
                            break;
                        }

                    case "Char":
                        {
                            Char d = (Char)o;
                            res = (Double)d;
                            break;
                        }

                    case "Boolean":
                        {
                            Boolean d = (Boolean)o;

                            if (d)
                                res = 1;
                            else
                                res = 0;

                            break;
                        }

                    default:
                        throw new Exception("Can't convert to double -> " + o.ToString());  
                }
            }

            return res;
        }


        public static byte ToByte(object o)
        {
            byte res = 0;

            string typeName = o.GetType().Name;

            if (typeName.IndexOf("System.") == 0)
                typeName = typeName.Substring(7);

            if (typeName == "String")
            {
                Double d = Convert.ToDouble(o.ToString());
                res = (byte)d;
            }
            else
            {
                switch (typeName)
                {
                    case "Double":
                        {
                            Double d = (Double)o;
                            res = (byte)d;
                            break;
                        }

                    case "Single":
                        {
                            Single d = (Single)o;
                            res = (byte)d;
                            break;
                        }

                    case "Int64":
                        {
                            Int64 d = (Int64)o;
                            res = (byte)d;
                            break;
                        }

                    case "Int32":
                        {
                            Int32 d = (Int32)o;
                            res = (byte)d;
                            break;
                        }

                    case "Int16":
                        {
                            Int16 d = (Int16)o;
                            res = (byte)d;
                            break;
                        }

                    case "UInt64":
                        {
                            UInt64 d = (UInt64)o;
                            res = (byte)d;
                            break;
                        }

                    case "UInt32":
                        {
                            UInt32 d = (UInt32)o;
                            res = (byte)d;
                            break;
                        }

                    case "UInt16":
                        {
                            UInt16 d = (UInt16)o;
                            res = (byte)d;
                            break;
                        }

                    case "Byte":
                        {
                            Byte d = (Byte)o;
                            res = (byte)d;
                            break;
                        }

                    case "SByte":
                        {
                            SByte d = (SByte)o;
                            res = (byte)d;
                            break;
                        }

                    case "Char":
                        {
                            Char d = (Char)o;
                            res = (byte)d;
                            break;
                        }

                    case "Boolean":
                        {
                            Boolean d = (Boolean)o;

                            if (d)
                                res = 1;
                            else
                                res = 0;

                            break;
                        }
                    default:
                        throw new Exception("Can't convert to byte -> " + o.ToString());
                }
            }

            return res;
        }


        public static sbyte ToSByte(object o)
        {
            sbyte res = 0;

            string typeName = o.GetType().Name;

            if (typeName.IndexOf("System.") == 0)
                typeName = typeName.Substring(7);

            if (typeName == "String")
            {
                Double d = Convert.ToDouble(o.ToString());
                res = (sbyte)d;
            }
            else
            {
                switch (typeName)
                {
                    case "Double":
                        {
                            Double d = (Double)o;
                            res = (sbyte)d;
                            break;
                        }

                    case "Single":
                        {
                            Single d = (Single)o;
                            res = (sbyte)d;
                            break;
                        }

                    case "Int64":
                        {
                            Int64 d = (Int64)o;
                            res = (sbyte)d;
                            break;
                        }

                    case "Int32":
                        {
                            Int32 d = (Int32)o;
                            res = (sbyte)d;
                            break;
                        }

                    case "Int16":
                        {
                            Int16 d = (Int16)o;
                            res = (sbyte)d;
                            break;
                        }

                    case "UInt64":
                        {
                            UInt64 d = (UInt64)o;
                            res = (sbyte)d;
                            break;
                        }

                    case "UInt32":
                        {
                            UInt32 d = (UInt32)o;
                            res = (sbyte)d;
                            break;
                        }

                    case "UInt16":
                        {
                            UInt16 d = (UInt16)o;
                            res = (sbyte)d;
                            break;
                        }

                    case "Byte":
                        {
                            Byte d = (Byte)o;
                            res = (sbyte)d;
                            break;
                        }

                    case "SByte":
                        {
                            SByte d = (SByte)o;
                            res = (sbyte)d;
                            break;
                        }

                    case "Char":
                        {
                            Char d = (Char)o;
                            res = (sbyte)d;
                            break;
                        }

                    case "Boolean":
                        {
                            Boolean d = (Boolean)o;

                            if (d)
                                res = 1;
                            else
                                res = 0;

                            break;
                        }
                    default:
                        throw new Exception("Can't convert to byte -> " + o.ToString());
                }
            }

            return res;
        }



        public static Int16 ToShort(object o)
        {
            Int16 res = 0;

            string typeName = o.GetType().Name;

            if (typeName.IndexOf("System.") == 0)
                typeName = typeName.Substring(7);

            if (typeName == "String")
            {
                Double d = Convert.ToDouble(o.ToString());
                res = (Int16)d;
            }
            else
            {
                switch (typeName)
                {
                    case "Double":
                        {
                            Double d = (Double)o;
                            res = (Int16)d;
                            break;
                        }

                    case "Single":
                        {
                            Single d = (Single)o;
                            res = (Int16)d;
                            break;
                        }

                    case "Int64":
                        {
                            Int64 d = (Int64)o;
                            res = (Int16)d;
                            break;
                        }

                    case "Int32":
                        {
                            Int32 d = (Int32)o;
                            res = (Int16)d;
                            break;
                        }

                    case "Int16":
                        {
                            Int16 d = (Int16)o;
                            res = (Int16)d;
                            break;
                        }

                    case "UInt64":
                        {
                            UInt64 d = (UInt64)o;
                            res = (Int16)d;
                            break;
                        }

                    case "UInt32":
                        {
                            UInt32 d = (UInt32)o;
                            res = (Int16)d;
                            break;
                        }

                    case "UInt16":
                        {
                            UInt16 d = (UInt16)o;
                            res = (Int16)d;
                            break;
                        }

                    case "Byte":
                        {
                            Byte d = (Byte)o;
                            res = (Int16)d;
                            break;
                        }

                    case "SByte":
                        {
                            SByte d = (SByte)o;
                            res = (Int16)d;
                            break;
                        }

                    case "Char":
                        {
                            Char d = (Char)o;
                            res = (Int16)d;
                            break;
                        }

                    case "Boolean":
                        {
                            Boolean d = (Boolean)o;

                            if (d)
                                res = 1;
                            else
                                res = 0;

                            break;
                        }
                    default:
                        throw new Exception("Can't convert to Int16 -> " + o.ToString());
                }
            }

            return res;
        }



        public static int ToInt(object o)
        {
            Int32 res = 0;

            string typeName = o.GetType().Name;

            if (typeName.IndexOf("System.") == 0)
                typeName = typeName.Substring(7);

            if (typeName == "String")
            {
                Double d = Convert.ToDouble(o.ToString());
                res = (Int32)d;
            }
            else
            {
                switch (typeName)
                {
                    case "Double":
                        {
                            Double d = (Double)o;
                            res = (Int32)d;
                            break;
                        }

                    case "Single":
                        {
                            Single d = (Single)o;
                            res = (Int32)d;
                            break;
                        }

                    case "Int64":
                        {
                            Int64 d = (Int64)o;
                            res = (Int32)d;
                            break;
                        }

                    case "Int32":
                        {
                            Int32 d = (Int32)o;
                            res = (Int32)d;
                            break;
                        }

                    case "Int16":
                        {
                            Int16 d = (Int16)o;
                            res = (Int32)d;
                            break;
                        }

                    case "UInt64":
                        {
                            UInt64 d = (UInt64)o;
                            res = (Int32)d;
                            break;
                        }

                    case "UInt32":
                        {
                            UInt32 d = (UInt32)o;
                            res = (Int32)d;
                            break;
                        }

                    case "UInt16":
                        {
                            UInt16 d = (UInt16)o;
                            res = (Int32)d;
                            break;
                        }

                    case "Byte":
                        {
                            Byte d = (Byte)o;
                            res = (Int32)d;
                            break;
                        }

                    case "SByte":
                        {
                            SByte d = (SByte)o;
                            res = (Int32)d;
                            break;
                        }

                    case "Char":
                        {
                            Char d = (Char)o;
                            res = (Int32)d;
                            break;
                        }

                    case "Boolean":
                        {
                            Boolean d = (Boolean)o;

                            if (d)
                                res = 1;
                            else
                                res = 0;

                            break;
                        }

                    default:
                        throw new Exception("Can't convert to Int32 -> " + o.ToString());  
                }
            }

            return res;
        }


        public static Int64 ToLong(object o)
        {
            Int64 res = 0;

            string typeName = o.GetType().Name;

            if (typeName.IndexOf("System.") == 0)
                typeName = typeName.Substring(7);

            if (typeName == "String")
            {
                Double d = Convert.ToDouble(o.ToString());
                res = (Int64)d;
            }
            else
            {
                switch (typeName)
                {
                    case "Double":
                        {
                            Double d = (Double)o;
                            res = (Int64)d;
                            break;
                        }

                    case "Single":
                        {
                            Single d = (Single)o;
                            res = (Int64)d;
                            break;
                        }

                    case "Int64":
                        {
                            Int64 d = (Int64)o;
                            res = (Int64)d;
                            break;
                        }

                    case "Int32":
                        {
                            Int32 d = (Int32)o;
                            res = (Int64)d;
                            break;
                        }

                    case "Int16":
                        {
                            Int16 d = (Int16)o;
                            res = (Int64)d;
                            break;
                        }

                    case "UInt64":
                        {
                            UInt64 d = (UInt64)o;
                            res = (Int64)d;
                            break;
                        }

                    case "UInt32":
                        {
                            UInt32 d = (UInt32)o;
                            res = (Int64)d;
                            break;
                        }

                    case "UInt16":
                        {
                            UInt16 d = (UInt16)o;
                            res = (Int64)d;
                            break;
                        }

                    case "Byte":
                        {
                            Byte d = (Byte)o;
                            res = (Int64)d;
                            break;
                        }

                    case "SByte":
                        {
                            SByte d = (SByte)o;
                            res = (Int64)d;
                            break;
                        }

                    case "Char":
                        {
                            Char d = (Char)o;
                            res = (Int64)d;
                            break;
                        }

                    case "Boolean":
                        {
                            Boolean d = (Boolean)o;

                            if (d)
                                res = 1;
                            else
                                res = 0;

                            break;
                        }

                    default:
                        throw new Exception("Can't convert to Int64 -> " + o.ToString());
                }
            }

            return res;
        }



        public static UInt16 ToUShort(object o)
        {
            UInt16 res = 0;

            string typeName = o.GetType().Name;

            if (typeName.IndexOf("System.") == 0)
                typeName = typeName.Substring(7);

            if (typeName == "String")
            {
                Double d = Convert.ToDouble(o.ToString());
                res = (UInt16)d;
            }
            else
            {
                switch (typeName)
                {
                    case "Double":
                        {
                            Double d = (Double)o;
                            res = (UInt16)d;
                            break;
                        }

                    case "Single":
                        {
                            Single d = (Single)o;
                            res = (UInt16)d;
                            break;
                        }

                    case "Int64":
                        {
                            Int64 d = (Int64)o;
                            res = (UInt16)d;
                            break;
                        }

                    case "Int32":
                        {
                            Int32 d = (Int32)o;
                            res = (UInt16)d;
                            break;
                        }

                    case "Int16":
                        {
                            Int16 d = (Int16)o;
                            res = (UInt16)d;
                            break;
                        }

                    case "UInt64":
                        {
                            UInt64 d = (UInt64)o;
                            res = (UInt16)d;
                            break;
                        }

                    case "UInt32":
                        {
                            UInt32 d = (UInt32)o;
                            res = (UInt16)d;
                            break;
                        }

                    case "UInt16":
                        {
                            UInt16 d = (UInt16)o;
                            res = (UInt16)d;
                            break;
                        }

                    case "Byte":
                        {
                            Byte d = (Byte)o;
                            res = (UInt16)d;
                            break;
                        }

                    case "SByte":
                        {
                            SByte d = (SByte)o;
                            res = (UInt16)d;
                            break;
                        }

                    case "Char":
                        {
                            Char d = (Char)o;
                            res = (UInt16)d;
                            break;
                        }

                    case "Boolean":
                        {
                            Boolean d = (Boolean)o;

                            if (d)
                                res = 1;
                            else
                                res = 0;

                            break;
                        }
                    default:
                        throw new Exception("Can't convert to UInt16 -> " + o.ToString());
                }
            }

            return res;
        }


        public static UInt32 ToUInt(object o)
        {
            UInt32 res = 0;

            string typeName = o.GetType().Name;

            if (typeName.IndexOf("System.") == 0)
                typeName = typeName.Substring(7);

            if (typeName == "String")
            {
                Double d = Convert.ToDouble(o.ToString());
                res = (UInt32)d;
            }
            else
            {
                switch (typeName)
                {
                    case "Double":
                        {
                            Double d = (Double)o;
                            res = (UInt32)d;
                            break;
                        }

                    case "Single":
                        {
                            Single d = (Single)o;
                            res = (UInt32)d;
                            break;
                        }

                    case "Int64":
                        {
                            Int64 d = (Int64)o;
                            res = (UInt32)d;
                            break;
                        }

                    case "Int32":
                        {
                            Int32 d = (Int32)o;
                            res = (UInt32)d;
                            break;
                        }

                    case "Int16":
                        {
                            Int16 d = (Int16)o;
                            res = (UInt32)d;
                            break;
                        }

                    case "UInt64":
                        {
                            UInt64 d = (UInt64)o;
                            res = (UInt32)d;
                            break;
                        }

                    case "UInt32":
                        {
                            UInt32 d = (UInt32)o;
                            res = (UInt32)d;
                            break;
                        }

                    case "UInt16":
                        {
                            UInt16 d = (UInt16)o;
                            res = (UInt32)d;
                            break;
                        }

                    case "Byte":
                        {
                            Byte d = (Byte)o;
                            res = (UInt32)d;
                            break;
                        }

                    case "SByte":
                        {
                            SByte d = (SByte)o;
                            res = (UInt32)d;
                            break;
                        }

                    case "Char":
                        {
                            Char d = (Char)o;
                            res = (UInt32)d;
                            break;
                        }

                    case "Boolean":
                        {
                            Boolean d = (Boolean)o;

                            if (d)
                                res = 1;
                            else
                                res = 0;

                            break;
                        }

                    default:
                        throw new Exception("Can't convert to UInt32 -> " + o.ToString());
                }
            }

            return res;
        }



        public static UInt64 ToULong(object o)
        {
            UInt64 res = 0;

            string typeName = o.GetType().Name;

            if (typeName.IndexOf("System.") == 0)
                typeName = typeName.Substring(7);

            if (typeName == "String")
            {
                Double d = Convert.ToDouble(o.ToString());
                res = (UInt64)d;
            }
            else
            {
                switch (typeName)
                {
                    case "Double":
                        {
                            Double d = (Double)o;
                            res = (UInt64)d;
                            break;
                        }

                    case "Single":
                        {
                            Single d = (Single)o;
                            res = (UInt64)d;
                            break;
                        }

                    case "Int64":
                        {
                            Int64 d = (Int64)o;
                            res = (UInt64)d;
                            break;
                        }

                    case "Int32":
                        {
                            Int32 d = (Int32)o;
                            res = (UInt64)d;
                            break;
                        }

                    case "Int16":
                        {
                            Int16 d = (Int16)o;
                            res = (UInt64)d;
                            break;
                        }

                    case "UInt64":
                        {
                            UInt64 d = (UInt64)o;
                            res = (UInt64)d;
                            break;
                        }

                    case "UInt32":
                        {
                            UInt32 d = (UInt32)o;
                            res = (UInt64)d;
                            break;
                        }

                    case "UInt16":
                        {
                            UInt16 d = (UInt16)o;
                            res = (UInt64)d;
                            break;
                        }

                    case "Byte":
                        {
                            Byte d = (Byte)o;
                            res = (UInt64)d;
                            break;
                        }

                    case "SByte":
                        {
                            SByte d = (SByte)o;
                            res = (UInt64)d;
                            break;
                        }

                    case "Char":
                        {
                            Char d = (Char)o;
                            res = (UInt64)d;
                            break;
                        }

                    case "Boolean":
                        {
                            Boolean d = (Boolean)o;

                            if (d)
                                res = 1;
                            else
                                res = 0;

                            break;
                        }

                    default:
                        throw new Exception("Can't convert to Int64 -> " + o.ToString());
                }
            }

            return res;
        }



        public static char ToChar(object o)
        {
            char res = '\0';

            string typeName = o.GetType().Name;

            if (typeName.IndexOf("System.") == 0)
                typeName = typeName.Substring(7);

            if (typeName == "String")
            {
                Double d = Convert.ToDouble(o.ToString());
                res = (char)d;
            }
            else
            {
                switch (typeName)
                {
                    case "Double":
                        {
                            Double d = (Double)o;
                            res = (char)d;
                            break;
                        }

                    case "Single":
                        {
                            Single d = (Single)o;
                            res = (char)d;
                            break;
                        }

                    case "Int64":
                        {
                            Int64 d = (Int64)o;
                            res = (char)d;
                            break;
                        }

                    case "Int32":
                        {
                            Int32 d = (Int32)o;
                            res = (char)d;
                            break;
                        }

                    case "Int16":
                        {
                            Int16 d = (Int16)o;
                            res = (char)d;
                            break;
                        }

                    case "UInt64":
                        {
                            UInt64 d = (UInt64)o;
                            res = (char)d;
                            break;
                        }

                    case "UInt32":
                        {
                            UInt32 d = (UInt32)o;
                            res = (char)d;
                            break;
                        }

                    case "UInt16":
                        {
                            UInt16 d = (UInt16)o;
                            res = (char)d;
                            break;
                        }

                    case "Byte":
                        {
                            Byte d = (Byte)o;
                            res = (char)d;
                            break;
                        }

                    case "SByte":
                        {
                            SByte d = (SByte)o;
                            res = (char)d;
                            break;
                        }

                    case "Char":
                        {
                            Char d = (Char)o;
                            res = (char)d;
                            break;
                        }

                    case "Boolean":
                        {
                            Boolean d = (Boolean)o;

                            if (d)
                                res = '1';
                            else
                                res = '0';

                            break;
                        }

                    default:
                        throw new Exception("Can't convert to char -> " + o.ToString());
                }
            }

            return res;
        }


        public static DateTime ToDateTime(object o)
        {
            return (DateTime)o;
        }



       
        public static double ToRadians(double deg)
        {
            return deg * System.Math.PI / 180.0;
        }

        public static double ToDegrees(double rad)
        {
            return rad * 180.0 / System.Math.PI;
        }


        public static short[] ConvertArrayToShort(object data)
        {
            if (data == null)
                return null;

            Array arr = (Array)data;
            short[] res = new short[arr.Length];
            for (int i = 0; i < arr.Length; i++)
                res[i] = Util.ToShort(GetTypedArrayValue(data, i));

            return res;
        }


        public static int[] ConvertArrayToInt(object data)
        {
            if (data == null)
                return null;

            Array arr = (Array)data;
            int[] res = new int[arr.Length];
            for (int i = 0; i < arr.Length; i++)
                res[i] = Util.ToInt(GetTypedArrayValue(data, i));

            return res;
        }


        public static long[] ConvertArrayToLong(object data)
        {
            if (data == null)
                return null;

            Array arr = (Array)data;
            long[] res = new long[arr.Length];
            for (int i = 0; i < arr.Length; i++)
                res[i] = Util.ToLong(GetTypedArrayValue(data, i));

            return res;
        }


        public static ushort[] ConvertArrayToUShort(object data)
        {
            if (data == null)
                return null;

            Array arr = (Array)data;
            ushort[] res = new ushort[arr.Length];
            for (int i = 0; i < arr.Length; i++)
                res[i] = Util.ToUShort(GetTypedArrayValue(data, i));

            return res;
        }


        public static uint[] ConvertArrayToUInt(object data)
        {
            if (data == null)
                return null;

            Array arr = (Array)data;
            uint[] res = new uint[arr.Length];
            for (int i = 0; i < arr.Length; i++)
                res[i] = Util.ToUInt(GetTypedArrayValue(data, i));

            return res;

        }



        public static ulong[] ConvertArrayToULong(object data)
        {
            if (data == null)
                return null;

            Array arr = (Array)data;
            ulong[] res = new ulong[arr.Length];
            for (int i = 0; i < arr.Length; i++)
                res[i] = Util.ToULong(GetTypedArrayValue(data, i));

            return res;
        }



        public static byte[] ConvertArrayToByte(object data)
        {
            if (data == null)
                return null;


            Array arr = (Array)data;
            byte[] res = new byte[arr.Length];
            for (int i = 0; i < arr.Length; i++)
                res[i] = Util.ToByte(GetTypedArrayValue(data, i));

            return res;
        }


        public static sbyte[] ConvertArrayToSByte(object data)
        {
            if (data == null)
                return null;


            Array arr = (Array)data;
            sbyte[] res = new sbyte[arr.Length];
            for (int i = 0; i < arr.Length; i++)
                res[i] = Util.ToSByte(GetTypedArrayValue(data, i));

            return res;
        }


        public static char[] ConvertArrayToChar(object data)
        {
            if (data == null)
                return null;


            Array arr = (Array)data;
            char[] res = new char[arr.Length];
            for (int i = 0; i < arr.Length; i++)
                res[i] = Util.ToChar(GetTypedArrayValue(data, i));

            return res;
        }

        public static float[] ConvertArrayToFloat(object data)
        {
            if (data == null)
                return null;


            Array arr = (Array)data;
            float[] res = new float[arr.Length];
            for (int i = 0; i < arr.Length; i++)
                res[i] = Util.ToFloat(GetTypedArrayValue(data, i));

            return res;

        }

        public static double[] ConvertArrayToDouble(object data)
        {
            if (data == null)
                return null;

            Array arr = (Array)data;
            double[] res = new double[arr.Length];
            for (int i = 0; i < arr.Length; i++)
                res[i] = Util.ToDouble(GetTypedArrayValue(data, i));

            return res;
        }

        public static DateTime[] ConvertArrayToDateTime(object data)
        {
            if (data == null)
                return null;


            Array arr = (Array)data;
            DateTime[] res = new DateTime[arr.Length];
            for (int i = 0; i < arr.Length; i++)
                res[i] = Util.ToDateTime(GetTypedArrayValue(data, i));

            return res;

        }


        public static bool[] ConvertArrayToBool(object data)
        {
            if (data == null)
                return null;

            Array arr = (Array)data;
            bool[] res = new bool[arr.Length];
            for (int i = 0; i < arr.Length; i++)
                res[i] = Util.ToBool(GetTypedArrayValue(data, i));

            return res;
        }


        public static string[] ConvertArrayToString(object data)
        {
            if (data == null)
                return null;


            Array arr = (Array)data;
            string[] res = new string[arr.Length];
            for (int i = 0; i < arr.Length; i++)
                res[i] = GetTypedArrayValue(data, i).ToString();

            return res;

        }

        public static byte[] CreateArrayByte(int size)
        {
            return new byte[size];
        }

        public static sbyte[] CreateArraySByte(int size)
        {
            return new sbyte[size];
        }

        public static char[] CreateArrayChar(int size)
        {
            return new char[size];
        }

        public static short[] CreateArrayShort(int size)
        {
            return new short[size];
        }

        public static ushort[] CreateArrayUShort(int size)
        {
            return new ushort[size];
        }

        public static int[] CreateArrayInt(int size)
        {
            return new int[size];
        }

        public static uint[] CreateArrayUInt(int size)
        {
            return new uint[size];
        }

        public static long[] CreateArrayLong(int size)
        {
            return new long[size];
        }

        public static ulong[] CreateArrayULong(int size)
        {
            return new ulong[size];
        }

        public static float[] CreateArrayFloat(int size)
        {
            return new float[size];
        }

        public static double[] CreateArrayDouble(int size)
        {
            return new double[size];
        }

        public static bool[] CreateArrayBool(int size)
        {
            return new bool[size];
        }

        public static string[] CreateArrayString(int size)
        {
            return new string[size];
        }

        public static DateTime[] CreateArrayDateTime(int size)
        {
            return new DateTime[size];
        }

        public static object[] CreateArrayObject(int size)
        {
            return new object[size];
        }


        


        public static void SetTypedArrayValue(object instance, int index, object data)
        {
            if (instance == null)
            {
                throw new Exception("[SetTypedArrayValue] Instance is null");                
            }

            string tName = instance.GetType().Name;

            if (tName.IndexOf("System.") == 0)
                tName = tName.Substring(7);

            if (tName.IndexOf("Reflection.") == 0)
                tName = tName.Substring(11);

            if (tName.IndexOf("IO.") == 0)
                tName = tName.Substring(3);

            if (tName.IndexOf("Text.") == 0)
                tName = tName.Substring(5);

            if (tName.IndexOf("Runtime.") == 0)
                tName = tName.Substring(8);

            if (tName.IndexOf("InteropServices.") == 0)
                tName = tName.Substring(16);

            if (tName.IndexOf("Collections.") == 0)
                tName = tName.Substring(12);

            if (tName.IndexOf("Windows.") == 0)
                tName = tName.Substring(8);

            if (tName.IndexOf("Forms.") == 0)
                tName = tName.Substring(6);

            if (tName.IndexOf("ComponentModel.") == 0)
                tName = tName.Substring(15);

            if (tName.IndexOf("Diagnostics.") == 0)
                tName = tName.Substring(12);

            if (tName.IndexOf("SPL.Script.Engine.") == 0)
                tName = tName.Substring(18);


            try
            {
                switch (tName)
                {
                    case "Int16[]":
                        {
                            Int16[] target = (Int16[])instance;
                            target[index] = Util.ToShort(data);
                        }
                        break;

                    case "Int32[]":
                        {
                            Int32[] target = (Int32[])instance;
                            target[index] = Util.ToInt(data);
                        }
                        break;

                    case "Int64[]":
                        {
                            Int64[] target = (Int64[])instance;
                            target[index] = Util.ToLong(data);
                        }
                        break;
                    case "UInt16[]":
                        {
                            UInt16[] target = (UInt16[])instance;
                            target[index] = Util.ToUShort(data);
                        }
                        break;

                    case "UInt32[]":
                        {
                            UInt32[] target = (UInt32[])instance;
                            target[index] = Util.ToUInt(data);
                        }
                        break;

                    case "UInt64[]":
                        {
                            UInt64[] target = (UInt64[])instance;
                            target[index] = Util.ToULong(data);
                        }
                        break;
                    case "Byte[]":
                        {
                            Byte[] target = (Byte[])instance;
                            target[index] = Util.ToByte(data);
                        }
                        break;
                    case "SByte[]":
                        {
                            SByte[] target = (SByte[])instance;
                            target[index] = Util.ToSByte(data);
                        }
                        break;

                    case "Double[]":
                        {
                            Double[] target = (Double[])instance;
                            target[index] = Util.ToDouble(data);
                        }
                        break;

                    case "Single[]":
                        {
                            Single[] target = (Single[])instance;
                            target[index] = Util.ToFloat(data);
                        }
                        break;
                    case "String[]":
                        {
                            String[] target = (String[])instance;
                            target[index] = data.ToString();
                        }
                        break;

                    case "Object[]":
                        {
                            Object[] target = (Object[])instance;
                            target[index] = data;
                        }
                        break;

                    case "DateTime[]":
                        {
                            DateTime[] target = (DateTime[])instance;
                            target[index] = Util.ToDateTime(data);
                        }
                        break;

                    default:
                        throw new Exception("[SetTypedArrayValue] " + "It's not supportable array type -> " + tName + " / " + index.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("[SetTypedArrayValue] " + ex.ToString());
            }
        }



        public static object GetTypedArrayValue(object instance, int index)
        {
            object res = null;

            if (instance == null)
            {
                throw new Exception("[GetTypedArrayValue] Instance is null");
            }

            string tName = instance.GetType().Name;

            //System.***[] 형태의 변수는 마지막 타입만 남겨둠
            if (tName.IndexOf("System.") == 0)
                tName = tName.Substring(7);

            if (tName.IndexOf("Reflection.") == 0)
                tName = tName.Substring(11);

            if (tName.IndexOf("IO.") == 0)
                tName = tName.Substring(3);

            if (tName.IndexOf("Text.") == 0)
                tName = tName.Substring(5);

            if (tName.IndexOf("Runtime.") == 0)
                tName = tName.Substring(8);

            if (tName.IndexOf("InteropServices.") == 0)
                tName = tName.Substring(16);

            if (tName.IndexOf("Collections.") == 0)
                tName = tName.Substring(12);

            if (tName.IndexOf("Windows.") == 0)
                tName = tName.Substring(8);

            if (tName.IndexOf("Forms.") == 0)
                tName = tName.Substring(6);

            if (tName.IndexOf("ComponentModel.") == 0)
                tName = tName.Substring(15);

            if (tName.IndexOf("Diagnostics.") == 0)
                tName = tName.Substring(12);

            if (tName.IndexOf("SPL.Script.Engine.") == 0)
                tName = tName.Substring(18);


            try
            {
                switch (tName)
                {
                    case "Int16[]":
                        {
                            Int16[] target = (Int16[])instance;
                            res = target[index];
                        }
                        break;

                    case "Int32[]":
                        {
                            Int32[] target = (Int32[])instance;
                            res = target[index];
                        }
                        break;

                    case "Int64[]":
                        {
                            Int64[] target = (Int64[])instance;
                            res = target[index];
                        }
                        break;
                    case "UInt16[]":
                        {
                            UInt16[] target = (UInt16[])instance;
                            res = target[index];
                        }
                        break;

                    case "UInt32[]":
                        {
                            UInt32[] target = (UInt32[])instance;
                            res = target[index];
                        }
                        break;

                    case "UInt64[]":
                        {
                            UInt64[] target = (UInt64[])instance;
                            res = target[index];
                        }
                        break;
                    case "Byte[]":
                        {
                            Byte[] target = (Byte[])instance;
                            res = target[index];
                        }
                        break;
                    case "SByte[]":
                        {
                            SByte[] target = (SByte[])instance;
                            res = target[index];
                        }
                        break;

                    case "Double[]":
                        {
                            Double[] target = (Double[])instance;
                            res = target[index];
                        }
                        break;

                    case "Single[]":
                        {
                            Single[] target = (Single[])instance;
                            res = target[index];
                        }
                        break;
                    case "String[]":
                        {
                            String[] target = (String[])instance;
                            res = target[index];
                        }
                        break;

                    case "Object[]":
                        {
                            Object[] target = (Object[])instance;
                            res = target[index];
                        }
                        break;

                    case "DateTime[]":
                        {
                            DateTime[] target = (DateTime[])instance;
                            res = target[index];
                        }
                        break;

                    default:
                        throw new Exception("[GetTypedArrayValue] " + "It's not supportable array type -> " + tName + " / " + index.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("[GetTypedArrayValue] " + ex.ToString());
            }

            return res;
        }


    }
}
