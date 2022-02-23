using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class callBackLogger : MonoBehaviour
{
    public static void sendMessage(string Message)
    {
        Debug.Log(Message);
    }
    public static void throwError(string Message)
    {
        Debug.LogError(Message);
    }
}