# .NET 10.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that an .NET 10.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10.0 upgrade.
3. Upgrade ViSo.SharedEnums\ViSo.SharedEnums.csproj
4. Upgrade GeneralExtensions\GeneralExtensions.csproj
5. Upgrade IconSet\IconSet.csproj
6. Upgrade ViSo.Common\ViSo.Common.csproj
7. Upgrade ERD.Base\ERD.Base.csproj
8. Upgrade WPF.Tools\WPF.Tools.csproj
9. Upgrade ERD.Models\ERD.Models.csproj
10. Upgrade ERD.Common\ERD.Common.csproj
11. Upgrade ViSo.Dialogs\ViSo.Dialogs.csproj
12. Upgrade ERD.DatabaseScripts\ERD.DatabaseScripts.csproj
13. Upgrade REPORT.Data\REPORT.Data.csproj
14. Upgrade ERD.FileManagement\ERD.FileManagement.csproj
15. Upgrade ERD.Build\ERD.Build.csproj
16. Upgrade ERD.DataExport\ERD.DataExport.csproj
17. Upgrade REPORT.Builder\REPORT.Builder.csproj
18. Upgrade Report.Web.Presenters\Report.Web.Presenters.csproj
19. Upgrade ERD.Viewer\ERD.Viewer.csproj

## Settings

This section contains settings and data used by execution steps.

### Excluded projects

Table below contains projects that do belong to the dependency graph for selected projects and should not be included in the upgrade.

| Project name | Description |
|:-------------|:------------|

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                                           | Current Version | New Version | Description                                      |
|:-------------------------------------------------------|:---------------:|:-----------:|:-------------------------------------------------|
| Microsoft.Bcl.AsyncInterfaces                         | 8.0.0           | 10.0.10     | Recommended for .NET 10.0                       |
| Microsoft.Extensions.Caching.Abstractions              | 8.0.0           | 10.0.10     | Recommended for .NET 10.0                       |
| Microsoft.Extensions.Caching.Memory                    | 8.0.1           | 10.0.10     | Recommended for .NET 10.0                       |
| Microsoft.Extensions.Configuration                     | 8.0.0           | 10.0.10     | Recommended for .NET 10.0                       |
| Microsoft.Extensions.Configuration.Abstractions        | 8.0.0           | 10.0.10     | Recommended for .NET 10.0                       |
| Microsoft.Extensions.Configuration.Binder              | 8.0.2           | 10.0.10     | Recommended for .NET 10.0                       |
| Microsoft.Extensions.DependencyInjection               | 8.0.1           | 10.0.10     | Recommended for .NET 10.0                       |
| Microsoft.Extensions.DependencyInjection.Abstractions  | 8.0.2           | 10.0.10     | Recommended for .NET 10.0                       |
| Microsoft.Extensions.Logging                           | 8.0.1           | 10.0.10     | Recommended for .NET 10.0                       |
| Microsoft.Extensions.Logging.Abstractions              | 8.0.3           | 10.0.10     | Recommended for .NET 10.0                       |
| Microsoft.Extensions.Options                           | 8.0.2           | 10.0.10     | Recommended for .NET 10.0                       |
| Microsoft.Extensions.Primitives                        | 8.0.0           | 10.0.10     | Recommended for .NET 10.0                       |
| Microsoft.Identity.Client                              | 3.0.8           | 4.87.0      | Deprecated package version; update recommended  |
| System.Buffers                                         | 4.5.1           |             | Remove; functionality included in framework     |
| System.Collections.Immutable                           | 10.0.8          | 10.0.10     | Recommended for .NET 10.0                       |
| System.ComponentModel.Annotations                      | 5.0.0           |             | Remove; functionality included in framework     |
| System.Configuration.ConfigurationManager              | 10.0.8          | 10.0.10     | Recommended for .NET 10.0                       |
| System.Data.Common                                     | 4.3.0           |             | Remove; functionality included in framework     |
| System.Diagnostics.DiagnosticSource                    | 9.0.11          | 10.0.10     | Recommended for .NET 10.0                       |
| System.Drawing.Common                                  | 8.0.0           | 10.0.10     | Recommended for .NET 10.0                       |
| System.Management                                      | 8.0.0           | 10.0.10     | Recommended for .NET 10.0                       |
| System.Memory                                          | 4.5.4           |             | Remove; functionality included in framework     |
| System.Numerics.Vectors                                | 4.5.0           |             | Remove; functionality included in framework     |
| System.Runtime.Serialization.Formatters                | 10.0.8          |             | Remove; functionality included in framework     |
| System.Text.Encodings.Web                              | 10.0.8          | 10.0.10     | Recommended for .NET 10.0                       |
| System.Text.Json                                       | 10.0.8          | 10.0.10     | Recommended for .NET 10.0                       |
| System.Threading.Channels                              | 6.0.0           | 10.0.10     | Recommended for .NET 10.0                       |
| System.Threading.Tasks.Extensions                      | 4.5.4           |             | Remove; functionality included in framework     |

