using System.ComponentModel.DataAnnotations;

namespace DBModel;

public class Dept
{
    private static string _TableName = "tblDept";

    /// <summary>
    /// 資料表名稱
    /// </summary>
    public static string TableName
    {
        get => _TableName;
        set => _TableName = value;
    }

    /// <summary>
    /// 部門代號 (主鍵)
    /// </summary>
    [Key]
    [StringLength(10)]
    [Required]
    public string DeptCode { get; set; } = string.Empty;

    /// <summary>
    /// 部門簡稱
    /// </summary>
    [StringLength(20)]
    [Required]
    public string DeptSName { get; set; } = string.Empty;

    /// <summary>
    /// 部門名稱
    /// </summary>
    [StringLength(60)]
    [Required]
    public string DeptName { get; set; } = string.Empty;

    /// <summary>
    /// 備註
    /// </summary>
    [StringLength(200)]
    public string Note { get; set; } = string.Empty;

    /// <summary>
    /// 最後修改日 (預設: 系統當前時間)
    /// </summary>
    public DateTime FixDate { get; set; } = DateTime.Now;

    /// <summary>
    /// 最後修改者
    /// </summary>
    [StringLength(10)]
    public string FixBy { get; set; } = string.Empty;

    /// <summary>
    /// 是否停用 (預設: false)
    /// </summary>
    public bool IsNotValid { get; set; } = false;
}
