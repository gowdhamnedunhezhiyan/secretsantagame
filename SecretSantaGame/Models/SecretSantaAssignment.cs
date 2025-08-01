using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSantaGame.Models
{
    public class SecretSantaAssignment
    {
        public Employee Employee { get; set; }
        public Employee SecretChild { get; set; }

        public SecretSantaAssignment() { }

        public SecretSantaAssignment(Employee employee, Employee secretChild)
        {
            Employee = employee ?? throw new ArgumentNullException(nameof(employee));
            SecretChild = secretChild ?? throw new ArgumentNullException(nameof(secretChild));
        }

        public override string ToString()
        {
            return $"{Employee.Name} -> {SecretChild.Name}";
        }
    }
}
