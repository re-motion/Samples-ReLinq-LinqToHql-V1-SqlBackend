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
using System.Linq.Expressions;
using Remotion.Data.Linq.QueryProviderImplementation;
using Remotion.Data.Linq.SqlGeneration;
using Remotion.Mixins;
using Remotion.Reflection;


namespace NHibernate.ReLinq
{
  public class Queryable<T> : QueryableBase<T>, IQueryableInfo
  {
    public Queryable (QueryProvider provider, Expression expression)
        : base (provider, expression)
    {
    }

    public Queryable (ISqlGenerator sqlGenerator, ISession session)
        : base (new QueryProvider(ObjectFactory.Create<QueryExecutor<T>>(ParamList.Create (sqlGenerator, session))))
    {
    }

    public override string ToString ()
    {
      return "ReLinqNHibernateQueryFactory<" + typeof (T).Name + ">";
    }

    public new QueryProvider Provider
    {
      get { return (QueryProvider) base.Provider; }
    }

    public QueryExecutor<T> GetExecutor ()
    {
      return (QueryExecutor<T>) Provider.Executor;
    }

    public CommandData GetCommandData ()
    {
      return ((QueryExecutorBase) Provider.Executor).CommmandData;
    }
  }
}