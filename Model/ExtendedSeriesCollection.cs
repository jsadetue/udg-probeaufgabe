using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdgChallenge.Model
{
    public class ExtendedSeriesCollection : SeriesCollection
    {
        public string Name { get; set; }

        public ExtendedSeriesCollection(string name) : base() => Name = name;
    }
}
