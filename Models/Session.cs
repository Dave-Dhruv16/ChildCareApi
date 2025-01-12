namespace ChildCareApi.Models
{
    public class Session
    {
        public int SessionId { get; set; }
        public int RequestId { get; set; }
        public int TutorId { get; set; }
        public int StudentId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int LocationId { get; set; } 
    }
}
