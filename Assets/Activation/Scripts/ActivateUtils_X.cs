using UnityEngine;
using System.Collections;

using System;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.IO;
using System.Collections.Specialized;


using System.Security.Cryptography;
using System.Security.AccessControl;
using System.Security.Principal;
using System.IO.Compression;

using System.Net;
using System.Net.Sockets;
using System.Threading;

public class ActivateUtils : MonoBehaviour {


	public static string GetDeviceUniqueIdentifier()
	{
		return SystemInfo.deviceUniqueIdentifier.ToString();
	}


    public static string GetLengthStr(string str, int length)
    {
        string res = "";
        string temp_str = str;

        if (string.IsNullOrEmpty(str))
            return "";

        if (temp_str.Length >= length)
            res = temp_str.Substring(0, length);
        else
        {
            while (temp_str.Length < length)
                temp_str += str;

            res = temp_str.Substring(0, length);
        }

        return res;
    }


    public static string DecodeFromHexString(string hexStr)
    {
        if (string.IsNullOrEmpty(hexStr))
            return "";

        byte[] codedData = HexToBytes(hexStr);

        if (codedData == null)
            return "";

        if (codedData != null)
            return Encoding.ASCII.GetString(codedData);
        else
            return "";
    }


    public static byte[] HexToBytes(string hexStr)
    {
        if (string.IsNullOrEmpty(hexStr))
            return null;

        byte[] res = null;

        hexStr = hexStr.ToLower();

        if (hexStr.IndexOf("0x") == 0)
            hexStr = hexStr.Substring(2);

        if (hexStr.Length == 0)
            return null;

        if ((hexStr.Length % 2) == 1)
            return null;

        res = new byte[hexStr.Length / 2];

        for (int i = 0; i < hexStr.Length; i = i + 2)
        {
            string hex = hexStr.Substring(i, 2);

            res[i / 2] = HexToByte(hex);
        }

        return res;
    }



    public static byte HexToByte(string hexStr)
    {
        int res = 0;

        hexStr = hexStr.ToLower();

        if (hexStr.IndexOf("0x") == 0)
            hexStr = hexStr.Substring(2);

        if (hexStr.Length == 0)
            throw new Exception("Can't convert from hex to byte -> " + hexStr);

        if (hexStr.Length != 2)
            throw new Exception("Can't convert from hex to byte -> " + hexStr);

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
                    throw new Exception("Can't convert from hex to byte -> " + hexStr);
            }

            if (i == 0)
                res = res + num;
            else
                res = res + (num * 16);

        }

        return (byte)res;
    }




    public static string EncodeToHexString(string inputData)
    {
        if (string.IsNullOrEmpty(inputData))
            return "";

        byte[] res = Encoding.ASCII.GetBytes(inputData);

        if (res == null)
            return "";

        return BytesToHex(res);
    }


    public static string BytesToHex(byte[] arr_byteStr)
    {
        if (arr_byteStr != null)
            return BytesToHex(arr_byteStr, arr_byteStr.Length);
        else
            return "";
    }


    public static string BytesToHex(byte[] arr_byteStr, int count)
    {
        string resultHex = "";

        for (int i = 0; i < count; i++)
        {
            byte byteStr = arr_byteStr[i];

            resultHex += ByteToHex(byteStr);
        }
        return resultHex;
    }


    private static string ByteToHex_Internal(byte value)
    {
        int d1 = value / 16;
        int d2 = value % 16;

        return DigitToHex(d1) + DigitToHex(d2);
    }

    public static string ByteToHex(byte value)
    {
        return ByteToHex_Internal(value);
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


}


