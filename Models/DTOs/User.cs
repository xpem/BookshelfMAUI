namespace Models.DTOs
{
    public class User
    {
        public required int Id { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        [Obsolete("Use RefreshToken instead. Kept for migration compatibility.")]
        public string? Password { get; set; }

        public string? Token { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}
