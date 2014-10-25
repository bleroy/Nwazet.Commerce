using Orchard.DisplayManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nwazet.Commerce.Tests.Stubs {
    class ShapeFactoryStub : IShapeFactory {
        public IShape Create(string shapeType, INamedEnumerable<object> parameters, Func<dynamic> createShape) {
            throw new NotImplementedException();
        }

        public IShape Create(string shapeType, INamedEnumerable<object> parameters) {
            throw new NotImplementedException();
        }

        public IShape Create(string shapeType) {
            throw new NotImplementedException();
        }
    }
}
