namespace Backend.Helpers;

public static class Snowflake
{
    public static IConfiguration config;
    public static void Initialize(IConfiguration Configuration)
    {
        config = Configuration;
        machineId = GetMachineId();
    }
    
    // Snowflake ID components
    private const long Epoch = 1609459200000; // January 1, 2021 (UTC)
    private const int TimestampBits = 41;
    private const int MachineIdBits = 10;
    private const int SequenceBits = 12;

    private static long machineId;
    private static long sequence = 0;
    private static long lastTimestamp = -1;

    // Lock for thread safety
    private static readonly object lockObject = new object();

    public static long GenerateId()
    {
        lock (lockObject)
        {
            long timestamp = GetCurrentTimestamp();

            if (timestamp == lastTimestamp)
            {
                sequence = (sequence + 1) & ((1 << SequenceBits) - 1);

                if (sequence == 0)
                {
                    // Wait for the next millisecond
                    timestamp = WaitNextMillis(lastTimestamp);
                }
            }
            else
            {
                sequence = 0;
            }

            lastTimestamp = timestamp;

            // Combine components to generate the final snowflake ID
            long id = ((timestamp - Epoch) << (MachineIdBits + SequenceBits)) | (machineId << SequenceBits) | sequence;
            return id;
        }
    }

    private static long GetCurrentTimestamp()
    {
        return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
    }

    private static long WaitNextMillis(long lastTimestamp)
    {
        long timestamp = GetCurrentTimestamp();
        while (timestamp <= lastTimestamp)
        {
            timestamp = GetCurrentTimestamp();
        }
        return timestamp;
    }

    private static long GetMachineId()
    {
        return config.GetValue<long>("Snowflake:MachineId");
    }
}