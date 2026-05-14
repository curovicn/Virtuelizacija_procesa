using SensorContracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SensorClient
{
    public class CsvReader
    {
        public List<SensorSample> ReadSamples(string path)
        {
            List<SensorSample> samples =
                new List<SensorSample>();

            string[] lines = File.ReadAllLines(path);

            for (int i = 1; i < lines.Length && i <= 100; i++)
            {
                try
                {
                    string[] values = lines[i].Split(',');

                    SensorSample sample =
                            new SensorSample
                            {
                                DateTime = DateTime.Parse(values[0]),

                                Volume = double.Parse(values[1],
                                    CultureInfo.InvariantCulture),

                                T_DHT = double.Parse(values[3],
                                    CultureInfo.InvariantCulture),

                                Pressure = double.Parse(values[4],
                                    CultureInfo.InvariantCulture),

                                T_BMP = double.Parse(values[5],
                                    CultureInfo.InvariantCulture)
                            };


                    samples.Add(sample);
                }
                catch
                {
                    File.AppendAllText("rejects.csv",
                        lines[i] + Environment.NewLine);
                }
            }

            return samples;
        }
    }
}