using System.ServiceModel;

namespace SensorContracts
{
    [ServiceContract]
    public interface ISensorService
    {
        [OperationContract]
        TransferStatus StartSession(SessionMeta meta);

        [OperationContract]
        TransferStatus PushSample(SensorSample sample);

        [OperationContract]
        TransferStatus EndSession();
    }
}