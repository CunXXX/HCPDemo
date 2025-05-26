using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DBModel
{
    /// <summary>
    /// 員工基本檔
    /// </summary>
    [Table("tblSysUser")]
    public class SysUser
    {
        private static string _TableName = "tblSysUser";

        /// <summary>
        /// 資料表名稱
        /// </summary>
        public static string TableName
        {
            get => _TableName;
            set => _TableName = value;
        }

        /// <summary>
        /// 使用者代號 (主鍵)
        /// </summary>
        [Key]
        [StringLength(10)]
        [Required]
        public string UserID { get; set; }

        /// <summary>
        /// 使用者名稱
        /// </summary>
        [StringLength(30)]
        public string UserName { get; set; } = "";

        /// <summary>
        /// 部門代號
        /// </summary>
        [StringLength(10)]
        public string DeptCode { get; set; } = "";

        /// <summary>
        /// 是否停用 (預設: false)
        /// </summary>
        public bool IsNotValid { get; set; } = false;

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
}
