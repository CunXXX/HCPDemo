using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DBModel;

/// <summary>
/// 存放地區基本檔
/// </summary>
[Table("tblLocation")]
public class Location
{
    private static string _TableName = "tblLocation";

    /// <summary>
    /// 資料表名稱
    /// </summary>
    public static string TableName
    {
        get => _TableName;
        set => _TableName = value;
    }

    /// <summary>
    /// 存放地區代號 (主鍵)
    /// </summary>
    [Key]
    [StringLength(10)]
    [Required]
    public string LocationID { get; set; } = "";

    /// <summary>
    /// 存放地區名稱
    /// </summary>
    [StringLength(100)]
    public string LocationName { get; set; } = "";

    /// <summary>
    /// 是否停用 (預設: false)
    /// </summary>
    public bool IsNotValid { get; set; } = false;

    /// <summary>
    /// 最後修改者
    /// </summary>
    [StringLength(10)]
    public string FixBy { get; set; } = "";

    /// <summary>
    /// 最後修改日期 (預設: 系統當前時間)
    /// </summary>
    public DateTime FixDate { get; set; } = DateTime.Now;
}
