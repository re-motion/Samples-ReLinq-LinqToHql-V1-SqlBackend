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
using Remotion.Data.Linq.DataObjectModel;

namespace NHibernate.ReLinq.HqlGeneration
{
  public static class HqlUtility
  {
    public static string GetColumnString (Column column)
    {
      if (column.Name != null)
      {
        if (column.Name == "*")
        {
          return column.ColumnSource.Alias;
        }
        else
          return WrapSqlIdentifier (column.ColumnSource.Alias) + "." + WrapSqlIdentifier (column.Name);
      }

      if (column.ColumnSource.IsTable)
        return WrapSqlIdentifier (column.ColumnSource.Alias);

      return WrapSqlIdentifier (column.ColumnSource.Alias) + "." + WrapSqlIdentifier (column.ColumnSource.Alias);
    }

    public static string WrapSqlIdentifier (string identifier)
    {
      if (identifier != "*")
        return identifier;
      else
        throw new NotSupportedException ("calls with '*' currently not supported");
    }

    public static string GetTableDeclaration (Table table)
    {
      return WrapSqlIdentifier (table.Name) + " as " + WrapSqlIdentifier (table.Alias);
    }
  }
}