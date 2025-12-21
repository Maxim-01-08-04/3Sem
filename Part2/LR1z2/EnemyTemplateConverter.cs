using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EnemyEditor
{
    public class EnemyTemplateConverter : JsonConverter<CEnemyTemplate>
    {
        public override CEnemyTemplate Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            JsonElement root = document.RootElement;

            if (!root.TryGetProperty("$type", out JsonElement typeElement))
                return new CNormalEnemyTemplate();

            string typeName = typeElement.GetString();

            try
            {
                string json = root.GetRawText();
                return typeName switch
                {
                    "CNormalEnemyTemplate" => JsonSerializer.Deserialize<CNormalEnemyTemplate>(json, options),
                    "CArmoredEnemyTemplate" => JsonSerializer.Deserialize<CArmoredEnemyTemplate>(json, options),
                    "CShrinkingEnemyTemplate" => JsonSerializer.Deserialize<CShrinkingEnemyTemplate>(json, options),
                    "CHealingEnemyTemplate" => JsonSerializer.Deserialize<CHealingEnemyTemplate>(json, options),
                    _ => throw new NotSupportedException($"Unknown enemy type: {typeName}")
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deserializing enemy type {typeName}: {ex.Message}");
                return new CNormalEnemyTemplate();
            }
        }

        public override void Write(Utf8JsonWriter writer, CEnemyTemplate value, JsonSerializerOptions options)
        {
            string type = value.GetType().Name;
            string json = JsonSerializer.Serialize(value, value.GetType(), options);

            using JsonDocument jsonDoc = JsonDocument.Parse(json);

            writer.WriteStartObject();
            writer.WriteString("$type", type);

            foreach (var property in jsonDoc.RootElement.EnumerateObject())
            {
                property.WriteTo(writer);
            }

            writer.WriteEndObject();
        }
    }
}