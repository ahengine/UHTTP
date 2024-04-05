using UnityEngine;
using IEnumerator = System.Collections.IEnumerator;

namespace UHTTP.Helpers
{
    public static class ConnectionChecker 
    {
        private static Coroutine coroutine;
        public static void Run(float checkingTime = 5)
        {
            if (coroutine != null)
                HTTPRequestCoroutineRunner.Stop(coroutine);

            coroutine = HTTPRequestCoroutineRunner.Run(ConnectionChecker.Checking(checkingTime));
        }

        public static void Stop() =>
            HTTPRequestCoroutineRunner.Stop(coroutine);

        public static bool IsConnected { private set; get; }
        private static IEnumerator Checking(float checkingTime = 5)
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(checkingTime);
                Ping request = new Ping("http://google.com");
                while (!request.isDone)
                    yield return new WaitForEndOfFrame();

                IsConnected = request.isDone;
            }
        }
    }
}

