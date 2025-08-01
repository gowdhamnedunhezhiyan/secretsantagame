using SecretSantaGame.Exceptions;
using SecretSantaGame.Models;
using SecretSantaGame.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSanta.Test.Services
{
    public class CsvFileProcessorTests : IDisposable
    {
        private readonly CsvFileProcessor _processor;
        private readonly List<string> _tempFiles;

        public CsvFileProcessorTests()
        {
            _processor = new CsvFileProcessor();
            _tempFiles = new List<string>();
        }

        [Fact]
        public void ReadEmployeesFromCsv_ValidFile_ReturnsEmployees()
        {
            // Arrange
            var csvContent = "Employee_Name,Employee_EmailID\nJohn Doe,john.doe@acme.com\nJane Smith,jane.smith@acme.com";
            var filePath = CreateTempFile(csvContent);

            // Act
            var employees = _processor.ReadEmployeesFromCsv(filePath);

            // Assert
            Assert.Equal(2, employees.Count);
            Assert.Equal("John Doe", employees[0].Name);
            Assert.Equal("john.doe@acme.com", employees[0].EmailId);
            Assert.Equal("Jane Smith", employees[1].Name);
            Assert.Equal("jane.smith@acme.com", employees[1].EmailId);
        }

        [Fact]
        public void ReadEmployeesFromCsv_EmptyFile_ThrowsSecretSantaException()
        {
            // Arrange
            var filePath = CreateTempFile("");

            // Act & Assert
            var exception = Assert.Throws<SecretSantaException>(() => _processor.ReadEmployeesFromCsv(filePath));
            Assert.Contains("Employee file is empty", exception.Message);
        }

        [Fact]
        public void ReadEmployeesFromCsv_OnlyHeader_ThrowsSecretSantaException()
        {
            // Arrange
            var csvContent = "Employee_Name,Employee_EmailID";
            var filePath = CreateTempFile(csvContent);

            // Act & Assert
            var exception = Assert.Throws<SecretSantaException>(() => _processor.ReadEmployeesFromCsv(filePath));
            Assert.Contains("No valid employees found in the file", exception.Message);
        }

        [Fact]
        public void ReadEmployeesFromCsv_InvalidData_ThrowsSecretSantaException()
        {
            // Arrange
            var csvContent = "Employee_Name,Employee_EmailID\nJohn Doe"; // Missing email
            var filePath = CreateTempFile(csvContent);

            // Act & Assert
            var exception = Assert.Throws<SecretSantaException>(() => _processor.ReadEmployeesFromCsv(filePath));
            Assert.Contains("Invalid data format at line 2", exception.Message);
        }

        [Fact]
        public void ReadEmployeesFromCsv_NonExistentFile_ThrowsFileNotFoundException()
        {
            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => _processor.ReadEmployeesFromCsv("nonexistent.csv"));
        }

        [Fact]
        public void ReadEmployeesFromCsv_NullFilePath_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _processor.ReadEmployeesFromCsv(null));
        }

        [Fact]
        public void ReadPreviousAssignmentsFromCsv_ValidFile_ReturnsAssignments()
        {
            // Arrange
            var csvContent = "Employee_Name,Employee_EmailID,Secret_Child_Name,Secret_Child_EmailID\n" +
                           "John Doe,john.doe@acme.com,Jane Smith,jane.smith@acme.com\n" +
                           "Jane Smith,jane.smith@acme.com,Bob Johnson,bob.johnson@acme.com";
            var filePath = CreateTempFile(csvContent);

            // Act
            var assignments = _processor.ReadPreviousAssignmentsFromCsv(filePath);

            // Assert
            Assert.Equal(2, assignments.Count);
            Assert.Equal("John Doe", assignments[0].Employee.Name);
            Assert.Equal("Jane Smith", assignments[0].SecretChild.Name);
        }

        [Fact]
        public void ReadPreviousAssignmentsFromCsv_NonExistentFile_ReturnsEmptyList()
        {
            // Act
            var assignments = _processor.ReadPreviousAssignmentsFromCsv("nonexistent.csv");

            // Assert
            Assert.Empty(assignments);
        }

        [Fact]
        public void ReadPreviousAssignmentsFromCsv_NullFilePath_ReturnsEmptyList()
        {
            // Act
            var assignments = _processor.ReadPreviousAssignmentsFromCsv(null);

            // Assert
            Assert.Empty(assignments);
        }

        [Fact]
        public void WriteAssignmentsToCsv_ValidAssignments_WritesFile()
        {
            // Arrange
            var assignments = new List<SecretSantaAssignment>
            {
                new SecretSantaAssignment(
                    new Employee("John Doe", "john.doe@acme.com"),
                    new Employee("Jane Smith", "jane.smith@acme.com")
                ),
                new SecretSantaAssignment(
                    new Employee("Jane Smith", "jane.smith@acme.com"),
                    new Employee("John Doe", "john.doe@acme.com")
                )
            };
            var filePath = GetTempFilePath();

            // Act
            _processor.WriteAssignmentsToCsv(assignments, filePath);

            // Assert
            Assert.True(File.Exists(filePath));
            var content = File.ReadAllText(filePath);
            Assert.Contains("Employee_Name,Employee_EmailID,Secret_Child_Name,Secret_Child_EmailID", content);
            Assert.Contains("John Doe,john.doe@acme.com,Jane Smith,jane.smith@acme.com", content);
            Assert.Contains("Jane Smith,jane.smith@acme.com,John Doe,john.doe@acme.com", content);
        }

        [Fact]
        public void WriteAssignmentsToCsv_NullAssignments_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _processor.WriteAssignmentsToCsv(null, "test.csv"));
        }

        [Fact]
        public void WriteAssignmentsToCsv_NullFilePath_ThrowsArgumentException()
        {
            // Arrange
            var assignments = new List<SecretSantaAssignment>();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _processor.WriteAssignmentsToCsv(assignments, null));
        }

        [Fact]
        public void WriteAssignmentsToCsv_FieldsWithCommas_EscapesCorrectly()
        {
            // Arrange
            var assignments = new List<SecretSantaAssignment>
            {
                new SecretSantaAssignment(
                    new Employee("Doe, John", "john.doe@acme.com"),
                    new Employee("Smith, Jane", "jane.smith@acme.com")
                )
            };
            var filePath = GetTempFilePath();

            // Act
            _processor.WriteAssignmentsToCsv(assignments, filePath);

            // Assert
            var content = File.ReadAllText(filePath);
            Assert.Contains("\"Doe, John\",john.doe@acme.com,\"Smith, Jane\",jane.smith@acme.com", content);
        }

        private string CreateTempFile(string content)
        {
            var filePath = GetTempFilePath();
            File.WriteAllText(filePath, content);
            return filePath;
        }

        private string GetTempFilePath()
        {
            var filePath = Path.GetTempFileName();
            _tempFiles.Add(filePath);
            return filePath;
        }

        public void Dispose()
        {
            foreach (var file in _tempFiles)
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
        }
    }
}
