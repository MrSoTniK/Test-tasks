using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gridnine.FlightCodingTest
{
    class Program
    {        
        private static List<Flight> flights;

        static void Main(string[] args)
        {         
            FlightBuilder flightBuilder = new FlightBuilder();          
            flights = flightBuilder.GetFlights().ToList();
            bool isExit = false;
            string userInput;
            Dictionary<int, string> checkVariants = new Dictionary<int, string>();
            checkVariants.Add(1, "исключить вылеты до текущего момента времени");
            checkVariants.Add(2, "исключить сегменты с датой прилёта раньше даты вылета");
            checkVariants.Add(3, "исключить перелеты, общее время которых, проведённое на земле, превышает два часа");
            Console.BufferWidth = 100;
            Console.BufferHeight = 100;

            while (isExit == false) 
            {
                Console.Clear();
                Console.WriteLine("exit - выход ");               
                foreach(var chaeckVariant in checkVariants)
                {
                    Console.WriteLine(chaeckVariant.Key.ToString() + " - " + chaeckVariant.Value);
                }

                userInput = Console.ReadLine();
                Console.Clear();
                switch (userInput) 
                { 
                    case "1":
                        ExcludeByCurrentTime(); 
                        break;
                    case "2":
                        ExcludeByEarlierDate();
                        break;
                    case "3":
                        ExcludeByGeneralTime();
                        break;
                    case "exit":
                        isExit = true;
                        Console.WriteLine("Выход...");
                        break;
                    default:
                        Console.WriteLine("Ошибка. Такой команды не существует.");                      
                        break;
                }

                Console.WriteLine("Для продолжения нажмите любую клавишу...");
                Console.ReadKey();
            }         
        }

        private static void Show(List<Flight> flights) 
        {
            int count = 0;
            foreach (var flight in flights) 
            {
                count++;
                Console.WriteLine("Перелет " + count.ToString());
                flight.Show();              
            }
        }

        private static void ExcludeByCurrentTime() 
        {
            DateTime currentMomentOfTime = DateTime.Now;
            List<Flight> filteredFlights = new List<Flight>();

            foreach(var flight in flights)
            {
                var filter = flight.Segments.Where(segment => segment.DepartureDate <= currentMomentOfTime).ToList();
                if (filter.Count == 0) 
                {
                    filteredFlights.Add(flight);
                }
            }

            Show(filteredFlights);          
        }

        private static void ExcludeByEarlierDate() 
        {
            List<Flight> filteredFlights = new List<Flight>();
            foreach (var flight in flights)
            {
                var filter = flight.Segments.Where(segment => segment.ArrivalDate < segment.DepartureDate).ToList();
                if (filter.Count == 0)
                {
                    filteredFlights.Add(flight);
                }
            }

            Show(filteredFlights); 
        }

        private static void ExcludeByGeneralTime()
        {
            TimeSpan hoursSum = new TimeSpan(0, 0, 0, 0);
            List<Flight> filteredFlights = new List<Flight>();
            foreach (var flight in flights) 
            {
                if (flight.Segments.Count > 1) 
                {
                    hoursSum = flight.CalculateHoursSum();

                    if (hoursSum.Days == 0 && hoursSum.Hours <= 2)
                    {
                        filteredFlights.Add(flight);
                    }
                }
                else 
                {
                    filteredFlights.Add(flight);
                }
            }

            Show(filteredFlights); 
        }
    }

    public class FlightBuilder
    {
        private DateTime _threeDaysFromNow;

        public FlightBuilder()
        {
            _threeDaysFromNow = DateTime.Now.AddDays(3);
        }

        public IList<Flight> GetFlights()
        {
            return new List<Flight>
			           {
                           //A normal flight with two hour duration
			               CreateFlight(_threeDaysFromNow, _threeDaysFromNow.AddHours(2)),

                           //A normal multi segment flight
			               CreateFlight(_threeDaysFromNow, _threeDaysFromNow.AddHours(2), _threeDaysFromNow.AddHours(3), _threeDaysFromNow.AddHours(5)),
                           
                           //A flight departing in the past
                           CreateFlight(_threeDaysFromNow.AddDays(-6), _threeDaysFromNow),

                           //A flight that departs before it arrives
                           CreateFlight(_threeDaysFromNow, _threeDaysFromNow.AddHours(-6)),

                           //A flight with more than two hours ground time
                           CreateFlight(_threeDaysFromNow, _threeDaysFromNow.AddHours(2), _threeDaysFromNow.AddHours(5), _threeDaysFromNow.AddHours(6)),

                            //Another flight with more than two hours ground time
                           CreateFlight(_threeDaysFromNow, _threeDaysFromNow.AddHours(2), _threeDaysFromNow.AddHours(3), _threeDaysFromNow.AddHours(4), _threeDaysFromNow.AddHours(6), _threeDaysFromNow.AddHours(7))
			           };
        }

        private static Flight CreateFlight(params DateTime[] dates)
        {
            if (dates.Length % 2 != 0) throw new ArgumentException("You must pass an even number of dates,", "dates");

            var departureDates = dates.Where((date, index) => index % 2 == 0);
            var arrivalDates = dates.Where((date, index) => index % 2 == 1);

            var segments = departureDates.Zip(arrivalDates,
                                              (departureDate, arrivalDate) =>
                                              new Segment { DepartureDate = departureDate, ArrivalDate = arrivalDate }).ToList();

            return new Flight { Segments = segments };
        }
    }

    public class Flight
    {
        public IList<Segment> Segments { get; set; }

        public void Show() 
        { 
            int count = 0;
            foreach(var segment in Segments)
            {
                count++;
                Console.WriteLine("Сегмент " + count.ToString() + ":");
                segment.Show();               
            }
        }

        public TimeSpan CalculateHoursSum() 
        {
            TimeSpan hoursSum = new TimeSpan(0,0,0,0);          

            for (int i = 1; i < Segments.Count; i++)
            {
                hoursSum += Segments[i].DepartureDate.Subtract(Segments[i - 1].ArrivalDate);
            }

            return hoursSum;
        }
    }

    public class Segment
    {
        public DateTime DepartureDate { get; set; }
        public DateTime ArrivalDate { get; set; }

        public void Show() 
        {
            Console.WriteLine("Вылет: " + DepartureDate + " / Прилет: " + ArrivalDate);
        }
    }
}
