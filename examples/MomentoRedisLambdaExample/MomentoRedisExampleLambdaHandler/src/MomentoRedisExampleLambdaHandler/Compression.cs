using System.IO.Compression;
using System.Text;

namespace MomentoRedisExampleLambdaHandler;

public static class Compression
{
    public static string Decompress(byte[] value)
    {
        using var gzipStream = new GZipStream(new MemoryStream(value), CompressionMode.Decompress);
        using (StreamReader reader = new StreamReader(gzipStream))
        {
            return reader.ReadToEnd();
        }
    }

    public static byte[] Compress(string value)
    {
        using var stream = new MemoryStream();
        //using var gzipStream = new GZipStream(stream, CompressionLevel.SmallestSize);
        using var gzipStream = new GZipStream(stream, CompressionLevel.Fastest);
        var buffer = Encoding.UTF8.GetBytes(value);
        gzipStream.Write(buffer, 0, buffer.Length);
        gzipStream.Flush();
        stream.Seek(0, SeekOrigin.Begin);
        return stream.ToArray();
    }
}

