namespace StepUp.Application.Exceptions
{
    public class WeakPasswordException : Exception
    {
        public IReadOnlyList<string> Errors { get; }
        public WeakPasswordException(IEnumerable<string> errors)
            : base(string.Join("\n", errors))
        {
            Errors = errors.ToList();
        }
    }
}