using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Xml.Serialization;
using Currency.Models;

namespace Currency.UserClasses
{
    /// <summary>
    /// Класс для парсинга XML в экземпляр класса CurrencyRateRequest 
    /// </summary> 
    internal class XMLToCurrencyParser
    {
        //Ссылка на ресурс ЦБ
        private static readonly string urlCB = "https://cbr.ru/scripts/XML_daily.asp?date_req=";
        static readonly HttpClient client = new HttpClient();

        /// <summary>        
        /// Парсинг XML файла с сайта ЦБ в экземпляр класса CurrencyRateRequest для указанной даты (dateOfRequest).
        /// Сначала запрашивается файл XML, затем полученный файл парсится в экземпляр класса CurrencyRateRequest
        /// </summary>
        /// <param name="dateOfRequest">Дата запроса. Формат dd/MM/yyyy</param>
        public static CurrencyRateRequest ParseXMLToCurrencyRate(string dateOfRequest)
        {
            CurrencyRateRequest currencyRateRequest = new CurrencyRateRequest();
            try
            {
                if (!dateOfRequest.IsCorrectForRequest())
                    throw new CurrencyException($"Date format of {nameof(dateOfRequest)} = {dateOfRequest} is invalid");

                string pathToXMLResultOfRequest = GetXMLWithCurrencyRatesAsync(dateOfRequest).Result;
                ValCurs? valCurs = ParseXMLToValCurs(pathToXMLResultOfRequest);

                if (valCurs is null)
                    throw new CurrencyException("The instance ValCurs is null after parsing of the XML");

                currencyRateRequest = ParseValuteToModel(valCurs, dateOfRequest);

                if (currencyRateRequest is null)
                    throw new CurrencyException("The instance CurrencyRate is still null after filling");
            }
            catch (CurrencyException ex) 
            { 
                Console.WriteLine(ex.ToString());
                throw ex;
            }
            
            return currencyRateRequest;
        }

        /// <summary>        
        /// Получение XML файла с сайта ЦБ для указанной даты (dateOfRequest) и запись его в файл. На выходе - путь до этого файла
        /// </summary>
        /// <param name="dateOfRequest">Дата запроса. Формат dd/MM/yyyy</param>
        /// <returns>Путь до XML файла в формате string</returns> 
        private static async Task<string> GetXMLWithCurrencyRatesAsync(string dateOfRequest)
        {
            string path =  "";
            try
            {
                if (!dateOfRequest.IsCorrectForRequest())
                    throw new CurrencyException($"Date format of {nameof(dateOfRequest)} = {dateOfRequest} is invalid");

                string fullURLOfCB = urlCB + dateOfRequest;

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Encoding.GetEncoding("windows-1251");

                string responseBody = await client.GetStringAsync(fullURLOfCB);

                path = $"xmlFile_{dateOfRequest.Replace('/', '_')}.xml";
                
                StreamWriter streamWriter = new StreamWriter(path);
                streamWriter.Write(responseBody);
                streamWriter.Close();
                streamWriter.Dispose();

                Console.WriteLine($"XML file successful created at {path}");
            }
            catch (CurrencyException ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }

            return path;
        }

        /// <summary>        
        /// Парсинг полученного XML файла с сайта ЦБ в экземпляр временного класса ValCurs. 
        /// </summary>
        /// <param name="path">Путь до XML файла</param>
        private static ValCurs? ParseXMLToValCurs(string path)
        {   
            ValCurs? valCurs;
            try
            {
                if (path == "" || !File.Exists(path))
                    throw new CurrencyException("Path to XML is invalid");

                using (TextReader reader = new StreamReader(path))
                {
                    XmlSerializer xmlSerializerCodeInfo = new XmlSerializer(typeof(ValCurs));
                    valCurs = (ValCurs?)xmlSerializerCodeInfo.Deserialize(reader);
                }
            }
            catch (CurrencyException ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }
            return valCurs;
        }

