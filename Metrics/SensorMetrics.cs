using Prometheus;

public static class SensorMetrics
{
    public static readonly Gauge Temperatura =
        Metrics.CreateGauge(
            "temperatura_solo",
            "Temperatura do solo",
            new GaugeConfiguration
            {
                LabelNames = new[] { "talhao" }
            });

    public static readonly Gauge Umidade =
        Metrics.CreateGauge(
            "umidade_solo",
            "Umidade do solo",
            new GaugeConfiguration
            {
                LabelNames = new[] { "talhao" }
            });

    public static readonly Gauge Precipitacao =
        Metrics.CreateGauge(
            "precipitacao",
            "Nivel de precipitacao",
            new GaugeConfiguration
            {
                LabelNames = new[] { "talhao" }
            });
}