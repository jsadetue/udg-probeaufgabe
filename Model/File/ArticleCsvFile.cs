using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdgChallenge.Model.File
{
    public class ArticleCsvFile : CsvFile
	{
		public override string[] Schema => new string[]
		{
			"Hauptartikelnr",
			"Artikelname",
			"Hersteller",
			"Beschreibung",
			"Materialangaben",
			"Geschlecht",
			"Produktart",
			"Ärmel",
			"Bein",
			"Kragen",
			"Herstellung",
			"Taschenart",
			"Grammatur",
			"Material",
			"Ursprungsland",
			"Bildname"
		};

		public ArticleCsvFile(string path) : base(path) { }
		public ArticleCsvFile() : base() { }
	}
}
