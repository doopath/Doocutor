using CUI.ColorSchemes;
using Utils.Exceptions.NotExitExceptions;

namespace CUI
{
    public class ColorSchemesRepository
    {
        private readonly Dictionary<string, IColorScheme> _repository;

        public ColorSchemesRepository()
        {
            _repository = new();
        }

        public void Add(IColorScheme scheme)
            => _repository.Add(scheme.Name, scheme);

        public IColorScheme Get(string schemeName)
        {
            try
            {
                return _repository[schemeName];
            }
            catch (ArgumentNullException error)
            {
                throw new ColorSchemeWasNotFoundException($"Color scheme with name={schemeName} was not found!");
            }
        }
    }
}