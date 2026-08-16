namespace N88.Loader.Decoders
{
    using System.Threading.Tasks;

    public class UTF8Decoder : IDecoder<string>
    {
        public Task<string> Decode(byte[]? raw)
        {
            if(raw == null){return Task.FromResult(string.Empty);}
            var text = System.Text.Encoding.UTF8.GetString(raw);
            return Task.FromResult(text);
        }
    }
}