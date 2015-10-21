# LiveCodingChat
C# livecoding.tv XMPP chat connection
## LiveCodingChat
LiveCodingChat is a small library to login to https://www.livecoding.tv/ and connect to the channel chats.
Working login methods are:
  - Direct livecoding login
  - Twitch login
  - github login
Still missing:
  - Google
  - Facebook
  - WinLive
  - LinkedIn
  
## NeinTom
NeinTom is a small chat program using the LiveCodingChat library to connect to the livecoding chats.
Implementing a little GUI in Windows-Forms(due to Mono compatibilty).
Custom ChatLog-Control to render chat messages using GDI(TextRenderer.DrawString).
Features:
  - Animated smilies(editable)
  - Links
  - Scrolling
  - Formatted Text

It has still some rendering issues due to differences from Linux to Windows.
###TODO:
  - render text on baseline
  - word wrap problems
  - fix other rendering problems
  - others
  
Feel free to contribute in any way.
