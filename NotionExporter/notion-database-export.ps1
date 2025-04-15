
param (
    [string]$DatabaseId = "",
    [string]$ExporterPath = ".\NotionExporter\bin\Debug\net9.0\NotionExporter.exe"
)

& $ExporterPath databases --id $DatabaseId |
    ConvertFrom-Json |
    ForEach-Object { $_.results } |
    ForEach-Object {
        [PSCustomObject]@{
            Id = $_.id
            Name = $_.properties.Name.title[0].plain_text
            Url = $_.url
        }
    } | Format-Table -AutoSize
