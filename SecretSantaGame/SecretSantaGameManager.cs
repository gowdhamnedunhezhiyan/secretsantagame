using SecretSantaGame.Exceptions;
using SecretSantaGame.Interface;
using SecretSantaGame.Interface.SecretSantaGame.Interfaces;
using SecretSantaGame.Models;
using SecretSantaGame.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSantaGame
{
    public class SecretSantaGameManager
    {
        private readonly IFileProcessor _fileProcessor;
        private readonly ISecretSantaAssigner _assigner;

        public SecretSantaGameManager() : this(new CsvFileProcessor(), new SecretSantaAssigner()) { }

        public SecretSantaGameManager(IFileProcessor fileProcessor, ISecretSantaAssigner assigner)
        {
            _fileProcessor = fileProcessor ?? throw new ArgumentNullException(nameof(fileProcessor));
            _assigner = assigner ?? throw new ArgumentNullException(nameof(assigner));
        }

        public void RunSecretSantaGame(string employeeFilePath, string outputFilePath, string previousAssignmentsFilePath = null)
        {
            try
            {
                Console.WriteLine("Starting Secret Santa Game...");

                // Read employees
                Console.WriteLine($"Reading employees from: {employeeFilePath}");
                var employees = _fileProcessor.ReadEmployeesFromCsv(employeeFilePath);
                Console.WriteLine($"Found {employees.Count} employees");

                // Read previous assignments
                List<SecretSantaAssignment> previousAssignments = null;
                if (!string.IsNullOrWhiteSpace(previousAssignmentsFilePath))
                {
                    Console.WriteLine($"Reading previous assignments from: {previousAssignmentsFilePath}");
                    previousAssignments = _fileProcessor.ReadPreviousAssignmentsFromCsv(previousAssignmentsFilePath);
                    Console.WriteLine($"Found {previousAssignments.Count} previous assignments");
                }

                // Create new assignments
                Console.WriteLine("Creating new Secret Santa assignments...");
                var newAssignments = _assigner.AssignSecretChildren(employees, previousAssignments);
                Console.WriteLine($"Successfully created {newAssignments.Count} assignments");

                // Write output
                Console.WriteLine($"Writing assignments to: {outputFilePath}");
                _fileProcessor.WriteAssignmentsToCsv(newAssignments, outputFilePath);
                Console.WriteLine("Secret Santa assignments saved successfully!");

                // Display summary
                DisplayAssignmentSummary(newAssignments);
            }
            catch (SecretSantaException)
            {
                // Re-throw SecretSantaExceptions directly
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
                throw new SecretSantaException("An unexpected error occurred during Secret Santa game execution.", ex);
            }
        }

        private void DisplayAssignmentSummary(List<SecretSantaAssignment> assignments)
        {
            Console.WriteLine("\nAssignment Summary:");
            Console.WriteLine(new string('-', 50));
            foreach (var assignment in assignments)
            {
                Console.WriteLine($"{assignment.Employee.Name} → {assignment.SecretChild.Name}");
            }
            Console.WriteLine(new string('-', 50));
        }
    }
}
