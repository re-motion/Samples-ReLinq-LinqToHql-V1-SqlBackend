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
using Remotion.Data.Linq.DataObjectModel;
using Remotion.Data.Linq.SqlGeneration;
using Remotion.Utilities;

namespace NHibernate.ReLinq.HqlGeneration
{
  public class SelectBuilder : ISelectBuilder
  {
    private readonly CommandBuilder _commandBuilder;

    public SelectBuilder (CommandBuilder commandBuilder)
    {
      ArgumentUtility.CheckNotNull ("commandBuilder", commandBuilder);
      _commandBuilder = commandBuilder;
    }

    public void BuildSelectPart (IEvaluation selectEvaluation, List<MethodCall> resultModifiers)
    {
      ArgumentUtility.CheckNotNull ("selectEvaluation", selectEvaluation);
      bool evaluation = true;
      _commandBuilder.Append ("select ");
      // Currently only single list member supported
      if (resultModifiers != null)
      {
        foreach (var methodCall in resultModifiers)
        {
          string method = methodCall.EvaluationMethodInfo.Name;

          if (method == "Count")
          {
            _commandBuilder.Append ("count(*) ");
            evaluation = false;
          }
          else
          {
            if (method == "Distinct")
              _commandBuilder.Append ("distinct ");
            else
            {
              string message = string.Format ("Method '{0}' is not supported.", method);
              throw new NotSupportedException (message);
            }
          }
        }
      }

      if (evaluation)
      {
        AppendEvaluation (selectEvaluation);
      }
    }

    private void AppendEvaluation (IEvaluation selectEvaluation)
    {
      _commandBuilder.AppendEvaluation (selectEvaluation);
      _commandBuilder.Append (" ");
    }
  }
}