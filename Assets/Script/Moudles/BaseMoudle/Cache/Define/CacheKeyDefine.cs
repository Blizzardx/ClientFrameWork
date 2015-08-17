using UnityEngine;
using System.Collections;

public class CacheKeyDefine
{
    public static readonly CacheHeadKey ByteKey = new CacheHeadKey(new BytesTranscoder());
    public static readonly CacheHeadKey TextKey = new CacheHeadKey(new StringTranscoder());
}
