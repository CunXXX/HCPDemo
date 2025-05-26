using static Model.Model;

namespace HCPService.Repositories;

public interface IAssetRepository
{
    public Task<PagedResult<AssetViewModel>> GetAssetsByUserAsync(string strUserId, int iPageIndex, int iPageSize);
    public Task TransferAssetsAsync(List<TransferItem> lstItems);
}
