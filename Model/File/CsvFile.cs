using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdgChallenge.Model.File
{
    public class CsvFile : NativeFile
    {
        public ExtendedDataTable DataTable { get; } = new ExtendedDataTable();

        public virtual CsvConfiguration ReaderConfig { get; set; } = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            NewLine = Environment.NewLine,
            Delimiter = ";",
        };

        public virtual string[] Schema { get; } = null;

        public CsvFile(string path) : base(path) { }
        public CsvFile() : base() { }

        public override void Read(Stream stream)
        {
            var dataTable = DataTable;

            using (var streamReader = new StreamReader(stream))
            using (var csvReader = new CsvReader(streamReader, ReaderConfig))
            using (var csvDataReader = new CsvDataReader(csvReader))
            {
                // apparently the CsvDataReader constructor does csvReader.Read(), doing it again would cause us to read the wrong line as header
                csvReader.ReadHeader();

                if (Schema != null && !csvReader.HeaderRecord.SequenceEqual(Schema))
                {
                    throw new Exception("CSV header record does not match schema");
                }

                dataTable.Load(csvDataReader);
            }
        }

        public override void Write(Stream stream)
        {
            var dataTable = DataTable;

            using (var streamWriter = new StreamWriter(stream))
            {
                using (var csvWriter = new CsvWriter(streamWriter, ReaderConfig))
                {
                    foreach (DataColumn dataColumn in dataTable.Columns)
                    {
                        csvWriter.WriteField(dataColumn.ColumnName);
                    }

                    csvWriter.NextRecord();

                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        for (int i = 0; i < dataTable.Columns.Count; i++)
                        {
                            csvWriter.WriteField(dataRow[i]);
                        }

                        csvWriter.NextRecord();
                    }
                }
            }
        }

        public void Load(string path)
        {
            FullPath = path;

            using (var stream = System.IO.File.Open(FullPath, FileMode.Open, FileAccess.Read))
            {
                stream.Position = 0;

                Read(stream);
            }

            IsTemporary = false;
        }

        public void Save(string path)
        {
            FullPath = path;

            using (FileStream stream = System.IO.File.Open(FullPath, FileMode.Create, FileAccess.Write))
            {
                stream.Position = 0;
                stream.SetLength(0);

                Write(stream);
            }

            IsTemporary = false;
        }
    }
}
