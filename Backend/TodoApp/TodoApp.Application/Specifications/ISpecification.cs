﻿using System.Linq.Expressions;

namespace TodoApp.Application.Specifications;

public interface ISpecification<T>
{
    Expression<Func<T, bool>>? Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    Expression<Func<T, object>>? OrderBy { get; }
    Expression<Func<T, object>>? OrderByDescending { get; }
    IQueryable<T> Apply(IQueryable<T> query);


    int Skip { get; }
    int Take { get; }
    bool IsPagingEnabled { get; }
}

