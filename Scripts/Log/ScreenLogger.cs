using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;


namespace RedHoney.Log
{
    ///////////////////////////////////////////////////////////////////////////////
    public class ScreenLogger : MonoBehaviour
    {
        public Color textColor = Color.white;
        public Color logColor = Color.white;
        public Color warningColor = Color.yellow;
        public Color errorColor = Color.red;

        public int textSize = 20;
        public int maxLogsNumber = 20;

        [Range(0, 1)]
        public float width = 0.5f;

        private Dictionary<LogType, string> logTypeColors;
        private GUIStyle labelStyle;
        private readonly Queue logs = new Queue();
        private string logsString;

        ///////////////////////////////////////////////////////////////////////////
        void Awake()
        {
            labelStyle = new GUIStyle();
            labelStyle.normal.textColor = textColor;
            labelStyle.fontSize = textSize;
            labelStyle.wordWrap = true;

            string colorToCode(Color c) => $"#{ColorUtility.ToHtmlStringRGBA(c)}";
            logTypeColors = new Dictionary<LogType, string>
            {
                [LogType.Log] = colorToCode(logColor),
                [LogType.Warning] = colorToCode(warningColor),
                [LogType.Error] = colorToCode(errorColor),
                [LogType.Exception] = colorToCode(errorColor),
                [LogType.Assert] = colorToCode(errorColor),
            };
        }

        ///////////////////////////////////////////////////////////////////////////
        void OnEnable()
        {
            Application.logMessageReceivedThreaded += HandleLog;
        }

        ///////////////////////////////////////////////////////////////////////////
        void OnDisable()
        {
            Application.logMessageReceivedThreaded -= HandleLog;
        }

        ///////////////////////////////////////////////////////////////////////////
        void HandleLog(string newLog, string stackTrace, LogType type)
        {
            StringBuilder newScreenLog = new StringBuilder();
            newScreenLog.Append($"<color={logTypeColors[type]}>");
            newScreenLog.Append($"{newLog}\n");
            if (type == LogType.Exception)
                newScreenLog.Append($"\t{stackTrace}\n");
            newScreenLog.Append("</color>");

            logs.Enqueue(newScreenLog);
            if (logs.Count > maxLogsNumber)
                logs.Dequeue();

            logsString = string.Join("", logs.ToArray());
        }

        ///////////////////////////////////////////////////////////////////////////
        void OnGUI()
        {
            GUILayout.Label(logsString, labelStyle, GUILayout.MaxWidth(Screen.width * width));
        }
    }
}