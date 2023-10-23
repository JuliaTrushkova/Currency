using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Currency.Models;

/// <summary>
/// Таблица параметров котировок валют
/// </summary>
public partial class CurrencyCodeInfo
{
    public string IdCurrencyCodeInfo { get; set; } = null!;

    public short? Numcode { get; set; }

    public string? Charcode { get; set; }

    public string? NameCurrency { get; set; }

    public int? Nominal { get; set; }

    public virtual ICollection<CurrencyRatesValue> CurrencyRatesValues { get; set; } = new List<CurrencyRatesValue>();
}
