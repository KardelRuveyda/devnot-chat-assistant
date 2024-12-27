
# Devnot Yazılım Topluluğu Chat Asistanı

Bu proje, Devnot Yazılım Topluluğu ile ilgili sorulara cevap verebilen bir chat asistanı oluşturmak için geliştirilmiştir. Asistan, kullanıcıların yazılım etkinlikleri, konuşmacılar ve diğer yazılım topluluğu aktiviteleri hakkında bilgi edinmelerini sağlar. Bu sistem, OpenAI API'si kullanarak metin verileriyle embeddingler oluşturur ve sorgulara daha doğru ve hızlı yanıtlar verir.

## Proje Bileşenleri

Bu proje, iki ana bileşenden oluşmaktadır:

1. **API Projesi**: Kullanıcı isteklerini işleyen ve AI modellerini kullanan backend hizmeti.
2. **UI Projesi**: Kullanıcıların API ile etkileşim kurmasını sağlayan, chat arayüzünü sunan frontend uygulaması.

---

## API Projesi

API, Devnot Yazılım Topluluğu Asistanı'nın backend kısmını oluşturur. OpenAI API'sini kullanarak kullanıcının sorularına yanıt verir. API şu adımlardan oluşur:

### API Özellikleri

1. **Devnot Verilerini Yükleyip Vektör Veritabanına Eklemek**: `UploadDevnotChatbotDataFileAsync` metodu, `devnot.json` dosyasındaki veriyi yükler ve OpenAI'ye yükler.
2. **Assistant Oluşturma**: `CreateAssistant` metodu, yüklenen verilerle asistanı oluşturur. Bu asistan, Devnot Yazılım Topluluğu ile ilgili sorulara yanıt verir.
3. **Thread Başlatma**: `StartdevnotChatbotInterpretationThreadAsync` metodu, kullanıcının sorusunu işlemek için bir thread başlatır.
4. **Thread Sonucunu Alma**: `GetdevnotChatbotInterpretationResultsAsync` metodu, başlatılan thread'in yanıtlarını alır.
5. **Genel İşlemi Yöneten Metot**: `InterpretDevNot` metodu, bir soruyu işleyip en iyi sonucu döndürür.

### API Sonuçları

API, aşağıdaki temel endpoint'i sunar:

- **POST /api/devnotbot/interpret**
  - Kullanıcıdan gelen bir soruyu işleyip yanıt verir.
  - **Request**: Kullanıcının sorusu (`devnotRequest.Request`).
  - **Response**: Assistant'tan alınan yanıt.

### API Kullanımı

1. **Devnot Verilerini Yükleme ve Kaydetme**:
   - `UploadDevnotChatbotDataFileAsync` metodu, `devnot.json` verisini yükler ve işlenebilir hale getirir.

2. **Soru ve Yanıt**:
   - `InterpretDevNot` metodu, kullanıcıdan gelen soruyu işleyip yanıtları döner. Bu metod, veri yükleme, asistan oluşturma, thread başlatma ve sonuçları almak için gerekli adımları içerir.

### Teknolojiler

- **.NET 9.0**: API projesi .NET 9.0 ile geliştirilmiştir.
- **OpenAI API**: OpenAI'nın GPT-4o modelini kullanarak sorulara yanıt üretilir.
- **File I/O**: Veriler dosyaya kaydedilir ve bellekte saklanır.

---

## UI Projesi

UI projesi, kullanıcıların chat asistanı ile etkileşimde bulunabileceği bir frontend sağlar. Basit ve kullanıcı dostu bir tasarıma sahiptir. Kullanıcılar sorularını yazabilir ve anlık yanıtları görebilirler.

### UI Özellikleri

- **Chat Arayüzü**: Kullanıcılar, yazılım topluluğu ile ilgili sorularını yazarak asistanla etkileşime geçebilirler.
- **Anlık Yanıtlar**: Kullanıcı, yazdığı soruya anında yanıt alır. Bu yanıtlar, backend API tarafından işlenir.
- **Yumuşak Geçişler ve Etkileşimler**: Modern bir tasarım ve kullanıcı dostu etkileşimler sağlar.

### UI Teknolojileri

- **Blazor WebAssembly**: Frontend kısmı, Blazor WebAssembly ile geliştirilmiştir. Bu sayede frontend ve backend arasındaki etkileşim hızlı ve sorunsuz olur.
- **CSS Flexbox**: Chat arayüzü tasarımı için modern CSS teknikleri kullanılmıştır.
- **JavaScript (Blazor Interop)**: UI ile backend arasında veri alışverişi sağlanır.

### UI Kullanımı

1. **Chat Başlatma**:
   - Kullanıcılar chat kutusuna sorularını yazarak asistanla etkileşime geçer.
2. **Yanıt Görüntüleme**:
   - Asistanın yanıtları anında UI'da gösterilir.

---

## Başlangıç

Projeyi çalıştırmak için aşağıdaki adımları takip edebilirsiniz.

### API Projesi

1. Projeyi indirin ve çözümü açın.
2. `DevnotChatbot.API` projesine gidin.
3. `appsettings.json` dosyasına API anahtarınızı ve `devnot.json` dosyanızın yolunu ekleyin.
4. API projesini başlatın:
   ```bash
   dotnet run
   ```

### UI Projesi

1. Projeyi indirin ve çözümü açın.
2. `DevnotChatbot.UI` projesine gidin.
3. Blazor WebAssembly için gerekli bağımlılıkları yükleyin:
   ```bash
   dotnet restore
   ```
4. UI projesini başlatın:
   ```bash
   dotnet run
   ```

UI ve API projeleri çalışmaya başladıktan sonra, API'nin endpoint'leri üzerinden asistanla etkileşime geçebilirsiniz.

---

## Katkıda Bulunma

Bu projeye katkıda bulunmak isterseniz, lütfen bir **pull request** gönderin veya bir **issue** açın. Her türlü geri bildirim ve iyileştirme önerisi memnuniyetle karşılanır.

---

## Lisans

Bu proje [MIT Lisansı](https://opensource.org/licenses/MIT) ile lisanslanmıştır.
