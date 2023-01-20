using System;
using System.Collections.Generic;
using System.Reflection;

namespace ThrowException.CSharpLibs.ConfigParserLib
{
    public abstract class SettingManagerConfig<T> : SettingManager
        where T : IConfig, new()
    {
        protected SettingManagerConfig(PropertyInfo property, SettingAttribute attribute) : base(property, attribute)
        {
        }

        protected T Create(SettingInstance value)
        {
            var manager = new ConfigManager<T>();
            return manager.Apply(value);
        }
    }
}
