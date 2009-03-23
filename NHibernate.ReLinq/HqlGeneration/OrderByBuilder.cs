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
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.DataObjectModel;
using Remotion.Data.Linq.SqlGeneration;
using Remotion.Utilities;

namespace NHibernate.ReLinq.HqlGeneration
{
  public class OrderByBuilder : IOrderByBuilder
  {
    private static string GetOrderedDirectionString (OrderDirection direction)
    {
      switch (direction)
      {
        case OrderDirection.Asc:
          return "asc";
        case OrderDirection.Desc:
          return "desc";
        default:
          throw new NotSupportedException ("OrderDirection " + direction + " is not supported.");
      }
    }

    private readonly CommandBuilder _commandBuilder;

    public OrderByBuilder (CommandBuilder commandBuilder)
    {
      ArgumentUtility.CheckNotNull ("commandBuilder", commandBuilder);
      _commandBuilder = commandBuilder;
    }

    public void BuildOrderByPart (List<OrderingField> orderingFields)
    {
      if (orderingFields.Count != 0)
      {
        _commandBuilder.Append (" order by ");
        _commandBuilder.AppendSeparatedItems (orderingFields, AppendOrderingField);
      }
    }

    private void AppendOrderingField (OrderingField orderingField)
    {
      _commandBuilder.AppendEvaluation (orderingField.Column);
      _commandBuilder.Append (" ");
      _commandBuilder.Append (GetOrderedDirectionString (orderingField.Direction));
    }
  }
}