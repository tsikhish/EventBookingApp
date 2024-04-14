using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Tickets
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string EventName { get; set; }
        public int Count { get; set; }
        public int MaxBooking { get;set; }
        public ICollection<CreateEvent> CreateEvents { get; set; }
        public string AppUserId { get; set; }
    }
}
