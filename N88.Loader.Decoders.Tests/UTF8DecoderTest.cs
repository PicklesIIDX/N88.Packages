namespace N88.Loader.Decoders.Tests
{
    using System.Text;
    using System.Threading.Tasks;
    using AwesomeAssertions;

    [TestFixture]
    [TestOf(typeof(UTF8Decoder))]
    public class UTF8DecoderTest
    {
        [Test]
        public async Task Decode_WhenGivenUTF8Text_ReturnsExpectedText()
        {
            var decoder = new UTF8Decoder();
            const string AnyUTF8Text = "hello world";
            var bytes = Encoding.UTF8.GetBytes(AnyUTF8Text);
            var decodedText = await decoder.Decode(bytes);
            decodedText.Should().Be(AnyUTF8Text);
        }
        
        [Test]
        public async Task Decode_WhenGivenNullBytes_ReturnsEmptyString()
        {
            var decoder = new UTF8Decoder();
            var result = await decoder.Decode(null!);
            result.Should().BeEmpty();
        }
    }
}