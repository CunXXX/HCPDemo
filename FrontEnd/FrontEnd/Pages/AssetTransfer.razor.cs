using FrontEnd.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static Model.Model;

namespace FrontEnd.Pages;

public partial class AssetTransfer
{
    [Inject] private AppSettingsService m_AppSettings { get; set; } = default!;
    [Inject] private IWebAssemblyHostEnvironment m_HostEnvironment { get; set; } = default!;
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;

    private AssetTransferService m_AssetTransferService = default!;
    private IJSObjectReference? m_Module;

    #region 屬性和欄位

    private string m_sVersion = string.Empty;
    private string m_strKeyword = string.Empty;

    private int m_PageIndex = 1;
    private int m_PageSize = 1;
    private int m_TotalCount = 0;
    private int TotalPages => (int)Math.Ceiling((double)m_TotalCount / m_PageSize);

    private bool CanPrevious => m_PageIndex > 1;
    private bool CanNext => m_PageIndex < TotalPages;

    /// <summary>
    /// 所有資產清單(資產轉移用)
    /// </summary>
    private List<AssetViewModel> m_lstAllAssets = new();

    /// <summary>
    /// 當前頁面顯示的資產清單
    /// </summary>
    private List<AssetViewModel> m_lstAssets = new();

    /// <summary>
    /// 使用者清單
    /// </summary>
    private List<EmployeeSuggest> m_lstEmployeeSuggest = new();

    protected override async Task OnInitializedAsync()
    {
        m_AssetTransferService = new AssetTransferService(Http, m_AppSettings);

        if (m_HostEnvironment.IsDevelopment())
            m_sVersion = $"?v={DateTime.Now.Ticks}";
        else
            m_sVersion = m_AppSettings.Get<string>("Version");

        await m_AppSettings.LoadAsync();
    }

    protected override async Task OnAfterRenderAsync(bool bFirstRender)
    {
        if (bFirstRender)
        {
            m_Module = await JS.InvokeAsync<IJSObjectReference>("import", $"./Pages/AssetTransfer.razor.js{m_sVersion}");
        }
    }

    #endregion

    #region 員工搜尋相關

    /// <summary>
    /// 查詢使用者
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    private async Task OnKeywordInput(ChangeEventArgs e)
    {
        var keyword = e.Value?.ToString();

        if (string.IsNullOrWhiteSpace(keyword))
            return;

        // 如果已經存在選項中，就不要再查詢
        if (m_lstEmployeeSuggest.Any(x => x.DisplayName == keyword))
            return;

        m_lstEmployeeSuggest = await m_AssetTransferService.SearchEmployeesAsync(keyword);
    }

    /// <summary>
    /// 查詢接收者
    /// </summary>
    /// <param name="assetModel"></param>
    /// <param name="keyword"></param>
    /// <returns></returns>
    private async Task OnReceiverInput(AssetViewModel assetModel, string? keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return;

        string parsedKeyword = keyword;
        if (keyword.Contains(" - "))
        {
            parsedKeyword = keyword.Split(" - ")[0];
        }

        var employees = await m_AssetTransferService.SearchEmployeesAsync(parsedKeyword);
        assetModel.ReceiverSuggests = employees;

        // 若使用者選了 DisplayName，找出對應員工
        var matchedEmployee = employees.FirstOrDefault(e => e.DisplayName == keyword);

        if (matchedEmployee != null)
        {
            assetModel.ReceiverId = matchedEmployee.EmployeeNo;
            assetModel.ReceiverName = matchedEmployee.Name;

            // 同步更新到 m_lstAllAssets
            var allAsset = m_lstAllAssets.FirstOrDefault(a => a.AssetId == assetModel.AssetId);
            if (allAsset != null)
            {
                allAsset.ReceiverId = matchedEmployee.EmployeeNo;
                allAsset.ReceiverName = matchedEmployee.Name;
                allAsset.ReceiverKeyword = keyword;
                allAsset.ReceiverSuggests = employees;
            }
        }
        else
        {
            assetModel.ReceiverId = string.Empty;
            assetModel.ReceiverName = string.Empty;
        }
    }

    #endregion

    #region 資產搜尋相關

