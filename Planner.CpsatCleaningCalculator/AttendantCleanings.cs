using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using Google.OrTools.Sat;

namespace Planner.CpsatCleaningCalculator
{
    public class AttendantCleanings
    {

        public AttendantCleanings(Attendant a, int maxCleanings_, int maxCredits_)
        {
            attendant = a;
            maxCleanings = maxCleanings_;
            maxCredits = maxCredits_;
        }

        public int getOptionalCleanings()
        {
            return optionalCleanings_;
        }

        public int addOptionalCleanings(int count)
        {
            optionalCleanings_ += count;
            return optionalCleanings_;
        }

        public int getOptionalPriorityCleanings()
        {
            return optionalPriorityCleanings_;
        }

        public int addOptionalPriorityCleanings(int count)
        {
            optionalPriorityCleanings_ += count;
            return optionalPriorityCleanings_;
        }

        public int getAssignedCleanings()
        {
            return assignedCleanings_;
        }

        public int addAssignedCleanings(int count)
        {
            assignedCleanings_ += count;
            return assignedCleanings_;
        }

        public void setFullyAssigned()
        {
            attendantFullyAssigned_ = true;
        }

        public Boolean isFullyAssigned(Boolean firstPass = true)
        {
            if (attendantFullyAssigned_)
            {
                return true;
            }
            if (assignedCredits_ >= maxCredits)
            {
                 return true;
            }

            if (assignedCleanings_ >= maxCleanings)
            {
                 return true;
            }
            if (firstPass)
            {
                if (optionalCleanings_ >= multiplier_ * maxCleanings)
                {
                    return true;
                }

                if (optionalPriorityCleanings_ >= multiplier_ * maxCleanings)
                {
                    return true;
                }
            }
            else
            {
                if (optionalCleanings_ >= 2 * multiplier_ * maxCleanings)
                {
                    return true;
                }

                if (optionalPriorityCleanings_ >= 2 * multiplier_ * maxCleanings)
                {
                    return true;
                }
            }
            return false;
        }

        public Boolean isOptionalPriorityFullyAssigned()
        {
            if (optionalPriorityCleanings_ > multiplier_ * maxCleanings)
            {
                return true;
            }
            return false;
        }

        public Attendant attendant { get; }
        public int maxCleanings { get; }
        public int maxCredits { get; }
        private int optionalCleanings_;
        private int optionalPriorityCleanings_;
        // private int optionalCredits_;
        private int assignedCleanings_;
        private int assignedCredits_;
        private Boolean attendantFullyAssigned_ = false;
        private double multiplier_ = 4;

    }
}
