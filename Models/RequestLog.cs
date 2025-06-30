namespace UserManagementAPI.Models
{
    public class RequestLog
    {
        public int Id { get; set; }
        public string Path { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
