using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookings
{

    internal class Booking
    {
        private int room_;
        private DateTime date_;
        private string guest_;

        public Booking(int room, DateTime date, string guest)
        {
            room_ = room;
            date_ = date;
            guest_ = guest;
        }
    }

}
