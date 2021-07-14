namespace CityBuilderCore
{
    /// <summary>
    /// manages the employment system
    /// <para>
    /// distributes the different populations to their appropriate workplaces<br/>
    /// when workers are missing, employees are assigned by employment group priority<br/>
    /// this priority can be changed at runtime
    /// </para>
    /// </summary>
    public interface IEmploymentManager
    {
        void AddEmployment(IEmployment employment);
        void RemoveEmployment(IEmployment employment);

        void CheckEmployment();

        int GetAvailable(Population population, EmploymentGroup group = null);
        int GetEmployed(Population population, EmploymentGroup group = null);
        int GetNeeded(Population population, EmploymentGroup group = null);
        float GetEmploymentRate(Population population);

        int GetPriority(EmploymentGroup group);
        void SetPriority(EmploymentGroup group, int priority);

        string SaveData();
        void LoadData(string json);
    }
}