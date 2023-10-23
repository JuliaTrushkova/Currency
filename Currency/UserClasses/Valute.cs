using System.Xml.Serialization;

namespace Currency.UserClasses
{
    /// <summary>
    /// Временный класс для заполнения данных по всему запросу (всем валютам) в результате парсинга XML
    /// </summary>
    public class Valute
    {
        private string? _id;
        private string? _value;
        private string? _vunitRate;
        private short? _numCode;
        private string? _charCode;
        private string? _name;
        private int? _nominal;

        public Valute() { }

        [XmlAttribute(AttributeName = "ID")]
        public string ID
        {
            get
            {
                return this._id;
            }
            set
            {
                this._id = value;
            }
        }

        public string? Value
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
            }
        }

        public string? VunitRate
        {
            get
            {
                return this._vunitRate;
            }
            set
            {
                this._vunitRate = value;
            }
        }

        public short? NumCode
        {
            get
            {
                return this._numCode;
            }
            set
            {
                this._numCode = value;
            }
        }

        public string? CharCode
        {
            get
            {
                return this._charCode;
            }
            set
            {
                this._charCode = value;
            }
        }

        public string? Name
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

        public int? Nominal
        {
            get
            {
                return this._nominal;
            }
            set
            {
                this._nominal = value;
            }
        }

    }
}
