using Microsoft.CognitiveServices.Speech;
using PIC.Classes;
using PIC.DataBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;

namespace PIC2019
{
    public partial class Form1 : Form
    {
        Fila fila = new Fila();
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            int startTextFromIndex = 0;
            while (true)
            {
                await RecognizeSpeechAsync(startTextFromIndex);
            }
        }

        public async Task RecognizeSpeechAsync(int startTextFromIndex)
        {
            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key and service region (e.g., "westus").
            var config = SpeechConfig.FromSubscription("199b2e472a8c416da895333a283b6577", "westus");
            config.SpeechRecognitionLanguage = "pt-BR";
            config.SetProfanity(ProfanityOption.Raw);

            // Creates a speech recognizer.
            using (var recognizer = new SpeechRecognizer(config))
            {
                Console.WriteLine("Say something...");

                // Starts speech recognition, and returns after a single utterance is recognized. The end of a
                // single utterance is determined by listening for silence at the end or until a maximum of 15
                // seconds of audio is processed.  The task returns the recognition text as result. 
                // Note: Since RecognizeOnceAsync() returns only a single utterance, it is suitable only for single
                // shot recognition like command or query. 
                // For long-running multi-utterance recognition, use StartContinuousRecognitionAsync() instead.
                recognizer.Recognized += async (e, r) =>
                {
                    if (r.Result.Text.Length < startTextFromIndex)
                    {
                        startTextFromIndex = 0;
                    }
                    string speechProcessed = await TextProcessing.GetTextProcessed(r.Result.Text, startTextFromIndex);
                    await Task.Run(() =>
                    {
                        startTextFromIndex = r.Result.Text.Length;
                    });
                    await TryToTranslate(speechProcessed);
                };
                var result = await recognizer.RecognizeOnceAsync();

                // Checks result.
                if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    Console.WriteLine($"We recognized: {result.Text}");
                    startTextFromIndex = 0;
                    mediaPlayer.URL = @"C:\Users\alann\Videos\bomdia.mp4";
                    mediaPlayer.Ctlcontrols.play();
                }
                else if (result.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(result);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                        Console.WriteLine($"CANCELED: Did you update the subscription info?");
                    }
                }
            }
        }

        private async Task TryToTranslate(string txt)
        {
            await Task.Run(() =>
            {
                if (!String.IsNullOrEmpty(txt))
                {
                    string speech = txt.Remove(txt.Length - 1);
                    string videoPath = SQLDataBase.GetVideoPath(speech);
                    if (videoPath != null)
                    {
                        Console.WriteLine(videoPath);
                        fila.Enfileira(videoPath);
                        if (fila.GetVideos.Count == 1)
                        {
                            mediaPlayer.Ctlcontrols.stop();
                            mediaPlayer.URL = @"C:\Users\alann\Videos\" + fila.Desenfileira();
                            mediaPlayer.Ctlcontrols.play();
                        }
                    }
                }
            });
        }

        private void mediaPlayer_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (mediaPlayer.playState == WMPLib.WMPPlayState.wmppsStopped)
            {
                if (fila.GetVideos.Count > 0)
                {
                    mediaPlayer.Ctlcontrols.stop();
                    mediaPlayer.URL = @"C:\Users\alann\Videos\" + fila.Desenfileira();
                    mediaPlayer.Ctlcontrols.play();
                }
            }
        }

        private void mediaPlayer_MediaError(object sender, AxWMPLib._WMPOCXEvents_MediaErrorEvent e)
        {
            try
            // If the Player encounters a corrupt or missing file, 
            // show the hexadecimal error code and URL.
            {
                IWMPMedia2 errSource = e.pMediaObject as IWMPMedia2;
                IWMPErrorItem errorItem = errSource.Error;
                MessageBox.Show(errorItem.errorDescription + " - " + errSource.sourceURL);
            }
            catch (InvalidCastException)
            // In case pMediaObject is not an IWMPMedia item.
            {
                MessageBox.Show("Error.");
            }

        }

        //private void mediaPlayer_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        //{
        //    if (mediaPlayer.playState == WMPLib.WMPPlayState.wmppsStopped)
        //    {
        //        if (fila.GetVideos.Count > 0)
        //        {
        //            mediaPlayer.Ctlcontrols.stop();
        //            mediaPlayer.URL = @"C:\Users\alann\Videos\" + fila.Desenfileira();
        //            mediaPlayer.Ctlcontrols.play();
        //        }
        //    }
        //}
    }
}