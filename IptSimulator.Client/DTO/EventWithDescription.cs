namespace IptSimulator.Client.DTO
{
    public class EventWithDescription
    {
        public EventWithDescription(string @event, string description)
        {
            Event = @event;
            Description = description;
        }

        public string Event { get; set; }
        public string Description { get; set; }
    }
}
