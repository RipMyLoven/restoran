namespace Restoran.DTOs
{
    public class TableDto
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int Seats { get; set; }
        public int RestaurantId { get; set; }
    }

    public class CreateTableDto
    {
        public int Number { get; set; }
        public int Seats { get; set; }
    }
}
