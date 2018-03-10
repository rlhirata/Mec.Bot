using System.Threading.Tasks;

namespace Mec.Bot.Dialogs
{
    internal class RequestIntentLUIS : Task
    {
        private string text;

        public RequestIntentLUIS(string text)
        {
            this.text = text;
        }
    }
}