using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityBuilderCore
{
    /// <summary>
    /// building component that uses employees to influence the buildings efficiency
    /// </summary>
    public class EmploymentComponent : BuildingComponent, IEmployment, IEfficiencyFactor
    {
        public override string Key => "EMP";

        public PopulationEmployment[] PopulationEmployments;

        public bool IsFullyStaffed => Dependencies.Get<IGameSettings>().HasEmployment ? PopulationEmployments.All(p => p.IsFullyStaffed) : true;
        public float EmploymentRate => Dependencies.Get<IGameSettings>().HasEmployment ? PopulationEmployments.Select(p => p.Rate).Aggregate((a, b) => a * b) : 1f;

        public bool IsWorking => IsFullyStaffed;
        public float Factor => EmploymentRate;

        private void Start()
        {
            Dependencies.Get<IEmploymentManager>().AddEmployment(this);
        }
        private void OnDestroy()
        {
            Dependencies.Get<IEmploymentManager>().RemoveEmployment(this);
        }

        public int GetNeeded(EmploymentGroup employmentGroup, Population population)
        {
            return PopulationEmployments.Where(e => e.Group == employmentGroup && e.Population == population).Sum(e => e.Needed);
        }
        public virtual int AssignAvailable(EmploymentGroup employmentGroup, Population population, int quantity)
        {
            return PopulationEmployments.First(e => e.Group == employmentGroup && e.Population == population).AssignAvailable(quantity);
        }

        public IEnumerable<EmploymentGroup> GetEmploymentGroups() => PopulationEmployments.Select(p => p.Group);
        public IEnumerable<Population> GetPopulations() => PopulationEmployments.Select(p => p.Population);

        public override string GetDebugText() => $"EMP: {PopulationEmployments.Sum(p => p.Available)}/{PopulationEmployments.Sum(p => p.Needed)}";
        public override string GetDescription()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var population in PopulationEmployments)
            {
                sb.AppendLine($"{population.Population.Name}: {population.Available}/{population.Needed}");
            }

            return sb.ToString();
        }
    }
}