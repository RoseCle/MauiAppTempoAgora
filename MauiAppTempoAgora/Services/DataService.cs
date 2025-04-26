using MauiAppTempoAgora.Models;
using Newtonsoft.Json.Linq;

namespace MauiAppTempoAgora.Services
{
    public class DataService
    {
        public static async Task<Tempo?> GetPrevisao(string cidade)
        {
            Tempo? t = null;

            string chave = "ae13388ac8c59b26afd770774bfde9dd";
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={cidade}&units=metric&appid={chave}";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage resp = await client.GetAsync(url);

                    if (resp.IsSuccessStatusCode)
                    {
                        string json = await resp.Content.ReadAsStringAsync();
                        var rascunho = JObject.Parse(json);

                        DateTime sunrise = DateTime.UnixEpoch.AddSeconds((double)rascunho["sys"]["sunrise"]).ToLocalTime();
                        DateTime sunset = DateTime.UnixEpoch.AddSeconds((double)rascunho["sys"]["sunset"]).ToLocalTime();

                        t = new Tempo
                        {
                            lat = (double)rascunho["coord"]["lat"],
                            lon = (double)rascunho["coord"]["lon"],
                            description = (string)rascunho["weather"][0]["description"],
                            main = (string)rascunho["weather"][0]["main"],
                            temp_min = (double)rascunho["main"]["temp_min"],
                            temp_max = (double)rascunho["main"]["temp_max"],
                            speed = (double)rascunho["wind"]["speed"],
                            visibility = (int)rascunho["visibility"],
                            sunrise = sunrise.ToString("HH:mm:ss"),
                            sunset = sunset.ToString("HH:mm:ss"),
                        };
                    }
                    else
                    {
                        if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            throw new Exception("Cidade não encontrada. Verifique o nome da cidade.");
                        }
                        else if (resp.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                        {
                            throw new Exception("Erro interno no servidor. Tente novamente mais tarde.");
                        }
                        else
                        {
                            throw new Exception("Erro ao obter dados da previsão. Tente novamente mais tarde.");
                        }
                    }
                }
            }
            catch (HttpRequestException)
            {
                throw new Exception("Sem conexão com a internet. Verifique sua conexão e tente novamente.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter dados: {ex.Message}");
            }

            return t;
        }
    }
}
