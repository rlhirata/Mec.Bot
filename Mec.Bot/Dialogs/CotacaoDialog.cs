using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Mec.Bot.Serialization;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace Mec.Bot.Dialogs
{
    [Serializable]
    //[LuisModel("2be5435f-bf47-4059-a876-493962f6bae9", "111b1b9d42ce4ea09e84eb1ae9aa4f90")]
    public class CotacaoDialog : LuisDialog<object>
    {
        public CotacaoDialog(ILuisService service) : base(service) { }

        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Desculpe, não consegui entender a frase {result.Query}");
        }

        [LuisIntent("")]
        public async Task Nothing(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Desculpe, não consegui entender a frase {result.Query}");
        }

        [LuisIntent("Sobre")]
        public async Task Sobre(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Eu sou um bot e estou sempre aprendendo, por isso tenha paciência comigo!");
        }

        [LuisIntent("Cotacao")]
        public async Task Cotação(IDialogContext context, LuisResult result)
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            var client = new System.Net.Http.HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var urllink = "http://api.promasters.net.br/cotacao/v1/valores";
            var response = await client.GetAsync(urllink);
            var JsonResult = response.Content.ReadAsStringAsync().Result;
            var js = new DataContractJsonSerializer(typeof(MoedaCotacao));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(JsonResult));
            var cotacao = (MoedaCotacao)js.ReadObject(ms);

            var msgRetorno = new StringBuilder();
            var moedas = result.Entities?.Select(e => e.Entity);

            foreach (string nomeMoeda in moedas)
            {
                switch (nomeMoeda.ToLower())
                {
                    case "dolar":
                    case "dólar":
                    case "dollar":
                        msgRetorno.AppendLine("A cotação do " + nomeMoeda + " é " + cotacao.valores.USD.valor.ToString("C") + " (" + cotacao.valores.USD.fonte + "). ");
                        break;
                    case "euro":
                        msgRetorno.AppendLine("A cotação do " + nomeMoeda + " é " + cotacao.valores.EUR.valor.ToString("C") + " (" + cotacao.valores.EUR.fonte + "). ");
                        break;
                    case "bitcoin":
                        msgRetorno.AppendLine("A cotação do " + nomeMoeda + " é " + cotacao.valores.BTC.valor.ToString("C") + " (" + cotacao.valores.BTC.fonte + "). ");
                        break;
                    case "libra":
                        msgRetorno.AppendLine("A cotação do " + nomeMoeda + " é " + cotacao.valores.GBP.valor.ToString("C") + " (" + cotacao.valores.GBP.fonte + "). ");
                        break;
                    case "peso":
                        msgRetorno.AppendLine("A cotação do " + nomeMoeda + " argentino é " + cotacao.valores.ARS.valor.ToString("C") + " (" + cotacao.valores.ARS.fonte + "). ");
                        break;
                }
            }

            await context.PostAsync($"Um minuto que farei uma cotação para as moedas {string.Join(",", moedas.ToArray())}");
            if (!msgRetorno.Equals(""))
            {
                await context.PostAsync(msgRetorno.ToString());
            }
            /////////////////////////////////////////////////////////////////////////////////////////////////////////
        }

        [LuisIntent("Cumprimento")]
        public async Task Cumprimento(IDialogContext context, LuisResult result)
        {
            var moedas = result.Entities?.Select(e => e.Entity);
            await context.PostAsync("Oi beleza e você?");
        }
    }
}