namespace SI
{
    internal interface IState
    {
        void Goto(State next);
    }
}
