using System;

namespace SensorServiceHost
{
    public class WarningEventArgs : EventArgs
    {
        public string WarningType { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
    }
}