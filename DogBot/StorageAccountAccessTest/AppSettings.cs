namespace StorageAccountAccessTest;

public class AppSettings
{
    public string BlobContainerUri { get; set; } = "";
    public AccountKeyMethod AccountKeyMethod { get; set; } = new AccountKeyMethod();
    public ConnectionStringMethod ConnectionStringMethod { get; set; } = new ConnectionStringMethod();
    public SasTokenMethod SasTokenMethod { get; set; } = new SasTokenMethod();
}

public class AccountKeyMethod
{
    public string AccountName { get; set; } = "";
    public string AccountKey { get; set; } = "";
}

public class ConnectionStringMethod
{
    public string ConnectionString { get; set; } = "";
    public string ContainerName { get; set; } = "";
}

public class SasTokenMethod
{
    public string SasToken { get; set; } = "";
}