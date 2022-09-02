using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Binders;
using Loxodon.Framework.Binding.Converters;
using Loxodon.Framework.Binding.Paths;
using Loxodon.Framework.Binding.Proxy.Sources;
using Loxodon.Framework.Binding.Proxy.Sources.Expressions;
using Loxodon.Framework.Binding.Proxy.Sources.Object;
using Loxodon.Framework.Binding.Proxy.Sources.Text;
using Loxodon.Framework.Binding.Proxy.Targets;

namespace SnkFramework.FluentBinding.Base
{
    public abstract class SnkBindingSetup : ISnkBindingSetup
    {
        private SnkIoCProvider _iocProvider;

        public void Initialize()
        {
            _initializePrimary();
            _InitializeSecondary();
        }

        private void _initializePrimary()
        {
            this._iocProvider = SnkIoCProvider.Instance;
        }

        protected virtual ObjectSourceProxyFactory CreateObjectSourceProxyFactory()
        {
            ObjectSourceProxyFactory objectSourceProxyFactory = new ObjectSourceProxyFactory();
            objectSourceProxyFactory.Register(new UniversalNodeProxyFactory(), 0);
            return objectSourceProxyFactory;
        }

        protected virtual TargetProxyFactory CreateTargetProxyFactory()
        {
            TargetProxyFactory targetFactory = new TargetProxyFactory();
            targetFactory.Register(new UniversalTargetProxyFactory(), 0);
            return targetFactory;
        }

        private void _InitializeSecondary()
        {
            PathParser pathParser = new PathParser();
            ConverterRegistry converterRegistry = new ConverterRegistry();

            SourceProxyFactory sourceFactory = new SourceProxyFactory();
            sourceFactory.Register(new LiteralSourceProxyFactory(), 0);

            ExpressionPathFinder expressionPathFinder = new ExpressionPathFinder();
            sourceFactory.Register(new ExpressionSourceProxyFactory(sourceFactory, expressionPathFinder), 1);

            ObjectSourceProxyFactory objectSourceProxyFactory = CreateObjectSourceProxyFactory();
            sourceFactory.Register(objectSourceProxyFactory, 2);

            TargetProxyFactory targetFactory = CreateTargetProxyFactory();
            BindingFactory bindingFactory = new BindingFactory(sourceFactory, targetFactory);
            StandardBinder binder = new StandardBinder(bindingFactory);

            SnkSnkMainThreadAsyncDispatcher snkSnkMainThreadAsyncDispatcher = new SnkSnkMainThreadAsyncDispatcher();

            _iocProvider.Register<ISnkMainThreadAsyncDispatcher>(snkSnkMainThreadAsyncDispatcher);

            _iocProvider.Register<IBinder>(binder);
            _iocProvider.Register<IBindingFactory>(bindingFactory);
            _iocProvider.Register<IConverterRegistry>(converterRegistry);

            _iocProvider.Register<IExpressionPathFinder>(expressionPathFinder);
            _iocProvider.Register<IPathParser>(pathParser);

            _iocProvider.Register<INodeProxyFactory>(objectSourceProxyFactory);
            _iocProvider.Register<INodeProxyFactoryRegister>(objectSourceProxyFactory);

            _iocProvider.Register<ISourceProxyFactory>(sourceFactory);
            _iocProvider.Register<ISourceProxyFactoryRegistry>(sourceFactory);

            _iocProvider.Register<ITargetProxyFactory>(targetFactory);
            _iocProvider.Register<ITargetProxyFactoryRegister>(targetFactory);
        }

        public void Uninitialize()
        {
            _iocProvider.Unregister<ISnkMainThreadAsyncDispatcher>();

            _iocProvider.Unregister<IBinder>();
            _iocProvider.Unregister<IBindingFactory>();
            _iocProvider.Unregister<IConverterRegistry>();

            _iocProvider.Unregister<IExpressionPathFinder>();
            _iocProvider.Unregister<IPathParser>();

            _iocProvider.Unregister<INodeProxyFactory>();
            _iocProvider.Unregister<INodeProxyFactoryRegister>();

            _iocProvider.Unregister<ISourceProxyFactory>();
            _iocProvider.Unregister<ISourceProxyFactoryRegistry>();

            _iocProvider.Unregister<ITargetProxyFactory>();
            _iocProvider.Unregister<ITargetProxyFactoryRegister>();
        }
    }
}