using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DBModel
{
    /// <summary>
    /// 員工基本檔
    /// </summary>
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
        public string UserID { get; set; } = string.Empty;

        /// <summary>
        /// 使用者名稱
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 部門代號
        /// </summary>
        public string DeptCode { get; set; } = string.Empty;

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
        public string FixBy { get; set; } = string.Empty;
    }
}
