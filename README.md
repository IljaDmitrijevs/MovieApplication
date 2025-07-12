# 🎬 Movie Finder

A Blazor WebAssembly app with a .NET Web API backend for searching movies using the OMDb API, displaying details, and tracking recent searches in a database.

## 📁 Project Structure
MovieApplication/
├── Client/ # Blazor WebAssembly frontend
├── MovieApplicationApi/ # ASP.NET Core Web API
├── MovieApplicationTests/ # Unit and integration tests

## 🚀 Getting Started

### ✅ Prerequisites

- [.NET 7+ SDK](https://dotnet.microsoft.com/download)
- Visual Studio 2022+ or VS Code
- Optional: SQLite or SQL Server installed locally

---

## ⚙️ Configuration

### 🔑 OMDb API Key

1. Sign up at [OMDb API](https://www.omdbapi.com/apikey.aspx) and get your API key.
2. Add the key to the server's configuration in `appsettings.json` in API project:

```json
"OmdbSettings": {
  "ApiKey": "your-api-key-here",
  "BaseUrl": "https://www.omdbapi.com/"
}
"ConnectionStrings": {
  "DBConnection": "Server=(localdb)\\mssqllocaldb;Database=MovieFinderDb;Trusted_Connection=True;"
}
 ```
▶️ Run the Application:
1. dotnet restore
2. dotnet ef database update --project MovieApplicationApi
3. dotnet run --project MovieApplicationApi
4. dotnet run --project Client

To update the API address, edit `appsettings.Development.json` in the Blazor project:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7136" -- Update it your local api adress you can check it launchSettings.json in API project https: "applicationUrl" or just start API project and copt from browser.
  }
}
```
