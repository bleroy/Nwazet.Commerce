using System;
using System.Collections.Generic;
using Orchard;
using System.Web;

namespace Nwazet.Commerce.Tests.Stubs {
    public class WorkContextAccessorStub : IWorkContextAccessor {
        private readonly IDictionary<Type, object> _state;

        public WorkContextAccessorStub(IDictionary<Type, object> state) {
            _state = state;
        }

        public WorkContext GetContext(HttpContextBase httpContext) {
            return GetContext();
        }

        public IWorkContextScope CreateWorkContextScope(HttpContextBase httpContext) {
            throw new System.NotImplementedException();
        }

        public WorkContext GetContext() {
            return new WorkContextImpl(_state);
        }

        public IWorkContextScope CreateWorkContextScope() {
            throw new System.NotImplementedException();
        }
    }

    public class WorkContextImpl : WorkContext {
        private readonly IDictionary<Type, object> _state;

        public WorkContextImpl(IDictionary<Type, object> state) {
            _state = state;
        }

        public override T Resolve<T>() {
            return (T)_state[typeof(T)];
        }

        public override bool TryResolve<T>(out T service) {
            if (_state.ContainsKey(typeof (T))) {
                service = (T)_state[typeof (T)];
                return true;
            }
            service = default(T);
            return false;
        }

        public override T GetState<T>(string name) {
            return Resolve<T>();
        }

        public override void SetState<T>(string name, T value) {
            throw new System.NotImplementedException();
        }

        public override object Resolve(Type serviceType) {
            throw new NotImplementedException();
        }

        public override bool TryResolve(Type serviceType, out object service) {
            throw new NotImplementedException();
        }
    }
}
