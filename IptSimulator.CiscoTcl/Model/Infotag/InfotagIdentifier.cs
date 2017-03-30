namespace IptSimulator.CiscoTcl.Model.Infotag
{
    public class InfotagIdentifier
    {
        public string Identifier { get; private set; }
        public InfotagKind Kind { get; private set; }
        public string Description { get; private set; }

        public InfotagIdentifier(string identifier, InfotagKind kind)
        {
            Identifier = identifier;
            Kind = kind;
        }

        public InfotagIdentifier(string identifier, InfotagKind kind, string description)
        {
            Identifier = identifier;
            Kind = kind;
            Description = description;
        }
    }
}
