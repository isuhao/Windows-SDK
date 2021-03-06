﻿using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
// Using VungleSDK namespace
using VungleSDK;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Animation;

namespace CS_sample
{
    public sealed partial class MainPage : Page
    {
        VungleAd sdkInstance;

        //private string appID = "59792a4f057243276200298a";
        //private string placement1 = "DEFAULT18154";
        //private string placement2 = "PLACEME92007";
        //private string placement3 = "REWARDP93292";

        private string appID = "598a531697c455bc70001f98";
        private string placement1 = "DEFAULT59086";
        private string placement2 = "NONREWA96669";
        private string placement3 = "REWARDE30999";

        public MainPage()
        {
            InitializeComponent();

            appIDTextBlock.Text = "AppID: " + appID;
            placement1IDTextBlock.Text = "PlacementID: " + placement1;
            placement2IDTextBlock.Text = "PlacementID: " + placement2;
            placement3IDTextBlock.Text = "PlacementID: " + placement3;
        }

        //Event handler for OnInitComleted event
        private async void SdkInstance_OnInitCompleted(object sender, ConfigEventArgs e)
        {
            var placementsInfo = "OnInitCompleted: " + e.Initialized;
            if (e.Initialized == true)
            {
                for (var i = 0; i < e.Placements.Length; i++)
                {
                    placementsInfo += "\n\tPlacement" + (i + 1) + ": " + e.Placements[i].ReferenceId;
                    if (e.Placements[i].IsAutoCached == true)
                        placementsInfo += " (Auto-Cached)";
                }
            }
            else
            {
                placementsInfo += "\n\t" + e.ErrorMessage;
            }
            System.Diagnostics.Debug.WriteLine(placementsInfo);
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() => ChangeLoadButtonsState(e.Initialized)));
        }

        // Event handler called when e.AdPlayable is changed
        private async void SdkInstance_OnAdPlayableChanged(object sender, AdPlayableEventArgs e)
        {
            // e.AdPlayable - true if an ad is available to play, false otherwise
            // e.Placement  - placement ID in string
            System.Diagnostics.Debug.WriteLine("OnAdPlayable: " + e.Placement + " - " + e.AdPlayable);
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() => ChangePlayButtonState(e.AdPlayable, e.Placement)));
        }

        // Event Handler called before playing an ad
        private void SdkInstance_OnAdStart(object sender, AdEventArgs e)
        {
            // e.Id        - Vungle app ID in string
            // e.Placement - placement ID in string

            System.Diagnostics.Debug.WriteLine("OnAdStart(" + e.Id + "): " + e.Placement);
        }

        // DEPRECATED - use SdkInstance_OnAdEnd() instead
        private void SdkInstance_OnVideoView(object sender, AdViewEventArgs e) { }

        // Event handler called when the user leaves ad and control is return to the hosting app
        private void SdkInstance_OnAdEnd(object sender, AdEndEventArgs e)
        {
            // e.Id                  - Vungle app ID in string
            // e.Placement           - placement ID in string
            // e.IsCompletedView     - true when 80% or more of the video was watched
            // e.CallToActionClicked - true when the user has clicked download button on end card
            // e.WatchedDuration     - duration of video watched
            // e.VideoDuration       - DEPRECATED

            System.Diagnostics.Debug.WriteLine("OnVideoEnd(" + e.Id + "): " +
               "\n\tPlacement: " + e.Placement +
               "\n\tIsCompletedView: " + e.IsCompletedView +
               "\n\tCallToActionClicked: " + e.CallToActionClicked +
               "\n\tWatchedDuration: " + e.WatchedDuration);
        }

        // Event handler called when SDK wants to print diagnostic logs
        private void SdkInstance_Diagnostic(object sender, DiagnosticLogEvent e)
        {
            System.Diagnostics.Debug.WriteLine(e.Level + " " + e.Type + " " + e.Exception + " " + e.Message);
        }

        private void InitSDK_Click(Object sender, RoutedEventArgs e)
        {
            string[] placementList = new string[] { placement1, placement2, placement3 };

            //Obtain Vungle SDK instance
            sdkInstance = AdFactory.GetInstance(appID, placementList);

            //Register event handlers
            sdkInstance.OnAdPlayableChanged += SdkInstance_OnAdPlayableChanged;
            sdkInstance.OnAdStart += SdkInstance_OnAdStart;
            sdkInstance.OnAdEnd += SdkInstance_OnAdEnd;
            sdkInstance.Diagnostic += SdkInstance_Diagnostic;
            sdkInstance.OnInitCompleted += SdkInstance_OnInitCompleted;

            InitSDK.IsEnabled = false;
        }

        private void LoadPlacement2_Click(Object sender, RoutedEventArgs e)
        {
            //Load ad for placement2
            sdkInstance.LoadAd(placement2);
        }

        private void LoadPlacement3_Click(Object sender, RoutedEventArgs e)
        {
            //Load ad for placement3
            sdkInstance.LoadAd(placement3);
        }

        private async void PlayPlacement1_Click(Object sender, RoutedEventArgs e)
        {
            //Play ad for placement1
            await sdkInstance.PlayAdAsync(new AdConfig(), placement1);
        }

        //plays in native container
        private async void PlayPlacement2_Click(Object sender, RoutedEventArgs e)
        {
            //Play ad for placement2
            embeddedControl.AppID = appID;
            embeddedControl.Placements = placement1 + "," + placement2 + "," + placement3;
            embeddedControl.Placement = placement2;
            embeddedControl.SoundEnabled = false;

            embeddedControl.OnAdStart += Embedded_OnAdStart;
            embeddedControl.OnAdEnd += Embedded_OnAdEnd;

            var nEmb = await embeddedControl.PlayAdAsync();
        }

        private async void PlayPlacement3_Click(Object sender, RoutedEventArgs e)
        {
            //Play ad for placement3
            AdConfig adConfig = new AdConfig();

            adConfig.IncentivizedDialogBody = null;
            adConfig.IncentivizedDialogCloseButton = "";
            adConfig.IncentivizedDialogContinueButton = "";
            adConfig.IncentivizedDialogTitle = "";
            adConfig.UserId = "VTest";

            await sdkInstance.PlayAdAsync(adConfig, placement3);
        }

        private void ChangeLoadButtonsState(bool isInitialized)
        {
            LoadPlacement2.IsEnabled = isInitialized;
            LoadPlacement3.IsEnabled = isInitialized;
        }

        private void ChangePlayButtonState(bool adPlayable, string placement)
        {
            if (placement.Equals(placement1))
            {
                PlayPlacement1.IsEnabled = adPlayable;
            }
            else if (placement.Equals(placement2))
            {
                PlayPlacement2.IsEnabled = adPlayable;
            }
            else if (placement.Equals(placement3))
            {
                PlayPlacement3.IsEnabled = adPlayable;
            }
        }

        private void AnimateHeight(double value)
        {
            var anim = new DoubleAnimation()
            {
                From = embeddedControl.Height,
                To = value,
                Duration = TimeSpan.FromMilliseconds(500),
                EnableDependentAnimation = true
            };
            Storyboard.SetTarget(anim, embeddedControl);
            Storyboard.SetTargetProperty(anim, "Height");
            var sb = new Storyboard();
            sb.Children.Add(anim);
            sb.Begin();
        }

        // Event Handler called before playing an ad
        private void Embedded_OnAdStart(object sender, AdEventArgs e)
        {
            var nowait = Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                    AnimateHeight(200);
            });
        }

        // Event handler called when the user leaves ad and control is return to the hosting app
        private void Embedded_OnAdEnd(object sender, AdEndEventArgs e)
        {
            var nowait = Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                    AnimateHeight(1);
            });
        }
    }
}
