using SensorContracts;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace SensorClient
{
    internal class Program
    {
        static void Main(string[] args)
        {

            NetTcpBinding binding = new NetTcpBinding();

            EndpointAddress address =
                new EndpointAddress("net.tcp://localhost:9000/SensorService");

            ChannelFactory<ISensorService> factory =
                new ChannelFactory<ISensorService>(binding, address);

            ISensorService client = factory.CreateChannel();

            SessionMeta meta = new SessionMeta
            {
                Volume = 10,
                T_DHT = 20,
                T_BMP = 21,
                Pressure = 1000,
                DateTime = DateTime.Now
            };

            TransferStatus startResponse = client.StartSession(meta);
            Console.WriteLine(startResponse.Message);

            CsvReader reader = new CsvReader();

            List<SensorSample> samples =
                reader.ReadSamples("dataset.csv");
            Console.WriteLine("Broj ucitanih redova: " + samples.Count);
            foreach (SensorSample sample in samples)
            {
                try
                {
                    TransferStatus sampleResponse =
                        client.PushSample(sample);
                    Console.WriteLine(sampleResponse.Message);
                }
                catch (FaultException<ValidationFault> ex)
                {
                    Console.WriteLine("VALIDATION FAULT:");
                    Console.WriteLine(ex.Detail.Reason);
                    Console.WriteLine(ex.Detail.FieldName);
                }
            }

            TransferStatus endResponse = client.EndSession();
            Console.WriteLine(endResponse.Message);

            ((IClientChannel)client).Close();
            factory.Close();

            Console.ReadLine();
        }
    }
}