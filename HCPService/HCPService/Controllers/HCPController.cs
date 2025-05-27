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

            // 取得使用者資訊
            var _mEmployee = await m_Service.GetEmployeeAsync(UserId);
            string _strEmployeeName = _mEmployee?.Name ?? UserId;
            string _strDeptCode = _mEmployee?.DeptCode ?? "";

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var _exlPackage = new ExcelPackage();
            var ws = _exlPackage.Workbook.Worksheets.Add("資產移轉確認單");

            // 設定欄位寬度
            ws.Column(1).Width = 15;  // 資產編號
            ws.Column(2).Width = 20;  // 資產名稱
            ws.Column(3).Width = 8;   // 數量
            ws.Column(4).Width = 15;  // 存放地點
            ws.Column(5).Width = 20;  // 細項備註
            ws.Column(6).Width = 20;  // 接收人員簽名

            // 標題區域 (第1-3行)
            ws.Cells[1, 1].Value = $"原保管人員：{_strEmployeeName}";
            ws.Cells[2, 1].Value = $"原保管部門：{_strDeptCode}";

            // 主標題
            ws.Cells[1, 3, 1, 4].Merge = true;
            ws.Cells[1, 3].Value = "資產移轉確認單";
            ws.Cells[1, 3].Style.Font.Size = 16;
            ws.Cells[1, 3].Style.Font.Bold = true;
            ws.Cells[1, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            // 右上角資訊
            ws.Cells[2, 5].Value = "列印日期：" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            // 表頭 (第5行)
            var _arrHeaders = new[] { "資產編號", "資產名稱", "數量", "存放地點", "細項備註", "接收人員簽名" };
            for (int i = 0; i < _arrHeaders.Length; i++)
            {
                ws.Cells[5, i + 1].Value = _arrHeaders[i];
                ws.Cells[5, i + 1].Style.Font.Bold = true;
                ws.Cells[5, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                // 設定邊框
                ws.Cells[5, i + 1].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                ws.Cells[5, i + 1].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                ws.Cells[5, i + 1].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                ws.Cells[5, i + 1].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            }

            // 資料列 (從第6行開始)
            int _iRow = 6;
            foreach (var asset in _lstAssets)
            {
                ws.Cells[_iRow, 1].Value = asset.AssetNumber;
                ws.Cells[_iRow, 2].Value = asset.AssetName;
                ws.Cells[_iRow, 3].Value = asset.Unit;
                ws.Cells[_iRow, 4].Value = asset.Location;
                ws.Cells[_iRow, 5].Value = asset.Remark;
                ws.Cells[_iRow, 6].Value = ""; // 接收人員簽名欄位留空

                // 設定資料列邊框
                for (int col = 1; col <= 6; col++)
                {
                    ws.Cells[_iRow, col].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    ws.Cells[_iRow, col].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    ws.Cells[_iRow, col].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    ws.Cells[_iRow, col].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }

                _iRow++;
            }

            // 底部簽名區域
            int _lastRow = _iRow + 2;
            ws.Cells[_lastRow, 1].Value = "移轉人簽名：________________";
            ws.Cells[_lastRow, 4].Value = "接收人簽名：________________";

            ws.Cells[_lastRow + 1, 1].Value = "日期：________________";
            ws.Cells[_lastRow + 1, 4].Value = "日期：________________";

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
