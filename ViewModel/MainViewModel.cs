using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using UdgChallenge.Model;
using UdgChallenge.Model.File;

namespace UdgChallenge.ViewModel
{
    public class MainViewModel : ViewModel
    {
        #region Fields
        private CsvFile m_CurrentFile;
        private IList m_SelectedItems = new ArrayList();
        private bool m_ShowVisualization = true;

        private readonly string[] PieChartColumnNames = { "Geschlecht", "Herstellung" };
        private readonly ObservableCollection<ExtendedSeriesCollection> m_PieSeriesCollections = new ObservableCollection<ExtendedSeriesCollection>();
        #endregion

        #region Properties
        public CsvFile CurrentFile
        {
            get => m_CurrentFile;
            set
            {
                m_CurrentFile = value;
                NotifyFileChanged();
            }
        }

        public bool HasCurrentFile { get => CurrentFile != null; }
        public bool IsCurrentFilePermanent { get => HasCurrentFile && !CurrentFile.IsTemporary; }

        public IList SelectedItems
        {
            get => m_SelectedItems;
            set
            {
                m_SelectedItems = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(HasSelection));
            }
        }

        public List<DataRow> SelectedRows
        {
            get
            {
                var rows = new List<DataRow>();
                foreach (var item in SelectedItems)
                {
                    var rowView = item as DataRowView;
                    var row = rowView.Row;
                    rows.Add(row);
                }
                return rows;
            }
        }

        public bool HasSelection { get => SelectedItems.Count > 0; }

        public ObservableCollection<DataColumn> DataColumns { get; set; } = new ObservableCollection<DataColumn>();

        public bool ShowVisualization
        {
            get => m_ShowVisualization;
            set
            {
                var show = value;
                if (show != m_ShowVisualization)
                {
                    m_ShowVisualization = show;
                    NotifyPropertyChanged();
                }
            }
        }

        // TO DO: auto-update charts based on binding data
        public ObservableCollection<ExtendedSeriesCollection> PieSeriesCollections
        {
            get
            {
                var dataTable = HasCurrentFile ? CurrentFile.DataTable : null;
                m_PieSeriesCollections.Clear();
                foreach (var columnName in PieChartColumnNames)
                {
                    var seriesCollection = GetPieSeriesCollection(dataTable, columnName);
                    m_PieSeriesCollections.Add(seriesCollection);
                }
                return m_PieSeriesCollections;
            }
        }

        public DelegateCommand OpenFileCommand { get; }
        public DelegateCommand NewFileCommand { get; }
        public DelegateCommand SaveFileCommand { get; }
        public DelegateCommand SaveFileAsCommand { get; }
        public DelegateCommand CloseFileCommand { get; }
        public DelegateCommand ExitApplicationCommand { get; }

        public DelegateCommand AddRowCommand { get; }
        public DelegateCommand DuplicateRowsCommand { get; }
        public DelegateCommand RemoveRowsCommand { get; }
        public DelegateCommand ClearRowsCommand { get; }
        public DelegateCommand ClearAllRowsCommand { get; }

        public DelegateCommand EditColumnsCommand { get; }
        #endregion

        public MainViewModel()
        {
            OpenFileCommand = new DelegateCommand((o) => OpenFile());
            NewFileCommand = new DelegateCommand((o) => NewFile());
            SaveFileCommand = new DelegateCommand((o) => SaveFile());
            SaveFileAsCommand = new DelegateCommand((o) => SaveFileAs());
            CloseFileCommand = new DelegateCommand((o) => CloseFile());
            ExitApplicationCommand = new DelegateCommand((o) => ExitApplication());

            AddRowCommand = new DelegateCommand((o) => AddRow());
            DuplicateRowsCommand = new DelegateCommand((o) => DuplicateRows());
            RemoveRowsCommand = new DelegateCommand((o) => RemoveRows());
            ClearRowsCommand = new DelegateCommand((o) => ClearRows());
            ClearAllRowsCommand = new DelegateCommand((o) => ClearAllRows());

            EditColumnsCommand = new DelegateCommand((o) => EditColumns());

            AddConverters();
        }

        private void AddConverters()
        {
            // fix empty DataColumn item names inside Collection Editor window
            TypeDescriptor.AddAttributes(typeof(DataColumn), new Attribute[]{ new TypeConverterAttribute() } );
        }

        private void NotifyFileChanged()
        {
            NotifyPropertyChanged(nameof(CurrentFile));
            NotifyPropertyChanged(nameof(HasCurrentFile));
            NotifyPropertyChanged(nameof(IsCurrentFilePermanent));

            NotifyPropertyChanged(nameof(PieSeriesCollections));
        }

        public void OpenFile()
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "CSV file|*.csv|All files|*.*";

            var dialogResult = dialog.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.FileName;

