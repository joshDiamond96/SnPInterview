using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookings
{
    internal class InvalidRoomNumberException : Exception
    {
        private int room_;

        public int Room { get { return room_; } }

        public InvalidRoomNumberException(int room)
        {
            room_ = room;
        }

    }
}
