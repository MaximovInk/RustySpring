using System.Text;
using TMPro;
using UnityEngine;

namespace MaximovInk
{
    public class Console : MonoBehaviour
    {
        public TextMeshProUGUI output;

        private void Awake()
        {
            Application.logMessageReceived += Log;

            Debug.Log("log");
            Debug.LogWarning("log warn");
            Debug.Log("log");
            Debug.LogError("log eerror");
            Debug.Log("log");
        }

        private Color GetColorOfType(LogType type)
        {
            return (type == LogType.Error || type == LogType.Exception) ? Color.red : type == LogType.Warning ? Color.yellow : Color.white;
        }

        private void Log(string condition, string stackTrace, LogType type)
        {
            var color = GetColorOfType(type);

            if (color == Color.white)
                WriteLine(condition);
            else
                WriteLine($"<color={ColorUtility.ToHtmlStringRGB(color)}>{condition}</color>");
        }

        public void Clear()
        {
            output.text = string.Empty;
        }

        public void Write(params object[] objs)
        {
            var str = new StringBuilder();

            for (int i = 0; i < objs.Length; i++)
            {
                str.Append(objs[i].ToString());
            }

            output.text += str.ToString();
        }

        public void WriteLine(params object[] objs)
        {
            var str = new StringBuilder();

            for (int i = 0; i < objs.Length; i++)
            {
                str.Append(objs[i]);
            }

            str.Append("\n");

            output.text += str.ToString();
        }
    }
}