using System;
using UnityEngine;

namespace SimpleSC.Server.Controllers
{
    internal class DistributionServer : MonoBehaviour
    {
        void Start ()
        {
            print("Distribution server initialized;");
        }
        public void Connect(Socket socket)
        {
            Console.WriteLine($"Connected;");
            socket.AddListener("session_request", (object type) =>
            {
                int session_type = (int)type;
                print($"Got session_request {session_type}");
                switch (session_type)
                {
                    case 0:
                        SpawnSessionServer<AISessionServer>(socket);
                        break;
                    default:
                        break;
                }
            });
        }
        void SpawnSessionServer<T>(Socket socket) where T: SessionServer
        {
            SessionServer server = gameObject.AddComponent<T>();
            server.Connect(socket);
        }
        public void Disconnect(Socket socket)
        {
            socket.RemoveListener("session_request");
        }
        public void StopServer()
        {
            Destroy(gameObject);
        }
    }
}
