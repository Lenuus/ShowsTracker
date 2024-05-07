namespace ShowsTracker.Application.Service.Show.Dtos
{
    public class ShowListLinkDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Link { get; set; }

        public bool IsDefault { get; set; }
    }
}