using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IptSimulator.Core.Tcl
{
    public class TclReservedVariables
    {
        public static IReadOnlyCollection<string> All { get; } = new[]
        {
            "tcl_version","tcl_patchLevel","errorCode","errorInfo","tcl_precision","argv0","tcl_rcFileName","tcl_library",
            "tcl_nonwordchars","tcl_wordchars","auto_source_path","env","eagle_shell","eagle_tests","eagle_debugger",
            "eagle_paths","auto_index","tcl_platform","eagle_platform","tcl_interactive","tcl_interactiveLoops","null"
        };
    }
}
