using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Planner.CpsatCleaningCalculator
{
    /// <summary>
    /// Collecting functions related to calculating room to room distances.
    /// Can probably be integrated directly into Model.Room when functions have stabilized.
    /// </summary>
    public class BuildingsMath
    {

        /// <summary> Given two Building Names, return time in minutes
        ///
        /// </summary>
        //public static Func<string, string, string,int> BuildingToBuildingDistance = (a,b,c) => 20;
        public static int BuildingToBuildingDistance (string a , string b, Distances c)
        {
            if (c == null)
            {
                // Log.GetLog().Debug("distance matrix is null");
                return 0;
            }

            //return 0;
            int distance= c.GetBuildingDistance(a, b);
            if (distance < 0)
                return distance * -2;
            // Log.GetLog().Debug($"distance from {a} to {b} is {distance}");
            return distance;

        }
    }
}
