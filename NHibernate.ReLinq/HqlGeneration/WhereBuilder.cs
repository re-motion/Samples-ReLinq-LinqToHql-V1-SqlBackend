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
using Remotion.Data.Linq;
using Remotion.Data.Linq.DataObjectModel;
using Remotion.Data.Linq.SqlGeneration;
using Remotion.Utilities;

namespace NHibernate.ReLinq.HqlGeneration
{
  public class WhereBuilder : IWhereBuilder
  {
    private readonly CommandBuilder _commandBuilder;
    private readonly BinaryConditionBuilder _builder;

    public WhereBuilder (CommandBuilder commandBuilder, IDatabaseInfo databaseInfo)
    {
      ArgumentUtility.CheckNotNull ("commandBuilder", commandBuilder);
      ArgumentUtility.CheckNotNull ("databaseInfo", databaseInfo);
      _commandBuilder = commandBuilder;
      _builder = new BinaryConditionBuilder (_commandBuilder);
    }

    public void BuildWherePart (ICriterion criterion)
    {
      if (criterion != null)
      {
        _commandBuilder.Append (" where ");
        AppendCriterion (criterion);
      }
    }

    private void AppendCriterion (ICriterion criterion)
    {
      if (criterion is BinaryCondition)
        AppendBinaryCondition ((BinaryCondition) criterion);
      else if (criterion is ComplexCriterion)
        AppendComplexCriterion ((ComplexCriterion) criterion);
      else if (criterion is NotCriterion)
        AppendNotCriterion ((NotCriterion) criterion);
      else if (criterion is Constant || criterion is Column) // cannot use "as" operator here because Constant/Column are value types
        AppendTopLevelValue (criterion);
      else
        throw new NotSupportedException ("The criterion kind " + criterion.GetType().Name + " is not supported.");
    }

    private void AppendBinaryCondition (BinaryCondition condition)
    {
      _builder.BuildBinaryConditionPart (condition);
    }

    private void AppendTopLevelValue (IValue value)
    {
      if (value is Constant)
      {
        Constant constant = (Constant) value;
        if (constant.Value == null)
          throw new NotSupportedException ("NULL constants are not supported as WHERE conditions.");
        else
          _commandBuilder.AppendEvaluation (constant);
      }
      else
      {
        _commandBuilder.AppendEvaluation ((Column) value);
        _commandBuilder.Append ("=1");
      }
    }

    private void AppendComplexCriterion (ComplexCriterion criterion)
    {
      _commandBuilder.Append ("(");
      AppendCriterion (criterion.Left);

      switch (criterion.Kind)
      {
        case ComplexCriterion.JunctionKind.And:
          _commandBuilder.Append (" and ");
          break;
        case ComplexCriterion.JunctionKind.Or:
          _commandBuilder.Append (" or ");
          break;
      }

      AppendCriterion (criterion.Right);
      _commandBuilder.Append (")");
    }

    private void AppendNotCriterion (NotCriterion criterion)
    {
      _commandBuilder.Append ("not ");
      AppendCriterion (criterion.NegatedCriterion);
    }
  }
}