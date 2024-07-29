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

        [XmlElement("cursedEnergyMaintainCost")]
        public float CursedEnergyMaintainCost = 5f;


        [XmlIgnore]
        public HediffDef HediffDef => !string.IsNullOrEmpty(HediffDefName) ? DefDatabase<HediffDef>.GetNamed(HediffDefName) : null;

        [XmlIgnore]
        public BodyPartDef BodyPartDef => !string.IsNullOrEmpty(BodyPartDefName) ? DefDatabase<BodyPartDef>.GetNamed(BodyPartDefName) : null;
    }
}