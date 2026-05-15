using SensorContracts;
using System;
using System.Configuration;
using System.Globalization;

namespace SensorServiceHost
{
    public class SensorAnalyzer
    {
        public event EventHandler<WarningEventArgs> OnWarningRaised;

        private SensorSample previousSample;

        private double volumeSum = 0;
        private int sampleCount = 0;

        private double V_threshold;
        private double T_dht_threshold;
        private double T_bmp_threshold;
        private double meanDeviationPercent;

        public SensorAnalyzer()
        {
            V_threshold = double.Parse(
                ConfigurationManager.AppSettings["V_threshold"],
                CultureInfo.InvariantCulture);

            T_dht_threshold = double.Parse(
                ConfigurationManager.AppSettings["T_dht_threshold"],
                CultureInfo.InvariantCulture);

            T_bmp_threshold = double.Parse(
                ConfigurationManager.AppSettings["T_bmp_threshold"],
                CultureInfo.InvariantCulture);

            meanDeviationPercent = double.Parse(
                ConfigurationManager.AppSettings["MeanDeviationPercent"],
                CultureInfo.InvariantCulture);
        }

        public void Analyze(SensorSample currentSample)
        {
            sampleCount++;
            volumeSum += currentSample.Volume;

            double volumeMean = volumeSum / sampleCount;

            if (previousSample != null)
            {
                double deltaV = currentSample.Volume - previousSample.Volume;
                double deltaDht = currentSample.T_DHT - previousSample.T_DHT;
                double deltaBmp = currentSample.T_BMP - previousSample.T_BMP;

                if (Math.Abs(deltaV) > V_threshold)
                {
                    RaiseWarning("VolumeSpike",
                        "Nagla promena buke. DeltaV = " + deltaV);
                }

                if (Math.Abs(deltaDht) > T_dht_threshold)
                {
                    RaiseWarning("TemperatureSpikeDHT",
                        "Nagla promena DHT temperature. DeltaTdht = " + deltaDht);
                }

                if (Math.Abs(deltaBmp) > T_bmp_threshold)
                {
                    RaiseWarning("TemperatureSpikeBMP",
                        "Nagla promena BMP temperature. DeltaTbmp = " + deltaBmp);
                }
            }

            if (currentSample.Volume < (1 - meanDeviationPercent) * volumeMean)
            {
                RaiseWarning("OutOfBandWarning",
                    "Volume je ispod ocekivane vrednosti. V = " + currentSample.Volume);
            }

            if (currentSample.Volume > (1 + meanDeviationPercent) * volumeMean)
            {
                RaiseWarning("OutOfBandWarning",
                    "Volume je iznad ocekivane vrednosti. V = " + currentSample.Volume);
            }

            previousSample = currentSample;
        }

        private void RaiseWarning(string type, string message)
        {
            if (OnWarningRaised != null)
            {
                OnWarningRaised(this, new WarningEventArgs
                {
                    WarningType = type,
                    Message = message,
                    Time = DateTime.Now
                });
            }
        }
    }
}