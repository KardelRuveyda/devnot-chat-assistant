#pragma warning disable OPENAI001

using DevnotChatbot.API.Models.Constants;
using DevnotChatbot.API.Services;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Assistants;
using OpenAI.Files;
using System.ClientModel;
using System.IO;
using System.Runtime;
using System.Text.RegularExpressions;

namespace DevnotChatbot.API.Services
{
    public class DevnotChatbotService : IDevnotChatbotService
    {
        private readonly OpenAIClient _openAIClient;

        public DevnotChatbotService(IOptions<OpenAIConstants> options)
        {
            _openAIClient = new OpenAIClient(options.Value.ApiKey);
        }

        // 1. Adım: Devnot verilerini yükleyip vektör veritabanına eklemek
        private async Task<OpenAIFile> UploadDevnotChatbotDataFileAsync(string filePath)
        {
            using Stream document = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            OpenAIFile devnotChatbotFile = await _openAIClient.GetOpenAIFileClient().UploadFileAsync(
                document,
                "devnot.json",
                FileUploadPurpose.Assistants);

            return devnotChatbotFile;
        }

        // 2. Adım: Assistant oluşturma
        private Assistant CreateAssistant(string devnotChatbotFileId)
        {
            AssistantClient assistantClient = _openAIClient.GetAssistantClient();

            AssistantCreationOptions assistantOptions = new()
            {
                Name = "Devnot Yazılım Topluluğu Asistanı",
                Instructions = @"
                Sen, Devnot Yazılım Topluluğu ile ilgili yanıt veren bir yardımcısın.
                Kullanıcılarla konuşurken dost canlısı, samimi ve teknik bir yazılım bilgisiyle konuşarak sorularına yanıt bul.
                Yanıtlarında bol bol emojiler kullan ve kullanıcıya olumlu bir deneyim yaşatmaya çalış. Sen onların can dostusun! 😊

                Yanıt verirken:
                - Sana verilen dokümandaki tarih bilgilerine dikkat et. 
                  - Eğer bir etkinlik bugünün tarihindeyse, 'bugün gerçekleşiyor' diye belirt.
                  - Eğer etkinlik gelecekteyse, 'gelecekte olacak' gibi bir dil kullan.
                  - Eğer etkinlik geçmişteyse, 'geçmişte gerçekleşti' diyerek yanıt ver.
                - Sorulara sana verilen dokümandaki bilgileri ayrıntılı bir şekilde kullanarak cevap ver. 
                  - Örneğin, bir konuşmacıdan bahsediliyorsa, konuşmacının uzmanlık alanını ve etkinlikteki rolünü açıkla.
                  - Teknik bir oturum varsa, konunun yazılım dünyasındaki önemini vurgula.
                - Sana verilen bilgiler dışında bir konu hakkında soru sorulursa, yalnızca Devnot Yazılım Topluluğu ile ilgili yanıt verebileceğini belirt ve soruya yanıt verme. 
                  - Örneğin: 'Bu konuda yardımcı olamıyorum, ama Devnot Yazılım Topluluğu hakkında başka bir sorunuz varsa seve seve yardımcı olurum! 😊'

                Örnek yanıtlar:
                - Gelecek bir etkinlik için: '28 Aralık'ta gerçekleşecek olan .NET 9 Day etkinliği, .NET 9'un yeniliklerini ele alacak. 🚀 Etkinlikte Kardel Rüveyda Çetin gibi uzman konuşmacılar yer alacak.'
                - Bugün bir etkinlik için: 'Bugün Devnot Yazılım Topluluğu'nda .NET 9 Day etkinliği düzenleniyor! 🎉 Yazılım dünyasındaki en yeni gelişmeleri öğrenmek için harika bir fırsat!'
                - Geçmiş bir etkinlik için: 'Geçtiğimiz hafta gerçekleşen .NET 9 Day etkinliği, yazılım meraklılarının büyük ilgisini çekti. 📅 Umarım bir sonraki etkinlikte seni de görebiliriz! 😊'

                Sen kullanıcıların Devnot Yazılım Topluluğu hakkında bilgi almak için güvendiği bir rehbersin. Sorularına en doğru ve ayrıntılı şekilde yanıt vermeye özen göster!",
                Tools =
                {
                    new FileSearchToolDefinition(),
                },
                ToolResources = new()
                {
                    FileSearch = new()
                    {
                        NewVectorStores =
                        {
                            new VectorStoreCreationHelper([devnotChatbotFileId]),
                        }
                    }
                }
            };

            Assistant assistant = assistantClient.CreateAssistant("gpt-4o", assistantOptions);
            return assistant;
        }

        // 3. Adım: Thread başlatma
        private async Task<ThreadRun> StartdevnotChatbotInterpretationThreadAsync(Assistant assistant, string userdevnotChatbot)
        {
            AssistantClient assistantClient = _openAIClient.GetAssistantClient();

            ThreadCreationOptions threadOptions = new()
            {
                InitialMessages = { userdevnotChatbot }
            };

            ThreadRun threadRun = assistantClient.CreateThreadAndRun(assistant.Id, threadOptions);
            return threadRun;
        }

        // 4. Adım: Thread'in tamamlanmasını beklemek ve sonucu almak
        private async Task<List<string>> GetdevnotChatbotInterpretationResultsAsync(ThreadRun threadRun)
        {
            AssistantClient assistantClient = _openAIClient.GetAssistantClient();

            while (!threadRun.Status.IsTerminal)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                threadRun = await assistantClient.GetRunAsync(threadRun.ThreadId, threadRun.Id);
            }

            CollectionResult<ThreadMessage> messages = assistantClient.GetMessages(threadRun.ThreadId, new MessageCollectionOptions() { Order = MessageCollectionOrder.Ascending });

            var formattedMessages = messages
                 .SelectMany(message => message.Content.Select(content =>
                 {
                     // İstenmeyen ifadeyi temizleyin
                     string clearText = Regex.Replace(content.Text, @"【.*?†.*?】", "");
                     return $"<p>{clearText.Replace("\n", "</p><p>")}</p>";
                 }))
                 .ToList();


            return formattedMessages;
        }

        // Genel işlemi yöneten metot
        public async Task<List<string>> InterpretDevNot(string userdevnotChatbot, string apiKey, string filePath)
        {
            // 1. Devnot verilerini yükle
            var devnotChatbotFile = await UploadDevnotChatbotDataFileAsync(filePath);

            // 2. Assistant oluştur
            var assistant = CreateAssistant(devnotChatbotFile.Id);

            // 3. Thread başlat
            var threadRun = await StartdevnotChatbotInterpretationThreadAsync(assistant, userdevnotChatbot);

            // 4. Sonuçları al
            return await GetdevnotChatbotInterpretationResultsAsync(threadRun);
        }
    }
}

#pragma warning restore OPENAI001
