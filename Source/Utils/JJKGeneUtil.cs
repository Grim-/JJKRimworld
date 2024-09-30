using System.Collections.Generic;
using System.Linq;
using Verse;

namespace JJK
{
    public static class JJKGeneUtil
    {
        public static void GiveCursedEnergy(Pawn targetPawn)
        {
            if (targetPawn == null || targetPawn.genes == null) return;

            if (!targetPawn.genes.HasActiveGene(JJKDefOf.Gene_JJKCursedEnergy))
            {
                targetPawn.genes?.AddGene(JJKDefOf.Gene_JJKCursedEnergy, true);
            }
        }
        public static void RemoveCursedEnergy(Pawn targetPawn)
        {
            if (targetPawn == null || targetPawn.genes == null) return;

            if (targetPawn.genes.HasActiveGene(JJKDefOf.Gene_JJKCursedEnergy))
            {
                targetPawn.genes?.RemoveGene(targetPawn.genes.GetGene(JJKDefOf.Gene_JJKCursedEnergy));
            }
        }

        public static void TransferCursedEnergyGenes(Pawn sourcePawn, Pawn targetPawn)
        {
            var sourceGenes = sourcePawn.genes.GenesListForReading
                .Where(g => g.def.HasModExtension<CursedEnergyGeneExtension>())
                .ToList();

            foreach (Gene sourceGene in sourceGenes)
            {
                var extension = sourceGene.def.GetModExtension<CursedEnergyGeneExtension>();
                Gene existingGene = targetPawn.genes.GetGene(sourceGene.def);

                if (existingGene == null)
                {
                    targetPawn.genes.AddGene(sourceGene.def, true);
                }
                else
                {
                    var existingExtension = existingGene.def.GetModExtension<CursedEnergyGeneExtension>();
                    if (extension.priority > existingExtension.priority)
                    {
                        targetPawn.genes.RemoveGene(existingGene);
                        targetPawn.genes.AddGene(sourceGene.def, true);
                    }
                }
            }
        }

        public static void ForceSorcererGradeGene(Pawn targetPawn, GeneDef GeneToForce)
        {
            CursedEnergyGeneExtension geneToForceExtension = GeneToForce.GetModExtension<CursedEnergyGeneExtension>();
            if (geneToForceExtension == null)
            {
                Log.Message($"{GeneToForce} has no CursedEnergyGeneExtension.");
                return;
            }

            var allSorcererGenes = targetPawn.genes.GenesListForReading
                .Where(g => g.def.HasModExtension<CursedEnergyGeneExtension>())
                .ToList();

            foreach (Gene sourceGene in allSorcererGenes)
            {
                targetPawn.genes.RemoveGene(sourceGene);
            }

            targetPawn.genes.AddGene(GeneToForce, true);
        }

        public static void GiveRandomCursedTechnique(this Pawn Pawn)
        {


            List<GeneDef> sorcererGeneDefs = DefDatabase<GeneDef>.AllDefs
                 .Where(geneDef => geneDef.HasModExtension<CursedTechniqueGeneExtension>())
                 .ToList();

            if (sorcererGeneDefs.Count > 0)
            {
                GeneDef chosen = sorcererGeneDefs.RandomElement();
                if (chosen != null && !Pawn.genes.HasActiveGene(chosen))
                {
                    Pawn.genes.AddGene(chosen, true);
                }
            }
        }

        public static void GiveRandomSorcererGrade(Pawn targetPawn, bool overwriteExistingGrade = true)
        {
            List<GeneDef> sorcererGeneDefs = DefDatabase<GeneDef>.AllDefs
                .Where(geneDef => geneDef.HasModExtension<CursedEnergyGeneExtension>())
                .ToList();

            List<Gene> existingSorcererGenes = targetPawn.GetSorcererGradeGenes();


            if (sorcererGeneDefs.Count == 0)
            {
                Log.Warning("No GeneDefs found with CursedEnergyGeneExtension.");
                return;
            }


            if (existingSorcererGenes.Count > 0)
            {
                if (overwriteExistingGrade)
                {
                    foreach (Gene sourceGene in existingSorcererGenes)
                    {
                        targetPawn.genes.RemoveGene(sourceGene);
                    }
                }

            }
            GeneDef randomSorcererGeneDef = sorcererGeneDefs.RandomElement();
            targetPawn.genes.AddGene(randomSorcererGeneDef, true);
        }
        public static void RemoveAllTechniqueGenes(this Pawn Pawn)
        {
            List<Gene> FoundGenes = GetCursedTechniqueGenes(Pawn);

            foreach (var item in FoundGenes)
            {
                if (Pawn.genes.HasActiveGene(item.def))
                {
                    Pawn.genes.RemoveGene(item);
                }
            }
        }
        public static void RemoveAllGradeGenes(this Pawn Pawn)
        {
            List<Gene> FoundGenes = GetSorcererGradeGenes(Pawn);

            foreach (var item in FoundGenes)
            {
                if (Pawn.genes.HasActiveGene(item.def))
                {
                    Pawn.genes.RemoveGene(item);
                }
            }
        }

        public static List<Gene> GetSorcererGradeGenes(this Pawn Pawn)
        {
            if (Pawn.genes == null)
            {
                return null;
            }
            return Pawn.genes?.GenesListForReading
                    .Where(g => g.def.HasModExtension<CursedEnergyGeneExtension>())
                    .ToList();
        }
        public static Gene GetSorcererGradeGene(this Pawn Pawn)
        {
            if (Pawn.genes == null)
            {
                return null;
            }
            return Pawn.genes?.GenesListForReading
                    .Where(g => g.def.HasModExtension<CursedEnergyGeneExtension>())
                    .FirstOrDefault();
        }

