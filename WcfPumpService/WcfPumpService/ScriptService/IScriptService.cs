namespace WcfPumpService.ScriptService
{
    public interface IScriptService
    {
        bool Compile();
        void Run(int count);
    }
}
