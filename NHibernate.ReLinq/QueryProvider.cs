// This file is part of NHibernate.ReLinq an NHibernate (www.nhibernate.org) Linq-provider.
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// NHibernate.ReLinq is based on re-motion re-linq (http://www.re-motion.org/).
// 
// NHibernate.ReLinq is free software: you can redistribute it and/or modify
// it under the terms of the Lesser GNU General Public License as published by
// the Free Software Foundation, either version 2.1 of the License, or
// (at your option) any later version.
// 
// NHibernate.ReLinq is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// Lesser GNU General Public License for more details.
// 
// You should have received a copy of the Lesser GNU General Public License
// along with NHibernate.ReLinq.  If not, see http://www.gnu.org/licenses/.
// 
using System.Linq;
using System.Linq.Expressions;
using Remotion.Data.Linq;
using Remotion.Utilities;
using System;

namespace NHibernate.ReLinq
{
  public class QueryProvider : QueryProviderBase
  {
    public QueryProvider (QueryExecutorBase executor)
        : base (executor)
    {
    }

    protected override IQueryable<T> CreateQueryable<T> (Expression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      System.Type queryableType = typeof (Queryable<>).MakeGenericType (typeof (T));
      return (IQueryable<T>) Activator.CreateInstance (queryableType, this, expression);
    }

    // NHibernate count(*) returns long, Linq expects int, which gives rise to this rather ugly piece of code.
    public override TResult Execute<TResult> (Expression expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      var linqOperation = ((MethodCallExpression) expression).Method.Name;
      var expressionResult = Executor.ExecuteSingle (GenerateQueryModel (expression));
      if (linqOperation == "Count")
        return (TResult) (object) Convert.ToInt32(expressionResult);
      else
        return (TResult) expressionResult;
    }

  }
}