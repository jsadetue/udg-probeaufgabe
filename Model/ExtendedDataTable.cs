using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdgChallenge.Model
{
    public class ExtendedDataTable : DataTable
    {
        public bool HasChanges { get => GetChanges() != null; }

        public ExtendedDataTable() : base() { }
    }
}
