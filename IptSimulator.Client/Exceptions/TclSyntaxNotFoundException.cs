using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IptSimulator.Client.Exceptions
{
    public class TclSyntaxNotFoundException : Exception
    {
        public TclSyntaxNotFoundException(string searchedPath) : base("Could not find TCL syntax file at path: " + searchedPath)
        {            
        }
    }
}
