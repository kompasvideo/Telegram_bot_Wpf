using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using System.IO;
using Path = System.IO.Path;
using System.Collections.ObjectModel;

namespace Telegram_bot_Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static TelegramBotClient bot;
        static string path;
        static int poz;
        static List<TypeMessage> d_audioPhoto;
        static Dictionary<int, TypeMessage> d_files;
        static string token;
        static int photo = 0;
        static int audio = 0;
        static int voice = 0;
        static int video = 0;
        static ObservableCollection<string> listMessages { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        void Init()
        {
            token = Token.token;
            path = Directory.GetCurrentDirectory();
            d_files = new Dictionary<int, TypeMessage>();
            d_audioPhoto = new List<TypeMessage>();
            poz = 0;
            bot = new TelegramBotClient(token);
            bot.OnMessage += MessageListener;
            bot.OnMessageEdited += MessageListener;
            bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            bot.OnReceiveError += BotOnReceiveError;
            bot.StartReceiving(Array.Empty<UpdateType>());
            listMessages = new ObservableCollection<string>();
            lbMess.ItemsSource = listMessages;
            //Console.ReadLine();            
            //bot.StopReceiving();
        }

        /// <summary>
        /// Обрабатывает все сообщения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static async void MessageListener(object sender, MessageEventArgs e)
        {
            var message = e.Message;
            SaveToList(e);

            if (message == null || message.Type != MessageType.Text)
                return;

            var messageText = e.Message.Text;
            switch (messageText)
            {
                case null:
                    break;
                case "/start":
                    messageText = "Вас приветствует бот 'test20210404_bot'\n" +
                        "Вы можете управлять мной, отправляя эти команды: \n" +
                        "/list - просмотреть список загруженных файлов\n" +
                        "/load_n - скачать выбранный файл, где n - число\n" +
                        "/help - справка по поддерживаемым коммандам\n";
                    await bot.SendTextMessageAsync(e.Message.Chat.Id,
                        $"{messageText}");
                    break;

                case "/help":
                    messageText = "Вы можете управлять мной, отправляя эти команды: \n" +
                        "/list - просмотреть список загруженных файлов\n" +
                        "/load_n - скачать выбранный файл, где n - число\n" +
                        "/help - справка по поддерживаемым коммандам\n";
                    await bot.SendTextMessageAsync(e.Message.Chat.Id,
                        $"{messageText}");
                    break;

                case "/list":
                    messageText = GetFiles(SetDirectory(e.Message.From.Id));
                    if (messageText != "")
                        await bot.SendTextMessageAsync(e.Message.Chat.Id, $"{messageText}");
                    else
                        await bot.SendTextMessageAsync(e.Message.Chat.Id, $"Список сейчас пустой");
                    break;
                default:
                    MessageDefault(e);
                    break;
            }
        }

        /// <summary>
        /// Устанавливает текущую директория и возвращяет её
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Возвращяет текую директорию типа string</returns>
        static string SetDirectory(int userId)
        {
            string l_path = path + "\\" + userId.ToString();
            if (!Directory.Exists(l_path))
            {
                Directory.CreateDirectory(l_path);
            }
            Directory.SetCurrentDirectory(l_path);
            return l_path;
        }

        /// <summary>
        /// Сохраняет фото и аудио в список, документы в папку
        /// </summary>
        /// <param name="e"></param>
        private static void SaveToList(MessageEventArgs e)
        {
            string text = $"{DateTime.Now.ToLongTimeString()}: {e.Message.Chat.FirstName} {e.Message.Chat.Id} {e.Message.Text}";
            SetDirectory(e.Message.From.Id);
            //Console.WriteLine($"{text} TypeMessage: {e.Message.Type}");
            Dispatcher.BeginInvoke(delegate
            {
                тут я свободно меняю ListBox
                });
            listMessages.Add($"{text} TypeMessage: {e.Message.Type}");
            TypeMessage fileName = new TypeMessage();
            switch (e.Message.Type)
            {
                case MessageType.Document:
                    //Console.WriteLine(e.Message.Document.FileId);
                    //Console.WriteLine(e.Message.Document.FileName);
                    //Console.WriteLine(e.Message.Document.FileSize);
                    DownLoad(e.Message.Document.FileId, e.Message.Document.FileName);
                    break;

                case MessageType.Photo:
                    fileName.type = Type.Photo;
                    PhotoSize[] photoSizes = e.Message.Photo;
                    fileName.fileName = photoSizes[0].FileId;
                    d_audioPhoto.Add(fileName);
                    //Console.WriteLine(photoSizes[0].FileId);
                    DownLoad(photoSizes[0].FileId, "photo_" + photo.ToString());
                    photo++;
                    break;

                case MessageType.Audio:
                    fileName.type = Type.Audio;
                    fileName.fileName = e.Message.Audio.FileId;
                    d_audioPhoto.Add(fileName);
                    //Console.WriteLine(e.Message.Audio.FileId);
                    DownLoad(e.Message.Audio.FileId, "audio_" + audio.ToString());
                    audio++;
                    break;
                case MessageType.Voice:
                    fileName.type = Type.Voice;
                    fileName.fileName = e.Message.Voice.FileId;
                    d_audioPhoto.Add(fileName);
                    //Console.WriteLine(e.Message.Voice.FileId);
                    DownLoad(e.Message.Voice.FileId, "voice_" + voice.ToString());
                    voice++;
                    break;
                case MessageType.Video:
                    fileName.type = Type.Video;
                    fileName.fileName = e.Message.Video.FileId;
                    d_audioPhoto.Add(fileName);
                    //Console.WriteLine(e.Message.Video.FileId);
                    DownLoad(e.Message.Video.FileId, "video_" + video.ToString());
                    video++;
                    break;
            }
        }

        /// <summary>
        /// Обработка сообщений по умолчания (всё кроме команд /start /help /list)
        /// </summary>
        /// <param name="e"></param>
        private static async void MessageDefault(MessageEventArgs e)
        {
            string str_load = e.Message.Text.Substring(0, 1);
            if (str_load == "/")
            {
                if (e.Message.Text.Length >= 6)
                {
                    int i = e.Message.Text.CompareTo("/load_");
                    if (i >= 0)
                    {
                        string[] subString = e.Message.Text.Split('_');
                        string str_nomer = subString[1];
                        int nomer;
                        TypeMessage name = null;
                        if (int.TryParse(str_nomer, out nomer))
                        {
                            try
                            {
                                name = d_files[nomer];
                                switch (name.type)
                                {
                                    case Type.Document:
                                        await SendDocument(e.Message, name.fileName);
                                        break;
                                    case Type.Audio:
                                        await SendAudioMy(e.Message, name.fileName);
                                        //await bot.SendAudioAsync(e.Message.Chat.Id, name.fileName);
                                        break;
                                    case Type.Photo:
                                        await SendPhoto(e.Message, name.fileName);
                                        //await bot.SendPhotoAsync(e.Message.Chat.Id, name.fileName);
                                        break;
                                    case Type.Voice:
                                        await SendVoice(e.Message, name.fileName);
                                        //await bot.SendVoiceAsync(e.Message.Chat.Id, name.fileName);
                                        break;
                                    case Type.Video:
                                        await SendVideo(e.Message, name.fileName);
                                        //await bot.SendVideoAsync(e.Message.Chat.Id, name.fileName);
                                        break;
                                }
                            }
                            catch (ArgumentException)
                            {
                                //Console.WriteLine("Ошибка ArgumentException");
                                MessageBox.Show("Ошибка ArgumentException");
                            }
                            catch (KeyNotFoundException)
                            {
                                //Console.WriteLine("Ошибка KeyNotFoundException");
                                await bot.SendTextMessageAsync(e.Message.Chat.Id,
                                    "Нет документа с таким номером");
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Сохраняет документ
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="path"></param>
        static async void DownLoad(string fileId, string path)
        {
            var file = await bot.GetFileAsync(fileId);
            FileStream fs = new FileStream(path, FileMode.Create);
            await bot.DownloadFileAsync(file.FilePath, fs);
            fs.Close();
            fs.Dispose();
        }

        /// <summary>
        /// Создаёт список документов, аудио, фото и т.д
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Возвращяет список документов, аудио, фото и т.д типа string</returns>
        static string GetFiles(string path)
        {
            poz = 0;
            string str_return = "";
            d_files.Clear();
            string[] files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                TypeMessage fileName = new TypeMessage();
                fileName.fileName = Path.GetFileName(file);
                string[] words = fileName.fileName.Split('_');
                switch (words[0])
                {
                    case "photo":
                        fileName.type = Type.Photo;
                        d_files.Add(poz, fileName);
                        str_return += String.Format($"{poz} - (photo) {fileName.fileName}\n");
                        break;
                    case "audio":
                        fileName.type = Type.Audio;
                        d_files.Add(poz, fileName);
                        str_return += String.Format($"{poz} - (audio) {fileName.fileName}\n");
                        break;
                    case "voice":
                        fileName.type = Type.Voice;
                        d_files.Add(poz, fileName);
                        str_return += String.Format($"{poz} - (voice) {fileName.fileName}\n");
                        break;
                    case "video":
                        fileName.type = Type.Video;
                        d_files.Add(poz, fileName);
                        str_return += String.Format($"{poz} - (video) {fileName.fileName}\n");
                        break;
                    default:
                        fileName.type = Type.Document;
                        d_files.Add(poz, fileName);
                        str_return += String.Format($"{poz} - (document) {fileName.fileName}\n");
                        break;
                }
                poz++;
            }
            return str_return;
        }

        /// <summary>
        /// Отправляет документ
        /// </summary>
        /// <param name="message"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        static async Task SendDocument(Message message, string filePath)
        {
            await bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();
            await bot.SendDocumentAsync(
                chatId: message.Chat.Id,
                document: new InputOnlineFile(fileStream, fileName),
                caption: filePath
            );
        }
        /// <summary>
        /// Отправляет фото
        /// </summary>
        /// <param name="message"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        static async Task SendPhoto(Message message, string filePath)
        {
            await bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();
            await bot.SendPhotoAsync(
                chatId: message.Chat.Id,
                photo: new InputOnlineFile(fileStream, fileName),
                caption: filePath
            );
        }
        /// <summary>
        /// Отправляет аудио
        /// </summary>
        /// <param name="message"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        static async Task SendAudioMy(Message message, string filePath)
        {
            await bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();
            await bot.SendAudioAsync(
                chatId: message.Chat.Id,
                audio: new InputOnlineFile(fileStream, fileName),
                caption: filePath
            );
        }
        /// <summary>
        /// Отправляет запись
        /// </summary>
        /// <param name="message"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        static async Task SendVoice(Message message, string filePath)
        {
            await bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();
            await bot.SendVoiceAsync(
                chatId: message.Chat.Id,
                voice: new InputOnlineFile(fileStream, fileName),
                caption: filePath
            );
        }
        /// <summary>
        /// Отправляет видео
        /// </summary>
        /// <param name="message"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        static async Task SendVideo(Message message, string filePath)
        {
            await bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();
            await bot.SendVideoAsync(
                chatId: message.Chat.Id,
                video: new InputOnlineFile(fileStream, fileName),
                caption: filePath
            );
        }

        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            var callbackQuery = callbackQueryEventArgs.CallbackQuery;

            await bot.AnswerCallbackQueryAsync(
                callbackQueryId: callbackQuery.Id,
                text: $"Received {callbackQuery.Data}"
            );

            await bot.SendTextMessageAsync(
                chatId: callbackQuery.Message.Chat.Id,
                text: $"Received {callbackQuery.Data}"
            );
        }

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            //Console.WriteLine("Received error: {0} — {1}",
            //    receiveErrorEventArgs.ApiRequestException.ErrorCode,
            //    receiveErrorEventArgs.ApiRequestException.Message
            //);
            string str = string.Format("Received error: {0} — {1}",
                receiveErrorEventArgs.ApiRequestException.ErrorCode,
                receiveErrorEventArgs.ApiRequestException.Message
            );
            MessageBox.Show(str);
        }    
        private void SaveInJson_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            bot.StopReceiving();
        }
    }
}
