using Common;
using Dapper;
using DBModel;
using System.Drawing.Printing;
using System.Security.Principal;
using static Model.Model;

namespace HCPService.Repositories;

public class AssetRepository : IAssetRepository
{
    private readonly IDBService m_DB;

    public AssetRepository(IDBService db)
    {
        m_DB = db;
    }

    public async Task<List<EmployeeSuggest>> SearchEmployeeAsync(string strKeyword)
    {
        var _strSql = @$"
            SELECT DISTINCT 
                UserID AS EmployeeNo,
                UserName AS Name
            FROM {SysUser.TableName}
            WHERE IsNotValid = 0
              AND (UserID LIKE @kw OR UserName LIKE @kw)
            ORDER BY UserID";

        var _dpParameter = new DynamicParameters();

        _dpParameter.Add("@kw", $"%{strKeyword}%");

        var _ienumEmployees = await m_DB.QueryAsync<EmployeeSuggest>(_strSql, _dpParameter);
     
        return _ienumEmployees.ToList() ?? [];
    }

    /// <summary>
    /// 查詢使用者資產
    /// </summary>
    /// <param name="strUserId"></param>
    /// <param name="iPageIndex"></param>
    /// <param name="iPageSize"></param>
    /// <returns></returns>
    public async Task<PagedResult<AssetViewModel>> GetAssetsByUserAsync(string strUserId, int iPageIndex, int iPageSize)
    {
        //1.先查詢總數量
        var _strSql = @$"SELECT COUNT(*) FROM {FADet.TableName} WHERE StoreRecorder = @UserId";
        var _dpParameter = new DynamicParameters();

        _dpParameter.Add("@UserId", strUserId);

        var _ienumTotal = await m_DB.QueryAsync<int>(_strSql, _dpParameter);
        var _iTotal = _ienumTotal.FirstOrDefault();

        //2.查詢資產資料，分頁處理
        _strSql = $@"
            SELECT DISTINCT
                f.FACode AS AssetId,
                f.FACode AS AssetNumber,
                f.FAName AS AssetName,
                f.FASpec AS Spec,
                d.Qty AS Unit,
                l.LocationName AS Location,
                '' AS ReceiverName
            FROM {FA.TableName} f
            LEFT JOIN {FADet.TableName} d ON f.FACode = d.FACode
            LEFT JOIN {Location.TableName} l ON d.LocationID = l.LocationID
            WHERE d.StoreRecorder = @UserId
            ORDER BY f.FACode
            OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY";

        var _iSkip = (iPageIndex - 1) * iPageSize;
        _dpParameter = new DynamicParameters();
        _dpParameter.Add("@UserId", strUserId);
        _dpParameter.Add("@Skip", _iSkip);
        _dpParameter.Add("@Take", iPageSize);

        var _ienumItems = await m_DB.QueryAsync<AssetViewModel>(_strSql, _dpParameter);

        return new PagedResult<AssetViewModel>
        {
            TotalCount = _iTotal,
            Items = _ienumItems.ToList()
        };
    }

    /// <summary>
    /// 資產轉移
    /// </summary>
    /// <param name="lstItems"></param>
    /// <returns></returns>
    public async Task TransferAssetsAsync(List<TransferItem> lstItems)
    {
        try
        {
            var _strSql = @$"
                UPDATE {FADet.TableName}
                SET StoreRecorder = @ReceiverId,
                    FixDate = GETDATE(),
                    FixBy = @ReceiverId
                WHERE FACode = @AssetId";

            foreach (var item in lstItems)
            {
                await m_DB.ExecuteAsync(_strSql, item);
            }

            await m_DB.CommitAsync();
        }
        catch (Exception ex)
        {
            await m_DB.RollbackAsync();
            throw new Exception("資產移轉失敗，已回滾交易", ex);
        }
    }

    /// <summary>
    /// 取得資產資料以供匯出
    /// </summary>
    /// <param name="strUserId"></param>
    /// <returns></returns>
    public async Task<List<AssetViewModel>> GetAssetsForExportAsync(string strUserId)
    {
        var _strSql = $@"
        SELECT DISTINCT
            f.FACode AS AssetId,
            f.FACode AS AssetNumber,
            f.FAName AS AssetName,
            f.FASpec AS Spec,
            d.Qty AS Unit,
            l.LocationName AS Location,
            '' AS ReceiverName
        FROM {FA.TableName} f
        LEFT JOIN {FADet.TableName} d ON f.FACode = d.FACode
        LEFT JOIN {Location.TableName} l ON d.LocationID = l.LocationID
        WHERE d.StoreRecorder = @UserId
        ORDER BY f.FACode";

        var _dpParameter = new DynamicParameters();
        _dpParameter.Add("@UserId", strUserId);

        var result = await m_DB.QueryAsync<AssetViewModel>(_strSql, _dpParameter);
        return result.ToList();
    }

}