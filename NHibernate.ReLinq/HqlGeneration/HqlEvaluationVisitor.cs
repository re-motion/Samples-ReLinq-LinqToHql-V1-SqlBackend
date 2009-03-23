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


namespace NHibernate.ReLinq.HqlGeneration
{
  public class HqlEvaluationVisitor : IEvaluationVisitor
  {
    public HqlEvaluationVisitor (
        CommandBuilder commandBuilder,
        IDatabaseInfo databaseInfo,
        MethodCallSqlGeneratorRegistry methodCallRegistry)
    {
      ArgumentUtility.CheckNotNull ("commandBuilder", commandBuilder);
      ArgumentUtility.CheckNotNull ("databaseInfo", databaseInfo);
      ArgumentUtility.CheckNotNull ("methodCallRegistry", methodCallRegistry);

      CommandBuilder = commandBuilder;
      DatabaseInfo = databaseInfo;
      MethodCallRegistry = methodCallRegistry;
    }

    public CommandBuilder CommandBuilder { get; private set; }
    public IDatabaseInfo DatabaseInfo { get; private set; }
    public MethodCallSqlGeneratorRegistry MethodCallRegistry { get; private set; }

    #region IEvaluationVisitor Members

    public void VisitBinaryEvaluation (BinaryEvaluation binaryEvaluation)
    {
      throw new NotImplementedException();
    }

    public void VisitComplexCriterion (ComplexCriterion complexCriterion)
    {
      throw new NotImplementedException();
    }

    public void VisitNotCriterion (NotCriterion notCriterion)
    {
      throw new NotImplementedException();
    }

    public void VisitConstant (Constant constant)
    {
      ArgumentUtility.CheckNotNull ("constant", constant);

      if (constant.Value == null)
        CommandBuilder.CommandText.Append ("null");
      else if (constant.Value is ICollection)
        AddConstantCollection ((ICollection) constant.Value);
      else if (constant.Value.Equals (true))
        CommandBuilder.Append ("(1=1)");
      else if (constant.Value.Equals (false))
        CommandBuilder.Append ("(1<>1)");
      else
      {
        CommandParameter parameter = CommandBuilder.AddParameter (constant.Value);
        CommandBuilder.CommandText.Append (parameter.Name);
      }
    }

    public void VisitColumn (Column column)
    {
      ArgumentUtility.CheckNotNull ("column", column);
      CommandBuilder.CommandText.Append (HqlUtility.GetColumnString (column));
    }

    public void VisitBinaryCondition (BinaryCondition binaryCondition)
    {
      ArgumentUtility.CheckNotNull ("binaryCondition", binaryCondition);
      new BinaryConditionBuilder (CommandBuilder).BuildBinaryConditionPart (binaryCondition);
    }

    public void VisitSubQuery (SubQuery subQuery)
    {
      CommandBuilder.Append ("(");
      new InlineHqlGenerator (DatabaseInfo, CommandBuilder, subQuery.ParseMode).BuildCommand (subQuery.QueryModel);
      CommandBuilder.Append (")");
      if (subQuery.Alias != null)
      {
        CommandBuilder.Append (" [");
        CommandBuilder.Append (subQuery.Alias);
        CommandBuilder.Append ("]");
      }
    }

    public void VisitMethodCall (MethodCall methodCall)
    {
      ArgumentUtility.CheckNotNull ("methodCall", methodCall);

      MethodCallRegistry.GetGenerator (methodCall.EvaluationMethodInfo).GenerateSql (methodCall, CommandBuilder);
    }

    public void VisitNewObjectEvaluation (NewObject newObject)
    {
      throw new NotImplementedException();
    }

    public void VisitSourceMarkerEvaluation (SourceMarkerEvaluation sourceMarkerEvaluation)
    {
      throw new NotImplementedException();
    }

    #endregion

    private void AddConstantCollection (ICollection enumerable)
    {
      throw new NotImplementedException();
    }
  }
}