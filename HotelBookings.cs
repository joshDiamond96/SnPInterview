using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HotelBookings
{
    // Implementation of IBookingManager
    public class BookingManager : IBookingManager
    {
        private readonly object bookingLock_ = new object();
        private Dictionary<DateTime, Dictionary<int, Booking>> dateDict_ = new Dictionary<DateTime, Dictionary<int, Booking>>(); // dictionary to lookup booking by date first then room
        private HashSet<int> rooms_ = new HashSet<int> { 100, 101, 102, 103 };

        static void Main(string[] args)
        {
            try
            {
                // test harness for HotelBookings class
                IBookingManager bm = new BookingManager();// create Booking Manager

                DateTime today = new DateTime(2022, 12, 6);
                Console.WriteLine(bm.IsRoomAvailable(101, today)); // outputs true 
                bm.AddBooking("Patel", 101, today);

                IEnumerable<int> rooms = bm.getAvailableRooms(today);
                foreach (int room in rooms)
                {
                    Console.WriteLine("Room {0} is booked on {1:d}", room, today);
                }

                Console.WriteLine(bm.IsRoomAvailable(101, today)); // outputs false 
                bm.AddBooking("Li", 101, today); // throws an exception

            }
            catch (InvalidRoomNumberException ex)
            {
                Console.WriteLine(String.Format("Invalid room number {0}", ex.Room));
            }
            catch (RoomAlreadyBookedException ex)
            {
                Console.WriteLine(String.Format("Room {0} is already booked on date {1:d}", ex.Room, ex.Date));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }

        public bool IsRoomAvailable(int room, DateTime date)
        {
            lock (bookingLock_)
            {

                // check if room is an allowed room number
                if (!rooms_.Contains(room))
                {
                    throw new InvalidRoomNumberException(room);
                }

                // Note that this implementation only provides a way to tell if a room is available on a particular date as that's
                // all the question asks for. Being able to tell which dates a room is booked for would require an additional
                // dictionary

                // note that code would generate compiler warnings that bookingsByRoom is nullable and might be null (two separate warnings),
                // but I tell the compiler than bookingsByRoom is nullable (by adding the ? after the type) and I also check the result
                // of TryGetValue and explicitly check that bookingsByRoom is not null.
                if (dateDict_.TryGetValue(date, out Dictionary<int, Booking>? bookingsByRoom))
                {
                    // date has bookings, so check if the requested room is available
                    return !bookingsByRoom.ContainsKey(room);
                }
                return true;
            }
        }

        public void AddBooking(string guest, int room, DateTime date)
        {
            lock (bookingLock_)
            {
                if (!rooms_.Contains(room))
                {
                    throw new InvalidRoomNumberException(room);
                }

                // create new booking
                Booking booking = new Booking(room, date, guest);

                // it's possible that since we checked if the room was available that someone else has booked it,
                // so look date and room up again and only book it if it's still available
                if (dateDict_.TryGetValue(date, out Dictionary<int, Booking>? bookingsByRoom) && bookingsByRoom != null)
                {
                    // the date already has bookings
                    // is it booked for our room?
                    if (!bookingsByRoom.ContainsKey(room))
                    {
                        // no - add booking
                        bookingsByRoom.Add(room, booking);
                    }
                    else
                    {
                        // room is already booked on this date
                        throw new RoomAlreadyBookedException(date, room);
                    }
                }
                else
                {
                    // there are no bookings for that date, so book the room on that date
                    Dictionary<int, Booking> bookings = new Dictionary<int, Booking>();
                    bookings.Add(room, booking);
                    dateDict_.Add(date, bookings);
                }
            }
        }

        public IEnumerable<int> getAvailableRooms(DateTime date)
        {
            lock (bookingLock_)
            {
                List<int> bookings = new List<int>();
                if (dateDict_.TryGetValue(date, out Dictionary<int, Booking>? bookingsByRoom))
                {
                    foreach (KeyValuePair<int, Booking> entry in bookingsByRoom)
                    {
                        bookings.Add(entry.Key);
                    }

                }
                return bookings;
            }
        }
    }
}
