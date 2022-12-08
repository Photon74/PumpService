namespace PumpService.Interfaces
{
    public interface IStatisticsService
    {
        int SuccessTacts { get; set; }

        int ErrorTacts { get; set; }

        int AllTacts { get; set; }
    }
}
