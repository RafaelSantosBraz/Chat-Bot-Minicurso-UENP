using System.Threading.Tasks;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Prompts;
using Microsoft.Bot.Schema;

namespace ExemploSimples
{
    public class BotSimples : IBot
    {

        /// <summary>
        /// Every Conversation turn for our Bot will call this method. In here
        /// the bot checks the Activty type to verify it's a message, bumps the 
        /// turn conversation 'Turn' count, and then echoes the users typing
        /// back to them. 
        /// </summary>
        /// <param name="context">Turn scoped context containing all the data needed
        /// for processing this conversation turn. </param>        
        public async Task OnTurn(ITurnContext context)
        {
            // O bot só irá tratar mensagens.
            if (context.Activity.Type == ActivityTypes.Message)
            {
                // Obtenho o estado da conversação.
                BotSimplesState state = context.GetConversationState<BotSimplesState>();
                TextPrompt promptNome = new TextPrompt();

                if (!state.PergunteiNome)
                {
                    state.PergunteiNome = true;
                    await promptNome.Prompt(context, "Qual o seu nome ?");
                }
                else
                {
                    TextResult nome = await promptNome.Recognize(context);

                    if (!nome.Succeeded())
                    {
                        await promptNome.Prompt(context, "Desculpe, pode repetir ?");
                    }
                    else
                    {
                        state.PergunteiNome = false;
                        await context.SendActivity($"Oi {nome.Value}, seja bem vindo ao nosso chat de teste.");

                    }
                }

            }
        }
    }
}
