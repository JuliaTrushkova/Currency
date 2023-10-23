using Currency.Models;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace Currency.UserClasses
{   
    public class CurrencyRateRequest
    {        
        public List<CurrencyCodeInfo>? currencyCodeInfo { get; set; }

        public CurrencyRatesRequest? currencyRatesRequest { get; set; }
        
        public List<CurrencyRatesValue>? currencyRatesValue { get; set; }


        /// <summary>
        /// Создание экземпляра курсов валют CurrencyRateRequest по запросу в ЦБ на указанную дату dateOfRequest путем парсинга XML
        /// </summary>
        /// <param name="dateOfRequest">Дата запроса. Формат dd/MM/yyyy</param>
        public static CurrencyRateRequest? CreateCurrencyRates(string dateOfRequest)
        {
            if (!dateOfRequest.IsCorrectForRequest())
                throw new CurrencyException($"Date format of {nameof(dateOfRequest)} = {dateOfRequest} is invalid");

            CurrencyRateRequest? currencyRateRequest = new CurrencyRateRequest();
            try
            {  
                currencyRateRequest = XMLToCurrencyParser.ParseXMLToCurrencyRate(dateOfRequest);

                Console.WriteLine(currencyRateRequest.ToString());
            }
            catch (CurrencyException ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }
            return currencyRateRequest;
        }

        /// <summary>
        /// Создание коллекции экземпляров CurrencyRateRequest курсов валют CurrencyRateRequest по запросу в ЦБ на указанный период времени от startDateOfRequest до endDateOfRequest 
        /// (путем парсинга XML каждого дня по отдельности)        
        /// </summary>
        /// <param name="startDateOfRequest">Начальная дата запроса. Формат dd/MM/yyyy</param>
        /// <param name="endDateOfRequest">Конечная дата запроса. Формат dd/MM/yyyy</param>        
        public static List<CurrencyRateRequest> CreateCurrencyRates(string startDateOfRequest, string endDateOfRequest)
        {
            if (!startDateOfRequest.IsCorrectForRequest())
                throw new CurrencyException($"Date format of {nameof(startDateOfRequest)} = {startDateOfRequest} is invalid");
            if (!startDateOfRequest.IsCorrectForRequest())
                throw new CurrencyException($"Date format of {nameof(endDateOfRequest)} = {endDateOfRequest} is invalid");

            (DateTime startDate, DateTime endTime) = ConvertAndCheckDates(startDateOfRequest, endDateOfRequest);

            List<CurrencyRateRequest> currencyRates = new List<CurrencyRateRequest>();

            try
            {
                for (DateTime dateTime = startDate; dateTime <= endTime; dateTime = dateTime.AddDays(1))
                {
                    string dateTimeString = dateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    CurrencyRateRequest? currencyRateRequest = CreateCurrencyRates(dateTimeString);

                    if (currencyRateRequest == null)
                        throw new CurrencyException("The instance CurrencyRate is still null after filling");                   

                    Console.WriteLine(currencyRateRequest.ToString());
                }                
            }
            catch (CurrencyException ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }

            return currencyRates;
        }

        public override string ToString()
        {
            return $"CurrencyRateRequest has created {this.currencyRatesRequest.DateOfRequest} by {this.currencyRatesRequest.NameOfSource} and " +
                $"consists of {this.currencyRatesValue.Count} currencyRatesValues and {this.currencyCodeInfo.Count} currencyCodeInfos.";
        }        

        public string ToString(bool IsNotExists)
        {
            if (IsNotExists)
            {
                return $"CurrencyRateRequest created {this.currencyRatesRequest.DateOfRequest} by {this.currencyRatesRequest.NameOfSource} " +
                $"has loaded into the database";
            }
            else
            {
                return $"CurrencyRateRequest created {this.currencyRatesRequest.DateOfRequest} by {this.currencyRatesRequest.NameOfSource} " +
                    $"has already been in the database. So not loaded";
            }
        }


        /// <summary>
        /// Преобразование типа дат string в DateTime и проверка на то, что начальная дата меньше конечной (если нет, то меняются местами)
        /// </summary>
        /// <param name="startDateOfRequest">Начальная дата запроса. Формат dd/MM/yyyy</param>
        /// <param name="endDateOfRequest">Конечная дата запроса. Формат dd/MM/yyyy</param>      
        private static (DateTime startDate, DateTime endTime) ConvertAndCheckDates(string startDateOfRequest, string endDateOfRequest)
        {
            DateTime startDate = Convert.ToDateTime(startDateOfRequest);
            DateTime endTime = Convert.ToDateTime(endDateOfRequest);

            if (startDate > endTime)
            {
                (endTime, startDate) = (startDate, endTime);
            }

            return (startDate, endTime);
        }

    }
}
