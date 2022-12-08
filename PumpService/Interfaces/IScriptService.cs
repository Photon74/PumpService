namespace PumpService.Interfaces
{
    public interface IScriptService
    {
        bool Compile();
        void Run(int count);
    }
}
