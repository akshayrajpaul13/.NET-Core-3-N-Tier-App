namespace Web.Api.Data.Helpers
{
    /// <summary>
    /// Provides a central reference for settings that are not yet configurable, preventing the
    /// use of magic strings
    /// </summary>
    public static class FixedSettings
    {
        public static int MaxYearsToCompleteOpenStartSemester = 2;
        public static int BankAccountVerificationValidityMonths = 6; // how long a bank account verification is valid for
    }
}
