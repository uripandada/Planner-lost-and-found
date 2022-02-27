using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Google.OrTools.Sat;

namespace Planner.CpsatCleaningCalculator
{
    public class ObjectiveContribution
    {
        public ObjectiveContribution()
        {
            contributions = new Dictionary<string, long>();
        }
        public string label { get; set; }
        public Dictionary<string, long> contributions { get; set; }
    }

    // static public Boolean WriteOutObjectiveComponents(
    //     string json_output,
    //     List<Attendant, ObjectiveComponents> contributions
    // )
    // {
    //     /// make a JSON string of the contributions

    //     return false;
    // }

    /// <summary> Gathering the elements that make up the objective
    /// function.  The purpose is to display overall objective, as
    /// well as the contributions from each attentant's assignments.
    /// </summary>
    public class ObjectiveComponents
    {
        ///<summary> Pass in things.  I hate this and it needs to be
        ///refactored.  A better approach would be to pass in a list of triples
        ///of thing name, thing variable, and thing weight in objfn.
        ///Right now I have a hand-edited connection between each
        ///element in the objective and each element here, which is
        ///brtittle and hard to maintain.
        ///
        ///Plus, if we make this more like a generic objective
        ///function builder, then we can improve the code in the
        ///CleaningPlannerCPSAT</summary>
        public ObjectiveComponents(
            long _floortravel,
            long _buildingtravel,
            long _credits,
            List<CleaningInterval> _cleaning_jobs,
            long _room_awards,
            long _epsilonCredits,
            long _epsilonCleaningRooms,
            long _stays_cleaned,
            long _departs_cleaned,
            long _epsilonCleaningStays,
            long _epsilonCleaningDeparts,
            long _attendant_literal,
            int _weight_minimize_attendants,
            int _weight_epsilon_credits,
            int _weight_epsilon_rooms,
            int _weight_epsilon_stay_depart,
            int _weight_travel_time,
            int _weight_credits,
            int _weight_rooms_cleaned,
            int _weight_building_award,
            int _weight_level_award,
            int _weight_room_award
            )
        {
            attendant = null;
            floortravel = _floortravel;
            buildingtravel = _buildingtravel;
            credits = _credits;
            cleaning_jobs = _cleaning_jobs;
            // building_awards = _building_awards;
            // level_awards = _level_awards;
            room_awards = _room_awards;
            epsilonCredits = _epsilonCredits;
            epsilonCleaningRooms = _epsilonCleaningRooms;
            attendant_literal = _attendant_literal;
            weight_minimize_attendants = _weight_minimize_attendants;
            weight_epsilon_credits = _weight_epsilon_credits;
            weight_epsilon_rooms = _weight_epsilon_rooms;
            weight_travel_time = _weight_travel_time;
            weight_credits = _weight_credits;
            weight_rooms_cleaned = _weight_rooms_cleaned;
            weight_building_award = _weight_building_award;
            weight_level_award = _weight_level_award;
            weight_room_award = _weight_room_award;
            stays_cleaned = _stays_cleaned;
            departs_cleaned = _departs_cleaned;
            epsilonCleaningStays = _epsilonCleaningStays;
            epsilonCleaningDeparts = _epsilonCleaningDeparts;
        }


        public Attendant attendant { get; set; }
        public long floortravel { get; }
        public long buildingtravel { get; }
        public long credits { get; }
        public List<CleaningInterval> cleaning_jobs { get; }
        // public long building_awards { get; }
        // public long level_awards { get; }
        public long room_awards { get; }
        public long epsilonCredits { get; }
        public long epsilonCleaningRooms { get; }
        public long stays_cleaned { get; }
        public long departs_cleaned { get; }
        public long epsilonCleaningStays { get; }
        public long epsilonCleaningDeparts { get; }
        public long attendant_literal { get; }
        public int weight_minimize_attendants { get; }
        public int weight_epsilon_credits { get; }
        public int weight_epsilon_rooms { get; }
        public int weight_epsilon_stay_depart { get; }
        public int weight_travel_time { get; }
        public int weight_credits { get; }
        public int weight_rooms_cleaned { get; }
        public int weight_building_award { get; }
        public int weight_level_award { get; }
        public int weight_room_award { get; }



        public ObjectiveContribution get_contributions_dictionary()
        {
            var result = new ObjectiveContribution();
            int global_result = 1;
            if (attendant != null)
            {
                global_result = 0;
                result.label = $"{attendant.Name} Contribution";
            }
            else
            {
                result.label = "Overall Objective";
            }

            result.contributions["attendant used"] = attendant_literal * weight_minimize_attendants;
            result.contributions["credit deviation"] = epsilonCredits * weight_epsilon_credits;  // * global_result;
            result.contributions["room deviation"] = epsilonCleaningRooms * weight_epsilon_rooms;  // * global_result;
            result.contributions["stays deviation"] = epsilonCleaningStays * weight_epsilon_stay_depart;  // * global_result;
            result.contributions["departures deviation"] = epsilonCleaningDeparts * weight_epsilon_stay_depart;  // * global_result;
            result.contributions["floor to floor travel"] = floortravel * weight_travel_time;
            result.contributions["building to building travel"] = buildingtravel * weight_travel_time;
            result.contributions["credits"] = credits * weight_credits;
            result.contributions["rooms cleaned"] = cleaning_jobs.Count() * weight_rooms_cleaned;
            result.contributions["preferred building award"] = 0;
            result.contributions["preferred level award"] = 0;
            result.contributions["preferred room award"] = room_awards * weight_room_award;
            return result;
        }
        public long get_total_contribution()
        {
            var result = get_contributions_dictionary();
            var sum = result.contributions.Sum(x => x.Value);
            return sum;
        }
    }
}
