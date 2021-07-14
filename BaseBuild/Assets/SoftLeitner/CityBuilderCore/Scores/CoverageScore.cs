﻿using System;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// percentage of how much of the population is covered by certain buildings
    /// example: 1 temple covers 1x100 | 2 shrines cover 2x50 | population=400 => 50% coverage
    /// </summary>
    [CreateAssetMenu(menuName = "CityBuilder/Scores/" + nameof(CoverageScore))]
    public class CoverageScore : Score
    {
        public BuildingEvaluation[] Evaluations;
        public Population[] Populations;

        public override int Calculate()
        {
            int valueTotal = Evaluations.Sum(i => i.GetEvaluation());
            int populationTotal = Populations.Sum(p => Dependencies.Get<IPopulationManager>().GetQuantity(p));

            if (populationTotal == 0)
                return 0;

            return Math.Min(100, valueTotal * 100 / populationTotal);
        }
    }
}