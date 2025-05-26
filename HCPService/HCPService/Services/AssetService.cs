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

    public Task<PagedResult<AssetViewModel>> SearchAssetsByOwnerAsync(AssetQueryRequest Request)
    {
        return m_Repo.GetAssetsByUserAsync(Request.UserId, Request.PageIndex, Request.PageSize);
    }

    public Task TransferAssetsAsync(List<TransferItem> lstItems)
    {
        return m_Repo.TransferAssetsAsync(lstItems);
    }
}
