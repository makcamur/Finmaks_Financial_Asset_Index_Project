using Finmaks_Financial_Asset_Index_Project.DataAccess.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finmaks_Financial_Asset_Index_Project.DataAccess.Data.DTOs
{
    public class AssetIndexExchangeFinalTableDTO
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

    }
}
