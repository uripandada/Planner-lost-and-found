using System;
using Google.OrTools.Sat;

namespace Planner.CpsatCleaningCalculator
{
    // [START print_solution]
    public class VarArraySolutionPrinterWithObjective : CpSolverSolutionCallback
    {
        public VarArraySolutionPrinterWithObjective(IntVar[] variables, EventHandler<CleaningPlannerCPSAT.ProgressMessage> handler, TimeZoneInfo timeZoneInfo)
        {
            variables_ = variables;
            handler_ = handler;
            timeZoneInfo_ = timeZoneInfo;
        }

        public override void OnSolutionCallback()
        {

            var message = $"Solution #{solution_count_}: time = {WallTime():F2} s\n";
            message += $"  objective value = {ObjectiveValue()}\n";
            foreach (IntVar v in variables_)
            {
                message += $"  {v.ShortString()} = {Value(v)}\n</br>";
            }
            solution_count_++;
            Console.WriteLine(message);
            handler_?.Invoke(this, new CleaningPlannerCPSAT.ProgressMessage
            {
                CleaningPlanId = Guid.Empty,
                Message = message,
                StatusKey = CpsatProgressStatus.SOLVING.ToString(),
                DateTimeString = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo_).ToString("yyyy-MM-dd HH:mm"),
            });
        }

        public int SolutionCount()
        {
            return solution_count_;
        }

        private int solution_count_;
        private IntVar[] variables_;
        private EventHandler<CleaningPlannerCPSAT.ProgressMessage> handler_;
        private TimeZoneInfo timeZoneInfo_;
    }
    // [END print_solution
}
