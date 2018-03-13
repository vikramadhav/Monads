# Result Monads

Implementation is influened by [Mikhail Blog](https://mikhail.io/2016/01/monads-explained-in-csharp/")

## What is a Monad

>What a Monad is. Monads have the reputation of being something very abstract and very confusing for every developer who is not a hipster Haskell programmer. They say that once you understand what a monad is, you loose the ability to explain it in simple language. Doug Crockford was the first one to lay this rule down, but it becomes kind of obvious once you read 3 or 5 explanations on the web

__Monads are container types__

Monads represent a class of types which behave in the common way.

Monads are containers which encapsulate some kind of functionality. On top of that, they provide a way to combine two containers into one. And that's about it.

The goals of monads are similar to generic goals of any encapsulation in software development practices: hide the implementation details from the client, but provide a proper way to use the hidden functionality.

It's not because we want to be able to change the implementation, it's because we want to make the client as simple as possible and to enforce the best way of code structure. Quite often monads provide the way to avoid imperative code in favor of functional style.

Monads are flexible, so in C# we could try to represent a monadic type as a generic struct:

```C#
public struct Result<T> : IResult
{
  private Result(Error error)
    {
         _common = new ResultCommon(error.ToMaybe());
         _resultValue = default(T);
    }

    private Result(T resultValue)
    {
        _common = new ResultCommon(Maybe<Error>.Nothing);
        _resultValue = resultValue;
    }

    public static Result<T> Ok(T resultValue)
    {
      resultValue.ToMaybe().OrElse(() => new ArgumentException(nameof(resultValue)));
      return new Result<T>(resultValue);
    }

    public static Result<T> Failure(Error error)
    {
        return new Result<T>(error);
    }
}

```

__Examples :__

_Define Error Enum_

```C#

Enum CodeFailure
{
    Unknwon,
    CodeError
}

```

### Sucess Case

```C#
Result<string>.Ok(string.Empty);
```

### Error Case

```C#
Result<string>.Failure(CodeFailure.CodeError);
```

_Note: Create the Custom Error with ```Enum``` implementation,```Enum member``` should be the value which you want to pass between layer, and operate based on the ```EnumMember```_

### Chaining with Monads

```C#

  public static async Task<Result<TResult>> OnSuccessAsync<T, TResult>(
            this Result<T> result, Func<T, Task<TResult>> fn)
{
}

 public static Result<T> OnFailure<T>(this Result<T> result, Func<Result<T>> fn)
{
}

public static Result<TResult> OnSuccess<T, TResult>(this Result<T> result, Func<T, Result<TResult>> fn)
{
}

public static Result<T> OnSuccess<T>(this Result<T> result, Func<T, Result> fn)
{
}
public static async Task<Result> OnSuccessAsync<T>(this Result<T> result, Func<T, Task<Result>> fn)
{
}
public static Result<T> Log<T>(this Result<T> result, string processorName, Action<string, T> successLog, Action<string, Error> errorLog)
{
}

public static async Task<Result> OnTransientFailureAsync(this Task<Result> resultTask, Func<Task<Result>> fn)
{
}

public static Result OnBoth(this Result result, Action action)
{

}

```

__Useages:__

```C#

public enum CustomerError
{
  DivideByZero
}

public static Task<Result<int>> Divide(int x, int y) => x==0 || y==0 ? Result<int>.Failure(Error.Create(CustomerError.DivideByZero)): Result<int>.Ok(x / y);


Add.OnFailureAsync(()=> Task1)
   .OnSuccessAsync((=> Task2))
   .OnBoth(()=>Log();)
   .Log(x=>x.Result,Caller,ErrorLogAction,SucessLogAction);

```

#### Handling Errors

```C#
if(Add(2,0).ToMaybe().Select(x=> x.Error.ToErrorType<CustomerError>().Value == CustomerError.DivideByZero).Value)
{

}
```

**Nuget Package : In-Progress**