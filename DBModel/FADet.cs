using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DBModel;

/// <summary>
/// 資產資料細項 
/// </summary>
public class FADet
{
    private static string _TableName = "tblFADet";

    /// <summary>
    /// 資料表名稱
    /// </summary>
    public static string TableName
    {
        get => _TableName;
        set => _TableName = value;
    }

    /// <summary>
    /// 資產編號 
    /// </summary>
    public string FACode { get; set; } = string.Empty;
     
    /// <summary>
    /// 保管人 
    /// </summary>
    public string StoreRecorder { get; set; } = string.Empty;

    /// <summary>
    /// 保管部門 
    /// </summary>
    public string StoreDeptCode { get; set; } = string.Empty;

    /// <summary>
    /// 存放地區 
    /// </summary>
    public string LocationID { get; set; } = string.Empty;

    /// <summary>
    /// 保管數量
    /// </summary>
    public int Qty { get; set; } = 0;

    /// <summary>
    /// 已出售數 
    /// </summary>
    public int ExSellQty { get; set; } = 0;

    /// <summary>
    /// 已意外處分數量 
    /// </summary>
    public int ExAccQty { get; set; } = 0;

    /// <summary>
    /// 已報廢數量 
    /// </summary>
    public int ExScrapQty { get; set; } = 0;

    /// <summary>
    /// 細項備註
    /// </summary>
    public string DetNote { get; set; } = string.Empty;

    /// <summary>
    /// 最後修改日 
    /// </summary>
    public DateTime FixDate { get; set; } = DateTime.Now;

    /// <summary>
    /// 最後修改者
    /// </summary>
    public string FixBy { get; set; } = string.Empty;
}
