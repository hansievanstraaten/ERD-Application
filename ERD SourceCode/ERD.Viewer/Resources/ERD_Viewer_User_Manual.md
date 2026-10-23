# ERD.Viewer User Manual

This manual explains the user interface of **ERD.Viewer (ViSo-nice)** in simple terms, based on the menu and controls in `MainWindow.xaml`, and the windows/user controls opened from those actions.

## Document Index

- [1) Main Window Overview A](#1-main-window-overview)
- [2) Top Menu - Detailed Guide (Menu Order)](#2-top-menu---detailed-guide-menu-order)
  	- [File](#file)
    - [Project](#project)
    - [Git](#git)
    - [Database](#database)
    - [Build](#build)
    - [Report Builder](#report-builder)
    - [Help And About](#help-and-about)
- [3) Workspace Editing Areas](#3-workspace-editing-areas)
    - [Left Panel - Tools Stack](#left-panel---tools-stack)
    - [Left Panel - Database Tables List](#left-panel---database-tables-list)
    - [Workspace Tabs (Canvas Area)](#workspace-tabs-canvas-area)
- [4) Table and Relation Editing](#4-table-and-relation-editing)
    - [Table Object UI (inside a canvas)](#table-object-ui-inside-a-canvas)
    - [Relation Editing Flow](#relation-editing-flow)
- [5) Status, Locking, and Quick Start](#5-status-locking-and-quick-start)
    - [Status and Locking Behavior (User Visible)](#status-and-locking-behavior-user-visible)
    - [Practical Quick Start](#practical-quick-start)

---

## 1) Main Window Overview

When ERD.Viewer opens, you work in one main window with these areas:

- **Top Menu Bar**: File, Project, Git, Database, Build, Report Builder, Help And About.
- **Top Menu Action Label**: **Install Updates** appears when a newer version is available.
- **Left Panel (Tools)**: Drag-and-drop tools such as Table and Relation items.
- **Left Panel (Database Tables)**: Searchable list of tables known to the project.
- **Center/Right Workspace Tabs**: One tab per canvas (diagram segment), where tables and relations are visually edited.
- **Bottom Status Area**:
  - Active Git branch
  - Active database connection
  - Message area for warnings/status

---

## 2) Top Menu - Detailed Guide (Menu Order)

## File

### New Project
Opens **Project Setup** window.

Use this to create a new ERD project and define:
- Project metadata
- Default database connection
- Optional alternative connections

After saving, the app enables the working menus and loads tools/tables/canvas workflow.

### Open Project
Opens a file picker for `*.eprj` files and loads an existing project.

### Clone Repository
Opens a modal dialog titled **Clone Repository**.

Use this to:
- Provide repository URL/user/token details
- Choose local folder
- Clone and open the project automatically

If clone updates project credentials, ERD.Viewer may ask you to commit/push those changes.

### Save Project
Saves project and canvas changes.

### Exit
Closes the application and releases canvas locks.

### File-related fields
#### Project Setup (`ProjectSetup`)
- **Project Name**: Name of the ERD project.
- **File Directory**: Folder where project/canvas files are saved.
- **Allow Database Relations**: Enables database relation design tools.
- **Allow Virtual Relations**: Enables virtual relation design tools.
- **Keep Columns Unique**: Reuses column conventions across tables.
- **Lock Canvas on Editing**: Locks canvas while editing to reduce conflicts.

#### Default/Alternative Database fields
- **Database Type**: SQL / Postgres, etc. Controls available connection fields.
- **Server Name/Host**: Database server host.
- **Port**: Mainly relevant for Postgres.
- **Database Name**: Target database/schema container.
- **User Name** / **Password**: Login credentials.
- **Trusted Connection**: Use integrated security when supported.
- **SSL Mode**: Postgres SSL toggle.
- **Name** (alternative connection): Friendly key used in menus for this connection.

---

## Project

### Edit Project
Re-opens **Project Setup** for the currently loaded project.

### Add Project Canvas
Opens **Tab Setup** (`ErdTabSetup`) to define a new canvas/segment.

After confirmation, a new workspace tab is created.

### Project-related fields
#### Add/Edit Canvas (`ErdTabSetup` / Edit Canvas)
- **Tab Name**: Canvas tab display name.
- **Table Prefix**: Optional prefix auto-applied to new table names in that canvas.

---

## Git

### Fetch
Fetches remote changes and reports incoming change count.

### Pull
Opens **Pull Changes** dialog (identity input), then pulls changes.

If conflicts are detected, conflict resolution flow is triggered.

### Commit
Opens **Commit and Push Changes** dialog for commit details, then stages and commits.

### Push
Pushes local commits to a remote. If multiple remotes exist, prompts for remote selection.

### Pending Changes
Shows a formatted list of modified files in a **Monospaced Message Box** dialog.

### New Branch
Prompts for branch name and creates/checks out the branch.

### Get Remote Branch
Opens **Select Branch** dialog (remote branches), then creates local branch from selected remote.

### Change Branch
Opens **Select Branch** dialog (local branches), checks out selected branch, then reloads project if needed.

### Resolve Conflicts
Opens **Git Conflict Resolver** window.

### View Branch History
Shows commit history in a **Monospaced Message Box** dialog.

### Manage Branches
Opens **Git Branch Manager** window.

### Open in Browser
Opens the selected remote repository URL in a web browser.

### Git Credentials
Opens **Git Credentials** dialog.

Use this to update the stored Git username and password/token for the active project.

### Git-related fields
#### Clone Repository
- **Git Repository URL**: Remote repository URL.
- **Repository Local Path**: Local folder for clone.
- **User Name**: Optional auth user.
- **Password or Personal Access Token**: Optional auth token/password.

#### Pull/Commit identity
- **User Name**: Current OS user (read-only in identity dialog).
- **Email**: Commit/pull identity email.

#### Commit
- **Comment**: Commit message.

#### Remote/Branch selection
- **Remote Name**: Select target remote for push/open.
- **Branch**: Select local or remote branch depending on dialog.

#### Git Credentials
- **User Name**: Stored Git username for this project.
- **Password or Personal Access Token**: Stored Git password/token for this project.

### Git tools and dialogs
- **GitConflictResolver**: compare incoming/local/merged text and accept merge output.
- **GitBranchManager**: browse branches, inspect history, inspect changed files/content, checkout branches.
- **MonospacedMessageBox**: fixed-width text view used for pending changes/history lists.

---

## Database

These actions are connection-specific (items are generated per available connection).

### Get DB Tables
Runs reverse engineering and loads table metadata from the selected database into the left table list.

### Refresh Canvases From DB
Refreshes table/column metadata from database for tables already used in canvases.

### Compare to DB
Compares ERD model vs. database and opens **Compare Results** window if differences exist.

### Forward Engineer
Builds/applies schema changes from ERD model to selected database.

### Script Changes
Generates scripts for schema changes (without direct execute mode).

### Database browsing/export
- **BrowseData**: execute SQL and inspect results (supports F5 execution in query mode).
- **ExportManager**: export/migrate table data with merge options.
- **CompreResults**: list ERD vs DB differences, script changes, and edit discrepancy entries.

### Database export fields (`ExportManager`)
- **Source**: Read source connection.
- **Destination**: Destination connection or file mode.
- **Output Directory**: Folder for exported files.
- **Data rows max limit**: Optional row limit (0 = no limit).
- **Data Delimiter**: Delimiter for file generation.
- **Merge Identity Values**: Include identity values in merge logic.
- **Place Data In SQL**: Execute/generated SQL placement behavior.
- **Update existing rows in target**: Enables `MERGE UPDATE` behavior.
- **Insert new rows in target**: Enables `MERGE INSERT` behavior.
- **Delete unmatched row from target**: Enables delete behavior.

---

## Build

### Script Builder Setup
Opens **Build Setup** window.

Use this to configure script generation options and build types.

### Run Script Builder
Runs configured build scripts against current canvases.

### Build tools and controls
- **BuildSetup**: main script builder setup (tabs, options, parameters).
- **BuildOption**: per option setup + live sample output pane.
- **BuildTypeControl**: repeat-type and code snippet editor per build type.

### Script Builder fields (`BuildSetup` / `BuildOption` / `BuildTypeControl`)
#### Option setup fields
- **File Name (e.g. `[[CanvasName]].cs`)**: Output filename template.
- **Output Directory**: Output folder template.
- **Repeat Option**:
  - Foreach Canvas in Project
  - Foreach Table in Project
  - Single File for Project
- **Override File If Exists**: Replace existing files when checked.
- **Language Option**: Type mapping style used in template substitution.

#### Build Type fields
- **Build Type Name**: Logical block name (Body, Inputs, Mapping, etc.).
- **Repeat**: Iteration scope for this block (once, per table, per column, etc.).
- **Code**: Template code injected into parent blocks.

### Script Builder Deep Dive
Script Builder is one of the strongest accelerators in ERD.Viewer.

#### How it works
1. You define one or more **Build Options**.
2. Each option has one or more **Build Types**.
3. Build Types are linked using placeholder slots like `[[1]]`, `[[2]]`, `[[3]]`.
4. The engine expands placeholders by iteration scope and ERD metadata.
5. Output files are written using template-based filename/path rules.

#### Key placeholder tokens (supported by engine)
- `[[CanvasName]]`
- `[[TableName]]`, `[[TableFriendlyName]]`, `[[TablePluralName]]`, `[[TableDescription]]`
- `[[ColumnName]]`, `[[ColumnFriendlyName]]`, `[[ColumnDescription]]`
- `[[PrimaryKey]]`, `[[ForeignKey]]`, `[[NoKeyColumn]]`
- `[[ForeignTableName]]`, `[[ForeignTableFriendlyName]]`, `[[ForeignTablePluralName]]`
- `[[DataType]]`, `[[DatabaseColumnType]]`, `[[DatabaseColumnLength]]`, `[[DatabaseNullableType]]`, `[[DatabaseColumnTypeIDENTITY]]`

#### Advanced template behavior
- **Nested block injection** via `[[index]]` (example: body block contains `[[1]]`, `[[2]]`).
- **Conditional insertion** via `[[IF{{A==B then Text}}]]`.
- **Optional trailing text markers** via `[[? ... ]]` to prevent trailing separators.

#### Recommended workflow for teams
1. Start with one “golden” `.estp` setup.
2. Create options per output type (API model, mapper, UI, SQL script).
3. Use strict naming conventions in `OutputFileName` and `OutputDirectory`.
4. Keep `Override File If Exists` off by default; enable only where safe.
5. Version control generated outputs and template files.

---

## Report Builder

### Setup
Opens **Report System Setup** control in a dialog.

Use this to configure report storage and data source options.

### Reports
Opens **Report Selector** control.

Use this to work with report categories and page sections.

### Reporting tools and controls
- **ReportSystemSetup**: report backend/config setup.
- **ReportSelector**: report pages/categories tabs.

### Report System Setup fields (`ReportSystemSetup`)
- **Database Type**: Storage backend (SQLite / MsSql / Postgres).
- **DB File Location**: Required for SQLite mode.
- **Database Setup fields** (if MsSql/Postgres): server/port/db/user/password/SSL/trusted as applicable.

### Report Designer main fields (`ReportDesigner` -> Report model)
- **Name**: Report name.
- **Description**: High-level report description.
- **Report Type**: Cover/Header-Footer/Final/Data (read-only context).
- **Report Version**: XML design version to save/edit.
- **Is Active Version**: Marks default active version.
- **Paper Kind**: Paper size.
- **Page Orientation**: Portrait/Landscape.
- **Production Connection**: Runtime database connection for data reports.
- **Page Left/Right/Top/Bottom Margin**: Print layout margins.

### Report Builder & Designer Deep Dive
Report Builder is a full report-definition environment with categories, versions, print setup, and visual section design.

#### Report Builder structure
- **Setup (Report System Setup)**: configure where report metadata is stored.
- **Reports (Report Selector)**:
  - **Data Report** tab
  - **Cover Pages** tab
  - **Headers and Footers** tab
  - **Final Pages** tab

#### Category management (Data Report)
- Add/Delete hierarchical categories.
- Select category to filter/manage report list.

#### Report listing and versioning
- Grid shows Report ID, Name, XML Version.
- Description preview panel.
- Edit existing reports.
- Save multiple versions and mark active version.

#### Report Designer capabilities
- Left panel: Save, Preview, Export PDF, Help actions.
- Metadata editor: name/type/version/margins.
- Toolbox items: Label, Border, Current Date, Image, Horizontal Line, Vertical Line, Page Break, Report Sum.
- Canvas/sections: multi-section layout with rulers and collapsible sections.
- Right properties panel: selected object properties, replace rules, where-link builder, SQL preview.
- Data report support: data source selector and table/column mapping tools.

---

## Help And About

### About
Opens **About** panel in a dialog.

Shows product information and a link to the GitHub repository.

### Reference Document
Opens this user manual from the embedded application resource.

### Help-related controls
- **About**: product info and GitHub link.
- **ReleaseNotes**: placeholder control exists in project; menu entry is currently disabled in the main menu.

---

## 3) Workspace Editing Areas

## Left Panel - Tools Stack
When a project is active, the tool stack normally contains:

- **Table tool** (`TableMenuItem` with a "new table" model)
- **Database Relation tool** (`RelationMenuItem`) if allowed by project settings
- **Virtual Relation tool** (`RelationMenuItem`) if allowed by project settings

### How tools work
- Drag **Table** into a canvas to create/add a table.
- Drag **Relation** into a canvas to start relation definition.

## Left Panel - Database Tables List
Area title: **Database Tables**

Features:
- Search box (**Search Tables**) filters table list by name.
- Each item is a **TableMenuItem**:
  - Left text: table name
  - Right text: canvas location/segment marker
  - Double-click: jump to that table in its canvas tab
  - Drag support (if table is not already bound to fixed location)

## Workspace Tabs (Canvas Area)
Each tab is a **TableCanvas** for one ERD segment.

Top toolbar in each canvas tab:
- **Reference Tables / Include in Context Build**
  - Opens **Selected Tables** window
  - Choose extra reference tables used in script build context
- **Print Canvas**
  - Opens print dialog and prints visual canvas
- **Edit Canvas Metadata**
  - Opens modal dialog **Edit Canvas**
  - Used for metadata such as table prefix
- **Metadata Labels**
  - Current prefix information
  - Lock status message

Main drawing surface:
- Pan/zoom enabled visual canvas (`TableWorkingCanvas` in pannable scroll viewer)
- Drag tables/relations onto canvas
- Move table objects visually

---

## 4) Table and Relation Editing

## Table Object UI (inside a canvas)
Each table shown on canvas is a **TableObject** control.

Visible content:
- Table title
- Columns grid (PK, FK, Column Name, Data Type, Allow Null)
- Add button for column creation

### Table header context menu
Right-click table title to access:

- **Edit Table Header**
  - Opens modal dialog **Edit Table Properties**
- **View Data**
  - Opens **Browse Data** window for selected connection
  - If multiple connections exist, choose a connection from a submenu
- **Export Data**
  - Opens **Export Manager** window
  - If multiple connections exist, choose a connection from a submenu
- **View Database Script**
  - Generates table create/foreign key script file and opens it
- **View Generated Script** (when build setup exists)
  - Script all / selected build option script preview file generation
- **Undo Changes**
  - Restores tracked modified values
- **Remove from Canvas**
  - Removes table visual from the current canvas
- **Drop Table**
  - Marks table for deletion in next forward engineering cycle

### Column grid context menu
Right-click columns area to access:

- **Column Usage**
  - Opens **Browse Data** with usage query
- **Add Column**
  - Opens **Column Edit** window
- **Edit Column**
  - Opens **Column Edit** window for selected column
- **Delete Column**
  - Marks selected column as deleted for next build/apply cycle

### Table and column edit fields
#### Table Edit (`TableEdit` / Edit Table Properties)
- **Name**: Physical table name.
- **Schema**: Database schema (default often `dbo`).
- **Friendly Name**: Display/business name.
- **Plural Name**: Useful for generated code/UI labels.
- **Description**: Free text documentation.

#### Column Edit (`ColumnsEdit`)
- **Column Name**: Physical column name (can use system suggestions).
- **Friendly Name**: Human-readable label.
- **Data Type**: SQL type and length/precision format.
- **In Primary Key**: Marks column as part of PK.
- **Allow Nulls**: Nullable toggle.
- **Description**: Column documentation.

Notes:
- PK columns may lock some fields (name/key/null behavior).
- Data type can include forms like `varchar(255)`, `decimal(18,6)`, `int IDENTITY`.

## Relation Editing Flow
Dragging a relation tool to a canvas opens **Relation Editor** window.

In Relation Editor:
- Select parent table and child table
- Select participating columns
- Confirm to create/update relation mapping

The relation is then drawn on the canvas.

---

## 5) Status, Locking, and Quick Start

## Status and Locking Behavior (User Visible)
- Canvas edits can trigger lock indicators to avoid conflicting updates.
- Bottom status displays branch/connection and runtime messages.
- Conflict workflows can require manual merge acceptance before continuing.

## Practical Quick Start
1. **File -> New Project** (or Open Project).
2. Add a canvas via **Project -> Add Project Canvas**.
3. Load database tables via **Database -> Get DB Tables**.
4. Drag tables from left list into canvas tabs.
5. Edit tables/columns/relations from canvas context menus.
6. Save via **File -> Save Project**.
7. Compare/apply DB changes with **Database** menu.
8. Configure/generate scripts with **Build** menu.
9. Use **Git** menu for collaboration workflows.
