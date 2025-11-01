using System.IO;

namespace BackEndService.Gateway.Adapters
{
    public class TextStreamAdapter
    {
        public Stream Adapt(Stream input) => input;
    }
}


