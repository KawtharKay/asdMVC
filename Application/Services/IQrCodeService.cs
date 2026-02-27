using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public interface IQrCodeService
    {
        string GenerateQrCodeBase64(string content);
        string GenerateQrCodeImage(string content, string fileName);
    }
}
