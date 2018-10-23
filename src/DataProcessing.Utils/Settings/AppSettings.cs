namespace DataProcessing.Utils.Settings
{
    public class AppSettings
    {
        public const string ConnectionStringNamesSectionName = "ConnectionStringNames";

        public string ConnectionString { get; set; }
        public DbCreationSettings DbCreationSettings { get; set; }
        public DataSettings DataSettings { get; set; }
    }

    public class DbCreationSettings
    {
        public bool ForceRecreation { get; set; }
    }

    public class DataSettings
    {
        public int DatasetsCount { get; set; }
        public int ClientsPerDataset { get; set; }
        public int DepositsPerClient { get; set; }
        public int LoansPerClient { get; set; }

        public override string ToString()
        {
            return $"{nameof(DatasetsCount)}: {DatasetsCount}, {nameof(ClientsPerDataset)}: {ClientsPerDataset}, {nameof(DepositsPerClient)}: {DepositsPerClient}, {nameof(LoansPerClient)}: {LoansPerClient}";
        }
    }
}
