using System.Buffers;
using System.IO.Pipelines;
using System.Text;

namespace MyAdmin.Core.Utilities;

public static class StreamHandler
{
    // todo 读取了两遍 
    /// <summary>
    /// 使用管道读取内容, 默认leaveOpen为true,使用之后需要手动关闭流
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static string  GetStringByPipe(this Stream stream)
    {
        if (stream.Position != 0 && stream.CanSeek)
        {
            stream.Seek(0, SeekOrigin.Begin);
        }
        var reader = PipeReader.Create(stream, new StreamPipeReaderOptions(leaveOpen:true));
        var sb = new StringBuilder();
        try
        {
            while (true)
            {
                ReadResult readResult = reader.ReadAsync().Result;
                ReadOnlySequence<byte> buffer = readResult.Buffer;

                if (buffer.Length > 0)
                {
                    sb.Append(Encoding.UTF8.GetString(buffer));
                }

                reader.AdvanceTo(buffer.Start, buffer.End);
                if (readResult.IsCompleted || readResult.IsCanceled)
                {
                    break;
                }
            }
        }
        finally
        {
            reader.Complete();
        }
        
        return sb.ToString();
    }

    public static string GetString(this Stream stream, bool autoCloseStream = true)
    {
        if (stream.Position != 0 && stream.CanSeek)
        {
            stream.Seek(0, SeekOrigin.Begin);
        }
        StreamReader reader = null;
        try
        {
            reader = new StreamReader(stream);
            if (autoCloseStream)
            {
                return reader.ReadToEnd();
            }
            else
            {
                // 手动处理读取，不自动关闭流
                string content = "";
                char[] buffer = new char[1024];
                int numRead;
                while ((numRead = reader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    content += new string(buffer, 0, numRead);
                }
                return content;
            }
        }
        finally
        {
            if (reader!= null && autoCloseStream)
            {
                reader.Dispose();
            }
        }
    }
}