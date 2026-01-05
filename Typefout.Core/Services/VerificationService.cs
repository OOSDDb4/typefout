using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Util;
using Typefout.Core.Interfaces;

namespace Typefout.Core.Services
{
    public class VerificationService : IVerificationService
    {
        private static Dictionary<string, string> _activeCodes = new Dictionary<string, string>();

        public string GenerateVerificationCode(string email)
        {
            if(_activeCodes.ContainsKey(email))
            {
                System.Diagnostics.Debug.WriteLine($"{_activeCodes[email]}");
                return _activeCodes[email];
            }

            string code = Random.Shared.Next(0, 1000000).ToString("D6");

            _activeCodes[email] = code;

            RemoveCodeAfterDelay(email, 300);

            System.Diagnostics.Debug.WriteLine($"Code voor {email} is: {code}");
            return code;
        }

        private async void RemoveCodeAfterDelay(string email, int seconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));

            if (_activeCodes.ContainsKey(email))
            {
                ReleaseCode(email);
                System.Diagnostics.Debug.WriteLine($"Code voor {email} is verlopen.");
            }
        }

        public void ReleaseCode(string email)
        {
            if (!string.IsNullOrEmpty(email) && _activeCodes.ContainsKey(email))
            {
                _activeCodes.Remove(email);
                System.Diagnostics.Debug.WriteLine($"Code is vrijgegeven voor {email} en kan weer gebruikt worden.");
            }
        }

        public bool TryValidateCode(string email, string inputCode)
        {
            if (_activeCodes.TryGetValue(email, out string correctCode))
            {
                if (correctCode == inputCode)
                {
                    ReleaseCode(email);
                    return true;
                }
            }
            return false;
        }
    }
}
