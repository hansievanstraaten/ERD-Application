using ERD.Base;
using ERD.Common;
using ERD.Models;
using ERD.Models.ReportModels;
using REPORT.Data;
using REPORT.Data.SQLRepository.Repositories;
using ViSo.SharedEnums.ReportEnums;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//General.ProjectModel.ModelName
builder.Services.AddSingleton<Connections>(serviceProvider =>
{
    IConfiguration config = serviceProvider.GetRequiredService<IConfiguration>();

    // **********************************************************************************
    // * I stored my DatabaseName, UserName and Password as IDE Secrets,                *
    // * so I can keep them out of source control.                                      *
    // * If you want to use this method, you can set them in your IDE's secret manager. *
    // **********************************************************************************

    DatabaseModel databaseModel = new DatabaseModel
    {
        ServerName = config["Database:ServerName"] ?? "localhost",
        PortName = config["Database:PortName"] ?? "3306",
        DatabaseName = config["Database:DatabaseName"] ?? "reporting",
        DatabaseType = Enum.TryParse(config["Database:DatabaseType"], out DatabaseTypeEnum dbType) ? dbType : DatabaseTypeEnum.SQL,
        UserName = config["Database:UserName"] ?? "root",
        Password = config["Database:Password"] ?? "password",
        TrustedConnection = bool.TryParse(config["Database:TrustedConnection"], out bool trusted) ? trusted : false,
        SSLMode = bool.TryParse(config["Database:SSLMode"], out bool ssl) ? ssl : false
    };

    Connections.Instance.SetDefaultDatabaseModel(databaseModel, false);

    return Connections.Instance;
});
builder.Services.AddSingleton<DbConfiguration>(serviceProvider =>
{
    IConfiguration config = serviceProvider.GetRequiredService<IConfiguration>();

    // **********************************************************************************
    // * I stored my DatabaseName, UserName and Password as IDE Secrets,                *
    // * so I can keep them out of source control.                                      *
    // * If you want to use this method, you can set them in your IDE's secret manager. *
    // **********************************************************************************

    DatabaseModel databaseModel = new DatabaseModel
    {
        ServerName = config["Reports:ServerName"] ?? "localhost",
        PortName = config["Reports:PortName"] ?? "3306",
        DatabaseName = config["Reports:DatabaseName"] ?? "reporting",
        DatabaseType = Enum.TryParse(config["Reports:DatabaseType"], out DatabaseTypeEnum dbType) ? dbType : DatabaseTypeEnum.SQL,
        UserName = config["Reports:UserName"] ?? "root",
        Password = config["Reports:Password"] ?? "password",
        TrustedConnection = bool.TryParse(config["Reports:TrustedConnection"], out bool trusted) ? trusted : false,
        SSLMode = bool.TryParse(config["Reports:SSLMode"], out bool ssl) ? ssl : false
    };

    DbConfiguration.Instance.ReportSetup = new ReportSetupModel
    {
        DataBaseSource = databaseModel,
        FileDirectory = config["Reports:SQLiteDirectory"] ?? "C:\\Reports",
        StorageType = Enum.TryParse(config["Reports:StorageType"], out StorageTypeEnum storageType) ? storageType : StorageTypeEnum.SQLite
    };

    return DbConfiguration.Instance;
});
builder.Services.AddSingleton<DatabaseConnection>(serviceProvider =>
{
    DatabaseConnection.Instance.InitializeConnectionString(DbConfiguration.Instance.ReportSetup);

    return DatabaseConnection.Instance;
});

builder.Services.AddTransient<ReportTablesRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => Results.Redirect("/swagger"));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

_ = app.Services.GetRequiredService<Connections>();
_ = app.Services.GetRequiredService<DbConfiguration>();
_ = app.Services.GetRequiredService<DatabaseConnection>();

app.Run();
