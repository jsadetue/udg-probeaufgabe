using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdgChallenge.Model
{
    public class ArticleData
    {
        [Index(0)] public string? Hauptartikelnr { get; set; }
        [Index(1)] public string? Artikelname { get; set; }
        [Index(2)] public string? Hersteller { get; set; }
        [Index(3)] public string? Beschreibung { get; set; }
        [Index(4)] public string? Materialangaben { get; set; }
        [Index(5)] public string? Geschlecht { get; set; }
        [Index(6)] public string? Produktart { get; set; }
        [Index(7)] public string? Ärmel { get; set; }
        [Index(8)] public string? Bein { get; set; }
        [Index(9)] public string? Kragen { get; set; }
        [Index(10)] public string? Herstellung { get; set; }
        [Index(11)] public string? Taschenart { get; set; }
        [Index(12)] public string? Grammatur { get; set; }
        [Index(13)] public string? Material { get; set; }
        [Index(14)] public string? Ursprungsland { get; set; }
        [Index(15)] public string? Bildname { get; set; }
    }
}
