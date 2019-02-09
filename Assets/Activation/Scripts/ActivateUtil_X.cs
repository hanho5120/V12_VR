using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Net.Cache;
using System.Net.Sockets;

public class ActivateUtil
{

    public static DateTime GetDateTimeFrom23Str(string dt23_str)
    {
        DateTime enteredDate = DateTime.Parse(dt23_str);
        return enteredDate;
    }


	public static string GetDateTime23Str()
	{
		DateTime nn = System.DateTime.Now;
		
		string timeStr = nn.Year.ToString() + "-" + nn.Month.ToString().PadLeft(2, '0') + "-" + nn.Day.ToString().PadLeft(2, '0');
		timeStr += " " + nn.Hour.ToString().PadLeft(2, '0') + ":" + nn.Minute.ToString().PadLeft(2, '0') + ":" + nn.Second.ToString().PadLeft(2, '0') + "." + nn.Millisecond.ToString().PadLeft(3, '0');
		
		return timeStr;
	}
	
	public static string GetDateTime23Str(System.DateTime nn)
	{
		string timeStr = nn.Year.ToString() + "-" + nn.Month.ToString().PadLeft(2, '0') + "-" + nn.Day.ToString().PadLeft(2, '0');
		timeStr += " " + nn.Hour.ToString().PadLeft(2, '0') + ":" + nn.Minute.ToString().PadLeft(2, '0') + ":" + nn.Second.ToString().PadLeft(2, '0') + "." + nn.Millisecond.ToString().PadLeft(3, '0');
		
		return timeStr;
	}
	
	public static string GetAddedDateTime23Str(int days)
	{
		DateTime nn = System.DateTime.Now;
		nn = nn.AddDays(days);
		
		string timeStr = nn.Year.ToString() + "-" + nn.Month.ToString().PadLeft(2, '0') + "-" + nn.Day.ToString().PadLeft(2, '0');
		timeStr += " " + nn.Hour.ToString().PadLeft(2, '0') + ":" + nn.Minute.ToString().PadLeft(2, '0') + ":" + nn.Second.ToString().PadLeft(2, '0') + "." + nn.Millisecond.ToString().PadLeft(3, '0');
		
		return timeStr;
	}

}
