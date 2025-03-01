namespace Data.Configs
{

    public class DatabaseConfig
    {

        public string ConnectionString { get; set; } = string.Empty;

        public int Timeout { get; set; } = 30;

    }

}