        public static void UpgradeSorcererGrade(this Pawn targetPawn)
        {
            // Get all sorcerer grade gene definitions
            List<GeneDef> sorcererGeneDefs = DefDatabase<GeneDef>.AllDefs
                .Where(geneDef => geneDef.HasModExtension<CursedEnergyGeneExtension>())
                .OrderByDescending(geneDef => geneDef.GetModExtension<CursedEnergyGeneExtension>().priority)
                .ToList();

            if (sorcererGeneDefs.Count == 0)
            {
                Log.Warning("No GeneDefs found with CursedEnergyGeneExtension.");
                return;
            }

            // Get the pawn's current sorcerer grade gene
            Gene currentGradeGene = targetPawn.GetSorcererGradeGene();

            if (currentGradeGene == null)
            {
                GeneDef randomSorcererGeneDef = sorcererGeneDefs.RandomElement();
                targetPawn.genes.AddGene(randomSorcererGeneDef, true);
                Log.Message($"Gave {targetPawn.Name} a random sorcerer grade: {randomSorcererGeneDef.defName}");
                return;
            }

            int currentIndex = sorcererGeneDefs.FindIndex(def => def == currentGradeGene.def);

            if (currentIndex == -1)
            {
                Log.Error($"Current grade gene {currentGradeGene.def.defName} not found in the list of sorcerer grade genes.");
                return;
            }

            if (currentIndex > 0)
            {
                GeneDef nextGradeDef = sorcererGeneDefs[currentIndex - 1];
                targetPawn.genes.RemoveGene(currentGradeGene);
                targetPawn.genes.AddGene(nextGradeDef, true);

                Log.Message($"Upgraded {targetPawn.Name}'s sorcerer grade from {currentGradeGene.def.defName} to {nextGradeDef.defName}");
            }
        }

        public static bool HasGradeGene(this Pawn Pawn)
        {
            List<Gene> genes = GetSorcererGradeGenes(Pawn);
            return genes != null && genes.Count > 0;
        }

        public static void TransferGenes(Pawn sourcePawn, Pawn targetPawn, GeneDef GeneDefToTransfer, bool IsXenogerm = true)
        {
            if (sourcePawn == null || targetPawn == null || sourcePawn.genes == null || targetPawn.genes == null) return;

            if (sourcePawn.genes.HasActiveGene(GeneDefToTransfer))
            {
                if (!targetPawn.genes.HasActiveGene(GeneDefToTransfer))
                {
                    Gene SourceGene = sourcePawn.genes.GetGene(GeneDefToTransfer);
                    sourcePawn.genes.RemoveGene(SourceGene);
                    targetPawn.genes.AddGene(GeneDefToTransfer, IsXenogerm);

                }
            }
        }
        public static bool HasCursedTechnique(this Pawn Pawn)
        {
            if (Pawn.genes == null)
            {
                return false;
            }

            return Pawn.genes.GenesListForReading
                    .Where(g => g.def.HasModExtension<CursedTechniqueGeneExtension>()).Count() > 0;
        }

        public static Gene GetCursedTechniqueGene(this Pawn Pawn)
        {
            return Pawn.genes.GenesListForReading
                    .Where(g => g.def.HasModExtension<CursedTechniqueGeneExtension>()).FirstOrDefault();
        }
        public static List<Gene> GetCursedTechniqueGenes(this Pawn Pawn)
        {
            return Pawn.genes.GenesListForReading
                    .Where(g => g.def.HasModExtension<CursedTechniqueGeneExtension>()).ToList();
        }
        public static void TryUpgradeSorcererGrade(Pawn targetPawn)
        {
            List<GeneDef> sorcererGeneDefs = DefDatabase<GeneDef>.AllDefs
                .Where(geneDef => geneDef.HasModExtension<CursedEnergyGeneExtension>())
                .OrderBy(x => x.GetModExtension<CursedEnergyGeneExtension>().priority)
                .ToList();

            if (sorcererGeneDefs.Count == 0)
            {
                Log.Warning("No GeneDefs found with CursedEnergyGeneExtension.");
                return;
            }

            // Find the pawn's current sorcerer gene
            Gene currentSorcererGene = targetPawn.genes.GenesListForReading
                .FirstOrDefault(g => g.def.HasModExtension<CursedEnergyGeneExtension>());

            if (currentSorcererGene == null)
            {
                // If no gene exists, add the lowest grade gene (lowest priority number)
                GeneDef lowestGradeGene = sorcererGeneDefs.First();
                targetPawn.genes.AddGene(lowestGradeGene, true);
                Log.Message($"Added initial sorcerer gene {lowestGradeGene.defName} to {targetPawn.Name}");
                return;
            }

            int currentPriority = currentSorcererGene.def.GetModExtension<CursedEnergyGeneExtension>().priority;

            // Find the next higher grade gene (higher priority number)
            GeneDef upgradedGeneDef = sorcererGeneDefs
                .FirstOrDefault(geneDef => geneDef.GetModExtension<CursedEnergyGeneExtension>().priority > currentPriority);

            if (upgradedGeneDef != null)
            {
                // Remove the current gene
                targetPawn.genes.RemoveGene(currentSorcererGene);

                // Add the upgraded gene
                targetPawn.genes.AddGene(upgradedGeneDef, true);

                Log.Message($"Upgraded {targetPawn.Name}'s sorcerer grade from {currentSorcererGene.def.defName} to {upgradedGeneDef.defName}");
            }
            else
            {
                Log.Message($"{targetPawn.Name} already has the highest sorcerer grade available.");
            }
        }

    }
}

