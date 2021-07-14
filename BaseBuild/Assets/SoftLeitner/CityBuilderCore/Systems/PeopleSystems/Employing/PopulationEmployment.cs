using System;
using UnityEngine;

namespace CityBuilderCore
{
    [Serializable]
    public class PopulationEmployment
    {
        public EmploymentGroup Group;
        public Population Population;
        public int Needed;
        public int Available { get; private set; }

        public float Rate => Needed == 0 ? 1f : Available / (float)Needed;
        public bool IsFullyStaffed => Available >= Needed;

        /// <summary>
        /// assigns the quantity needed and returns the rest
        /// </summary>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public int AssignAvailable(int quantity)
        {
            int assigned = Mathf.Min(quantity, Needed);
            Available = assigned;
            return quantity - assigned;
        }
    }
}