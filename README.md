# NotionExporter.exe

**NotionExporter** is a Windows command-line tool built with .NET that exports data from the [Notion API](https://developers.notion.com/) into JSON format. It supports exporting databases, pages, blocks, and allows for custom filters and sorting via query files.

---

## 🚀 Features

- ✅ Export Notion **databases**, **pages**, and **blocks**
- ✅ Output to **JSON file** or **standard output**
- ✅ Support for Notion **filters and sorts**
- ✅ Accepts Notion **API token** from CLI, environment variables, config file, or secure prompt
- ✅ Clean and modular CLI built with [`Spectre.Console.Cli`](https://spectreconsole.net/cli/)

---

## 🛠️ Requirements

- [.NET 9 SDK (preview)](https://dotnet.microsoft.com/) or newer
- Windows 10 or higher

---

## ⚙️ Build & Run

1. Build the application:

```bash
dotnet publish -c Release
```

2. Run it:

```bash
.\NotionExporter\bin\Release\net9.0\publish\NotionExporter.exe [command] [options]
```

---

## 📦 Usage

### 🔸 Export a database to a file

```powershell
.\NotionExporter.exe databases --id 1234abcd --output data.json
```

### 🔸 Export a database to standard output

```powershell
.\NotionExporter.exe databases --id 1234abcd
```

### 🔸 Use a custom filter query

```powershell
.\NotionExporter.exe databases --id 1234abcd --filter-file query.json
```

**Example `query.json`:**

```json
{
  "filter": {
    "property": "Date",
    "date": {
      "this_week": {}
    }
  },
  "sorts": [
    {
      "property": "Name",
      "direction": "ascending"
    }
  ]
}
```

---

## 🔐 Getting a Notion API Token

1. Create a [Notion integration](https://developers.notion.com/docs/create-a-notion-integration)
2. Obtain your **Internal Integration Token**
3. Use the token in one of the following ways:
    - As CLI argument: `--token <your-token>`
    - As environment variable: `NOTION_API_TOKEN`
    - Or enter it securely via prompt if not provided

---

## 🧪 PowerShell Example: Filter and Format Output

```powershell
.\NotionExporter.exe databases --id 1234abcd |
    ConvertFrom-Json |
    % { $_.results } |
    % { [PSCustomObject]@{ Name = $_.properties.Name.title[0].plain_text } } |
    Format-Table -AutoSize
```

---

## 📂 Configuration (Optional)

You can provide configuration via:

- `appsettings.json`
- Environment variables


---

## 🤝 Contributing

Pull requests are welcome!  
Feel free to open [issues](https://github.com/patriksima/NotionExporter/issues) for bugs, suggestions, or feedback.

---

## 📄 License

This project is licensed under the **MIT License**. See [LICENSE](./LICENSE) for details.
