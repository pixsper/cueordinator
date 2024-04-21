using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace Pixsper.Cueordinator.Models;

public interface IObservableValue<out T> : IObservable<T>
{
    T Value { get; }
}
internal class SourceValue<T> : IObservableValue<T>, IDisposable
{
    private readonly BehaviorSubject<T> _subject;
    private readonly IEqualityComparer<T> _equalityComparer;

    public SourceValue(T value, IEqualityComparer<T>? equalityComparer = null)
    {
        _subject = new BehaviorSubject<T>(value);
        _equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;
    }

    public void Dispose()
    {
        _subject.Dispose();
    }

    public T Value
    {
        get => _subject.Value;
        set
        {
            if (!_equalityComparer.Equals(_subject.Value, value))
                _subject.OnNext(value);
        }
    }

    public IDisposable Subscribe(IObserver<T> observer) => _subject.Subscribe(observer);

    public IObservableValue<T> AsObservableValue() => new ReadOnlyObservableValue(_subject);

    private class ReadOnlyObservableValue : IObservableValue<T>
    {
        private readonly BehaviorSubject<T> _subject;

        public ReadOnlyObservableValue(BehaviorSubject<T> subject)
        {
            _subject = subject;
        }

        public T Value => _subject.Value;

        public IDisposable Subscribe(IObserver<T> observer) => _subject.Subscribe();
    }
}