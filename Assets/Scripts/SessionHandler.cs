using SocketIO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class SessionHandler : MonoBehaviour
{
    [SerializeField] SocketIOComponent socket;
    void Start()
    {
        StartCoroutine(SessionInitRoutine());
    }
    IEnumerator SessionInitRoutine()
    {
        socket.On("session_response", (SocketIOEvent ev) => { Debug.Log("abc"); });

        yield return new WaitForSeconds(0.5f);
        string session_request = JsonConvert.SerializeObject(new SessionRequest());

        socket.Emit("session_request", JSONObject.Create(session_request));
    }
}
