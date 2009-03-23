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
using NHibernate.ReLinq.HqlGeneration;
using Remotion.Data.Linq.SqlGeneration;
using Remotion.Utilities;

namespace NHibernate.ReLinq
{
  public class QueryFactory
  {
    public static Queryable<T> CreateLinqQuery<T> (ISqlGenerator sqlGenerator, ISession session)
    {
      ArgumentUtility.CheckNotNull ("sqlGenerator", sqlGenerator);
      ArgumentUtility.CheckNotNull ("session", session);
      return new Queryable<T> (sqlGenerator, session);
    }

    public static Queryable<T> CreateLinqQuery<T> (ISession session)
    {
      return new Queryable<T> (new HqlGenerator (new NHibernateInfo (session)), session);
    }
  }
}