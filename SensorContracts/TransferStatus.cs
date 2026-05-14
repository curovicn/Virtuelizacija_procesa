using System.Runtime.Serialization;

namespace SensorContracts
{
    [DataContract]
    public class TransferStatus
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string Status { get; set; }
    }
}