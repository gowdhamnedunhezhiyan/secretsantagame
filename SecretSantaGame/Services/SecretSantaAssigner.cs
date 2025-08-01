using SecretSantaGame.Exceptions;
using SecretSantaGame.Interface;
using SecretSantaGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSantaGame.Services
{
    public class SecretSantaAssigner : ISecretSantaAssigner
    {
        private readonly Random _random;
        private const int MaxRetries = 1000;

        public SecretSantaAssigner()
        {
            _random = new Random();
        }

        public SecretSantaAssigner(int seed)
        {
            _random = new Random(seed);
        }

        public List<SecretSantaAssignment> AssignSecretChildren(
            List<Employee> employees,
            List<SecretSantaAssignment> previousAssignments = null)
        {
            if (employees == null)
                throw new ArgumentNullException(nameof(employees));

            if (employees.Count < 2)
                throw new InvalidOperationException("At least 2 employees are required for Secret Santa.");

            // Remove duplicates
            var uniqueEmployees = employees.Distinct().ToList();

            if (uniqueEmployees.Count < 2)
                throw new InvalidOperationException("At least 2 unique employees are required for Secret Santa.");

            previousAssignments ??= new List<SecretSantaAssignment>();

            // Create a lookup for previous assignments
            var previousAssignmentLookup = CreatePreviousAssignmentLookup(previousAssignments, uniqueEmployees);

            // Try to create valid assignments
            for (int attempt = 0; attempt < MaxRetries; attempt++)
            {
                var assignments = TryCreateAssignments(uniqueEmployees, previousAssignmentLookup);
                if (assignments != null)
                {
                    return assignments;
                }
            }

            throw new SecretSantaException(
                $"Unable to create valid Secret Santa assignments after {MaxRetries} attempts. " +
                "This might be due to too many constraints from previous year's assignments.");
        }

        private Dictionary<string, string> CreatePreviousAssignmentLookup(
            List<SecretSantaAssignment> previousAssignments,
            List<Employee> currentEmployees)
        {
            var lookup = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var currentEmployeeEmails = new HashSet<string>(
                currentEmployees.Select(e => e.EmailId),
                StringComparer.OrdinalIgnoreCase);

            foreach (var assignment in previousAssignments)
            {
                // Only consider previous assignments where both employees are still in the company
                if (currentEmployeeEmails.Contains(assignment.Employee.EmailId) &&
                    currentEmployeeEmails.Contains(assignment.SecretChild.EmailId))
                {
                    lookup[assignment.Employee.EmailId] = assignment.SecretChild.EmailId;
                }
            }

            return lookup;
        }

        private List<SecretSantaAssignment> TryCreateAssignments(
            List<Employee> employees,
            Dictionary<string, string> previousAssignmentLookup)
        {
            var assignments = new List<SecretSantaAssignment>();
            var availableSecretChildren = new List<Employee>(employees);
            var shuffledEmployees = employees.OrderBy(x => _random.Next()).ToList();

            foreach (var employee in shuffledEmployees)
            {
                var validSecretChildren = availableSecretChildren
                    .Where(child => IsValidAssignment(employee, child, previousAssignmentLookup))
                    .ToList();

                if (validSecretChildren.Count == 0)
                {
                    return null; // Failed to create valid assignment
                }

                var selectedChild = validSecretChildren[_random.Next(validSecretChildren.Count)];
                assignments.Add(new SecretSantaAssignment(employee, selectedChild));
                availableSecretChildren.Remove(selectedChild);
            }

            return assignments;
        }

        private bool IsValidAssignment(
            Employee employee,
            Employee potentialChild,
            Dictionary<string, string> previousAssignmentLookup)
        {
            // Rule 1: Cannot assign to themselves
            if (employee.Equals(potentialChild))
                return false;

            // Rule 2: Cannot assign to the same person as last year
            if (previousAssignmentLookup.TryGetValue(employee.EmailId, out var previousChildEmail) &&
                string.Equals(previousChildEmail, potentialChild.EmailId, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }
    }
}
