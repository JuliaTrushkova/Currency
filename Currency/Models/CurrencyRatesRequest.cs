using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Currency.Models;

/// <summary>
/// Таблица запросов котировок валют с параметрами (id запроса, дата запроса, дата котировки в ответе, источник данных)
/// </summary>
public partial class CurrencyRatesRequest
{    
    public DateOnly DateOfRequest { get; set; }
    
    public DateOnly? DateOfSetting { get; set; }
    
    public string? NameOfSource { get; set; } = null!;
    
    public int IdCurrencyRateRequest { get; set; }   
    
    public virtual ICollection<CurrencyRatesValue> CurrencyRatesValues { get; set; } = new List<CurrencyRatesValue>();
}
