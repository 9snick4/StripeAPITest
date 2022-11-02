namespace StripeAPITest.Shared.Models
{
    public class BaseLookup
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class BaseLookupWithGuid
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
