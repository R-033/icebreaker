using System;
using System.Linq.Expressions;
using System.Reflection;

namespace CoreExtensions.Reflection
{
	public class PrecompiledConstructor<T>
	{
		private delegate S ConstructorActivator<out S>(params object[] args);

		private readonly ConstructorActivator<T> _activator;

		public PrecompiledConstructor(Type type) : this(type, null)
		{
		}

		public PrecompiledConstructor(Type type, Type[] constructorArgTypes)
		{
			if (type is null) throw new ArgumentNullException(nameof(type));

			if (constructorArgTypes is null)
			{
				var ctor = type.GetConstructor(Type.EmptyTypes);
				if (ctor is null) throw new Exception($"Constructor of type {type} with argument types passed cannot be found");
				_activator = GenerateEmptyActivator(ctor);
			}
			else
			{
				var ctor = type.GetConstructor(constructorArgTypes);
				if (ctor is null) throw new Exception($"Constructor of type {type} with argument types passed cannot be found");
				var @params = ctor.GetParameters();
				_activator = GenerateFilledActivator(ctor, @params);
			}
		}

		public PrecompiledConstructor(ConstructorInfo constructor)
		{
			if (constructor is null) throw new ArgumentNullException(nameof(constructor));
			var @params = constructor.GetParameters();
			_activator = @params is null || @params.Length == 0
				? GenerateEmptyActivator(constructor)
				: GenerateFilledActivator(constructor, @params);
		}

		private ConstructorActivator<T> GenerateEmptyActivator(ConstructorInfo ctor)
		{
			var expression = Expression.New(ctor);
			return Expression.Lambda<ConstructorActivator<T>>(expression).Compile();
		}

		private ConstructorActivator<T> GenerateFilledActivator(ConstructorInfo ctor, ParameterInfo[] @params)
		{
			var parameter = Expression.Parameter(typeof(object[]), "Arguments");
			var args = new Expression[@params.Length];

			for (int i = 0; i < args.Length; ++i)
			{
				var constant = Expression.Constant(i);
				var paramType = @params[i].ParameterType;

				var paramAccessor = Expression.ArrayIndex(parameter, constant);
				var paramCast = Expression.Convert(paramAccessor, paramType);
				args[i] = paramCast;
			}

			var invoker = Expression.New(ctor, args);
			return Expression.Lambda<ConstructorActivator<T>>(invoker, parameter).Compile();
		}

		public T Invoke(params object[] args) => _activator(args);
	}
}
