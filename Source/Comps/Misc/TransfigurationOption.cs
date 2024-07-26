using System.Xml.Serialization;
using Verse;

namespace JJK
{
    public class TransfigurationOption
    {
        public string OptionLabel = "SET LABEL";

        [XmlElement("hediffDef")]
        public string HediffDefName;

        [XmlElement("bodyPartDef")]
        public string BodyPartDefName;

        [XmlIgnore]
        public HediffDef HediffDef => DefDatabase<HediffDef>.GetNamed(HediffDefName);

        [XmlIgnore]
        public BodyPartDef BodyPartDef => DefDatabase<BodyPartDef>.GetNamed(BodyPartDefName);
    }
}