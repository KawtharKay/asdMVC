using Application.Services;
using Infrastructure.Settings;
using Microsoft.Extensions.Options;
using QRCoder;

namespace Infrastructure.Services
{
    public class QrCodeService : IQrCodeService
    {
        private readonly FileSettings _settings;
        public QrCodeService(IOptions<FileSettings> settings)
        {
            _settings = settings.Value;
        }

        public string GenerateQrCodeImage(string content, string fileName)
        {
            using var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(
                content, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeBytes = qrCode.GetGraphic(20);

            var folderPath = _settings.QrCodesPath;

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, $"{fileName}.png");
            File.WriteAllBytes(filePath, qrCodeBytes);

            return $"/qrcodes/{fileName}.png";
        }
    }
}