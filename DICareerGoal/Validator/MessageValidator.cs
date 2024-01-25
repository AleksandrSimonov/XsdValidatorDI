using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Diagnostics;

namespace DICareerGoal.Validator
{
    /// <summary>
    /// Валидация отправляемых и принимаемых сообщений по xsd схеме
    /// </summary>
    public class MessageValidator : IMessageValidator
    {
        private readonly XmlSchemaSet _schemas;
        private readonly StringBuilder _errors;
        private readonly string _xsdFullName;
        private readonly string _xsdSchemaForValidation;

        /// <summary>
        /// Конструктор класса
        /// </summary>
        public MessageValidator(IOptions<AppSetting> appSettings)
        {
            Debugger.Launch();
            Debugger.Break();
            var x = appSettings.Value;
            _xsdSchemaForValidation = "xsd/XsdSchemaForValidation.xsd";
            //_xsdSchemaForValidation = appSettings?.Value?.XsdSchemaForValidation ?? throw new ArgumentNullException(nameof(appSettings));
            _schemas = GetXmlSchemaSetForValidation();
            _errors = new StringBuilder();
        }

        public ValidationResult ValidateFile(string fileFullName)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Schemas.Add(_schemas);
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += ValidationBySchemaHandler;
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;

            using (XmlReader r = XmlReader.Create(fileFullName, settings))
            {
                try
                {
                    while (r.Read()) { }
                }
                catch (Exception ex)
                {
                    _errors.AppendLine($"Ошибка валидации по xsd схеме {ex.Message}");
                }
            }

            ValidationResult result = new ValidationResult()
            {
                XmlFileFullName = fileFullName,
                XsdFileFullName = _xsdFullName,
                IsValid = _errors.Length == 0,
                Message = _errors.ToString()
            };

            string fileName = Path.GetFileName(fileFullName);

            return result;
        }

        /// <summary>
        /// Получает схему для валидации
        /// </summary>
        /// <returns>xml схема валидации</returns>
        private XmlSchemaSet GetXmlSchemaSetForValidation()
        {
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            Uri baseSchema = new Uri(AppDomain.CurrentDomain.BaseDirectory);
            string mySchema = new Uri(baseSchema, _xsdSchemaForValidation).ToString();
            XmlSchema schema = XmlSchema.Read(new XmlTextReader(mySchema), null);
            schemaSet.Add(schema);

            return schemaSet;
        }

        /// <summary>
        /// Обработчик события при валидации 
        /// </summary>
        /// <param name="sender">источник</param>
        /// <param name="e">событие</param>
        private void ValidationBySchemaHandler(object sender, ValidationEventArgs e)
        {
            _errors.AppendLine(e.Message);
        }
    }
}
