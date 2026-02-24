namespace APIAgroCoreDados.Model
{
    public class Sensor
    {
        public int Id { get; set; }
        public int IdTalhao { get; set; }
        public double UmidadeSolo { get; set; }
        public double Temperatura { get; set; }
        public double NivelPrecipitacao { get; set; }
        public DateTime DataUltimaAtualizacao { get; set; }
    }
}
