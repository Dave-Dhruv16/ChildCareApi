namespace ChildCareApi.Models
{
    public class Tutor
    {
        public int TutorId { get; set; }
        public int UserId { get; set; }
        public string? Qualifications { get; set; }
        public string? Specialization { get; set; }
        public int? ExperienceYears { get; set; }
    }
}
