using EchoBot1.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBot1.Bots
{
    public class WelcomeUserBot : ActivityHandler
    {
        private readonly string WelcomeMessage = "Chao ban, toi co the giup gi cho ban";
        private readonly string InfoMessage = "Ban can giup do gi khong";
        private readonly string PatternMessage = "A, B, C, D";

        private BotState _userState;
        private BotState _conversationState;

        private IStatePropertyAccessor<UserProfile> _userStateAccessors;
        private IStatePropertyAccessor<ConversationLog> _conversationStateAccessors;

        public WelcomeUserBot(UserState userState)
        {
            _userState = userState;
        }

        public WelcomeUserBot(UserState userState, ConversationState conversationState)
        {
            _userState = userState;
            _conversationState = conversationState;

            //Base on data type and name of data type, [CreateProperty] provides accessors for loading corressponding data from storage
            _userStateAccessors = _userState.CreateProperty<UserProfile>(nameof(UserProfile));
            _conversationStateAccessors = _conversationState.CreateProperty<ConversationLog>(nameof(ConversationLog));
        }

        //This method is called while members added/connected with a bot
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded,
            ITurnContext<IConversationUpdateActivity> turnContext,
            CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync("Chao " + member.Name, cancellationToken: cancellationToken);
                    await turnContext.SendActivityAsync(InfoMessage, cancellationToken: cancellationToken);
                    //await turnContext.SendActivityAsync(PatternMessage, cancellationToken: cancellationToken);

                }
            }
        }

        private async Task SendIntroCardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var card = new HeroCard();
            //card.Title = "Xin chao!";
            //card.Text = "";
            card.Images = new List<CardImage>()
            {
                new CardImage("https://aka.ms/bf-welcome-card-image")
            };

            card.Buttons = new List<CardAction>()
            {
                new CardAction(ActionTypes.OpenUrl, "title", null, "text", "displayText", "https://www.samarpaninfotech.com/"),
                new CardAction(ActionTypes.OpenUrl, "Contact us", null, "Ask a question", "Ask a question", "https://www.samarpaninfotech.com/contact-us/"),
                new CardAction(ActionTypes.OpenUrl, "Learn more", null, "Learn how to deploy", "Learn how to deploy", "https://www.samarpaninfotech.com/blog/"),
            };

            var response = MessageFactory.Attachment(card.ToAttachment());
            await turnContext.SendActivityAsync(response, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // Get the state data form the turnContext
            var conversationLog = await _conversationStateAccessors.GetAsync(turnContext, () => new ConversationLog());
            var userProfile = await _userStateAccessors.GetAsync(turnContext, () => new UserProfile());

            if (string.IsNullOrEmpty(userProfile.Name))
            {
                if (conversationLog.DidAskTheUserName)
                {
                    userProfile.Name = turnContext.Activity.Text?.Trim();

                    // Acknowledge that got the user name
                    await turnContext.SendActivityAsync("Uh" + $". {userProfile.Name} co van de gi can tu van phai khong nhi!");
                }
                else
                {
                    // asking the name of user
                    await turnContext.SendActivityAsync("Chao ban, minh co the biet ten cua ban khong");
                    conversationLog.DidAskTheUserName = true;
                }
            }
            else
            {
                var messageTimeOffset = (DateTimeOffset)turnContext.Activity.Timestamp;
                var localMsgTime = messageTimeOffset.ToLocalTime();

                conversationLog.Timestamp = localMsgTime.ToString();
                conversationLog.ChannelId = turnContext.Activity.ChannelId.ToString();

                await turnContext.SendActivityAsync($"{userProfile.Name} sent: {turnContext.Activity.Text}");
                await turnContext.SendActivityAsync($"Message received at: {conversationLog.Timestamp}");
                await turnContext.SendActivityAsync($"Message received from: {conversationLog.ChannelId}");

                // trivial preprocessing
                var msgfromUser = turnContext.Activity.Text.ToLower();
                var responseMsg = string.Empty;
                if (Int32.TryParse(msgfromUser, out int number))
                {
                    responseMsg = (number++).ToString();
                }
                else {
                    switch (msgfromUser)
                    {
                        case "xin chao":
                        case "hi":
                        case "hello":
                        case ".":
                            responseMsg = $"Chao {userProfile.Name}!";
                            await turnContext.SendActivityAsync(responseMsg, cancellationToken: cancellationToken);
                            break;
                        default:
                            responseMsg = $"{userProfile.Name} noi gi minh khong hieu. Co phai ban can giai dap nhung thong tin ben duoi";
                            await turnContext.SendActivityAsync(responseMsg, cancellationToken: cancellationToken);
                            await SendIntroCardAsync(turnContext, cancellationToken);
                            break;
                    }
                }
            }

            //Save any state change
            await _userState.SaveChangesAsync(turnContext);
            await _conversationState.SaveChangesAsync(turnContext);
        }
    }
}
