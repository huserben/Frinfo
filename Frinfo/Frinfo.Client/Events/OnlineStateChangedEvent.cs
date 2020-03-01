namespace Frinfo.Client.Events
{
   public class OnlineStateChangedEvent
   {
      public OnlineStateChangedEvent(bool isOnline)
      {
         IsOnline = isOnline;
      }

      public bool IsOnline { get; }
   }
}
