using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class CreateEvent
    {
        public int Id { get; set; }
        public string EventName {  get; set; }
        public string Location { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public int MaxBooking {  get; set; }
        public Tickets Tickeets { get; set; }
    }
}
