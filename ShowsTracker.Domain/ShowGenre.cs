using System.ComponentModel.DataAnnotations;

namespace ShowsTracker.Domain
{
    public class ShowGenre : IBaseEntity, ISoftDeletable
    {
        public Guid Id { get; set; }

        [Required]
        public Guid ShowId { get; set; }

        public Show Show { get; set; }

        [Required]
        public Guid GenreId { get; set; }

        public Genre Genre { get; set; }

        public bool IsDeleted { get; set; }
    }
}