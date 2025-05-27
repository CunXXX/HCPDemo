namespace Model;

public class Model
{

    public class AppSettings
    {
        public string Version { get; set; } = "";
        public string ApiBaseUrl { get; set; } = "";
        public bool EnableFeatureX { get; set; } = false;
    }

    public class AssetViewModel
    {
        public string AssetId { get; set; } = string.Empty;
        public string AssetNumber { get; set; } = string.Empty;
        public string AssetName { get; set; } = string.Empty;
        public string Spec { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string? ReceiverName { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public bool IsSelected { get; set; } = true;

        public string? ReceiverKeyword { get; set; }
        public List<EmployeeSuggest> ReceiverSuggests { get; set; } = new();
    }

    public class TransferItem
    {
        public string AssetId { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
    }

    public class EmployeeSuggest
    {
        public string DisplayName => $"{EmployeeNo} - {Name}";
        public string Name { get; set; } = "";
        public string EmployeeNo { get; set; } = "";
    }

    public class AssetQueryRequest
    {
        public string UserId { get; set; } = string.Empty;
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class PagedResult<T>
    {
        public int TotalCount { get; set; } = 0;
        public List<T> Items { get; set; } = new();
    }

}
