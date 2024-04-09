using System;
using System.Collections.Generic;
using UnityEngine;

namespace UHTTP
{
    [CreateAssetMenu(fileName = "HTTPRequestParamsCard", menuName = "Cards/HTTPRequestParamsCard", order = 0)]
    public class HTTPRequestParamsCard : ScriptableObject
    {
        [field: SerializeField] public List<KeyValueItem> Parameters { private set; get; }

        public bool HasEmptyParams() =>
            Parameters.Exists(keyValueItems =>
            string.IsNullOrEmpty(keyValueItems.key) || string.IsNullOrEmpty(keyValueItems.value));

        public void AddParam(string key,string value)
        {
            if (Parameters.Exists(keyValueItems => keyValueItems.key == key)) return;
            Parameters.Add(new KeyValueItem() { key = key, value = value });
        }

        [ContextMenu("Add Default Header Params")]
        public void AddDefaultHeaderParams()
        {
            AddParam("Content-Type", "application/json");
            AddParam("Accept", "application/json");
        }
    }

    [Serializable]
    public class KeyValueItem
    {
        public string key;
        public string value;
    }
}