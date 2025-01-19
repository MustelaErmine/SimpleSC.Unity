using UnityEngine;

namespace SimpleSC.Server.Controllers
{
    public class SessionServer : MonoBehaviour
    {
        public Socket Socket { get; set; }
        public virtual void NextStep()
        {

        }
        public void Connect(Socket socket)
        {
            Socket = socket;
            InitConnection();
        }
        protected virtual void InitConnection() { }
        public void StopServer()
        {
            Destroy(this);
        }
    }
}
