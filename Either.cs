using System;

abstract class Either<R, S> {
  abstract public bool IsLeft { get; }
  abstract public bool IsRight { get; }
  abstract public R LeftValue { get; }
  abstract public S RightValue { get; }
  abstract public R LeftOr(R dflt);
  abstract public R LeftOrElse(Lazy<R> dflt);
  abstract public S RightOr(S dflt);
  abstract public S RightOrElse(Lazy<S> dflt);
  abstract public T Match<T>(Func<R, T> f, Func<S, T> g);
  abstract public Either<U, S> MapLeft<U>(Func<R, U> f);
  abstract public Either<R, V> MapRight<V>(Func<S, V> g);
  abstract public Either<U, S> FlatMapLeft<U>(Func<R, Either<U, S>> f);
  abstract public Either<R, V> FlatMapRight<V>(Func<S, Either<R, V>> g);
}

class Left<R, S> : Either<R, S> {
  private R lvalue;

  public Left(R lvalue) {
    this.lvalue = lvalue;
  }
  override public bool IsLeft {
    get { return true; }
  }

  override public bool IsRight {
    get { return false; }
  }

  override public R LeftValue {
    get { return this.lvalue; }
  }

  override public S RightValue {
    get { throw new System.InvalidOperationException("Cannot get a Right value out of Left"); }
  }

  override public R LeftOr(R dflt) {
    return this.lvalue;
  }

  override public R LeftOrElse(Lazy<R> dflt) {
    return this.lvalue;
  }

  override public S RightOr(S dflt) {
    return dflt;
  }

  override public S RightOrElse(Lazy<S> dflt) {
    return dflt.Value;
  }

  override public T Match<T>(Func<R, T> f, Func<S, T> g) {
    return f(this.lvalue);
  }

  override public Either<U, S> MapLeft<U>(Func<R, U> f) {
    return new Left<U, S>(f(this.lvalue));
  }

  override public Either<R, V> MapRight<V>(Func<S, V> g) {
    return new Left<R, V>(this.lvalue);
  }

  override public Either<U, S> FlatMapLeft<U>(Func<R, Either<U, S>> f) {
    return f(this.lvalue);
  }

  override public Either<R, V> FlatMapRight<V>(Func<S, Either<R, V>> g) {
    return new Left<R, V>(this.lvalue);
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
    return this.lvalue.Equals(((Left<R, S>)obj).lvalue);
  }

  override public int GetHashCode() {
    return this.lvalue.GetHashCode() << 1;
  }
}

class Right<R, S> : Either<R, S> {
  private S rvalue;

  public Right(S rvalue) { this.rvalue = rvalue; }
  override public bool IsLeft { get { return false; } }
  override public bool IsRight { get { return true; } }
  override public R LeftValue {
    get { throw new InvalidOperationException("Cannot get a Left value out of Right"); }
  }
  override public S RightValue { get { return this.rvalue; } }

  override public R LeftOr(R dflt) {
    return dflt;
  }

  override public R LeftOrElse(Lazy<R> dflt) {
    return dflt.Value;
  }

  override public S RightOr(S dflt) {
    return this.rvalue;
  }

  override public S RightOrElse(Lazy<S> dflt) {
    return this.rvalue;
  }
  override public T Match<T>(Func<R, T> f, Func<S, T> g) {
    return g(this.rvalue);
  }

  override public Either<U, S> MapLeft<U>(Func<R, U> f) {
    return new Right<U, S>(this.rvalue);
  }

  override public Either<R, V> MapRight<V>(Func<S, V> g) {
    return new Right<R, V>(g(this.rvalue));
  }

  override public Either<U, S> FlatMapLeft<U>(Func<R, Either<U, S>> f) {
    return new Right<U, S>(this.rvalue);
  }

  override public Either<R, V> FlatMapRight<V>(Func<S, Either<R, V>> g) {
    return g(this.rvalue);
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
    return this.rvalue.Equals(((Right<R, S>)obj).rvalue);
  }

  override public int GetHashCode() {
    return 1 + this.rvalue.GetHashCode() << 1;
  }
}