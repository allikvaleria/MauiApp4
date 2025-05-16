﻿using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using MauiApp4.Database;
using MauiApp4.Models;

namespace MauiApp4.View
{
    public partial class UserTreeningudPage : ContentPage
    {
        private readonly TreeningudDatabase database;

        public UserTreeningudPage()
        {
            Title = "Minu treeningud";

            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tervisepaevik.db");
            database = new TreeningudDatabase(dbPath);

            var treeningud = database.GetTreeningud()
                .OrderByDescending(t => t.Kuupaev)
                .ToList();

            var carousel = new CarouselView
            {
                ItemsSource = treeningud,
                ItemTemplate = new DataTemplate(() =>
                {
                    var nimiLabel = new Label
                    {
                        FontSize = 22,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Center
                    };
                    nimiLabel.SetBinding(Label.TextProperty, "Treeningu_nimi");

                    var tyyppLabel = new Label();
                    tyyppLabel.SetBinding(Label.TextProperty, new Binding("Treeningu_tuup", stringFormat: "Tüüp: {0}"));

                    var kuupaevLabel = new Label();
                    kuupaevLabel.SetBinding(Label.TextProperty, new Binding("Kuupaev", stringFormat: "Kuupäev: {0:dd.MM.yyyy}"));

                    var kellaaegLabel = new Label();
                    kellaaegLabel.SetBinding(Label.TextProperty, new Binding("Kallaaeg", stringFormat: "Kellaaeg: {0}"));

                    var sammudLabel = new Label();
                    sammudLabel.SetBinding(Label.TextProperty, new Binding("Kirjeldus", stringFormat: "Kirjeldus: {0}"));

                    var kaloridLabel = new Label();
                    kaloridLabel.SetBinding(Label.TextProperty, new Binding("Kulutud_kalorid", stringFormat: "Kalorid: {0}"));

                    var image = new Image
                    {
                        HeightRequest = 200,
                        Aspect = Aspect.AspectFill
                    };
                    image.SetBinding(Image.SourceProperty, new Binding("Treeningu_foto", converter: new ByteArrayToImageSourceConverter()));

                    var juhisLabel = new Label
                    {
                        FontSize = 10,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Center,
                        Text= "Video vaatamiseks klõpsake pildil"
                    };

                    // TapGestureRecognizer to open video link
                    var tapGesture = new TapGestureRecognizer();
                    tapGesture.Tapped += async (s, e) =>
                    {
                        if (((Image)s).BindingContext is TreeningudClass treening && !string.IsNullOrWhiteSpace(treening.Link))
                        {
                            try
                            {
                                await Browser.OpenAsync(treening.Link, BrowserLaunchMode.SystemPreferred);
                            }
                            catch (Exception ex)
                            {
                                await Application.Current.MainPage.DisplayAlert("Viga", $"Linki ei saa avada: {ex.Message}", "OK");
                            }
                        }
                    };
                    image.GestureRecognizers.Add(tapGesture);

                    return new ScrollView
                    {
                        Content = new StackLayout
                        {
                            Padding = new Thickness(20),
                            Spacing = 12,
                            Children =
                            {
                                nimiLabel,
                                tyyppLabel,
                                kuupaevLabel,
                                kellaaegLabel,
                                sammudLabel,
                                kaloridLabel,
                                image,
                                juhisLabel
                            }
                        }
                    };
                })
            };

            Content = carousel;
        }
    }

    // Конвертер из byte[] в ImageSource
    public class ByteArrayToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte[] bytes && bytes.Length > 0)
            {
                return ImageSource.FromStream(() => new MemoryStream(bytes));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
