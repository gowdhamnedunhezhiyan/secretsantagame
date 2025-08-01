using SecretSantaGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSantaGame.Interface
{
    public interface ISecretSantaAssigner
    {
        List<SecretSantaAssignment> AssignSecretChildren(
            List<Employee> employees,
            List<SecretSantaAssignment> previousAssignments = null);
    }
}
