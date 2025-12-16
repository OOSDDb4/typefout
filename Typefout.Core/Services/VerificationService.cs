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
        private static HashSet<string> _activeCodes = new HashSet<string>();

        public string GenerateVerificationCode()
        {
            string code;
            int safetyCounter = 0;

            do
            {
                code = Random.Shared.Next(0, 1000000).ToString("D6");

                safetyCounter++;
                if (safetyCounter > 100) throw new Exception("Kan geen unieke code genereren");

            }
            while (_activeCodes.Contains(code));

            _activeCodes.Add(code);
            RemoveCodeAfterDelay(code, 300); 

            System.Diagnostics.Debug.WriteLine($"Code is: {code}");
            return code;
        }

        private async void RemoveCodeAfterDelay(string code, int seconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
            ReleaseCode(code);
        }

        public void ReleaseCode(string code)
        {
            if (!string.IsNullOrEmpty(code) && _activeCodes.Contains(code))
            {
                _activeCodes.Remove(code);
                System.Diagnostics.Debug.WriteLine($"Code {code} is vrijgegeven en kan weer gebruikt worden.");
            }
        }
    }
}
