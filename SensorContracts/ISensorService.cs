using System.ServiceModel;

namespace SensorContracts
{
    [ServiceContract]
    public interface ISensorService
    {
        [OperationContract]
        TransferStatus StartSession(SessionMeta meta);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        TransferStatus PushSample(SensorSample sample);

        [OperationContract]
        TransferStatus EndSession();
    }
}