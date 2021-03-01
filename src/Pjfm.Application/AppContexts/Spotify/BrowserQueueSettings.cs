namespace Pjfm.WebClient.Services
{
    public class BrowserQueueSettings
    {
        public string Genre { get; set; }
        public QueueSettingsValue Tempo { get; set; }
        public QueueSettingsValue Instrumentalness { get; set; }
        public QueueSettingsValue Popularity { get; set; }
        public QueueSettingsValue Energy { get; set; }
        public QueueSettingsValue DanceAbility { get; set; }
        public QueueSettingsValue Valence { get; set; }
    }
}