using System.IO;
using Assets.Scripts.Core.Utils;


public interface  ICacheTranscoder
{
    object Decode(byte[] codeBuffer);
    byte[] Encode(object source);
}

public class CacheHeadKey
{
    public bool                     m_bIsNeedCompress;
    public ICacheTranscoder         m_Transcoder;

    public CacheHeadKey( ICacheTranscoder transcoder, bool isCompress = false)
    {
        m_bIsNeedCompress = isCompress;
        m_Transcoder = transcoder;
    }
    public CacheHeadInfo BuildCacheInfo(string name, string path = "")
    {
        return new CacheHeadInfo(this, name, path);
    }
}
public class CacheHeadInfo
{
    public CacheHeadInfo(CacheHeadKey key,string name, string path )
    {
        m_Key = key;
        m_strName = name;
        m_strPath = path;
    }
    public CacheHeadKey m_Key;
    public string       m_strPath;
    public string       m_strName;

}
public class CacheManager : Singleton<CacheManager>
{
    private string m_strSavePath;
    private string m_strDownloadPath = "DownloadPath/";
    private string m_strPersonalPath = "Personal/";

    public void Initialize(string savePath)
    {
        m_strSavePath = savePath;
        m_strDownloadPath = Path.Combine(m_strSavePath, m_strDownloadPath);
        m_strPersonalPath = Path.Combine(m_strSavePath, m_strPersonalPath);
    }
    public void Set(CacheHeadInfo headInfo,object source)
    {
        if (source == null)
        {
            return;
        }

        byte[] data = null;
        data = headInfo.m_Key.m_Transcoder.Encode(source);

        string path = Path.Combine(headInfo.m_strPath, headInfo.m_strName);
        path = Path.Combine(m_strPersonalPath, path);

        FileUtils.WriteByteFile(path, data);
    }
    public object Get(CacheHeadInfo headInfo)
    {
        string path = Path.Combine(headInfo.m_strPath, headInfo.m_strName);
        path = Path.Combine(m_strPersonalPath, path);
        byte[] data = FileUtils.ReadByteFile(path);
        object res = null;

        if (null != data)
        {
            res = headInfo.m_Key.m_Transcoder.Decode(data);
        }

        return res;
    }
}
