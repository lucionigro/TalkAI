using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3;

//Pasos:
//1- Conseguir una ApiKey en el sitio de OpenAI y cambiar el valor del string _apiKey (linea 18)
//2- Realize su pregunta a debatir en la variable aiTalk (linea 10)
//3- Ejecutar!

var aiTalk = new AITalk(" Ingrese aqui su tema a desarrollo ");
await aiTalk.Run(5);

public class AITalk
{
    private string _topic;
    private string _messageA;
    private string _messageB;

    private string _apiKey = " Inserte su ApiKey ";
    private OpenAIService _openAIService;
    private readonly string _continueText = "Contesta a lo siguiente de forma que parezca un debate: ";
    private readonly string _initialText = "Que tema te gustaria debatir? ";
    public string InitQuestion
    {
        get
        {
            return _initialText + _topic;
        }
    }
    public AITalk(string topic)
    {
        _topic = topic;
        _openAIService = new OpenAIService (new OpenAiOptions()
        {
            ApiKey = _apiKey
        });
    }



    public async Task Run(int limit = 10)
    {
        Console.WriteLine("Tema a desarrollar " + _topic);
        await Send(InitQuestion, (string response) =>
        {
            _messageA = response;
            Console.WriteLine("Robot A dice: " + _messageA);
            Console.WriteLine("----------------------------");
        });
        for (var i = 0; i < limit; i++)
        {
            await Send(_continueText + _messageA, (string response) =>
            {
                _messageB = response;
                Console.WriteLine("Robot B dice: " + _messageB);
                Console.WriteLine("----------------------------");
            });
            await Send(_continueText + _messageB, (string response) =>
            {
                _messageA = response;
                Console.WriteLine("Robot A dice: " + _messageA);
                Console.WriteLine("----------------------------");
            });
        }
        
    }

    private async Task Send(string message, Action<string> fnSetResponse)
    {
        var completionResult = await _openAIService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Messages = new List<ChatMessage>
            {
                ChatMessage.FromUser(message)
            },
            Model = Models.ChatGpt3_5Turbo
        });

        if (completionResult.Successful)
        {
            fnSetResponse(completionResult.Choices.First().Message.Content);
        }
        else
        {
            if (completionResult.Error == null)
            {
                throw new Exception("Error Desconocido");
            }
            Console.WriteLine($"{completionResult.Error.Code}: {completionResult.Error.Message}");
        }
    }
}
