using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSantaGame.Models
{
    public class Employee
    {
        public string Name { get; set; }
        public string EmailId { get; set; }

        public Employee() { }

        public Employee(string name, string emailId)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            EmailId = emailId ?? throw new ArgumentNullException(nameof(emailId));
        }

        public override bool Equals(object obj)
        {
            if (obj is Employee other)
            {
                return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase) &&
                       string.Equals(EmailId, other.EmailId, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                Name?.ToLowerInvariant(),
                EmailId?.ToLowerInvariant()
            );
        }

        public override string ToString()
        {
            return $"{Name} ({EmailId})";
        }
    }
}
