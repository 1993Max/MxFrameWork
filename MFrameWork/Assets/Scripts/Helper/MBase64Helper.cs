using System;
using System.Text;

public static class MBase64Helper
{
    /// <summary>
    /// Base64加密，采用utf8编码方式加密
    /// </summary>
    /// <param name="source">待加密的明文</param>
    /// <returns>加密后的字符串</returns>
    public static string EncodeBase64(string source)
    {
        return EncodeBase64(Encoding.UTF8, source);
    }

    /// <summary>
    /// Base64加密
    /// </summary>
    /// <param name="encoding">加密采用的编码方式</param>
    /// <param name="source">待加密的明文</param>
    /// <returns></returns>
    public static string EncodeBase64(Encoding encoding, string source)
    {
        string result = string.Empty;
        byte[] bytes = encoding.GetBytes(source);
        try
        {
            result = Convert.ToBase64String(bytes);
        }
        catch
        {
            result = source;
        }
        return result;
    }

    /// <summary>
    /// Base64解密，采用utf8编码方式解密
    /// </summary>
    /// <param name="source">待解密的密文</param>
    /// <returns>解密后的字符串</returns>
    public static string DecodeBase64(string source)
    {
        return DecodeBase64(Encoding.UTF8, source);
    }

    /// <summary>
    /// Base64解密
    /// </summary>
    /// <param name="encoding">解密采用的编码方式，注意和加密时采用的方式一致</param>
    /// <param name="source">待解密的密文</param>
    /// <returns>解密后的字符串</returns>
    public static string DecodeBase64(Encoding encoding, string source)
    {
        string result = string.Empty;
        byte[] bytes = Convert.FromBase64String(source);
        try
        {
            result = encoding.GetString(bytes);
        }
        catch
        {
            result = source;
        }
        return result;
    }
}
