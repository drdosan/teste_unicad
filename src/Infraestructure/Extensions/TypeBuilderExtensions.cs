using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Raizen.UniCad.Extensions
{
    /// <summary>
    /// Extenções para TypeBuilder
    /// </summary>
    public static class TypeBuilderExtensions
    {
        /// <summary>
        /// Adiciona uma propriedade completa (Propriedade pública, Campo privado, Getter e Setter) baseado nas informações de outra propriedade.
        /// </summary>
        /// <param name="builder">TypeBuilder.</param>
        /// <param name="property">A propriedade referência para edição do tipo.</param>
        public static void AddProperty(this TypeBuilder builder, PropertyInfo property)
        {
            if (builder == null) throw new ArgumentNullException("builder");
            if (property == null) throw new ArgumentNullException("property");

            var _fieldBuilder = builder.DefineField(
                    string.Format("_{0}", property.Name),
                    property.PropertyType,
                    FieldAttributes.Private);

            var _propertyBuilder = builder.DefineProperty(
                property.Name,
                System.Reflection.PropertyAttributes.None,
                property.PropertyType,
                new Type[] { property.PropertyType });

            var _getSetAttributes = MethodAttributes.Public | MethodAttributes.HideBySig;

            var _getProperty = builder.DefineMethod(
                string.Format("_get_{0}", property.Name),
                _getSetAttributes,
                property.PropertyType,
                Type.EmptyTypes);

            var _getPropertyIL = _getProperty.GetILGenerator();
            _getPropertyIL.Emit(OpCodes.Ldarg_0);
            _getPropertyIL.Emit(OpCodes.Ldfld, _fieldBuilder);
            _getPropertyIL.Emit(OpCodes.Ret);

            var _setProperty = builder.DefineMethod(
                string.Format("_set_{0}", property.Name),
                _getSetAttributes,
                null,
                new Type[] { property.PropertyType });

            var _setPropertyIL = _setProperty.GetILGenerator();
            _setPropertyIL.Emit(OpCodes.Ldarg_0);
            _setPropertyIL.Emit(OpCodes.Ldarg_1);
            _setPropertyIL.Emit(OpCodes.Stfld, _fieldBuilder);
            _setPropertyIL.Emit(OpCodes.Ret);

            _propertyBuilder.SetGetMethod(_getProperty);
            _propertyBuilder.SetSetMethod(_setProperty);
        }

        /// <summary>
        /// Adiciona propriedades completas (Propriedade pública, Campo privado, Getter e Setter) baseado nas informações de outras propriedades.
        /// </summary>
        /// <param name="builder">TypeBuilder</param>
        /// <param name="properties">As propriedades referência para edição do tipo.</param>
        public static void AddProperties(this TypeBuilder builder, params PropertyInfo[] properties)
        {
            if (builder == null) throw new ArgumentNullException("builder");

            if (properties != null)
                foreach (var _property in properties)
                {
                    builder.AddProperty(_property);
                }
        }
    }
}