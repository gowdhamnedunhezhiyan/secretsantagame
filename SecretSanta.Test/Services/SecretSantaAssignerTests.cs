using SecretSantaGame.Models;
using SecretSantaGame.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSanta.Test.Services
{
    public class SecretSantaAssignerTests
    {
        private readonly SecretSantaAssigner _assigner;

        public SecretSantaAssignerTests()
        {
            _assigner = new SecretSantaAssigner(12345); // Fixed seed for predictable tests
        }

        [Fact]
        public void AssignSecretChildren_NullEmployees_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _assigner.AssignSecretChildren(null));
        }

        [Fact]
        public void AssignSecretChildren_LessThanTwoEmployees_ThrowsInvalidOperationException()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee("John Doe", "john.doe@acme.com")
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _assigner.AssignSecretChildren(employees));
        }

        [Fact]
        public void AssignSecretChildren_TwoEmployees_CreatesValidAssignments()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee("John Doe", "john.doe@acme.com"),
                new Employee("Jane Smith", "jane.smith@acme.com")
            };

            // Act
            var assignments = _assigner.AssignSecretChildren(employees);

            // Assert
            Assert.Equal(2, assignments.Count);
            Assert.All(assignments, assignment =>
                Assert.NotEqual(assignment.Employee, assignment.SecretChild));

            // Verify each employee has exactly one assignment
            var employeeAssignments = assignments.GroupBy(a => a.Employee).ToList();
            Assert.All(employeeAssignments, group => Assert.Single(group));

            // Verify each employee is assigned as secret child exactly once
            var childAssignments = assignments.GroupBy(a => a.SecretChild).ToList();
            Assert.All(childAssignments, group => Assert.Single(group));
        }

        [Fact]
        public void AssignSecretChildren_MultipleEmployees_CreatesValidAssignments()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee("John Doe", "john.doe@acme.com"),
                new Employee("Jane Smith", "jane.smith@acme.com"),
                new Employee("Bob Johnson", "bob.johnson@acme.com"),
                new Employee("Alice Brown", "alice.brown@acme.com")
            };

            // Act
            var assignments = _assigner.AssignSecretChildren(employees);

            // Assert
            Assert.Equal(4, assignments.Count);

            // Verify no self-assignments
            Assert.All(assignments, assignment =>
                Assert.NotEqual(assignment.Employee, assignment.SecretChild));

            // Verify each employee has exactly one assignment
            var employeeAssignments = assignments.GroupBy(a => a.Employee).ToList();
            Assert.Equal(4, employeeAssignments.Count);
            Assert.All(employeeAssignments, group => Assert.Single(group));

            // Verify each employee is assigned as secret child exactly once
            var childAssignments = assignments.GroupBy(a => a.SecretChild).ToList();
            Assert.Equal(4, childAssignments.Count);
            Assert.All(childAssignments, group => Assert.Single(group));
        }

        [Fact]
        public void AssignSecretChildren_WithPreviousAssignments_AvoidsRepeatingAssignments()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee("John Doe", "john.doe@acme.com"),
                new Employee("Jane Smith", "jane.smith@acme.com"),
                new Employee("Bob Johnson", "bob.johnson@acme.com")
            };

            var previousAssignments = new List<SecretSantaAssignment>
            {
                new SecretSantaAssignment(employees[0], employees[1]) // John -> Jane
            };

            // Act
            var assignments = _assigner.AssignSecretChildren(employees, previousAssignments);

            // Assert
            Assert.Equal(3, assignments.Count);

            // Verify John is not assigned to Jane again
            var johnAssignment = assignments.First(a => a.Employee.Equals(employees[0]));
            Assert.NotEqual(employees[1], johnAssignment.SecretChild);
        }

        [Fact]
        public void AssignSecretChildren_DuplicateEmployees_RemovesDuplicates()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee("John Doe", "john.doe@acme.com"),
                new Employee("Jane Smith", "jane.smith@acme.com"),
                new Employee("John Doe", "john.doe@acme.com"), // Duplicate
                new Employee("Bob Johnson", "bob.johnson@acme.com")
            };

            // Act
            var assignments = _assigner.AssignSecretChildren(employees);

            // Assert
            Assert.Equal(3, assignments.Count); // Should only have 3 unique employees
        }
    }
}
