using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DrrrAsync
{
    /* https://stackoverflow.com/questions/59097784/system-text-json-deserialize-json-with-automatic-casting
    Some of of Drrr's API behaves strangely when a string value is composed of numbers, so we include this
    */
    public class AutoNumberToStringConverter : JsonConverter<object>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(string) == typeToConvert;
        }
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if(reader.TokenType == JsonTokenType.Number) {
                return reader.TryGetInt64(out long l) ?
                    l.ToString():
                    reader.GetDouble().ToString();
            }
            if(reader.TokenType == JsonTokenType.String) {
                return reader.GetString();
            }
            using(JsonDocument document = JsonDocument.ParseValue(ref reader)){
                return document.RootElement.Clone().ToString();
            }
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            writer.WriteStringValue( value.ToString());
        }
    }
}
