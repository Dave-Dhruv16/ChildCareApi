namespace ChildCareApi.Models
{
    public class TutoringRequest
    {
        public int RequestId { get; set; }
        public int StudentId { get; set; }
        public string Subject { get; set; }
        public string? Description { get; set; }
        public DateTime RequestedAt { get; set; }
        public string Status { get; set; } = "Pending"; 
        public int? TutorId { get; set; }
        public DateTime? AcceptedAt { get; set; }
    }
}
