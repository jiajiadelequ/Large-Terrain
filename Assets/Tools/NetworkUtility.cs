using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using UnityEngine;

public static class NetworkUtility
{
    public static string GetRandomAssetIdStr()
    {
        string a = "abcdef";
        string b = "1234567890";
        var sb = new StringBuilder();
        for (int i = 0; i < 20; i++)
        {
            string t = "";
            if (Random.Range(0, 2) == 1)
                t = a[Random.Range(0, a.Length)].ToString();
            else
                t = b[Random.Range(0, b.Length)].ToString();
            sb.Append(t);
        }
        return sb.ToString();
    }

    public static bool PortInUse(int port)
    {
        bool inUse = false;

        IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
        IPEndPoint[] ipEndPoints = ipProperties.GetActiveUdpListeners();

        foreach (IPEndPoint endPoint in ipEndPoints)
        {
            if (endPoint.Port == port)
            {

                inUse = true;
                break;
            }
        }
        return inUse;
    }

    public static int GetPort()
    {
        int tempPort = Random.Range(12345, 54321);
        if (PortInUse(tempPort))
        {
            return GetPort();
        }
        else
        {
            return tempPort;
        }
    }
}
