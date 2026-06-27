
namespace WalrusWallet.Api.Options;

public class AppInfoOptions
{
    public string Version { get; set; } = "0.0.0-dev";
    public string CommitHash { get; set; } = "unknown";
    public string BuildDate { get; set; } = "";
}