using SimpleSC.Server.Controllers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ServerAdapter : MonoBehaviour
{
    [SerializeField] DistributionServer server;

    public void Connect (Socket socket)
    {
        server.Connect(socket);
    }
}