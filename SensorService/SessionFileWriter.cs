using System;
using System.IO;

namespace SensorServiceHost
{
    public class SessionFileWriter : IDisposable
    {
        private FileStream measurementsStream;
        private StreamWriter measurementsWriter;

        private FileStream rejectsStream;
        private StreamWriter rejectsWriter;

        private FileStream warningsStream;
        private StreamWriter warningsWriter;

        private bool disposed = false;

        public SessionFileWriter(string measurementsPath, string rejectsPath, string warningsPath)
        {
            measurementsStream = new FileStream(measurementsPath, FileMode.Create, FileAccess.Write);
            measurementsWriter = new StreamWriter(measurementsStream);

            rejectsStream = new FileStream(rejectsPath, FileMode.Create, FileAccess.Write);
            rejectsWriter = new StreamWriter(rejectsStream);

            warningsStream = new FileStream(warningsPath, FileMode.Create, FileAccess.Write);
            warningsWriter = new StreamWriter(warningsStream);

            warningsWriter.WriteLine("WarningType,Message,Time");
            measurementsWriter.WriteLine("Volume,T_DHT,T_BMP,Pressure,DateTime");
            rejectsWriter.WriteLine("Volume,T_DHT,T_BMP,Pressure,DateTime");
        }

        public void WriteMeasurement(string line)
        {
            measurementsWriter.WriteLine(line);
            measurementsWriter.Flush();
        }

        public void WriteReject(string line)
        {
            rejectsWriter.WriteLine(line);
            rejectsWriter.Flush();
        }
        public void WriteWarning(string line)
        {
            warningsWriter.WriteLine(line);
            warningsWriter.Flush();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    measurementsWriter?.Close();
                    measurementsStream?.Close();

                    rejectsWriter?.Close();
                    rejectsStream?.Close();

                    warningsWriter?.Close();
                    warningsStream?.Close();
                }

                disposed = true;
            }
        }

        ~SessionFileWriter()
        {
            Dispose(false);
        }
    }
}