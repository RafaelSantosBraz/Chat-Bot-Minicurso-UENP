using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace ExemploBot02
{
    public class Simples : IBot
    {
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // Obtenho o estado da conversação.

                string turn = turnContext.Activity.Text;

                if (turn.ToUpper() == "BOM DIA")
                {
                    await turnContext.SendActivityAsync($"Uenp Boot: Ótimo dia");
                }
                else if (turn.ToUpper() == "BOA TARDE")
                {
                    await turnContext.SendActivityAsync($"Uenp Boot: Ótima Tarde");
                }
                else if (turn.ToUpper() == "BOA NOITE")
                {
                    await turnContext.SendActivityAsync($"Uenp Boot: Ótima Noite");
                }
                else
                {
                    await turnContext.SendActivityAsync($"Uenp Boot: Que dia hein!?!?!?");
                }
            }

        }
    }
}
