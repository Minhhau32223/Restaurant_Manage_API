using System.Text.Json;
using System.Text.Json.Serialization;

namespace RestaurantAPI.src.Validator.Converters {
    public class DecimalFormatConverter : JsonConverter<decimal> {
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            return reader.GetDecimal();
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options) {
            // FIX 2 chữ số thập phân
            writer.WriteNumberValue(Math.Round(value, 2));
        }
    }
}
