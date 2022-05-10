using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GameStore.Core.Extensions;

public static class ObjectExtensions
{
    public static byte[] ToByteArray(this object value)
    {
        var binaryFormatter = new BinaryFormatter();
        
        using var memoryStream = new MemoryStream();
        binaryFormatter.Serialize(memoryStream, value);
        
        return memoryStream.ToArray();
    }
}