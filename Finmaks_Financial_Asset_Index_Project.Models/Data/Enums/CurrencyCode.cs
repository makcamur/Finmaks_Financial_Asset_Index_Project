using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finmaks_Financial_Asset_Index_Project.DataAccess.Data.Enums
{
    public enum CurrencyCode
    {
        [Description("AMERİKAN DOLARI")]
        USD = 1,

        [Description("KUVEYT DİNARI")]
        KWD = 2,

        [Description("KANADA DOLARI")]
        CAD = 3,

        [Description("AVUSTRALYA DOLARI")]
        AUD = 4,

        [Description("BULGAR LEVASI")]
        BGL = 5,

        [Description("100 IRAN RİYALİ")]
        IRR = 6,

        [Description("RUMEN LEYI")]
        RON = 7,

        [Description("SURİYE LİRASI")]
        SYP = 8,

        [Description("ÜRDÜN DİNARI")]
        JOD = 9,

        [Description("YENİ İSRAİL ŞEKELİ")]
        ILS = 10,

        [Description("MACAR FORİNTİ")]
        HUF = 11,

        [Description("AVRUPA PARA BİRİMİ")]
        EUR = 12,

        [Description("ESKİ TÜRKMENİSTAN MANATI")]
        TMM = 13,

        [Description("IRAK DİNARI")]
        IQD = 14,

        [Description("LİBYA DİNARI")]
        LYD = 15,

        [Description("ÇEK KORUNASI")]
        CZK = 16,

        [Description("RUS RUBLESİ")]
        RUB = 17,

        [Description("ALMAN MARKI")]
        DEM = 18,

        [Description("BELÇİKA FRANGI")]
        BEF = 19,

        [Description("LÜKSEMBURG FRANGI")]
        LUF = 20,

        [Description("İSPANYOL PESETASI")]
        ESP = 21,

        [Description("FRANSIZ FRANGI")]
        FRF = 22,

        [Description("İNGİLİZ STERLİNİ")]
        GBP = 23,

        [Description("İRLANDA LİRASI")]
        IEP = 24,

        [Description("100 İTALYAN LİRETİ")]
        ITL = 25,

        [Description("HOLLANDA FLORİNİ")]
        NLG = 26,

        [Description("AVUSTURYA ŞİLİNİ")]
        ATS = 27,

        [Description("PORTEKİZ ESKÜDOSU")]
        PTE = 28,

        [Description("FİN MARKKASI")]
        FIM = 29,

        [Description("YUNAN DRAHMİSİ")]
        GRD = 30,

        [Description("TÜRKMENİSTAN MANATI")]
        TMT = 31,

        [Description("GÜRCİSTAN LARİSİ")]
        GEL = 32,

        [Description("KAZAKİSTAN TENGESİ")]
        KZT = 33,

        [Description("İSVİÇRE FRANGI")]
        CHF = 34,

        [Description("MAKEDONYA DİNARI")]
        MKD = 35,

        [Description("BOSNA HERSEK MARK")]
        BAM = 36,

        [Description("AZERBAYCAN MANATI")]
        AZN = 37,

        [Description("OZBEKİSTAN SUM")]
        UZS = 38,

        [Description("BİRLEŞİK ARAP EMİR. DİRHEMİ")]
        AED = 39,

        [Description("HIRVAT KUNASI")]
        HRK = 40,

        [Description("YENİ BULGAR LEVASI")]
        BGN = 41,

        [Description("100 JAPON YENİ")]
        JPY = 42,

        [Description("HİNDİSTAN RUPİSİ")]
        INR = 43,

        [Description("ÇİN YUANI RENMİNBİ")]
        CNY = 44,

        [Description("İSVEÇ KRONU")]
        SEK = 45,

        [Description("DANİMARKA KRONU")]
        DKK = 46,

        [Description("NORVEÇ KRONU")]
        NOK = 47,

        [Description("5 GR ZİRAAT ALTINI")]
        A05 = 48,

        [Description("10 GR ZİRAAT ALTINI")]
        A10 = 49,

        [Description("1 GR KÜLÇE ALTIN")]
        A01 = 50,

        [Description("ALTIN")]
        XAU = 51,

        [Description("MEVDUAT ALTIN")]
        A02 = 52,

        [Description("CEZAYIR DINARI")]
        DZD = 53,

        [Description("KATAR RİYALİ")]
        QAR = 54,

        [Description("UMMAN RİYALİ")]
        OMR = 55,

        [Description("TÜRK LİRASI")]
        TRY = 56,

        [Description("SUUDİ ARABİSTAN RİYALİ")]
        SAR = 57,

        [Description("BAHREYN DINARI")]
        BHD = 58,

        [Description("POTA DÖVİZ KODU")]
        SDR = 59,

        [Description("EVALUASYON")]
        EVA = 60,

        [Description("FAS PARA BİRİMİ (DİRHEM)")]
        MAD = 61,

        [Description("SLOVAK KORUNASI")]
        SKK = 62,

        [Description("GÜNEY AFRİKA RANDI")]
        ZAR = 63,

        [Description("POLONYA ZLOTİSİ")]
        PLN = 64,

        [Description("MISIR POUNDU")]
        EGP = 65,

        [Description("TUNUS DİNARI")]
        TND = 66,

        [Description("CIN OFFSHORE YUAN")]
        CNH = 67,

        [Description("RUMEN LEYI")]
        ROL = 68,

        [Description("AVRUPA PARA BİRİMİ")]
        ECU = 69,

        [Description("GÜMÜŞ")]
        XAG = 70,

        [Description("YENİ ZELANDA DOLARI")]
        NZD = 71,

        [Description("PLATİN (GR)")]
        PLT = 72,

        [Description("ALÜMİNYUM (TON)")]
        ALM = 73,

        [Description("Endonezya Rupıah")]
        IDR = 74,

        [Description("Meksika Pezosu")]
        MXN = 75,

        [Description("Nepal Rupisi")]
        NPR = 76,

        [Description("Brezilya Real")]
        BRL = 77,

        [Description("Tayland Bahtı")]
        THB = 78,

        [Description("Singapur Doları")]
        SGD = 79,

        [Description("Şili Pezosu")]
        CLP = 80,

        [Description("Hong Kong Doları")]
        HKD = 81,

        [Description("Güney Kore Wonu")]
        KRW = 82,

        [Description("Malezya Ringgiti")]
        MYR = 83,

        [Description("Yeni Tavyan Doları")]
        TWD = 84
    }
}
