using SecretSantaGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSantaGame.Interface
{
    namespace SecretSantaGame.Interfaces
    {
        public interface IFileProcessor
        {
            List<Employee> ReadEmployeesFromCsv(string filePath);
            List<SecretSantaAssignment> ReadPreviousAssignmentsFromCsv(string filePath);
            void WriteAssignmentsToCsv(List<SecretSantaAssignment> assignments, string filePath);
        }
    }
}
