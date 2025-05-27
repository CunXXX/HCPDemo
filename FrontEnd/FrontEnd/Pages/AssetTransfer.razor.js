export async function downloadExcel(url) {
    const response = await fetch(url);
    const blob = await response.blob();

    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = "資產移轉確認單.xlsx";
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}