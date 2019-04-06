namespace Game1
{
    internal interface IState
    {
        void Goto(State next);
    }
}
