using UnityEngine;

namespace UHTTP
{
    public class Provider<T> : ScriptableObject where T : Provider<T>
    {
        public static T Load =>
            Resources.Load<T>(typeof(T).Name);
    }
}
