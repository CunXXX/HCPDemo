using FrontEnd.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static Model.Model;

namespace FrontEnd.Pages;

public partial class AssetTransfer
{
    /// <summary>
    /// 注入應用程式設定服務
    /// </summary>
    [Inject] private AppSettingsService m_AppSettings { get; set; } = default!;

    /// <summary>
    /// 注入 WebAssembly 環境資訊
    /// </summary>
    [Inject] private IWebAssemblyHostEnvironment m_HostEnvironment { get; set; } = default!;

    /// <summary>
    /// 注入 HttpClient，用於發送 API 請求
    /// </summary>
    [Inject] private HttpClient m_Http { get; set; } = default!;

    /// <summary>
    /// 注入 IJSRuntime，用於調用 JavaScript 函數
    /// </summary>
    [Inject] private IJSRuntime m_JS { get; set; } = default!;

    /// <summary>
    /// 資產轉移服務，用於處理資產相關的 API 請求
    /// </summary>
    private AssetTransferService m_AssetTransferService = default!;

    /// <summary>
    /// JavaScript 模組引用，用於與前端 JavaScript 互動
    /// </summary>
    private IJSObjectReference? m_Module;

    #region 屬性和欄位

    /// <summary>
    /// 版本號
    /// </summary>
    private string m_strVersion = string.Empty;

    /// <summary>
    /// 關鍵字搜尋的使用者名稱或工號
    /// </summary>
    private string m_strKeyword = string.Empty;

    /// <summary>
    /// 當前頁數
    /// </summary>
    private int m_iPageIndex = 1;

    /// <summary>
    /// 每頁顯示的資產數量
    /// </summary>
    private int m_iPageSize = 10;

    /// <summary>
    /// 總資產數量
    /// </summary>
    private int m_iTotalCount = 0;

    /// <summary>
    /// 總頁數
    /// </summary>
    private int m_iTotalPages => (int)Math.Ceiling((double)m_iTotalCount / m_iPageSize);

    /// <summary>
    /// 是否可以翻到上一頁
    /// </summary>
    private bool m_bCanPrevious => m_iPageIndex > 1;

    /// <summary>
    /// 是否可以翻到下一頁
    /// </summary>
    private bool m_bCanNext => m_iPageIndex < m_iTotalPages;

    /// <summary>
    /// 計算全選 checkbox 的狀態
    /// </summary>
    private bool m_bIsAllSelected
    {
        get
        {
            if (m_lstAssets == null || !m_lstAssets.Any())
                return false;

            return m_lstAssets.All(x => x.IsSelected);
        }
    }

    /// <summary>
    /// 是否已完成初始化流程，並可進行畫面渲染
    /// </summary>
    private bool m_bIsReadyToRender = false;

    /// <summary>
    /// 是否已載入 JavaScript 模組（AssetTransfer.razor.js）
    /// </summary>
    private bool m_bIsModuleLoaded = false;

    /// <summary>
    /// 所有資產清單(資產轉移用)
    /// </summary>
    private List<AssetViewModel> m_lstAllAssets = new();

    /// <summary>
    /// 當前頁面顯示的資產清單
    /// </summary>
    private List<AssetViewModel> m_lstAssets = new();

    /// <summary>
    /// 員工清單
    /// </summary>
    private List<EmployeeSuggest> m_lstEmployeeSuggest = new();

    protected override async Task OnInitializedAsync()
    {
        m_AssetTransferService = new AssetTransferService(m_Http, m_AppSettings);

        await m_AppSettings.LoadAsync();

        // 根據環境決定 JS 模組的版本參數：
        // - 開發環境使用目前時間的 Ticks，以避免取舊的 JavaScript 快取
        // - 正式環境則從設定中讀取固定版本號
        if (m_HostEnvironment.IsDevelopment())
            m_strVersion = $"?v={DateTime.Now.Ticks}";
        else
            m_strVersion = m_AppSettings.Get<string>("Version");

        m_bIsReadyToRender = true;

        StateHasChanged();
    }

