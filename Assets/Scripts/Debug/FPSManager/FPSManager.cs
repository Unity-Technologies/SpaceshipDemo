using GameOptionsUtility;
using GameplayIngredients;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.UI;

[ManagerDefaultPrefab("FPSManager")]
public class FPSManager : Manager
{
    public KeyCode ToggleKey = KeyCode.F8;

    public GameObject FPSRoot;
    public Text FPSCounter;
    public Text MillisecondCounter;

    public KeyCode PauseKey = KeyCode.F5;
    public KeyCode StepKey = KeyCode.F6;

    bool paused = false;
    bool step = false;
    private void OnDisable()
    {
        if (recording)
            EndRecord();
    }

    public void SetActive(bool active)
    {
        if(!recording && FPSRoot != null)
        {
            FPSRoot.SetActive(active);
        }
    }

    private void Update()
    {
        float dt = GetSmoothDeltaTime();

        if (Input.GetKeyDown(ToggleKey) && FPSRoot != null && !recording)
        {
            FPSRoot.SetActive(!FPSRoot.activeInHierarchy);
        }

        if (FPSRoot.activeInHierarchy)
        {
            if (FPSCounter != null)
                FPSCounter.text = $"FPS: {((1.0f / dt).ToString("F1"))}";

            if (MillisecondCounter != null)
                MillisecondCounter.text = $"{((dt * 1000).ToString("F2"))}ms.";

            if (paused && step)
            {
                step = false;
                Time.timeScale = 0.0f;
            }

            if (Input.GetKeyDown(PauseKey) && !recording)
            {
                if (recording)
                    EndRecord();

                paused = !paused;
                Time.timeScale = paused ? 0.0f : 1.0f;
            }
            else if (Input.GetKeyDown(StepKey) && !recording)
            {
                if (recording)
                    EndRecord();

                paused = true;
                step = true;
                Time.timeScale = 1.0f;
            }
        }

        UpdateViz(dt);

        if (recording && !recordingPaused)
        {
            if (recordTTL < 0)
            {
                recordTTL = RecordInterval;
                timings.Add(dt * 1000);
            }
            recordTTL -= Time.unscaledDeltaTime;
        }
    }

    const int MAX_QUEUE = 64;
    Queue<float> queue = new Queue<float>();

    float acc;

    void ResetSmoothDeltaTime()
    {
        acc = 0;
        if (queue == null)
            queue = new Queue<float>();
        else
            queue.Clear();
    }

    float GetSmoothDeltaTime()
    {
        if (queue.Count == MAX_QUEUE)
        {
            acc -= queue.Peek();
            queue.Dequeue();
        }

        float dt = Time.unscaledDeltaTime;
        queue.Enqueue(dt);
        acc += dt;

        return acc / queue.Count;
    }

    #region Visualizer
    [Header("FPS Visualizer")]
    public Image visualizerImage;
    [SerializeField]
    float HeatMapScale = 85.0f;
    [SerializeField]
    Gradient VizHeatMap;
    [SerializeField]
    float updatePeriod = 0;
    const int SAMPLES = 180;
    CustomRenderTexture m_VisSampler;
    public Shader updateShader;
    public Shader drawShader;
    Material m_VisUpdate;
    Material m_VisDraw;
    void InitVisualizer()
    {
        m_VisSampler = new CustomRenderTexture(SAMPLES, 1, RenderTextureFormat.RHalf);
        m_VisSampler.initializationMode = CustomRenderTextureUpdateMode.OnLoad;
        m_VisSampler.initializationColor = Color.black;
        m_VisSampler.doubleBuffered = true;
        m_VisSampler.wrapMode = TextureWrapMode.Clamp;
        m_VisSampler.filterMode = FilterMode.Point;
        m_VisSampler.useMipMap = false;
        m_VisSampler.updatePeriod = updatePeriod;
        m_VisSampler.updateMode = CustomRenderTextureUpdateMode.Realtime;
        m_VisUpdate = new Material(updateShader);
        m_VisSampler.material = m_VisUpdate;
        m_VisDraw = new Material(drawShader);
        m_VisUpdate.SetFloat("_Value", 1.0f);
        m_VisDraw.SetTexture("_MainTex", m_VisSampler);
        m_VisDraw.SetTexture("_HeatMap", BakeGradient(VizHeatMap));

        visualizerImage.material = m_VisDraw;
    }

    Texture2D BakeGradient(Gradient g, int width = 128)
    {
        Texture2D t = new Texture2D(width, 1, TextureFormat.RGB24, false);
        t.wrapMode = TextureWrapMode.Clamp;
        Color[] pixels = new Color[width];
        for (int i = 0; i < width; i++)
        {
            pixels[i] = g.Evaluate((float)i / (width - 1));
        }
        t.SetPixels(pixels);
        t.Apply();
        return t;
    }

    Queue<float> lastsamples = new Queue<float>(SAMPLES);

    void UpdateViz(float dt)
    {
        if (m_VisSampler == null)
            InitVisualizer();

        lastsamples.Enqueue(dt);
        if (lastsamples.Count > SAMPLES)
            lastsamples.Dequeue();

        float maxdt = 0;
        foreach (var time in lastsamples)
            maxdt = Mathf.Max(maxdt, time);

        float dtms = maxdt * 1000;
        dtms = Mathf.Ceil(dtms / 16.666666f) * 16.666666f;

        m_VisUpdate.SetFloat("_Value", dt);
        m_VisDraw.SetFloat("_VizScale", 1000 / dtms);
        m_VisDraw.SetFloat("_HeatMapScale", 1000 / HeatMapScale);
    }

    #endregion

    #region HTML Recorder

    public struct RecordingResults
    {
        public float minMs;
        public float maxMs;
        public float avgMs;
    }

