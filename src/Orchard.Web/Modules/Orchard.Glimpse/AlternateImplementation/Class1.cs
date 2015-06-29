using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.SqlCommand;
using NHibernate.Type;
using Orchard.Data;
using Orchard.Data.Providers;
using Orchard.Utility;

namespace Orchard.Glimpse.AlternateImplementation
{
    public class GlimpseInterceptor : ISessionInterceptor {
        private readonly IInterceptor _decoratedService;

        public GlimpseInterceptor() {
            _decoratedService = new EmptyInterceptor();
        }

        public bool OnLoad(object entity, object id, object[] state, string[] propertyNames, IType[] types) {

            Trace.WriteLine(entity, "On Load");
            return _decoratedService.OnLoad(entity, id, state, propertyNames, types);
        }

        public bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types) {
            return _decoratedService.OnFlushDirty(entity, id, currentState, previousState, propertyNames, types);
        }

        public bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types) {
            return _decoratedService.OnSave(entity, id, state, propertyNames, types);
        }

        public void OnDelete(object entity, object id, object[] state, string[] propertyNames, IType[] types) {
            _decoratedService.OnDelete(entity, id, state, propertyNames, types);
        }

        public void OnCollectionRecreate(object collection, object key) {
            _decoratedService.OnCollectionRecreate(collection, key);
        }

        public void OnCollectionRemove(object collection, object key) {
            _decoratedService.OnCollectionRemove(collection, key);
        }

        public void OnCollectionUpdate(object collection, object key) {
            _decoratedService.OnCollectionUpdate(collection, key);
        }

        public void PreFlush(ICollection entities) {
            _decoratedService.PreFlush(entities);
        }

        public void PostFlush(ICollection entities) {
            _decoratedService.PostFlush(entities);
        }

        public bool? IsTransient(object entity) {
            return _decoratedService.IsTransient(entity);
        }

        public int[] FindDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types) {
            return _decoratedService.FindDirty(entity, id, currentState, previousState, propertyNames, types);
        }

        public object Instantiate(string entityName, EntityMode entityMode, object id) {
            return _decoratedService.Instantiate(entityName, entityMode, id);
        }

        public string GetEntityName(object entity) {
            return _decoratedService.GetEntityName(entity);
        }

        public object GetEntity(string entityName, object id) {
            return _decoratedService.GetEntity(entityName, id);
        }

        public void AfterTransactionBegin(ITransaction tx) {

            Trace.WriteLine(tx, "After TX Begin");
            _decoratedService.AfterTransactionBegin(tx);
        }

        public void BeforeTransactionCompletion(ITransaction tx) {

            Trace.WriteLine(tx, "Before TX Completion");
            _decoratedService.BeforeTransactionCompletion(tx);
        }

        public void AfterTransactionCompletion(ITransaction tx) {

            Trace.WriteLine(tx, "After TX Completion");
            _decoratedService.AfterTransactionCompletion(tx);
        }

        public SqlString OnPrepareStatement(SqlString sql) {
            var sqlString = _decoratedService.OnPrepareStatement(sql);

            Trace.WriteLine(sqlString);

            return sqlString;
        }

        public void SetSession(ISession session) {
            _decoratedService.SetSession(session);
        }
    }
}