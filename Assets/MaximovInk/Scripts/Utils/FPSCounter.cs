using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;

namespace MaximovInk
{
    public class FPSCounter : MonoBehaviour
    {
        public Color tx_Color = Color.white;
        private StringBuilder tx;
        public TextMeshProUGUI text;

        private float updateInterval = 0.2f;
        private float lastInterval;
        private float frames = 0;

        private float framesavtick = 0;
        private float framesav = 0.0f;

        // Use this for initialization
        private void Start()
        {
            lastInterval = Time.realtimeSinceStartup;
            frames = 0;
            framesav = 0;
            tx = new StringBuilder();
            tx.Capacity = 200;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        // Update is called once per frame
        private void Update()
        {
            ++frames;

            var timeNow = Time.realtimeSinceStartup;

            if (timeNow > lastInterval + updateInterval)
            {
                float fps = frames / (timeNow - lastInterval);
                float ms = 1000.0f / Mathf.Max(fps, 0.00001f);

                ++framesavtick;
                framesav += fps;
                float fpsav = framesav / framesavtick;

                tx.Length = 0;

                tx.AppendFormat("Time: {0,0:F1} ms\nFPS current {1,0:F1}\nFPS average {2,0:F1}\n", ms, fps, fpsav)

                .AppendFormat("\nRAM usage: {0} mb\n",
                Profiler.usedHeapSizeLong / 1048576
                );
                tx.AppendFormat("\nOS: {0}(RAM: {1} mb)\nGPU: {2}({3} mb)\nCPU: {4}({6}x{5} MHz)",
                                SystemInfo.operatingSystem,
                                SystemInfo.systemMemorySize,
                                SystemInfo.graphicsDeviceName,
                                SystemInfo.graphicsMemorySize,
                                SystemInfo.processorType,
                                SystemInfo.processorFrequency,
                                SystemInfo.processorCount
                                );

                text.text = tx.ToString();
                frames = 0;
                lastInterval = timeNow;
            }
        }
    }
}