using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System.Net.WebSockets;

public class NetAdapter : MonoBehaviour
{
    [SerializeField] SocketIOComponent socketIO;
    void Start()
    {
        socketIO.On("abc", (SocketIOEvent ev) => { Debug.Log("abc"); });
        socketIO.On("abc", (SocketIOEvent ev) => { Debug.Log("def"); });
    }
}
