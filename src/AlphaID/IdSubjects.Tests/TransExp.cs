using System.Linq.Expressions;

namespace IdSubjects.Tests;
public static class TransExp<TIn, TOut>
{
    private static readonly Func<TIn, TOut> cache = GetFunc();
    private static Func<TIn, TOut> GetFunc()
    {
        ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "p");
        List<MemberBinding> memberBindingList = [];

        foreach (var item in typeof(TOut).GetProperties())
        {
            if (!item.CanWrite) continue;
            MemberExpression property = Expression.Property(parameterExpression, typeof(TIn).GetProperty(item.Name));
            MemberBinding memberBinding = Expression.Bind(item, property);
            memberBindingList.Add(memberBinding);
        }

        MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), [.. memberBindingList]);
        Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, [parameterExpression]);

        return lambda.Compile();
    }

    public static TOut Trans(TIn tIn)
    {
        return cache(tIn);
    }
}