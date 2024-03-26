using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UHTTP
{
    [CreateAssetMenu(fileName = "HTTPRequestParamsCard", menuName = "Cards/HTTPRequestParamsCard", order = 0)]
    public partial class HTTPRequestParamsCard : ScriptableObject
    {
        [field: SerializeField] public List<KeyValueItem> Parameters { private set; get; }
    }

    [Serializable]
    public class KeyValueItem
    {
        public string key;
        public string value;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(HTTPRequestParamsCard))]
    public class HTTPRequestParamsCardEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var httpRequestParamsCard = (HTTPRequestParamsCard)target;

            if (HasEmptyParams(httpRequestParamsCard))
                EditorGUILayout.HelpBox("Parameters cannot have empty keys or values.", MessageType.Error);

            EditorGUILayout.Space();

            if (GUILayout.Button("Add Default Header Params"))
                AddDefaultHeaderParams(httpRequestParamsCard);
        }

        private static bool HasEmptyParams(HTTPRequestParamsCard httpRequestParamsCard)
        {
            return httpRequestParamsCard.Parameters.Exists(keyValueItems =>
                string.IsNullOrEmpty(keyValueItems.key) || string.IsNullOrEmpty(keyValueItems.value));
        }

        private static void AddDefaultHeaderParams(HTTPRequestParamsCard httpRequestParamsCard)
        {
            AddHeaderParamIfNotExists(httpRequestParamsCard, "Content-Type", "application/json");
            AddHeaderParamIfNotExists(httpRequestParamsCard, "Accept", "application/json");
        }

        private static void AddHeaderParamIfNotExists(HTTPRequestParamsCard httpRequestParamsCard, string key,
            string value)
        {
            if (httpRequestParamsCard.Parameters.Exists(keyValueItems => keyValueItems.key == key)) return;

            var keyValueItem = new KeyValueItem() { key = key, value = value };
            httpRequestParamsCard.Parameters.Add(keyValueItem);
        }
    }
#endif
}