    /// <summary>
    /// 查詢資產
    /// </summary>
    /// <returns></returns>
    private async Task SearchAssets()
    {
        m_lstAssets.Clear();
        m_TotalCount = 0;

        var matchedEmployee = m_lstEmployeeSuggest
            .FirstOrDefault(e => e.DisplayName == m_strKeyword);

        if (matchedEmployee == null)
        {
            await JS.InvokeVoidAsync("alert", "找不到對應的使用者，無法查詢資產。");
            return;
        }

        var request = new AssetQueryRequest
        {
            UserId = matchedEmployee.EmployeeNo,
            PageIndex = m_PageIndex,
            PageSize = m_PageSize
        };

        var (success, result, errorMessage) = await m_AssetTransferService.SearchAssetsByUserAsync(request);

        if (!success)
        {
            await JS.InvokeVoidAsync("alert", errorMessage);
            return;
        }

        if (result == null)
        {
            await JS.InvokeVoidAsync("alert", "查無資產資料，請確認工號是否正確。");
            return;
        }

        m_TotalCount = result.TotalCount;

        // 合併狀態的邏輯
        m_lstAssets = result.Items
            .Select(newAsset =>
            {
                // 從全部資產清單中查找是否已經存在
                var existingAsset = m_lstAllAssets.FirstOrDefault(a => a.AssetId == newAsset.AssetId);
                if (existingAsset != null)
                {
                    // 如果存在，保留舊的狀態
                    newAsset.IsSelected = existingAsset.IsSelected;
                    newAsset.ReceiverId = existingAsset.ReceiverId;
                    newAsset.ReceiverName = existingAsset.ReceiverName;
                    newAsset.ReceiverKeyword = existingAsset.ReceiverKeyword;
                    newAsset.ReceiverSuggests = existingAsset.ReceiverSuggests;
                }
                return newAsset;
            }).ToList();

        // 更新 m_lstAllAssets：移除當前頁的舊資料，然後加入新的
        var currentPageAssetIds = m_lstAssets.Select(x => x.AssetId).ToList();
        m_lstAllAssets.RemoveAll(a => currentPageAssetIds.Contains(a.AssetId));
        m_lstAllAssets.AddRange(m_lstAssets);

        if (m_TotalCount == 0)
        {
            await JS.InvokeVoidAsync("alert", "查無資產資料，請確認工號是否正確。");
        }
    }

    #endregion

    #region 分頁相關

    /// <summary>
    /// 上一頁
    /// </summary>
    /// <returns></returns>
    private async Task PreviousPage()
    {
        if (CanPrevious)
        {
            m_PageIndex--;
            await SearchAssets();
        }
    }

    /// <summary>
    /// 下一頁
    /// </summary>
    /// <returns></returns>
    private async Task NextPage()
    {
        if (CanNext)
        {
            m_PageIndex++;
            await SearchAssets();
        }
    }

    #endregion

    #region 選擇相關

    /// <summary>
    /// 全選
    /// </summary>
    /// <param name="e"></param>
    private void ToggleSelectAll(ChangeEventArgs e)
    {
        bool isChecked = (bool)e.Value!;

        foreach (var asset in m_lstAssets)
        {
            asset.IsSelected = isChecked;

            var allAsset = m_lstAllAssets.FirstOrDefault(a => a.AssetId == asset.AssetId);
            if (allAsset != null)
                allAsset.IsSelected = isChecked;
        }
    }

    private void OnAssetCheckboxChanged(string assetId, bool isSelected)
    {
        var pageItem = m_lstAssets.FirstOrDefault(a => a.AssetId == assetId);
        if (pageItem != null)
            pageItem.IsSelected = isSelected;

        var allItem = m_lstAllAssets.FirstOrDefault(a => a.AssetId == assetId);
        if (allItem != null)
            allItem.IsSelected = isSelected;
    }

    #endregion

    #region 動作相關

    private async Task ExportExcel()
    {
        var matchedEmployee = m_lstEmployeeSuggest
            .FirstOrDefault(e => e.DisplayName == m_strKeyword);

        if (matchedEmployee == null)
        {
            await JS.InvokeVoidAsync("alert", "請先選擇有效的保管人！");
            return;
        }

        string _strUrl = $"{m_AppSettings.Get<string>("ApiBaseUrl")}/api/v1/HCP/ExportAssetTransfer?userId={matchedEmployee.EmployeeNo}";

        if (m_Module != null)
        {
            await m_Module.InvokeVoidAsync("downloadExcel", _strUrl);
        }
    }

    /// <summary>
    /// 送出移轉
    /// </summary>
    /// <returns></returns>
    private async Task SubmitTransfers()
    {
        try
        {
            // 檢查是否有沒選接收人的資產
            if (m_lstAllAssets.Any(a => a.IsSelected && string.IsNullOrWhiteSpace(a.ReceiverId)))
            {
                await JS.InvokeVoidAsync("alert", "部分資產的接收人未正確選取，請重新確認！");
                return;
            }

            var transferItems = m_lstAllAssets
                .Where(a => a.IsSelected && !string.IsNullOrEmpty(a.ReceiverId))
                .Select(a => new TransferItem { AssetId = a.AssetId, ReceiverId = a.ReceiverId })
                .ToList();

            var (success, message) = await m_AssetTransferService.SubmitTransfersAsync(transferItems);

            await JS.InvokeVoidAsync("alert", message);

            if (success)
            {
                // 移轉成功後重新查詢並回到第一頁
                m_PageIndex = 1;
                m_lstAllAssets.Clear();
                await SearchAssets();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await JS.InvokeVoidAsync("alert", "發生錯誤，請稍後再試或聯絡系統管理員。");
        }
        finally
        {
            StateHasChanged();
        }
    }

    #endregion
}