namespace DICareerGoal.Validator
{
    public interface IMessageValidator
    {
        /// <summary>
        /// Проверить xml файл по xsd схеме
        /// </summary>
        /// <param name="fileFullName">полный путь к проверяемому файлу</param>
        /// <returns>результат проверки</returns>
        ValidationResult ValidateFile(string fileFullName);
    }
}
