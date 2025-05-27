using static Model.Model;

namespace HCPService.Repositories;

public interface IAssetRepository
{
    public Task<List<EmployeeSuggest>> SearchEmployeeAsync(string strKeyword);
    public Task<PagedResult<AssetViewModel>> GetAssetsByUserAsync(string strKeyword, int iPageIndex, int iPageSize);
    public Task TransferAssetsAsync(List<TransferItem> lstItems);
    public Task<List<AssetViewModel>> GetAssetsForExportAsync(string strKeyword);
    public Task<EmployeeSuggest?> GetEmployeeAsync(string strUserId);
}
