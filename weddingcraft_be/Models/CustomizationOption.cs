namespace weddingcraft_be.Models
{
    public class CustomizationOption
    {
        public int Id { get; set; }
        public string OptionName { get; set; } = "";
        public string ValuesJson { get; set; } = "";
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
