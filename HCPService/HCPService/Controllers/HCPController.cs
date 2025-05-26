using HCPService.Services;
using Microsoft.AspNetCore.Mvc;
using static Model.Model;

namespace HCPService.Controllers;


[ApiController]
[Route("api/v1/[controller]/[action]")]
public class HCPController : ControllerBase
{
    private readonly AssetService m_Service;

    public HCPController(AssetService service)
    {
        m_Service = service;
    }

    [HttpPost]
    public async Task<IActionResult> SearchAssetsByUserAsync([FromBody] AssetQueryRequest Request)
    {
        try
        {
            var result = await m_Service.SearchAssetsByOwnerAsync(Request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, new { message = "查詢資產資料失敗，請聯絡系統管理員。" });
        }
    }

    [HttpPost("batch-transfer")]
    public async Task<IActionResult> TransferAssets([FromBody] List<TransferItem> lstItems)
    {
        await m_Service.TransferAssetsAsync(lstItems);
        return Ok();
    }

}
