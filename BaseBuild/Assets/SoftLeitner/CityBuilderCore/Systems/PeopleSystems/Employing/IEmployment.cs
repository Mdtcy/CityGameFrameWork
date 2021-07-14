using System.Collections.Generic;

namespace CityBuilderCore
{
    /// <summary>
    /// a building component that uses employees
    /// </summary>
    public interface IEmployment : IBuildingComponent
    {
        /// <summary>
        /// all needed employees are assigned
        /// </summary>
        bool IsFullyStaffed { get; }
        /// <summary>
        /// the rate of employees assigned from 0 to 1
        /// </summary>
        float EmploymentRate { get; }

        IEnumerable<EmploymentGroup> GetEmploymentGroups();
        IEnumerable<Population> GetPopulations();

        int GetNeeded(EmploymentGroup employmentGroup, Population population);
        /// <summary>
        /// assigns the quantity needed and returns the rest
        /// </summary>
        /// <param name="employmentGroup"></param>
        /// <param name="population"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        int AssignAvailable(EmploymentGroup employmentGroup, Population population, int quantity);
    }
}