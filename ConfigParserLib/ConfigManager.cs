using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ThrowException.CSharpLibs.TypeParserLib;

namespace ThrowException.CSharpLibs.ConfigParserLib
{
    public class ConfigManager<T>
        where T : new()
    {
        private readonly List<SettingManager> _settings;

        private SettingManager CreateSettingManagerScalar(Type baseType, PropertyInfo property, SettingAttribute attribute)
        {
            var finalType = typeof(SettingManagerScalar<>).MakeGenericType(baseType);
            var setting = (SettingManager)finalType
                .GetConstructor(new Type[] { typeof(PropertyInfo), typeof(SettingAttribute) })
                .Invoke(new object[] { property, attribute });
            setting.Setup();
            return setting;
        }

        private SettingManager CreateSettingManagerList(Type listType, PropertyInfo property, SettingAttribute attribute)
        {
            var finalType = typeof(SettingManagerList<>).MakeGenericType(listType);
            var setting = (SettingManager)finalType
                .GetConstructor(new Type[] { typeof(PropertyInfo), typeof(SettingAttribute) })
                .Invoke(new object[] { property, attribute });
            setting.Setup();
            return setting;
        }

        private SettingManager CreateSettingManagerScalarConfig(Type baseType, PropertyInfo property, SettingAttribute attribute)
        {
            var finalType = typeof(SettingManagerScalarConfig<>).MakeGenericType(baseType);
            var setting = (SettingManager)finalType
                .GetConstructor(new Type[] { typeof(PropertyInfo), typeof(SettingAttribute) })
                .Invoke(new object[] { property, attribute });
            setting.Setup();
            return setting;
        }

        private SettingManager CreateSettingManagerListConfig(Type listType, PropertyInfo property, SettingAttribute attribute)
        {
            var finalType = typeof(SettingManagerListConfig<>).MakeGenericType(listType);
            var setting = (SettingManager)finalType
                .GetConstructor(new Type[] { typeof(PropertyInfo), typeof(SettingAttribute) })
                .Invoke(new object[] { property, attribute });
            setting.Setup();
            return setting;
        }

        public ConfigManager()
        {
            _settings = new List<SettingManager>();

            var verbType = typeof(T);

            foreach (var property in verbType.GetProperties())
            {
                var settingAttribute = (SettingAttribute)property
                    .GetCustomAttributes(typeof(SettingAttribute), true)
                    .SingleOrDefault();

                if (settingAttribute != null)
                {
                    var baseType = property.PropertyType;

                    if (baseType.IsConstructedGenericType &&
                        (baseType.GenericTypeArguments.Length == 1))
                    {
                        var genericTypeArgument = baseType.GenericTypeArguments.Single();
                        if ((baseType == typeof(List<>).MakeGenericType(genericTypeArgument)) ||
                            (baseType == typeof(IEnumerable<>).MakeGenericType(genericTypeArgument)))
                        {
                            if (genericTypeArgument.GetInterfaces().Contains(typeof(IConfig)))
                            {
                                _settings.Add(CreateSettingManagerListConfig(genericTypeArgument, property, settingAttribute));
                            }
                            else
                            {
                                _settings.Add(CreateSettingManagerList(genericTypeArgument, property, settingAttribute));
                            }
                        }
                        else
                        {
                            if (baseType.GetInterfaces().Contains(typeof(IConfig)))
                            {
                                _settings.Add(CreateSettingManagerScalarConfig(baseType, property, settingAttribute));
                            }
                            else
                            {
                                _settings.Add(CreateSettingManagerScalar(baseType, property, settingAttribute));
                            }
                        }
                    }
                    else
                    {
                        if (baseType.GetInterfaces().Contains(typeof(IConfig)))
                        {
                            _settings.Add(CreateSettingManagerScalarConfig(baseType, property, settingAttribute));
                        }
                        else
                        {
                            _settings.Add(CreateSettingManagerScalar(baseType, property, settingAttribute));
                        }
                    }
                }
            }
        }

        private SettingManager GetSettingManager(SettingInstance setting)
        {
            var result = _settings.SingleOrDefault(o => o.Match(setting.Name));

            if (result == null)
            {
                throw new ConfigParseException("Unknown config setting {0}", setting.Name);
            }

            return result;
        }

        public T Apply(SettingInstance value)
        {
            T result = new T();

            foreach (var child in value.Children)
            {
                var manager = GetSettingManager(child);
                manager.Add(child);
            }

            foreach (var manager in _settings)
            {
                manager.Apply(result);
            }

            return result;
        }

        public SettingInstance Get(T value, string name)
        {
            var setting = new SettingInstance(name, null);

            foreach (var sub in _settings)
            {
                foreach (var subSetting in sub.Get(value))
                {
                    setting.Add(subSetting);
                }
            }

            return setting;
        }
    }
}
