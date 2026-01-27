namespace Document_Manager.Domain.ValueObjects
{
    public class FilePath
    {
        public string Value { get; }

        public FilePath(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("La ruta no puede estar vacía");

            Value = value;
        }
    }
}
