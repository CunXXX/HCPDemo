using DBModel;
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

    /// <summary>
    /// 使用者查詢
    /// </summary>
    /// <param name="strKeyword"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> SearchEmployeeAsync([FromQuery] string strKeyword)
    {
        if (string.IsNullOrWhiteSpace(strKeyword))
            return Ok(new List<EmployeeSuggest>());

        try
        {
            if (string.IsNullOrWhiteSpace(strKeyword))
                return Ok(new List<EmployeeSuggest>());

            var result = await m_Service.SearchEmployeeAsync(strKeyword);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, new { message = "查詢資產資料失敗，請聯絡系統管理員。" });
        }
    }

    /// <summary>
    /// 資產查詢
    /// </summary>
    /// <param name="Request"></param>
    /// <returns></returns>
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

    [HttpPost]
    public async Task<IActionResult> TransferAssets([FromBody] List<TransferItem> lstItems)
    {
        await m_Service.TransferAssetsAsync(lstItems);
        return Ok();
    }

}
