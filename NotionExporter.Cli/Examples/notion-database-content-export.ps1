<#
    .DESCRIPTION
    Exports a database content
#>

param (
    [string]$Token,
    [string]$DatabaseId,
    [string]$ExporterPath = ".\NotionExporter.cli.exe",
    [switch]$Help,
    [switch]$Usage
)

function Show-Help
{
    Write-Host "notion-database-content-export.ps1" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Usage:"
    Write-Host "  .\notion-database-content-export.ps1 -Token <Token> -DatabaseId <ID> [-ExporterPath <path>]"
    Write-Host ""
    Write-Host "Parameters:"
    Write-Host "  -Token          Notion API token"
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

$filterJson = @"
{"filter":{"property":"Date","date":{"this_week":{}}}}
"@

$pageIds = & $ExporterPath databases export --token $Token --id $DatabaseId --filter-json $filterJson |
    ConvertFrom-Json |
    ForEach-Object { $_.results } |
    ForEach-Object { $_.id }

foreach ($pageId in $pageIds)
{
    Write-Host "Exporting blocks for page ID: $pageId" -ForegroundColor Cyan
    
    $json = & $ExporterPath blocks --token $token --id $pageId | ConvertFrom-Json

    foreach ($block in $json.results)
    {
        $type = $block.type
        if ($block.$type -and $block.$type.rich_text)
        {
            foreach ($t in $block.$type.rich_text)
            {
                $t.plain_text
            }
        }
    }
}
