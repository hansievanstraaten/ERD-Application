namespace REPORT.Data.Migrations.SystemTablesConfig
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SystemTablesMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Lookup",
                c => new
                    {
                        LookupGroup = c.String(nullable: false, maxLength: 128),
                        GroupKey = c.Int(nullable: false),
                        GroupDescription = c.String(),
                    })
                .PrimaryKey(t => new { t.LookupGroup, t.GroupKey });
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Lookup");
        }
    }
}
