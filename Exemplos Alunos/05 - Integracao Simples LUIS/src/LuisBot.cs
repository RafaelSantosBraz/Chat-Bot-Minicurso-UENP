using System.Threading.Tasks;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Ai.LUIS;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Schema;

namespace IntegracaoSimples
{
    public class LuisBot : IBot
    {
        /// <summary>
        /// Every Conversation turn for our EchoBot will call this method. In here
        /// the bot checks the Activty type to verify it's a message, bumps the 
        /// turn conversation 'Turn' count, and then echoes the users typing
        /// back to them. 
        /// </summary>
        /// <param name="context">Turn scoped context containing all the data needed
        /// for processing this conversation turn. </param>        
        public async Task OnTurn(ITurnContext context)
        {
            // This bot is only handling Messages
            if (context.Activity.Type == ActivityTypes.Message)
            {
                    
                RecognizerResult resultado = context.Services.Get<RecognizerResult>(LuisRecognizerMiddleware.LuisRecognizerResultKey);

                var intencaoMaisPontuada = resultado?.GetTopScoringIntent();

                switch ((intencaoMaisPontuada != null) ? intencaoMaisPontuada.Value.intent : null)
                {
                    case "None":
                        await context.SendActivity("Não entendi.");
                        break;
                    case "PedirPizza":
                        await context.SendActivity("Você quer pedir uma pizza.");
                        break;
                    case "PrevisaoTempo":
                        await context.SendActivity("Você quer saber a previsão do tempo.");
                        break;
                    case "MarcarConsulta":
                        await context.SendActivity("Você quer marcar uma consulta.");
                        break;
                }
            }
        }
    }
}
