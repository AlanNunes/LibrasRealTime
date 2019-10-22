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
        bool ended = true;
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            int startTextFromIndex = 0;
            //mediaPlayer.URL = @"C:\Users\alann\Videos\oi.mp4";
            //mediaPlayer.Ctlcontrols.next(); // activates the next button
            //WMPLib.IWMPMedia media = mediaPlayer.newMedia(@"C:\Users\alann\Videos\tudobem.mp4");
            //mediaPlayer.currentPlaylist.appendItem(media);
            //mediaPlayer.Ctlcontrols.play(); // activates the play button
            //WMPLib.IWMPMedia media2 = mediaPlayer.newMedia(@"C:\Users\alann\Videos\oi.mp4");
            //mediaPlayer.currentPlaylist.appendItem(media2);
            while (true)
            {
                await RecognizeSpeechAsync(startTextFromIndex);
            }
        }

        public async Task RecognizeSpeechAsync(int startTextFromIndex)
        {
            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key and service region (e.g., "westus").
            var config = SpeechConfig.FromSubscription("74b9da07361f4ee5b42ba0f5c432084d", "westus");
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
                    //Console.WriteLine("Recognized:::" + r.Result.Text);
                    if (r.Result.Text.Length < startTextFromIndex)
                    {
                        startTextFromIndex = 0;
                    }
                    string speechProcessed = await TextProcessing.GetTextProcessed(r.Result.Text, startTextFromIndex);
                    Console.WriteLine("Text Processed:::" + speechProcessed);
                    startTextFromIndex = r.Result.Text.Length;
                    await TryToTranslate(speechProcessed);
                };
                var result = await recognizer.RecognizeOnceAsync();

                // Checks result.
                if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    Console.WriteLine($"We recognized: {result.Text}");
                    //startTextFromIndex = 0;
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
                    string videoPath;
                    string speech = txt.Remove(txt.Length - 1);
                    string[] words = speech.Split(' ');
                    int i = 0;
                    while (true)
                    {
                        if (words.Length == 1)
                        {
                            videoPath = SQLDataBase.GetVideoPath(words[i]);
                            if (!String.IsNullOrEmpty(videoPath))
                            {
                                Enfileira(videoPath);
                            }
                            break;
                        }
                        if ((words.Length - 1) < i)
                        {
                            break;
                        }
                        if((words.Length - 1) < i + 1)
                        {
                            videoPath = null;
                        }
                        else
                        {
                            videoPath = SQLDataBase.GetVideoPath(words[i] + ' ' + words[i + 1]);
                        }
                        if (String.IsNullOrEmpty(videoPath))
                        {
                            videoPath = SQLDataBase.GetVideoPath(words[i]);
                            if (!String.IsNullOrEmpty(videoPath))
                            {
                                Enfileira(videoPath);
                            }
                            i++;
                        }
                        else
                        {
                            Enfileira(videoPath);
                            i = i + 2;
                        }
                    }
                }
            });
        }

        private void Enfileira(string videoPath)
        {
            if (videoPath != null)
            {
                Console.WriteLine(videoPath);
                fila.Enfileira(videoPath);
                if (fila.GetVideos.Count == 1)
                {
                    if (mediaPlayer.playState != WMPPlayState.wmppsPlaying && mediaPlayer.playState != WMPPlayState.wmppsTransitioning)
                    {
                        lock (fila)
                        {
                            string _fila = fila.Desenfileira();
                            Console.WriteLine("entrou: " + _fila);
                            if (!String.IsNullOrEmpty(_fila))
                            {
                                Console.WriteLine("entrou2: " + _fila);
                                try
                                {
                                    //mediaPlayer.URL = @"C:\Users\alann\Videos\" + _fila;
                                    WMPLib.IWMPMedia media = mediaPlayer.newMedia(@"C:\Users\alann\Videos\" + _fila);
                                    mediaPlayer.currentPlaylist.appendItem(media);
                                    mediaPlayer.Ctlcontrols.play(); // activates the play button
                                    //mediaPlayer.Ctlcontrols.play();
                                    //mediaPlayer.URL = fullPathOfYourFirstMedia;
                                    //mediaPlayer.Ctlcontrols.play(); // activates the play button
                                    //mediaPlayer.Ctlcontrols.next(); // activates the next button
                                    //WMPLib.IWMPMedia media = mediaPlayer.newMedia(fullPathOfYourSecondMedia);
                                    //mediaPlayer.currentPlaylist.appendItem(media);
                                }
                                catch (Exception e)
                                {

                                }
                            }
                        }
                    }
                }
            }
        }

        private void mediaPlayer_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (e.newState == 8)//Media finished
            {
                ended = true;
            }

            if (e.newState != 8)//Stopped & ended
            {
                if (fila.GetVideos.Count > 0)
                {
                    lock (fila)
                    {
                        //mediaPlayer.Ctlcontrols.stop();
                        //mediaPlayer.URL = @"C:\Users\alann\Videos\" + fila.Desenfileira();
                        //mediaPlayer.Ctlcontrols.play();
                        WMPLib.IWMPMedia media = mediaPlayer.newMedia(@"C:\Users\alann\Videos\" + fila.Desenfileira());
                        mediaPlayer.currentPlaylist.appendItem(media);
                        mediaPlayer.Ctlcontrols.play(); // activates the play button
                    }
                }
                ended = false;
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
    }
}