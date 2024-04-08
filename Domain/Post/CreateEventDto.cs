using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Post
{
    public class CreateEventDto
    {
        public string EventName { get; set; }
        public int MaxBooking {  get; set; }
        public string Location { get; set; }

    }
}