    [Header("Recording")]
    public float RecordInterval = 1.0f;

    bool recording = false;
    public bool recordingPaused = false;
    List<float> timings;
    string HTMLPath;
    float recordTTL;


    public void Record(string filename)
    {
        if (recording)
            EndRecord();

        var now = DateTime.Now;

        HTMLPath = $"{filename}-{now.Year.ToString("D4")}{now.Month.ToString("D2")}{now.Day.ToString("D2")}-{now.Hour.ToString("D2")}{now.Minute.ToString("D2")}{now.Second.ToString("D2")}.html";
        Debug.Log($"Started Recording benchmark at path: {HTMLPath}");
        recording = true;
        recordingPaused = false;
        timings = new List<float>();
        recordTTL = RecordInterval;
        ResetSmoothDeltaTime();
    }

    public RecordingResults results { get; private set; }

    public void EndRecord(bool abort = false)
    {
        recording = false;
        recordingPaused = false;

        if (abort || timings == null || timings.Count == 0)
            return;

        var currentCulture = System.Globalization.CultureInfo.CurrentCulture;
        System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

        string frameTimeStr = string.Join(",", timings);

        float med = 0f;
        float min = float.PositiveInfinity;
        float max = float.NegativeInfinity;

        foreach (var time in timings)
        {
            min = Mathf.Min(time, min);
            max = Mathf.Max(time, max);
            med += time;
        }

        med /= timings.Count;

        results = new RecordingResults
        {
            minMs = min,
            maxMs = max,
            avgMs = med
        };

        GraphicOption go = GameOption.Get<GraphicOption>();
        SpaceshipOptions o = GameOption.Get<SpaceshipOptions>();
        float p = o.screenPercentage / 100f;
        float mPix = (go.width * p * go.height * p) / 1000000;


        string dateTime = $"{DateTime.Now.ToLongDateString()} {DateTime.Now.ToShortTimeString()}";
        string operatingSystem = $"{SystemInfo.operatingSystem}";
        string sp = o.screenPercentage == 100 ? $"Native" : $"{o.screenPercentage}% SP ({o.upsamplingMethod.ToString()})";

        string settings = $"{go.width}x{go.height}@{go.refreshRate}Hz ({go.fullScreenMode}) {sp} ({mPix.ToString("F2")} MegaPixels)- {QualitySettings.names[QualitySettings.GetQualityLevel()]} Quality";
        string bestFPS = $"{(1000 / min).ToString("F1")}fps ({min.ToString("F2")}ms)";
        string worstFPS = $"{(1000 / max).ToString("F1")}fps ({max.ToString("F2")}ms)";
        string averageFPS = $"{(1000 / med).ToString("F1")}fps ({med.ToString("F2")}ms)";
        string msPerMPix = $"{(med/mPix).ToString("F2")} ms/MPix";

        string systemInfo = $"{SystemInfo.deviceModel}";
        string cpuInfo = $" {SystemInfo.processorType} ({SystemInfo.processorCount} threads) @ {(SystemInfo.processorFrequency / 1000f).ToString("F2")} GHz.";
        string gpuInfo = $"{SystemInfo.graphicsDeviceName}({SystemInfo.graphicsDeviceType}) {SystemInfo.graphicsMemorySize / 1000}GB VRAM";
        string memInfo = $"{SystemInfo.systemMemorySize / 1000}GB.";


#if UNITY_STANDALONE && !UNITY_EDITOR
        try
        {
            string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string spaceshipPath = Path.Combine(myDocumentsPath, "Spaceship Demo");

            if (!Directory.Exists(spaceshipPath))
                Directory.CreateDirectory(spaceshipPath);

            var writer = new StreamWriter($"{spaceshipPath}/{HTMLPath}");
            writer.Write(@$"<html>
<head>
    <link href=""https://cdnjs.cloudflare.com/ajax/libs/c3/0.7.20/c3.css"" rel=""stylesheet"">
    <link rel=""preconnect"" href=""https://fonts.gstatic.com"">
    <link href=""https://fonts.googleapis.com/css2?family=Roboto:wght@400;700&display=swap"" rel=""stylesheet"">
    <script src=""https://cdnjs.cloudflare.com/ajax/libs/d3/5.16.0/d3.min.js"" charset=""utf-8"" ></script>
    <script src=""https://cdnjs.cloudflare.com/ajax/libs/c3/0.7.20/c3.min.js""></script>
<style>
h1, h2, h3, h4, h5, body, p
{{
font-family: 'Roboto', sans-serif;
}}
</style>
</head>
<body>
<h1> Spaceship - Benchmark Results</h1>
<b>Average FPS : </b> {averageFPS} <br/>
<b>Average MS per MegaPixel : </b> {msPerMPix} <br/>
<b>Best FPS : </b> {bestFPS} <br/>
<b>Worst FPS : </b> {worstFPS} <br/>
<br/>
<b>Recorded on : </b>{dateTime} <br/>
<b>Computer : </b> {systemInfo}<br/>
<b>Operating System : </b>{operatingSystem} <br/>
<b>Settings:</b> {settings}<br/>
<b>CPU : </b> {cpuInfo} <br/>
<b>Memory : </b> {memInfo} <br/>
<b>GPU : </b> {gpuInfo} <br/>

<div id=""chart""></div>
</body>
<script>
var chart = c3.generate({{
    bindto: '#chart',
    data: {{
      columns: [
        ['Frame Time (milliseconds)', {frameTimeStr}]
      ]
    }}
}});
</script>
</html>
");

            writer.Close();
            System.Globalization.CultureInfo.CurrentCulture = currentCulture;

        } 
        catch (Exception e)
        {
           
        }
#endif
    }

    #endregion
}
