using SensorContracts;
using System;
using System.IO;
using System.ServiceModel;

namespace SensorServiceHost
{
    public class SensorService : ISensorService
    {
        private static string filePath = "measurements_session.csv";

        private static SensorAnalyzer analyzer = new SensorAnalyzer();
        private static bool eventsSubscribed = false;
        private static SessionFileWriter fileWriter;
        public static event EventHandler OnTransferStarted;
        public static event EventHandler OnSampleReceived;
        public static event EventHandler OnTransferCompleted;

        public TransferStatus StartSession(SessionMeta meta)
        {
            fileWriter = new SessionFileWriter(
              "measurements_session.csv",
              "rejects.csv",
              "warnings.csv");

            Console.WriteLine("Sesija zapoceta.");

            OnTransferStarted?.Invoke(null, EventArgs.Empty);
            if (!eventsSubscribed)
            {
                analyzer.OnWarningRaised += Analyzer_OnWarningRaised;
                OnTransferStarted += SensorService_OnTransferStarted;
                OnSampleReceived += SensorService_OnSampleReceived;
                OnTransferCompleted += SensorService_OnTransferCompleted;
                eventsSubscribed = true;
            }


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
              $"{sample.Volume},{sample.T_DHT},{sample.T_BMP},{sample.Pressure},{sample.DateTime}";

            if (!IsValid(sample))
            {
                fileWriter.WriteReject(line);

                Console.WriteLine("Odbacen nevalidan sample.");

                
                throw new FaultException<ValidationFault>(
                    new ValidationFault
                    {
                        Reason = "Sample contains invalid data.",
                        FieldName = "SensorSample"
                    },
                    "Validation error");
            }


            fileWriter.WriteMeasurement(line);

            Console.WriteLine("Primljen sample.");

            OnSampleReceived?.Invoke(null, EventArgs.Empty);
            analyzer.Analyze(sample);

            return new TransferStatus
            {
                Success = true,
                Message = "Sample received",
                Status = "IN_PROGRESS"
            };
        }

        private bool IsValid(SensorSample sample)
        {
            if (sample == null)
                return false;

            if (sample.Pressure <= 0)
                return false;

            if (sample.Volume < 0)
                return false;

            if (sample.T_DHT < -50 || sample.T_DHT > 80)
                return false;

            if (sample.T_BMP < -50 || sample.T_BMP > 80)
                return false;

            if (sample.DateTime == default(DateTime))
                return false;

            return true;
        }

        public TransferStatus EndSession()
        {
            fileWriter.Dispose();

            Console.WriteLine("Sesija zavrsena.");
            OnTransferCompleted?.Invoke(null, EventArgs.Empty);
            Console.WriteLine("Resursi zatvoreni pomocu Dispose pattern-a.");

            return new TransferStatus
            {
                Success = true,
                Message = "Transfer completed",
                Status = "COMPLETED"
            };
        }
        private static void Analyzer_OnWarningRaised(object sender, WarningEventArgs e)
        {
            Console.WriteLine("UPOZORENJE: " + e.WarningType);
            Console.WriteLine(e.Message);
            Console.WriteLine("Vreme: " + e.Time);

            string line =
                $"{e.WarningType},{e.Message},{e.Time}";

            fileWriter.WriteWarning(line);
        }
        private static void SensorService_OnTransferStarted(object sender, EventArgs e)
        {
            Console.WriteLine("DOGADJAJ: Transfer je zapocet.");
        }

        private static void SensorService_OnSampleReceived(object sender, EventArgs e)
        {
            Console.WriteLine("DOGADJAJ: Primljen je sample.");
        }

        private static void SensorService_OnTransferCompleted(object sender, EventArgs e)
        {
            Console.WriteLine("DOGADJAJ: Transfer je zavrsen.");
        }


    }
}