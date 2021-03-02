using System;

namespace Pjfm.WebClient.Services
{
    public class BrowserQueueSettings
    {
        public string[] Genres { get; set; }
        public string[] SeedTracks { get; set; }
        public string[] SeedArtists { get; set; }
        public QueueSettingsValue Tempo { get; set; }
        public QueueSettingsValue Instrumentalness { get; set; }
        public QueueSettingsValue Popularity { get; set; }
        public QueueSettingsValue Energy { get; set; }
        public QueueSettingsValue DanceAbility { get; set; }
        public QueueSettingsValue Valence { get; set; }

        public (int? Min, int? Max, int? Target) GetTempoValues()
        {
            switch (Tempo)
            {
                case QueueSettingsValue.Not:
                    return (null, null, null); 
                case QueueSettingsValue.Minimal:
                    return (0, 60, 30);
                case QueueSettingsValue.Little:
                    return (30, 80, 50);
                case QueueSettingsValue.Average:
                    return (80, 130, 100);
                case QueueSettingsValue.Much:
                    return (100, 180, 140);
                case QueueSettingsValue.Maximal:
                    return (160, 300, 200);
                default:
                    return (null, null, null);
            }
        }
        public (decimal? Min, decimal? Max, decimal? Target) GetInstrumentalnessValues()
        {
            switch (Instrumentalness)
            {
                case QueueSettingsValue.Not:
                    return (null, null, null); 
                case QueueSettingsValue.Minimal:
                    return (0.00m, 0.20m, 0.10m);
                case QueueSettingsValue.Little:
                    return (0.10m, 0.35m, 0.20m);
                case QueueSettingsValue.Average:
                    return (0.20m, 0.60m, 0.40m);
                case QueueSettingsValue.Much:
                    return (0.60m, 0.90m, 0.75m);
                case QueueSettingsValue.Maximal:
                    return (0.80m, 1.00m, 0.90m);
                default:
                    return (null, null, null);
            }
        }
        public (decimal? Min, decimal? Max, decimal? Target) GetEnergyValues()
        {
            switch (Energy)
            {
                case QueueSettingsValue.Not:
                    return (null, null, null); 
                case QueueSettingsValue.Minimal:
                    return (0.00m, 0.20m, 0.10m);
                case QueueSettingsValue.Little:
                    return (0.10m, 0.35m, 0.20m);
                case QueueSettingsValue.Average:
                    return (0.20m, 0.60m, 0.40m);
                case QueueSettingsValue.Much:
                    return (0.60m, 0.90m, 0.75m);
                case QueueSettingsValue.Maximal:
                    return (0.80m, 1.00m, 0.90m);
                default:
                    return (null, null, null);
            }
        }
        public (decimal? Min, decimal? Max, decimal? Target) GetDanceAbilityValues()
        {
            switch (DanceAbility)
            {
                case QueueSettingsValue.Not:
                    return (null, null, null); 
                case QueueSettingsValue.Minimal:
                    return (0.00m, 0.20m, 0.10m);
                case QueueSettingsValue.Little:
                    return (0.10m, 0.35m, 0.20m);
                case QueueSettingsValue.Average:
                    return (0.20m, 0.60m, 0.40m);
                case QueueSettingsValue.Much:
                    return (0.60m, 0.90m, 0.75m);
                case QueueSettingsValue.Maximal:
                    return (0.80m, 1.00m, 0.90m);
                default:
                    return (null, null, null);
            }
        }
        public (int? Min, int? Max, int? Target) GetPopularityValues()
        {
            switch (Popularity)
            {
                case QueueSettingsValue.Not:
                    return (null, null, null); 
                case QueueSettingsValue.Minimal:
                    return (0, 10, 5);
                case QueueSettingsValue.Little:
                    return (0, 30, 15);
                case QueueSettingsValue.Average:
                    return (20, 50, 30);
                case QueueSettingsValue.Much:
                    return (50, 80, 60);
                case QueueSettingsValue.Maximal:
                    return (80, 100, 90);
                default:
                    return (null, null, null);
            }
        }
    }
}