using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IptSimulator.Client.Model
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
