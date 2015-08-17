public class StringTranscoder : ICacheTranscoder
{
    public object Decode(byte[] codeBuffer)
    {
        return System.Text.Encoding.Default.GetString(codeBuffer);
    }

    public byte[] Encode(object source)
    {
        return  System.Text.Encoding.UTF8.GetBytes(source as string);
    }
}