using System.Xml.Serialization;

namespace Currency.UserClasses
{
    /// <summary>
    /// Временный класс для заполнения данных по отдельной валюте в результате парсинга XML
    /// </summary>    
    [XmlRoot(ElementName = "ValCurs")]
    public class ValCurs
    {
        private Valute[]? _valutes;

        private string? _name;

        private string? _date;

        public ValCurs() { }

        [XmlAttribute(AttributeName = "Date")]
        public string? Date
        {
            get
            {
                return this._date;
            }
            set
            {
                this._date = value;
            }
        }

        [XmlAttribute(AttributeName = "name")]
        public string? name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }

        [XmlElement("Valute")]
        public Valute[] Valutes 
        {
            get
            {
                return this._valutes;
            }
            set 
            {
                this._valutes = value;
            } 
        }
    }
}
