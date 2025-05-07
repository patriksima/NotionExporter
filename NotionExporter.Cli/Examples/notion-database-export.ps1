<#
    .DESCRIPTION
    Exports a database
#>

param (
    [string]$DatabaseId,
    [string]$ExporterPath = ".\NotionExporter.cli.exe",
    [switch]$Help,
    [switch]$Usage
)

function Show-Help
{
    Write-Host "notion-database-export.ps1" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Usage:"
    Write-Host "  .\notion-database-export.ps1 -DatabaseId <ID> [-ExporterPath <path>]"
    Write-Host ""
    Write-Host "Parameters:"
    Write-Host "  -DatabaseId     ID of the Notion database to export from (required)"
    Write-Host "  -ExporterPath   Path to NotionExporter.cli.exe (default: .\NotionExporter.cli.exe)"
    Write-Host "  -Help, -Usage   Show this help message"
    Write-Host ""
}

if ($Help -or $Usage -or [string]::IsNullOrWhiteSpace($DatabaseId))
{
    Show-Help
    return
}

& $ExporterPath databases export --id $DatabaseId |
        ConvertFrom-Json |
        ForEach-Object {
            $_.results
        } |
        ForEach-Object {
            [PSCustomObject]@{
                Id = $_.id
                Name = $_.properties.Name.title[0].plain_text
                Url = $_.url
            }
        } | Format-Table -AutoSize
