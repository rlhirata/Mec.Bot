using System;
using System.Collections.Generic;
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
        public MecBotDialog(ILuisService service) : base(service) { }

        //Não foi possível reconher a intenção
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Desculpe, mas não consegui enteder o que você disse em '{result.Query}'");
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
                    Value = "YesAjuda",
                    Title = "Sim"
                },

                new CardAction
                {
                    Text = "btnNo",
                    DisplayText = "Não",
                    Type = ActionTypes.PostBack,
                    Value = "NoAjuda",
                    Title = "Não"
                }
            };

            message.Attachments = new List<Attachment>();
            message.Attachments.Add(objHeroCard.ToAttachment());

            await context.PostAsync(message);
            
        }

        //A inteção é um sobre
        [LuisIntent("Sobre")]
        public async Task Sobre(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("**(▀̿Ĺ̯▀̿ ̿)** - Eu sou famoso **Bot Inteligentão**\nEstou aqui para te ajudar...");
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
                        msgRetorno = msgRetorno + "xiiii.... o **Rogério** é conhecido por ser 4% kkkkkk \n";
                        break;
                    case "anderson":
                        msgRetorno = msgRetorno + "eita o **Anderson** já tirou foto com a Bel Pesce!!! \n";
                        break;
                    case "wellington":
                        msgRetorno = msgRetorno + "cuidado porque o **Wellington** desta gente muito feliz hahahaha \n";
                        break;
                    case "danilo":
                        msgRetorno = msgRetorno + "vixe o **Danilo** sempre quer dois salgados.... \n";
                        break;
                }
            }

            msgRetorno = msgRetorno + ":)";
            await context.PostAsync(msgRetorno);
        }
    }
}