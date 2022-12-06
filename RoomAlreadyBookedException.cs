using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookings
{
    internal class RoomAlreadyBookedException : Exception
    {
        private DateTime date_;
        private int room_;

        public DateTime Date { get { return date_; } }
        public int Room { get { return room_; } }

        public RoomAlreadyBookedException(DateTime date, int room)
        {
            date_ = date;
            room_ = room;
        }
    }
}
