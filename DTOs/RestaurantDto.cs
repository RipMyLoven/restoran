namespace Restoran.DTOs
{
    public class RestaurantDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TableCount { get; set; }
        public string AllergyTags { get; set; } = string.Empty;
        public string DietTags { get; set; } = string.Empty;
    }

    public class CreateRestaurantDto
    {
        public string Name { get; set; } = string.Empty;
        public int TableCount { get; set; }
        public string AllergyTags { get; set; } = string.Empty;
        public string DietTags { get; set; } = string.Empty;
    }
}
