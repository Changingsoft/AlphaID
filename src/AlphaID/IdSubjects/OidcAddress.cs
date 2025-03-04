namespace IdSubjects
{
    internal class OidcAddress
    {
        public string Formatted { get; set; } = null!;

        public string? StreetAddress { get; set; }

        public string? Locality { get; set; }

        public string? Region { get; set; }

        public string? PostalCode { get; set; }

        public string? Country { get; set; }
    }
}
