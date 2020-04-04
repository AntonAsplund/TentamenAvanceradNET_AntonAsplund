namespace TentamenAvanceradNET_AntonAsplund.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _22_43 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Doctors", "SkillLevel", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Doctors", "SkillLevel", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
