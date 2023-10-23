using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Currency.Models;

/// <summary>
/// Таблица значений котировок валют 
/// </summary>
public partial class CurrencyRatesValue
{
    public string? CurrencyValue { get; set; }

    public string? CurrencyVunitrate { get; set; }

    public string CurrencyCodeInfoId { get; set; } = null!;

    public int CurrencyRateRequestId { get; set; }

    public int IdCurrencyRateValue { get; set; }

    public virtual CurrencyCodeInfo CurrencyCodeInfo { get; set; } = null!;

    public virtual CurrencyRatesRequest CurrencyRateRequest { get; set; } = null!;
}
