using System;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EnemyEditor
{
    public class BigNumberConverter : JsonConverter<BigNumber>
    {
        public override BigNumber Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt64(out long longValue))
                {
                    return new BigNumber(longValue);
                }
            }
            else if (reader.TokenType == JsonTokenType.String)
            {
                var stringValue = reader.GetString();
                if (BigInteger.TryParse(stringValue, out BigInteger bigIntValue))
                {
                    return new BigNumber(bigIntValue);
                }
            }

            return new BigNumber(0);
        }

        public override void Write(Utf8JsonWriter writer, BigNumber value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}