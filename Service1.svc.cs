using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace OOP_Projekt6_WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        readonly DataClasses1DataContext db = new DataClasses1DataContext(); //Establishes connection to the Divadlo database

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        /// <summary>
        /// This method is used to add a new show to the programme. It creates an entry in the Show table with date, time and name and creates 80 entries representing seats in the Seating table. The default state of the seat is false as in free to be reserved/purchased.
        /// 
        /// It also calls CheckOldEntries() right before it returns to shrink the tables.
        /// </summary>
        /// <param name="dateTime">Date and time, when the show takes place.</param>
        /// <param name="name">Name of the show.</param>
        /// <returns>False if there is a show on the same date and time.
        /// True, if the operation was a success.</returns>
        public bool AddShow(DateTime dateTime, string name)
        {
            Show show = new Show() { DateTime = dateTime, Name = name};

            //Returns false if there already is a show on the same date and time
            if (db.Shows.Any(a => a.DateTime.Equals(show.DateTime)))
            {
                CheckOldEntries();
                return false;
            }

            //Inserts show in the Show table
            db.Shows.InsertOnSubmit(show);

            db.SubmitChanges();

            //Creates 80 seats with default State as false (empty, able to be reserved)
            List<Seating> seatings = new List<Seating>();
            for (int seat = 1; seat <= 80; seat++)
            {
                seatings.Add(new Seating() { DateTime = dateTime, Seat = seat, State = false });
            }

            //Inserts those seats into the Seating table
            db.Seatings.InsertAllOnSubmit(seatings);

            db.SubmitChanges();

            CheckOldEntries();
            return true;
        }

        /// <summary>
        /// This method is used to retrieve seating information about a show taking place at the date and time passed as the parameter.
        /// The key is the number of a seat and the value is the state (true - reserved, false - empty).
        /// </summary>
        /// <param name="dateTime">Date and time, when the show takes place.</param>
        /// <returns>Returns empty Dictionary<int, bool?>, if there is no show on chosen date and time. Otherwise returns as stated before.</returns>
        public Dictionary<int, bool?> GetSeatingInfo(DateTime dateTime)
        {
            Dictionary<int, bool?> seatingInfo = new Dictionary<int, bool?>();

            //Returns empty Dictionary<int, bool?> if there is no show at selected date and time.
            if (!db.Seatings.Any(a => a.DateTime.Equals(dateTime)))
            {
                return seatingInfo;
            }

            var seatings =
                from seats in db.Seatings
                where seats.DateTime == dateTime
                select seats;

            foreach (var seat in seatings)
            {
                seatingInfo.Add(seat.Seat, seat.State);
            }

            return seatingInfo;
        }

        /// <summary>
        /// This method returns all available shows.
        /// </summary>
        /// <returns>A Dictionary<Datetime, string> where the key is date and time when show takes place and value is the name of the show.</returns>
        public Dictionary<DateTime, string> GetShows()
        {
            Dictionary<DateTime, string> shows = new Dictionary<DateTime, string>();

            var availableshows =
                from shws in db.Shows
                select shws;

            foreach (var show in availableshows)
            {
                shows.Add(show.DateTime, show.Name);
            }

            return shows;
        }

        /// <summary>
        /// This method returns all available shows by selected name.
        /// </summary>
        /// <param name="name">Name of the show.</param>
        /// <returns>A Dictionary<Datetime, string> where the key is date and time when show takes place and value is the name of the show.</returns>
        public Dictionary<DateTime, string> GetShowsByName(string name)
        {
            Dictionary<DateTime, string> shows = new Dictionary<DateTime, string>();

            var showname =
                from show in db.Shows
                where show.Name.Equals(name)
                select show;

            foreach (var show in showname)
            {
                shows.Add(show.DateTime, show.Name);
            }

            return shows;
        }

        /// <summary>
        /// This methods returns all available shows by selected date.
        /// </summary>
        /// <param name="date">Date, when the shows take place.</param>
        /// <returns>A Dictionary<Datetime, string> where the key is date and time when show takes place and value is the name of the show.</returns>
        public Dictionary<DateTime, string> GetShowsByDate(DateTime date)
        {
            Dictionary<DateTime, string> shows = new Dictionary<DateTime, string>();

            var showname =
                from show in db.Shows
                where show.DateTime.Date.Equals(date.Date)
                select show;

            foreach (var show in showname)
            {
                shows.Add(show.DateTime, show.Name);
            }

            return shows;
        }

        /// <summary>
        /// This method is used to reserve a specific seat determined by the datetime and seat.
        /// </summary>
        /// <param name="dateTime">Date and time, when the show takes place.</param>
        /// <param name="seat">Number of the seat.</param>
        /// <returns>False if there is no show on chosen date and time or if the seat doesnt exist. Otherwise returns true.</returns>
        public bool MakeReservation(DateTime dateTime, int seat)
        {
            if (!db.Seatings.Any(a => a.DateTime.Equals(dateTime) && a.Seat == seat))
            {
                return false;
            }
            Seating seating = db.Seatings.Single(a => a.DateTime == dateTime && a.Seat == seat);
            seating.State = true;

            db.SubmitChanges();

            return true;
        }

        /// <summary>
        /// This method is used to cancel a reservation on a specific seat determined by the datetime and seat.
        /// </summary>
        /// <param name="dateTime">Date and time, when the show takes place.</param>
        /// <param name="seat">Number of the seat.</param>
        /// <returns>False if there is no show on chosen date and time or if the seat doesnt exist. Otherwise returns true.</returns>
        public bool CancelReservation(DateTime dateTime, int seat)
        {
            if (!db.Seatings.Any(a => a.DateTime.Equals(dateTime) && a.Seat == seat))
            {
                return false;
            }
            Seating seating = db.Seatings.Single(a => a.DateTime == dateTime && a.Seat == seat);
            seating.State = false;

            db.SubmitChanges();

            return true;
        }

        private void CheckOldEntries()
        {
            DateTime today = DateTime.Today;
            DateTime monthAgo = today.AddMonths(-1);

            //Returns if there are no shows older than one month
            if (!(db.Shows.Any(a => a.DateTime < monthAgo) || (db.Seatings.Any(a => a.DateTime < monthAgo))))
            {
                return;
            }

            var oldShows =
                from shows in db.Shows
                where shows.DateTime < monthAgo
                select shows;

            var oldSeatings =
                from seatings in db.Seatings
                where seatings.DateTime < monthAgo
                select seatings;

            db.Shows.DeleteAllOnSubmit(oldShows);
            db.Seatings.DeleteAllOnSubmit(oldSeatings);

            db.SubmitChanges();
        }
    }
}