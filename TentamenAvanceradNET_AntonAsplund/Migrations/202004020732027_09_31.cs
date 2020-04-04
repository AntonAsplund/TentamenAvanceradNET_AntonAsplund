namespace TentamenAvanceradNET_AntonAsplund.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _09_31 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AfterLives",
                c => new
                    {
                        AfterLifeID = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.AfterLifeID);
            
            CreateTable(
                "dbo.Patients",
                c => new
                    {
                        PatientID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        SSN = c.Double(nullable: false),
                        Age = c.Int(nullable: false),
                        ConditionLevel = c.Int(nullable: false),
                        ArrivalAtHospital = c.DateTime(nullable: false),
                        AssingedToHopsitalBed = c.DateTime(),
                        SignedOut = c.DateTime(),
                        AfterLifeID = c.Int(),
                        DischargedID = c.Int(),
                        IntensiveCareUnitID = c.Int(nullable: false),
                        PatientQueueID = c.Int(nullable: false),
                        SanatoriumID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PatientID)
                .ForeignKey("dbo.AfterLives", t => t.AfterLifeID)
                .ForeignKey("dbo.Dischargeds", t => t.DischargedID)
                .ForeignKey("dbo.IntensiveCareUnits", t => t.IntensiveCareUnitID, cascadeDelete: true)
                .ForeignKey("dbo.PatientQueues", t => t.PatientQueueID, cascadeDelete: true)
                .ForeignKey("dbo.Sanatoriums", t => t.SanatoriumID, cascadeDelete: true)
                .Index(t => t.AfterLifeID)
                .Index(t => t.DischargedID)
                .Index(t => t.IntensiveCareUnitID)
                .Index(t => t.PatientQueueID)
                .Index(t => t.SanatoriumID);
            
            CreateTable(
                "dbo.Dischargeds",
                c => new
                    {
                        DischargedID = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.DischargedID);
            
            CreateTable(
                "dbo.IntensiveCareUnits",
                c => new
                    {
                        IntensiveCareUnitID = c.Int(nullable: false, identity: true),
                        AvailableBed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.IntensiveCareUnitID);
            
            CreateTable(
                "dbo.Doctors",
                c => new
                    {
                        DoctorID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        SkillLevel = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NumberOfRotationsLeft = c.Int(nullable: false),
                        IntensiveCareUnit_IntensiveCareUnitID = c.Int(),
                        Sanatorium_SanatoriumID = c.Int(),
                    })
                .PrimaryKey(t => t.DoctorID)
                .ForeignKey("dbo.IntensiveCareUnits", t => t.IntensiveCareUnit_IntensiveCareUnitID)
                .ForeignKey("dbo.Sanatoriums", t => t.Sanatorium_SanatoriumID)
                .Index(t => t.IntensiveCareUnit_IntensiveCareUnitID)
                .Index(t => t.Sanatorium_SanatoriumID);
            
            CreateTable(
                "dbo.PatientQueues",
                c => new
                    {
                        PatientQueueID = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.PatientQueueID);
            
            CreateTable(
                "dbo.Sanatoriums",
                c => new
                    {
                        SanatoriumID = c.Int(nullable: false, identity: true),
                        AvailableBed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.SanatoriumID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Patients", "SanatoriumID", "dbo.Sanatoriums");
            DropForeignKey("dbo.Doctors", "Sanatorium_SanatoriumID", "dbo.Sanatoriums");
            DropForeignKey("dbo.Patients", "PatientQueueID", "dbo.PatientQueues");
            DropForeignKey("dbo.Patients", "IntensiveCareUnitID", "dbo.IntensiveCareUnits");
            DropForeignKey("dbo.Doctors", "IntensiveCareUnit_IntensiveCareUnitID", "dbo.IntensiveCareUnits");
            DropForeignKey("dbo.Patients", "DischargedID", "dbo.Dischargeds");
            DropForeignKey("dbo.Patients", "AfterLifeID", "dbo.AfterLives");
            DropIndex("dbo.Doctors", new[] { "Sanatorium_SanatoriumID" });
            DropIndex("dbo.Doctors", new[] { "IntensiveCareUnit_IntensiveCareUnitID" });
            DropIndex("dbo.Patients", new[] { "SanatoriumID" });
            DropIndex("dbo.Patients", new[] { "PatientQueueID" });
            DropIndex("dbo.Patients", new[] { "IntensiveCareUnitID" });
            DropIndex("dbo.Patients", new[] { "DischargedID" });
            DropIndex("dbo.Patients", new[] { "AfterLifeID" });
            DropTable("dbo.Sanatoriums");
            DropTable("dbo.PatientQueues");
            DropTable("dbo.Doctors");
            DropTable("dbo.IntensiveCareUnits");
            DropTable("dbo.Dischargeds");
            DropTable("dbo.Patients");
            DropTable("dbo.AfterLives");
        }
    }
}
