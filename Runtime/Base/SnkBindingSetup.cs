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
    public class SnkBindingSetup
    {
        static public void Initialize()
        {
            SnkIoCProvider iocProvider = SnkIoCProvider.Instance;

            PathParser pathParser = new PathParser();
            ExpressionPathFinder expressionPathFinder = new ExpressionPathFinder();
            ConverterRegistry converterRegistry = new ConverterRegistry();

            ObjectSourceProxyFactory objectSourceProxyFactory = new ObjectSourceProxyFactory();
            objectSourceProxyFactory.Register(new UniversalNodeProxyFactory(), 0);

            SourceProxyFactory sourceFactory = new SourceProxyFactory();
            sourceFactory.Register(new LiteralSourceProxyFactory(), 0);
            sourceFactory.Register(new ExpressionSourceProxyFactory(sourceFactory, expressionPathFinder), 1);
            sourceFactory.Register(objectSourceProxyFactory, 2);

            TargetProxyFactory targetFactory = new TargetProxyFactory();
            targetFactory.Register(new UniversalTargetProxyFactory(), 0);
            targetFactory.Register(new UnityTargetProxyFactory(), 10);
#if UNITY_2019_1_OR_NEWER
            targetFactory.Register(new VisualElementProxyFactory(), 30);
#endif

            BindingFactory bindingFactory = new BindingFactory(sourceFactory, targetFactory);
            StandardBinder binder = new StandardBinder(bindingFactory);

            SnkSnkMainThreadAsyncDispatcher snkSnkMainThreadAsyncDispatcher = new SnkSnkMainThreadAsyncDispatcher();


            iocProvider.Register<ISnkMainThreadAsyncDispatcher>(snkSnkMainThreadAsyncDispatcher);

            iocProvider.Register<IBinder>(binder);
            iocProvider.Register<IBindingFactory>(bindingFactory);
            iocProvider.Register<IConverterRegistry>(converterRegistry);

            iocProvider.Register<IExpressionPathFinder>(expressionPathFinder);
            iocProvider.Register<IPathParser>(pathParser);

            iocProvider.Register<INodeProxyFactory>(objectSourceProxyFactory);
            iocProvider.Register<INodeProxyFactoryRegister>(objectSourceProxyFactory);

            iocProvider.Register<ISourceProxyFactory>(sourceFactory);
            iocProvider.Register<ISourceProxyFactoryRegistry>(sourceFactory);

            iocProvider.Register<ITargetProxyFactory>(targetFactory);
            iocProvider.Register<ITargetProxyFactoryRegister>(targetFactory);
        }

        static public void Uninitialize()
        {
            SnkIoCProvider iocProvider = SnkIoCProvider.Instance;

            iocProvider.Unregister<ISnkMainThreadAsyncDispatcher>();

            iocProvider.Unregister<IBinder>();
            iocProvider.Unregister<IBindingFactory>();
            iocProvider.Unregister<IConverterRegistry>();

            iocProvider.Unregister<IExpressionPathFinder>();
            iocProvider.Unregister<IPathParser>();

            iocProvider.Unregister<INodeProxyFactory>();
            iocProvider.Unregister<INodeProxyFactoryRegister>();

            iocProvider.Unregister<ISourceProxyFactory>();
            iocProvider.Unregister<ISourceProxyFactoryRegistry>();

            iocProvider.Unregister<ITargetProxyFactory>();
            iocProvider.Unregister<ITargetProxyFactoryRegister>();
        }
    }
}