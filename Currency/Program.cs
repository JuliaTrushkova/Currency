using System.Data;

using Currency.UserClasses;


namespace Currency
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Заполняет таблицу котировок значениями за последний месяц. Уже заполнено и поэтому закомментировано. (но есть проверка на дубликаты, поэтому в любом случае база не будет заполняться)
            //CurrentyRatesService.AddingCurrencyRateForLastMonth();

            //Заполняет таблицу котировок значениями за текущий день
            try
            {
                CurrentyRatesService.AddingTodayCurrencyRate();                
            }
            catch (CurrencyException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }
    }
}