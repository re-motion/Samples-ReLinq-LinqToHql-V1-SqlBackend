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
using Remotion.Data.Linq.SqlGeneration;
using Remotion.Utilities;

namespace NHibernate.ReLinq.HqlGeneration
{
  public class HqlGenerationContext : ISqlGenerationContext
  {
    public HqlGenerationContext (IDatabaseInfo databaseInfo, MethodCallSqlGeneratorRegistry methodCallRegistry)
    {
      ArgumentUtility.CheckNotNull ("databaseInfo", databaseInfo);
      ArgumentUtility.CheckNotNull ("methodCallRegistry", methodCallRegistry);

      CommandBuilder = new CommandBuilder (
          new StringBuilder(), new List<CommandParameter>(), databaseInfo, methodCallRegistry);
    }

    public HqlGenerationContext (CommandBuilder commandBuilder)
    {
      ArgumentUtility.CheckNotNull ("commandBuilder", commandBuilder);
      CommandBuilder = commandBuilder;
    }

    public CommandBuilder CommandBuilder { get; private set; }

    public string CommandText
    {
      get { return CommandBuilder.GetCommandText(); }
    }

    public CommandParameter[] CommandParameters
    {
      get { return CommandBuilder.GetCommandParameters(); }
    }
  }
}