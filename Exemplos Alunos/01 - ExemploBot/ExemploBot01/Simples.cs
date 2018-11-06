using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace ExemploBot01
{
    public class Simples : IBot
    {
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                string turn = turnContext.Activity.Text;

                await turnContext.SendActivityAsync($"Uenp Boot: {turn}");
            }
        }
    }
}
