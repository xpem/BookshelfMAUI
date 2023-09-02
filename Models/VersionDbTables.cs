namespace Models
{
    /// <summary>
    /// Table Versions
    /// </summary>
    public record VersionDbTables
    {
        public required int Id { get; set; }

        public required int VERSION { get; set; }
    }
}