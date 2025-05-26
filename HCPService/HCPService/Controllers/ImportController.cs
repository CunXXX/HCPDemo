
using Common;
using DBModel;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;


namespace AssetTransferApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImportController : ControllerBase
{
    private readonly IDBService _db;

    public ImportController(IDBService db)
    {
        _db = db;
    }

    [HttpPost("all")]
    public async Task<IActionResult> ImportAll(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("請提供 Excel 檔案");

        try
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;


            using var stream = file.OpenReadStream();
            using var package = new ExcelPackage(stream);

            await ImportSysUser(package);
            await ImportDept(package);
            await ImportSysStatus(package);
            await ImportLocation(package);
            await ImportFA(package);
            await ImportFADet(package);

            await _db.CommitAsync();
            return Ok("全部匯入成功！");
        }
        catch (Exception ex)
        {
            await _db.RollbackAsync();
            return StatusCode(500, $"匯入錯誤: {ex.Message}");
        }
    }

    private async Task ImportSysUser(ExcelPackage pkg)
    {
        var ws = pkg.Workbook.Worksheets["tblSysUser"];
        if (ws == null) return;

        for (int row = 2; row <= ws.Dimension.End.Row; row++)
        {
            var model = new SysUser
            {
                UserID = ws.Cells[row, 1].Text,
                UserName = ws.Cells[row, 2].Text,
                DeptCode = ws.Cells[row, 3].Text,
                IsNotValid = ws.Cells[row, 4].Text == "1",
                FixDate = DateTime.TryParse(ws.Cells[row, 5].Text, out var dt) ? dt : DateTime.Now,
                FixBy = ws.Cells[row, 6].Text
            };
            await _db.ExecuteAsync($@"
                INSERT INTO {SysUser.TableName} (UserID, UserName, DeptCode, IsNotValid, FixDate, FixBy)
                VALUES (@UserID, @UserName, @DeptCode, @IsNotValid, @FixDate, @FixBy)", model);
        }
    }

    private async Task ImportDept(ExcelPackage pkg)
    {
        var ws = pkg.Workbook.Worksheets["tblDept"];
        if (ws == null) return;

        for (int row = 2; row <= ws.Dimension.End.Row; row++)
        {
            var model = new Dept
            {
                DeptCode = ws.Cells[row, 1].Text,
                DeptSName = ws.Cells[row, 2].Text,
                DeptName = ws.Cells[row, 3].Text,
                Note = ws.Cells[row, 4].Text,
                FixDate = DateTime.TryParse(ws.Cells[row, 5].Text, out var dt) ? dt : DateTime.Now,
                FixBy = ws.Cells[row, 6].Text,
                IsNotValid = ws.Cells[row, 7].Text == "1"
            };
            await _db.ExecuteAsync($@"
                INSERT INTO {Dept.TableName} (DeptCode, DeptSName, DeptName, Note, FixDate, FixBy, IsNotValid)
                VALUES (@DeptCode, @DeptSName, @DeptName, @Note, @FixDate, @FixBy, @IsNotValid)", model);
        }
    }

    private async Task ImportSysStatus(ExcelPackage pkg)
    {
        var ws = pkg.Workbook.Worksheets["tblSysStatus"];
        if (ws == null) return;

        for (int row = 2; row <= ws.Dimension.End.Row; row++)
        {
            var model = new SysStatus
            {
                Status = int.TryParse(ws.Cells[row, 1].Text, out var stat) ? stat : (int)0,
                StatusName = ws.Cells[row, 2].Text
            };
            await _db.ExecuteAsync($@"
                INSERT INTO {SysStatus.TableName} (Status, StatusName)
                VALUES (@Status, @StatusName)", model);
        }
    }

    private async Task ImportLocation(ExcelPackage pkg)
    {
        var ws = pkg.Workbook.Worksheets["tblLocation"];
        if (ws == null) return;

        for (int row = 2; row <= ws.Dimension.End.Row; row++)
        {
            var model = new Location
            {
                LocationID = ws.Cells[row, 1].Text,
                LocationName = ws.Cells[row, 2].Text,
                IsNotValid = ws.Cells[row, 3].Text == "1",
                FixBy = ws.Cells[row, 4].Text,
                FixDate = DateTime.TryParse(ws.Cells[row, 5].Text, out var dt) ? dt : DateTime.Now
            };
            await _db.ExecuteAsync($@"
                INSERT INTO {Location.TableName} (LocationID, LocationName, IsNotValid, FixBy, FixDate)
                VALUES (@LocationID, @LocationName, @IsNotValid, @FixBy, @FixDate)", model);
        }
    }

    private async Task ImportFA(ExcelPackage pkg)
    {
        var ws = pkg.Workbook.Worksheets["tblFA"];
        if (ws == null) return;

        for (int row = 2; row <= ws.Dimension.End.Row; row++)
        {
            var model = new FA
            {
                FACode = ws.Cells[row, 1].Text,
                FAName = ws.Cells[row, 2].Text,
                FASpec = ws.Cells[row, 3].Text,
                Catego = ws.Cells[row, 4].Text,
                FAType = int.TryParse(ws.Cells[row, 5].Text, out var faType) ? faType : (int)0,
                BuyDate = int.TryParse(ws.Cells[row, 6].Text, out var buyDate) ? buyDate : 0,
                SupplierID = ws.Cells[row, 7].Text,
                Qty = int.TryParse(ws.Cells[row, 8].Text, out var qty) ? qty : 0,
                Unit = ws.Cells[row, 9].Text,
                Recorder = ws.Cells[row, 10].Text,
                DeptCode = ws.Cells[row, 11].Text,
                Status = int.TryParse(ws.Cells[row, 12].Text, out var stat) ? stat : (int)0,
                MainNote = ws.Cells[row, 13].Text,
                FixDate = DateTime.TryParse(ws.Cells[row, 14].Text, out var fix) ? fix : DateTime.Now,
                FixBy = ws.Cells[row, 15].Text
            };
            await _db.ExecuteAsync($@"
                INSERT INTO {FA.TableName} (FACode, FAName, FASpec, Catego, FAType, BuyDate, SupplierID, Qty, Unit, Recorder, DeptCode, Status, MainNote, FixDate, FixBy)
                VALUES (@FACode, @FAName, @FASpec, @Catego, @FAType, @BuyDate, @SupplierID, @Qty, @Unit, @Recorder, @DeptCode, @Status, @MainNote, @FixDate, @FixBy)", model);
        }
    }

    private async Task ImportFADet(ExcelPackage pkg)
    {
        var ws = pkg.Workbook.Worksheets["tblFADet"];
        if (ws == null) return;

        for (int row = 2; row <= ws.Dimension.End.Row; row++)
        {
            var model = new FADet
            {
                FACode = ws.Cells[row, 1].Text,
                StoreRecorder = ws.Cells[row, 2].Text,
                StoreDeptCode = ws.Cells[row, 3].Text,
                LocationID = ws.Cells[row, 4].Text,
                Qty = int.TryParse(ws.Cells[row, 5].Text, out var q1) ? q1 : 0,
                ExSellQty = int.TryParse(ws.Cells[row, 6].Text, out var q2) ? q2 : 0,
                ExAccQty = int.TryParse(ws.Cells[row, 7].Text, out var q3) ? q3 : 0,
                ExScrapQty = int.TryParse(ws.Cells[row, 8].Text, out var q4) ? q4 : 0,
                DetNote = ws.Cells[row, 9].Text,
                FixDate = DateTime.TryParse(ws.Cells[row, 10].Text, out var dt) ? dt : DateTime.Now,
                FixBy = ws.Cells[row, 11].Text
            };
            await _db.ExecuteAsync($@"
                INSERT INTO {FADet.TableName} (FACode, StoreRecorder, StoreDeptCode, LocationID, Qty, ExSellQty, ExAccQty, ExScrapQty, DetNote, FixDate, FixBy)
                VALUES (@FACode, @StoreRecorder, @StoreDeptCode, @LocationID, @Qty, @ExSellQty, @ExAccQty, @ExScrapQty, @DetNote, @FixDate, @FixBy)", model);
        }
    }
}
