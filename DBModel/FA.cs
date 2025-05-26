using System.ComponentModel.DataAnnotations;

namespace DBModel;

public class FA
{
    private static string _TableName = "tblFA";

    /// <summary>
    /// 資料表名稱
    /// </summary>
    public static string TableName
    {
        get => _TableName;
        set => _TableName = value;
    }

    /// <summary>
    /// 資產編號 (主鍵)
    /// </summary>
    [Key]
    [StringLength(20)]
    [Required]
    public string FACode { get; set; } = string.Empty;

    /// <summary>
    /// 資產名稱
    /// </summary>
    [StringLength(30)]
    [Required]
    public string FAName { get; set; } = string.Empty;

    /// <summary>
    /// 資產規格
    /// </summary>
    [StringLength(30)]
    public string FASpec { get; set; } = string.Empty;

    /// <summary>
    /// 資產類別
    /// </summary>
    [StringLength(10)]
    [Required]
    public string Catego { get; set; } = string.Empty;

    /// <summary>
    /// 資產型態
    /// </summary>
    [Required]
    public int FAType { get; set; }

    /// <summary>
    /// 取得日期 (使用整數格式存儲，如: 20240101)
    /// </summary>
    [Required]
    public int BuyDate { get; set; }

    /// <summary>
    /// 廠商代號
    /// </summary>
    [StringLength(10)]
    public string SupplierID { get; set; } = "";

    /// <summary>
    /// 數量
    /// </summary>
    [Required]
    public int Qty { get; set; }

    /// <summary>
    /// 單位
    /// </summary>
    [StringLength(10)]
    [Required]
    public string Unit { get; set; }

    /// <summary>
    /// 經辦人
    /// </summary>
    [StringLength(10)]
    [Required]
    public string Recorder { get; set; }

    /// <summary>
    /// 部門代號
    /// </summary>
    [StringLength(10)]
    [Required]
    public string DeptCode { get; set; }

    /// <summary>
    /// 狀態 (預設: 0)
    /// </summary>
    public int Status { get; set; } = 0;

    /// <summary>
    /// 主項備註
    /// </summary>
    [StringLength(500)]
    public string MainNote { get; set; } = "";

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