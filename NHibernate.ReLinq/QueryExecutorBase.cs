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
using System;
using System.Collections;
using Remotion.Data.Linq;
using Remotion.Data.Linq.DataObjectModel;
using Remotion.Data.Linq.SqlGeneration;
using Remotion.Utilities;

namespace NHibernate.ReLinq
{
  public abstract class QueryExecutorBase : IQueryExecutor
  {
    public QueryExecutorBase (ISqlGenerator sqlGenerator, ISession session)
    {
      SqlGenerator = sqlGenerator;
      Session = session;
    }

    public ISqlGenerator SqlGenerator { get; private set; }
    public ISession Session { get; private set; }
    public CommandData CommmandData { get; private set; }

    public object ExecuteSingle (QueryModel queryModel)
    {
      IEnumerable results = ExecuteCollection (queryModel);
      var resultList = new ArrayList();
      foreach (object o in results)
        resultList.Add (o);
      if (resultList.Count == 1)
        return resultList[0];
      else
      {
        string message = string.Format ("ExecuteSingle must return a single object, but the query returned {0} objects.", resultList.Count);
        throw new InvalidOperationException (message);
      }
    }

    public IEnumerable ExecuteCollection (QueryModel queryModel)
    {
      IQuery query = CreateQuery("<dynamic query>", queryModel);
      return query.Enumerable();
    }

    private void CheckProjection (IEvaluation evaluation)
    {
      if (!(evaluation is Column))
      {
        string message = string.Format ("This query provider does not support the given select projection ('{0}'). The projection must select "
                                        + "single DomainObject instances.", evaluation.GetType ().Name);
        throw new InvalidOperationException (message);
      }
      
      var column = (Column) evaluation;
      if (column.Name != "*")
      {
        string message = string.Format (
            "This query provider does not support selecting single columns ('{0}'). The projection must select whole DomainObject instances.",
            column.ColumnSource.AliasString + "." + column.Name);
        throw new InvalidOperationException (message);
      }
    }

    public virtual IQuery CreateQuery (string id, QueryModel queryModel)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      CommandData commandData = CreateStatement (queryModel);
      CheckProjection (commandData.SqlGenerationData.SelectEvaluation);

      return CreateQuery (id, commandData.Statement, commandData.Parameters);
    }

    public virtual IQuery CreateQuery(string id, string statement, CommandParameter[] commandParameters)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ArgumentUtility.CheckNotNull ("statement", statement);
      ArgumentUtility.CheckNotNull ("commandParameters", commandParameters);
      return CreateQuery(Session, statement, commandParameters);
    }

    private IQuery CreateQuery (ISession session, string statement, CommandParameter[] commandParameters)
    {
      IQuery hqlQuery = session.CreateQuery (statement);
      for (int i = 0; i < commandParameters.Length; i++)
      {
        var hqlQueryParameter = commandParameters[i];
        hqlQuery.SetParameter (hqlQueryParameter.Name.Replace (":", ""), hqlQueryParameter.Value);
      }
      return hqlQuery;
    }

    public virtual CommandData CreateStatement (QueryModel queryModel)
    {
      CommmandData = SqlGenerator.BuildCommand (queryModel);
      return CommmandData;
    }
  }
}