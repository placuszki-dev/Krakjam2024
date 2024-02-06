using UnityEngine;

namespace WebglExample
{
    public class JoinServer : MonoBehaviour
    {
        public SignalrConnection Connection;
        void Start()
        {
            Connection.StartConnection();
        }
    }
}