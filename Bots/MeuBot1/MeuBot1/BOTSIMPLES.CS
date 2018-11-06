using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace MeuBot1
{
    public class BOTSIMPLES : IBot
    {
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                string turn = turnContext.Activity.Text;
                if (turn.ToUpper() == "RAFAEL")
                {
                    await turnContext.SendActivityAsync($"BOOT: {turn} you're amazing! I've never seen someone like you before!");
                }
                else
                {
                    await turnContext.SendActivityAsync($"BOOT: {turn} I don't like you!");
                }
            }
        }
    }
}
