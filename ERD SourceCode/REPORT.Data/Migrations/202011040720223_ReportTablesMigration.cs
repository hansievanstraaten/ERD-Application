namespace REPORT.Data.Migrations.ReportTablesConfig
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReportTablesMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ReportCategory",
                c => new
                    {
                        CategoryId = c.Long(nullable: false, identity: true),
                        CategoryName = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        ParentCategoryId = c.Long(),
                    })
                .PrimaryKey(t => t.CategoryId);
            
            CreateTable(
                "dbo.ReportConnection",
                c => new
                    {
                        MasterReport_Id = c.Long(nullable: false),
                        ReportConnectionName = c.String(nullable: false, maxLength: 128),
                        DatabaseTypeEnum = c.Int(nullable: false),
                        ServerName = c.String(),
                        DatabaseName = c.String(),
                        UserName = c.String(),
                        Password = c.String(),
                        TrustedConnection = c.Boolean(nullable: false),
                        IsProductionConnection = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.MasterReport_Id, t.ReportConnectionName });
            
            CreateTable(
                "dbo.ReportMaster",
                c => new
                    {
                        MasterReport_Id = c.Long(nullable: false, identity: true),
                        CategoryId = c.Long(),
                        ReportName = c.String(),
                        Description = c.Binary(),
                        ReportTypeEnum = c.Int(nullable: false),
                        PaperKindEnum = c.Int(nullable: false),
                        PageOrientationEnum = c.Int(nullable: false),
                        CoverPage_Id = c.Long(),
                        HeaderAndFooterPage_Id = c.Long(),
                        FinalPage_Id = c.Long(),
                        PageMarginLeft = c.Int(nullable: false),
                        PageMarginRight = c.Int(nullable: false),
                        PageMarginTop = c.Int(nullable: false),
                        PageMarginBottom = c.Int(nullable: false),
                        ProjectName = c.String(),
                    })
                .PrimaryKey(t => t.MasterReport_Id);
            
            CreateTable(
                "dbo.ReportXML",
                c => new
                    {
                        ReportXMLVersion = c.Int(nullable: false),
                        MasterReport_Id = c.Long(nullable: false),
                        BinaryXML = c.Binary(),
                        PrintCount = c.Long(nullable: false),
                        IsActiveVersion = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.ReportXMLVersion, t.MasterReport_Id });
            
            CreateTable(
                "dbo.ReportXMLPrintParameter",
                c => new
                    {
                        TableName = c.String(nullable: false, maxLength: 128),
                        ColumnName = c.String(nullable: false, maxLength: 128),
                        ReportXMLVersion = c.Int(nullable: false),
                        MasterReport_Id = c.Long(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        FilterCaption = c.String(),
                        DefaultValue = c.String(),
                        IsRequired = c.Boolean(),
                    })
                .PrimaryKey(t => new { t.TableName, t.ColumnName, t.ReportXMLVersion, t.MasterReport_Id });
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ReportXMLPrintParameter");
            DropTable("dbo.ReportXML");
            DropTable("dbo.ReportMaster");
            DropTable("dbo.ReportConnection");
            DropTable("dbo.ReportCategory");
        }
    }
}
