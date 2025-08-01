using SecretSantaGame;
using SecretSantaGame.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSanta.Test.Integration
{
    public class IntegrationTests : IDisposable
    {
        private readonly List<string> _tempFiles;
        private readonly SecretSantaGameManager _gameManager;

        public IntegrationTests()
        {
            _tempFiles = new List<string>();
            _gameManager = new SecretSantaGameManager();
        }

        [Fact]
        public void EndToEndTest_CompleteWorkflow_ProducesValidOutput()
        {
            // Arrange
            var employeeCsv = "Employee_Name,Employee_EmailID\n" +
                             "John Doe,john.doe@acme.com\n" +
                             "Jane Smith,jane.smith@acme.com\n" +
                             "Bob Johnson,bob.johnson@acme.com\n" +
                             "Alice Brown,alice.brown@acme.com";

            var previousCsv = "Employee_Name,Employee_EmailID,Secret_Child_Name,Secret_Child_EmailID\n" +
                             "John Doe,john.doe@acme.com,Jane Smith,jane.smith@acme.com";

            var employeeFile = CreateTempFile(employeeCsv);
            var previousFile = CreateTempFile(previousCsv);
            var outputFile = GetTempFilePath();


            _gameManager.RunSecretSantaGame(employeeFile, outputFile, previousFile);
            Assert.True(File.Exists(outputFile));

            var processor = new CsvFileProcessor();
            var results = processor.ReadPreviousAssignmentsFromCsv(outputFile);

            Assert.Equal(4, results.Count);

            // Verify no self-assignments
            Assert.All(results, assignment =>
                Assert.NotEqual(assignment.Employee, assignment.SecretChild));

            // Verify each employee appears exactly once as giver and receiver
            var givers = results.Select(r => r.Employee.EmailId).ToList();
            var receivers = results.Select(r => r.SecretChild.EmailId).ToList();

            Assert.Equal(4, givers.Distinct().Count());
            Assert.Equal(4, receivers.Distinct().Count());

            // Verify John is not assigned to Jane (previous year constraint)
            var johnAssignment = results.First(r => r.Employee.EmailId == "john.doe@acme.com");
            Assert.NotEqual("jane.smith@acme.com", johnAssignment.SecretChild.EmailId);
        }

        [Fact]
        public void EndToEndTest_MinimalCase_TwoEmployees()
        {
            // Arrange
            var employeeCsv = "Employee_Name,Employee_EmailID\n" +
                             "John Doe,john.doe@acme.com\n" +
                             "Jane Smith,jane.smith@acme.com";

            var employeeFile = CreateTempFile(employeeCsv);
            var outputFile = GetTempFilePath();

            _gameManager.RunSecretSantaGame(employeeFile, outputFile);
            Assert.True(File.Exists(outputFile));

            var processor = new CsvFileProcessor();
            var results = processor.ReadPreviousAssignmentsFromCsv(outputFile);

            Assert.Equal(2, results.Count);

            // With only 2 employees, they must be assigned to each other
            var johnAssignment = results.First(r => r.Employee.Name == "John Doe");
            var janeAssignment = results.First(r => r.Employee.Name == "Jane Smith");

            Assert.Equal("Jane Smith", johnAssignment.SecretChild.Name);
            Assert.Equal("John Doe", janeAssignment.SecretChild.Name);
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
