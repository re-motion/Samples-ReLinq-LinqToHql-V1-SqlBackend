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
using System.Collections.Generic;
using Remotion.Data.Linq;
using Remotion.Data.Linq.DataObjectModel;
using Remotion.Data.Linq.Parsing;
using Remotion.Data.Linq.SqlGeneration;
using Remotion.Utilities;

namespace NHibernate.ReLinq.HqlGeneration
{
  public class FromBuilder : IFromBuilder
  {
    private readonly CommandBuilder _commandBuilder;
    private readonly IDatabaseInfo _databaseInfo;

    public FromBuilder (CommandBuilder commandBuilder, IDatabaseInfo databaseInfo)
    {
      ArgumentUtility.CheckNotNull ("commandBuilder", commandBuilder);
      _commandBuilder = commandBuilder;
      _databaseInfo = databaseInfo;
    }

    public void BuildFromPart (List<IColumnSource> fromSources, JoinCollection joins)
    {
      _commandBuilder.Append ("from ");

      bool first = true;
      foreach (IColumnSource fromSource in fromSources)
      {
        Table table = fromSource as Table;
        Assertion.IsNotNull (table, "table must not be null.");
        if (!first)
            _commandBuilder.Append (", ");
          _commandBuilder.Append (HqlUtility.GetTableDeclaration (table));
        

        if (joins != null)
          AppendJoinPart (joins[fromSource]);
        first = false;
      }
    }



    protected virtual ISqlGenerator CreateSqlGeneratorForSubQuery (
        SubQuery subQuery, IDatabaseInfo databaseInfo, CommandBuilder commandBuilder)
    {
      return new InlineHqlGenerator (databaseInfo, commandBuilder, ParseMode.SubQueryInFrom);
    }

    private void AppendJoinPart (IEnumerable<SingleJoin> joins)
    {
      foreach (SingleJoin join in joins)
        AppendJoinExpression (join);
    }

    private void AppendJoinExpression (SingleJoin join)
    {
      _commandBuilder.Append (" join ");
      _commandBuilder.Append (GetColumnString (join.LeftColumn));
      _commandBuilder.Append (" as ");
      _commandBuilder.Append (join.RightColumn.ColumnSource.Alias);
    }

    private static string GetColumnString (Column column)
    {
      if (column.Name != null)
        return WrapHqlIdentifier (column.ColumnSource.Alias) + "." + WrapHqlIdentifier (column.Name);
      if (column.ColumnSource.IsTable)
        return WrapHqlIdentifier (column.ColumnSource.Alias);
      return WrapHqlIdentifier (column.ColumnSource.Alias) + "." + WrapHqlIdentifier (column.ColumnSource.Alias);
    }

    private static string WrapHqlIdentifier (string identifier)
    {
      return identifier;
    }


    public void BuildLetPart (List<LetData> letDataCollection)
    {
      ArgumentUtility.CheckNotNull ("letData", letDataCollection);
      if (letDataCollection.Count > 0)
        throw new NotImplementedException ("implement conversion of let to hql");   
    }
  }
}