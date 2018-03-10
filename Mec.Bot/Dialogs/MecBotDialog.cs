using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace Mec.Bot.Dialogs
{

    [Serializable]
    public class MecBotDialog : LuisDialog<object>
    {
        //public MecBotDialog(ILuisService service) : base(service) { }
        public MecBotDialog() : base(new LuisService(new LuisModelAttribute(
            ConfigurationManager.AppSettings["LuisId"],
            ConfigurationManager.AppSettings["LuisSubscriptionKey"]))) { }

        //Não foi possível reconher a intenção
        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Desculpe, mas não consegui enteder o que você disse em '{result.Query}'");

            context.Done<object>(null);
        }

        //A inteção é um sobre
        [LuisIntent("Sobre")]
        public async Task Sobre(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("**(▀̿Ĺ̯▀̿ ̿)** - Eu sou famoso **Bot Inteligentão**\n\nEstou aqui para te ajudar...");

            context.Done<object>(null);
        }

        //A inteção é um informacoes
        [LuisIntent("Informacoes")]
        public async Task Informacoes(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Segue algumas informações sobre o bot:\n\n" +
                $"Activity.ChannelId: {context.Activity.ChannelId}\n\n" +
                $"Activity.ConversationId: {context.Activity.Conversation.Id}\n\n" +
                $"Activity.Id: {context.Activity.Id}\n\n" +
                $"Activity.From.Id: {context.Activity.From.Id}\n\n" +
                $"Activity.From.Name: {context.Activity.From.Name}\n\n" +
                $"Activity.LocalTimestamp: {context.Activity.LocalTimestamp}\n\n" +
                $"Activity.ServiceUrl: {context.Activity.ServiceUrl}\n\n" +
                $"Activity.ChannelData: {context.Activity.ChannelData}");

            context.Done<object>(null);
        }

        //A inteção é um EasterEgg-Rogerio
        [LuisIntent("EasterEgg")]
        public async Task EasterEgg(IDialogContext context, LuisResult result)
        {
            var criadores = result.Entities?.Select(e => e.Entity);
            var msgRetorno = "";

            foreach (string nomeCriador in criadores)
            {
                switch (nomeCriador.ToLower())
                {
                    case "rogerio":
                    case "rogério":
                        msgRetorno = msgRetorno + "xiiii.... o **Rogério** é conhecido por ser 4% kkkkkk \U0001F308 \n\n";
                        break;
                    case "anderson":
                        msgRetorno = msgRetorno + "\U0001F648 eita o **Anderson** já tirou foto com a Bel Pesce!!! \n\n";
                        break;
                    case "wellington":
                        msgRetorno = msgRetorno + "\U0001F632 cuidado porque o **Wellington** desta gente muito feliz hahahaha \n\n";
                        break;
                    case "danilo":
                        msgRetorno = msgRetorno + "\U0001F950 vixe o **Danilo** sempre quer dois salgados \U0001F35F \n\n";
                        break;
                }
            }

            msgRetorno = msgRetorno + ":)";
            await context.PostAsync(msgRetorno);

            context.Done<object>(null);
        }

        //A inteção é um cumprimento
        [LuisIntent("Cumprimento")]
        public async Task Cumprimento(IDialogContext context, LuisResult result)
        {
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time")).TimeOfDay;
            string saudacao;

            if (now > TimeSpan.FromHours(4) && now < TimeSpan.FromHours(12)) saudacao = "Bom dia";
            else if (now >= TimeSpan.FromHours(12) && now < TimeSpan.FromHours(18)) saudacao = "Boa tarde";
            else saudacao = "Boa noite";

            var mensagem = $"Olá, {saudacao}!!! Eu sou o Mec, o assistente virtual que vai ajudá-lo a lembrar " +
                           "quando será a próxima troca de óleo do seu veículo.";

            await context.PostAsync(mensagem);
            await this.SendPerguntaAjuda(context);
        }
        
        public async Task SendPerguntaAjuda(IDialogContext context)
        { 
            var message = context.MakeMessage();
            var objHeroCard = new HeroCard();

            objHeroCard.Subtitle = "Gostaria de saber se você deseja essa ajuda?";

            objHeroCard.Buttons = new List<CardAction>
            {
                new CardAction
                {
                    Text = "btnYes",
                    DisplayText = "Sim",
                    Type = ActionTypes.PostBack,
                    Value = "/YesAjuda",
                    Title = "Sim"
                },

                new CardAction
                {
                    Text = "btnNo",
                    DisplayText = "Não",
                    Type = ActionTypes.PostBack,
                    Value = "/NoAjuda",
                    Title = "Não"
                }
            };

            message.Attachments = new List<Attachment>();
            message.Attachments.Add(objHeroCard.ToAttachment());

            await context.PostAsync(message);

            //context.Done<object>(null);
            context.Wait(this.AjudaAfter);
        }

        private async Task AjudaAfter(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if (message.Text == "/YesAjuda")
            {
                await context.PostAsync("\U0001F609 Legal!! Fico feliz que queira uma ajudinha, afinal fui criado justamente para você não se preocupar com isso!");
                await this.SendPerguntaKMVeiculo(context);
            } else if (message.Text == "/NoAjuda")
            {
                await context.PostAsync("Ok, sem problemas. Então podemos enviar uma mensagem para você quando tivermos outros serviços disponíveis ou promoções de nossos parceiros?");
                context.Done<object>(null);
            } else
            {
                await context.PostAsync("Desculpe, mas não entendi a sua resposta.");
                await this.SendPerguntaAjuda(context);
            }
        }

        private async Task SendPerguntaKMVeiculo(IDialogContext context)
        {
            await context.PostAsync("Bom para que eu possa lhe avisar, preciso de apenas 3 informações.\n\n\U0001F697 Com quantos quilômetros rodados está o seu veículo no momento ? ");
            context.Wait(this.PerguntaKMVeiculoAfter);
        }


        private async Task PerguntaKMVeiculoAfter(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            var information = new Serialization.ConversationData();

            //tratar o retorno
            if (!int.TryParse(message.Text, out int kmrodados))
            {
                //busca a intenção no LUIS
                var intentLUIS = await this.ConecatarLUIS(message.Text);
                var resultado = JsonConvert.DeserializeObject<Serialization.LUISIntent.ResultLUISIntent>(intentLUIS);

                if (resultado.TopScoringIntent.intent != "QuilometragemVeiculo")
                {
                    await context.PostAsync("Desculpe, mas não entendi a sua resposta.");
                    await this.SendPerguntaKMVeiculo(context);
                    return;
                }
                else
                {
                    kmrodados = Convert.ToInt32(resultado.entities[0].entity);
                }
            }

            information.kmrodados = kmrodados;
            await this.SendPerguntaKMProximaTroca(context);
        }

        private async Task SendPerguntaKMProximaTroca(IDialogContext context)
        {
            await context.PostAsync("\U0001F6E2 Com base na última troca de óleo, com quantos quilômetros está prevista a próxima?");

            context.Wait(this.PerguntaKMProximaTrocaAfter);
        }

        private async Task PerguntaKMProximaTrocaAfter(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            //tratar o retorno

            await this.SendPerguntaKMPorSemana(context);
        }

        private async Task SendPerguntaKMPorSemana(IDialogContext context)
        {
            await context.PostAsync("\U0001F6E3 Quantos quilômetros seu veículo roda em média por semana?");

            context.Wait(this.PerguntaKMPorSemanaAfter);
        }

        private async Task PerguntaKMPorSemanaAfter(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            //tratar o retorno

            await this.SendCalculoDiasTroca(context);
        }

        private async Task SendCalculoDiasTroca(IDialogContext context)
        {
            var strEmoji = "\U0001F60E";

            await context.PostAsync($"Opa! Então conforme o que me passou a próxima troca de óleo deve acontecer em 187 dias {strEmoji}");

            context.Done<object>(null);
        }

        private async Task<string> ConecatarLUIS(string mensagem)
        {
            var luisAppId = ConfigurationManager.AppSettings["LuisId"];
            var subscriptionKey = ConfigurationManager.AppSettings["LuisSubscriptionKey"];
            var luisURL = ConfigurationManager.AppSettings["LuisURL"];
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            queryString["q"] = mensagem;

            // These optional request parameters are set to their default values
            queryString["timezoneOffset"] = "0";
            queryString["verbose"] = "false";
            queryString["spellCheck"] = "false";
            queryString["staging"] = "false";

            var uri = luisURL + luisAppId + "?" + queryString;
            var response = await client.GetAsync(uri);

            return await response.Content.ReadAsStringAsync();
        }

    }
}