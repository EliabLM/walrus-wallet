namespace WalrusWallet.Domain.Common.Interfaces;

public interface IAppInfoService
{
    string Version { get; }
    string Environment { get; }
    DateTime BuildDate { get; }
}