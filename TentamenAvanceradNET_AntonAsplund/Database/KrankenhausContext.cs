using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TentamenAvanceradNET_AntonAsplund.Database
{
    class KrankenhausContext : DbContext
    {
        public KrankenhausContext()
        {
            this.Database.Connection.ConnectionString = "Data Source=DESKTOP-6MGRJ4I\\SQLEXPRESS02; Initial Catalog = TentamenAvanceradNET_AntonAsplund; Integrated Security = True";
        }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Sanatorium> Sanatorium { get; set; }
        public DbSet<IntensiveCareUnit> IntesiveCareUnit { get; set; }
        public DbSet<Discharged> Dischargeds { get; set; }
        public DbSet<PatientQueue> PatientQueue { get; set; }
        public DbSet<AfterLife> AfterLives { get; set; }
        public DbSet<PatientHistory> PatientHistories { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>()
                .Property(P => P.PatientQueueID)
                .IsOptional();

            modelBuilder.Entity<Patient>()
                .Property(P => P.IntensiveCareUnitID)
                .IsOptional();

            modelBuilder.Entity<Patient>()
                .Property(P => P.SanatoriumID)
                .IsOptional();

            modelBuilder.Entity<IntensiveCareUnit>()
                .Property(ICU => ICU.DoctorID)
                .IsOptional();

            modelBuilder.Entity<Sanatorium>()
                .Property(S => S.DoctorID)
                .IsOptional();



        }

    }
}
