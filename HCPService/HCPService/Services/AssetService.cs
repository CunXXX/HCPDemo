using HCPService.Repositories;
using static Model.Model;

namespace HCPService.Services;

public class AssetService
{
    private readonly IAssetRepository m_Repo;

    public AssetService(IAssetRepository repo)
    {
        m_Repo = repo;
    }

    public Task<List<EmployeeSuggest>> SearchEmployeeAsync(string strKeyword)
    {
        if (string.IsNullOrWhiteSpace(strKeyword))
            return Task.FromResult(new List<EmployeeSuggest>());
        return m_Repo.SearchEmployeeAsync(strKeyword);
    }

    public Task<PagedResult<AssetViewModel>> SearchAssetsByOwnerAsync(AssetQueryRequest Request)
    {
        return m_Repo.GetAssetsByUserAsync(Request.UserId, Request.PageIndex, Request.PageSize);
    }

    public Task TransferAssetsAsync(List<TransferItem> lstItems)
    {
        return m_Repo.TransferAssetsAsync(lstItems);
    }

    public Task<List<AssetViewModel>> GetAssetsForExportAsync(string strUserId)
    {
        return m_Repo.GetAssetsForExportAsync(strUserId);
    }

    public Task<EmployeeSuggest?> GetEmployeeAsync(string strUserId)
    {
        return m_Repo.GetEmployeeAsync(strUserId);
    }
}
