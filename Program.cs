using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Cars
{
    internal class Program
    {
        static void Main(string[] args)
        {
            InsertData();
            QueryData();
        }

        private static void QueryData()
        {
            var db = new CarDb();
            db.Database.Log = Console.WriteLine;

            var query =
                from car in db.Cars
                group car by car.Manufacturer into manufacturer
                select new
                {
                           Name = manufacturer.Key,
                           Cars = (from car in manufacturer
                                  orderby car.Combined descending
                                  select car).Take(2)
                };

            foreach(var group in query)
            {
                Console.WriteLine(group.Name);
                foreach (var car in group.Cars)
                {
                    Console.WriteLine($"\t{car.Name} : {car.Combined}");
                }
            }
        }

        private static void InsertData()
        {
            var cars = ProcessFile("fuel.csv");
            var db = new CarDb();

            if(!db.Cars.Any())
            {
                foreach (var car in cars)
                {
                    db.Cars.Add(car);
                }
                db.SaveChanges();
            }
        }

        private static List<Car> ProcessFile(string path)
        {
            var query =

                 File.ReadAllLines(path)
                     .Skip(1)
                     .Where(l => l.Length > 1)
                     .ToCar();
            
            return query.ToList();
        }

        private static List<Manufacturer> ProcessManufacturers(string path)
        {
            var query =

                 File.ReadAllLines(path)
                     .Where(l => l.Length > 1)
                     .Select(l =>
                     {
                         var columns = l.Split(',');
                         return new Manufacturer
                         {
                             Name = columns[0],
                             Headquarters = columns[1],
                             Year = int.Parse(columns[2])
                         };
                     });

            return query.ToList();
        }
    }
}
