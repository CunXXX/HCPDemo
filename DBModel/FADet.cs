using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DBModel;

/// <summary>
/// 資產資料細項 (複合主鍵)
/// </summary>
[Table("tblFADet")]
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
    [Key]
    [Column(Order = 1)]
    [StringLength(20)]
    [Required]
    public string FACode { get; set; }

    /// <summary>
    /// 保管人 
    /// </summary>
    [Key]
    [Column(Order = 2)]
    [StringLength(10)]
    [Required]
    public string StoreRecorder { get; set; }

    /// <summary>
    /// 保管部門 
    /// </summary>
    [Key]
    [Column(Order = 3)]
    [StringLength(10)]
    [Required]
    public string StoreDeptCode { get; set; }

    /// <summary>
    /// 存放地區 
    /// </summary>
    [Key]
    [Column(Order = 4)]
    [StringLength(30)]
    [Required]
    public string LocationID { get; set; }

    /// <summary>
    /// 保管數量 (預設: 0)
    /// </summary>
    public int Qty { get; set; } = 0;

    /// <summary>
    /// 已出售數 (預設: 0)
    /// </summary>
    public int ExSellQty { get; set; } = 0;

    /// <summary>
    /// 已意外處分數量 (預設: 0)
    /// </summary>
    public int ExAccQty { get; set; } = 0;

    /// <summary>
    /// 已報廢數量 (預設: 0)
    /// </summary>
    public int ExScrapQty { get; set; } = 0;

    /// <summary>
    /// 細項備註
    /// </summary>
    [StringLength(500)]
    public string DetNote { get; set; } = "";

    /// <summary>
    /// 最後修改日 (預設: 系統當前時間)
    /// </summary>
    public DateTime FixDate { get; set; } = DateTime.Now;

    /// <summary>
    /// 最後修改者
    /// </summary>
    [StringLength(10)]
    public string FixBy { get; set; } = "";
}
