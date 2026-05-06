namespace EventManager.Application.Interfaces;

public interface IQrCodeGenerator
{
    byte[] Generate(string content);
}
