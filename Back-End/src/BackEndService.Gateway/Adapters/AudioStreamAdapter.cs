using System.IO;

namespace BackEndService.Gateway.Adapters
{
    public class AudioStreamAdapter
    {
        public Stream Adapt(Stream input) => input;
    }
}


