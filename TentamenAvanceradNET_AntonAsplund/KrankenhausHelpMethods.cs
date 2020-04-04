using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TentamenAvanceradNET_AntonAsplund.Database;
using RandomNameGeneratorLibrary;

namespace TentamenAvanceradNET_AntonAsplund.Database
{
    /// <summary>
    /// Holds small help methods used by krankenhaus simulation
    /// </summary>
    class KrankenhausHelpMethods
    {
        /// <summary>
        /// Returns a new random patient instance
        /// </summary>
        /// <param name="rnd"></param>
        /// <returns></returns>
        public static Patient GetANewRandomPatient(Random rnd)
        {
            var patientToReturn = new Patient();

            var rndName = new PersonNameGenerator(rnd);
            patientToReturn.Name = rndName.GenerateRandomFirstAndLastName();

            StringBuilder tempSSN = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {{}
                tempSSN.Append(rnd.Next(1, 10));
            }

            patientToReturn.SSN = double.Parse(tempSSN.ToString());
            patientToReturn.Age = rnd.Next(1, 120);
            patientToReturn.ArrivalAtHospital = DateTime.UtcNow;
            patientToReturn.ConditionLevel = rnd.Next(1, 10);

            return patientToReturn;

        }
        /// <summary>
        /// Returns a new random doctor instance
        /// </summary>
        /// <param name="rnd"></param>
        /// <returns></returns>
        public static Doctor GetANewRandomDoctor(Random rnd)
        {
            var doctorToReturn = new Doctor();

            var rndName = new PersonNameGenerator(rnd);
            doctorToReturn.Name = rndName.GenerateRandomFirstAndLastName();
            doctorToReturn.SkillLevel = rnd.Next(1, 101);
            doctorToReturn.NumberOfRotationsLeft = 3;

            return doctorToReturn;
        }
        /// <summary>
        /// Returns a new patientHistory instance based on a patient instance and simulation number.
        /// </summary>
        /// <param name="patientToConvert"></param>
        /// <param name="simulationNumber"></param>
        /// <returns></returns>
        public static PatientHistory GetAPatientHistory(Patient patientToConvert, int simulationNumber)
        {
            var patientsHistory = new PatientHistory();
            patientsHistory.Name = patientToConvert.Name;
            patientsHistory.SSN = patientToConvert.SSN;
            patientsHistory.Age = patientToConvert.Age;
            patientsHistory.Status = patientToConvert.AfterLifeID == null ? "Recovered" : "Deceased";
            patientsHistory.SimulationNumber = simulationNumber;
            patientsHistory.ArrivalAtHospital = patientToConvert.ArrivalAtHospital;
            patientsHistory.AssingedToHopsitalBed = patientToConvert.AssingedToHopsitalBed;
            patientsHistory.SignedOut = patientToConvert.SignedOut;

            return patientsHistory;
        }
        /// <summary>
        /// Modifys an int value coming from a patient in connection to the patientQueue table based on modifiers given by client
        /// </summary>
        /// <param name="rnd"></param>
        /// <param name="conditionLevel"></param>
        /// <returns></returns>
        public static int ModifyCoditionLevelQueue(Random rnd, int conditionLevel)
        {
            //1-50 = no change in conditionLevel
            //51-60 = minus 1 to conditionLevel
            //61-90 = plus 1 to conditionLevel
            //91-100 = plus 3 to conditionLevel

            int randomValue = rnd.Next(1, 101);

            if (randomValue >= 51 && randomValue <= 60)
            {
                conditionLevel += -1;
            }
            else if (randomValue >= 61 && randomValue <= 90)
            {
                conditionLevel += 1;
            }
            else if (randomValue >= 91 && randomValue <= 100)
            {
                conditionLevel += 3;
            }

            if (conditionLevel > 10)
            {
                conditionLevel = 10;
            }
            if (conditionLevel < 0)
            {
                conditionLevel = 0;
            }

            return conditionLevel;
        }
        /// <summary>
        /// Modifys an int value coming from a patient in connection to the Sanatorium table based on modifiers given by client
        /// </summary>
        /// <param name="rnd"></param>
        /// <param name="conditionLevel"></param>
        /// <param name="doctorSkill"></param>
        /// <returns></returns>
        public static int ModifyCoditionLevelSanatorium(Random rnd, int conditionLevel, int doctorSkill = 0)
        {
            //1-65 = no change in conditionLevel
            //66-85 = minus 1 to conditionLevel
            //86-95 = plus 1 to conditionLevel
            //96-100 = plus 3 to conditionLevel

            int randomValue = rnd.Next(1, 101);

            if (randomValue >= 66 && randomValue <= 85)
            {
                conditionLevel += -1;
            }
            else if (randomValue >= 86 && randomValue <= 95)
            {
                conditionLevel += 1;
            }
            else if (randomValue >= 96 && randomValue <= 100)
            {
                conditionLevel += 3;
            }

            int randomDoctorValue = rnd.Next(1,101);

            if (doctorSkill >= randomDoctorValue)
            {
                conditionLevel += -1;
            }
            

            if (conditionLevel > 10)
            {
                conditionLevel = 10;
            }
            if (conditionLevel < 0)
            {
                conditionLevel = 0;
            }

            return conditionLevel;
        }
        /// <summary>
        /// Modifys an int value coming from a patient in connection to the ICU table based on modifiers given by client
        /// </summary>
        /// <param name="rnd"></param>
        /// <param name="conditionLevel"></param>
        /// <param name="doctorSkill"></param>
        /// <returns></returns>
        public static int ModifyCoditionLevelICU(Random rnd, int conditionLevel, int doctorSkill = 0)
        {
            //1-20 = no change in conditionLevel
            //21-80 = minus 3 to conditionLevel
            //81-90 = plus 1 to conditionLevel
            //91-100 = plus 2 to conditionLevel

            int randomValue = rnd.Next(1, 101);

            if (randomValue >= 21 && randomValue <= 80)
            {
                conditionLevel += -3;
            }
            else if (randomValue >= 81 && randomValue <= 90)
            {
                conditionLevel += 1;
            }
            else if (randomValue >= 91 && randomValue <= 100)
            {
                conditionLevel += 2;
            }

            int randomDoctorValue = rnd.Next(1, 101);

            if (doctorSkill >= randomDoctorValue)
            {
                conditionLevel += -1;
            }


            if (conditionLevel > 10)
            {
                conditionLevel = 10;
            }
            if (conditionLevel < 0)
            {
                conditionLevel = 0;
            }

            return conditionLevel;
        }
    }
}
