namespace Application.Services
{
    public interface IQrCodeService
    {
        string GenerateQrCodeImage(string content, string fileName);
    }
}
