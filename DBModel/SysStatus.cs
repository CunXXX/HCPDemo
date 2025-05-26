using System.ComponentModel.DataAnnotations;

namespace DBModel;

public class SysStatus
{
    private static string _TableName = "tblSysStatus";

    /// <summary>
    /// 資料表名稱
    /// </summary>
    public static string TableName
    {
        get => _TableName;
        set => _TableName = value;
    }

    /// <summary>
    /// 狀態代號 (主鍵)
    /// </summary>
    [Key]
    [Required]
    public int Status { get; set; }

    /// <summary>
    /// 狀態說明
    /// </summary>
    [StringLength(10)]
    [Required]
    public string StatusName { get; set; }
}
