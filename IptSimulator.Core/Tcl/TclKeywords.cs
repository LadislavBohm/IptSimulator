using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IptSimulator.Core.Tcl
{
    public class TclKeywords
    {
        public static IReadOnlyCollection<string> All { get; } = new[]
        {
            "after","append","array","auto_execok","auto_import","auto_load","auto_load_index","auto_qualify","binary",
            "bgerror","break","catch","cd","clock","close","concat","continue","dde","default","else","elseif","encoding",
            "eof","error","eval","exec","exit","expr","fblocked","fconfigure","fcopy","file","fileevent","flush","for",
            "foreach","format","gets","glob","global","history","if","incr","info","interp","join","lappend","lindex",
            "linsert","list","llength","load","lrange","lreplace","lsearch","lsort","namespace","open","package","pid",
            "pkg_mkIndex","proc","puts","pwd","read","regexp","regsub","rename","resource","return","scan","seek","set",
            "socket","source","split","string","subst","switch","tclLog","tell","time","trace","unknown","unset","update",
            "uplevel","upvar","variable","vwait","while"
        };
    }
}
