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
using Remotion.Data.Linq.Parsing;
using Remotion.Data.Linq.SqlGeneration;
using Remotion.Utilities;

namespace NHibernate.ReLinq.HqlGeneration
{
  public class BinaryConditionBuilder
  {
    private readonly CommandBuilder _commandBuilder;

    public BinaryConditionBuilder (CommandBuilder commandBuilder)
    {
      ArgumentUtility.CheckNotNull ("command", commandBuilder);
      _commandBuilder = commandBuilder;
    }

    public void BuildBinaryConditionPart (BinaryCondition binaryCondition)
    {
      if (binaryCondition.Left.Equals (new Constant (null)))
        AppendNullCondition (binaryCondition.Right, binaryCondition.Kind);
      else if (binaryCondition.Right.Equals (new Constant (null)))
        AppendNullCondition (binaryCondition.Left, binaryCondition.Kind);
      else if (binaryCondition.Kind == BinaryCondition.ConditionKind.Contains)
        AppendContainsCondition (binaryCondition.Left, binaryCondition.Right);
      else
        AppendGeneralCondition (binaryCondition);
    }

    private void AppendNullCondition (IValue value, BinaryCondition.ConditionKind kind)
    {
      AppendValue (value);
      switch (kind)
      {
        case BinaryCondition.ConditionKind.Equal:
          _commandBuilder.Append (" is null");
          break;
        default:
          Assertion.IsTrue (kind == BinaryCondition.ConditionKind.NotEqual, "null can only be compared via == and !=");
          _commandBuilder.Append (" is not null");
          break;
      }
    }

    private void AppendContainsCondition (IEvaluation left, IValue right)
    {
      if (left is Constant)
      {
        var constant = (Constant) left;
        if (constant.Value is ICollection)
        {
          var possibleEmptyCollection = (ICollection) constant.Value;
          if (possibleEmptyCollection.Count == 0)
            _commandBuilder.Append ("(0 = 1)");
          else
            AppendContainsForSubQuery (left, right);
        }
      }
      else
        AppendContainsForSubQuery (left, right);
    }

    private void AppendContainsForSubQuery (IEvaluation left, IValue right)
    {
      AppendValue (right);
      _commandBuilder.Append (" (");
      _commandBuilder.AppendEvaluation (left);
      _commandBuilder.Append (")");
    }

    protected virtual ISqlGenerator CreateSqlGeneratorForSubQuery (SubQuery subQuery, IDatabaseInfo databaseInfo, CommandBuilder commandBuilder)
    {
      return new InlineHqlGenerator (databaseInfo, commandBuilder, ParseMode.SubQueryInWhere);
    }

    private void AppendGeneralCondition (BinaryCondition binaryCondition)
    {
      _commandBuilder.Append ("(");
      AppendNullChecks (binaryCondition.Left, binaryCondition.Right, binaryCondition.Kind);

      AppendValue (binaryCondition.Left);
      _commandBuilder.Append (" ");
      AppendConditionKind (binaryCondition.Kind);
      _commandBuilder.Append (" ");
      AppendValue (binaryCondition.Right);
      _commandBuilder.Append (")");
    }

    private void AppendNullChecks (IValue left, IValue right, BinaryCondition.ConditionKind conditionKind)
    {
      if (left is Column || right is Column)
      {
        switch (conditionKind)
        {
          case BinaryCondition.ConditionKind.Equal:
          case BinaryCondition.ConditionKind.LessThanOrEqual:
          case BinaryCondition.ConditionKind.GreaterThanOrEqual:
            AppendNullChecksForEqualKinds (left, right);
            break;
          case BinaryCondition.ConditionKind.NotEqual:
            AppendNullChecksForNotEqualKind (left, right);
            break;
        }
      }
    }

    private void AppendNullChecksForEqualKinds (IValue left, IValue right)
    {
      if (left is Column && right is Column)
      {
        _commandBuilder.Append ("(");
        AppendNullCondition (left, BinaryCondition.ConditionKind.Equal);
        _commandBuilder.Append (" and ");
        AppendNullCondition (right, BinaryCondition.ConditionKind.Equal);
        _commandBuilder.Append (") or ");
      }
    }

    private void AppendNullChecksForNotEqualKind (IValue left, IValue right)
    {
      if (left is Column && right is Column)
      {
        _commandBuilder.Append ("(");
        AppendNullCondition (left, BinaryCondition.ConditionKind.Equal);
        _commandBuilder.Append (" and ");
        AppendNullCondition (right, BinaryCondition.ConditionKind.NotEqual);
        _commandBuilder.Append (") or ");
        _commandBuilder.Append ("(");
        AppendNullCondition (left, BinaryCondition.ConditionKind.NotEqual);
        _commandBuilder.Append (" and ");
        AppendNullCondition (right, BinaryCondition.ConditionKind.Equal);
        _commandBuilder.Append (") or ");
      }
      else if (left is Column)
      {
        AppendNullCondition (left, BinaryCondition.ConditionKind.Equal);
        _commandBuilder.Append (" or ");
      }
      else
      {
        AppendNullCondition (right, BinaryCondition.ConditionKind.Equal);
        _commandBuilder.Append (" or ");
      }
    }

    private void AppendValue (IValue value)
    {
      _commandBuilder.AppendEvaluation (value);
    }

    private void AppendConditionKind (BinaryCondition.ConditionKind kind)
    {
      string commandString;
      switch (kind)
      {
        case BinaryCondition.ConditionKind.Equal:
          commandString = "=";
          break;
        case BinaryCondition.ConditionKind.NotEqual:
          commandString = "<>";
          break;
        case BinaryCondition.ConditionKind.LessThan:
          commandString = "<";
          break;
        case BinaryCondition.ConditionKind.LessThanOrEqual:
          commandString = "<=";
          break;
        case BinaryCondition.ConditionKind.GreaterThan:
          commandString = ">";
          break;
        case BinaryCondition.ConditionKind.GreaterThanOrEqual:
          commandString = ">=";
          break;
        case BinaryCondition.ConditionKind.Like:
          commandString = "like";
          break;

        case BinaryCondition.ConditionKind.Add:
          commandString = "+";
          break;
        case BinaryCondition.ConditionKind.Subtract:
          commandString = "-";
          break;
        case BinaryCondition.ConditionKind.Multiply:
          commandString = "*";
          break;
        case BinaryCondition.ConditionKind.Divide:
          commandString = "/";
          break;
        case BinaryCondition.ConditionKind.Modulo:
          commandString = "%";
          break;


        default:
          throw new NotSupportedException ("The binary condition kind " + kind + " is not supported.");
      }
      _commandBuilder.Append (commandString);
    }
  }
}