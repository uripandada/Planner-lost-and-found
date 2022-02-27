using System;
using System.Collections.Generic;
using System.Text;

namespace Planner.Domain.Values
{
    public static class ClaimsKeys
    {
        public const string ISSUER = "Planner";

        public const string GivenName = "given_name";
        public const string FamilyName = "family_name";
        public const string HotelId = "hotel_id";
        public const string HotelGroupId = "hotel_group_id";


        public static class SettingsClaimKeys
        {
            public const string Rooms = "rooms";
            public const string Assets = "assets";
            public const string Users = "users";
            public const string RoleManagement = "role_management";
            public const string RoomCategories = "room_categories";
            public const string HotelSettings = "hotel_settings";
            public const string Colors = "colors";
        }

        public static class ManagementClaimKeys
        {
            public const string RoomInsights = "room_insights";
            public const string UserInsights = "user_insights";
            public const string Tasks = "tasks";
            public const string Reservations = "reservations";
            public const string CleaningPlanner = "cleaning_planner";
            public const string CleaningCalendar = "cleaning_calendar";
            public const string ReservationCalendar = "reservation_calendar";
            public const string LostAndFound = "lost_and_found";
            public const string OnGuard = "on_guard";
            //public const string OnGuard = "on_guard";
        }
    }
}
