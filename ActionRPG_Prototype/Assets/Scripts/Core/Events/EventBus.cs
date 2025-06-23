using System;
using UniRx;

namespace Core.Events
{
    public interface IEventBus
    {
        void Publish<T>(T message);
        IObservable<T> Receive<T>();
    }

    public class EventBus : IEventBus
    {
        private readonly MessageBroker _broker = new();

        public void Publish<T>(T message)   => _broker.Publish(message);
        public IObservable<T> Receive<T>()  => _broker.Receive<T>();
    }
}