using System.IO;

namespace BackEndService.Core.Models.Shared
{
    public class AudioData
    {
        public Stream Stream { get; set; }
        public string? MimeType { get; set; }

        public AudioData(Stream stream, string? mimeType = null)
        {
            Stream = stream;
            MimeType = mimeType;
        }
    }
}


