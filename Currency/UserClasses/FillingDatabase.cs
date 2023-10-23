using Currency.Models;

namespace Currency.UserClasses
{
    /// <summary>
    /// Заполнение базы значениями котировок валют
    /// </summary>
    internal static class FillingDatabase
    {

        /// <summary>
        /// Заполняет базу данных котировок значениями, полученными после парсинга XML ЦБ за одну дату
        /// </summary>
        /// <param name="currencyRateRequest">Данные по котировкам, полученные в результате парсинга XML ЦБ за одну дату </param>
        public static void FillOneCurrencyRate(CurrencyRateRequest currencyRateRequest)
        {            
            try
            {
                if (currencyRateRequest is null)
                    throw new CurrencyException(message: "No request result");

                using (CurrencyRatesDbContext db = new CurrencyRatesDbContext())
                {
                    if (currencyRateRequest.currencyRatesRequest is null)
                        throw new CurrencyException(message: "No request parameters");
                    if (currencyRateRequest.currencyCodeInfo is null)
                        throw new CurrencyException(message: "No currency code info"); 
                    if (currencyRateRequest.currencyRatesValue is null)
                        throw new CurrencyException(message: "No values of currency rates");

                    //Проверка на наличие такой же записи запроса (чтобы не было дубликатов) по дате запроса, дате утверждения и названию источника. Если нет, то записывает результат запроса в базу
                    if (!db.CurrencyRatesRequests.Where(x => ((x.DateOfRequest == currencyRateRequest.currencyRatesRequest.DateOfRequest) &&
                                                              (x.DateOfSetting == currencyRateRequest.currencyRatesRequest.DateOfSetting) &&
                                                              (x.NameOfSource == currencyRateRequest.currencyRatesRequest.NameOfSource))).Any())
                    {
                        //Запись в таблицу запросов с параметрами
                        db.CurrencyRatesRequests.Add(currencyRateRequest.currencyRatesRequest);
                        db.SaveChanges();

                        //Запись в таблицу параметров котировок валют
                        foreach (CurrencyCodeInfo currCodeInfo in currencyRateRequest.currencyCodeInfo)
                        {
                            //Проверка на наличие такого же типа валюты по сверке ID валюты. Если нет, то записывает новую строчку в таблице параметров валют
                            if (!db.CurrencyCodeInfos.Where(x => x.IdCurrencyCodeInfo == currCodeInfo.IdCurrencyCodeInfo).Any())
                            {
                                db.CurrencyCodeInfos.Add(currCodeInfo);
                            }
                        }

                        db.SaveChanges();

                        //Запись в таблицу значений котировок
                        foreach (var currRatesValue in currencyRateRequest.currencyRatesValue)
                        {
                            currRatesValue.CurrencyRateRequestId = currencyRateRequest.currencyRatesRequest.IdCurrencyRateRequest;
                            db.CurrencyRatesValues.Add(currRatesValue);
                        }

                        db.SaveChanges();

                        Console.WriteLine(currencyRateRequest.ToString(true));
                    }
                    else
                    {
                        Console.WriteLine(currencyRateRequest.ToString(false));
                    }
                }
            }
            catch (CurrencyException ex)
            {
                Console.WriteLine(ex.ToString());                
                throw ex;
            }
        }

        /// <summary>
        /// Заполняет базу данных котировок значениями, полученными за период времени
        /// </summary>
        /// <param name="currencyRates">Данные по котировкам, полученные в результате парсинга XML ЦБ за период времени</param>
        public static void FillSeveralCurrencyRate(List<CurrencyRateRequest> currencyRates)
        {
            try
            {
                if (currencyRates is null)
                    return;

                using (CurrencyRatesDbContext db = new CurrencyRatesDbContext())
                {
                    foreach (CurrencyRateRequest currencyRate in currencyRates)
                    {
                        FillOneCurrencyRate(currencyRate);
                    }
                    db.SaveChanges();
                }
            }
            catch
            {
                throw new CurrencyException();
            }
        }

    }
}
