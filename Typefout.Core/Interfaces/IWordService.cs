using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Typefout.Core.Models;

namespace Typefout.Core.Interfaces
{
    public interface IWordService
    {
        OefenWoord GetRandomized();
        List<OefenWoord> GetAllWords();
    }
}
