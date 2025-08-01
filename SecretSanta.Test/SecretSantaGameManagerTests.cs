using Moq;
using SecretSantaGame;
using SecretSantaGame.Exceptions;
using SecretSantaGame.Interface;
using SecretSantaGame.Interface.SecretSantaGame.Interfaces;
using SecretSantaGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSanta.Test
{
    public class SecretSantaGameManagerTests
    {
        private readonly Mock<IFileProcessor> _mockFileProcessor;
        private readonly Mock<ISecretSantaAssigner> _mockAssigner;
        private readonly SecretSantaGameManager _gameManager;

        public SecretSantaGameManagerTests()
        {
            _mockFileProcessor = new Mock<IFileProcessor>();
            _mockAssigner = new Mock<ISecretSantaAssigner>();
            _gameManager = new SecretSantaGameManager(_mockFileProcessor.Object, _mockAssigner.Object);
        }

        [Fact]
        public void Constructor_NullFileProcessor_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SecretSantaGameManager(null, _mockAssigner.Object));
        }

        [Fact]
        public void Constructor_NullAssigner_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SecretSantaGameManager(_mockFileProcessor.Object, null));
        }

        [Fact]
        public void RunSecretSantaGame_ValidInputs_CompletesSuccessfully()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee("John Doe", "john.doe@acme.com"),
                new Employee("Jane Smith", "jane.smith@acme.com")
            };

            var assignments = new List<SecretSantaAssignment>
            {
                new SecretSantaAssignment(employees[0], employees[1]),
                new SecretSantaAssignment(employees[1], employees[0])
            };

            _mockFileProcessor.Setup(fp => fp.ReadEmployeesFromCsv("employees.csv"))
                             .Returns(employees);
            _mockFileProcessor.Setup(fp => fp.ReadPreviousAssignmentsFromCsv("previous.csv"))
                             .Returns(new List<SecretSantaAssignment>());
            _mockAssigner.Setup(a => a.AssignSecretChildren(employees, It.IsAny<List<SecretSantaAssignment>>()))
                        .Returns(assignments);

            // Act & Assert (Should not throw)
            _gameManager.RunSecretSantaGame("employees.csv", "output.csv", "previous.csv");

            // Verify all methods were called
            _mockFileProcessor.Verify(fp => fp.ReadEmployeesFromCsv("employees.csv"), Times.Once);
            _mockFileProcessor.Verify(fp => fp.ReadPreviousAssignmentsFromCsv("previous.csv"), Times.Once);
            _mockFileProcessor.Verify(fp => fp.WriteAssignmentsToCsv(assignments, "output.csv"), Times.Once);
            _mockAssigner.Verify(a => a.AssignSecretChildren(employees, It.IsAny<List<SecretSantaAssignment>>()), Times.Once);
        }

        [Fact]
        public void RunSecretSantaGame_FileProcessorThrowsException_ThrowsSecretSantaException()
        {
            // Arrange
            _mockFileProcessor.Setup(fp => fp.ReadEmployeesFromCsv(It.IsAny<string>()))
                             .Throws(new FileNotFoundException("File not found"));

            // Act & Assert
            var exception = Assert.Throws<SecretSantaException>(() => _gameManager.RunSecretSantaGame("employees.csv", "output.csv"));
            Assert.Contains("An unexpected error occurred during Secret Santa game execution", exception.Message);
            Assert.IsType<FileNotFoundException>(exception.InnerException);
        }

        [Fact]
        public void RunSecretSantaGame_AssignerThrowsException_ThrowsSecretSantaException()
        {
            // Arrange
            var employees = new List<Employee> { new Employee("John Doe", "john.doe@acme.com") };

            _mockFileProcessor.Setup(fp => fp.ReadEmployeesFromCsv(It.IsAny<string>()))
                             .Returns(employees);
            _mockAssigner.Setup(a => a.AssignSecretChildren(It.IsAny<List<Employee>>(), It.IsAny<List<SecretSantaAssignment>>()))
                        .Throws(new InvalidOperationException("Not enough employees"));

            // Act & Assert
            var exception = Assert.Throws<SecretSantaException>(() => _gameManager.RunSecretSantaGame("employees.csv", "output.csv"));
            Assert.Contains("An unexpected error occurred during Secret Santa game execution", exception.Message);
            Assert.IsType<InvalidOperationException>(exception.InnerException);
        }

        [Fact]
        public void RunSecretSantaGame_SecretSantaExceptionThrown_RethrowsDirectly()
        {
            // Arrange
            var secretSantaException = new SecretSantaException("Custom error");
            _mockFileProcessor.Setup(fp => fp.ReadEmployeesFromCsv(It.IsAny<string>()))
                             .Throws(secretSantaException);

            // Act & Assert
            var thrownException = Assert.Throws<SecretSantaException>(() => _gameManager.RunSecretSantaGame("employees.csv", "output.csv"));
            Assert.Same(secretSantaException, thrownException);
        }
    }
}
