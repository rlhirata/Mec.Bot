using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

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

        //A inteção é um sobre
        [LuisIntent("Sobre")]
        public async Task Sobre(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("**(▀̿Ĺ̯▀̿ ̿)** - Eu sou famoso **Bot Inteligentão**\nEstou aqui para te ajudar...");

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
                        msgRetorno = msgRetorno + "xiiii.... o **Rogério** é conhecido por ser 4% kkkkkk \U0001F308 \n";
                        break;
                    case "anderson":
                        msgRetorno = msgRetorno + "\U0001F648 eita o **Anderson** já tirou foto com a Bel Pesce!!! \n";
                        break;
                    case "wellington":
                        msgRetorno = msgRetorno + "\U0001F632 cuidado porque o **Wellington** desta gente muito feliz hahahaha \n";
                        break;
                    case "danilo":
                        msgRetorno = msgRetorno + "\U0001F950 vixe o **Danilo** sempre quer dois salgados \U0001F35F \n";
                        break;
                }
            }

            msgRetorno = msgRetorno + ":)";
            await context.PostAsync(msgRetorno);

            context.Done<object>(null);
        }

        private async Task AjudaAfter(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if (message.Text == "/YesAjuda")
            {
                await context.PostAsync("\U0001F609 Legal!! Fico feliz que queira uma ajudinha, afinal fui criado justamente para você não se preocupar com isso!");
            }

            await this.SendPerguntaKMVeiculo(context);
        }

        private async Task SendPerguntaKMVeiculo(IDialogContext context)
        {
            await context.PostAsync("Bom para que eu possa lhe avisar, preciso de apenas 3 informações.\n\n\U0001F697 Com quantos quilômetros rodados está o seu veículo no momento ? ");
            context.Wait(this.PerguntaKMVeiculoAfter);
        }


        private async Task PerguntaKMVeiculoAfter(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            //tratar o retorno

            await this.SendPerguntaKMProximaTroca(context);
        }

        private async Task SendPerguntaKMProximaTroca(IDialogContext context)
        {
            await context.PostAsync("\U0001F6E3 Com base na última troca de óleo, com quantos quilômetros está prevista a próxima?");

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
            await context.PostAsync("\U0001F6E2 Quantos quilômetros seu veículo roda em média por semana?");

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
    }
}