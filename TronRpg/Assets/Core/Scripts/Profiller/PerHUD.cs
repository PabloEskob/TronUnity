using System.Text;
using UnityEngine;
using Unity.Profiling;

public class PerfHud : MonoBehaviour
{
    [SerializeField] int avgSamples = 60;

    ProfilerRecorder mainThreadTime;
    ProfilerRecorder gcAllocInFrame;
    ProfilerRecorder systemUsedMem;

    readonly StringBuilder _sb = new StringBuilder(256);
    bool _visible = true;
    ulong _lastCpuFt, _lastGpuFt; // in nanoseconds

    static double AvgNs(ProfilerRecorder r)
    {
        int n = r.Capacity;
        if (n == 0 || !r.Valid) return 0;
        double sum = 0;
        unsafe
        {
            var tmp = stackalloc ProfilerRecorderSample[n];
            r.CopyTo(tmp, n);
            for (int i = 0; i < n; i++) sum += tmp[i].Value;
        }

        return sum / n;
    }

    void OnEnable()
    {
        mainThreadTime = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", avgSamples);
        // Если в вашей версии нет счётчика "GC Allocated In Frame", просто будет 0/невалидно.
        gcAllocInFrame = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocated In Frame", avgSamples);
        systemUsedMem = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "System Used Memory");
    }

    void OnDisable()
    {
        mainThreadTime.Dispose();
        gcAllocInFrame.Dispose();
        systemUsedMem.Dispose();
    }

    void Update()
    {
        // Запрашиваем свежие frame timings (CPU/GPU) — работает в Dev Build всегда,
        // в Release требует включенной PlayerSettings.enableFrameTimingStats.
        UnityEngine.FrameTimingManager.CaptureFrameTimings();
        var frames = new UnityEngine.FrameTiming[1];
        if (UnityEngine.FrameTimingManager.GetLatestTimings(1, frames) > 0)
        {
            _lastCpuFt = (ulong)(frames[0].cpuFrameTime * 1_000_000); // ms->ns
            _lastGpuFt = (ulong)(frames[0].gpuFrameTime * 1_000_000);
        }
    }

    void OnGUI()
    {
        if (!_visible) return;

        double mainNs = AvgNs(mainThreadTime);
        double fps = 1.0 / Mathf.Max(1e-6f, Time.smoothDeltaTime);

        _sb.Length = 0;
        _sb.AppendLine($"FPS: {fps:F1}");
        _sb.AppendLine($"Main Thread: {mainNs * 1e-6:F2} ms");
        _sb.AppendLine($"CPU frame:   {_lastCpuFt * 1e-6:F2} ms   GPU frame: {_lastGpuFt * 1e-6:F2} ms");
        if (gcAllocInFrame.Valid) _sb.AppendLine($"GC / frame:  {gcAllocInFrame.LastValue / 1024f:F0} KB");
        _sb.AppendLine($"Sys Mem:     {systemUsedMem.LastValue / (1024f * 1024f):F1} MB");

        var size = GUI.skin.label.CalcSize(new GUIContent(_sb.ToString()));
        var rect = new Rect(10, 10, Mathf.Max(240, size.x + 12), size.y + 8);
        GUI.Box(rect, GUIContent.none);
        GUI.Label(new Rect(rect.x + 6, rect.y + 4, rect.width - 12, rect.height - 8), _sb.ToString());
    }
}