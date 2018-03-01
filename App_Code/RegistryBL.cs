using System;
//using System.Collections.Generic;
//using System.Web;
using Microsoft.Win32;

/// <summary>
/// Summary description for RegistryBL
/// </summary>
public class RegistryBL
{
	public RegistryBL()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public string[] getregistryuser()
    {
        RegistryKey rk = Registry.LocalMachine.OpenSubKey(
             @"SOFTWARE\Microsoft\Windows\CurrentVersion\DateTime\Servers",
             true);
        string sDefault = (String)rk.GetValue("");
        int iDefault = Convert.ToInt32(sDefault);
        //this an array of all the server names
        string[] sServers = rk.GetValueNames(); //requires enumerate sub keys
        iDefault++;
        if (iDefault >= sServers.Length)
            iDefault = 1;
        rk.SetValue("", iDefault.ToString());
        return sServers;
    }
    public void roger()
    {
        string Value;
        string strPath = "";
        RegistryKey regKeyAppRoot = Registry.CurrentUser.CreateSubKey("Software").CreateSubKey("BudgetAndForecasting").CreateSubKey(strPath);
        Value = (string)regKeyAppRoot.GetValue("Sample");
       // return Value;
    }
    public void WriteRegistry(string Key, string value)
    {
        string strPath = "";
        RegistryKey regKeyAppRoot = Registry.CurrentUser.CreateSubKey("Software").CreateSubKey("BudgetAndForecasting").CreateSubKey(strPath);
        regKeyAppRoot.SetValue(Key, value);
    }

}