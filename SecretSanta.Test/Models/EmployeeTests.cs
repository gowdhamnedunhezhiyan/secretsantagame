using SecretSantaGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSanta.Test.Models
{
    public class EmployeeTests
    {
        [Fact]
        public void Constructor_ValidInputs_SetsProperties()
        {
            // Arrange
            var name = "John Doe";
            var email = "john.doe@acme.com";

            // Act
            var employee = new Employee(name, email);

            // Assert
            Assert.Equal(name, employee.Name);
            Assert.Equal(email, employee.EmailId);
        }

        [Fact]
        public void Constructor_NullName_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new Employee(null, "test@acme.com"));
        }

        [Fact]
        public void Constructor_NullEmail_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new Employee("John Doe", null));
        }

        [Fact]
        public void Equals_SameEmployees_ReturnsTrue()
        {
            // Arrange
            var employee1 = new Employee("John Doe", "john.doe@acme.com");
            var employee2 = new Employee("John Doe", "john.doe@acme.com");

            // Act & Assert
            Assert.True(employee1.Equals(employee2));
            Assert.True(employee2.Equals(employee1));
        }

        [Fact]
        public void Equals_SameEmployeesDifferentCase_ReturnsTrue()
        {
            // Arrange
            var employee1 = new Employee("John Doe", "john.doe@acme.com");
            var employee2 = new Employee("JOHN DOE", "JOHN.DOE@ACME.COM");

            // Act & Assert
            Assert.True(employee1.Equals(employee2));
        }

        [Fact]
        public void Equals_DifferentEmployees_ReturnsFalse()
        {
            // Arrange
            var employee1 = new Employee("John Doe", "john.doe@acme.com");
            var employee2 = new Employee("Jane Smith", "jane.smith@acme.com");

            // Act & Assert
            Assert.False(employee1.Equals(employee2));
        }

        [Fact]
        public void GetHashCode_SameEmployees_ReturnsSameHashCode()
        {
            // Arrange
            var employee1 = new Employee("John Doe", "john.doe@acme.com");
            var employee2 = new Employee("John Doe", "john.doe@acme.com");

            // Act & Assert
            Assert.Equal(employee1.GetHashCode(), employee2.GetHashCode());
        }
    }
}
