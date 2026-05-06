using EventManager.Application.Interfaces;
using QRCoder;

namespace EventManager.Infrastructure.Email;

public sealed class QrCodeGenerator : IQrCodeGenerator
{
    public byte[] Generate(string content)
    {
        using var generator = new QRCodeGenerator();
        var data = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
        var png = new PngByteQRCode(data);
        return png.GetGraphic(20);
    }
}
