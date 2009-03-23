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
using System.Text;
using Remotion.Data.Linq;
using Remotion.Data.Linq.DataObjectModel;
using Remotion.Data.Linq.SqlGeneration;
using Remotion.Utilities;

namespace NHibernate.ReLinq.HqlGeneration
{
  public class CommandBuilder : ICommandBuilder
  {
    public CommandBuilder (
        StringBuilder commandText,
        List<CommandParameter> commandParameters,
        IDatabaseInfo databaseInfo,
        MethodCallSqlGeneratorRegistry methodCallRegistry)
    {
      ArgumentUtility.CheckNotNull ("commandText", commandText);
      ArgumentUtility.CheckNotNull ("commandParameters", commandParameters);
      ArgumentUtility.CheckNotNull ("databaseInfo", databaseInfo);
      ArgumentUtility.CheckNotNull ("methodCallRegistry", methodCallRegistry);

      CommandText = commandText;
      CommandParameters = commandParameters;
      DatabaseInfo = databaseInfo;
      MethodCallRegistry = methodCallRegistry;
    }

    public StringBuilder CommandText { get; private set; }
    public List<CommandParameter> CommandParameters { get; private set; }
    public IDatabaseInfo DatabaseInfo { get; private set; }
    public MethodCallSqlGeneratorRegistry MethodCallRegistry { get; private set; }

    public string GetCommandText ()
    {
      return CommandText.ToString();
    }

    public CommandParameter[] GetCommandParameters ()
    {
      return CommandParameters.ToArray();
    }

    public void Append (string text)
    {
      CommandText.Append (text);
    }

    public void AppendEvaluation (IEvaluation evaluation)
    {
      HqlEvaluationVisitor visitor = new HqlEvaluationVisitor (this, DatabaseInfo, MethodCallRegistry);
      evaluation.Accept (visitor);
    }

    public void AppendSeparatedItems<T> (IEnumerable<T> items, Action<T> appendAction)
    {
      bool first = true;
      foreach (T item in items)
      {
        if (!first)
          Append (", ");
        appendAction (item);
        first = false;
      }
    }

    public void AppendEvaluations (IEnumerable<IEvaluation> evaluations)
    {
      AppendSeparatedItems (evaluations, AppendEvaluation);
    }

    public CommandParameter AddParameter (object value)
    {
      CommandParameter parameter = new CommandParameter (":p" + (CommandParameters.Count + 1), value);
      CommandParameters.Add (parameter);
      return parameter;
    }
  }
}