    protected override async Task OnAfterRenderAsync(bool bFirstRender)
    {
        if (m_bIsReadyToRender && !m_bIsModuleLoaded)
        {
            m_Module = await m_JS.InvokeAsync<IJSObjectReference>("import", $"./Pages/AssetTransfer.razor.js{m_strVersion}");
            m_bIsModuleLoaded = true;
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
        var _strKeyword = e.Value?.ToString();

        if (string.IsNullOrWhiteSpace(_strKeyword))
        {
            m_lstEmployeeSuggest.Clear();
            return;
        }

        // 如果已經存在選項中，就不要再查詢
        if (m_lstEmployeeSuggest.Any(x => x.DisplayName == _strKeyword))
            return;

        m_lstEmployeeSuggest = await m_AssetTransferService.SearchEmployees(_strKeyword);
    }

    /// <summary>
    /// 查詢接收者
    /// </summary>
    /// <param name="assetModel"></param>
    /// <param name="strKeyword"></param>
    /// <returns></returns>
    private async Task OnReceiverInput(AssetViewModel assetModel, string? strKeyword)
    {
        if (string.IsNullOrWhiteSpace(strKeyword))
            return;

        string _strParsedKeyword = strKeyword;
        if (strKeyword.Contains(" - "))
        {
            _strParsedKeyword = strKeyword.Split(" - ")[0];
        }

        var employees = await m_AssetTransferService.SearchEmployees(_strParsedKeyword);
        assetModel.ReceiverSuggests = employees;

        // 若使用者選了 DisplayName，找出對應員工
        var _mMatchedEmployee = employees.FirstOrDefault(e => e.DisplayName == strKeyword);

        if (_mMatchedEmployee != null)
        {
            assetModel.ReceiverId = _mMatchedEmployee.EmployeeNo;
            assetModel.ReceiverName = _mMatchedEmployee.Name;

            // 同步更新到 m_lstAllAssets
            var allAsset = m_lstAllAssets.FirstOrDefault(a => a.AssetId == assetModel.AssetId);
            if (allAsset != null)
            {
                allAsset.ReceiverId = _mMatchedEmployee.EmployeeNo;
                allAsset.ReceiverName = _mMatchedEmployee.Name;
                allAsset.ReceiverKeyword = strKeyword;
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
        m_iTotalCount = 0;

        var matchedEmployee = m_lstEmployeeSuggest
            .FirstOrDefault(e => e.DisplayName == m_strKeyword);

        if (matchedEmployee == null)
        {
            await m_JS.InvokeVoidAsync("alert", "找不到對應的使用者，無法查詢資產。");
            return;
        }

        var request = new AssetQueryRequest
        {
            UserId = matchedEmployee.EmployeeNo,
            PageIndex = m_iPageIndex,
            PageSize = m_iPageSize
        };

        var (success, result, errorMessage) = await m_AssetTransferService.SearchAssetsByUser(request);

        if (!success)
        {
            await m_JS.InvokeVoidAsync("alert", errorMessage);
            return;
        }

        if (result == null)
        {
            await m_JS.InvokeVoidAsync("alert", "查無資產資料，請確認工號是否正確。");
            return;
        }

        m_iTotalCount = result.TotalCount;

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

        if (m_iTotalCount == 0)
        {
            await m_JS.InvokeVoidAsync("alert", "查無資產資料，請確認工號是否正確。");
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
        if (m_bCanPrevious)
        {
            m_iPageIndex--;
            await SearchAssets();
        }
    }

    /// <summary>
    /// 下一頁
    /// </summary>
    /// <returns></returns>
    private async Task NextPage()
    {
        if (m_bCanNext)
        {
            m_iPageIndex++;
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

    /// <summary>
    /// 勾選資產
    /// </summary>
    /// <param name="strAssetId"></param>
    /// <param name="bIsSelected"></param>
    private void OnAssetCheckboxChanged(string strAssetId, bool bIsSelected)
    {
        var _mPageItem = m_lstAssets.FirstOrDefault(a => a.AssetId == strAssetId);
        if (_mPageItem != null)
            _mPageItem.IsSelected = bIsSelected;

        var _mAllItem = m_lstAllAssets.FirstOrDefault(a => a.AssetId == strAssetId);
        if (_mAllItem != null)
            _mAllItem.IsSelected = bIsSelected;
    }


    #endregion

    #region 動作相關

    /// <summary>
    /// 匯出 Excel 檔案
    /// </summary>
    /// <returns></returns>
    private async Task ExportExcel()
    {
        var matchedEmployee = m_lstEmployeeSuggest
            .FirstOrDefault(e => e.DisplayName == m_strKeyword);

        if (matchedEmployee == null)
        {
            await m_JS.InvokeVoidAsync("alert", "請先選擇有效的保管人！");
            return;
        }

        string _strUrl = $"{m_AppSettings.Get<string>("ApiBaseUrl")}/api/v1/HCP/ExportAssetTransfer?UserId={matchedEmployee.EmployeeNo}";

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
                await m_JS.InvokeVoidAsync("alert", "部分資產的接收人未正確選取，請重新確認！");
                return;
            }

            var transferItems = m_lstAllAssets
                .Where(a => a.IsSelected && !string.IsNullOrEmpty(a.ReceiverId))
                .Select(a => new TransferItem { AssetId = a.AssetId, ReceiverId = a.ReceiverId })
                .ToList();

            var (success, message) = await m_AssetTransferService.SubmitTransfers(transferItems);

            await m_JS.InvokeVoidAsync("alert", message);

            if (success)
            {
                // 移轉成功後重新查詢並回到第一頁
                m_iPageIndex = 1;
                m_lstAllAssets.Clear();
                await SearchAssets();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await m_JS.InvokeVoidAsync("alert", "發生錯誤，請稍後再試或聯絡系統管理員。");
        }
        finally
        {
            StateHasChanged();
        }
    }

    #endregion
}