                if (!string.IsNullOrEmpty(path))
                {
                    DoOpenFile(path);
                }
            }
        }

        public void NewFile()
        {
            CurrentFile = new ArticleCsvFile();
        }

        public void SaveFile()
        {
            if (!IsCurrentFilePermanent)
            {
                SaveFileAs();
                return;
            }

            DoSaveFile(CurrentFile.FullPath);
        }

        public void SaveFileAs()
        {
            var dialog = new System.Windows.Forms.SaveFileDialog();
            dialog.Filter = "CSV file|*.csv|All files|*.*";

            var dialogResult = dialog.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.FileName;

                if (!string.IsNullOrEmpty(path))
                {
                    DoSaveFile(path);
                }
            }
        }

        public void CloseFile()
        {
            if (!IsCurrentFilePermanent)
            {
                DoDeleteFile(CurrentFile.FullPath);
            }
            CurrentFile = null;
        }

        public void ExitApplication()
        {
            Application.Current.Shutdown();
        }

        public void WindowClosing(object sender, CancelEventArgs e)
        {
            if (HasCurrentFile && CurrentFile.DataTable.HasChanges)
            {
                var messageBoxResult = MessageBox.Show("You have unsaved changes. Are you sure you want to exit?", "Exit", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                if (messageBoxResult == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private void DoOpenFile(string path)
        {
            if (HasCurrentFile)
            {
                CloseFile();
            }

            var csvFile = new ArticleCsvFile(path);
#if DEBUG
            csvFile.Load(csvFile.FullPath);
#else
            try
            {
                csvFile.Load(csvFile.FullPath);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Open file", MessageBoxButton.OK, MessageBoxImage.Error);
            }
#endif
            CurrentFile = csvFile;

            var dataTable = HasCurrentFile ? CurrentFile.DataTable : null;
            dataTable.AcceptChanges();
        }

        private void DoSaveFile(string path)
        {
            var dataTable = HasCurrentFile ? CurrentFile.DataTable : null;
            dataTable.AcceptChanges();
#if DEBUG
            CurrentFile.Save(path);
#else
            try
            {
                CurrentFile.Save(path);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Save file", MessageBoxButton.OK, MessageBoxImage.Error);
            }
#endif
        }

        private void DoDeleteFile(string path)
        {
            System.IO.File.Delete(path);
        }

        private void DoRemoveRow(DataRow dataRow)
        {
            var dataTable = HasCurrentFile ? CurrentFile.DataTable : null;
            dataTable.Rows.Remove(dataRow);
        }

        private void DoClearRow(DataRow dataRow)
        {
            var dataTable = HasCurrentFile ? CurrentFile.DataTable : null;

            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                var dataColumn = dataTable.Columns[i];
                var columnName = dataColumn.ColumnName;

                dataRow[columnName] = null;
            }
        }

        public void AddRow()
        {
            var dataTable = HasCurrentFile ? CurrentFile.DataTable : null;
            var dataRow = dataTable.NewRow();
            dataTable.Rows.Add(dataRow);

            var dataView = dataTable.DefaultView;

            var selectedItem = dataView[dataTable.Rows.IndexOf(dataRow)];

            var selectedItems = SelectedItems;
            selectedItems.Clear();
            selectedItems.Add(selectedItem);
        }

        public void DuplicateRows()
        {
            var dataTable = HasCurrentFile ? CurrentFile.DataTable : null;

            var duplicateDataRowDict = new Dictionary<int, DataRow>();

            foreach (DataRow selectedDataRow in SelectedRows)
            {
                var selectedRowIndex = dataTable.Rows.IndexOf(selectedDataRow);

                if (selectedRowIndex != -1)
                {
                    var duplicateDataRow = dataTable.NewRow();
                    var duplicateDataItemList = new ArrayList();

                    foreach (var item in selectedDataRow.ItemArray)
                    {
                        duplicateDataItemList.Add(item);
                    }

                    duplicateDataRow.ItemArray = duplicateDataItemList.ToArray();

                    duplicateDataRowDict.Add(selectedRowIndex, duplicateDataRow);
                }
            }

            foreach (var index in duplicateDataRowDict.Keys)
            {
                var dataRow = duplicateDataRowDict[index];

                dataTable.Rows.InsertAt(dataRow, index);
            }
        }

        public void RemoveRows() => SelectedRows.ForEach(DoRemoveRow);
        public void ClearRows() => SelectedRows.ForEach(DoClearRow);

        public void ClearAllRows()
        {
            var dataTable = HasCurrentFile ? CurrentFile.DataTable : null;
            dataTable.Rows.Clear();
        }

        public void EditColumns()
        {
            //// known broken and beyond project scope
            //var dataTable = HasCurrentFile ? CurrentFile.DataTable : null;

            //var collection = dataTable.Columns;

            //var dialog = new Xceed.Wpf.Toolkit.CollectionControlDialog(); // from Extended.Wpf.Toolkit
            //dialog.ItemsSource = collection;
            //dialog.ItemsSourceType = typeof(Collection<DataColumn>);
            //dialog.ShowDialog();
        }

        private static ExtendedSeriesCollection GetPieSeriesCollection(DataTable dataTable, string columnName)
        {
            var collection = new ExtendedSeriesCollection(columnName);

            if (dataTable != null)
            {
                var dict = new Dictionary<string, int>();

                foreach (DataRow dataRow in dataTable.Rows)
                {
                    var field = dataRow[columnName];
                    var name = field.ToString();

                    if (!string.IsNullOrEmpty(name))
                    {
                        if (dict.TryGetValue(name, out var _))
                        {
                            dict[name]++;
                        }
                        else
                        {
                            dict[name] = 1;
                        }
                    }
                }

                for (int i = 0; i < dict.Keys.Count; i++)
                {
                    var key = dict.Keys.ElementAt(i);
                    var value = dict[key];

                    var series = new PieSeries
                    {
                        Title = key,
                        Values = new ChartValues<int>() { value },
                        DataLabels = true
                    };

                    collection.Add(series);
                }

            }

            return collection;
        }
    }
}
