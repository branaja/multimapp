using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultimAPP
{
    public interface IStrategy
    {
        void Execute(string path, string zipName);
    }
}
