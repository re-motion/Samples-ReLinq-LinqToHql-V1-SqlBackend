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
using System.Reflection;
using NHibernate.Impl;
using Remotion.Collections;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Utilities;

namespace NHibernate.ReLinq
{
  public class NHibernateInfo : IDatabaseInfo
  {
    private readonly ISession _session;

    public NHibernateInfo(ISession session)
    {
      _session = session;
    }

    public string GetTableName (FromClauseBase fromClause)
    {
      ArgumentUtility.CheckNotNull ("fromClause", fromClause);
      return fromClause.Identifier.Type.Name;
    }

    public string GetRelatedTableName (MemberInfo relationMember)
    {
      if (IsRelationProperty(relationMember))
      {
        return relationMember.Name;
      }
      else
      {
        return null;
      }
    }

    // TODO RELINQUING NHIBERNATE: Use NHibernate API to determine whether relationMember-property represents a relation
    private bool IsRelationProperty (MemberInfo relationMember)
    {
      var propertyInfo = relationMember as PropertyInfo;
      if(propertyInfo == null)
      {
        return false;
      }
      var entityName = propertyInfo.PropertyType.FullName;
      // If an NHibernate persister exists for the PropertyType, assume it represents a relation
      return ((SessionFactoryImpl) _session.SessionFactory).TryGetEntityPersister (entityName) != null;
    }

    public string GetColumnName (MemberInfo member)
    {
      ArgumentUtility.CheckNotNull ("member", member);
      PropertyInfo property = member as PropertyInfo;
      if (property == null)
      {
        return null;
      }
      else
      {
        return property.Name;
      }
    }

    public Tuple<string, string> GetJoinColumnNames (MemberInfo relationMember)
    {
      return Remotion.Collections.Tuple.NewTuple (relationMember.Name, "NOT_NEEDED_FOR_RELINQUING_NHIBERNATE");
    }

    public object ProcessWhereParameter (object parameter)
    {
      return parameter;
    }

    public MemberInfo GetPrimaryKeyMember (System.Type entityType)
    {
      // TODO RELINQUING NHIBERNATE: Return primary key if available
      return null;
    }

    public bool IsTableType (System.Type type)
    {
      throw new System.NotImplementedException();
    }

  }
}