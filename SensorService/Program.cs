using SensorContracts;
using System;
using System.ServiceModel;

namespace SensorServiceHost
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(SensorService));

            host.Open();


            Console.WriteLine("Sensor servis je pokrenut.");
            Console.WriteLine("Pritisni ENTER za gasenje servisa...");
            Console.ReadLine();

            host.Close();
        }
    }
}