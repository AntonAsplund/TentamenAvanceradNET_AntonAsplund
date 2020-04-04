using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using TentamenAvanceradNET_AntonAsplund.Database;

namespace TentamenAvanceradNET_AntonAsplund
{
    /// <summary>
    /// Holds the event instances needed for simluation, and all individual thread methods.
    /// </summary>
    class KrankenhausMain
    {
        //Event which fires at the end of a loop if any patients are moved onto a new bed
        public event EventHandler<KrankenhausMovedPatientsEventArgs> PatientsMoved;
        //Event which fires at the end of a loop if any patients are discharged or declared deceased
        public event EventHandler<KrankenhausEndOfSimlationEventArgs> SimulationOver;

        //Object instance which is used to lock access to the database. To avoid optimistic concurrency exception
        internal readonly object krankenhausLock = new object();

        //Random generator to help with randomization of names and SSN
        private readonly Random rnd = new Random();

        public KrankenhausMain()
        {
            //Generates 10 beds in the sanatorium and 5 beds in the ICU if empty
            //Also generates a record in AfterLives and Dischargeds to hold references to any patients which belong to that category
            using (var db = new KrankenhausContext())
            {
                if (db.Sanatorium.FirstOrDefault() == null)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        var ICUSpot = new IntensiveCareUnit() { AvailableBed = true };
                        db.IntesiveCareUnit.Add(ICUSpot);
                    }


                    for (int i = 0; i < 10; i++)
                    {
                        var sanatoriumBed = new Sanatorium() { AvailableBed = true };
                        db.Sanatorium.Add(sanatoriumBed);
                    }


                    db.AfterLives.Add(new AfterLife() { });
                    db.Dischargeds.Add(new Discharged() { });

                    db.SaveChanges();
                }
            }

        }

        

        /// <summary>
        /// Creates 30 patients and adds them to the PatienQueue
        /// </summary>
        public void ThreadOne()
        {
            lock (krankenhausLock)
            {
                using (var db = new KrankenhausContext())
                {

                    for (int i = 0; i < 30; i++)
                    {
                        var newQueueSlip = new PatientQueue();
                        db.PatientQueue.Add(newQueueSlip);
                        db.SaveChanges();

                        var newPatient = KrankenhausHelpMethods.GetANewRandomPatient(rnd);
                        newPatient.PatientQueue = newQueueSlip;
                        db.Patients.Add(newPatient);
                        db.SaveChanges();
                    }

                    db.SaveChanges();
                }
            }

        }

        /// <summary>
        /// Moves patients from the PatientQueue to the either the IntesiveCareUnit or Sanatorium. Prioritizes by high severity first then high age.
        /// </summary>
        public void ThreadTwo()
        {
            lock (krankenhausLock)
            {
                using (var db = new KrankenhausContext())
                {
                    //Records the number of patient moves for KrankenhausMovedPatientsEventArgs
                    int numberOfPatientsAddedToICU = 0;
                    int numberOfPatientsAddedToICUFromQueue = 0;
                    int numberOfPatientsAddedToICUFromSanatorium = 0;
                    int numberOfPatietnsAddedToSanatoriumFromQueue = 0;

                    var freeICUBeds = db.IntesiveCareUnit.Where(ICU => ICU.AvailableBed == true).ToList();

                    var patientsToRelocateFromQueueQuery = db.Patients.Where(P => P.PatientQueueID != null && P.ConditionLevel != 10 && P.ConditionLevel != 0);
                    var patientsToRelocateFromSanatoriumQuery = db.Patients.Where(P => P.SanatoriumID != null && P.ConditionLevel != 10 && P.ConditionLevel != 0);
                    var patientsToRelocate = patientsToRelocateFromSanatoriumQuery.Concat(patientsToRelocateFromQueueQuery).ToList();

                    patientsToRelocate = patientsToRelocate.OrderByDescending(P => P.ConditionLevel).ThenByDescending(P => P.Age).ToList();

                    if (freeICUBeds.Count != 0 && patientsToRelocate.Count != 0)
                    {
                        //Makes sure that out of range index exception doesn't appear when patients are fewer than number of free beds 
                        int maxNumberOfPatientsAssignableToICU = freeICUBeds.Count <= patientsToRelocate.Count ? freeICUBeds.Count : patientsToRelocate.Count;

                        for (int i = 0; i < maxNumberOfPatientsAssignableToICU; i++)
                        {
                            int? intToQueryAgainstPatientQueue = patientsToRelocate[i].PatientQueueID;

                            int? intToQueryAgainstSanatorium = patientsToRelocate[i].SanatoriumID;

                            var patientInQueue = db.PatientQueue.Where(PQ => PQ.PatientQueueID == intToQueryAgainstPatientQueue).FirstOrDefault<PatientQueue>();
                            var patientsSanatoriumBed = db.Sanatorium.FirstOrDefault<Sanatorium>(S => S.SanatoriumID == intToQueryAgainstSanatorium);

                            if (patientInQueue != null)
                            {
                                db.PatientQueue.Remove(patientInQueue);
                                patientsToRelocate[i].AssingedToHopsitalBed = DateTime.UtcNow;
                                db.SaveChanges();

                                freeICUBeds[i].AvailableBed = false;
                                patientsToRelocate[i].IntensiveCareUnitID = freeICUBeds[i].IntensiveCareUnitID;
                                patientsToRelocate[i].PatientQueueID = null;
                                numberOfPatientsAddedToICUFromQueue++;
                                numberOfPatientsAddedToICU++;
                                db.SaveChanges();
                            }
                            else if (patientsSanatoriumBed != null)
                            {
                                patientsSanatoriumBed.Patient = null;
                                patientsSanatoriumBed.AvailableBed = true;

                                freeICUBeds[i].AvailableBed = false;

                                patientsToRelocate[i].IntensiveCareUnitID = freeICUBeds[i].IntensiveCareUnitID;
                                patientsToRelocate[i].SanatoriumID = null;
                                numberOfPatientsAddedToICUFromSanatorium++;
                                numberOfPatientsAddedToICU++;
                                db.SaveChanges();
                            }

                            db.SaveChanges();

                        }

                        if (numberOfPatientsAddedToICU > 0)
                        {
                            patientsToRelocate.RemoveRange(0, numberOfPatientsAddedToICU);
                        }

                    }

                    var freeSanatoriumBeds = (from S in db.Sanatorium
                                              where S.AvailableBed == true
                                              select S).ToList();

                    //Removes all patiens who are already assinged to a bed i.e. a Sanatorium bed.
                    patientsToRelocate.RemoveAll(P => P.AssingedToHopsitalBed != null);

                    if (freeSanatoriumBeds.Count != 0 && patientsToRelocate.Count != 0)
                    {

                        //Makes sure that out of range index exception doesn't appear. 
                        int maxNumberOfPatientsAssignableToSanatorium = freeSanatoriumBeds.Count <= patientsToRelocate.Count ? freeSanatoriumBeds.Count : patientsToRelocate.Count;

                        for (int i = 0; i < maxNumberOfPatientsAssignableToSanatorium; i++)
                        {
                            var intToQueryAgaint = patientsToRelocate[i].PatientQueueID;
                            var patientInQueue = db.PatientQueue.FirstOrDefault<PatientQueue>(PQ => PQ.PatientQueueID == intToQueryAgaint);

                            if (patientInQueue != null)
                            {
                                db.PatientQueue.Remove(patientInQueue);
                                db.SaveChanges();

                                freeSanatoriumBeds[i].AvailableBed = false;
                                patientsToRelocate[i].AssingedToHopsitalBed = DateTime.UtcNow;
                                patientsToRelocate[i].Sanatorium = freeSanatoriumBeds[i];
                                patientsToRelocate[i].PatientQueue = null;
                                numberOfPatietnsAddedToSanatoriumFromQueue++;
                            }



                            db.SaveChanges();
                        }

                    }

                    db.SaveChanges();

                    if (numberOfPatientsAddedToICUFromQueue + numberOfPatientsAddedToICUFromSanatorium + numberOfPatietnsAddedToSanatoriumFromQueue > 0)
                    {
                        PatientsMoved?.Invoke(this, new KrankenhausMovedPatientsEventArgs() { NumberOfPatientsFromQueueToSanatorium = numberOfPatietnsAddedToSanatoriumFromQueue, NumberOfPatientsFromQueueToICU = numberOfPatientsAddedToICUFromQueue, NumberOfPatientsFromSanatoriumToICU = numberOfPatientsAddedToICUFromSanatorium });
                    }
                }
            }
        }
        /// <summary>
        /// Updates the conditionLevel of the patients and adds a rotation point to the doctor assigned to the ICU and Sanatorium
        /// </summary>
        public void ThreadThree()
        {
            lock (krankenhausLock)
            {
                using (var db = new KrankenhausContext())
                {

                    var patientsToUpdate = db.Patients.Where(P => P.SignedOut == null && P.ConditionLevel > 0 && P.ConditionLevel < 10).ToList();
                    var doctorInICU = db.Doctors.Where(D => D.assignedToICU == true).FirstOrDefault();
                    var doctorInSanatorium = db.Doctors.Where(D => D.assignedToSantorium == true).FirstOrDefault();

                    int doctorInICUSkill = 0;
                    int doctorInSanatoriumSkill = 0;

                    if (doctorInICU != null)
                    {
                        doctorInICUSkill = doctorInICU.SkillLevel;
                        doctorInICUSkill = doctorInICU.NumberOfRotationsLeft > 0 ? doctorInICU.SkillLevel : 0;
                        doctorInICU.NumberOfRotationsLeft += -1;
                        db.SaveChanges();
                    }
                    if (doctorInSanatorium != null)
                    {
                        doctorInSanatoriumSkill = doctorInSanatorium.SkillLevel;
                        doctorInSanatoriumSkill = doctorInSanatorium.NumberOfRotationsLeft > 0 ? doctorInSanatorium.SkillLevel : 0;
                        doctorInSanatorium.NumberOfRotationsLeft += -1;
                        db.SaveChanges();
                    }


                    for (int i = 0; i < patientsToUpdate.Count; i++)
                    {
                        if (patientsToUpdate[i].PatientQueueID != null)
                        {
                            patientsToUpdate[i].ConditionLevel = KrankenhausHelpMethods.ModifyCoditionLevelQueue(rnd, patientsToUpdate[i].ConditionLevel);
                        }
                        else if (patientsToUpdate[i].SanatoriumID != null)
                        {
                            patientsToUpdate[i].ConditionLevel = KrankenhausHelpMethods.ModifyCoditionLevelSanatorium(rnd, patientsToUpdate[i].ConditionLevel, doctorInSanatoriumSkill);
                        }
                        else if (patientsToUpdate[i].IntensiveCareUnitID != null)
                        {
                            patientsToUpdate[i].ConditionLevel = KrankenhausHelpMethods.ModifyCoditionLevelICU(rnd, patientsToUpdate[i].ConditionLevel, doctorInICUSkill);
                        }

                        db.SaveChanges();
                    }

                }
            }
        }
        /// <summary>
        /// Removes patients from ICU and sanatorium to AfterLife or Discharged depending on their status
        /// </summary>
        public void ThreadFour()
        {
            lock (krankenhausLock)
            {
                using (var db = new KrankenhausContext())
                {
                    int numberOfPatientsToAfterLife = 0;
                    int numberOfPatientsDischarged = 0;

                    var patientsToRelocate = db.Patients.Where(P => P.ConditionLevel == 0 && P.DischargedID == null || P.ConditionLevel == 10 && P.AfterLifeID == null).ToList();
                    var discharged = db.Dischargeds.FirstOrDefault();
                    var afterLifte = db.AfterLives.FirstOrDefault();

                    for (int i = 0; i < patientsToRelocate.Count; i++)
                    {

                        if (patientsToRelocate[i].PatientQueueID != null)
                        {
                            int? patientQueueIDToQueryAgainst = patientsToRelocate[i].PatientQueueID;
                            var patientQueue = db.PatientQueue.Where(PQ => PQ.PatientQueueID == patientQueueIDToQueryAgainst).FirstOrDefault();
                            db.PatientQueue.Remove(patientQueue);
                        }
                        else if (patientsToRelocate[i].SanatoriumID != null)
                        {
                            int? patientSanatoriumIDToQueryAgainst = patientsToRelocate[i].SanatoriumID;
                            var sanatoriumBed = db.Sanatorium.Where(S => S.SanatoriumID == patientSanatoriumIDToQueryAgainst).FirstOrDefault();
                            patientsToRelocate[i].SanatoriumID = null;
                            sanatoriumBed.AvailableBed = true;
                        }
                        else if (patientsToRelocate[i].IntensiveCareUnitID != null)
                        {
                            int? patientICUIDToQueryAgainst = patientsToRelocate[i].IntensiveCareUnitID;
                            var iCUBed = db.IntesiveCareUnit.Where(ICU => ICU.IntensiveCareUnitID == patientICUIDToQueryAgainst).FirstOrDefault();
                            patientsToRelocate[i].IntensiveCareUnitID = null;
                            iCUBed.AvailableBed = true;
                        }

                        if (patientsToRelocate[i].ConditionLevel == 10)
                        {
                            patientsToRelocate[i].Afterlife = afterLifte;
                            numberOfPatientsToAfterLife++;
                        }
                        else if (patientsToRelocate[i].ConditionLevel == 0)
                        {
                            patientsToRelocate[i].Discharged = discharged;
                            numberOfPatientsDischarged++;
                        }

                        patientsToRelocate[i].SignedOut = DateTime.UtcNow;
                        db.SaveChanges();
                    }

                    if (numberOfPatientsDischarged + numberOfPatientsToAfterLife != 0)
                    {
                        PatientsMoved?.Invoke(this, new KrankenhausMovedPatientsEventArgs() { NumberOfDeceasedPatients = numberOfPatientsToAfterLife, NumberOfRecoveredPatients = numberOfPatientsDischarged });
                    }

                }
            }
        }
        /// <summary>
        /// Creates 10 doctors and adds them to the Doctors table
        /// </summary>
        public void ThreadFive()
        {
            //Skapar doctors och lägger till dem i doctorstabellen
            using (var db = new KrankenhausContext())
            {
                for (int i = 0; i < 10; i++)
                {
                    var doctor = KrankenhausHelpMethods.GetANewRandomDoctor(rnd);
                    db.Doctors.Add(doctor);
                    db.SaveChanges();
                }
            }
        }
        /// <summary>
        /// Removes worn out doctor and replaces them with a fresh one if one exsists. Also tries to add the doctor with highest skill to ICU and second highest to sanatorium.
        /// </summary>
        public void ThreadSix()
        {
            lock (krankenhausLock)
            {
                using (var db = new KrankenhausContext())
                {

                    var iCUBeds = db.IntesiveCareUnit.ToList();
                    var sanatoriumBed = db.Sanatorium.ToList();

                    int? iCUDoctorIDToQueryAgainst = iCUBeds[0].DoctorID;
                    int? sanatoriumDoctorIDToQueryAgainst = sanatoriumBed[0].DoctorID;

                    var doctorInICU = db.Doctors.Where(D => D.DoctorID == iCUDoctorIDToQueryAgainst).FirstOrDefault();
                    var doctorInSanatorium = db.Doctors.Where(D => D.DoctorID == sanatoriumDoctorIDToQueryAgainst).FirstOrDefault();

                    if (doctorInICU != null && doctorInICU.NumberOfRotationsLeft < 1)
                    {
                        for (int i = 0; i < iCUBeds.Count; i++)
                        {
                            iCUBeds[i].DoctorID = null;
                        }
                        doctorInICU.assignedToICU = false;
                        db.SaveChanges();
                    }

                    if (doctorInSanatorium != null && doctorInSanatorium.NumberOfRotationsLeft < 1)
                    {
                        for (int i = 0; i < sanatoriumBed.Count; i++)
                        {
                            sanatoriumBed[i].DoctorID = null;
                        }
                        doctorInSanatorium.assignedToSantorium = false;
                        db.SaveChanges();
                    }

                    iCUBeds = db.IntesiveCareUnit.ToList();
                    iCUDoctorIDToQueryAgainst = iCUBeds[0].DoctorID;
                    var freshDoctorToICU = db.Doctors.Where(D => D.NumberOfRotationsLeft > 1 && D.assignedToSantorium == false && D.assignedToICU == false).OrderByDescending(D => D.SkillLevel).FirstOrDefault();
                    doctorInICU = db.Doctors.Where(D => D.DoctorID == iCUDoctorIDToQueryAgainst).FirstOrDefault();

                    if (doctorInICU == null && freshDoctorToICU != null)
                    {
                        for (int i = 0; i < iCUBeds.Count; i++)
                        {
                            iCUBeds[i].DoctorID = freshDoctorToICU.DoctorID;
                            freshDoctorToICU.assignedToICU = true;
                            db.SaveChanges();
                        }
                    }


                    sanatoriumBed = db.Sanatorium.ToList();
                    sanatoriumDoctorIDToQueryAgainst = sanatoriumBed[0].DoctorID;
                    doctorInSanatorium = db.Doctors.Where(D => D.DoctorID == sanatoriumDoctorIDToQueryAgainst).FirstOrDefault();
                    var freshDoctorToSanatorium = db.Doctors.Where(D => D.NumberOfRotationsLeft > 1 && D.assignedToSantorium == false && D.assignedToICU == false).OrderByDescending(D => D.SkillLevel).FirstOrDefault();

                    if (doctorInSanatorium == null && freshDoctorToSanatorium != null)
                    {
                        for (int i = 0; i < sanatoriumBed.Count; i++)
                        {
                            sanatoriumBed[i].DoctorID = freshDoctorToSanatorium.DoctorID;
                            freshDoctorToSanatorium.assignedToSantorium = true;
                            db.SaveChanges();
                        }
                    }

                }
            }
        }
        /// <summary>
        /// Continuously checks if the simulation is over by querying patients table to se if any patient is not signedout
        /// </summary>
        public void ThreadCheckIfSimulationOver()
        {
            lock (krankenhausLock)
            {
                using (var db = new KrankenhausContext())
                {
                    var patientsLeftInSimulation = db.Patients.Where(P => P.SignedOut == null).ToList();

                    if (patientsLeftInSimulation.Count == 0)
                    { 
                        SimulationOver?.Invoke(this, new KrankenhausEndOfSimlationEventArgs() { SimulationOver = true });
                    }
                }
            }
        }
        /// <summary>
        /// Removes all "current" data from the simulations tables. A record of all patients is being kept in patients history from all previous simulations.
        /// </summary>
        public void RemoveAllFromDatabase()
        {
            using (var db = new KrankenhausContext())
            {
                var patients = db.Patients.ToList();
                if (patients != null)
                {
                    for (int i = 0; i < patients.Count; i++)
                    {
                        patients[i].Afterlife = null;
                        patients[i].Discharged = null;
                        patients[i].IntensiveCareUnit = null;
                        patients[i].PatientQueue = null;
                        patients[i].Sanatorium = null;
                        db.SaveChanges();
                    }

                    db.SaveChanges();
                }
                var afterLifeList = db.AfterLives.ToList();
                if (afterLifeList != null)
                {
                    for (int i = 0; i < afterLifeList.Count; i++)
                    {
                        afterLifeList[i].Patients = null;
                        db.SaveChanges();
                    }

                    db.SaveChanges();
                }

                var dichargedList = db.Dischargeds.ToList();
                if (dichargedList != null)
                {
                    for (int i = 0; i < dichargedList.Count; i++)
                    {
                        dichargedList[i].Patients = null;
                        db.SaveChanges();
                    }

                    db.SaveChanges();
                }

                var doctors = db.Doctors.ToList();
                if (doctors != null)
                {
                    for (int i = 0; i < doctors.Count; i++)
                    {
                        db.SaveChanges();
                    }

                    db.SaveChanges();
                }

                var intensiveCareUnits = db.IntesiveCareUnit.ToList();
                if (intensiveCareUnits != null)
                {
                    for (int i = 0; i < intensiveCareUnits.Count; i++)
                    {
                        intensiveCareUnits[i].Patient = null;
                        intensiveCareUnits[i].Doctor = null;
                        intensiveCareUnits[i].DoctorID = null;
                        db.SaveChanges();
                    }

                    db.SaveChanges();
                }


                var patientQueues = db.PatientQueue.ToList();
                if (patientQueues != null)
                {
                    for (int i = 0; i < patientQueues.Count; i++)
                    {
                        patientQueues[i].Patient = null;
                        db.SaveChanges();
                    }

                    db.SaveChanges();
                }

                var sanatoria = db.Sanatorium.ToList();
                if (sanatoria != null)
                {
                    for (int i = 0; i < sanatoria.Count; i++)
                    {
                        sanatoria[i].Patient = null;
                        sanatoria[i].Doctor = null;
                        sanatoria[i].DoctorID = null;
                        db.SaveChanges();
                    }

                    db.SaveChanges();
                }

                db.SaveChanges();

                db.Patients.RemoveRange(patients);
                db.AfterLives.RemoveRange(afterLifeList);
                db.Dischargeds.RemoveRange(dichargedList);
                db.Doctors.RemoveRange(doctors);
                db.IntesiveCareUnit.RemoveRange(intensiveCareUnits);
                db.PatientQueue.RemoveRange(patientQueues);
                db.Sanatorium.RemoveRange(sanatoria);

                db.SaveChanges();
            }
        }




    }
}
