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
using NHibernate.ReLinq.HqlGeneration.MethodCallGenerators;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Parsing;
using Remotion.Data.Linq.SqlGeneration;

namespace NHibernate.ReLinq.HqlGeneration
{
  public class HqlGenerator : SqlGeneratorBase<HqlGenerationContext>
  {
    public HqlGenerator (IDatabaseInfo databaseInfo)
        : this (databaseInfo, ParseMode.TopLevelQuery)
    {
    }

    protected HqlGenerator (IDatabaseInfo databaseInfo, ParseMode parseMode)
        : base (databaseInfo, parseMode)
    {
      MethodCallRegistry.Register (typeof (string).GetMethod ("ToUpper", new System.Type[] { }), new MethodCallUpper());
      MethodCallRegistry.Register (typeof (string).GetMethod ("ToLower", new System.Type[] { }), new MethodCallLower());
    }

    protected override HqlGenerationContext CreateContext ()
    {
      return new HqlGenerationContext (DatabaseInfo, MethodCallRegistry);
    }

    protected override IOrderByBuilder CreateOrderByBuilder (HqlGenerationContext context)
    {
      return new OrderByBuilder (context.CommandBuilder);
    }

    protected override IWhereBuilder CreateWhereBuilder (HqlGenerationContext context)
    {
      return new WhereBuilder (context.CommandBuilder, DatabaseInfo);
    }

    protected override IFromBuilder CreateFromBuilder (HqlGenerationContext context)
    {
      return new FromBuilder (context.CommandBuilder, DatabaseInfo);
    }

    protected override ISelectBuilder CreateSelectBuilder (HqlGenerationContext context)
    {
      return new SelectBuilder (context.CommandBuilder);
    }
  }
}