using System.Text;

namespace JobTracker.Api.Utilities;

public static class Converter
{
    private static readonly string _base64Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
    private static readonly int[] _base64Position = new int[256];

    static Converter()
    {
        for(int i = 0; i < 64; i++)
        {
            _base64Position[_base64Alphabet[i]] = i;
        }
    }
    
    public static string Base64Encode(byte[] bytes)
    {
        int length = bytes.Length;

        StringBuilder result = new StringBuilder();

        for(int i = 0; i < length; i += 3)
        {
            int remaining = length - i;

            byte byte1 = bytes[i];
            byte byte2 = remaining >= 2 ? bytes[i+1] : (byte) 0;
            byte byte3 = remaining >= 3 ? bytes[i+2] : (byte) 0;

            int n = 0;

            n = n << 8 | byte1;
            n = n << 8 | byte2;
            n = n << 8 | byte3;

            int block1 = n >> 18 & 0b111111;
            int block2 = n >> 12 & 0b111111;
            int block3 = n >> 6  & 0b111111;
            int block4 = n >> 0  & 0b111111;

            result.Append(_base64Alphabet[block1]);
            result.Append(_base64Alphabet[block2]);
            result.Append(remaining >= 2 ? _base64Alphabet[block3] : '=');
            result.Append(remaining >= 3 ? _base64Alphabet[block4] : '=');
        }

        return result.ToString();
    }

    public static byte[] Base64Decode(string s)
    {
        int length = s.Length;

        List<byte> result = new List<byte>();

        for(int i = 0; i < length; i += 4)
        {
            char char1 = s[i];
            char char2 = s[i+1];
            char char3 = s[i+2];
            char char4 = s[i+3];

            int value1 = _base64Position[char1];
            int value2 = _base64Position[char2];
            int value3 = _base64Position[char3];
            int value4 = _base64Position[char4];

            int n = 0;

            n = n << 6 | value1;
            n = n << 6 | value2;
            n = n << 6 | value3;
            n = n << 6 | value4;

            byte block1 = (byte) (n >> 16 & 0xFF);
            byte block2 = (byte) (n >> 8  & 0xFF);
            byte block3 = (byte) (n >> 0  & 0xFF);

            result.Add(block1);
            
            if(char3 != '=')
            {
                result.Add(block2);
            }
            if(char4 != '=')
            {
                result.Add(block3);
            }
        }

        return result.ToArray();
    }

    public static string Base64UrlEncode(byte[] bytes)
    {
        string base64 = Base64Encode(bytes);

        string base64Url = base64.Replace('+', '-').Replace('/', '_').TrimEnd('=');

        return base64Url;
    }

    public static byte[] Base64UrlDecode(string s)
    {
        while(s.Length % 4 != 0)
        {
            s += '=';
        }

        s = s.Replace('-', '+').Replace('_', '/');

        return Base64Decode(s);
    }
}