namespace TentamenAvanceradNET_AntonAsplund.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _19_31 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Doctors", "IntensiveCareUnit_IntensiveCareUnitID", "dbo.IntensiveCareUnits");
            DropForeignKey("dbo.Doctors", "Sanatorium_SanatoriumID", "dbo.Sanatoriums");
            DropIndex("dbo.Doctors", new[] { "IntensiveCareUnit_IntensiveCareUnitID" });
            DropIndex("dbo.Doctors", new[] { "Sanatorium_SanatoriumID" });
            AddColumn("dbo.IntensiveCareUnits", "DoctorID", c => c.Int());
            AddColumn("dbo.Doctors", "assignedToICU", c => c.Boolean(nullable: false));
            AddColumn("dbo.Doctors", "assignedToSantorium", c => c.Boolean(nullable: false));
            AddColumn("dbo.Sanatoriums", "DoctorID", c => c.Int());
            CreateIndex("dbo.IntensiveCareUnits", "DoctorID");
            CreateIndex("dbo.Sanatoriums", "DoctorID");
            AddForeignKey("dbo.IntensiveCareUnits", "DoctorID", "dbo.Doctors", "DoctorID");
            AddForeignKey("dbo.Sanatoriums", "DoctorID", "dbo.Doctors", "DoctorID");
            DropColumn("dbo.Doctors", "IntensiveCareUnit_IntensiveCareUnitID");
            DropColumn("dbo.Doctors", "Sanatorium_SanatoriumID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Doctors", "Sanatorium_SanatoriumID", c => c.Int());
            AddColumn("dbo.Doctors", "IntensiveCareUnit_IntensiveCareUnitID", c => c.Int());
            DropForeignKey("dbo.Sanatoriums", "DoctorID", "dbo.Doctors");
            DropForeignKey("dbo.IntensiveCareUnits", "DoctorID", "dbo.Doctors");
            DropIndex("dbo.Sanatoriums", new[] { "DoctorID" });
            DropIndex("dbo.IntensiveCareUnits", new[] { "DoctorID" });
            DropColumn("dbo.Sanatoriums", "DoctorID");
            DropColumn("dbo.Doctors", "assignedToSantorium");
            DropColumn("dbo.Doctors", "assignedToICU");
            DropColumn("dbo.IntensiveCareUnits", "DoctorID");
            CreateIndex("dbo.Doctors", "Sanatorium_SanatoriumID");
            CreateIndex("dbo.Doctors", "IntensiveCareUnit_IntensiveCareUnitID");
            AddForeignKey("dbo.Doctors", "Sanatorium_SanatoriumID", "dbo.Sanatoriums", "SanatoriumID");
            AddForeignKey("dbo.Doctors", "IntensiveCareUnit_IntensiveCareUnitID", "dbo.IntensiveCareUnits", "IntensiveCareUnitID");
        }
    }
}