        /// <summary>        
        /// Заполнение экземпляра класса CurrencyRateRequest данными из временного класса ValCurs, в который парсился XML файл с сайта ЦБ. 
        /// </summary>
        /// <param name="dateOfRequest">Дата запроса. Формат dd/MM/yyyy</param>
        /// <param name="valCurs">Временный класс, в который парсился XML файл</param>
        private static CurrencyRateRequest ParseValuteToModel(ValCurs valCurs, string dateOfRequest)
        {
            if (!dateOfRequest.IsCorrectForRequest())
                throw new CurrencyException($"Date format of {nameof(dateOfRequest)} = {dateOfRequest} is invalid");
            if (valCurs is null)
                throw new CurrencyException("The instance ValCurs is null after parsing of the XML");

            CurrencyRateRequest currencyRate = new CurrencyRateRequest
            {
                currencyCodeInfo = new List<CurrencyCodeInfo>(),
                currencyRatesValue = new List<CurrencyRatesValue>()
            };

            try
            { 
                foreach (Valute? valute in valCurs.Valutes)
                {
                    if (valute is null)
                        throw new CurrencyException("The instance Valute is null after parsing of the XML");

                    CurrencyCodeInfo currCodeInfo = FillCurrencyCodeInfoByValute(valute);
                    currencyRate.currencyCodeInfo.Add(currCodeInfo);

                    CurrencyRatesValue currRatesValue = FillCurrencyRatesValueByValute(valute);
                    currencyRate.currencyRatesValue.Add(currRatesValue);
                }

                currencyRate.currencyRatesRequest = FillCurrencyRatesRequestByValute(valCurs, dateOfRequest);
            }
            catch (CurrencyException ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }

            return currencyRate;
        }

        /// <summary>        
        /// Заполнение экземпляра класса CurrencyCodeInfo данными из временного класса Valute, в который парсились данные по каждой котировке из XML файла
        /// </summary>
        /// <param name="valute">Временный класс, в который парсились данные по каждой котировке из XML файла</param>
        private static CurrencyCodeInfo FillCurrencyCodeInfoByValute(Valute valute)
        {
            CurrencyCodeInfo currCodeInfo = new CurrencyCodeInfo()
            {
                IdCurrencyCodeInfo = valute.ID,
                Numcode = valute.NumCode,
                Charcode = valute.CharCode,
                NameCurrency = valute.Name,
                Nominal = valute.Nominal
            };
            return currCodeInfo;
        }

        /// <summary>        
        /// Заполнение экземпляра класса CurrencyRatesValue данными из временного класса Valute, в который парсились данные по каждой котировке из XML файла
        /// </summary>
        /// <param name="valute">Временный класс, в который парсились данные по каждой котировке из XML файла</param>
        private static CurrencyRatesValue FillCurrencyRatesValueByValute(Valute valute)
        {
            CurrencyRatesValue currRatesValue = new CurrencyRatesValue()
            {
                CurrencyValue = valute.Value,
                CurrencyVunitrate = valute.VunitRate,
                CurrencyCodeInfoId = valute.ID
            };
            return currRatesValue;
        }

        /// <summary>        
        /// Заполнение экземпляра класса CurrencyRatesRequest данными из временного класса Valute, в который парсились данные по каждой котировке из XML файла
        /// </summary>
        /// <param name="valute">Временный класс, в который парсились данные по каждой котировке из XML файла</param>
        private static CurrencyRatesRequest FillCurrencyRatesRequestByValute(ValCurs valCurs, string DateOfRequest)
        {
            CurrencyRatesRequest currencyRatesRequest = new CurrencyRatesRequest()
            {               
                NameOfSource = valCurs.name,
                DateOfRequest = DateOnly.Parse(DateOfRequest)                
            };

            if (valCurs.Date != null)
                currencyRatesRequest.DateOfSetting = DateOnly.Parse(valCurs.Date);

            return currencyRatesRequest;
        }
    }
}
