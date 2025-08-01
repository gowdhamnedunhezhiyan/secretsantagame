using SecretSantaGame.Exceptions;
using SecretSantaGame.Interface.SecretSantaGame.Interfaces;
using SecretSantaGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSantaGame.Services
{
    public class CsvFileProcessor : IFileProcessor
    {
        public List<Employee> ReadEmployeesFromCsv(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Employee file not found: {filePath}");

            try
            {
                var employees = new List<Employee>();
                var lines = File.ReadAllLines(filePath);

                if (lines.Length == 0)
                    throw new SecretSantaException("Employee file is empty.");

                // Skip header row
                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i].Trim();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var parts = ParseCsvLine(line);
                    if (parts.Length < 2)
                        throw new SecretSantaException($"Invalid data format at line {i + 1}: {line}");

                    var employee = new Employee(parts[0].Trim(), parts[1].Trim());

                    if (string.IsNullOrWhiteSpace(employee.Name) || string.IsNullOrWhiteSpace(employee.EmailId))
                        throw new SecretSantaException($"Employee name and email cannot be empty at line {i + 1}");

                    employees.Add(employee);
                }

                if (employees.Count == 0)
                    throw new SecretSantaException("No valid employees found in the file.");

                return employees;
            }
            catch (Exception ex) when (!(ex is SecretSantaException) && !(ex is ArgumentException) && !(ex is FileNotFoundException))
            {
                throw new SecretSantaException($"Error reading employee file: {ex.Message}", ex);
            }
        }

        public List<SecretSantaAssignment> ReadPreviousAssignmentsFromCsv(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return new List<SecretSantaAssignment>();

            if (!File.Exists(filePath))
                return new List<SecretSantaAssignment>();

            try
            {
                var assignments = new List<SecretSantaAssignment>();
                var lines = File.ReadAllLines(filePath);

                if (lines.Length <= 1) // Only header or empty
                    return assignments;

                // Skip header row
                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i].Trim();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var parts = ParseCsvLine(line);
                    if (parts.Length < 4)
                        continue; // Skip invalid lines in previous assignments

                    var employee = new Employee(parts[0].Trim(), parts[1].Trim());
                    var secretChild = new Employee(parts[2].Trim(), parts[3].Trim());

                    assignments.Add(new SecretSantaAssignment(employee, secretChild));
                }

                return assignments;
            }
            catch (Exception ex)
            {
                throw new SecretSantaException($"Error reading previous assignments file: {ex.Message}", ex);
            }
        }

        public void WriteAssignmentsToCsv(List<SecretSantaAssignment> assignments, string filePath)
        {
            if (assignments == null)
                throw new ArgumentNullException(nameof(assignments));

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Output file path cannot be null or empty.", nameof(filePath));

            try
            {
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var writer = new StreamWriter(filePath))
                {
                    // Write header
                    writer.WriteLine("Employee_Name,Employee_EmailID,Secret_Child_Name,Secret_Child_EmailID");

                    // Write assignments
                    foreach (var assignment in assignments)
                    {
                        var line = $"{EscapeCsvField(assignment.Employee.Name)}," +
                                  $"{EscapeCsvField(assignment.Employee.EmailId)}," +
                                  $"{EscapeCsvField(assignment.SecretChild.Name)}," +
                                  $"{EscapeCsvField(assignment.SecretChild.EmailId)}";
                        writer.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new SecretSantaException($"Error writing assignments to file: {ex.Message}", ex);
            }
        }

        private string[] ParseCsvLine(string line)
        {
            var result = new List<string>();
            var current = "";
            var inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                var c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(current);
                    current = "";
                }
                else
                {
                    current += c;
                }
            }

            result.Add(current);
            return result.ToArray();
        }

        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return "";

            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
            {
                return "\"" + field.Replace("\"", "\"\"") + "\"";
            }

            return field;
        }
    }

}
