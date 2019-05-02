using System;

abstract class Option<T> {
  abstract public bool IsSome { get; }
  abstract public bool IsNone { get; }
  abstract public T Value { get; }
  abstract public T ValueOr(T defaultValue);
  abstract public T ValueOrElse(Lazy<T> defaultValue);
  abstract public Option<U> Map<U>(Func<T, U> f);
  abstract public Option<U> FlatMap<U>(Func<T, Option<U>> f);
  abstract public U MapOr<U>(U defaultValue, Func<T, U> f);
  abstract public U MapOrElse<U>(Lazy<U> defaultValue, Func<T, U> f);

  public static Option<A> From<A>(A value) {
    return value != null ? (Option<A>)(new Some<A>(value)) : (Option<A>)(new None<A>());
  }
}

class Some<T> : Option<T> {
  private readonly T value;

  public Some(T value) {
    if (value == null) {
      throw new InvalidOperationException("Cannot create a Some value from null.");
    }
    this.value = value;
  }

  public override bool IsSome {
    get { return true; }
  }

  public override bool IsNone {
    get { return false; }
  }

  public override T Value {
    get { return this.value; }
  }

  public override T ValueOr(T defaultValue) {
    return this.value;
  }

  public override T ValueOrElse(Lazy<T> defaultValue) {
    return this.value;
  }

  public override Option<U> Map<U>(Func<T, U> f) {
    return Option<U>.From(f(this.value));
  }

  public override Option<U> FlatMap<U>(Func<T, Option<U>> f) {
    return f(this.value);
  }

  public override U MapOr<U>(U defaultValue, Func<T, U> f) {
    return f(this.value);
  }

  public override U MapOrElse<U>(Lazy<U> defaultValue, Func<T, U> f) {
    return f(this.value);
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
    return this.value.Equals(((Some<T>)obj).value);
  }

  override public int GetHashCode() {
    return this.value.GetHashCode();
  }}

class None<T> : Option<T> {
  public None() {
  }

  public override bool IsSome {
    get { return false; }
  }

  public override bool IsNone {
    get { return true; }
  }

  public override T Value {
    get { throw new InvalidOperationException("Cannot get a Some value out of None."); }
  }

  public override T ValueOr(T defaultValue) {
    return defaultValue;
  }

  public override T ValueOrElse(Lazy<T> defaultValue) {
    return defaultValue.Value;
  }

  public override Option<U> Map<U>(Func<T, U> f) {
    return new None<U>();
  }

  public override Option<U> FlatMap<U>(Func<T, Option<U>> f) {
    return new None<U>();
  }

  public override U MapOr<U>(U defaultValue, Func<T, U> f) {
    return defaultValue;
  }

  public override U MapOrElse<U>(Lazy<U> defaultValue, Func<T, U> f) {
    return defaultValue.Value;
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
    return true;
  }

  override public int GetHashCode() {
    return 0;
  }}