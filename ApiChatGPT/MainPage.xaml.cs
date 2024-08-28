using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

namespace ApiChatGPT
{
    public partial class MainPage : ContentPage
    {
        private OpenAIClient _chatGptClient;

        public MainPage()
        {
            InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, EventArgs e)
        {
            var openAiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            _chatGptClient = new(openAiKey);
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {

        }

        private async void OnRestaurantClicked(object sender, EventArgs e)
        {
            await GetRecommendationAsync("restaurant");
        }

        private async void OnHotelClicked(object sender, EventArgs e)
        {
            await GetRecommendationAsync("hotel");
        }

        private async void OnAttractionClicked(object sender, EventArgs e)
        {
            await GetRecommendationAsync("attraction");
        }

        private async Task GetRecommendationAsync(string recommendationType)
        {
            if (string.IsNullOrWhiteSpace(LocationEntry.Text))
            {
                await DisplayAlert("Empty location", "Please enter a location (city or postal code)", "OK");
                return;
            }

            // The model passed to GetChatClient must match an available OpenAI
            // model in your account.
            var client = _chatGptClient.GetChatClient("gpt-3.5-turbo-16k");
            string prompt = $"What is a recommended {recommendationType} near {LocationEntry.Text}";

            AsyncResultCollection<StreamingChatCompletionUpdate> updates = client.CompleteChatStreamingAsync(prompt);
            StringWriter responseWriter = new();

            await foreach (StreamingChatCompletionUpdate update in updates)
            {
                foreach (ChatMessageContentPart updatePart in update.ContentUpdate)
                {
                    responseWriter.Write(updatePart.Text);
                }
            }

            var returnMessage = responseWriter.ToString();
            SmallLabel.Text = returnMessage;
        }

    }
}
