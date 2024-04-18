# if UNITY_EDITOR
using UHTTP;
using UnityEditor;
using UnityEngine;

namespace HTTPRequestService.Editor
{
    public class URLUpdater : EditorWindow
    {
        private string OfflineURL
        {
            get
            {
                return EditorPrefs.GetString("Offline_Url", "localhost");
            }

            set
            {
                EditorPrefs.SetString("Offline_Url", value);
            }
        }

        private string OnlineURL
        {
            get
            {
                return EditorPrefs.GetString("Online_Url", "localhost");
            }

            set
            {
                EditorPrefs.SetString("Online_Url", value);
            }
        }

        private string OfflinePort
        {
            get
            {
                return EditorPrefs.GetString("Offline_Port", "7055");
            }

            set
            {
                EditorPrefs.SetString("Offline_Port", value);
            }
        }

        private string OnlinePort
        {
            get
            {
                return EditorPrefs.GetString("Online_Port", "7055");
            }

            set
            {
                EditorPrefs.SetString("Online_Port", value);
            }
        }

        private string Protocol
        {
            get
            {
                return EditorPrefs.GetString("Protocol", "http");
            }

            set
            {
                EditorPrefs.SetString("Protocol", value);
            }
        }

        private int LastSet
        {
            get
            {
                return EditorPrefs.GetInt("Last_Set", 0);
            }
            set
            {
                EditorPrefs.SetInt("Last_Set", value);
            }
        }

        [MenuItem("URL Updater", menuItem = "Services/HTTP/URL Updater")]
        public static void OpenWindow()
        {
            URLUpdater uRLUpdater = GetWindow<URLUpdater>();
            uRLUpdater.titleContent = new GUIContent("URL Updater");
        }

        private void OnGUI()
        {
            string status = LastSet == 0 ? "Offline" : "Online";
            EditorGUILayout.HelpBox($"Status:{status}", MessageType.Info);

            Protocol = EditorGUILayout.TextField("Protocol:", Protocol);
            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("Offline Settings:");
            OfflineURL = EditorGUILayout.TextField("Offline URL", OfflineURL);
            OfflinePort = EditorGUILayout.TextField("Offline Port", OfflinePort);
            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("Online Settings:");
            OnlineURL = EditorGUILayout.TextField("Online URL", OnlineURL);
            OnlinePort = EditorGUILayout.TextField("Online Port", OnlinePort);
            if (GUILayout.Button("Set Online"))
            {
                LastSet = 1;
                UpdateURLs(Protocol, OnlineURL, OnlinePort);
            }

            if (GUILayout.Button("Set Offline"))
            {
                LastSet = 0;
                UpdateURLs(Protocol, OfflineURL, OfflinePort);
            }
        }

        private void UpdateURLs(string protocol, string address, string port)
        {
            string[] paths = AssetDatabase.FindAssets($"t:{nameof(HTTPRequestCard)}");
            for (int i = 0; i < paths.Length; i++)
            {
                HTTPRequestCard card = (HTTPRequestCard)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(paths[i]), typeof(HTTPRequestCard));
                EditorUtility.DisplayProgressBar("Updating URL's", $"URL {card.name}", (i + 1) / (float)(paths.Length));
                string baseUrl = $"{protocol}://{address}:{port}";
                string url = card.URL;
                url = ClearUrlBeforeThirdSlash(url);
                url = url.Insert(0, baseUrl);
                card.SetURL(url);
                EditorUtility.SetDirty(card);
            }

            EditorUtility.ClearProgressBar();
        }


        //Thanks to chatgpt
        private string ClearUrlBeforeThirdSlash(string url)
        {
            int firstIndex = url.IndexOf("/");
            int secondIndex = url.IndexOf("/", firstIndex + 1);
            int thirdIndex = url.IndexOf("/", secondIndex + 1);

            if (thirdIndex != -1)
            {
                string clearedUrl = url.Substring(thirdIndex);
                return clearedUrl;
            }
            else
            {
                return url;
            }
        }
    }
}
#endif