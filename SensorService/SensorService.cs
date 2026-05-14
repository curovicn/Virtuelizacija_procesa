using SensorContracts;
using System;
using System.IO;

namespace SensorServiceHost
{
    public class SensorService : ISensorService
    {
        private static string filePath = "measurements_session.csv";

        public TransferStatus StartSession(SessionMeta meta)
        {
            File.WriteAllText(filePath,
                "Volume,T_DHT,T_BMP,Pressure,DateTime\n");

            Console.WriteLine("Sesija zapoceta.");

            return new TransferStatus
            {
                Success = true,
                Message = "Session started",
                Status = "IN_PROGRESS"
            };
        }

        public TransferStatus PushSample(SensorSample sample)
        {
            string line =
                $"{sample.Volume}," +
                $"{sample.T_DHT}," +
                $"{sample.T_BMP}," +
                $"{sample.Pressure}," +
                $"{sample.DateTime}";

            File.AppendAllText(filePath, line + Environment.NewLine);

            Console.WriteLine("Primljen sample.");

            return new TransferStatus
            {
                Success = true,
                Message = "Sample received",
                Status = "IN_PROGRESS"
            };
        }

        public TransferStatus EndSession()
        {
            Console.WriteLine("Sesija zavrsena.");

            return new TransferStatus
            {
                Success = true,
                Message = "Transfer completed",
                Status = "COMPLETED"
            };
        }
    }
}