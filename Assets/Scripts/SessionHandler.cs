using System.Collections;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Events;
using SimpleSC.Server.Data;

public class SessionHandler : MonoBehaviour
{
    Socket socket;
    [SerializeField] UnityEvent onGameStart, onState;
    [SerializeField] UnityEvent<bool> onGameEnd;

    [SerializeField] ServerAdapter serverAdapter;

    AISessionState currentState;
    public AISessionState CurrentState
    {
        get => currentState;
        set
        {
            currentState = value;
            onState.Invoke();
        }
    }
    void Start()
    {
        socket = gameObject.AddComponent<Socket>();
        socket.AddListener("session_response", (object state) => {
            print("Session response recieved");
            currentState = (AISessionState)state;
            onGameStart.Invoke();
            onState.Invoke();
        });
        socket.AddListener("state", (object state) => {
            print("State recieved");
            CurrentState = (AISessionState)state;
        });
        socket.AddListener("end_game", (object win) =>
        {
            print("End game recieved");
            bool result = (bool) win;
            onGameEnd.Invoke(result);
        });
        serverAdapter.Connect(socket);
        RestartGame();
    }
    public void RestartGame()
    {
        socket.Emit("session_request", 0);
    }
    public void Emit(string message, object arg)
    {
        socket.Emit(message, arg);
    }
}
