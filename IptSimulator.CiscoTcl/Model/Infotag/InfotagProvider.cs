using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IptSimulator.CiscoTcl.Model.Infotag
{
    public class InfotagProvider
    {
        private static readonly IDictionary<string, InfotagKind> InfotagKindPrefixes = new Dictionary<string, InfotagKind>
        {
            { "leg", InfotagKind.Leg },
            { "cfg", InfotagKind.Configuration },
            { "con", InfotagKind.Connection},
            { "evt", InfotagKind.Event },
            { "med", InfotagKind.MediaServices },
            { "aaa", InfotagKind.Radius },
            { "sys", InfotagKind.System }
        };

        public static IDictionary<string, InfotagIdentifier> All { get; private set; }

        static InfotagProvider()
        {
            All = new Dictionary<string, InfotagIdentifier>
            {
                { "evt_dcdigits", CreateGlobal("evt_dcdigits") }
            };
        }

        private static InfotagIdentifier CreateGlobal(string name)
        {
            var prefix = name.Substring(0, name.IndexOf("_", StringComparison.Ordinal));
            return new InfotagIdentifier(name, InfotagKindPrefixes[prefix]);
        }
    }
}
