using DBModel;
using HCPService.Services;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
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
    /// <param name="Keyword"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> SearchEmployee([FromQuery] string Keyword)
    {
        if (string.IsNullOrWhiteSpace(Keyword))
            return Ok(new List<EmployeeSuggest>());

        try
        {
            if (string.IsNullOrWhiteSpace(Keyword))
                return Ok(new List<EmployeeSuggest>());

            var result = await m_Service.SearchEmployeeAsync(Keyword);
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
    public async Task<IActionResult> SearchAssetsByUser([FromBody] AssetQueryRequest Request)
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

    /// <summary>
    /// 資產轉移
    /// </summary>
    /// <param name="lstItems"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> TransferAssets([FromBody] List<TransferItem> lstItems)
    {
        try
        {
            await m_Service.TransferAssetsAsync(lstItems);
            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, new { message = "轉移資產資料失敗，請聯絡系統管理員。" });
        }
    }

    /// <summary>
    /// 匯出資產移轉確認單
    /// </summary>
    /// <param name="strUserId"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> ExportAssetTransfer([FromQuery] string UserId)
    {
        try
        {
            var _lstAssets = await m_Service.GetAssetsForExportAsync(UserId); 

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var _exlPackage = new ExcelPackage();
            var ws = _exlPackage.Workbook.Worksheets.Add("資產移轉確認單");

            // 標題列
            var _arrHeaders = new[] { "資產編號", "資產名稱", "單位", "使用地點", "細項備註", "接收人員簽名" };
            for (int i = 0; i < _arrHeaders.Length; i++)
            {
                ws.Cells[1, i + 1].Value = _arrHeaders[i];
                ws.Cells[1, i + 1].Style.Font.Bold = true;
            }

            // 資料列
            int _iRow = 2;
            foreach (var asset in _lstAssets)
            {
                ws.Cells[_iRow, 1].Value = asset.AssetNumber;
                ws.Cells[_iRow, 2].Value = asset.AssetName;
                ws.Cells[_iRow, 3].Value = asset.Unit;
                ws.Cells[_iRow, 4].Value = asset.Location;
                ws.Cells[_iRow, 5].Value = asset.Remark;
                ws.Cells[_iRow, 6].Value = "";
                _iRow++;
            }

            ws.Cells.AutoFitColumns();

            var _mStream = new MemoryStream();
            _exlPackage.SaveAs(_mStream);
            _mStream.Position = 0;

            return File(_mStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "資產移轉確認單.xlsx");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "匯出失敗，請稍後再試");
        }
    }

}