### Project upgrade details

#### ViSo.SharedEnums\ViSo.SharedEnums.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - No package changes suggested.

#### GeneralExtensions\GeneralExtensions.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0-windows` to `net10.0-windows`

NuGet packages changes:
  - `System.Runtime.Serialization.Formatters` should be removed because functionality is included in the framework reference.

#### IconSet\IconSet.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0-windows` to `net10.0-windows`

NuGet packages changes:
  - `System.Drawing.Common` should be updated from `8.0.0` to `10.0.10`.

#### ViSo.Common\ViSo.Common.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0-windows7.0` to `net10.0-windows7.0`

NuGet packages changes:
  - `System.Configuration.ConfigurationManager` should be updated from `10.0.8` to `10.0.10`.

#### ERD.Base\ERD.Base.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - No package changes suggested.

#### WPF.Tools\WPF.Tools.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0-windows` to `net10.0-windows`

NuGet packages changes:
  - No package changes suggested.

#### ERD.Models\ERD.Models.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0-windows` to `net10.0-windows`

NuGet packages changes:
  - No package changes suggested.

#### ERD.Common\ERD.Common.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0-windows` to `net10.0-windows`

NuGet packages changes:
  - `System.Management` should be updated from `8.0.0` to `10.0.10`.

#### ViSo.Dialogs\ViSo.Dialogs.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0-windows` to `net10.0-windows`

NuGet packages changes:
  - No package changes suggested.

#### ERD.DatabaseScripts\ERD.DatabaseScripts.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0-windows` to `net10.0-windows`

NuGet packages changes:
  - `Microsoft.Bcl.AsyncInterfaces` should be updated from `8.0.0` to `10.0.10`.
  - `System.Collections.Immutable` should be updated from `10.0.8` to `10.0.10`.
  - `System.Diagnostics.DiagnosticSource` should be updated from `9.0.11` to `10.0.10`.
  - `System.Text.Encodings.Web` should be updated from `10.0.8` to `10.0.10`.
  - `System.Text.Json` should be updated from `10.0.8` to `10.0.10`.
  - `System.Threading.Channels` should be updated from `6.0.0` to `10.0.10`.
  - `System.Buffers` should be removed because functionality is included in the framework reference.
  - `System.Memory` should be removed because functionality is included in the framework reference.
  - `System.Numerics.Vectors` should be removed because functionality is included in the framework reference.
  - `System.Threading.Tasks.Extensions` should be removed because functionality is included in the framework reference.

#### REPORT.Data\REPORT.Data.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0-windows` to `net10.0-windows`

