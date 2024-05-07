using System.ComponentModel.DataAnnotations;

namespace ShowsTracker.Domain
{
    public class Genre : IBaseEntity, ISoftDeletable
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(512)]
        public string Name { get; set; }

        public ICollection<ShowGenre> Shows { get; set; }

        public int? MyAnimeListId { get; set; }

        public bool IsDeleted { get; set; }
    }
}