using System;
using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IoTSuite.Server.Extensions
{
    public class UnixToDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                // try to parse number directly from bytes
                ReadOnlySpan<byte> span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
                if (Utf8Parser.TryParse(span, out long number, out int bytesConsumed) && span.Length == bytesConsumed)
                    return DateTimeOffset.FromUnixTimeSeconds(number).DateTime;

                // try to parse from a string if the above failed, this covers cases with other escaped/UTF characters
                if (Int64.TryParse(reader.GetString(), out number))
                    return DateTimeOffset.FromUnixTimeSeconds(number).DateTime;
            }

            // fallback to default handling
            return reader.GetDateTime();
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(((DateTimeOffset)value).ToUnixTimeSeconds());
        }
    }
}
