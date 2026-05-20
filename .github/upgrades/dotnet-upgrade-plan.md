# .NET8.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that an .NET8.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET8.0 upgrade.
3. Upgrade C:\AAA\ERD Application\ERD-Application\ERD SourceCode\ViSo.SharedEnums\ViSo.SharedEnums.csproj
4. Upgrade C:\AAA\ERD Application\ERD-Application\ERD SourceCode\GeneralExtensions\GeneralExtensions.csproj
5. Upgrade C:\AAA\ERD Application\ERD-Application\ERD SourceCode\ViSo.Common\ViSo.Common.csproj
6. Upgrade C:\AAA\ERD Application\ERD-Application\ERD SourceCode\IconSet\IconSet.csproj
7. Upgrade C:\AAA\ERD Application\ERD-Application\ERD SourceCode\ERD.Base\ERD.Base.csproj
8. Upgrade C:\AAA\ERD Application\ERD-Application\ERD SourceCode\WPF.Tools\WPF.Tools.csproj
9. Upgrade C:\AAA\ERD Application\ERD-Application\ERD SourceCode\ERD.Models\ERD.Models.csproj
10. Upgrade C:\AAA\ERD Application\ERD-Application\ERD SourceCode\ERD.Common\ERD.Common.csproj
11. Upgrade C:\AAA\ERD Application\ERD-Application\ERD SourceCode\ViSo.Dialogs\ViSo.Dialogs.csproj
12. Upgrade C:\AAA\ERD Application\ERD-Application\ERD SourceCode\ERD.DatabaseScripts\ERD.DatabaseScripts.csproj
13. Upgrade C:\AAA\ERD Application\ERD-Application\ERD SourceCode\REPORT.Data\REPORT.Data.csproj
14. Upgrade C:\AAA\ERD Application\ERD-Application\ERD SourceCode\Report.Web.Presenters\Report.Web.Presenters.csproj
15. Upgrade C:\AAA\ERD Application\ERD-Application\ERD SourceCode\REPORT.Builder\REPORT.Builder.csproj
16. Upgrade C:\AAA\ERD Application\ERD-Application\ERD SourceCode\ERD.FileManagement\ERD.FileManagement.csproj
17. Upgrade C:\AAA\ERD Application\ERD-Application\ERD SourceCode\ERD.DataExport\ERD.DataExport.csproj
18. Upgrade C:\AAA\ERD Application\ERD-Application\ERD SourceCode\ERD.Build\ERD.Build.csproj
19. Upgrade C:\AAA\ERD Application\ERD-Application\ERD SourceCode\REPORT.Web\REPORT.Web.csproj
20. Upgrade C:\AAA\ERD Application\ERD-Application\ERD SourceCode\ERD.Viewer\ERD.Viewer.csproj
21. Run unit tests to validate upgrade in the projects listed below:

## Settings

### Excluded projects

| Project name | Description |
|:-----------------------------------------------|:---------------------------:|

### Aggregate NuGet packages modifications across all projects

| Package Name | Current Version | New Version | Description |
|:------------------------------------|:---------------:|:-----------:|:----------------------------------------------|
| Antlr |3.5.0.2 |4.6.6 | Recommended replacement for Antlr3.5.0.2 |
| EntityFramework |6.4.4 |6.5.2 | Deprecated version - replace with6.5.2 |
| Microsoft.Bcl.AsyncInterfaces |6.0.0 |8.0.0 | Recommended update for .NET8.0 |
| Microsoft.Bcl.HashCode |1.1.1;1.1.0 |6.0.0 | Recommended update |
| Microsoft.Data.SqlClient.SNI |1.1.0 | | No supported version found |
| Microsoft.Extensions.* |3.1.9 |8.x.x | Update all Microsoft.Extensions packages |
| Npgsql |4.1.12 |10.0.2 | Security vulnerability - update recommended |
| Newtonsoft.Json |13.0.3 |13.0.4 | Minor update available |
| System.* (various) |4.x/6.x |8.x/10.x | Update or remove as functionality included in framework |

### Project upgrade details

#### C:\AAA\ERD Application\ERD-Application\ERD SourceCode\ViSo.SharedEnums\ViSo.SharedEnums.csproj modifications

Project properties changes:
 - Target framework should be changed from `.NETFramework,Version=v4.7.2` to `net8.0`

NuGet packages changes:
 - No package changes suggested.

Other changes:
 - Convert project file to SDK-style.

#### C:\AAA\ERD Application\ERD-Application\ERD SourceCode\GeneralExtensions\GeneralExtensions.csproj modifications

Project properties changes:
 - Target framework should be changed from `.NETFramework,Version=v4.7.2` to `net8.0-windows`

NuGet packages changes:
 - `Newtonsoft.Json` update from `13.0.3` to `13.0.4`

Other changes:
 - Convert project file to SDK-style.

... (project details continued for all projects)
