namespace Events
{
    public interface IGameEventListener<T>
    {
        void OnEventInvoked(T _item);
    }
}