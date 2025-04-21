<#
    .DESCRIPTION
    Exports a page content
#>

param (
    [string]$PageId,
    [string]$ExporterPath = ".\NotionExporter.cli.exe",
    [switch]$Help,
    [switch]$Usage
)

function Show-Help
{
    Write-Host "notion-page-export.ps1" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Usage:"
    Write-Host "  .\notion-page-export.ps1 -PageId <ID> [-ExporterPath <path>]"
    Write-Host ""
    Write-Host "Parameters:"
    Write-Host "  -PageId         ID of the Notion page to export from (required)"
    Write-Host "  -ExporterPath   Path to NotionExporter.cli.exe (default: .\NotionExporter.cli.exe)"
    Write-Host "  -Help, -Usage   Show this help message"
    Write-Host ""
}

if ($Help -or $Usage -or [string]::IsNullOrWhiteSpace($PageId))
{
    Show-Help
    return
}

$allText = & $ExporterPath blocks --id $PageId |
        ConvertFrom-Json |
        ForEach-Object { $_.results } |
        ForEach-Object {
            $block = $_
            $type = $block.type

            if ($block.$type -and $block.$type.rich_text)
            {
                $block.$type.rich_text | ForEach-Object {
                    $_.plain_text
                }
            }
        }
$allText -join "`n"
