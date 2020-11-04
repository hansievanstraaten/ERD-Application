namespace REPORT.Data.Migrations.DataSourceConfig
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataSourceMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DataSourceMaster",
                c => new
                    {
                        MasterReport_Id = c.Long(nullable: false),
                        MainTableName = c.String(),
                    })
                .PrimaryKey(t => t.MasterReport_Id);
            
            CreateTable(
                "dbo.DataSourceTable",
                c => new
                    {
                        MasterReport_Id = c.Long(nullable: false),
                        TableName = c.String(nullable: false, maxLength: 128),
                        IsAvailable = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.MasterReport_Id, t.TableName });
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DataSourceTable");
            DropTable("dbo.DataSourceMaster");
        }
    }
}
