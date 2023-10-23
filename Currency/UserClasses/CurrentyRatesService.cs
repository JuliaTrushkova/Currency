using Currency.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Currency.UserClasses
{
    /// <summary>
    /// Сервис по работе с котировками валют (запускает парсинг XML ЦБ и запись в базу данных полученных значений котировок)
    /// </summary>
    internal class CurrentyRatesService
    {
        /// <summary>
        /// Заполняет базу данных котировок значениями, полученными для указанной даты (DateOfRequest). 
        /// Сначала парсит XML файл с сайта ЦБ в экземпляр класса CurrencyRateRequest, затем записывает полученные данные в базу данных 
        /// </summary>
        /// <param name="dateOfRequest">Дата запроса. Формат dd/MM/yyyy</param>
        public static void AddingNewCurrencyRate(string dateOfRequest)
        {
            try
            {
                if (!dateOfRequest.IsCorrectForRequest())
                    throw new CurrencyException($"Date format of {nameof(dateOfRequest)} = {dateOfRequest} is invalid");

                CurrencyRateRequest? currencyRateRequest = CurrencyRateRequest.CreateCurrencyRates(dateOfRequest);

                if (currencyRateRequest is null)
                    throw new CurrencyException("The instance CurrencyRateRequest is still null after filling");

                FillingDatabase.FillOneCurrencyRate(currencyRateRequest);
            }
            catch (CurrencyException ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// Заполняет базу данных котировок значениями, полученными за указанный период времени (от startDateOfRequest до endDateOfRequest). 
        /// Сначала парсит XML файл с сайта ЦБ для каждой даты по отдельности в коллекцию экземпляров класса CurrencyRateRequest, затем записывает полученные данные в базу данных 
        /// </summary>
        /// <param name="startDateOfRequest">Начальная дата запроса. Формат dd/MM/yyyy</param>
        /// <param name="endDateOfRequest">Конечная дата запроса. Формат dd/MM/yyyy</param>
        public static void AddingSeveralCurrencyRate(string startDateOfRequest, string endDateOfRequest)
        {
            try
            {
                if (!startDateOfRequest.IsCorrectForRequest())
                    throw new CurrencyException($"Date format of {nameof(startDateOfRequest)} = {startDateOfRequest} is invalid");
                if (!startDateOfRequest.IsCorrectForRequest())
                    throw new CurrencyException($"Date format of {nameof(endDateOfRequest)} = {endDateOfRequest} is invalid");

                List<CurrencyRateRequest> currencyRates = CurrencyRateRequest.CreateCurrencyRates(startDateOfRequest, endDateOfRequest);

                FillingDatabase.FillSeveralCurrencyRate(currencyRates);
            }
            catch (CurrencyException ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// Заполняет базу данных котировок значениями за текущий день. 
        /// Сначала парсит XML файл с сайта ЦБ за текущую дату в экземпляр класса CurrencyRateRequest, затем записывает полученные данные в базу данных 
        /// </summary>
        public static void AddingTodayCurrencyRate()
        {
            DateTime dtNow = DateTime.Now;

            try
            {
                CurrencyRateRequest? currencyRateRequest = CurrencyRateRequest.CreateCurrencyRates(dtNow.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));

                if (currencyRateRequest is null)
                    throw new CurrencyException("The instance CurrencyRateRequest is still null after filling");

                FillingDatabase.FillOneCurrencyRate(currencyRateRequest);
            }
            catch (CurrencyException ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// Заполняет базу данных котировок значениями за последний месяц не включая текущую дату. 
        /// Сначала парсит XML файл с сайта ЦБ для каждой даты по отдельности в коллекцию экземпляров класса CurrencyRateRequest, затем записывает полученные данные в базу данных  
        /// </summary>
        public static void AddingCurrencyRateForLastMonth()
        {
            try
            {
                (string startDateOfRequest, string endDateOfRequest) = ConvertLastMonthToDays();

                if (!startDateOfRequest.IsCorrectForRequest())
                    throw new CurrencyException($"Date format of {nameof(startDateOfRequest)} = {startDateOfRequest} is invalid");
                if (!startDateOfRequest.IsCorrectForRequest())
                    throw new CurrencyException($"Date format of {nameof(endDateOfRequest)} = {endDateOfRequest} is invalid");

                List<CurrencyRateRequest> currencyRates = CurrencyRateRequest.CreateCurrencyRates(startDateOfRequest, endDateOfRequest);

                FillingDatabase.FillSeveralCurrencyRate(currencyRates);
            }
            catch (CurrencyException ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// Получает начальную и конечную даты последнего месяца (не включая текущую дату) 
        /// </summary>
        private static (string startDateOfRequest, string endDateOfRequest) ConvertLastMonthToDays()
        {
            DateTime dtNow = DateTime.Now;
            dtNow = dtNow.AddDays(-1);
            DateTime dtMonthAgo = dtNow.AddMonths(-1);

            string startDateOfRequest = dtMonthAgo.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            string endDateOfRequest = dtNow.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            return (startDateOfRequest, endDateOfRequest);
        }
    }
}
