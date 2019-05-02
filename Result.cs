using System;

public abstract class Result<R, S> {
  abstract public bool IsOk { get; }
  abstract public bool IsErr { get; }
  abstract public R Value { get; }
  abstract public S Error { get; }
  abstract public R ValueOr(R dflt);
  abstract public R ValueOrElse(Func<S, R> op);
  abstract public Result<U, S> Map<U>(Func<R, U> f);
  abstract public Result<R, V> MapError<V>(Func<S, V> g);
  abstract public U MapOrElse<U>(Func<R, U> f, Func<S, U> g);
}

public class Ok<R, S> : Result<R, S> {
  private readonly R okValue;

  public Ok(R value) {
    this.okValue = value;
  }

  public override bool IsOk {
    get { return true; }
  }

  public override bool IsErr {
    get { return false; }
  }

  public override R Value {
    get { return this.okValue; }
  }

  public override S Error {
    get { throw new InvalidOperationException("Cannot unwrap error value from Ok."); }
  }

  public override R ValueOr(R dflt) {
    return this.okValue;
  }

  public override R ValueOrElse(Func<S, R> op) {
    return this.okValue;
  }

  public override Result<U, S> Map<U>(Func<R, U> f) {
    return new Ok<U, S>(f(this.okValue));
  }

  public override Result<R, V> MapError<V>(Func<S, V> f) {
    return new Ok<R, V>(this.okValue);
  }

  public override U MapOrElse<U>(Func<R, U> f, Func<S, U> g) {
    return f(this.okValue);
  }

  override public bool Equals(object obj) {
    if (object.ReferenceEquals(obj, null)) {
      return false;
    }
    if (object.ReferenceEquals(this, obj)) {
      return true;
    }
    if (this.GetType() != obj.GetType()) {
      return false;
    }
    return this.okValue.Equals(((Ok<R, S>)obj).okValue);
  }

  override public int GetHashCode() {
    return this.okValue.GetHashCode() << 1;
  }
}

public class Err<R, S> : Result<R, S> {
  private readonly S errValue;

  public Err(S error) {
    this.errValue = error;
  }

  public override bool IsOk {
    get { return false; }
  }

  public override bool IsErr {
    get { return true; }
  }

  public override R Value {
    get { throw new InvalidOperationException("Cannot get an Ok value from an Err."); }
  }

  public override S Error {
    get { return this.errValue; }
  }

  public override R ValueOr(R dflt) {
    return dflt;
  }

  public override R ValueOrElse(Func<S, R> op) {
    return op(this.errValue);
  }

  public override Result<U, S> Map<U>(Func<R, U> f) {
    return new Err<U, S>(this.errValue);
  }

  public override Result<R, V> MapError<V>(Func<S, V> g) {
    return new Err<R, V>(g(this.errValue));
  }

  public override U MapOrElse<U>(Func<R, U> f, Func<S, U> g) {
    return g(this.errValue);
  }

  override public bool Equals(object obj) {
    if (object.ReferenceEquals(obj, null)) {
      return false;
    }
    if (object.ReferenceEquals(this, obj)) {
      return true;
    }
    if (this.GetType() != obj.GetType()) {
      return false;
    }
    return this.errValue.Equals(((Err<R, S>)obj).errValue);
  }

  override public int GetHashCode() {
    return 1 + this.errValue.GetHashCode() << 1;
  }
}