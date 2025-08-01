using SecretSantaGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSanta.Test.Models
{
    public class SecretSantaAssignmentTests
    {
        [Fact]
        public void Constructor_ValidInputs_SetsProperties()
        {
            // Arrange
            var employee = new Employee("John Doe", "john.doe@acme.com");
            var secretChild = new Employee("Jane Smith", "jane.smith@acme.com");

            // Act
            var assignment = new SecretSantaAssignment(employee, secretChild);

            // Assert
            Assert.Equal(employee, assignment.Employee);
            Assert.Equal(secretChild, assignment.SecretChild);
        }

        [Fact]
        public void Constructor_NullEmployee_ThrowsArgumentNullException()
        {
            // Arrange
            var secretChild = new Employee("Jane Smith", "jane.smith@acme.com");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SecretSantaAssignment(null, secretChild));
        }

        [Fact]
        public void Constructor_NullSecretChild_ThrowsArgumentNullException()
        {
            // Arrange
            var employee = new Employee("John Doe", "john.doe@acme.com");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SecretSantaAssignment(employee, null));
        }
    }
}
