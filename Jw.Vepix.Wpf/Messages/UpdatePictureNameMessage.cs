namespace Jw.Vepix.Wpf.Messages
{
    public class UpdatePictureNameMessage
    {
        public string NewName { get; }

        public UpdatePictureNameMessage(string newName)
        {
            NewName = newName;
        }
    }
}
