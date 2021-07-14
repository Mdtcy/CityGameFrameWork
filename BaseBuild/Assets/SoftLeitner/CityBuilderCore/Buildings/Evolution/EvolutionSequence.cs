using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// sequence of building evolution stages including descriptions and the logic to determine the current stage<br/>
    /// </summary>
    [CreateAssetMenu(menuName = "CityBuilder/" + nameof(EvolutionSequence))]
    public class EvolutionSequence : ScriptableObject
    {
        [Tooltip("conditions for lowest stage should be left empty, conditions do not have to be repeated in higher ones")]
        public EvolutionStage[] Stages;
        [Tooltip("description displayed when a building has achieved the highest possible stage")]
        public string EvolvedDescription;
        [Tooltip("description displayed during the evolution delay")]
        public string EvolutionDescription;
        [Tooltip("default description when the building can still evolve, placeholder({0}) for the missing conditions for the next stage")]
        public string Description;
        [Tooltip("description displayed during the devolution delay, placeholder({0}) for the failed conditions")]
        public string DevolutionDescription;

        public EvolutionStage GetStage(Vector2Int position, Service[] services, Item[] items)
        {
            EvolutionStage passedStage = null;
            foreach (var stage in Stages)
            {
                if (stage.Check(position, services, items))
                    passedStage = stage;
                else
                    return passedStage;
            }
            return passedStage;
        }
        public bool GetDirection(BuildingInfo current, BuildingInfo evolution)
        {
            EvolutionStage currentStage = Stages.First(s => s.BuildingInfo == current);
            EvolutionStage evolutionStage = Stages.First(s => s.BuildingInfo == evolution);

            return Array.IndexOf(Stages, evolutionStage) > Array.IndexOf(Stages, currentStage);
        }
        public string GetDescription(BuildingInfo current, BuildingInfo evolution, Vector2Int position, IEnumerable<Service> services, IEnumerable<Item> items)
        {
            EvolutionStage currentStage = Stages.First(s => s.BuildingInfo == current);

            if (evolution == null)
            {
                int index = Array.IndexOf(Stages, currentStage);

                if (index == Stages.Length - 1)
                {
                    return EvolvedDescription;
                }
                else
                {
                    return string.Format(Description, string.Join(", ", getMissing(Stages[index + 1], position, services.ToArray(), items.ToArray())));
                }
            }
            else
            {
                EvolutionStage evolutionStage = Stages.First(s => s.BuildingInfo == evolution);

                if (Array.IndexOf(Stages, evolutionStage) > Array.IndexOf(Stages, currentStage))
                {
                    return string.Format(EvolutionDescription, string.Join(", ", getMissing(evolutionStage, position, services.ToArray(), items.ToArray())));
                }
                else
                {
                    return string.Format(DevolutionDescription, string.Join(", ", getMissing(currentStage, position, services.ToArray(), items.ToArray())));
                }
            }
        }

        private IEnumerable<string> getMissing(EvolutionStage stage, Vector2Int position, Service[] services, Item[] items)
        {
            List<string> missing = new List<string>();

            foreach (var serviceRequirement in stage.Services)
            {
                if (!services.Contains(serviceRequirement))
                    missing.Add(serviceRequirement.Name);
            }
            foreach (var itemRequirement in stage.Items)
            {
                if (!items.Contains(itemRequirement))
                    missing.Add(itemRequirement.Name);
            }
            foreach (var itemCategoryRequirement in stage.ItemCategoryRequirements)
            {
                if (!itemCategoryRequirement.IsFulfilled(items))
                    missing.Add(itemCategoryRequirement.ItemCategory.GetName(itemCategoryRequirement.Quantity));
            }
            foreach (var layerRequirement in stage.LayerRequirements)
            {
                if (!layerRequirement.IsFulfilled(position))
                    missing.Add(layerRequirement.Layer.Name);
            }

            return missing;
        }
    }
}