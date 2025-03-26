using System.Text.Json.Serialization;

namespace foodOrderingApp.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] // ✅ Hide if null
        public T? Data { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] // ✅ Hide if null
        public string? Message { get; set; }

        public ApiResponse(bool success, T? data = default, string? message = null)
        {
            Success = success;
            Data = data;
            Message = message;
        }
    }

    public class ApiResponse
    {
        public bool Success { get; set; }

        // [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] // ✅ Hide if null
        public string? Message { get; set; }

        public ApiResponse(bool success, string message )
        {
            Success = success;
            Message = message;
        }
    }
}
