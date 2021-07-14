using CityBuilderCore;
using UnityEngine;

namespace CityBuilderDefense
{
    public class ThreeManager : MonoBehaviour
    {
        void Update()
        {
            var populationManager = Dependencies.Get<IPopulationManager>();
            var employmentManager = Dependencies.Get<IEmploymentManager>();

            foreach (var population in Dependencies.Get<IObjectSet<Population>>().Objects)
            {
                var migration = populationManager.GetMigration(population);

                var populationQuantity = populationManager.GetQuantity(migration.Population, true);
                var employmentQuantity = Mathf.Max(employmentManager.GetNeeded(migration.Population), 20);//20 people always stay even if they are unemployed
                var difference = populationQuantity - employmentQuantity;

                if (difference == 0 || (difference > 0 && difference < 10))
                    migration.Sentiment = 0f;//close enough
                else
                    migration.Sentiment = -((float)populationQuantity / (float)employmentQuantity - 1f);//simply balance population for full employment
            }

        }
    }
}