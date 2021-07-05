using System;
using System.Linq;


namespace WeatherLab
{
    class Program
    {
        static string dbfile = @".\data\climate.db";

        static void Main(string[] args)
        {
            var measurements = new WeatherSqliteContext(dbfile).Weather;

            var total_2020_precipitation = (from data in measurements
                                            where data.year == 2020
                                            select data.precipitation).Sum();
            Console.WriteLine($"Total precipitation in 2020: {total_2020_precipitation} mm\n");

            var mintemp = from minYear in measurements
                          group minYear by minYear.year into res
                          orderby res.Key
                          select new
                          {
                              Key = res.Key,
                              heating_d_d = res.Where(row => row.mintemp < 18).Count(),
                              cooling_d_d = res.Where(row => row.mintemp >= 18).Count(),
                          };

            //
            // Heating Degree days have a mean temp of < 18C
            //   see: https://en.wikipedia.org/wiki/Heating_degree_day
            //

            // ?? TODO ??

            //
            // Cooling degree days have a mean temp of >=18C
            //

            // ?? TODO ??

            //
            // Most Variable days are the days with the biggest temperature
            // range. That is, the largest difference between the maximum and
            // minimum temperature
            //
            // Oh: and number formatting to zero pad.
            // 
            // For example, if you want:
            //      var x = 2;
            // To display as "0002" then:
            //      $"{x:d4}"
            //
            Console.WriteLine("Year\tHDD\tCDD");

            foreach(var minYear in mintemp)
            {
                Console.WriteLine($"{minYear.Key}\t{minYear.heating_d_d}\t{minYear.cooling_d_d}");
            };

            var value = from temp in measurements
                       orderby (temp.maxtemp - temp.mintemp) descending
                       select new
                       {
                           date = $"{temp.year}-{temp.month:d2}-{temp.day:d2}",
                           d_temp = (temp.maxtemp - temp.mintemp),
                       };

            Console.WriteLine("\nTop 5 Most Variable Days");
            Console.WriteLine("YYYY-MM-DD\tDelta");

            int a = 0;
            foreach(var b in value)
            {
                if(a < 5)
                {
                    Console.WriteLine($"{b.date}\t{b.d_temp}");
                    a++;
                }
                else
                {
                    break;
                }
            }
        }
    }
}
