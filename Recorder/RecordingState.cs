using System.ComponentModel;

namespace Recorder
{
    public enum RecordingState
    {
        [Description("停止")]
        Stopped,
        [Description("监测")]
        Monitoring,
        [Description("录音")]
        Recording,
        [Description("请求停止")]
        RequestedStop
    }
}