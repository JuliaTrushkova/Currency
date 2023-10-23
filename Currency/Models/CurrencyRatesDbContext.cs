using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Currency.Models;


public partial class CurrencyRatesDbContext : DbContext
{
    public CurrencyRatesDbContext()
    {
    }

    public CurrencyRatesDbContext(DbContextOptions<CurrencyRatesDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CurrencyCodeInfo> CurrencyCodeInfos { get; set; }

    public virtual DbSet<CurrencyRatesRequest> CurrencyRatesRequests { get; set; }

    public virtual DbSet<CurrencyRatesValue> CurrencyRatesValues { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<CurrencyRatesDbContext>().Build();
        optionsBuilder.UseNpgsql(config["Postgres:ConnectionString"]);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CurrencyCodeInfo>(entity =>
        {
            entity.HasKey(e => e.IdCurrencyCodeInfo).HasName("currency_params_pkey");

            entity.ToTable("currency_code_infos");

            entity.HasIndex(e => e.Charcode, "UNI_currency_params_charcode").IsUnique();

            entity.HasIndex(e => e.Numcode, "UNI_currency_params_numcode").IsUnique();

            entity.Property(e => e.IdCurrencyCodeInfo).HasColumnName("id_currency_code_info");
            entity.Property(e => e.Charcode)
                .HasMaxLength(6)
                .HasColumnName("charcode");
            entity.Property(e => e.NameCurrency).HasColumnName("name_currency");
            entity.Property(e => e.Nominal).HasColumnName("nominal");
            entity.Property(e => e.Numcode).HasColumnName("numcode");
        });
        
        modelBuilder.Entity<CurrencyRatesRequest>(entity =>
        {
            entity.HasKey(e => e.IdCurrencyRateRequest).HasName("pk_currency_rate_requests");

            entity.ToTable("currency_rates_requests");

            entity.Property(e => e.IdCurrencyRateRequest).HasColumnName("id_currency_rate_request");
            entity.Property(e => e.DateOfRequest).HasColumnName("date_of_request");
            entity.Property(e => e.DateOfSetting).HasColumnName("date_of_setting");
            entity.Property(e => e.NameOfSource).HasColumnName("name_of_source");
        });

        modelBuilder.Entity<CurrencyRatesValue>(entity =>
        {
            entity.HasKey(e => e.IdCurrencyRateValue).HasName("currency_rates_info_pkey");

            entity.ToTable("currency_rates_values");

            entity.Property(e => e.IdCurrencyRateValue).HasColumnName("id_currency_rate_value");
            entity.Property(e => e.CurrencyCodeInfoId)
                .HasMaxLength(12)
                .HasColumnName("currency_code_info_id");
            entity.Property(e => e.CurrencyRateRequestId)
                .ValueGeneratedOnAdd()
                .HasColumnName("currency_rate_request_id");
            entity.Property(e => e.CurrencyValue)
                .HasMaxLength(12)
                .HasColumnName("currency_value");
            entity.Property(e => e.CurrencyVunitrate)
                .HasMaxLength(14)
                .HasColumnName("currency_vunitrate");

            entity.HasOne(d => d.CurrencyCodeInfo).WithMany(p => p.CurrencyRatesValues)
                .HasForeignKey(d => d.CurrencyCodeInfoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_currency_rates_value_currency_code_info");

            entity.HasOne(d => d.CurrencyRateRequest).WithMany(p => p.CurrencyRatesValues)
                .HasForeignKey(d => d.CurrencyRateRequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_currency_rates_values_currency_rates_request");
        });

        
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
