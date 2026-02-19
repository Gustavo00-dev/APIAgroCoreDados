namespace APIAgroCoreDados.Model
{
    public class Propriedade
    {
        public int IdPropriedade { get; set; }
        public int IdUsers { get; set; }
        public string Nome { get; set; } = string.Empty;
        public double Area { get; set; }
    }
}
