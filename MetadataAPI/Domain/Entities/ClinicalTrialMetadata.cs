using System.ComponentModel.DataAnnotations;

namespace MetadataAPI.Domain.Entities
{
    public class ClinicalTrialMetadata
    {
        [Key]
        [MaxLength(100)]
        public string TrialId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public int Participants { get; set; } = 1;
        public string Status { get; set; } = string.Empty; // Status: "Not Started", "Ongoing", "Completed"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int DurationInDays { get; set; }
    }
}
