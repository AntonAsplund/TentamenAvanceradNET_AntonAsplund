namespace TentamenAvanceradNET_AntonAsplund.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _22_02 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PatientHistories",
                c => new
                    {
                        PatientHistoryID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        SSN = c.Double(nullable: false),
                        Age = c.Int(nullable: false),
                        Status = c.String(),
                        SimulationNumber = c.Int(nullable: false),
                        ArrivalAtHospital = c.DateTime(nullable: false),
                        AssingedToHopsitalBed = c.DateTime(),
                        SignedOut = c.DateTime(),
                    })
                .PrimaryKey(t => t.PatientHistoryID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PatientHistories");
        }
    }
}
