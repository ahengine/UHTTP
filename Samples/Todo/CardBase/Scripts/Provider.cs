using UnityEngine;

namespace UHTTP.Sample.Todo
{
    public class Provider<T> : ScriptableObject where T : Provider<T>
    {
        public static T GetResource() =>
            Resources.Load<T>(typeof(T).Name);
    }
}