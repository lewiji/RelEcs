namespace RelEcs
{
    public interface ISystem
    {
        World World { get; set; }
        void Run();
    }
}