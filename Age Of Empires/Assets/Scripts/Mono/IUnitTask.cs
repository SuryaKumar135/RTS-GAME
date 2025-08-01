namespace RTSGame
{
    public interface IUnitTask
    {
        /// <summary>Called once to begin the task.</summary>
        void Start(UnitBehaviour unit);

        /// <summary>Called to interrupt/stop the task.</summary>
        void Stop();
    }
}
