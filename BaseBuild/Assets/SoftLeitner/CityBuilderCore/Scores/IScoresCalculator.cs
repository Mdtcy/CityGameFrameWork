using System;

namespace CityBuilderCore
{
    public interface IScoresCalculator
    {
        event Action Calculated;
        int GetValue(Score score);
    }
}