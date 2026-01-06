using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Typefout.Core.Interfaces
{
    public interface IVerificationService
    {
        public string GenerateVerificationCode(string email);

        public void ReleaseCode(string email);

        public bool TryValidateCode(string email, string inputCode);
    }
}
