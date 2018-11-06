using System.Threading.Tasks;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Prompts;
using System;
using System.Linq;
using Microsoft.Recognizers.Text;
using System.Collections.Generic;
using static Microsoft.Bot.Builder.Prompts.DateTimeResult;
using Microsoft.Bot.Builder.Ai.LUIS;
using Newtonsoft.Json.Linq;

namespace ExemploDialogo
{
    public class Jarvis : IBot
    {
        DialogSet dialogos;       

        public Jarvis()
        {
            dialogos = new DialogSet();

            // Dialogo pra previsao do tempo
            dialogos.Add("previsaoTempo", new WaterfallStep[] {
                async (dc, args, next) =>
                {
                    IDictionary<string, object> estadoDialogo = dc.ActiveDialog.State as IDictionary<string, object>;

                    object aux = null;

                    if (args.TryGetValue("Cidade", out aux))
                    {
                        estadoDialogo.Add("Cidade", aux.ToString());
                        dc.ActiveDialog.State = estadoDialogo;
                        await dc.Continue();
                    }
                    else
                    {
                        await dc.Prompt("capturaTexto",$"Qual a cidade ?");
                    }
                },
                async (dc, args, next) =>
                {
                    IDictionary<string, object> estadoDialogo = dc.ActiveDialog.State as IDictionary<string, object>;

                    if (!estadoDialogo.ContainsKey("Cidade"))
                    {
                        estadoDialogo.Add("Cidade", ((TextResult)args).Value);
                    }
                    
                    await dc.Context.SendActivity($"A previsão do tempo no {estadoDialogo["Cidade"]} é de tempo bom.");
                    await dc.End();
                }
            });

            // Dialogo pra marcar consulta
            dialogos.Add("marcarConsulta", new WaterfallStep[] {
                async (dc, args, next) =>
                {
                   await dc.Prompt("capturaTexto","Qual o seu nome ?");
                },
                async (dc, args, next) =>
                {
                    await dc.Prompt("capturaTexto",$"Qual o convênio ?");
                },
                async (dc, args, next) =>
                {
                    await dc.Prompt("capturaDataHora","Certo... Qual o dia e horário ?");
                },
                async (dc, args, next) =>
                {
                    DateTimeResolution resultado = ((DateTimeResult)args).Resolution.First();
                    var dataHora = Convert.ToDateTime(resultado.Value);

                    await dc.Context.SendActivity($"Está marcado. Dia {dataHora.ToString("dd/MM/yyyy HH:mm:ss")}");
                    await dc.End();
                },
            });

            // Pedir pizza 
            dialogos.Add("pedirPizza", new WaterfallStep[] {               
                async (dc, args, next) =>
                {
                    IDictionary<string, object> estadoDialogo = dc.ActiveDialog.State as IDictionary<string, object>;

                    object aux = null;

                    if (args.TryGetValue("Sabor", out aux))
                    {
                        estadoDialogo.Add("Sabor", aux.ToString());
                    }

                    if (args.TryGetValue("Tamanho", out aux))
                    {
                        estadoDialogo.Add("Tamanho", aux.ToString());
                    }

                    dc.ActiveDialog.State = estadoDialogo;
                    if (!estadoDialogo.ContainsKey("Sabor"))
                    {
                       await dc.Prompt("capturaTexto",$"Qual o sabor da pizza ?");
                    }
                    else
                    {
                        await dc.Continue();
                    }
                },
                async (dc, args, next) =>
                {
                    IDictionary<string, object> estadoDialogo = dc.ActiveDialog.State as IDictionary<string, object>;

                    if (!estadoDialogo.ContainsKey("Sabor"))
                    {
                        estadoDialogo.Add("Sabor", ((TextResult)args).Value);
                        dc.ActiveDialog.State = estadoDialogo;
                    }

                    if (!estadoDialogo.ContainsKey("Tamanho"))
                    {
                        await dc.Prompt("capturaTexto","Qual o tamanho ?");
                    }
                    else
                    {
                        await dc.Continue();
                    }
                },
                async (dc, args, next) =>
                {
                    IDictionary<string, object> estadoDialogo = dc.ActiveDialog.State as IDictionary<string, object>;

                    if (!estadoDialogo.ContainsKey("Tamanho"))
                    {
                        estadoDialogo.Add("Tamanho", ((TextResult)args).Value);
                        dc.ActiveDialog.State = estadoDialogo;
                    }

                    await dc.Prompt("capturaTexto","Qual o endereço de entrega ?");
                },                
                async (dc, args, next) =>
                {
                    IDictionary<string, object> estadoDialogo = dc.ActiveDialog.State as IDictionary<string, object>;

                    await dc.Prompt("capturaTexto",$"Ok, sua pizza {estadoDialogo["Tamanho"]} de {estadoDialogo["Sabor"]} será entregue por volta de 40 minutos.");
                    await dc.End();
                }
            });

            dialogos.Add("capturaDataHora", new Microsoft.Bot.Builder.Dialogs.DateTimePrompt(Culture.Portuguese));
            dialogos.Add("capturaTexto", new Microsoft.Bot.Builder.Dialogs.TextPrompt());
        }

        public async Task OnTurn(ITurnContext context)
        {

            Dictionary<string, object> state = ConversationState<Dictionary<string, object>>.Get(context);
            DialogContext dc = dialogos.CreateContext(context, state);
            await dc.Continue();

            if (context.Activity.Type == ActivityTypes.Message)
            {
                RecognizerResult resultado = context.Services.Get<RecognizerResult>(LuisRecognizerMiddleware.LuisRecognizerResultKey);

                var intencaoMaisPontuada = resultado?.GetTopScoringIntent();
                if (!context.Responded)
                {
                    IDictionary<string, object> argumentos = new Dictionary<string, object>();

                    if (this.ObterEntidade<string>(resultado, "Cidade") != null)                    
                        argumentos.Add("Cidade", this.ObterEntidade<string>(resultado, "Cidade"));

                    if (this.ObterEntidade<string>(resultado, "Tamanho") != null)
                        argumentos.Add("Tamanho", this.ObterEntidade<string>(resultado, "Tamanho"));

                    if (this.ObterEntidade<string>(resultado, "Sabor") != null)
                        argumentos.Add("Sabor", this.ObterEntidade<string>(resultado, "Sabor"));

                    switch ((intencaoMaisPontuada != null) ? intencaoMaisPontuada.Value.intent : null)
                    {
                        case "None":
                            await context.SendActivity("Não entendi.");
                            break;
                        case "PedirPizza":
                            await dc.Begin("pedirPizza", argumentos);
                            break;
                        case "PrevisaoTempo":
                            await dc.Begin("previsaoTempo", argumentos);
                            break;
                        case "MarcarConsulta":
                            await dc.Begin("marcarConsulta");
                            break;
                    }
                }
            }
        }

        private T ObterEntidade<T>(RecognizerResult luisResult, string entityKey)
        {
            var data = luisResult.Entities as IDictionary<string, JToken>;
            if (data.TryGetValue(entityKey, out JToken value))
            {
                return value.First.Value<T>();
            }
            return default(T);
        }
    }
}
