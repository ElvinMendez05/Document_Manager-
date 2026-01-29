namespace Document_Manager.Presentation.Services
{
    public class TokenStorageService
    {
        private string? _token;

        public void SaveToken(string token)
        {
            _token = token;
        }

        public string? GetToken()
        {
            return _token;
        }

        public void Clear()
        {
            _token = null;
        }
    }
}
