namespace TentamenAvanceradNET_AntonAsplund.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _09_39 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Patients", "IntensiveCareUnitID", "dbo.IntensiveCareUnits");
            DropForeignKey("dbo.Patients", "PatientQueueID", "dbo.PatientQueues");
            DropForeignKey("dbo.Patients", "SanatoriumID", "dbo.Sanatoriums");
            DropIndex("dbo.Patients", new[] { "IntensiveCareUnitID" });
            DropIndex("dbo.Patients", new[] { "PatientQueueID" });
            DropIndex("dbo.Patients", new[] { "SanatoriumID" });
            AlterColumn("dbo.Patients", "IntensiveCareUnitID", c => c.Int());
            AlterColumn("dbo.Patients", "PatientQueueID", c => c.Int());
            AlterColumn("dbo.Patients", "SanatoriumID", c => c.Int());
            CreateIndex("dbo.Patients", "IntensiveCareUnitID");
            CreateIndex("dbo.Patients", "PatientQueueID");
            CreateIndex("dbo.Patients", "SanatoriumID");
            AddForeignKey("dbo.Patients", "IntensiveCareUnitID", "dbo.IntensiveCareUnits", "IntensiveCareUnitID");
            AddForeignKey("dbo.Patients", "PatientQueueID", "dbo.PatientQueues", "PatientQueueID");
            AddForeignKey("dbo.Patients", "SanatoriumID", "dbo.Sanatoriums", "SanatoriumID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Patients", "SanatoriumID", "dbo.Sanatoriums");
            DropForeignKey("dbo.Patients", "PatientQueueID", "dbo.PatientQueues");
            DropForeignKey("dbo.Patients", "IntensiveCareUnitID", "dbo.IntensiveCareUnits");
            DropIndex("dbo.Patients", new[] { "SanatoriumID" });
            DropIndex("dbo.Patients", new[] { "PatientQueueID" });
            DropIndex("dbo.Patients", new[] { "IntensiveCareUnitID" });
            AlterColumn("dbo.Patients", "SanatoriumID", c => c.Int(nullable: false));
            AlterColumn("dbo.Patients", "PatientQueueID", c => c.Int(nullable: false));
            AlterColumn("dbo.Patients", "IntensiveCareUnitID", c => c.Int(nullable: false));
            CreateIndex("dbo.Patients", "SanatoriumID");
            CreateIndex("dbo.Patients", "PatientQueueID");
            CreateIndex("dbo.Patients", "IntensiveCareUnitID");
            AddForeignKey("dbo.Patients", "SanatoriumID", "dbo.Sanatoriums", "SanatoriumID", cascadeDelete: true);
            AddForeignKey("dbo.Patients", "PatientQueueID", "dbo.PatientQueues", "PatientQueueID", cascadeDelete: true);
            AddForeignKey("dbo.Patients", "IntensiveCareUnitID", "dbo.IntensiveCareUnits", "IntensiveCareUnitID", cascadeDelete: true);
        }
    }
}
