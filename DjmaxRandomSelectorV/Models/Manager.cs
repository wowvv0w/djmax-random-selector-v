using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.Models
{
    public class Manager
    {
        public static List<Track> ReadTrackList()
        {
            string userData = @"C:\Projects\DjmaxRandomSelectorV\DjmaxRandomSelectorV\DataFiles\test_data.csv";

            using (var reader = new StreamReader(userData, Encoding.UTF8))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<MusicMap>();
                var records = csv.GetRecords<Track>().ToList();
                return records;
            }
        }


        public static void UpdateTrackList()
        {

        }
    }
}
