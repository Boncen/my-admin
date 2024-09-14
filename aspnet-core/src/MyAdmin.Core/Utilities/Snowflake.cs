namespace MyAdmin.Core.Utilities;

public class Snowflake
{
    private const long EPOCH = 1514736000000L; // 假设以2018年1月1日作为纪元时间
    private long workerId;

    private long sequence;
    public Snowflake(long workerId)
    {
        if (workerId > 1023 || workerId < 0)
            throw new ArgumentException("Worker Id can't be greater than 1023 or less than 0");
        this.workerId = workerId;
        this.sequence = 0;
    }
    public long GenerateId()
    {
        long timestamp = GetCurrentTimestamp();

        // 如果时间向后回拨，则抛出异常
        if (timestamp < LastTimestamp && LastTimestamp != 0)
            throw new InvalidOperationException("Clock moved backwards. Refusing to generate id.");

        // 如果时间戳没有变化，则在同一毫秒内生成的ID需要使用sequence
        if (timestamp == LastTimestamp)
        {
            sequence = (sequence + 1) & 0xFFF; // 12位的序列号
            if (sequence == 0)
                timestamp = WaitNextMillisecond(timestamp);
        }
        else
        {
            sequence = 0;
        }

        LastTimestamp = timestamp;
        return ((timestamp - EPOCH) << 22) // 时间戳部分
               | (workerId << 12) // 机器ID部分
               | sequence; // 序列号部分
    }


    private long GetCurrentTimestamp()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    private long WaitNextMillisecond(long lastTimestamp)
    {
        while (GetCurrentTimestamp() <= lastTimestamp)
            System.Threading.Thread.Yield();
        return GetCurrentTimestamp();
    }
    [ThreadStatic]
    private static long LastTimestamp;
}