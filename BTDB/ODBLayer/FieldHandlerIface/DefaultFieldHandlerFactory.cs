﻿using System;
using System.Reflection;
using BTDB.ODBLayer.FieldHandlerImpl;

namespace BTDB.ODBLayer.FieldHandlerIface
{
    public class DefaultFieldHandlerFactory : IFieldHandlerFactory
    {
        static readonly IFieldHandler[] FieldHandlers = new IFieldHandler[]
            {
                new StringFieldHandler(),
                new SignedFieldHandler(),
                new UnsignedFieldHandler(),
                new BoolFieldHandler(),
                new DoubleFieldHandler(),
                new DBObjectFieldHandler(),
            };

        public IFieldHandler CreateFromProperty(string tableName, Type clientType, PropertyInfo property)
        {
            var propertyType = property.PropertyType;
            foreach (var fieldHandler in FieldHandlers)
            {
                if (fieldHandler.IsCompatibleWith(propertyType)) return fieldHandler;
            }
            return null;
        }

        public IFieldHandler CreateFromName(string tableName, string fieldName, string handlerName, byte[] configuration)
        {
            foreach (var fieldHandler in FieldHandlers)
            {
                if (fieldHandler.Name == handlerName) return fieldHandler;
            }
            return null;
        }
    }
}