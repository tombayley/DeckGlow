using OpenMacroBoard.SDK;
using StreamDeckSharp;
using System;

namespace DeckGlow.Services
{
    public class StreamDeckService
    {

        private IMacroBoard Deck;

        private int LastBrightness = -1;

        public StreamDeckService()
        {
            // Keeps the device connection stream open while program is running.
            // When closing the device and it is disposed in StreamDeckSharp, the elgato logo is displayed.
            // Therefore need to either:
            //  - keep device open
            //  - open device each time the brightness needs to be set
            //  - fork the StreamDeckSharp library and remove the line setting the elgato logo when device is closed
            // TODO: fork StreamDeckSharp library
            Deck = StreamDeck.OpenDevice();
        }

        public void SetBrightness(int brightness)
        {
            brightness = Math.Clamp(brightness, 0, 100);

            // Brightness is already set at this value
            if (brightness == LastBrightness) return;

            // TODO: potentially check if device is connected, since at the moment the device is left open all the time
            //if (Deck.IsConnected)
            //{
            //}

            Deck.SetBrightness(Convert.ToByte(brightness));

            LastBrightness = brightness;
        }

    }
}
