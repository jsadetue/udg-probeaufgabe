using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UdgChallenge.Control
{
    public class ExtendedDataGrid : DataGrid
    {
        public new IList SelectedItems
        {
            get { return (IList)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
            nameof(SelectedItems),
            typeof(IList),
            typeof(ExtendedDataGrid),
            new PropertyMetadata(null));

        public ExtendedDataGrid()
        {
            SelectionChanged += ExtendedDataGrid_SelectionChanged;
        }

        private void ExtendedDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedItems = base.SelectedItems;
        }
    }
}
