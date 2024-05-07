namespace ShowsTracker.Application.Service.Voting.Dtos
{
    public class VoteSeasonDetailShowDto
    {
        public Guid Id { get; set; }

        public Guid ShowId { get; set; }

        public string Name { get; set; }

        public bool IsWinner { get; set; }

        public int DisplayOrder { get; set; }

        public long TotalVote { get; set; }

        public string CoverImageUrl { get; set; }

        public bool SelectedByCurrentUser { get; set; }
    }
}