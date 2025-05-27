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
    /// 資產編號 
    /// </summary>
    public string FACode { get; set; } = string.Empty;

    /// <summary>
    /// 資產名稱
    /// </summary>
    public string FAName { get; set; } = string.Empty;

    /// <summary>
    /// 資產規格
    /// </summary>
    public string FASpec { get; set; } = string.Empty;

    /// <summary>
    /// 資產類別
    /// </summary>
    public string Catego { get; set; } = string.Empty;

    /// <summary>
    /// 資產型態
    /// </summary>
    public int FAType { get; set; } = 0;

    /// <summary>
    /// 取得日期 (使用整數格式存儲，如: 20240101)
    /// </summary>
    public int BuyDate { get; set; } = 0;

    /// <summary>
    /// 廠商代號
    /// </summary>
    public string SupplierID { get; set; } = string.Empty;

    /// <summary>
    /// 數量
    /// </summary>
    public int Qty { get; set; } = 0;

    /// <summary>
    /// 單位
    /// </summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>
    /// 經辦人
    /// </summary>
    public string Recorder { get; set; } = string.Empty;

    /// <summary>
    /// 部門代號
    /// </summary>
    public string DeptCode { get; set; } = string.Empty;

    /// <summary>
    /// 狀態 (預設: 0)
    /// </summary>
    public int Status { get; set; } = 0;

    /// <summary>
    /// 主項備註
    /// </summary>
    public string MainNote { get; set; } = "";

    /// <summary>
    /// 最後修改日 
    /// </summary>
    public DateTime FixDate { get; set; } = DateTime.Now;

    /// <summary>
    /// 最後修改者
    /// </summary>
    public string FixBy { get; set; } = "";
}