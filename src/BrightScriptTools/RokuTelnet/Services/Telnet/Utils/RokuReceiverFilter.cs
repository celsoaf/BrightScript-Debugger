using System.Text;
using SuperSocket.ProtoBase;

namespace RokuTelnet.Services.Telnet.Utils
{
    public class RokuReceiverFilter: IReceiveFilter<StringPackageInfo>, IPackageResolver<StringPackageInfo>
    {
        private readonly Encoding m_Encoding;
        private readonly IStringParser m_StringParser;

        public RokuReceiverFilter(Encoding mEncoding, IStringParser mStringParser)
        {
            m_Encoding = mEncoding;
            m_StringParser = mStringParser;
        }

        public StringPackageInfo Filter(BufferList data, out int rest)
        {
            rest = 0;
            var bufferStream = this.GetBufferStream(data);

            return ResolvePackage(bufferStream);
        }

        public void Reset()
        {
            NextReceiveFilter = null;
            State = FilterState.Normal;
        }

        public IReceiveFilter<StringPackageInfo> NextReceiveFilter { get; private set; }
        public FilterState State { get; private set; }

        public StringPackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            var encoding = m_Encoding;
            var totalLen = (int)bufferStream.Length;
            return new StringPackageInfo(bufferStream.ReadString(totalLen, encoding), m_StringParser);
        }
    }
}