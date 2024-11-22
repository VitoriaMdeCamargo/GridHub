namespace GridHub.API.Configuration
{
    public class APPConfiguration
    {
        public SwaggerInfo Swagger { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }
        public OracleFIAP OracleFIAP { get; set; }
    }

    public class ConnectionStrings
    {
        public string FIAPDatabase { get; set; }
        public string ProductsMongoDb { get; set; }
    }

    public class SwaggerInfo
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }

    public class OracleFIAP
    {
        public string URL { get; set; }
        public int Port { get; set; }
        public string SID { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }

        public string Connection
        {
            get { return $"Data Source={URL}:{Port}/{SID};User ID={User};Password={Password};"; }
        }
    }
}
