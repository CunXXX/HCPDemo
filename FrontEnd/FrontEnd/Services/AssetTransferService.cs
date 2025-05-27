using System.Net.Http.Json;
using static Model.Model;

namespace FrontEnd.Services;

public class AssetTransferService
{
    private readonly HttpClient m_HttpClient;
    private readonly AppSettingsService m_AppSettings;

    public AssetTransferService(HttpClient httpClient, AppSettingsService appSettings)
    {
        m_HttpClient = httpClient;
        m_AppSettings = appSettings;
    }

    /// <summary>
    /// 搜尋員工建議清單
    /// </summary>
    /// <param name="strKeyword">關鍵字</param>
    /// <returns>員工建議清單</returns>
    public async Task<List<EmployeeSuggest>> SearchEmployees(string strKeyword)
    {
        if (string.IsNullOrWhiteSpace(strKeyword))
            return new List<EmployeeSuggest>();

        try
        {
            var _strApiUrl = m_AppSettings.Get<string>("ApiBaseUrl");
            var result = await m_HttpClient.GetFromJsonAsync<List<EmployeeSuggest>>(
                $"{_strApiUrl}/api/v1/HCP/SearchEmployee?Keyword={strKeyword}"
            );

            return result ?? new List<EmployeeSuggest>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"搜尋員工失敗: {ex.Message}");
            return new List<EmployeeSuggest>();
        }
    }

    /// <summary>
    /// 根據使用者搜尋資產
    /// </summary>
    /// <param name="request">查詢請求</param>
    /// <returns>分頁資產結果</returns>
    public async Task<(bool Success, PagedResult<AssetViewModel>? Result, string ErrorMessage)> SearchAssetsByUser(AssetQueryRequest request)
    {
        try
        {
            var _strApiUrl = m_AppSettings.Get<string>("ApiBaseUrl");
            var response = await m_HttpClient.PostAsJsonAsync($"{_strApiUrl}/api/v1/HCP/SearchAssetsByUser", request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return (false, null, $"查詢失敗：{error}");
            }

            var result = await response.Content.ReadFromJsonAsync<PagedResult<AssetViewModel>>();
            if (result == null || result.Items == null)
            {
                return (false, null, "查無資料，請確認查詢條件。");
            }

            return (true, result, string.Empty);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"搜尋資產失敗: {ex.Message}");
            return (false, null, "發生錯誤，請稍後再試或聯絡系統管理員。");
        }
    }

    /// <summary>
    /// 提交資產移轉
    /// </summary>
    /// <param name="transferItems">移轉項目清單</param>
    /// <returns>移轉結果</returns>
    public async Task<(bool Success, string Message)> SubmitTransfers(List<TransferItem> transferItems)
    {
        try
        {
            if (transferItems == null || transferItems.Count == 0)
            {
                return (false, "請至少選擇一筆要移轉的資產！");
            }

            var _strApiUrl = m_AppSettings.Get<string>("ApiBaseUrl");
            var response = await m_HttpClient.PostAsJsonAsync($"{_strApiUrl}/api/v1/HCP/TransferAssets", transferItems);

            if (response.IsSuccessStatusCode)
            {
                return (true, "資產移轉成功！");
            }
            else
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                return (false, $"資產移轉失敗：{errorMsg}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"提交移轉失敗: {ex.Message}");
            return (false, "發生錯誤，請稍後再試或聯絡系統管理員。");
        }
    }
}