using UnityEngine;
using IEnumerator = System.Collections.IEnumerator;

namespace Networking.WebRequestHandler
{
    public class ConnectionChecker : MonoBehaviour
    {
        public static int CheckingTime = 5;

        public static void SetCheckingTime(int checkingTime) =>
            CheckingTime = checkingTime;

        public static bool AllowCheckingConnection { private set; get; } = true;

        public static void SetAllowCheckingConnection(bool value) =>
            AllowCheckingConnection = value;

        public static bool IsConnected { private set; get; }

        [RuntimeInitializeOnLoadMethod]
        private static void StartChecking()
        {
            if (GameObject.FindObjectOfType<ConnectionChecker>())
                return;

            new GameObject(typeof(ConnectionChecker).Name).AddComponent<ConnectionChecker>();
        }

        private void Start()
        {
            StartCoroutine(Checking());
        }

        private static IEnumerator Checking()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(CheckingTime);
                if (AllowCheckingConnection)
                {
                    Ping request = new Ping("http://google.com");
                    while (!request.isDone)
                        yield return new WaitForEndOfFrame();

                    // IsConnected = request. == null;
                }
            }
        }
    }
}

