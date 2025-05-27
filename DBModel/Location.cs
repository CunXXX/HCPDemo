using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DBModel;

/// <summary>
/// 存放地區基本檔
/// </summary>
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
    /// 存放地區代號 
    /// </summary>
    public string LocationID { get; set; } = string.Empty;

    /// <summary>
    /// 存放地區名稱
    /// </summary>
    public string LocationName { get; set; } = string.Empty;

    /// <summary>
    /// 是否停用 
    /// </summary>
    public bool IsNotValid { get; set; } = false;

    /// <summary>
    /// 最後修改者
    /// </summary>
    public string FixBy { get; set; } = string.Empty;

    /// <summary>
    /// 最後修改日期
    /// </summary>
    public DateTime FixDate { get; set; } = DateTime.Now;
}
