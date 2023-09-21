using Finmaks_Financial_Asset_Index_Project.DataAccess.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finmaks_Financial_Asset_Index_Project.DataAccess.Data.DTOs
{
    public class AssetIndexExchangeFinalTableVM
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ErrorMessage { get; set; }

        [Display(Name = "Tarih")]
        public List<DateTime> dateTimes { get; set; }

        [Display(Name = "Varlık Tarih Valık Tutarı")]
        public List<Decimal> Assets { get; set; }

        [Display(Name = "Önceki Aya Göre Varlık Artış ")]
        public List<Decimal> IncreaseInAssetsComparedToThePreviousMonth { get; set; }

        [Display(Name = "Varlık Değişim Oranı ")]
        public List<Decimal> AssetTurnoverRatio { get; set; }

        [Display(Name = "Varlık Tarih Dolar Kuru ")]
        public List<Decimal> AssetHistoricalExchangeRate { get; set; }

        [Display(Name = "Dolarizasyon Varlık Tutarı")]
        public List<Decimal> DollarizationAssetAmount { get; set; }

        [Display(Name = "Dolarizasyon Önceki Aya Göre Varlık Artış ")]
        public List<Decimal> DollarizationIncreaseComparedToThePreviousMonth { get; set; }

        [Display(Name ="Dolarizasyon Varlık Değişim Oranı ")]
        public List<Decimal> DollarizationAssetTurnoverRate { get; set; }

        [Display(Name = "Dolarizasyon Etkisi Yüzde ")]
        public List<Decimal> DollarizationImpactPercentage { get; set; }

        [Display(Name = "Üfe Endeks ")]
        public List<Decimal>  ProducerPriceIndex { get; set; }

        [Display(Name = "Enflasyon Varlık Tutarı ")]
        public List<Decimal> InflationAssetValue { get; set; }

        [Display(Name = "Enflasyon Önceki Aya Göre Varlık Artış ")]
        public List<Decimal> InflationAssetIncreaseComparedToThePreviousMonth { get; set; }

        [Display(Name = "Enflasyon Varlık Değişim Oranı ")]
        public List<Decimal> InflationAssetTurnoverRate { get; set; }

        [Display(Name ="Enflasyon Etkisi Yüzde ")]
        public List<Decimal> InflationImpactPercentage { get; set; }
 

    }
}
