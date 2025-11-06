namespace ThesisTestAPI.Services
{
    public static class Cursor
    {
        public static string Encode(DateTimeOffset createdAtUtc, Guid id)
        {
            var payload = $"{createdAtUtc.Ticks}:{id}";
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(payload));
        }

        public static (DateTimeOffset createdAtUtc, Guid id) Decode(string cursor)
        {
            var raw = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
            var parts = raw.Split(':', 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2) throw new FormatException("Invalid cursor.");
            var ticks = long.Parse(parts[0]);
            var id = Guid.Parse(parts[1]);
            return (new DateTime(ticks, DateTimeKind.Utc), id);
        }
    }
}
