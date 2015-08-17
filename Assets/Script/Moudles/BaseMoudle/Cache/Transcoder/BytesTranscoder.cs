public class BytesTranscoder : ICacheTranscoder
{
    public object Decode(byte[] codeBuffer)
    {
        return codeBuffer;
    }

    public byte[] Encode(object source)
    {
        return (byte[])source;
    }
}