NuGet packages changes:
  - `Microsoft.Bcl.AsyncInterfaces` should be updated from `8.0.0` to `10.0.10`.
  - `Microsoft.Extensions.Caching.Abstractions` should be updated from `8.0.0` to `10.0.10`.
  - `Microsoft.Extensions.Caching.Memory` should be updated from `8.0.1` to `10.0.10`.
  - `Microsoft.Extensions.Configuration` should be updated from `8.0.0` to `10.0.10`.
  - `Microsoft.Extensions.Configuration.Abstractions` should be updated from `8.0.0` to `10.0.10`.
  - `Microsoft.Extensions.Configuration.Binder` should be updated from `8.0.2` to `10.0.10`.
  - `Microsoft.Extensions.DependencyInjection` should be updated from `8.0.1` to `10.0.10`.
  - `Microsoft.Extensions.DependencyInjection.Abstractions` should be updated from `8.0.2` to `10.0.10`.
  - `Microsoft.Extensions.Logging` should be updated from `8.0.1` to `10.0.10`.
  - `Microsoft.Extensions.Logging.Abstractions` should be updated from `8.0.3` to `10.0.10`.
  - `Microsoft.Extensions.Options` should be updated from `8.0.2` to `10.0.10`.
  - `Microsoft.Extensions.Primitives` should be updated from `8.0.0` to `10.0.10`.
  - `System.Collections.Immutable` should be updated from `10.0.8` to `10.0.10`.
  - `System.Diagnostics.DiagnosticSource` should be updated from `9.0.11` to `10.0.10`.
  - `Microsoft.Identity.Client` should be updated from `3.0.8` to `4.87.0` because the current package version is deprecated.
  - `System.Buffers` should be removed because functionality is included in the framework reference.
  - `System.ComponentModel.Annotations` should be removed because functionality is included in the framework reference.
  - `System.Data.Common` should be removed because functionality is included in the framework reference.
  - `System.Memory` should be removed because functionality is included in the framework reference.
  - `System.Numerics.Vectors` should be removed because functionality is included in the framework reference.
  - `System.Threading.Tasks.Extensions` should be removed because functionality is included in the framework reference.

#### ERD.FileManagement\ERD.FileManagement.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0-windows7.0` to `net10.0-windows7.0`

NuGet packages changes:
  - No package changes suggested.

#### ERD.Build\ERD.Build.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0-windows` to `net10.0-windows`

NuGet packages changes:
  - No package changes suggested.

#### ERD.DataExport\ERD.DataExport.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0-windows` to `net10.0-windows`

NuGet packages changes:
  - No package changes suggested.

#### REPORT.Builder\REPORT.Builder.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0-windows` to `net10.0-windows`

NuGet packages changes:
  - No package changes suggested.

#### Report.Web.Presenters\Report.Web.Presenters.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0-windows7.0` to `net10.0-windows7.0`

NuGet packages changes:
  - No package changes suggested.

#### ERD.Viewer\ERD.Viewer.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0-windows` to `net10.0-windows`

NuGet packages changes:
  - `Microsoft.Bcl.AsyncInterfaces` should be updated from `8.0.0` to `10.0.10`.
  - `Microsoft.Extensions.Caching.Abstractions` should be updated from `8.0.0` to `10.0.10`.
  - `Microsoft.Extensions.Caching.Memory` should be updated from `8.0.1` to `10.0.10`.
  - `Microsoft.Extensions.Configuration` should be updated from `8.0.0` to `10.0.10`.
  - `Microsoft.Extensions.Configuration.Abstractions` should be updated from `8.0.0` to `10.0.10`.
  - `Microsoft.Extensions.Configuration.Binder` should be updated from `8.0.2` to `10.0.10`.
  - `Microsoft.Extensions.DependencyInjection` should be updated from `8.0.1` to `10.0.10`.
  - `Microsoft.Extensions.DependencyInjection.Abstractions` should be updated from `8.0.2` to `10.0.10`.
  - `Microsoft.Extensions.Logging` should be updated from `8.0.1` to `10.0.10`.
  - `Microsoft.Extensions.Logging.Abstractions` should be updated from `8.0.3` to `10.0.10`.
  - `Microsoft.Extensions.Options` should be updated from `8.0.2` to `10.0.10`.
  - `Microsoft.Extensions.Primitives` should be updated from `8.0.0` to `10.0.10`.
  - `System.Collections.Immutable` should be updated from `10.0.8` to `10.0.10`.
  - `System.Diagnostics.DiagnosticSource` should be updated from `9.0.11` to `10.0.10`.
  - `Microsoft.Identity.Client` should be updated from `3.0.8` to `4.87.0` because the current package version is deprecated.
  - `System.Buffers` should be removed because functionality is included in the framework reference.
  - `System.ComponentModel.Annotations` should be removed because functionality is included in the framework reference.
  - `System.Data.Common` should be removed because functionality is included in the framework reference.
  - `System.Memory` should be removed because functionality is included in the framework reference.
  - `System.Numerics.Vectors` should be removed because functionality is included in the framework reference.
  - `System.Runtime.Serialization.Formatters` should be removed because functionality is included in the framework reference.
  - `System.Threading.Tasks.Extensions` should be removed because functionality is included in the framework reference.
