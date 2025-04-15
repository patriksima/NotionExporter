param (
    [string]$PageId = "",
    [string]$ExporterPath = ".\NotionExporter\bin\Debug\net9.0\NotionExporter.exe"
)

$allText = & $ExporterPath blocks --id $PageId |
    ConvertFrom-Json |
    ForEach-Object { $_.results } |
    ForEach-Object {
        $block = $_
        $type = $block.type

        if ($block.$type -and $block.$type.rich_text) {
            $block.$type.rich_text | ForEach-Object {
                $_.plain_text
            }
        }
    }
$allText -join "`n"
